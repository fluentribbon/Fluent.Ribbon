using System;
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
        Ribbon ribbon = null;
        // Timer to show KeyTips with delay
        DispatcherTimer timer = null;
        // Is KeyTips Actived now
        KeyTipAdorner activeAdornerChain = null;
        // This element must be remembered to restore it
        IInputElement backUpFocusedElement = null;

        #endregion

        #region Initialization
                
        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="element">Host element</param>
        public KeyTipService(Ribbon ribbon)
        {
            this.ribbon = ribbon;

            if (!ribbon.IsLoaded) ribbon.Loaded += OnDelayedInitialization;
            else Attach(ribbon);

            // Initialize timer
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.7), DispatcherPriority.SystemIdle, OnDelayedShow, Dispatcher.CurrentDispatcher);
            timer.Stop();
        }

        void OnDelayedInitialization(object sender, EventArgs args)
        {
            ribbon.Loaded -= OnDelayedInitialization;
            Attach(ribbon);
        }

        void Attach(FrameworkElement element)
        {
            Window window = GetElementWindow(ribbon);
            if (window == null) return;

            window.PreviewKeyDown += new KeyEventHandler(OnWindowPreviewKeyDown);
            window.PreviewKeyUp += new KeyEventHandler(OnWindowPreviewKeyUp);

            // Hookup non client area messages
            ((HwndSource)PresentationSource.FromVisual(window)).AddHook(WindowProc);
        }

        // Window's messages hook up
        IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Mouse clicks in non client area
            if ((msg >= 161) && (msg <= 173))
            {
                if (activeAdornerChain != null)
                {
                    activeAdornerChain.Terminate();
                    activeAdornerChain = null;
                }
            }
            return IntPtr.Zero;
        }

        void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)        
        {
            
            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) ||
                (e.SystemKey == Key.RightAlt) ||
                (e.SystemKey == Key.F10)))
            {
                e.Handled = true;
                if (e.IsRepeat) return;
                if ((activeAdornerChain == null) || (!activeAdornerChain.IsAdornerChainAlive))
                {
                    activeAdornerChain = null;
                    timer.Start();
                }
                else { activeAdornerChain.Terminate(); activeAdornerChain = null; }
            }
        }

        void OnWindowPreviewKeyUp(object sender, KeyEventArgs e)
        {            
            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) ||
                (e.SystemKey == Key.RightAlt) ||
                (e.SystemKey == Key.F10)))
            {
                e.Handled = true;

                if (timer.IsEnabled)
                {
                    timer.Stop();
                    backUpFocusedElement = Keyboard.FocusedElement;

                    // Focus ribbon
                    FocusManager.SetIsFocusScope(ribbon, true);
                    ribbon.Focusable = true;
                    ribbon.Focus();

                    Show();
                }
                else if ((activeAdornerChain != null) && (activeAdornerChain.IsAdornerChainAlive))
                {
                    // Focus ribbon
                    backUpFocusedElement = Keyboard.FocusedElement;
                    FocusManager.SetIsFocusScope(ribbon, true);
                    ribbon.Focusable = true;
                    ribbon.Focus();
                }
            }
        }

        void OnAdornerChainTerminated(object sender, EventArgs e)
        {
            ribbon.Focusable = false;
            FocusManager.SetIsFocusScope(ribbon, false);
            ((KeyTipAdorner)sender).Terminated -= OnAdornerChainTerminated;
            if (backUpFocusedElement != null) backUpFocusedElement.Focus();
        }

        void OnDelayedShow(object sender, EventArgs e)
        {
            if (activeAdornerChain == null) Show();
            timer.Stop();
        }

        void Show()
        {          
            activeAdornerChain = new KeyTipAdorner(ribbon, ribbon, null);
            activeAdornerChain.Terminated += OnAdornerChainTerminated;

            if (ribbon.IsBackstageOpen) activeAdornerChain.Forward(ribbon.BackstageKeyTipKeys, false);
            else activeAdornerChain.Attach();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets window where element is loacated or null
        /// </summary>
        /// <param name="element">Elemet</param>
        /// <returns>Window where element is loacated or null</returns>
        Window GetElementWindow(UIElement element)
        {
            while (true)
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                if (element == null) return null;
                if (element is Window) return (Window)element;
            }
        }

        #endregion
    }
}
