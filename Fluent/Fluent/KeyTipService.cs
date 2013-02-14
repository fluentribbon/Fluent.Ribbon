#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Handles Alt, F10 and so on
    /// </summary>
    internal class KeyTipService
    {
        #region Fields

        // Host element, usually this is Ribbon
        Ribbon ribbon;
        // Timer to show KeyTips with delay
        DispatcherTimer timer;
        // Is KeyTips Actived now
        KeyTipAdorner activeAdornerChain;
        // This element must be remembered to restore it
        IInputElement backUpFocusedElement;
        // Window where we attached
        Window window;
        // Whether we attached to window
        bool attached;

        // Attached HWND source
        HwndSource attachedHwndSource;

        private KeyConverter keyConverter = new KeyConverter();
        private string currentUserInput;

        #endregion

        #region Initialization

        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="ribbon">Host element</param>
        public KeyTipService(Ribbon ribbon)
        {
            this.ribbon = ribbon;

            if (!ribbon.IsLoaded) ribbon.Loaded += OnDelayedInitialization;
            else Attach();

            // Initialize timer
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.7), DispatcherPriority.SystemIdle, OnDelayedShow, Dispatcher.CurrentDispatcher);
            timer.Stop();
        }

        private void OnDelayedInitialization(object sender, EventArgs args)
        {
            ribbon.Loaded -= OnDelayedInitialization;
            Attach();
        }

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

            this.window = GetElementWindow(this.ribbon);
            if (this.window == null)
            {
                return;
            }

            this.window.KeyDown += this.OnWindowKeyDown;
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
            if (!attached) return;
            attached = false;

            if (window != null)
            {
                window.KeyDown -= OnWindowKeyDown;
                window.KeyUp -= OnWindowKeyUp;

                this.window = null;
            }

            // Hookup non client area messages
            if ((attachedHwndSource != null) && (!attachedHwndSource.IsDisposed))
                attachedHwndSource.RemoveHook(WindowProc);
        }

        // Window's messages hook up
        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // We must terminate the keytip's adorner chain if:
            // - mouse clicks in non client area
            // - the window is deactivated
            if (((msg >= 161) && (msg <= 173)) || msg == NativeMethods.WM_NCACTIVATE)
            {
                if ((activeAdornerChain != null) && (activeAdornerChain.IsAdornerChainAlive))
                {
                    activeAdornerChain.Terminate();
                    activeAdornerChain = null;
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

            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) ||
                (e.SystemKey == Key.RightAlt) ||
                (e.SystemKey == Key.F10) ||
                (e.SystemKey == Key.Space)))
            {
                if (this.activeAdornerChain == null || (!this.activeAdornerChain.IsAdornerChainAlive || !this.activeAdornerChain.AreAnyKeyTipsVisible))
                {
                    if (this.activeAdornerChain != null)
                    {
                        this.activeAdornerChain.Terminate();
                    }

                    this.activeAdornerChain = null;
                    this.timer.Start();
                }
                else
                {
                    this.currentUserInput = string.Empty;
                    this.timer.Stop();
                }
            }
            else
            {
                if (e.Key == Key.System
                    && e.SystemKey != Key.Escape
                    && e.KeyboardDevice.Modifiers == ModifierKeys.Alt)
                {
                    if (this.activeAdornerChain == null || (!this.activeAdornerChain.IsAdornerChainAlive || !this.activeAdornerChain.AreAnyKeyTipsVisible))
                    {
                        this.timer.Stop();
                        this.backUpFocusedElement = Keyboard.FocusedElement;

                        // Focus ribbon
                        this.ribbon.Focusable = true;
                        this.ribbon.Focus();

                        this.Show();
                    }

                    if (this.activeAdornerChain != null)
                    {
                        this.currentUserInput += this.keyConverter.ConvertToString(e.SystemKey);

                        if (this.activeAdornerChain.ActiveKeyTipAdorner.Forward(this.currentUserInput, true))
                        {
                            this.currentUserInput = string.Empty;
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (this.ribbon.IsCollapsed)
            {
                return;
            }

            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) ||
                 (e.SystemKey == Key.RightAlt) ||
                 (e.SystemKey == Key.F10) ||
                 (e.SystemKey == Key.Space)))
            {
                this.currentUserInput = string.Empty;

                e.Handled = true;

                if (this.timer.IsEnabled)
                {
                    this.timer.Stop();
                    this.backUpFocusedElement = Keyboard.FocusedElement;

                    // Focus ribbon
                    this.ribbon.Focusable = true;
                    this.ribbon.Focus();

                    this.Show();
                }
                else if (this.activeAdornerChain != null && this.activeAdornerChain.IsAdornerChainAlive)
                {
                    // Focus ribbon
                    this.backUpFocusedElement = Keyboard.FocusedElement;
                    this.ribbon.Focusable = true;
                    this.ribbon.Focus();

                    this.activeAdornerChain.Terminate();
                    this.activeAdornerChain = null;
                }
            }
            else
            {
                this.timer.Stop();
            }
        }

        private void RestoreFocuses()
        {
            if (this.backUpFocusedElement != null)
            {
                this.backUpFocusedElement.Focus();
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
            if (activeAdornerChain == null)
            {
                this.Show();
            }

            this.timer.Stop();
        }

        private void Show()
        {
            // Check whether the window is still active
            // (it prevent keytips showing during Alt-Tab'ing)
            if (!this.window.IsActive)
            {
                this.RestoreFocuses();
                return;
            }

            this.currentUserInput = string.Empty;

            this.activeAdornerChain = new KeyTipAdorner(this.ribbon, this.ribbon, null);
            this.activeAdornerChain.Terminated += this.OnAdornerChainTerminated;

            // Special behavior for backstage
            var backstage = this.ribbon.Menu as Backstage;
            if (backstage != null && backstage.IsOpen)
            {
                var keys = KeyTip.GetKeys(backstage);
                if (!String.IsNullOrEmpty(keys))
                {
                    this.activeAdornerChain.Forward(KeyTip.GetKeys(backstage), false);
                }
                else
                {
                    this.activeAdornerChain.Attach();
                }
            }
            else
            {
                this.activeAdornerChain.Attach();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets window where element is loacated or null
        /// </summary>
        /// <param name="element">Elemet</param>
        /// <returns>Window where element is loacated or null</returns>
        static Window GetElementWindow(UIElement element)
        {
            while (true)
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                if (element == null) return null;
                Window window = element as Window;
                if (window != null) return window;
            }
        }

        #endregion
    }
}