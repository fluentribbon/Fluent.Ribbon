using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents menu item
    /// </summary>
    public class MenuItem: Button
    {
        #region Properies

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuItem()
        {            
        }

        #endregion

        #region Overrides

        #endregion

        #region Event Handling

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnClick(RoutedEventArgs e)
        {
            base.OnClick(e);
            RibbonPopup popup = FindParentPopup();
            if (popup != null) popup.IsOpen = false;
        }

        #endregion

        #region Private methods

        private RibbonPopup FindParentPopup()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                RibbonPopup ribbonPopup = element as RibbonPopup;
                if (ribbonPopup != null) return ribbonPopup;
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element);
                if (parent != null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element);
            }
            return null;
        }

        #endregion
    }
}
