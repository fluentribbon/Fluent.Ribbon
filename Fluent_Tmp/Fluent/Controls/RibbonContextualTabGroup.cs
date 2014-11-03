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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents contextual tab group
    /// </summary>
    public class RibbonContextualTabGroup : Control
    {
        #region Fields

        // Collection of ribbon tab items
        private readonly List<RibbonTabItem> items = new List<RibbonTabItem>();

        private Window parentWidow;

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
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RibbonContextualTabGroup),
            new UIPropertyMetadata("RibbonContextualTabGroup", OnHeaderChanged));

        /// <summary>
        /// Handles header chages
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets collection of tab items
        /// </summary>
        public List<RibbonTabItem> Items
        {
            get { return items; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether parent window is maximized
        /// </summary>
        public bool IsWindowMaximized
        {
            get { return (bool)GetValue(IsWindowMaximizedProperty); }
            set { SetValue(IsWindowMaximizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWindowMaximized.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWindowMaximizedProperty =
            DependencyProperty.Register("IsWindowMaximized", typeof(bool), typeof(RibbonContextualTabGroup), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the visibility this group for internal use (this enables us to hide this group when all items in this group are hidden)
        /// </summary>
        public Visibility InnerVisibility
        {
            get { return (Visibility)GetValue(InnerVisibilityProperty); }
            private set { SetValue(InnerVisibilityPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey InnerVisibilityPropertyKey =
            DependencyProperty.RegisterReadOnly("InnerVisibility", typeof(Visibility), typeof(RibbonContextualTabGroup), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerVisibilityProperty = InnerVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the first visible TabItem in this group
        /// </summary>
        public RibbonTabItem FirstVisibleItem
        {
            get
            {
                return this.GetFirstVisibleItem();
            }
        }

        /// <summary>
        /// Gets the last visible TabItem in this group
        /// </summary>
        public RibbonTabItem LastVisibleItem
        {
            get
            {
                return this.GetLastVisibleItem();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonContextualTabGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(typeof(RibbonContextualTabGroup)));
            VisibilityProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new PropertyMetadata(Visibility.Collapsed, OnVisibilityChanged));
            StyleProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(null, OnCoerceStyle));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = ((FrameworkElement)d).TryFindResource(typeof(RibbonContextualTabGroup));
            }

            return basevalue;
        }

        /// <summary>
        /// Handles visibility prioperty changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = (RibbonContextualTabGroup)d;

            foreach (var tab in group.Items)
            {
                tab.Visibility = group.Visibility;
            }

            group.UpdateInnerVisiblityAndGroupBorders();

            var titleBar = group.Parent as RibbonTitleBar;

            if (titleBar != null)
            {
                titleBar.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonContextualTabGroup()
        {
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            parentWidow = Window.GetWindow(this);

            this.SubscribeEvents();
            this.UpdateInnerVisibility();

            if (parentWidow != null)
            {
                IsWindowMaximized = parentWidow.WindowState == WindowState.Maximized;
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();

            this.parentWidow = null;
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();

            if (this.parentWidow != null)
            {
                this.parentWidow.StateChanged += this.OnParentWindowStateChanged;
            }
        }

        private void UnSubscribeEvents()
        {
            if (this.parentWidow != null)
            {
                this.parentWidow.StateChanged -= this.OnParentWindowStateChanged;
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Appends tab item
        /// </summary>
        /// <param name="item">Ribbon tab item</param>
        internal void AppendTabItem(RibbonTabItem item)
        {
            this.Items.Add(item);
            this.UpdateInnerVisiblityAndGroupBorders();
        }

        /// <summary>
        /// Removes tab item
        /// </summary>
        /// <param name="item">Ribbon tab item</param>
        internal void RemoveTabItem(RibbonTabItem item)
        {
            this.Items.Remove(item);
            this.UpdateInnerVisiblityAndGroupBorders();
        }

        private RibbonTabItem GetFirstVisibleItem()
        {
            return this.items.FirstOrDefault(item => item.Visibility == Visibility.Visible);
        }

        private RibbonTabItem GetLastVisibleItem()
        {
            return this.items.LastOrDefault(item => item.Visibility == Visibility.Visible);
        }

        /// <summary>
        /// Updates the group border
        /// </summary>
        public void UpdateInnerVisiblityAndGroupBorders()
        {
            this.UpdateInnerVisibility();

            var leftset = false;
            var rightset = false;

            for (var i = 0; i < this.items.Count; i++)
            {
                //if (i == 0) items[i].HasLeftGroupBorder = true;
                //else items[i].HasLeftGroupBorder = false;
                //if (i == items.Count - 1) items[i].HasRightGroupBorder = true;
                //else items[i].HasRightGroupBorder = false;

                //Workaround so you can have inivisible Tabs on a Group
                if (this.items[i].Visibility == Visibility.Visible
                    && leftset == false)
                {
                    this.items[i].HasLeftGroupBorder = true;
                    leftset = true;
                }
                else
                {
                    this.items[i].HasLeftGroupBorder = false;
                }

                if (this.items[this.items.Count - 1 - i].Visibility == Visibility.Visible
                    && rightset == false)
                {
                    this.items[this.items.Count - 1 - i].HasRightGroupBorder = true;
                    rightset = true;
                }
                else
                {
                    this.items[this.items.Count - 1 - i].HasRightGroupBorder = false;
                }
            }
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
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var firstVisibleItem = this.FirstVisibleItem;

            if (e.ClickCount == 1
                && firstVisibleItem != null)
            {
                if (firstVisibleItem.TabControlParent != null)
                {
                    var currentSelectedItem = firstVisibleItem.TabControlParent.SelectedItem as RibbonTabItem;

                    if (currentSelectedItem != null)
                    {
                        currentSelectedItem.IsSelected = false;
                    }
                }

                e.Handled = true;

                if (firstVisibleItem.TabControlParent != null)
                {
                    if (firstVisibleItem.TabControlParent.IsMinimized)
                    {
                        firstVisibleItem.TabControlParent.IsMinimized = false;
                    }

                    firstVisibleItem.IsSelected = true;
                }
            }

            base.OnMouseLeftButtonUp(e);
        }

        /// <summary>
        /// Raises the MouseDoubleClick routed event
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (this.parentWidow == null)
            {
                return;
            }

            this.parentWidow.WindowState = this.parentWidow.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        #endregion

        /// <summary>
        /// Updates the Visibility of the inner container
        /// </summary>
        private void UpdateInnerVisibility()
        {
            this.InnerVisibility = this.Visibility == Visibility.Visible && this.Items.Any(item => item.Visibility == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnParentWindowStateChanged(object sender, EventArgs e)
        {
            this.IsWindowMaximized = this.parentWidow.WindowState == WindowState.Maximized;
        }
    }
}
