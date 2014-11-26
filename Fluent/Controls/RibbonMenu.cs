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

        #endregion

        #region Overrides
#if NET45
        private object currentItem;
#endif
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
#if NET45
            var item = this.currentItem;
            this.currentItem = null;

            if (item != null)
            {
                var dataTemplate = this.ItemTemplate;
                if (dataTemplate == null && this.UsesItemContainerTemplate && this.ItemContainerTemplateSelector != null)
                    dataTemplate = this.ItemContainerTemplateSelector.SelectTemplate(item, this);
                if (dataTemplate != null)
                {
                    var dataTemplateContent = (object)dataTemplate.LoadContent();
                    if (dataTemplateContent is System.Windows.Controls.MenuItem
                        || dataTemplateContent is MenuItem || dataTemplateContent is Separator || dataTemplateContent is Gallery)
                    {
                        return dataTemplateContent as DependencyObject;
                    }
                }
            }
#endif
            return new MenuItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var isItemItsOwnContainerOverride = item is System.Windows.Controls.MenuItem 
                            || item is MenuItem || item is Separator || item is Gallery;

#if NET45
            if (isItemItsOwnContainerOverride == false)
            {
                this.currentItem = item;
            }
            else
                this.currentItem = null;
#endif

            return isItemItsOwnContainerOverride;
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
