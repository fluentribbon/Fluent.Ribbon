using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Fluent
{
    public class RibbonContextualTabGroup: Control
    {
        #region

        private List<RibbonTabItem> items = new List<RibbonTabItem>();

        private double cachedWidth = 0;

        #endregion

        #region Properties

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RibbonContextualTabGroup), new UIPropertyMetadata("RibbonContextualTabGroup", OnHeaderChanged));

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RibbonContextualTabGroup).cachedWidth = 0;
        }

        public double WhitespaceExtender
        {
            get { return (double)GetValue(WhitespaceExtenderProperty); }
            set { SetValue(WhitespaceExtenderProperty, value); }
        }

        internal List<RibbonTabItem> Items
        {
            get { return items; }
        }

        // Using a DependencyProperty as the backing store for RightOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WhitespaceExtenderProperty =
            DependencyProperty.Register("WhitespaceExtender", typeof(double), typeof(RibbonContextualTabGroup), new UIPropertyMetadata(0.0));


        internal double CachedWidth
        {
            get { return cachedWidth; }
            set { cachedWidth = value; }
        }

        #endregion

        #region Initialize

        static RibbonContextualTabGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(typeof(RibbonContextualTabGroup)));

            //VisibilityProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new PropertyMetadata(System.Windows.Visibility.Visible, OnVisibilityChanged));
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonContextualTabGroup group = d as RibbonContextualTabGroup;
            for (int i = 0; i < group.Items.Count; i++) group.Items[i].Visibility = group.Visibility;
        }

        public RibbonContextualTabGroup()
        {
            
        }

        #endregion

        #region Internal Methods

        internal void AppendTabItem(RibbonTabItem item)
        {
            Items.Add(item);
            item.Visibility = Visibility;
        }

        internal void RemoveTabItem(RibbonTabItem item)
        {
            Items.Remove(item);            
        }

        #endregion
    }
}
