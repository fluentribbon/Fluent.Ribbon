namespace Fluent.Internal
{
    using System;
    using System.Linq;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Fluent.Metro.Native;
    using ContextMenu = Fluent.ContextMenu;
    using MenuItem = Fluent.MenuItem;

    internal class WindowChrome
    {
        #region Constants

        private const int SwpFlags = NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOOWNERZORDER | NativeMethods.SWP_NOACTIVATE;

        /// <summary>
        /// Matrix of the hit test values to return when responding to NC window messages.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        private static readonly int[,] hitTestBorders =
        {
            { NativeMethods.HTTOPLEFT,    NativeMethods.HTTOP,     NativeMethods.HTTOPRIGHT    },
            { NativeMethods.HTLEFT,       NativeMethods.HTCLIENT,  NativeMethods.HTRIGHT       },
            { NativeMethods.HTBOTTOMLEFT, NativeMethods.HTBOTTOM,  NativeMethods.HTBOTTOMRIGHT }
        };

        #endregion

        private readonly RibbonWindow window;

        // Window native data
        private IntPtr handle;
        private HwndSource hwndSource;
        private bool isHooked;
        private IntPtr mouseHook;
        private NativeMethods.HookProc mouseProc;

        // These fields are for tracking workarounds for WPF 3.5SP1 behaviors.
        private bool isFixedUp;
        private bool isUserResizing;
        private bool hasUserMovedWindow;
        private Point windowPosAtStartOfUserMove = default(Point);

        // Field to track attempts to force off Device Bitmaps on Win7.
        private int blackGlassFixupAttemptCount;

        // Keep track of this so we can detect when we need to apply changes.  Tracking these separately
        // as I've seen using just one cause things to get enough out of sync that occasionally the caption will redraw.
        private WindowState lastRoundingState;
        private WindowState lastMenuState;
        private Grid mainGrid;
        private FrameworkElement iconImage;

        public WindowChrome(RibbonWindow window)
        {
            this.window = window;
            this.window.SourceInitialized += this.HandleWindowSourceInitialized;
            this.window.Closed += this.OnWindowClosed;
        }

        private void HandleWindowSourceInitialized(object sender, EventArgs e)
        {
            this.handle = new WindowInteropHelper(this.window).Handle;
            this.hwndSource = HwndSource.FromHwnd(this.handle);

            if (!this.window.DontUseDwm)
            {
                this.window.IsDwmEnabled = NativeMethods.IsDwmEnabled();
            }

            if (DesignerProperties.GetIsInDesignMode(this.window) == false)
            {
                this.ApplyCustomChrome();
            }            
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            NativeMethods.UnhookWindowsHookEx(mouseHook);
        }

        #region Private Methods

        private void ApplyCustomChrome()
        {
            if (!isHooked)
            {
                hwndSource.AddHook(this.WndProc);
                mouseProc = this.MouseWndProc;
#pragma warning disable 618
                mouseHook = NativeMethods.SetWindowsHookEx(NativeMethods.HookType.WH_MOUSE, mouseProc, IntPtr.Zero, AppDomain.GetCurrentThreadId());
                isHooked = true;
            }

            FixFrameworkIssues();

            // Force this the first time.
            UpdateSystemMenu(window.WindowState);
            UpdateFrameState(true);

            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);
        }

        public void FixFrameworkIssues()
        {
            if (DesignerProperties.GetIsInDesignMode(window))
            {
                return;
            }

            // This margin is only necessary if the client rectangle is going to be calculated incorrectly by WPF version less then 4.0
            if (FrameworkHelper.PresentationFrameworkVersion >= new Version("4.0"))
            {
                return;
            }

            if (window.Template == null)
            {
                // Nothing to fixup yet.  This will get called again when a template does get set.
                return;
            }

            // Guard against the visual tree being empty.
            if (VisualTreeHelper.GetChildrenCount(window) == 0)
            {
                // The template isn't null, but we don't have a visual tree.
                // Hope that ApplyTemplate is in the queue and repost this, because there's not much we can do right now.
                window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)FixFrameworkIssues);
                return;
            }

            var rootElement = (FrameworkElement)VisualTreeHelper.GetChild(window, 0);

            NativeMethods.Rect rcWindow = new NativeMethods.Rect();
            NativeMethods.GetWindowRect(handle, ref rcWindow);
            NativeMethods.Rect rcAdjustedClient = GetAdjustedWindowRect(rcWindow);

            Rect rcLogicalWindow = DpiHelper.DeviceRectToLogical(new Rect(rcWindow.Left, rcWindow.Top, rcWindow.Right - rcWindow.Left, rcWindow.Bottom - rcWindow.Top));
            Rect rcLogicalClient = DpiHelper.DeviceRectToLogical(new Rect(rcAdjustedClient.Left, rcAdjustedClient.Top, rcAdjustedClient.Right - rcAdjustedClient.Left, rcAdjustedClient.Bottom - rcAdjustedClient.Top));

            Thickness nonClientThickness = new Thickness(
               rcLogicalWindow.Left - rcLogicalClient.Left,
               rcLogicalWindow.Top - rcLogicalClient.Top,
               rcLogicalClient.Right - rcLogicalWindow.Right,
               rcLogicalClient.Bottom - rcLogicalWindow.Bottom);

            rootElement.Margin = new Thickness(
                0,
                0,
                -(nonClientThickness.Left + nonClientThickness.Right),
                -(nonClientThickness.Top + nonClientThickness.Bottom));

            // The negative thickness on the margin doesn't properly get applied in RTL layouts.
            // The width is right, but there is a black bar on the right.
            // To fix this we just add an additional RenderTransform to the root element.
            // This works fine, but if the window is dynamically changing its FlowDirection then this can have really bizarre side effects.
            // This will mostly work if the FlowDirection is dynamically changed, but there aren't many real scenarios that would call for
            // that so I'm not addressing the rest of the quirkiness.
            if (window.FlowDirection == FlowDirection.RightToLeft)
            {
                rootElement.RenderTransform = new MatrixTransform(1, 0, 0, 1, -(nonClientThickness.Left + nonClientThickness.Right), 0);
            }
            else
            {
                rootElement.RenderTransform = null;
            }

            if (!isFixedUp)
            {
                hasUserMovedWindow = false;
                window.StateChanged += this.FixRestoreBounds;

                isFixedUp = true;
            }
        }

        // There was a regression in DWM in Windows 7 with regard to handling WM_NCCALCSIZE to effect custom chrome.
        // When windows with glass are maximized on a multimonitor setup the glass frame tends to turn black.
        // Also when windows are resized they tend to flicker black, sometimes staying that way until resized again.
        //
        // This appears to be a bug in DWM related to device bitmap optimizations.  At least on RTM Win7 we can
        // evoke a legacy code path that bypasses the bug by calling an esoteric DWM function.  This doesn't affect
        // the system, just the application.
        // WPF also tends to call this function anyways during animations, so we're just forcing the issue
        // consistently and a bit earlier.
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void FixWindows7Issues()
        {
            if (blackGlassFixupAttemptCount > 5)
            {
                // Don't keep trying if there's an endemic problem with this.
                return;
            }

            if (window.IsDwmEnabled)
            {
                ++blackGlassFixupAttemptCount;

                bool success = false;
                try
                {
                    NativeMethods.DWM_TIMING_INFO? dti = NativeMethods.DwmGetCompositionTimingInfo(handle);
                    success = dti != null;
                }
                catch (Exception)
                {
                    // We aren't sure of all the reasons this could fail.
                    // If we find new ones we should consider making the NativeMethod swallow them as well.
                    // Since we have a limited number of retries and this method isn't actually critical, just repost.

                    // Disabling this for the published code to reduce debug noise.  This will get compiled away for retail binaries anyways.
                    //Assert.Fail(e.Message);
                }

                // NativeMethods.DwmGetCompositionTimingInfo swallows E_PENDING.
                // If the call wasn't successful, try again later.
                if (!success)
                {
                    window.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (ThreadStart)FixWindows7Issues);
                }
                else
                {
                    // Reset this.  We will want to force this again if DWM composition changes.
                    blackGlassFixupAttemptCount = 0;
                }
            }
        }

        private void FixRestoreBounds(object sender, EventArgs e)
        {
            if (window.WindowState == WindowState.Maximized
                || window.WindowState == WindowState.Minimized)
            {
                // Old versions of WPF sometimes force their incorrect idea of the Window's location
                // on the Win32 restore bounds.  If we have reason to think this is the case, then
                // try to undo what WPF did after it has done its thing.
                if (hasUserMovedWindow)
                {
                    hasUserMovedWindow = false;
                    NativeMethods.WINDOWPLACEMENT wp = new NativeMethods.WINDOWPLACEMENT();
                    NativeMethods.GetWindowPlacement(handle, wp);

                    NativeMethods.Rect adjustedDeviceRc = GetAdjustedWindowRect(new NativeMethods.Rect { Bottom = 100, Right = 100 });
                    Point adjustedTopLeft = DpiHelper.DevicePixelsToLogical(
                        new Point(
                            wp.rcNormalPosition.Left - adjustedDeviceRc.Left,
                            wp.rcNormalPosition.Top - adjustedDeviceRc.Top));

                    window.Top = adjustedTopLeft.Y;
                    window.Left = adjustedTopLeft.X;
                }
            }
        }

        private NativeMethods.Rect GetAdjustedWindowRect(NativeMethods.Rect rcWindow)
        {
            var style = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_STYLE);
            var exstyle = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_EXSTYLE);

            NativeMethods.AdjustWindowRectEx(ref rcWindow, (int)style, false, (int)exstyle);
            return rcWindow;
        }

        /// <summary>Add and remove a native WindowStyle from the HWND.</summary>
        /// <param name="removeStyle">The styles to be removed.  These can be bitwise combined.</param>
        /// <param name="addStyle">The styles to be added.  These can be bitwise combined.</param>
        /// <returns>Whether the styles of the HWND were modified as a result of this call.</returns>
        private bool ModifyStyle(int removeStyle, int addStyle)
        {
            var dwStyle = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_STYLE).ToInt32();
            var dwNewStyle = (dwStyle & ~removeStyle) | addStyle;
            if (dwStyle == dwNewStyle)
            {
                return false;
            }

            NativeMethods.SetWindowLongPtr(handle, NativeMethods.GWL_STYLE, new IntPtr(dwNewStyle));
            return true;
        }

        private int HitTestNonClientArea(Rect windowPosition, Point mousePosition)
        {
            // Determine if hit test is for resizing, default middle (1,1).
            int uRow = 1;
            int uCol = 1;
            bool onResizeBorder = false;

            // Determine if the point is at the top or bottom of the window.
            if (mousePosition.Y >= windowPosition.Top && mousePosition.Y < windowPosition.Top + window.ResizeBorderThickness.Top + window.CaptionHeight)
            {
                onResizeBorder = (mousePosition.Y < (windowPosition.Top + window.ResizeBorderThickness.Top));
                uRow = 0; // top (caption or resize border)
            }
            else if (mousePosition.Y < windowPosition.Bottom && mousePosition.Y >= windowPosition.Bottom - (int)window.ResizeBorderThickness.Bottom)
            {
                uRow = 2; // bottom
            }

            // Determine if the point is at the left or right of the window.
            if (mousePosition.X >= windowPosition.Left && mousePosition.X < windowPosition.Left + (int)window.ResizeBorderThickness.Left)
            {
                uCol = 0; // left side
            }
            else if (mousePosition.X < windowPosition.Right && mousePosition.X >= windowPosition.Right - window.ResizeBorderThickness.Right)
            {
                uCol = 2; // right side
            }

            // If the cursor is in one of the top edges by the caption bar, but below the top resize border,
            // then resize left-right rather than diagonally.
            if (uRow == 0 && uCol != 1 && !onResizeBorder)
            {
                uRow = 1;
            }

            int ht = hitTestBorders[uRow, uCol];

            if (ht == NativeMethods.HTTOP && !onResizeBorder)
            {
                ht = NativeMethods.HTCAPTION;
            }

            return ht;
        }

        /// <summary>Display the system menu at a specified location.</summary>
        /// <param name="screenLocation">The location to display the system menu, in logical screen coordinates.</param>
        public void ShowSystemMenu(Point screenLocation)
        {
            ShowSystemMenuPhysicalCoordinates(screenLocation);
        }

        private void ShowSystemMenuPhysicalCoordinates(Point physicalScreenLocation)
        {
            const uint TPM_RETURNCMD = 0x0100;
            const uint TPM_LEFTBUTTON = 0x0;

            if (handle == IntPtr.Zero)
            {
                return;
            }

            IntPtr hmenu = NativeMethods.GetSystemMenu(handle, false);

            uint cmd = NativeMethods.TrackPopupMenuEx(hmenu, TPM_LEFTBUTTON | TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, handle, IntPtr.Zero);
            if (0 != cmd)
            {
                NativeMethods.PostMessage(handle, NativeMethods.WM_SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        /// <summary>
        /// Get the WindowState as the native HWND knows it to be.  This isn't necessarily the same as what Window thinks.
        /// </summary>
        private WindowState GetHwndState()
        {
            NativeMethods.WINDOWPLACEMENT wpl = new NativeMethods.WINDOWPLACEMENT();
            NativeMethods.GetWindowPlacement(handle, wpl);
            switch (wpl.showCmd)
            {
                case NativeMethods.SW_SHOWMINIMIZED: return WindowState.Minimized;
                case NativeMethods.SW_SHOWMAXIMIZED: return WindowState.Maximized;
            }
            return WindowState.Normal;
        }

        // Check flag set
        private static bool IsFlagSet(int value, int mask)
        {
            return 0 != (value & mask);
        }

        /// <summary>
        /// Update the items in the system menu based on the current, or assumed, WindowState.
        /// </summary>
        /// <param name="assumeState">
        /// The state to assume that the Window is in.  This can be null to query the Window's state.
        /// </param>
        /// <remarks>
        /// We want to update the menu while we have some control over whether the caption will be repainted.
        /// </remarks>
        private void UpdateSystemMenu(WindowState? assumeState)
        {
            const uint mfEnabled = NativeMethods.MF_ENABLED | NativeMethods.MF_BYCOMMAND;
            const uint mfDisabled = NativeMethods.MF_GRAYED | NativeMethods.MF_DISABLED | NativeMethods.MF_BYCOMMAND;

            WindowState state = assumeState ?? GetHwndState();

            if (null != assumeState || lastMenuState != state)
            {
                lastMenuState = state;

                bool modified = ModifyStyle(NativeMethods.WS_VISIBLE, 0);
                IntPtr hmenu = NativeMethods.GetSystemMenu(handle, false);
                if (IntPtr.Zero != hmenu)
                {
                    var dwStyle = NativeMethods.GetWindowLongPtr(handle, NativeMethods.GWL_STYLE).ToInt32();

                    bool canMinimize = IsFlagSet((int)dwStyle, (int)NativeMethods.WS_MINIMIZEBOX);
                    bool canMaximize = IsFlagSet((int)dwStyle, (int)NativeMethods.WS_MAXIMIZEBOX);
                    bool canSize = IsFlagSet((int)dwStyle, (int)NativeMethods.WS_THICKFRAME);

                    switch (state)
                    {
                        case WindowState.Maximized:
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_RESTORE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MOVE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_SIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MINIMIZE, canMinimize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MAXIMIZE, mfDisabled);
                            break;
                        case WindowState.Minimized:
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_RESTORE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MOVE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_SIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MINIMIZE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MAXIMIZE, canMaximize ? mfEnabled : mfDisabled);
                            break;
                        default:
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_RESTORE, mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MOVE, mfEnabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_SIZE, canSize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MINIMIZE, canMinimize ? mfEnabled : mfDisabled);
                            NativeMethods.EnableMenuItem(hmenu, NativeMethods.SC_MAXIMIZE, canMaximize ? mfEnabled : mfDisabled);
                            break;
                    }
                }

                if (modified)
                {
                    ModifyStyle(0, NativeMethods.WS_VISIBLE);
                }
            }
        }

        public void UpdateFrameState(bool force)
        {
            if (IntPtr.Zero == handle)
            {
                return;
            }

            if (!window.IsDwmEnabled)
            {
                SetRoundingRegion(null);
            }
            else
            {
                ClearRoundingRegion();
                ExtendGlassFrame();

                FixWindows7Issues();
            }

            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SwpFlags);
            window.UpdateLayout();
        }

        private void ClearRoundingRegion()
        {
            NativeMethods.SetWindowRgn(handle, IntPtr.Zero, NativeMethods.IsWindowVisible(handle));
        }

        private void SetRoundingRegion(NativeMethods.WINDOWPOS? wp)
        {
            const int MONITOR_DEFAULTTONEAREST = 0x00000002;

            // We're early - WPF hasn't necessarily updated the state of the window.
            // Need to query it ourselves.
            NativeMethods.WINDOWPLACEMENT wpl = new NativeMethods.WINDOWPLACEMENT();
            NativeMethods.GetWindowPlacement(handle, wpl);

            if (wpl.showCmd == NativeMethods.SW_SHOWMAXIMIZED && window.IsDwmEnabled == true)
            {
                int left;
                int top;

                if (wp.HasValue)
                {
                    left = wp.Value.x;
                    top = wp.Value.y;
                }
                else
                {
                    NativeMethods.Rect r = new NativeMethods.Rect();
                    NativeMethods.GetWindowRect(handle, ref r);
                    left = r.Left;
                    top = r.Top;
                }

                IntPtr hMon = NativeMethods.MonitorFromWindow(handle, MONITOR_DEFAULTTONEAREST);

                NativeMethods.MonitorInfo mi = new NativeMethods.MonitorInfo();
                NativeMethods.GetMonitorInfo(hMon, mi);
                NativeMethods.Rect rcMax = mi.Work;
                // The location of maximized window takes into account the border that Windows was
                // going to remove, so we also need to consider it.
                rcMax.Left -= left; rcMax.Right -= left;
                rcMax.Top -= top; rcMax.Bottom -= top;

                IntPtr hrgn = IntPtr.Zero;
                try
                {
                    hrgn = NativeMethods.CreateRectRgnIndirect(ref rcMax);
                    NativeMethods.SetWindowRgn(handle, hrgn, NativeMethods.IsWindowVisible(handle));
                    hrgn = IntPtr.Zero;
                }
                finally
                {
                    NativeMethods.DeleteObject(hrgn);
                }
            }
            else
            {
                Size windowSize;

                // Use the size if it's specified.
                if (null != wp && !IsFlagSet(wp.Value.flags, NativeMethods.SWP_NOSIZE))
                {
                    windowSize = new Size(wp.Value.cx, wp.Value.cy);
                }
                else if (null != wp && (lastRoundingState == window.WindowState))
                {
                    return;
                }
                else
                {
                    NativeMethods.Rect r = new NativeMethods.Rect();
                    NativeMethods.GetWindowRect(handle, ref r);
                    Rect rect = new Rect(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
                    windowSize = rect.Size;
                }

                lastRoundingState = window.WindowState;

                IntPtr hrgn = IntPtr.Zero;
                try
                {
                    double shortestDimension = Math.Min(windowSize.Width, windowSize.Height);

                    double topLeftRadius = DpiHelper.LogicalPixelsToDevice(new Point(window.CornerRadius.TopLeft, 0)).X;
                    topLeftRadius = Math.Min(topLeftRadius, shortestDimension / 2);

                    if (IsUniform(window.CornerRadius))
                    {
                        // RoundedRect HRGNs require an additional pixel of padding.
                        hrgn = CreateRoundRectRgn(new Rect(windowSize), topLeftRadius);
                    }
                    else
                    {
                        // We need to combine HRGNs for each of the corners.
                        // Create one for each quadrant, but let it overlap into the two adjacent ones
                        // by the radius amount to ensure that there aren't corners etched into the middle
                        // of the window.
                        hrgn = CreateRoundRectRgn(new Rect(0, 0, windowSize.Width / 2 + topLeftRadius, windowSize.Height / 2 + topLeftRadius), topLeftRadius);

                        double topRightRadius = DpiHelper.LogicalPixelsToDevice(new Point(window.CornerRadius.TopRight, 0)).X;
                        topRightRadius = Math.Min(topRightRadius, shortestDimension / 2);
                        Rect topRightRegionRect = new Rect(0, 0, windowSize.Width / 2 + topRightRadius, windowSize.Height / 2 + topRightRadius);
                        topRightRegionRect.Offset(windowSize.Width / 2 - topRightRadius, 0);

                        CreateAndCombineRoundRectRgn(hrgn, topRightRegionRect, topRightRadius);

                        double bottomLeftRadius = /*DpiHelper.LogicalPixelsToDevice*/(new Point(window.CornerRadius.BottomLeft, 0)).X;
                        bottomLeftRadius = Math.Min(bottomLeftRadius, shortestDimension / 2);
                        Rect bottomLeftRegionRect = new Rect(0, 0, windowSize.Width / 2 + bottomLeftRadius, windowSize.Height / 2 + bottomLeftRadius);
                        bottomLeftRegionRect.Offset(0, windowSize.Height / 2 - bottomLeftRadius);

                        CreateAndCombineRoundRectRgn(hrgn, bottomLeftRegionRect, bottomLeftRadius);

                        double bottomRightRadius = DpiHelper.LogicalPixelsToDevice(new Point(window.CornerRadius.BottomRight, 0)).X;
                        bottomRightRadius = Math.Min(bottomRightRadius, shortestDimension / 2);
                        Rect bottomRightRegionRect = new Rect(0, 0, windowSize.Width / 2 + bottomRightRadius, windowSize.Height / 2 + bottomRightRadius);
                        bottomRightRegionRect.Offset(windowSize.Width / 2 - bottomRightRadius, windowSize.Height / 2 - bottomRightRadius);

                        CreateAndCombineRoundRectRgn(hrgn, bottomRightRegionRect, bottomRightRadius);
                    }

                    NativeMethods.SetWindowRgn(handle, hrgn, NativeMethods.IsWindowVisible(handle));
                    hrgn = IntPtr.Zero;
                }
                finally
                {
                    // Free the memory associated with the HRGN if it wasn't assigned to the HWND.
                    NativeMethods.DeleteObject(hrgn);
                }
            }
        }

        private static IntPtr CreateRoundRectRgn(Rect region, double radius)
        {
            // Round outwards.

            if (DoubleUtil.AreClose(0, radius))
            {
                return NativeMethods.CreateRectRgn(
                    (int)Math.Floor(region.Left),
                    (int)Math.Floor(region.Top),
                    (int)Math.Ceiling(region.Right),
                    (int)Math.Ceiling(region.Bottom));
            }

            // RoundedRect HRGNs require an additional pixel of padding on the bottom right to look correct.
            return NativeMethods.CreateRoundRectRgn(
                (int)Math.Floor(region.Left),
                (int)Math.Floor(region.Top),
                (int)Math.Ceiling(region.Right) + 1,
                (int)Math.Ceiling(region.Bottom) + 1,
                (int)Math.Ceiling(radius),
                (int)Math.Ceiling(radius));
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HRGNs")]
        private static void CreateAndCombineRoundRectRgn(IntPtr hrgnSource, Rect region, double radius)
        {
            IntPtr hrgn = IntPtr.Zero;
            try
            {
                hrgn = CreateRoundRectRgn(region, radius);
                int result = NativeMethods.CombineRgn(hrgnSource, hrgnSource, hrgn, NativeMethods.RGN_OR);
                if (result == 0)
                {
                    throw new InvalidOperationException("Unable to combine two HRGNs.");
                }
            }
            catch
            {
                NativeMethods.DeleteObject(hrgn);
                throw;
            }
        }

        private static bool IsUniform(CornerRadius cornerRadius)
        {
            if (!DoubleUtil.AreClose(cornerRadius.BottomLeft, cornerRadius.BottomRight))
            {
                return false;
            }

            if (!DoubleUtil.AreClose(cornerRadius.TopLeft, cornerRadius.TopRight))
            {
                return false;
            }

            if (!DoubleUtil.AreClose(cornerRadius.BottomLeft, cornerRadius.TopRight))
            {
                return false;
            }

            return true;
        }

        private void ExtendGlassFrame()
        {
            if (IntPtr.Zero == handle)
            {
                // Can't do anything with this call until the Window has been shown.
                return;
            }

            // Ensure standard HWND background painting when DWM isn't enabled.
            if (!window.IsDwmEnabled)
            {
                hwndSource.CompositionTarget.BackgroundColor = SystemColors.WindowColor;
            }
            else
            {
                // This makes the glass visible at a Win32 level so long as nothing else is covering it.
                // The Window's Background needs to be changed independent of this.

                // Apply the transparent background to the HWND
                hwndSource.CompositionTarget.BackgroundColor = Colors.Transparent;

                // Thickness is going to be DIPs, need to convert to system coordinates.
                Point deviceTopLeft = DpiHelper.LogicalPixelsToDevice(new Point(window.GlassBorderThickness.Left, window.GlassBorderThickness.Top));
                Point deviceBottomRight = DpiHelper.LogicalPixelsToDevice(new Point(window.GlassBorderThickness.Right, window.GlassBorderThickness.Bottom));

                var dwmMargin = new NativeMethods.MARGINS((int)Math.Ceiling(deviceTopLeft.X), (int)Math.Ceiling(deviceTopLeft.Y), (int)Math.Ceiling(deviceBottomRight.X), (int)Math.Ceiling(deviceBottomRight.Y));
                NativeMethods.DwmExtendFrameIntoClientArea(handle, dwmMargin);
            }
        }

        #endregion

        #region Window Function

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_SETTEXT:
                case NativeMethods.WM_SETICON:
                    {
                        bool modified = ModifyStyle(NativeMethods.WS_VISIBLE, 0);

                        // Setting the caption text and icon cause Windows to redraw the caption.
                        // Letting the default WndProc handle the message without the WS_VISIBLE
                        // style applied bypasses the redraw.
                        IntPtr lRet = NativeMethods.DefWindowProc(handle, msg, wParam, lParam);

                        // Put back the style we removed.
                        if (modified)
                        {
                            ModifyStyle(0, NativeMethods.WS_VISIBLE);
                        }
                        handled = true;
                        return lRet;
                    }
                case NativeMethods.WM_NCACTIVATE:
                    {
                        // Despite MSDN's documentation of lParam not being used,
                        // calling DefWindowProc with lParam set to -1 causes Windows not to draw over the caption.

                        // Directly call DefWindowProc with a custom parameter
                        // which bypasses any other handling of the message.
                        IntPtr lRet = NativeMethods.DefWindowProc(handle, NativeMethods.WM_NCACTIVATE, wParam, new IntPtr(-1));
                        window.IsNonClientAreaActive = (wParam != IntPtr.Zero);
                        handled = true;
                        return lRet;
                    }
                case NativeMethods.WM_NCCALCSIZE:
                    {
                        this.FixClientRect(lParam);

                        handled = true;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_NCHITTEST:
                    {
                        return DoHitTest(msg, wParam, lParam, out handled);
                    }
                case NativeMethods.WM_NCRBUTTONUP:
                    {
                        // Emulate the system behavior of clicking the right mouse button over the caption area
                        // to bring up the system menu.
                        if (NativeMethods.HTCAPTION == wParam.ToInt32())
                        {
                            ShowSystemMenuPhysicalCoordinates(new Point(NativeMethods.LowWord(lParam), NativeMethods.HiWord(lParam)));
                        }
                        handled = false;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_SIZE:
                    {
                        const int SIZE_MAXIMIZED = 2;

                        // Force when maximized.
                        // We can tell what's happening right now, but the Window doesn't yet know it's
                        // maximized.  Not forcing this update will eventually cause the
                        // default caption to be drawn.
                        WindowState? state = null;
                        if (wParam.ToInt32() == SIZE_MAXIMIZED)
                        {
                            state = WindowState.Maximized;
                        }
                        UpdateSystemMenu(state);

                        // Still let the default WndProc handle this.
                        handled = false;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_WINDOWPOSCHANGED:
                    {
                        if (!window.IsDwmEnabled)
                        {
                            var wp = (NativeMethods.WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.WINDOWPOS));
                            //ModifyStyle(NativeMethods.WS_VISIBLE, 0);
                            SetRoundingRegion(wp);
                            //ModifyStyle(0, NativeMethods.WS_VISIBLE);
                        }

                        // Still want to pass this to DefWndProc
                        handled = false;
                        return IntPtr.Zero;
                    }
                case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                    {
                        if (!window.DontUseDwm)
                        {
                            window.IsDwmEnabled = NativeMethods.IsDwmEnabled();
                        }

                        UpdateFrameState(false);

                        handled = false;
                        return IntPtr.Zero;
                    }
            }

            if (FrameworkHelper.PresentationFrameworkVersion < new Version("4.0"))
            {
                switch (msg)
                {
                    case NativeMethods.WM_SETTINGCHANGE:
                        {
                            FixFrameworkIssues();

                            handled = false;
                            return IntPtr.Zero;
                        }
                    case NativeMethods.WM_ENTERSIZEMOVE:
                        {
                            isUserResizing = true;

                            if (window.WindowState != WindowState.Maximized)
                            {
                                // Check for the docked window case.  The window can still be restored when it's in this position so 
                                // try to account for that and not update the start position.
                                if (!IsWindowDocked)
                                {
                                    windowPosAtStartOfUserMove = new Point(window.Left, window.Top);
                                }
                                // Realistically we also don't want to update the start position when moving from one docked state to another (or to and from maximized),
                                // but it's tricky to detect and this is already a workaround for a bug that's fixed in newer versions of the framework.
                                // Not going to try to handle all cases.
                            }

                            handled = false;
                            return IntPtr.Zero;
                        }
                    case NativeMethods.WM_EXITSIZEMOVE:
                        {
                            isUserResizing = false;

                            // On Win7 the user can change the Window's state by dragging the window to the top of the monitor.
                            // If they did that, then we need to try to update the restore bounds or else WPF will put the window at the maximized location (e.g. (-8,-8)).
                            if (window.WindowState == WindowState.Maximized)
                            {
                                window.Top = windowPosAtStartOfUserMove.Y;
                                window.Left = windowPosAtStartOfUserMove.X;
                            }

                            handled = false;
                            return IntPtr.Zero;
                        }
                    case NativeMethods.WM_MOVE:
                        {
                            if (isUserResizing)
                            {
                                hasUserMovedWindow = true;
                            }

                            handled = false;
                            return IntPtr.Zero;
                        }
                }
            }

            return IntPtr.Zero;
        }

        private void FixClientRect(IntPtr lParam)
        {
            // This is needed to fix https://fluent.codeplex.com/workitem/22440
            // Without this, maximizing the window on Windows 8.1 and higher renders the minimize, maximize and close buttons offscreen
            {
#if NET45
                var thickness = DpiHelper.LogicalThicknessToDevice(SystemParameters.WindowResizeBorderThickness);
#else
                var thickness = DpiHelper.LogicalThicknessToDevice(new Thickness(4, 4, 4, 4));
#endif
                var structure = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                structure.bottom -= (int)thickness.Bottom;
                Marshal.StructureToPtr(structure, lParam, false);
            }

            if (window.WindowState == WindowState.Maximized)
            {
                var abd = new NativeMethods.APPBARDATA();
                var temp = NativeMethods.SHAppBarMessage(4, ref abd);
                if (temp == 1)
                {
                    var ncParams = (NativeMethods.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));
                    ncParams.rect0.Bottom -= 9;
                    Marshal.StructureToPtr(ncParams, lParam, false);
                }
            }

            // below code should fix issue 18229, but causes the minimize, restore and close button to be unreachable when window is maximized
            ////// Fixes the client rect to render edge to edge on one display if maximized
            ////// Issue fixed with this method: 
            ////// http://fluent.codeplex.com/workitem/18229
            ////// "When maximized, client area goes 8px offscreen, killing perf on multimonitor"
            ////if (this.WindowState == WindowState.Maximized)
            ////{
            ////    var ncParams = (NativeMethods.NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));

            ////    ncParams.rect0.Left += 8;
            ////    ncParams.rect0.Top += 8;
            ////    ncParams.rect0.Right -= 8;
            ////    ncParams.rect0.Bottom -= 8;

            ////    Marshal.StructureToPtr(ncParams, lParam, false);

            ////    if (VisualTreeHelper.GetChildrenCount(this) != 0)
            ////    {
            ////        var rootElement = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);
            ////        rootElement.Margin = new Thickness(-8);
            ////    }
            ////}
            ////else
            ////{
            ////    if (VisualTreeHelper.GetChildrenCount(this) != 0)
            ////    {
            ////        var rootElement = (FrameworkElement)VisualTreeHelper.GetChild(this, 0);
            ////        rootElement.Margin = new Thickness(0);
            ////    }
            ////}
        }

        private IntPtr DoHitTest(int msg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            IntPtr lRet = IntPtr.Zero;
            handled = false;

            // Give DWM a chance at this first.
            if (window.IsDwmEnabled && (Mouse.Captured == null))
            {
                // If we're on Vista, give the DWM a chance to handle the message first.
                handled = NativeMethods.DwmDefWindowProc(handle, msg, wParam, lParam, ref lRet);
            }

            // Handle letting the system know if we consider the mouse to be in our effective non-client area.
            // If DWM already handled this by way of DwmDefWindowProc, then respect their call.
            if (IntPtr.Zero == lRet)
            {
                var mousePosScreen = new Point(NativeMethods.LowWord(lParam), NativeMethods.HiWord(lParam));
                NativeMethods.Rect wndPosition = new NativeMethods.Rect();
                NativeMethods.GetWindowRect(handle, ref wndPosition);
                Rect windowPosition = new Rect(wndPosition.Left, wndPosition.Top, wndPosition.Right - wndPosition.Left, wndPosition.Bottom - wndPosition.Top);

                int ht = HitTestNonClientArea(
                    DpiHelper.DeviceRectToLogical(windowPosition),
                    DpiHelper.DevicePixelsToLogical(mousePosScreen));

                // Don't blindly respect HTCAPTION.
                // We want UIElements in the caption area to be actionable so run through a hittest first.
                if ((ht != NativeMethods.HTCLIENT) && (mainGrid != null) && mainGrid.IsLoaded)
                {
                    /*int mp = lParam.ToInt32();
                    if (!mainGrid.IsVisible) return IntPtr.Zero;
                    Point ptMouse = new Point((short)(mp & 0x0000FFFF), (short)((mp >> 16) & 0x0000FFFF));
                    //ptMouse = DpiHelper.DevicePixelsToLogical(ptMouse);
                    ptMouse = mainGrid.PointFromScreen(ptMouse);*/
                    var ptMouse = mainGrid.PointFromScreen(mousePosScreen);

                    /*
                    Point mousePosWindow = mousePosScreen;
                    mousePosWindow.Offset(-windowPosition.X, -windowPosition.Y);*/

                    IInputElement inputElement = mainGrid.InputHitTest(ptMouse);
                    if (inputElement != null)
                    {
                        FrameworkElement frameworkElement = inputElement as FrameworkElement;
                        if ((frameworkElement != null) && (frameworkElement.Name == "PART_TitleBar")) ht = NativeMethods.HTCAPTION;
                        else if (inputElement != mainGrid) ht = NativeMethods.HTCLIENT;
                    }
                }

                // Check resize grip
                ResizeGrip grip = (this.window.Template.FindName("PART_ResizeGrip", this.window) as ResizeGrip);
                if ((grip != null) && (grip.IsLoaded) && (grip.InputHitTest(grip.PointFromScreen(mousePosScreen)) != null))
                {
                    if (this.window.FlowDirection == FlowDirection.LeftToRight) ht = NativeMethods.HTBOTTOMRIGHT;
                    else ht = NativeMethods.HTBOTTOMLEFT;
                }

                handled = true;
                lRet = new IntPtr(ht);
            }
            return lRet;
        }

        private IntPtr MouseWndProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if ((code >= 0) && (Mouse.Captured != null) && (window.IsActive))
            {
                int msg = wParam.ToInt32();
                NativeMethods.MOUSEHOOKSTRUCT cc = (NativeMethods.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(NativeMethods.MOUSEHOOKSTRUCT));
                switch (msg)
                {
                    case 0x0203:
                    case 0x0201:
                    case 0x0204:
                    case 0x0206:
                        // Check popups
                        if (IsInRootPopup(Mouse.Captured as DependencyObject))
                            return IntPtr.Zero;

                        //
                        IntPtr pp = Marshal.AllocHGlobal(Marshal.SizeOf(cc.pt));
                        Marshal.StructureToPtr(cc.pt, pp, false);
                        bool handled;
                        IntPtr htResult = DoHitTest(NativeMethods.WM_NCHITTEST, IntPtr.Zero, NativeMethods.MakeDWord(cc.pt.x, cc.pt.y), out handled);
                        if (!handled) htResult = NativeMethods.DefWindowProc(handle, NativeMethods.WM_NCHITTEST, IntPtr.Zero, NativeMethods.MakeDWord(cc.pt.x, cc.pt.y));
                        int htR = htResult.ToInt32();
                        int ncMessage = 0x00A1;
                        if (msg == 0x0203) ncMessage = 0x00A3;
                        else if ((msg == 0x0204) && ((htR == NativeMethods.HTCAPTION) || (htR == NativeMethods.HTTOP)))
                        {
                            NativeMethods.ReleaseCapture();
                            if (htR == NativeMethods.HTCAPTION) ShowSystemMenu(new Point(cc.pt.x, cc.pt.y));
                            ncMessage = 0x00A4;
                        }

                        if ((htR == NativeMethods.HTCAPTION) || (htR == NativeMethods.HTTOP))
                        {
                            NativeMethods.ReleaseCapture();
                            htResult = DoHitTest(NativeMethods.WM_NCHITTEST, IntPtr.Zero, NativeMethods.MakeDWord(cc.pt.x, cc.pt.y), out handled);
                            if (!handled) htResult = NativeMethods.DefWindowProc(handle, NativeMethods.WM_NCHITTEST, IntPtr.Zero, NativeMethods.MakeDWord(cc.pt.x, cc.pt.y));
                            htR = htResult.ToInt32();
                        }

                        if ((htR == 3) || (htR == 8) || (htR == 9) || (htR == 20) || (htR == 21))
                        {
                            NativeMethods.ReleaseCapture();
                            NativeMethods.SendMessage(handle, ncMessage, htResult, pp);
                        }
                        else if (htR != NativeMethods.HTCLIENT)
                        {
                            NativeMethods.ReleaseCapture();
                            window.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                                                   (ThreadStart)
                                                   (() => NativeMethods.SendMessage(handle, ncMessage, htResult, pp)));
                        }
                        return IntPtr.Zero;
                }
            }
            return NativeMethods.CallNextHookEx(mouseHook, code, wParam, lParam);
        }

        private static bool IsInRootPopup(DependencyObject element)
        {
            if (PopupService.IsMousePhysicallyOver(element as UIElement))
            {
                return true;
            }

            // Check if is drop down control
            var dropDown = element as IDropDownControl;
            if (dropDown != null
                && dropDown.IsDropDownOpen
                && dropDown.DropDownPopup != null
                && dropDown.DropDownPopup.Child != null
                && PopupService.IsMousePhysicallyOver(dropDown.DropDownPopup.Child))
            {
                return true;
            }

            // Check if is context menu
            var menu = element as ContextMenu;
            if (menu != null
                && menu.IsOpen
                && PopupService.IsMousePhysicallyOver(menu))
            {
                return true;
            }

            // Check if is menu item
            var menuItem = element as MenuItem;
            if (menuItem != null
                && menuItem.IsDropDownOpen
                && PopupService.IsMousePhysicallyOver(menuItem.DropDownPopup.Child))
            {
                return true;
            }

            // Check if is Popup
            var popup = element as Popup;
            if (popup != null
                && popup.IsOpen
                && PopupService.IsMousePhysicallyOver(popup.Child))
            {
                return true;
            }

            // Check childs
            var children = LogicalTreeHelper.GetChildren(element);

            // we are only interested in children of type FrameworkElement
            foreach (var child in children.OfType<FrameworkElement>())
            {
                if (child.IsVisible
                    && IsInRootPopup(child))
                {
                    return true;
                }

            }

            return false;
        }

        #endregion

        // Windows tries hard to hide this state from applications.
        // Generally you can tell that the window is in a docked position because the restore bounds from GetWindowPlacement
        // don't match the current window location and it's not in a maximized or minimized state.
        // Because this isn't docked or supported, it's also not incredibly consistent.  Sometimes some things get updated in
        // different orders, so this isn't absolutely reliable.
        private bool IsWindowDocked
        {
            get
            {
                if (window.WindowState != WindowState.Normal)
                {
                    return false;
                }

                var adjustedOffset = GetAdjustedWindowRect(new NativeMethods.Rect { Bottom = 100, Right = 100 });
                var windowTopLeft = new Point(window.Left, window.Top);
                windowTopLeft -= (Vector)DpiHelper.DevicePixelsToLogical(new Point(adjustedOffset.Left, adjustedOffset.Top));

                return window.RestoreBounds.Location != windowTopLeft;
            }
        }

        public void OnApplyTemplate()
        {
            this.mainGrid = this.window.Template.FindName("PART_MainGrid", this.window) as Grid;

            if (this.iconImage != null)
            {
                this.iconImage.MouseUp -= OnIconImageOnMouseUp;
                this.iconImage.MouseDown -= OnIconImageOnMouseDown;
            }

            this.iconImage = this.window.Template.FindName("PART_IconImage", this.window) as FrameworkElement;

            if (this.iconImage != null)
            {
                this.iconImage.MouseUp += OnIconImageOnMouseUp;
                this.iconImage.MouseDown += OnIconImageOnMouseDown;
            }
        }

        private void OnIconImageOnMouseUp(object o, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    this.HandleIconImageLeftMouseButtonUp(e);
                    break;

                case MouseButton.Right:
                    this.HandleIconImageRightMouseButtonUp();
                    break;
            }
        }

        private void HandleIconImageLeftMouseButtonUp(MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                var pos = this.iconImage.PointToScreen(new Point(0, 0));
                var size = new Size(this.iconImage.ActualWidth, this.iconImage.ActualHeight);
                size = DpiHelper.LogicalSizeToDevice(size);

                // There is no neet to shift System Menu in RightToLeft mode
                //if (FlowDirection == FlowDirection.RightToLeft) pos.X += size.Width;

                if (this.window.FlowDirection == FlowDirection.LeftToRight)
                {
                    this.ShowSystemMenu(new Point(pos.X, pos.Y + size.Height));
                }
                else
                {
                    // WPF has bug with incorrect coordinate detection when system menu is opened on RightToLeft FlowDirection 
                    // so for correct double click we need to pause system menu opening
                    var timer = new DispatcherTimer(DispatcherPriority.SystemIdle)
                    {
                        Interval = TimeSpan.FromSeconds(0.1)
                    };
                    timer.Tick += (s, ee) =>
                    {
                        this.ShowSystemMenu(new Point(pos.X, pos.Y + size.Height));
                        ((DispatcherTimer) s).Stop();
                    };
                    timer.Start();
                }
            }
        }

        private void HandleIconImageRightMouseButtonUp()
        {
            var pos = this.iconImage.PointToScreen(Mouse.GetPosition(this.iconImage));
            this.ShowSystemMenu(pos);
        }

        private void OnIconImageOnMouseDown(object o, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            if (e.ClickCount == 2)
            {
                this.window.Close();
            }
        }
    }
}