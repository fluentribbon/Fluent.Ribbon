using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents popup. This popup has Microsoft Office behavior
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1049")]
    public class RibbonPopup:Popup
    {
        #region Fields

        // Designates that next deactivate should be skipped
        private bool ignoreNextDeactivate;
        // Backup current active window to setup 
        // it when this popup will be closed
        [SuppressMessage("Microsoft.Reliability", "CA2006")]
        IntPtr previousActiveWindowHwnd = IntPtr.Zero;
        // Current HwndSource of this Popup
        HwndSource hwndSource;
        // Previous focused element
        IInputElement previousFocusedElement;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether next deactivate should be skipped
        /// </summary>
        internal bool IgnoreNextDeactivate
        {
            get { return ignoreNextDeactivate; }
            set
            {
                ignoreNextDeactivate = value;                
            }
        }
        /// <summary>
        /// Gets or sets parent popup
        /// </summary>
        internal RibbonPopup ParentPopup { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonPopup()
        {
            IgnoreNextDeactivate = false;            
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Responds to the condition in which the value of the Popup.IsOpen property 
        /// changes from false to true.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnOpened(EventArgs e)
        {            
            PopupAnimation = PopupAnimation.None;

            hwndSource = (HwndSource)PresentationSource.FromVisual(this.Child);
            if (hwndSource != null) hwndSource.AddHook(WindowProc);

            ParentPopup = FindParentPopup();
            if((ParentPopup!=null) &&(ParentPopup.IsOpen))
                ParentPopup.IgnoreNextDeactivate = true;

            // Backup previous active window and set popup's window as active
            previousActiveWindowHwnd = NativeMethods.GetActiveWindow();
            previousFocusedElement = Keyboard.FocusedElement;
            Activate();
        }
        
        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseDown attached event reaches
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            IntPtr hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
            NativeMethods.SetActiveWindow(hwnd);
            base.OnMouseDown(e);
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
        }

        /// <summary>
        /// Responds when the value of the Popup.IsOpen property changes from to true to false.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            PopupAnimation = PopupAnimation.None;

            // Restore active window and focus
            NativeMethods.SetActiveWindow(previousActiveWindowHwnd);
            Keyboard.Focus(previousFocusedElement);

            // Remove hook
            if ((hwndSource != null) && (!hwndSource.IsDisposed))
            {
                hwndSource.RemoveHook(WindowProc);
                hwndSource = null;
            }
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyDown attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;                
                if (ParentPopup != null) ParentPopup.IgnoreNextDeactivate = true;
                IsOpen = false;
                return;
            }
            if ((e.Key==Key.System)&&((e.SystemKey == Key.LeftAlt)||(e.SystemKey == Key.RightAlt)||(e.SystemKey == Key.F10)))
            {
                if (e.SystemKey != Key.F10)
                {
                    ClosePopup(IntPtr.Zero, IntPtr.Zero);
                }
                else e.Handled = true;
                return;
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region Private methods

        internal void Activate()
        {
            if(hwndSource!=null)NativeMethods.SetActiveWindow(hwndSource.Handle);
        }

        /// <summary>
        /// Find parent popup
        /// </summary>
        /// <returns>Parent popup or null if not finded</returns>
        private RibbonPopup FindParentPopup()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                RibbonPopup ribbonPopup = element as RibbonPopup;
                if (ribbonPopup != null) return ribbonPopup;
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element as DependencyObject);
                if(parent!=null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element as DependencyObject);
            }
            return null;
        }

        /// <summary>
        /// Closes popup
        /// </summary>
        /// <param name="wParam">wParam of windows message</param>
        /// <param name="lParam">lParam of windows message</param>
        private void ClosePopup(IntPtr wParam, IntPtr lParam)
        {
            if (hwndSource == null) return;
            IntPtr hwnd = hwndSource.Handle;
            if ((lParam != hwnd) && (!IgnoreNextDeactivate))
            {                                    
                PopupAnimation = PopupAnimation.Fade;
                IsOpen = false;
                if ((ParentPopup != null) && (ParentPopup.IsOpen))
                {
                    ParentPopup.ClosePopup(wParam, lParam);
                }
            }
            else IgnoreNextDeactivate = false;                
                        
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
        private IntPtr WindowProc(
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
                            ClosePopup(wParam, lParam);                     
                        }
                        else
                        {
                            if (ParentPopup == null)
                            {
                                IntPtr parentHwnd = previousActiveWindowHwnd;// (new WindowInteropHelper(Window.GetWindow(this))).Handle;
                                NativeMethods.SendMessage(parentHwnd, 0x0086, new IntPtr(1), IntPtr.Zero);                                
                            }
                        }
                        handled = true;
                        break;
                    }
                case 0x0021/*WM_MOUSEACTIVATE*/:
                    {
                        if (ParentPopup == null)
                        {
                            IntPtr parentHwnd = previousActiveWindowHwnd;// (new WindowInteropHelper(Window.GetWindow(this))).Handle;
                            NativeMethods.SendMessage(parentHwnd, 0x0086, new IntPtr(1), IntPtr.Zero);
                            handled = true;
                        }
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        

        #endregion
    }
}
