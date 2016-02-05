namespace Fluent
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using Fluent.Internal;
    using Fluent.Metro.Native;

    /// <summary>
    /// Handles Alt, F10 and so on
    /// </summary>
    internal class KeyTipService
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

        private static readonly KeyConverter keyConverter = new KeyConverter();
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

        #endregion

        #region Initialization

        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="ribbon">Host element</param>
        public KeyTipService(Ribbon ribbon)
        {
            this.ribbon = ribbon;

            if (this.ribbon.IsLoaded == false)
            {
                this.ribbon.Loaded += this.OnDelayedInitialization;
            }
            else
            {
                this.Attach();
            }

            // Initialize timer
            this.timer = new DispatcherTimer(TimeSpan.FromSeconds(0.7), DispatcherPriority.SystemIdle, this.OnDelayedShow, Dispatcher.CurrentDispatcher);
            this.timer.Stop();
        }

        private void OnDelayedInitialization(object sender, EventArgs args)
        {
            this.ribbon.Loaded -= this.OnDelayedInitialization;
            this.Attach();
        }

        #endregion

        /// <summary>
        /// Attaches self
        /// </summary>
        public void Attach()
        {
            if (this.attached)
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
            // We must terminate the keytip's adorner chain if:                        
            if (msg == Constants.WM_NCACTIVATE // mouse clicks in non client area
                || (msg == Constants.WM_ACTIVATE && wParam == IntPtr.Zero) // the window is deactivated
                                                                           // >= WM_NCLBUTTONDOWN <= WM_NCXBUTTONDBLCLK
                || (msg >= 161 && msg <= 173) // mouse click (non client area)
                || (msg >= 513 && msg <= 521) // mouse click
                )
            {
                if (this.activeAdornerChain != null
                    && this.activeAdornerChain.IsAdornerChainAlive)
                {
                    this.activeAdornerChain.Terminate();
                }
            }

            return IntPtr.Zero;
        }

        private void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }

            if (this.ribbon.IsCollapsed
                || this.ribbon.IsEnabled == false)
            {
                return;
            }

            // Keytips should be cancelled if Alt+Num0 is pressed #241.
            // This allows entering special keys via numpad.
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Alt
                && e.SystemKey >= Key.NumPad0
                && e.SystemKey <= Key.NumPad9)
            {
                this.activeAdornerChain?.Terminate();
                this.ClearUserInput();
                return;
            }

            if (IsShowOrHideKey(e))
            {
                if (this.activeAdornerChain == null
                    || this.activeAdornerChain.IsAdornerChainAlive == false
                    || this.activeAdornerChain.AreAnyKeyTipsVisible == false)
                {
                    this.ShowDelayed();
                }
                else if (this.activeAdornerChain != null
                    && this.activeAdornerChain.IsAdornerChainAlive)
                {
                    this.activeAdornerChain.Terminate();
                }
                else
                {
                    this.ClearUserInput();
                }
            }
            else if (e.Key == Key.Escape
                && this.activeAdornerChain != null)
            {
                this.activeAdornerChain.ActiveKeyTipAdorner.Back();
                this.ClearUserInput();
                e.Handled = true;
                return;
            }
            else
            {
                if ((e.Key != Key.System && this.activeAdornerChain == null)
                    || e.SystemKey == Key.Escape
                    || (e.KeyboardDevice.Modifiers != ModifierKeys.Alt && this.activeAdornerChain == null))
                {
                    return;
                }

                var actualKey = e.Key == Key.System ? e.SystemKey : e.Key;
                var isKeyRealInput = ((actualKey >= Key.A && actualKey <= Key.Z) || (actualKey >= Key.D0 && actualKey <= Key.D9) || (actualKey >= Key.NumPad0 && actualKey <= Key.NumPad9));

                // Don't do anything and let WPF handle the rest
                if (isKeyRealInput == false)
                {
                    return;
                }

                // Should we show the keytips and immediately react to key?
                if (this.activeAdornerChain == null
                    || this.activeAdornerChain.IsAdornerChainAlive == false
                    || this.activeAdornerChain.AreAnyKeyTipsVisible == false)
                {
                    this.ShowImmediatly();
                }

                if (this.activeAdornerChain == null)
                {
                    return;
                }

                var previousInput = this.currentUserInput;
                this.currentUserInput += keyConverter.ConvertToString(actualKey);

                // If no key tips match the current input, continue with the previously entered and still correct keys.
                if (this.activeAdornerChain.ActiveKeyTipAdorner.ContainsKeyTipStartingWith(this.currentUserInput) == false)
                {
                    this.currentUserInput = previousInput;
                    System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                    return;
                }
                else if (this.activeAdornerChain.ActiveKeyTipAdorner.Forward(this.currentUserInput, true))
                {
                    this.ClearUserInput();
                    e.Handled = true;
                    return;
                }
                else
                {
                    this.activeAdornerChain.ActiveKeyTipAdorner.FilterKeyTips(this.currentUserInput);
                    e.Handled = true;
                    return;
                }
            }
        }

        private void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (this.ribbon.IsCollapsed
                || this.ribbon.IsEnabled == false)
            {
                return;
            }

            if (IsShowOrHideKey(e))
            {
                this.ClearUserInput();

                e.Handled = true;

                if (this.timer.IsEnabled)
                {
                    this.ShowImmediatly();
                }
            }
            else
            {
                this.timer.Stop();
            }
        }

        private static bool IsShowOrHideKey(KeyEventArgs e)
        {
            return e.Key == Key.System
                && (e.SystemKey == Key.LeftAlt
                   || e.SystemKey == Key.RightAlt
                   || e.SystemKey == Key.F10
                   || e.SystemKey == Key.Space);
        }

        private void ClearUserInput()
        {
            this.currentUserInput = string.Empty;
        }

        private void RestoreFocus()
        {
            this.backUpFocusedControl?.Focus();
            this.backUpFocusedControl = null;
        }

        private void OnAdornerChainTerminated(object sender, EventArgs e)
        {
            this.activeAdornerChain.Terminated -= this.OnAdornerChainTerminated;
            this.activeAdornerChain = null;
            this.RestoreFocus();
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
            this.activeAdornerChain?.Terminate();

            this.timer.Start();
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

            this.backUpFocusedControl = FocusWrapper.GetWrapperForCurrentFocus();

            // Focus ribbon
            this.ribbon.Focus();

            this.ClearUserInput();

            this.activeAdornerChain = new KeyTipAdorner(this.ribbon, this.ribbon, null);
            this.activeAdornerChain.Terminated += this.OnAdornerChainTerminated;

            // Special behavior for backstage
            var specialControl = this.GetBackstage()
                ?? (DependencyObject)this.GetApplicationMenu();

            if (specialControl != null)
            {
                this.DirectlyForwardToSpecialControl(specialControl);
            }
            else
            {
                this.activeAdornerChain.Attach();
            }
        }

        private Backstage GetBackstage()
        {
            if (this.ribbon.Menu == null)
            {
                return null;
            }

            var control = this.ribbon.Menu as Backstage ?? UIHelper.FindImmediateVisualChild<Backstage>(this.ribbon.Menu, obj => obj.Visibility == Visibility.Visible && obj.IsOpen);

            if (control == null)
            {
                return null;
            }

            return control.IsOpen
                ? control
                : null;
        }

        private ApplicationMenu GetApplicationMenu()
        {
            if (this.ribbon.Menu == null)
            {
                return null;
            }

            var control = this.ribbon.Menu as ApplicationMenu ?? UIHelper.FindImmediateVisualChild<ApplicationMenu>(this.ribbon.Menu, obj => obj.Visibility == Visibility.Visible);

            if (control == null)
            {
                return null;
            }

            return control.IsDropDownOpen
                ? control
                : null;
        }

        private void DirectlyForwardToSpecialControl(DependencyObject specialControl)
        {
            var keys = KeyTip.GetKeys(specialControl);

            if (string.IsNullOrEmpty(keys) == false)
            {
                this.activeAdornerChain.Forward(keys, false);
            }
            else
            {
                this.activeAdornerChain.Attach();
            }
        }
    }
}