using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// This interface must be implemented for controls
    /// which are intended to insert to quick access toolbar
    /// </summary>
    public interface IQuickAccessItemProvider
    {
        /// <summary>
        /// Gets control which represents quick access toolbar item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control
        /// </summary>
        /// <returns>Control which represents quick access toolbar item</returns>
        UIElement GetQuickAccessToolbarItem();
    }

    /// <summary>
    /// The class responds to mine controls for QuickAccessToolbar
    /// </summary>
    public static class QuickAccessItemsProvider
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the given control can provide a quick access toolbar item
        /// </summary>
        /// <param name="element">Control</param>
        /// <returns>True if this control is able to provide
        /// a quick access toolbar item, false otherwise</returns>
        public static bool IsSupported(UIElement element)
        {
            if (element is IQuickAccessItemProvider) return true;
            if ((element is Button) ||
                (element is CheckBox)) return true;
            else return false;
        }

        /// <summary>
        /// Gets control which represents quick access toolbar item
        /// </summary>
        /// <param name="element">Host control</param>
        /// <returns>Control which represents quick access toolbar item</returns>
        public static UIElement GetItem(UIElement element)
        {
            // If control supports the interface just return what it provides
            if (element is IQuickAccessItemProvider) return (element as IQuickAccessItemProvider).GetQuickAccessToolbarItem();

            // Predefined controls
            if (element is Button) return GetButtonQuickAccessItem(element as Button);
            if (element is CheckBox) return GetCheckBoxQuickAccessItem(element as CheckBox);

            // The control isn't supported
            throw new ArgumentException("The contol " + element.GetType().Name + " is not able to provide a quick access toolbar item");
        }
        
        /// <summary>
        /// Finds the top supported control
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static UIElement FindAccessedControl(Visual visual, Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(visual, point);
            UIElement element = result.VisualHit as UIElement;
            while (element != null)
            {
                if(IsSupported(element)) return element;
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        #endregion

        #region Buttons

        static Dictionary<Button, Button> cachedQuickAccessButtons = new Dictionary<Button, Button>();
        static UIElement GetButtonQuickAccessItem(Button button)
        {
            if (cachedQuickAccessButtons.ContainsKey(button)) return cachedQuickAccessButtons[button];

            Button item = new Button();

            // Copy ScreenTip data
            CopyScreenTip(button, item);
            // Copy small icon
            RibbonControl.SetSmallIcon(item, RibbonControl.GetSmallIcon(button));
            
            // TODO: check, maybe copy style is not required for quick access toolbar items
            item.Style = button.Style;
            RibbonControl.SetSize(item, RibbonControlSize.Small);


            // Syncronization
            item.Click += new RoutedEventHandler(OnButtonClick);
            
            cachedQuickAccessButtons.Add(button, item);
            return item;
        }

        static void OnButtonClick(object sender, RoutedEventArgs e)
        {
            // Redirect to the host control
            cachedQuickAccessButtons.Where(x=>x.Value == sender).First().Key.RaiseEvent(e);
        }

        #endregion

        #region CheckBoxes

        static Dictionary<CheckBox, CheckBox> cachedQuickAccessCheckBoxes = new Dictionary<CheckBox, CheckBox>();
        static UIElement GetCheckBoxQuickAccessItem(CheckBox checkBox)
        {
            if (cachedQuickAccessCheckBoxes.ContainsKey(checkBox)) return cachedQuickAccessCheckBoxes[checkBox];

            CheckBox item = new CheckBox();
            item.Content = checkBox.Content.ToString();


            // Copy ScreenTip data
            CopyScreenTip(checkBox, item);
            // Copy small icon
            RibbonControl.SetSmallIcon(item, RibbonControl.GetSmallIcon(checkBox));
            
            // TODO: check, maybe copy style is not required for quick access toolbar items
            item.Style = checkBox.Style;

            // Syncronization
            item.Checked += new RoutedEventHandler(OnCheckBoxChecked);
            item.Unchecked += new RoutedEventHandler(OnCheckBoxChecked);
            checkBox.Checked += new RoutedEventHandler(OnHostCheckBoxChecked);
            checkBox.Unchecked += new RoutedEventHandler(OnHostCheckBoxChecked);

            cachedQuickAccessCheckBoxes.Add(checkBox, item);
            return item;
        }

        static void OnHostCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            cachedQuickAccessCheckBoxes[sender as CheckBox].IsChecked = (sender as CheckBox).IsChecked;
        }

        static void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            cachedQuickAccessCheckBoxes.Where(x => x.Value == sender).First().Key.IsChecked = (sender as CheckBox).IsChecked;
        }

        #endregion

        #region Common Stuff

        // Copies ScreenTip data
        static void CopyScreenTip(FrameworkElement from, FrameworkElement to)
        {
            ScreenTip.SetTitle(to, ScreenTip.GetTitle(from));
            ScreenTip.SetText(to, ScreenTip.GetText(from));
            ScreenTip.SetImage(to, ScreenTip.GetImage(from));
            ScreenTip.SetDisableReason(to, ScreenTip.GetDisableReason(from));
            ScreenTip.SetHelpTopic(to, ScreenTip.GetHelpTopic(from));
            ScreenTip.SetWidth(to, ScreenTip.GetWidth(from));
            to.ToolTip = from.ToolTip;
        }

        #endregion
    }
}
