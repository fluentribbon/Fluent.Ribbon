using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Dismiss popup mode
    /// </summary>
    public enum DismissPopupMode
    {
        /// <summary>
        /// Always dismiss popup
        /// </summary>
        Always,
        /// <summary>
        /// Dismiss only if mouse is not over popup
        /// </summary>
        MouseNotOver
    }

    /// <summary>
    /// Dismiss popup handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DismissPopupEventHandler(object sender, DismissPopupEventArgs e);

    /// <summary>
    /// Dismiss popup arguments
    /// </summary>
    public class DismissPopupEventArgs : RoutedEventArgs
    {
        #region Properties
        /// <summary>
        /// Popup dismiss mode
        /// </summary>
        public DismissPopupMode DismissMode { get; set; }

        #endregion

        /// <summary>
        /// Standard constructor
        /// </summary>
        public DismissPopupEventArgs()
            : this(DismissPopupMode.Always)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dismissMode">Dismiss mode</param>
        public DismissPopupEventArgs(DismissPopupMode dismissMode)
        {
            base.RoutedEvent = PopupService.DismissPopupEvent;
            this.DismissMode = dismissMode;
        }

        /// <summary>
        /// When overridden in a derived class, provides a way to invoke event handlers in a type-specific way, which can increase efficiency over the base implementation.
        /// </summary>
        /// <param name="genericHandler">The generic handler / delegate implementation to be invoked.</param><param name="genericTarget">The target on which the provided handler should be invoked.</param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            DismissPopupEventHandler handler = (DismissPopupEventHandler)genericHandler;
            handler(genericTarget, this);
        }
    }

    /// <summary>
    /// Represent additional popup functionality
    /// </summary>
    public static class PopupService
    {
        #region DismissPopup

        /// <summary>
        /// Occurs then popup is dismissed
        /// </summary>
        public static readonly RoutedEvent DismissPopupEvent = EventManager.RegisterRoutedEvent("DismissPopup", RoutingStrategy.Bubble, typeof(DismissPopupEventHandler), typeof(PopupService));

        /// <summary>
        /// Raises DismissPopup event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mode"></param>
        public static void RaiseDismissPopupEvent(object sender, DismissPopupMode mode)
        {
            object element = sender;
            /*while (!(element is IDropDownControl))
            {
                object parent = VisualTreeHelper.GetParent(element as DependencyObject);
                if (parent is IDropDownControl)
                {
                    element = parent;
                    break;
                }
                else if (parent != null) element = parent;
                else
                {
                    parent = LogicalTreeHelper.GetParent(element as DependencyObject);
                    if (parent != null)
                    {
                        element = parent;
                        if (parent is IDropDownControl) break;
                    }
                    else break;                    
                }
            }
            if (element != null) */(element as UIElement).RaiseEvent(new DismissPopupEventArgs(mode));
        }

        #endregion

        /// <summary>
        /// Set needed parameters to control
        /// </summary>
        /// <param name="classType">Control type</param>
        public static void Attach(Type classType)
        {
            EventManager.RegisterClassHandler(classType, Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(PopupService.OnClickThroughThunk));
            EventManager.RegisterClassHandler(classType, PopupService.DismissPopupEvent, new DismissPopupEventHandler(PopupService.OnDismissPopup));
            EventManager.RegisterClassHandler(classType, FrameworkElement.ContextMenuOpeningEvent, new ContextMenuEventHandler(PopupService.OnContextMenuOpened), true);
            EventManager.RegisterClassHandler(classType, FrameworkElement.ContextMenuClosingEvent, new ContextMenuEventHandler(PopupService.OnContextMenuClosed), true);
            EventManager.RegisterClassHandler(classType, FrameworkElement.LostMouseCaptureEvent, new MouseEventHandler(PopupService.OnLostMouseCapture));
        }
        /// <summary>
        /// Handles PreviewMouseDownOutsideCapturedElementEvent event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnClickThroughThunk(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                if (Mouse.Captured == sender)
                {
                    RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                }
            }
        }

        /// <summary>
        /// Handles lost mouse capture event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            //Debug.WriteLine("Lost Capture - " + Mouse.Captured);
            IDropDownControl control = sender as IDropDownControl;
            if (control == null) return;
            if ((Mouse.Captured != sender) && (control.IsDropDownOpen) && (!control.IsContextMenuOpened))
            {
                Popup popup = control.DropDownPopup;
                if ((popup == null) || (popup.Child == null))
                {
                    RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                    return;
                }

                if (e.OriginalSource == sender)
                {
                    // If Ribbon loses capture because something outside popup is clicked - close the popup
                    if (Mouse.Captured == null || !IsAncestorOf(popup.Child, Mouse.Captured as DependencyObject))
                    {
                        RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                    }
                }
                else
                {
                    // If control inside Ribbon loses capture - restore capture to Ribbon
                    if (IsAncestorOf(popup.Child, e.OriginalSource as DependencyObject))
                    {
                        if (Mouse.Captured == null)
                        {
                            Mouse.Capture(sender as IInputElement, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true whether parent is ancestor of element
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="element">Element</param>
        /// <returns>Returns true whether parent is ancestor of element</returns>
        public static bool IsAncestorOf(DependencyObject parent, DependencyObject element)
        {
            while (element != null)
            {
                if (element == parent) return true;
                DependencyObject elementParent = VisualTreeHelper.GetParent(element);
                if (elementParent == null) element = LogicalTreeHelper.GetParent(element);
                else element = elementParent;
            }
            return false;
        }

        /// <summary>
        /// Handles dismiss popup event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnDismissPopup(object sender, DismissPopupEventArgs e)
        {
            IDropDownControl control = sender as IDropDownControl;
            if (control == null) return;
            if (e.DismissMode == DismissPopupMode.Always)
            {
                if (Mouse.Captured == control) Mouse.Capture(null);
               // Debug.WriteLine("DropDown Closed");
                control.IsDropDownOpen = false;
            }
            else
            {
                if ((control.IsDropDownOpen)&&(!PopupService.IsMousePhysicallyOver(control.DropDownPopup.Child)))
                {
                    if (Mouse.Captured == control) Mouse.Capture(null);
                   // Debug.WriteLine("DropDown Closed");
                    control.IsDropDownOpen = false;
                }
                else
                {
                    if ((control.IsDropDownOpen) && (Mouse.Captured != control)) Mouse.Capture(sender as IInputElement, CaptureMode.SubTree);
                    if (control.IsDropDownOpen) e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Returns true whether mouse is physically over the element 
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Returns true whether mouse is physically over the element</returns>
        public static bool IsMousePhysicallyOver(UIElement element)
        {
            if (element == null)
            {
                return false;
            }
            Point position = Mouse.GetPosition(element);
            return ((position.X >= 0.0) && (position.Y >= 0.0)) && ((position.X <= element.RenderSize.Width) && (position.Y <= element.RenderSize.Height));
        }

        /// <summary>
        /// Handles context menu opened event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnContextMenuOpened(object sender, ContextMenuEventArgs e)
        {
            IDropDownControl control = sender as IDropDownControl;
            if (control != null)
            {
                control.IsContextMenuOpened = true;
               // Debug.WriteLine("Context menu opened");
            }
        }

        /// <summary>
        /// Handles context menu closed event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnContextMenuClosed(object sender, ContextMenuEventArgs e)
        {
            IDropDownControl control = sender as IDropDownControl;
            if (control != null)
            {
                //Debug.WriteLine("Context menu closed");
                control.IsContextMenuOpened = false;
                RaiseDismissPopupEvent(control, DismissPopupMode.MouseNotOver);
            }
        }
    }
}
