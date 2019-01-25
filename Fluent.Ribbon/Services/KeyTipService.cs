// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using ControlzEx.Standard;
    using Fluent.Internal;

    /// <summary>
    /// Handles Alt, F10 and so on
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class KeyTipService
    {
        #region Fields

        // Host element, usually this is Ribbon
        private readonly Ribbon ribbon;

        // Timer to show KeyTips with delay
        private readonly DispatcherTimer timer;

        // Is KeyTips Actived now
        private KeyTipAdorner activeAdornerChain;
        // This element must be remembered to restore focus
        private FocusWrapper backUpFocusedControl;

        // Window where we attached
        private Window window;

        // Whether we attached to window
        private bool attached;

        // Attached HWND source
        private HwndSource attachedHwndSource;

        private string currentUserInput;

        /// <summary>
        /// Checks if any keytips are visible.
        /// </summary>
        public bool AreAnyKeyTipsVisible
        {
            get
            {
                if (this.activeAdornerChain != null)
                {
                    return this.activeAdornerChain.AreAnyKeyTipsVisible;
                }

                return false;
            }
        }

        private static readonly Key[] modifierKeys =
        {
            Key.LeftShift,
            Key.RightShift,
            Key.LeftCtrl,
            Key.RightCtrl,
            Key.LeftAlt,
            Key.RightAlt,
        };

        /// <summary>
        /// The default keys used to activate key tips.
        /// </summary>
        public static IList<Key> DefaultKeyTipKeys =>
            new List<Key>
            {
                Key.LeftAlt,
                Key.RightAlt,
                Key.F10
            };

        /// <summary>
        /// List of key tip activation keys.
        /// </summary>
        public IList<Key> KeyTipKeys { get; } = DefaultKeyTipKeys;

        #endregion

        #region Initialization

        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="ribbon">Host element</param>
        public KeyTipService(Ribbon ribbon)
        {
            this.ribbon = ribbon ?? throw new ArgumentNullException(nameof(ribbon));

            // Initialize timer
            this.timer = new DispatcherTimer(TimeSpan.FromSeconds(0.7), DispatcherPriority.SystemIdle, this.OnDelayedShow, Dispatcher.CurrentDispatcher);
            this.timer.Stop();
        }

        #endregion

        /// <summary>
        /// Attaches self
        /// </summary>
        public void Attach()
        {
            if (this.attached
                || this.ribbon.IsKeyTipHandlingEnabled == false)
            {
                return;
            }

            this.attached = true;

            // KeyTip service must not work in design mode
            if (DesignerProperties.GetIsInDesignMode(this.ribbon))
            {
                return;
            }

            this.window = Window.GetWindow(this.ribbon);
            if (this.window == null)
            {
                return;
            }

            this.window.PreviewKeyDown += this.OnWindowPreviewKeyDown;
            this.window.KeyUp += this.OnWindowKeyUp;

            // Hookup non client area messages
            this.attachedHwndSource = (HwndSource)PresentationSource.FromVisual(this.window);
            this.attachedHwndSource?.AddHook(this.WindowProc);
        }

        /// <summary>
        /// Detachs self
        /// </summary>
        public void Detach()
        {
            if (this.attached == false)
            {
                return;
            }

            this.attached = false;

            // prevent delay show
            this.timer.Stop();

            if (this.window != null)
            {
                this.window.PreviewKeyDown -= this.OnWindowPreviewKeyDown;
                this.window.KeyUp -= this.OnWindowKeyUp;

                this.window = null;
            }

            // Unhook non client area messages
            this.attachedHwndSource?.RemoveHook(this.WindowProc);
        }

        // Window's messages hook up
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
#pragma warning disable 618
            var message = (WM)msg;

            // We must terminate the keytip's adorner chain if:
            if (message == WM.NCACTIVATE // mouse clicks in non client area
                || (message == WM.ACTIVATE && wParam == IntPtr.Zero) // the window is deactivated
                || (message >= WM.NCLBUTTONDOWN && message <= WM.NCXBUTTONDBLCLK) // mouse click (non client area)
                || (message >= WM.LBUTTONDOWN && message <= WM.MBUTTONDBLCLK)) // mouse click
            {
                if (this.activeAdornerChain?.IsAdornerChainAlive == true)
                {
                    this.Terminate();
                }
            }

            // Fix for #632.
            // Yes this looks awkward, calling the PopupService here, but the alternative would be to let the PopupService know about windows.
            if (message == WM.ACTIVATE
                && wParam == IntPtr.Zero) // the window is deactivated
            {
                PopupService.RaiseDismissPopupEvent(this.ribbon, DismissPopupMode.Always, DismissPopupReason.ApplicationLostFocus);
                PopupService.RaiseDismissPopupEvent(Mouse.Captured, DismissPopupMode.Always, DismissPopupReason.ApplicationLostFocus);
                PopupService.RaiseDismissPopupEvent(Keyboard.FocusedElement, DismissPopupMode.Always, DismissPopupReason.ApplicationLostFocus);
            }
#pragma warning restore 618

            return IntPtr.Zero;
        }

        private void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat
                || e.Handled)
            {
                return;
            }

            if (this.ribbon.IsCollapsed
                || this.ribbon.IsEnabled == false
                || this.window.IsActive == false)
            {
                return;
            }

            // Keytips should be cancelled if Alt+Num0 is pressed #241.
            // This allows entering special keys via numpad.
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt
                && e.SystemKey >= Key.NumPad0
                && e.SystemKey <= Key.NumPad9)
            {
                this.Terminate();
                return;
            }

            if (this.IsShowOrHideKey(e))
            {
                if (this.activeAdornerChain == null
                    || this.activeAdornerChain.IsAdornerChainAlive == false
                    || this.activeAdornerChain.AreAnyKeyTipsVisible == false)
                {
                    this.ShowDelayed();
                }
                else
                {
                    this.Terminate();
                }
            }
            else if (e.Key == Key.Escape
                && this.activeAdornerChain != null)
            {
                this.activeAdornerChain.ActiveKeyTipAdorner.Back();
                this.ClearUserInput();
                e.Handled = true;
            }
            else if ((e.Key != Key.System && this.activeAdornerChain == null)
                    || e.SystemKey == Key.Escape
                    || (e.KeyboardDevice.Modifiers != ModifierKeys.Alt && this.activeAdornerChain == null))
            {
                return;
            }
            else
            {
                var actualKey = e.Key == Key.System ? e.SystemKey : e.Key;
                // we need to get the real string input for the key because of keys like �,�,� #258
                var key = KeyEventUtility.GetStringFromKey(actualKey);
                var isKeyRealInput = string.IsNullOrEmpty(key) == false
                    && key != "\t";

                // Don't do anything and let WPF handle the rest
                if (isKeyRealInput == false)
                {
                    // This block is a "temporary" fix for keyboard navigation not matching the office behavior.
                    // If someone finds a way to implement it properly, here is your starting point.
                    // In office: If you navigate by keyboard (in menus) and keytips are shown they are shown or hidden based on the menu you are in.
                    // Implementing navigation the way office does would require complex focus/state tracking etc. so i decided to just terminate keytips and not restore focus.
                    {
                        this.backUpFocusedControl = null;
                        this.Terminate();
                    }

                    return;
                }

                var shownImmediately = false;

                // Should we show the keytips and immediately react to key?
                if (this.activeAdornerChain == null
                    || this.activeAdornerChain.IsAdornerChainAlive == false
                    || this.activeAdornerChain.AreAnyKeyTipsVisible == false)
                {
                    this.ShowImmediatly();
                    shownImmediately = true;
                }

                if (this.activeAdornerChain == null)
                {
                    return;
                }

                var previousInput = this.currentUserInput;
                this.currentUserInput += key;

                if (this.activeAdornerChain.ActiveKeyTipAdorner.ContainsKeyTipStartingWith(this.currentUserInput) == false)
                {
                    // Handles access-keys #258
                    if (shownImmediately)
                    {
                        this.Terminate();
                        return;
                    }

                    // If no key tips match the current input, continue with the previously entered and still correct keys.
                    this.currentUserInput = previousInput;
                    System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                    return;
                }

                if (this.activeAdornerChain.ActiveKeyTipAdorner.Forward(this.currentUserInput, true))
                {
                    this.ClearUserInput();
                    e.Handled = true;
                    return;
                }

                this.activeAdornerChain.ActiveKeyTipAdorner.FilterKeyTips(this.currentUserInput);
                e.Handled = true;
            }
        }

        private void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (this.ribbon.IsCollapsed
                || this.ribbon.IsEnabled == false
                || this.window.IsActive == false)
            {
                this.Terminate();
                return;
            }

            if (this.IsShowOrHideKey(e))
            {
                this.ClearUserInput();

                if (this.timer.IsEnabled)
                {
                    this.ShowImmediatly();
                }

                if (this.activeAdornerChain != null)
                {
                    e.Handled = true;
                }
            }
            else
            {
                this.timer.Stop();
            }
        }

        private bool IsShowOrHideKey(KeyEventArgs e)
        {
            var realKey = e.Key == Key.System
                              ? e.SystemKey
                              : e.Key;

            // Shift + F10 is meant to open the context menu. So we just ignore it.
            if (realKey == Key.F10
                && (Keyboard.IsKeyDown(Key.LeftShift)
                    || Keyboard.IsKeyDown(Key.RightShift)))
            {
                return false;
            }

            var isShowOrHideKey = this.KeyTipKeys.Any(x => x == realKey);

            if (isShowOrHideKey == false)
            {
                return false;
            }

            var blacklistedModifierKeys = modifierKeys.Except(this.KeyTipKeys);
            var blacklistedKeyPressed = blacklistedModifierKeys.Any(Keyboard.IsKeyDown);
            return blacklistedKeyPressed == false;
        }

        private void ClearUserInput()
        {
            this.currentUserInput = string.Empty;
        }

        private void ClosePopups()
        {
            PopupService.RaiseDismissPopupEvent(Keyboard.FocusedElement, DismissPopupMode.Always, DismissPopupReason.ShowingKeyTips);
        }

        private void RestoreFocus()
        {
            this.backUpFocusedControl?.Focus();
            this.backUpFocusedControl = null;
        }

        private void OnAdornerChainTerminated(object sender, KeyTipPressedResult e)
        {
            this.activeAdornerChain.Terminated -= this.OnAdornerChainTerminated;
            this.activeAdornerChain = null;
            this.ClearUserInput();

            if (e.PressedElementOpenedPopup == false)
            {
                this.ClosePopups();
            }

            if (e.PressedElementAquiredFocus == false)
            {
                this.RestoreFocus();
            }
        }

        private void OnDelayedShow(object sender, EventArgs e)
        {
            if (this.activeAdornerChain == null)
            {
                this.Show();
            }

            this.timer.Stop();
        }

        private void ShowImmediatly()
        {
            this.Show();
        }

        private void ShowDelayed()
        {
            this.Terminate();

            this.timer.Start();
        }

        private void Terminate()
        {
            this.activeAdornerChain?.Terminate(KeyTipPressedResult.Empty);
        }

        private void Show()
        {
            this.timer.Stop();

            // Check whether the window is
            // - still present (prevents exceptions when window is closed by system commands)
            // - still active (prevents keytips showing during Alt-Tab'ing)
            if (this.window == null
                || this.window.IsActive == false)
            {
                this.RestoreFocus();
                return;
            }

            // Special behavior for backstage, application menu and start screen.
            // If one of those is open we have to forward key tips directly to them.
            var keyTipsTarget = this.GetStartScreen()
                ?? this.GetBackstage()
                ?? this.GetApplicationMenu()
                ?? this.ribbon;

            if (keyTipsTarget == null)
            {
                return;
            }

            this.ClosePopups();

            // If focus is inside the Ribbon already we don't want to jump around after finishing with KeyTips
            if (UIHelper.GetParent<Ribbon>(Keyboard.FocusedElement as DependencyObject) == null)
            {
                this.backUpFocusedControl = FocusWrapper.GetWrapperForCurrentFocus();
            }

            if (keyTipsTarget is Ribbon targetRibbon
                && targetRibbon.IsMinimized == false
                && targetRibbon.SelectedTabIndex >= 0
                && targetRibbon.TabControl != null)
            {
                // Focus ribbon
                (this.ribbon.TabControl.ItemContainerGenerator.ContainerFromIndex(this.ribbon.TabControl.SelectedIndex) as UIElement)?.Focus();
            }

            this.ClearUserInput();

            this.activeAdornerChain = new KeyTipAdorner(keyTipsTarget, keyTipsTarget, null);
            this.activeAdornerChain.Terminated += this.OnAdornerChainTerminated;
            this.activeAdornerChain.Attach();
        }

        private FrameworkElement GetBackstage()
        {
            if (this.ribbon.Menu == null)
            {
                return null;
            }

            var control = this.ribbon.Menu as Backstage ?? UIHelper.FindImmediateVisualChild<Backstage>(this.ribbon.Menu, IsVisible);

            if (control == null)
            {
                return null;
            }

            return control.IsOpen
                ? control
                : null;
        }

        private FrameworkElement GetApplicationMenu()
        {
            if (this.ribbon.Menu == null)
            {
                return null;
            }

            var control = this.ribbon.Menu as ApplicationMenu ?? UIHelper.FindImmediateVisualChild<ApplicationMenu>(this.ribbon.Menu, IsVisible);

            if (control == null)
            {
                return null;
            }

            return control.IsDropDownOpen
                ? control
                : null;
        }

        private FrameworkElement GetStartScreen()
        {
            var control = this.ribbon.StartScreen;

            if (control == null)
            {
                return null;
            }

            return control.IsOpen
                ? control
                : null;
        }

        private static bool IsVisible(FrameworkElement obj)
        {
            return obj.Visibility == Visibility.Visible;
        }
    }
}