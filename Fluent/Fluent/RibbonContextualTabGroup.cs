#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents contextual tab group
    /// </summary>
    public class RibbonContextualTabGroup: Control
    {
        #region

        // Collection of ribbon tab items
        private List<RibbonTabItem> items = new List<RibbonTabItem>();

        // Cached width
        private double cachedWidth;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets group header
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RibbonContextualTabGroup), new UIPropertyMetadata("RibbonContextualTabGroup", OnHeaderChanged));

        /// <summary>
        /// Handles header chages
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RibbonContextualTabGroup).cachedWidth = 0;
        }

        /// <summary>
        /// Gets or space whitespace extender
        /// </summary>
        public double IndentExtender
        {
            get { return (double)GetValue(IndentExtenderProperty); }
            set { SetValue(IndentExtenderProperty, value); }
        }
        /// <summary>
        /// Gets collection of tab items
        /// </summary>
        internal List<RibbonTabItem> Items
        {
            get { return items; }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RightOffset.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IndentExtenderProperty =
            DependencyProperty.Register("IndentExtender", typeof(double), typeof(RibbonContextualTabGroup), new UIPropertyMetadata(0.0));

        /*/// <summary>
        /// Gets or sets chached width
        /// </summary>
        internal double CachedWidth
        {
            get { return cachedWidth; }
            set { cachedWidth = value; }
        }*/

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonContextualTabGroup()
        {
            //StyleProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(typeof(RibbonContextualTabGroup)));

            VisibilityProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new PropertyMetadata(System.Windows.Visibility.Collapsed, OnVisibilityChanged));
        }
        
        /// <summary>
        /// Handles visibility prioperty changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonContextualTabGroup group = d as RibbonContextualTabGroup;
            for (int i = 0; i < group.Items.Count; i++) group.Items[i].Visibility = group.Visibility;            
            if (group.Parent is RibbonTitleBar) (group.Parent as RibbonTitleBar).InvalidateMeasure();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonContextualTabGroup()
        {
            
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Appends tab item
        /// </summary>
        /// <param name="item">Ribbon tab item</param>
        internal void AppendTabItem(RibbonTabItem item)
        {
            Items.Add(item);
            item.Visibility = Visibility;
            UpdateGroupBorders();
        }

        // Update group border
        private void UpdateGroupBorders()
        {
            for (int i = 0; i < items.Count;i++ )
            {
                if (i == 0) items[i].HasLeftGroupBorder = true;
                else items[i].HasLeftGroupBorder = false;
                if (i == items.Count - 1) items[i].HasRightGroupBorder = true;
                else items[i].HasRightGroupBorder = false;
            }
        }

        /// <summary>
        /// Removes tab item
        /// </summary>
        /// <param name="item">Ribbon tab item</param>
        internal void RemoveTabItem(RibbonTabItem item)
        {
            Items.Remove(item);
            UpdateGroupBorders();
        }

        #endregion

        #region Override

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.MouseLeftButtonUp�routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to 
        /// add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((e.ClickCount == 1) && (items.Count > 0))
            {
                if (items[0].TabControlParent != null) if (items[0].TabControlParent.SelectedItem is RibbonTabItem)
                        (items[0].TabControlParent.SelectedItem as RibbonTabItem).IsSelected = false;
                e.Handled = true;
                if (items[0].TabControlParent != null) if (items[0].TabControlParent.IsMinimized) items[0].TabControlParent.IsMinimized = false;
                items[0].IsSelected = true;
            }
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            if(e.RightButton==MouseButtonState.Pressed)
            {
                RibbonWindow wnd = Window.GetWindow(this) as RibbonWindow;
                if (wnd != null) wnd.ShowSystemMenu(PointToScreen(e.GetPosition(this)));
            }
        }

        #endregion
    }
}
