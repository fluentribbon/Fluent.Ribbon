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
    public enum DismissPopupMode
    {
        Always,
        MouseNotOver
    }

    public delegate void DismissPopupEventHandler(object sender, DismissPopupEventArgs e);

    public class DismissPopupEventArgs : RoutedEventArgs
    {
        #region Properties

        public DismissPopupMode DismissMode { get; set; }

        #endregion


        // Methods
        public DismissPopupEventArgs()
            : this(DismissPopupMode.Always)
        {
        }

        public DismissPopupEventArgs(DismissPopupMode dismissMode)
        {
            base.RoutedEvent = PopupService.DismissPopupEvent;
            this.DismissMode = dismissMode;
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            DismissPopupEventHandler handler = (DismissPopupEventHandler)genericHandler;
            handler(genericTarget, this);
        }
    }

    public static class PopupService
    {
        #region DismissPopup

        public static readonly RoutedEvent DismissPopupEvent = EventManager.RegisterRoutedEvent("DismissPopup", RoutingStrategy.Bubble, typeof(DismissPopupEventHandler), typeof(PopupService));

        public static void RaiseDismissPopupEvent(object sender, DismissPopupMode mode)
        {
            (sender as UIElement).RaiseEvent(new DismissPopupEventArgs(DismissPopupMode.MouseNotOver));
        }

        #endregion

        public static void Attach(Type classType)
        {
            EventManager.RegisterClassHandler(classType, Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(PopupService.OnClickThroughThunk));
            EventManager.RegisterClassHandler(classType, PopupService.DismissPopupEvent, new DismissPopupEventHandler(PopupService.OnDismissPopup));
            EventManager.RegisterClassHandler(classType, FrameworkElement.ContextMenuOpeningEvent, new ContextMenuEventHandler(PopupService.OnContextMenuOpened), true);
            EventManager.RegisterClassHandler(classType, FrameworkElement.ContextMenuClosingEvent, new ContextMenuEventHandler(PopupService.OnContextMenuClosed), true);
            EventManager.RegisterClassHandler(classType, FrameworkElement.LostMouseCaptureEvent, new MouseEventHandler(PopupService.OnLostMouseCapture));
        }


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

        public static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Lost Capture - " + Mouse.Captured);
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

        public static void OnDismissPopup(object sender, DismissPopupEventArgs e)
        {
            IDropDownControl control = sender as IDropDownControl;
            if (control == null) return;
            if (e.DismissMode == DismissPopupMode.Always)
            {
                if (Mouse.Captured == control) Mouse.Capture(null);
                Debug.WriteLine("DropDown Closed");
                control.IsDropDownOpen = false;
            }
            else
            {
                if ((control.IsDropDownOpen)&&(!PopupService.IsMousePhysicallyOver(control.DropDownPopup.Child)))
                {
                    if (Mouse.Captured == control) Mouse.Capture(null);
                    Debug.WriteLine("DropDown Closed");
                    control.IsDropDownOpen = false;
                }
                else
                {
                    if ((control.IsDropDownOpen) && (Mouse.Captured != control)) Mouse.Capture(sender as IInputElement, CaptureMode.SubTree);
                    if (control.IsDropDownOpen) e.Handled = true;
                }
            }
        }

        public static bool IsMousePhysicallyOver(UIElement element)
        {
            if (element == null)
            {
                return false;
            }
            Point position = Mouse.GetPosition(element);
            return ((position.X >= 0.0) && (position.Y >= 0.0)) && ((position.X <= element.RenderSize.Width) && (position.Y <= element.RenderSize.Height));
        }

        public static void OnContextMenuOpened(object sender, ContextMenuEventArgs e)
        {
            IDropDownControl control = sender as IDropDownControl;
            if (control != null)
            {
                control.IsContextMenuOpened = true;
                Debug.WriteLine("Context menu opened");
            }
        }

        public static void OnContextMenuClosed(object sender, ContextMenuEventArgs e)
        {
            IDropDownControl control = sender as IDropDownControl;
            if (control != null)
            {
                Debug.WriteLine("Context menu closed");
                control.IsContextMenuOpened = false;
                RaiseDismissPopupEvent(control, DismissPopupMode.MouseNotOver);
            }
        }
    }
}
