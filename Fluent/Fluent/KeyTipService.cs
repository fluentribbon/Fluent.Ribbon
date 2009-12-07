using System;
using System.Globalization;
using System.Windows;
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
        FrameworkElement host = null;
        // Timer to show KeyTips with delay
        DispatcherTimer timer = null;
        // Is KeyTips Actived now
        KeyTipAdorner activeAdornerChain = null;

        #endregion

        #region Initialization
                
        /// <summary>
        /// Default constrctor
        /// </summary>
        /// <param name="element">Host element</param>
        public KeyTipService(FrameworkElement element)
        {
            host = element;

            if (!host.IsLoaded) host.Loaded += OnDelayedInitialization;
            else Attach(host);

            // Initialize timer
            timer = new DispatcherTimer(TimeSpan.FromSeconds(0.7), DispatcherPriority.SystemIdle, OnDelayedShow, Dispatcher.CurrentDispatcher);
            timer.Stop();
        }

        void OnDelayedInitialization(object sender, EventArgs args)
        {
            host.Loaded -= OnDelayedInitialization;
            Attach(host);
        }

        void Attach(FrameworkElement element)
        {
            Window window = GetElementWindow(host);
            if (window == null) return;

            window.PreviewKeyDown += new KeyEventHandler(OnWindowPreviewKeyDown);
            window.PreviewKeyUp += new KeyEventHandler(OnWindowPreviewKeyUp);
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
                else { activeAdornerChain.Detach(); activeAdornerChain = null; }
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
                    activeAdornerChain = new KeyTipAdorner(host, host, null);
                    activeAdornerChain.Attach();
                }
            }
        }

        void OnDelayedShow(object sender, EventArgs e)
        {
            if (activeAdornerChain == null)
            {
                activeAdornerChain = new KeyTipAdorner(host, host, null);
                activeAdornerChain.Attach();
            }
            timer.Stop();
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
