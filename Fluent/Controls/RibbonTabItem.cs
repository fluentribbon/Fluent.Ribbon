#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// Represents ribbon tab item
    /// </summary>
    [TemplatePart(Name = "PART_ContentContainer", Type = typeof(Border))]
    [ContentProperty("Groups")]
    [DefaultProperty("Groups")]
    [DefaultEvent("IsSelectedChanged")]
    public class RibbonTabItem : Control, IKeyTipedControl, IHeaderedControl
    {
        #region Fields

        // Content container
        private Border contentContainer;

        // Desired width
        private double desiredWidth;

        // Collection of ribbon groups
        private ObservableCollection<RibbonGroupBox> groups;

        // Ribbon groups container
        private readonly RibbonGroupsContainer groupsInnerContainer = new RibbonGroupsContainer();
        private readonly ScrollViewer groupsContainer = new ScrollViewer();

        // Cached width
        private double cachedWidth;

        #endregion

        #region Properties

        #region KeyTip

        /// <summary>
        /// Gets or sets KeyTip for element.
        /// </summary>
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(RibbonTabItem));

        #endregion

        /// <summary>
        /// Gets ribbon groups container
        /// </summary>
        public ScrollViewer GroupsContainer
        {
            get { return this.groupsContainer; }
        }

        /// <summary>
        /// Gets or sets whether ribbon is minimized
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)this.GetValue(IsMinimizedProperty); }
            set { this.SetValue(IsMinimizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMinimized.  
        /// This enables animation, styling, binding, etc...
        /// </summary>  
        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register("IsMinimized", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether ribbon is opened
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)this.GetValue(IsOpenProperty); }
            set { this.SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>  
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets reduce order
        /// </summary>
        public string ReduceOrder
        {
            get { return this.groupsInnerContainer.ReduceOrder; }
            set { this.groupsInnerContainer.ReduceOrder = value; }
        }

        #region IsContextual

        /// <summary>
        /// Gets or sets whether tab item is contextual
        /// </summary>
        public bool IsContextual
        {
            get { return (bool)this.GetValue(IsContextualProperty); }
            private set { this.SetValue(IsContextualPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsContextualPropertyKey =
            DependencyProperty.RegisterReadOnly("IsContextual", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsContextual.  
        /// This enables animation, styling, binding, etc...
        /// </summary>  
        public static readonly DependencyProperty IsContextualProperty = IsContextualPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return this.groupsContainer;
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets whether tab item is selected
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  
        /// This enables animation, styling, binding, etc...
        /// </summary>  
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Gets ribbon tab control parent
        /// </summary>
        internal RibbonTabControl TabControlParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as RibbonTabControl);
            }
        }


        /// <summary>
        /// Gets or sets indent
        /// </summary>
        public double Indent
        {
            get { return (double)this.GetValue(IndentProperty); }
            set { this.SetValue(IndentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderMargin.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IndentProperty =
            DependencyProperty.Register("Indent", typeof(double), typeof(RibbonTabItem), new UIPropertyMetadata((double)12.0));

        /// <summary>
        /// Gets or sets whether separator is visible
        /// </summary>
        public bool IsSeparatorVisible
        {
            get { return (bool)this.GetValue(IsSeparatorVisibleProperty); }
            set { this.SetValue(IsSeparatorVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSeparatorVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSeparatorVisibleProperty =
            DependencyProperty.Register("IsSeparatorVisible", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets ribbon contextual tab group
        /// </summary>
        public RibbonContextualTabGroup Group
        {
            get { return (RibbonContextualTabGroup)this.GetValue(GroupProperty); }
            set { this.SetValue(GroupProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Group.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("Group", typeof(RibbonContextualTabGroup), typeof(RibbonTabItem), new UIPropertyMetadata(null, OnGroupChanged));

        // handles Group property chanhged
        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = (RibbonTabItem)d;

            if (e.OldValue != null)
            {
                ((RibbonContextualTabGroup)e.OldValue).RemoveTabItem(tab);
            }

            if (e.NewValue != null)
            {
                ((RibbonContextualTabGroup)e.NewValue).AppendTabItem(tab);
                tab.IsContextual = true;
            }
            else
            {
                tab.IsContextual = false;
            }
        }

        /// <summary>
        /// Gets or sets desired width of the tab item
        /// </summary>
        internal double DesiredWidth
        {
            get { return this.desiredWidth; }
            set
            {
                this.desiredWidth = value;
                this.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets whether tab item has left group border
        /// </summary>
        public bool HasLeftGroupBorder
        {
            get { return (bool)this.GetValue(HasLeftGroupBorderProperty); }
            set { this.SetValue(HasLeftGroupBorderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HaseLeftGroupBorder.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasLeftGroupBorderProperty =
            DependencyProperty.Register("HasLeftGroupBorder", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether tab item has right group border
        /// </summary>
        public bool HasRightGroupBorder
        {
            get { return (bool)this.GetValue(HasRightGroupBorderProperty); }
            set { this.SetValue(HasRightGroupBorderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HaseLeftGroupBorder.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasRightGroupBorderProperty =
            DependencyProperty.Register("HasRightGroupBorder", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        /// <summary>
        /// get collection of ribbon groups
        /// </summary>
        public ObservableCollection<RibbonGroupBox> Groups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new ObservableCollection<RibbonGroupBox>();
                    this.groups.CollectionChanged += this.OnGroupsCollectionChanged;
                }
                return this.groups;
            }
        }

        // handles ribbon groups collection changes
        private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.groupsInnerContainer == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        this.groupsInnerContainer.Children.Insert(e.NewStartingIndex + i, (UIElement)e.NewItems[i]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<UIElement>())
                    {
                        this.groupsInnerContainer.Children.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.OfType<UIElement>())
                    {
                        this.groupsInnerContainer.Children.Remove(item);
                    }
                    foreach (var item in e.NewItems.OfType<UIElement>())
                    {
                        this.groupsInnerContainer.Children.Add(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.groupsInnerContainer.Children.Clear();

                    foreach (var group in this.groups)
                    {
                        this.groupsInnerContainer.Children.Add(group);
                    }
                    break;
            }

        }

        #region Header Property

        /// <summary>
        /// Gets or sets header of tab item
        /// </summary>
        public object Header
        {
            get { return (object)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(RibbonTabItem), new UIPropertyMetadata(null, OnHeaderChanged));

        // Header changed handler
        static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabItem tabItem = (RibbonTabItem)d;
            tabItem.CoerceValue(ToolTipProperty);
        }

        #endregion

        #region Focusable

        /// <summary>
        /// Handles IsEnabled changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">The event data.</param>
        private static void OnFocusableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces IsEnabled 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="basevalue"></param>
        /// <returns></returns>
        private static object CoerceFocusable(DependencyObject d, object basevalue)
        {
            var control = d as RibbonTabItem;
            if (control != null)
            {
                var ribbon = control.FindParentRibbon();
                if (ribbon != null)
                {
                    return ((bool)basevalue)
                        && ribbon.Focusable;
                }
            }

            return basevalue;
        }

        // Find parent ribbon
        private Ribbon FindParentRibbon()
        {
            var element = this.Parent;
            while (element != null)
            {
                var ribbon = element as Ribbon;
                if (ribbon != null)
                {
                    return ribbon;
                }

                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        #endregion

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(typeof(RibbonTabItem)));
            FocusableProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(OnFocusableChanged, CoerceFocusable));
            ToolTipProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(null, CoerceToolTip));
            VisibilityProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(OnVisibilityChanged));
            StyleProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(null, OnCoerceStyle));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonTabItem));
            }

            return basevalue;
        }

        // Handles visibility changes
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as RibbonTabItem;

            if (item == null)
            {
                return;
            }

            if (item.Group != null)
            {
                item.Group.UpdateInnerVisiblityAndGroupBorders();
            }

            if (item.IsSelected
                && (Visibility)e.NewValue == Visibility.Collapsed)
            {
                if (item.TabControlParent != null)
                {
                    if (item.TabControlParent.IsMinimized)
                    {
                        item.IsSelected = false;
                    }
                    else
                    {
                        item.TabControlParent.SelectedItem = item.TabControlParent.GetFirstVisibleItem();
                    }
                }
            }
        }

        // Coerce ToolTip to ensure that tooltip displays name of the tabitem
        private static object CoerceToolTip(DependencyObject d, object basevalue)
        {
            var tabItem = (RibbonTabItem)d;
            if (basevalue == null
                && tabItem.Header is string)
            {
                basevalue = tabItem.Header;
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabItem()
        {
            this.AddLogicalChild(this.groupsContainer);
            this.groupsContainer.Content = this.groupsInnerContainer;

            // Force redirection of DataContext. This is needed, because we detach the container from the visual tree and attach it to a diffrent one (the popup/dropdown) when the ribbon is minimized.
            this.groupsInnerContainer.SetBinding(DataContextProperty, new Binding("DataContext")
                                                                          {
                                                                              Source = this
                                                                          });

            ContextMenuService.Coerce(this);

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.contentContainer == null)
            {
                return base.MeasureOverride(constraint);
            }

            if (this.IsContextual && this.Group != null && this.Group.Visibility == Visibility.Collapsed)
            {
                return Size.Empty;
            }

            this.contentContainer.Padding = new Thickness(this.Indent, this.contentContainer.Padding.Top, this.Indent, this.contentContainer.Padding.Bottom);
            Size baseConstraint = base.MeasureOverride(constraint);
            double totalWidth = this.contentContainer.DesiredSize.Width - this.contentContainer.Margin.Left - this.contentContainer.Margin.Right;
            (this.contentContainer.Child).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double headerWidth = this.contentContainer.Child.DesiredSize.Width;
            if (totalWidth < headerWidth + this.Indent * 2)
            {
                double newPaddings = Math.Max(0, (totalWidth - headerWidth) / 2);
                this.contentContainer.Padding = new Thickness(newPaddings, this.contentContainer.Padding.Top, newPaddings, this.contentContainer.Padding.Bottom);
            }
            else
            {
                if (this.desiredWidth != 0)
                {
                    // If header width is larger then tab increase tab width
                    if ((constraint.Width > this.desiredWidth) && (this.desiredWidth > totalWidth)) baseConstraint.Width = this.desiredWidth;
                    else
                        baseConstraint.Width = headerWidth + this.Indent * 2 + this.contentContainer.Margin.Left + this.contentContainer.Margin.Right;
                }
            }

            if ((this.cachedWidth != baseConstraint.Width) && (this.IsContextual) && (this.Group != null))
            {
                this.cachedWidth = baseConstraint.Width;
                FrameworkElement parent = (VisualTreeHelper.GetParent(this.Group) as FrameworkElement);
                if (parent != null) parent.InvalidateMeasure();
            }

            return baseConstraint;
        }

        /// <summary>
        /// On new style applying
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.contentContainer = this.GetTemplateChild("PART_ContentContainer") as Border;
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.MouseLeftButtonDown�routed event is raised 
        /// on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source == this
                && e.ClickCount == 2)
            {
                e.Handled = true;

                if (this.TabControlParent != null)
                {
                    this.TabControlParent.IsMinimized = !this.TabControlParent.IsMinimized;
                }
            }
            else if (e.Source == this
                || !this.IsSelected)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    if (this.TabControlParent != null)
                    {
                        var newItem = this.TabControlParent.ItemContainerGenerator.ItemFromContainer(this);

                        if (this.TabControlParent.SelectedTabItem == newItem)
                        {
                            this.TabControlParent.IsDropDownOpen = !this.TabControlParent.IsDropDownOpen;
                        }
                        else
                        {
                            this.TabControlParent.SelectedItem = newItem;
                        }

                        this.TabControlParent.RaiseRequestBackstageClose();
                    }
                    else
                    {
                        this.IsSelected = true;
                    }

                    e.Handled = true;
                }
            }
        }

        #endregion

        #region Private methods

        // Handles IsSelected property changes
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabItem container = d as RibbonTabItem;
            bool newValue = (bool)e.NewValue;
            if (newValue)
            {
                if ((container.TabControlParent != null) && (container.TabControlParent.SelectedItem is RibbonTabItem) && (container.TabControlParent.SelectedItem != container))
                    (container.TabControlParent.SelectedItem as RibbonTabItem).IsSelected = false;
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }

        }
        /// <summary>
        /// Handles selected
        /// </summary>
        /// <param name="e">The event data</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(e);
        }
        /// <summary>
        /// handles unselected
        /// </summary>
        /// <param name="e">The event data</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(e);
        }

        #endregion

        #region Event handling

        // Handles IsSelected property changes
        private void HandleIsSelectedChanged(RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();

            if (this.groups != null)
            {
                this.groups.CollectionChanged += this.OnGroupsCollectionChanged;
            }
        }

        private void UnSubscribeEvents()
        {
            if (this.groups != null)
            {
                this.groups.CollectionChanged -= this.OnGroupsCollectionChanged;
            }
        }

        #endregion

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            if (this.TabControlParent != null)
            {
                var currentSelectedItem = this.TabControlParent.SelectedItem as RibbonTabItem;

                if (currentSelectedItem != null)
                {
                    currentSelectedItem.IsSelected = false;
                }
            }

            this.IsSelected = true;
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
            if (this.TabControlParent != null
                && this.TabControlParent.IsMinimized)
            {
                this.TabControlParent.IsDropDownOpen = false;
            }
        }
    }
}