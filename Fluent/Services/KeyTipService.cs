#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright ?Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

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
        // This element must be remembered to restore it
        IInputElement backUpFocusedElement;
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
                if (activeAdornerChain != null)
                {
                    return activeAdornerChain.AreAnyKeyTipsVisible;
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

            this.window.PreviewKeyDown += this.OnWindowKeyDown;
            this.window.KeyUp += this.OnWindowKeyUp;

            // Hookup non client area messages
            this.attachedHwndSource = (HwndSource)PresentationSource.FromVisual(this.window);
            if (this.attachedHwndSource != null)
            {
                this.attachedHwndSource.AddHook(this.WindowProc);
            }
        }

        /// <summary>
        /// Detachs self
        /// </summary>
        public void Detach()
        {
            if (!this.attached)
            {
                return;
            }

            this.attached = false;

            if (this.window != null)
            {
                this.window.PreviewKeyDown -= this.OnWindowKeyDown;
                this.window.KeyUp -= this.OnWindowKeyUp;

                this.window = null;
            }

            // Hookup non client area messages
            if (this.attachedHwndSource != null
                && this.attachedHwndSource.IsDisposed == false)
            {
                this.attachedHwndSource.RemoveHook(this.WindowProc);
            }
        }

        // Window's messages hook up
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // We must terminate the keytip's adorner chain if:
            // - mouse clicks in non client area
            // - the window is deactivated
            if (((msg >= 161) && (msg <= 173)) || msg == Constants.WM_NCACTIVATE)
            {
                if (this.activeAdornerChain != null
                    && this.activeAdornerChain.IsAdornerChainAlive)
                {
                    this.activeAdornerChain.Terminate();
                    this.activeAdornerChain = null;
                }
            }

            return IntPtr.Zero;
        }

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat)
            {
                return;
            }

            if (this.ribbon.IsCollapsed)
            {
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
                    // Focus ribbon
                    this.backUpFocusedElement = Keyboard.FocusedElement;
                    this.ribbon.Focusable = true;
                    //this.ribbon.Focus();

                    this.activeAdornerChain.Terminate();
                    this.activeAdornerChain = null;
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
            }
            else
            {
                // Should we show the keytips and immediately react to key?
                if (e.Key != Key.System
                    || e.SystemKey == Key.Escape
                    || e.KeyboardDevice.Modifiers != ModifierKeys.Alt)
                {
                    return;
                }

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

                this.currentUserInput += keyConverter.ConvertToString(e.SystemKey);

                if (this.activeAdornerChain.ActiveKeyTipAdorner.Forward(this.currentUserInput, true))
                {
                    this.ClearUserInput();
                    e.Handled = true;
                }
            }
        }

        private void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (this.ribbon.IsCollapsed)
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

        private void RestoreFocuses()
        {
            if (this.backUpFocusedElement != null)
            {
                this.backUpFocusedElement.Focus();
                this.backUpFocusedElement = null; // Release the reference, so GC can work
            }

            this.ribbon.Focusable = false;
        }

        private void OnAdornerChainTerminated(object sender, EventArgs e)
        {
            this.RestoreFocuses();
            ((KeyTipAdorner)sender).Terminated -= this.OnAdornerChainTerminated;
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
            this.timer.Stop();
            this.backUpFocusedElement = Keyboard.FocusedElement;

            // Focus ribbon
            this.ribbon.Focusable = true;
            //this.ribbon.Focus();

            this.Show();
        }

        private void ShowDelayed()
        {
            if (this.activeAdornerChain != null)
            {
                this.activeAdornerChain.Terminate();
            }

            this.activeAdornerChain = null;
            this.timer.Start();
        }

        private void Show()
        {
            // Check whether the window is still active
            // (it prevent keytips showing during Alt-Tab'ing)
            if (this.window.IsActive == false)
            {
                this.RestoreFocuses();
                return;
            }

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
                this.activeAdornerChain.Forward(KeyTip.GetKeys(specialControl), false);
            }
            else
            {
                this.activeAdornerChain.Attach();
            }
        }
    }
}