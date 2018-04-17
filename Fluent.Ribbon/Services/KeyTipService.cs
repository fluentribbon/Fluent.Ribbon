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

        private string currentUserInput;

        // List of invalid key tip keys
        private static KeyGesture[] invalidGestures = new KeyGesture[] { new KeyGesture(Key.F10, ModifierKeys.Shift) };

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

        // List of key tip activation keys
        public IList<KeyGesture> KeyTipKeys { get; } = new List<KeyGesture>();

        #endregion

        #region Initialization

        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="ribbon">Host element</param>
        public KeyTipService(Ribbon ribbon)
        {
            if (ribbon == null)
            {
                throw new ArgumentNullException(nameof(ribbon));
            }

            this.ribbon = ribbon;

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
            // We must terminate the keytip's adorner chain if:
            if (msg == (int)WM.NCACTIVATE // mouse clicks in non client area
                || (msg == (int)WM.ACTIVATE && wParam == IntPtr.Zero) // the window is deactivated
                                                                      // >= WM_NCLBUTTONDOWN <= WM_NCXBUTTONDBLCLK
                || (msg >= 161 && msg <= 173) // mouse click (non client area)
                || (msg >= 513 && msg <= 521)) // mouse click
            {
                if (this.activeAdornerChain != null
                    && this.activeAdornerChain.IsAdornerChainAlive)
                {
                    this.Terminate();
                }
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
            else
            {
                if ((e.Key != Key.System && this.activeAdornerChain == null)
                    || e.SystemKey == Key.Escape
                    || (e.KeyboardDevice.Modifiers != ModifierKeys.Alt && this.activeAdornerChain == null))
                {
                    return;
                }

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

        private bool IsShowOrHideKey(KeyEventArgs e)
        {
            var actualKey = e.Key == Key.System ? e.SystemKey : e.Key;

            if (invalidGestures.Any(x => x.Key == actualKey && x.Modifiers == Keyboard.Modifiers))
            {
                return false;
            }

            // Use default keys if no keys are defined.
            var keys = this.KeyTipKeys.Any() ? this.KeyTipKeys : DefaultKeys;

            return keys.Any(x => KeyIsValid(x, actualKey, Keyboard.Modifiers));
        }

        private static bool KeyIsValid(KeyGesture gesture, Key key, ModifierKeys modifiers)
        {
            // If the key is a modifier key, the modifier either needs to match or needs to be empty.
            if (key == Key.LeftAlt || key == Key.RightAlt)
            {
                return gesture.Key == key && (modifiers == ModifierKeys.Alt || modifiers == ModifierKeys.None);
            }
            else if (key == Key.LeftCtrl || key == Key.RightCtrl)
            {
                return gesture.Key == key && (modifiers == ModifierKeys.Control || modifiers == ModifierKeys.None);
            }
            else if (key == Key.LeftShift || key == Key.RightShift)
            {
                return gesture.Key == key && (modifiers == ModifierKeys.Shift || modifiers == ModifierKeys.None);
            }
            else if (key == Key.LWin || key == Key.RWin)
            {
                return gesture.Key == key && (modifiers == ModifierKeys.Windows || modifiers == ModifierKeys.None);
            }

            // For all other keys we do a normal check
            return gesture.Key == key && gesture.Modifiers == modifiers;
        }

        /// <summary>
        /// The default keys used to activate key tips.
        /// </summary>
        public static IList<KeyGesture> DefaultKeys => new List<KeyGesture>
        {
            new KeyGesture(Key.LeftAlt),
            new KeyGesture(Key.RightAlt),
            new KeyGesture(Key.F10)
        };

        private void ClearUserInput()
        {
            this.currentUserInput = string.Empty;
        }

        private void ClosePopups()
        {
            PopupService.RaiseDismissPopupEvent(Keyboard.FocusedElement, DismissPopupMode.Always);
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

            this.ClosePopups();

            // If focus is inside the Ribbon already we don't want to jump around after finishing with KeyTips
            if (UIHelper.GetParent<Ribbon>(Keyboard.FocusedElement as DependencyObject) == null)
            {
                this.backUpFocusedControl = FocusWrapper.GetWrapperForCurrentFocus();
            }

            // Focus ribbon
            this.ribbon.SelectedTabItem?.Focus();

            this.ClearUserInput();

            this.activeAdornerChain = new KeyTipAdorner(this.ribbon, this.ribbon, null);
            this.activeAdornerChain.Terminated += this.OnAdornerChainTerminated;

            // Special behavior for backstage
            var specialControl = this.GetBackstage()
                ?? this.GetApplicationMenu()
                ?? this.GetStartScreen();

            if (specialControl != null)
            {
                this.DirectlyForwardToSpecialControl(specialControl);
            }
            else
            {
                this.activeAdornerChain.Attach();
            }
        }

        private DependencyObject GetBackstage()
        {
            if (this.ribbon.Menu == null)
            {
                return null;
            }

            var control = this.ribbon.Menu as Backstage ?? UIHelper.FindImmediateVisualChild<Backstage>(this.ribbon.Menu, obj => obj.Visibility == Visibility.Visible);

            if (control == null)
            {
                return null;
            }

            return control.IsOpen
                ? control
                : null;
        }

        private DependencyObject GetApplicationMenu()
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

        private DependencyObject GetStartScreen()
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