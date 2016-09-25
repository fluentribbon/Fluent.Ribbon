using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    /// <summary>
    /// Represents menu in combo box and gallery
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class RibbonMenu : MenuBase
    {
        #region Constructors

        static RibbonMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenu), new FrameworkPropertyMetadata(typeof(RibbonMenu)));
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
            return item is System.Windows.Controls.MenuItem
                || item is Separator;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            //base.OnGotKeyboardFocus(e);
            var element = this.GetRootDropDownControl() as IInputElement;
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
                var popup = element as IDropDownControl;
                if (popup != null) return popup;
                var elementParent = VisualTreeHelper.GetParent(element);
                if (elementParent == null) element = LogicalTreeHelper.GetParent(element);
                else element = elementParent;
            }
            return null;
        }

        #endregion
    }
}
