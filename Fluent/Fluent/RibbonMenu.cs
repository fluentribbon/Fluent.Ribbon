using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents menu in combo box and gallery
    /// </summary>
    [ContentProperty("Items")]
    public class RibbonMenu : MenuBase
    {
        #region Constructors

        static RibbonMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenu), new FrameworkPropertyMetadata(typeof(RibbonMenu)));
            StyleProperty.OverrideMetadata(typeof(RibbonMenu), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonMenu));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonMenu()
        {
            FocusManager.SetIsFocusScope(this, false);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is System.Windows.Controls.MenuItem);
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            //base.OnGotKeyboardFocus(e);
            IInputElement element = GetRootDropDownControl() as IInputElement;
            if (element != null) Keyboard.Focus(element);
        }

        /*protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            IInputElement element = GetRootDropDownControl() as IInputElement;
            if ((element!=null)&&(Mouse.Captured!=element)) Mouse.Capture(element, CaptureMode.SubTree);
        }*/

        private IDropDownControl GetRootDropDownControl()
        {
            DependencyObject element = this;
            while (element != null)
            {
                IDropDownControl popup = element as IDropDownControl;
                if (popup != null) return popup;
                DependencyObject elementParent = VisualTreeHelper.GetParent(element);
                if (elementParent == null) element = LogicalTreeHelper.GetParent(element);
                else element = elementParent;
            }
            return null;
        }

        #endregion
    }
}
