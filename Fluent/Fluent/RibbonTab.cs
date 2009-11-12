using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Fluent
{
    internal enum RibbonTabState
    {
        Unselected = 0,
        Selected,
        SelectedForPopup,
        SelectedForKeyTip
    }

    public class RibbonTab:ContentControl
    {
        #region Attributes
        
        #endregion

        #region Properties

        /// <summary>
        /// Get os set tab header
        /// </summary>
        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Get os set tab header
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(RibbonTab), new UIPropertyMetadata(""));

        /// <summary>
        /// Get or set tab state
        /// </summary>
        internal RibbonTabState TabState
        {
            get { return (RibbonTabState)GetValue(TabStateProperty); }
            set { SetValue(TabStateProperty, value); }
        }

        /// <summary>
        /// Get or set tab state
        /// </summary>
        internal static readonly DependencyProperty TabStateProperty = DependencyProperty.Register("TabState", typeof(RibbonTabState), typeof(RibbonTab), new FrameworkPropertyMetadata(RibbonTabState.Unselected, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTabStateChanged));

        #endregion

        #region Events

        public event RoutedEventHandler TabChanged;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonTab()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTab), new FrameworkPropertyMetadata(typeof(RibbonTab)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTab()
        {
            
        }

        #endregion

        #region Event handling

        private static void OnTabStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTab tab = d as RibbonTab;
            if (tab.TabChanged != null) tab.TabChanged(tab, (RoutedEventArgs)EventArgs.Empty);
        }

        #endregion
    }
}
