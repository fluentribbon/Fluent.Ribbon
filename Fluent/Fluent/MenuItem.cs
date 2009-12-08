using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    public class MenuItem: Button
    {
        #region Properies

        #endregion


        #region Constructor

        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));            
        }

        public MenuItem()
        {            
        }

        #endregion

        #region Overrides

        #endregion

        #region Event Handling

        protected override void OnClick(RoutedEventArgs e)
        {
            base.OnClick(e);
            ExtendedPopup popup = FindParentPopup();
            if (popup != null) popup.IsOpen = false;
        }

        #endregion

        #region Private methods

        private ExtendedPopup FindParentPopup()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                if (element is ExtendedPopup) return element as ExtendedPopup;
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element as DependencyObject);
                if (parent != null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element as DependencyObject);
            }
            return null;
        }

        #endregion
    }
}
