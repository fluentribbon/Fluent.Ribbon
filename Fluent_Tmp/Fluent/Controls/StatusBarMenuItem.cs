using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Fluent;

namespace Fluent
{
    /// <summary>
    /// Represents menu item in ribbon status bar menu
    /// </summary>
    public class StatusBarMenuItem : Fluent.MenuItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets Ribbon Status Bar menu item
        /// </summary>
        public StatusBarItem StatusBarItem
        {
            get { return (StatusBarItem)GetValue(StatusBarItemProperty); }
            set { SetValue(StatusBarItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StatusBarItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StatusBarItemProperty =
            DependencyProperty.Register("StatusBarItem", typeof(StatusBarItem), typeof(StatusBarMenuItem), new UIPropertyMetadata(null));

        
        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static StatusBarMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBarMenuItem), new FrameworkPropertyMetadata(typeof(StatusBarMenuItem)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="item">Ribbon Status Bar menu item</param>
        public StatusBarMenuItem(StatusBarItem item)
        {
            StatusBarItem = item;
        }

        #endregion
    }
}
