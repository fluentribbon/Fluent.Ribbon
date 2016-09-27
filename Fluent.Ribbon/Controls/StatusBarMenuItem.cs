using System.Windows;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    /// <summary>
    /// Represents menu item in ribbon status bar menu
    /// </summary>
    public class StatusBarMenuItem : MenuItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets Ribbon Status Bar menu item
        /// </summary>
        public StatusBarItem StatusBarItem
        {
            get { return (StatusBarItem)this.GetValue(StatusBarItemProperty); }
            set { this.SetValue(StatusBarItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StatusBarItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StatusBarItemProperty =
            DependencyProperty.Register(nameof(StatusBarItem), typeof(StatusBarItem), typeof(StatusBarMenuItem), new PropertyMetadata());


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
            this.StatusBarItem = item;
        }

        #endregion
    }
}
