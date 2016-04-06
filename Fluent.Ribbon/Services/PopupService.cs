namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

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
            this.RoutedEvent = PopupService.DismissPopupEvent;
            this.DismissMode = dismissMode;
        }

        /// <summary>
        /// When overridden in a derived class, provides a way to invoke event handlers in a type-specific way, which can increase efficiency over the base implementation.
        /// </summary>
        /// <param name="genericHandler">The generic handler / delegate implementation to be invoked.</param><param name="genericTarget">The target on which the provided handler should be invoked.</param>
        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            var handler = (DismissPopupEventHandler)genericHandler;
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
        /// Raises DismissPopup event (Async)
        /// </summary>
        public static void RaiseDismissPopupEventAsync(object sender, DismissPopupMode mode)
        {
            var element = sender as UIElement;

            if (element == null)
            {
                return;
            }

            Debug.WriteLine("Dismissing Popup (async)");

            element.Dispatcher.BeginInvoke((Action)(() => RaiseDismissPopupEvent(sender, mode)));
        }

        /// <summary>
        /// Raises DismissPopup event
        /// </summary>
        public static void RaiseDismissPopupEvent(object sender, DismissPopupMode mode)
        {
            var element = sender as UIElement;

            if (element == null)
            {
                return;
            }

            Debug.WriteLine("Dismissing Popup");

            element.RaiseEvent(new DismissPopupEventArgs(mode));
        }

        #endregion

        /// <summary>
        /// Set needed parameters to control
        /// </summary>
        /// <param name="classType">Control type</param>
        public static void Attach(Type classType)
        {
            EventManager.RegisterClassHandler(classType, Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
            EventManager.RegisterClassHandler(classType, DismissPopupEvent, new DismissPopupEventHandler(OnDismissPopup));
            EventManager.RegisterClassHandler(classType, FrameworkElement.ContextMenuOpeningEvent, new ContextMenuEventHandler(OnContextMenuOpened), true);
            EventManager.RegisterClassHandler(classType, FrameworkElement.ContextMenuClosingEvent, new ContextMenuEventHandler(OnContextMenuClosed), true);
            EventManager.RegisterClassHandler(classType, UIElement.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
        }

        /// <summary>
        /// Handles PreviewMouseDownOutsideCapturedElementEvent event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnClickThroughThunk(object sender, MouseButtonEventArgs e)
        {
            ////Debug.WriteLine(string.Format("OnClickThroughThunk: sender = {0}; originalSource = {1}; mouse capture = {2}", sender, e.OriginalSource, Mouse.Captured));

            if (e.ChangedButton == MouseButton.Left
                || e.ChangedButton == MouseButton.Right)
            {
                if (Mouse.Captured == sender
                    // Special handling for unknown Popups (for example datepickers used in the ribbon)
                    || (sender is IDropDownControl
                        && IsPopupRoot(Mouse.Captured)
                        )
                    )
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
            Debug.WriteLine(string.Format("Sender         - {0}", sender));
            Debug.WriteLine(string.Format("OriginalSource - {0}", e.OriginalSource));
            Debug.WriteLine(string.Format("Mouse.Captured - {0}", Mouse.Captured));

            var control = sender as IDropDownControl;

            if (control == null)
            {
                return;
            }

            if (Mouse.Captured != sender
                && control.IsDropDownOpen
                && !control.IsContextMenuOpened)
            {
                var popup = control.DropDownPopup;

                if (popup == null
                    || popup.Child == null)
                {
                    RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                    return;
                }

                if (e.OriginalSource == sender)
                {
                    // If Ribbon loses capture because something outside popup is clicked - close the popup
                    if (Mouse.Captured == null
                        || IsAncestorOf(popup.Child, Mouse.Captured as DependencyObject) == false)
                    {
                        RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                    }

                    return;
                }

                if (IsAncestorOf(popup.Child, e.OriginalSource as DependencyObject) == false)
                {
                    RaiseDismissPopupEvent(sender, DismissPopupMode.MouseNotOver);
                    return;
                }

                if (e.OriginalSource != null
                    && Mouse.Captured == null
                    && (IsPopupRoot(e.OriginalSource) || IsAncestorOf(popup.Child, e.OriginalSource as DependencyObject)))
                {
                    Debug.WriteLine(string.Format("Setting mouse capture to: {0}", sender));
                    Mouse.Capture(sender as IInputElement, CaptureMode.SubTree);
                    e.Handled = true;
                    return;
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
                if (ReferenceEquals(element, parent))
                {
                    return true;
                }

                element = VisualTreeHelper.GetParent(element) ?? LogicalTreeHelper.GetParent(element);
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
            var control = sender as IDropDownControl;

            if (control == null)
            {
                return;
            }

            if (e.DismissMode == DismissPopupMode.Always)
            {
                if (Mouse.Captured == control)
                {
                    Mouse.Capture(null);
                }

                control.IsDropDownOpen = false;
            }
            else
            {
                if (control.IsDropDownOpen
                    && !IsMousePhysicallyOver(control.DropDownPopup))
                {
                    if (Mouse.Captured == control)
                    {
                        Mouse.Capture(null);
                    }

                    control.IsDropDownOpen = false;
                }
                else
                {
                    if (control.IsDropDownOpen
                        && Mouse.Captured != control)
                    {
                        Mouse.Capture(sender as IInputElement, CaptureMode.SubTree);
                    }

                    if (control.IsDropDownOpen)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true whether mouse is physically over the popup 
        /// </summary>
        /// <param name="popup">Element</param>
        /// <returns>Returns true whether mouse is physically over the popup</returns>
        public static bool IsMousePhysicallyOver(Popup popup)
        {
            if (popup == null
                || popup.Child == null)
            {
                return false;
            }

            return IsMousePhysicallyOver(popup.Child);
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

            var position = Mouse.GetPosition(element);
            return ((position.X >= 0.0) && (position.Y >= 0.0))
                && ((position.X <= element.RenderSize.Width) && (position.Y <= element.RenderSize.Height));
        }

        /// <summary>
        /// Handles context menu opened event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnContextMenuOpened(object sender, ContextMenuEventArgs e)
        {
            var control = sender as IDropDownControl;

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
            var control = sender as IDropDownControl;

            if (control != null)
            {
                //Debug.WriteLine("Context menu closed");
                control.IsContextMenuOpened = false;
                RaiseDismissPopupEvent(control, DismissPopupMode.MouseNotOver);
            }
        }

        private static bool IsPopupRoot(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();

            return type.FullName == "System.Windows.Controls.Primitives.PopupRoot"
                   || type.Name == "PopupRoot";
        }
    }
}