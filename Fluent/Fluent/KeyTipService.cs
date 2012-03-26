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
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;

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

        void OnDelayedInitialization(object sender, EventArgs args)
        {
            ribbon.Loaded -= OnDelayedInitialization;
            Attach();
        }

        /// <summary>
        /// Attaches self
        /// </summary>
        public void Attach()
        {
            if (attached) return;
            attached = true;

            // KeyTip service must not work in design mode
            if (DesignerProperties.GetIsInDesignMode(ribbon)) return;

            window = GetElementWindow(ribbon);
            if (window == null) return;

            window.KeyDown += OnWindowKeyDown;
            window.KeyUp += OnWindowKeyUp;

            // Hookup non client area messages
            attachedHwndSource = (HwndSource)PresentationSource.FromVisual(window);
            if (attachedHwndSource != null) attachedHwndSource.AddHook(WindowProc);
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
        IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
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

        void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsRepeat) return;
            timer.Stop();

            if (ribbon.IsCollapsed) return;
            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) ||
                (e.SystemKey == Key.RightAlt) ||
                (e.SystemKey == Key.F10) ||
                (e.SystemKey == Key.Space)))
            {
                if ((activeAdornerChain == null) || (!activeAdornerChain.IsAdornerChainAlive))
                {
                    activeAdornerChain = null;
                    timer.Start();
                }
                else { activeAdornerChain.Terminate(); activeAdornerChain = null; }
            }
        }

        void OnWindowKeyUp(object sender, KeyEventArgs e)
        {
            if (ribbon.IsCollapsed) return;
            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) ||
                (e.SystemKey == Key.RightAlt) ||
                (e.SystemKey == Key.F10) ||
                (e.SystemKey == Key.Space)))
            {
                e.Handled = true;
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    backUpFocusedElement = Keyboard.FocusedElement;

                    // Focus ribbon
                    ribbon.Focusable = true;
                    ribbon.Focus();

                    Show();
                }
                else if ((activeAdornerChain != null) && (activeAdornerChain.IsAdornerChainAlive))
                {
                    // Focus ribbon
                    backUpFocusedElement = Keyboard.FocusedElement;
                    ribbon.Focusable = true;
                    ribbon.Focus();
                }
            }
            else timer.Stop();
        }

        void RestoreFocuses()
        {
            if (backUpFocusedElement != null)
            {
                backUpFocusedElement.Focus();
            }
            ribbon.Focusable = false;
        }

        void OnAdornerChainTerminated(object sender, EventArgs e)
        {
            RestoreFocuses();
            ((KeyTipAdorner)sender).Terminated -= OnAdornerChainTerminated;
        }

        void OnDelayedShow(object sender, EventArgs e)
        {
            if (activeAdornerChain == null) Show();
            timer.Stop();
        }

        void Show()
        {
            // Check whether the window is still active
            // (it prevent keytips showing during Alt-Tab'ing)
            if (!window.IsActive)
            {
                RestoreFocuses();
                return;
            }

            activeAdornerChain = new KeyTipAdorner(ribbon, ribbon, null);
            activeAdornerChain.Terminated += OnAdornerChainTerminated;

            // Special behavior for backstage
            Backstage backstage = ribbon.Menu as Backstage;
            if (backstage != null && backstage.IsOpen)
            {
                string keys = KeyTip.GetKeys(backstage);
                if (!String.IsNullOrEmpty(keys)) activeAdornerChain.Forward(KeyTip.GetKeys(backstage), false);
                else activeAdornerChain.Attach();
            }
            else activeAdornerChain.Attach();
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
