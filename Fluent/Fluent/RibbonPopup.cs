#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents popup. This popup has Microsoft Office behavior
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    public class RibbonPopup : Popup
    {
        #region Static Methods

        // Currently opened popups
        static readonly List<Popup> openedPopups = new List<Popup>();

        static readonly Dictionary<IntPtr, Popup> openedPopupsByHandle = new Dictionary<IntPtr, Popup>();
        static readonly Dictionary<Popup, IntPtr> openedHandlesByPopup = new Dictionary<Popup, IntPtr>();

        /// <summary>
        /// Returns active popup
        /// </summary>
        /// <returns>Active popup or null if no popup is opened</returns>
        internal static Popup GetActivePopup()
        {
            if(openedPopups.Count==0) return null;
            return openedPopups[openedPopups.Count - 1];
        }

        /// <summary>
        /// Closes all currently opened popups
        /// </summary>
        public static void CloseAll()
        {
            ClosePopups(0);
        }

        // TODO: maybe rename CollapseCurrent -> CloseCurrent

        /// <summary>
        /// Closes the current popup
        /// </summary>
        public static void CollapseCurrent()
        {
            if (openedPopups.Count > 0) ClosePopups(openedPopups.Count - 1);
        }

        #endregion

        #region Fields

        /*// Current HwndSource of this Popup
        HwndSource hwndSource;
        // TODO: comment this field (isFirstMouseUp)
        bool isFirstMouseUp;*/

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonPopup()
        {
            Inititalize(this);
        }


        static RibbonPopup()
        {
            //IsOpenProperty.AddOwner(typeof(RibbonPopup), new FrameworkPropertyMetadata(OnIsOpenChanged));
        }
        /*
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue)
            {
                RibbonPopup popup = d as RibbonPopup;
                if (openedPopups.Contains(popup)) openedPopups.Remove(popup);
            }
        }*/

        #endregion

        #region Overrides
        
        /// <summary>
        /// Responds to the condition in which the value of the Popup.IsOpen property 
        /// changes from false to true.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            
            OnOpened(this,e);                                    
        }

        /*/// <summary>
        /// Provides class handling for the PreviewMouseLeftButtonDown event
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            isFirstMouseUp = false;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.PreviewMouseLeftButtonUp event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
                e.Handled = true;
            }
            if (isFirstMouseUp)
            {
                e.Handled = true;
                isFirstMouseUp = false;
            }
        }*/

        /// <summary>
        /// Responds when the value of the Popup.IsOpen property changes from to true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            OnClosed(this, e);
        }


        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyDown attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            OnKeyDown(this, e);
        }

        #endregion

        #region Private methods

        private static void ClosePopups(int index)
        {
            for (int i = openedPopups.Count - 1; i >= index; i--)
            {
                openedPopups[i].PopupAnimation = PopupAnimation.Fade;
                openedPopups[i].IsOpen = false;
                //openedPopups[i].hwndSource = null;
                if (openedPopups.Contains(openedPopups[i]))
                {
                    IntPtr handle = openedHandlesByPopup[openedPopups[i]];
                    openedHandlesByPopup.Remove(openedPopups[i]);
                    openedPopupsByHandle.Remove(handle);

                    openedPopups.Remove(openedPopups[i]);
                }
            }
        }


        internal void Activate()
        {
            Activate(this);
        }

        /// <summary>
        /// Window function
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private static IntPtr WindowProc(
                   IntPtr hwnd,
                   int msg,
                   IntPtr wParam,
                   IntPtr lParam,
                   ref bool handled)
        {            
            switch (msg)
            {
                case 0x0006/*WM_ACTIVATE*/:
                    {
                        if (((short)wParam.ToInt32()) == 0)
                        {
                            // BUG: Fix for MessageBox: without parent messagebox closes
                            //if (hwndSource != null)
                            {
                                if (NativeMethods.GetWindowLongPtr(lParam, NativeMethods.GWL_HWNDPARENT) ==
                                    hwnd)
                                    break;
                                if (openedPopups.Count(x => openedHandlesByPopup[x] == lParam) == 0)
                                {
                                    ClosePopups(0);
                                }
                            }
                        }
                        else
                        {
                            Popup popup = openedPopupsByHandle[hwnd];
                            int index = openedPopups.IndexOf(popup);
                            ClosePopups(index + 1);
                            Window wnd = Window.GetWindow(popup);
                            if (wnd != null)
                            {
                                IntPtr parentHwnd = (new WindowInteropHelper(wnd)).Handle;
                                NativeMethods.SendMessage(parentHwnd, 0x0086, new IntPtr(1), IntPtr.Zero);
                            }
                        }
                        break;
                    }
                case 0x0021/*WM_MOUSEACTIVATE*/:
                    {
                        Popup popup = openedPopupsByHandle[hwnd];
                        int index = openedPopups.IndexOf(popup);
                        ClosePopups(index + 1);
                        Window wnd = Window.GetWindow(popup);
                        if (wnd != null)
                        {
                            IntPtr parentHwnd = (new WindowInteropHelper(wnd)).Handle;
                            NativeMethods.SendMessage(parentHwnd, 0x0086, new IntPtr(1), IntPtr.Zero);
                        }
                        break;
                    }
                case 0x0002/*WM_DESTROY*/:
                    {
                        /*if ((hwndSource != null) && (!hwndSource.IsDisposed))
                        {
                            // Remove hook
                            hwndSource.RemoveHook(WindowProc);
                            hwndSource = null;
                        }*/
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        #endregion

        #region Static Methods for Popup override

        static DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);

        public static void AttachToPopup(Popup popup)
        {
            // Initialize 
            Inititalize(popup);

            bool isOpen = popup.IsOpen;
            popup.IsOpen = false;

            popup.Opened += OnOpened;
            popup.Closed += OnClosed;
            popup.KeyDown += OnKeyDown;
            popup.IsOpen = isOpen;
        }

        private static void Inititalize(Popup popup)
        {
            popup.StaysOpen = true;
            popup.Focusable = false;
            popup.ToolTip = new ToolTip();
            (popup.ToolTip as ToolTip).Template = null;
            popup.AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private static void OnClick(object sender, RoutedEventArgs e)
        {
            (sender as Popup).IsOpen = false;
        }

        private static void OnOpened(object sender, EventArgs e)
        {
           /* timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += OnTimerTick;
            timer.Start();*/
            Popup popup = sender as Popup;
            //isFirstMouseUp = true;
            popup.PopupAnimation = PopupAnimation.None;

            HwndSource hwndSource = null;

            if (popup.Child != null) hwndSource = (HwndSource)PresentationSource.FromVisual(popup.Child);
            if (hwndSource != null)
            {
                hwndSource.AddHook(WindowProc);
                // Set popup non-topmost to fix bug with tooltips
                /*NativeMethods.Rect rect = new NativeMethods.Rect();
                if (NativeMethods.GetWindowRect(hwndSource.Handle, ref rect))
                {
                    NativeMethods.SetWindowPos(hwndSource.Handle, new IntPtr(-2), rect.Left, rect.Top, (int)popup.Width,
                                               (int)popup.Height,
                                               NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
                                               NativeMethods.SWP_NOACTIVATE);
                }*/
                openedPopups.Add(popup);
                openedPopupsByHandle.Add(hwndSource.Handle, popup);
                openedHandlesByPopup.Add(popup, hwndSource.Handle);
            }
            
            Mouse.Capture(null);

            Activate(popup);
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            Debug.WriteLine(Mouse.Captured);
            //Mouse.Capture(null);
        }

        private static void OnClosed(object sender, EventArgs e)
        {
            //timer.Stop();
            Popup popup = sender as Popup;
            if (openedPopups.Contains(popup))
            {
                openedPopups.Remove(popup);
                IntPtr handle = openedHandlesByPopup[popup];
                openedHandlesByPopup.Remove(popup);
                openedPopupsByHandle.Remove(handle);
            }
            popup.PopupAnimation = PopupAnimation.None;

            if (openedPopups.Count == 0)
            {
                Window wnd = Window.GetWindow(popup);
                if (wnd != null) wnd.Focus();
            }
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            Popup popup = sender as Popup;
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                popup.IsOpen = false;
                return;
            }
            if ((e.Key == Key.System) && ((e.SystemKey == Key.LeftAlt) || (e.SystemKey == Key.RightAlt) || (e.SystemKey == Key.F10)))
            {
                if (e.SystemKey != Key.F10)
                {
                    ClosePopups(0);
                }
                else e.Handled = true;
                return;
            }
        }

        private static void Activate(Popup popup)
        {
            if (!openedHandlesByPopup.ContainsKey(popup)) return;
            IntPtr handle = openedHandlesByPopup[popup];            
            NativeMethods.SetActiveWindow(handle);
            NativeMethods.SetForegroundWindow(handle);
        }

        #endregion
    }
}
