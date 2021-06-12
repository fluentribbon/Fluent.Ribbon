// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using Fluent.Automation.Peers;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents ribbon tab item
    /// </summary>
    [TemplatePart(Name = "PART_HeaderContentHost", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ContentContainer", Type = typeof(Border))]
    [ContentProperty(nameof(Groups))]
    [DefaultProperty(nameof(Groups))]
    public class RibbonTabItem : Control, IKeyTipedControl, IHeaderedControl, ILogicalChildSupport
    {
        #region Fields

        // Content container
        private Border? contentContainer;

        // Desired width
        private double desiredWidth;

        // Collection of ribbon groups
        private ObservableCollection<RibbonGroupBox>? groups;

        // Ribbon groups container
        private readonly RibbonGroupsContainer groupsInnerContainer = new RibbonGroupsContainer();

        // Cached width
        private double cachedWidth;

        #endregion

        #region Properties

        internal FrameworkElement? HeaderContentHost { get; private set; }

        #region Colors/Brushes

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> which is used to render the background if this <see cref="RibbonTabItem"/> is the currently active/selected one.
        /// </summary>
        public Brush? ActiveTabBackground
        {
            get { return (Brush?)this.GetValue(ActiveTabBackgroundProperty); }
            set { this.SetValue(ActiveTabBackgroundProperty, value); }
        }

        /// <summary>Identifies the <see cref="ActiveTabBackground"/> dependency property.</summary>
        public static readonly DependencyProperty ActiveTabBackgroundProperty =
            DependencyProperty.Register(nameof(ActiveTabBackground), typeof(Brush), typeof(RibbonTabItem), new PropertyMetadata());

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> which is used to render the border if this <see cref="RibbonTabItem"/> is the currently active/selected one.
        /// </summary>
        public Brush? ActiveTabBorderBrush
        {
            get { return (Brush?)this.GetValue(ActiveTabBorderBrushProperty); }
            set { this.SetValue(ActiveTabBorderBrushProperty, value); }
        }

        /// <summary>Identifies the <see cref="ActiveTabBorderBrush"/> dependency property.</summary>
        public static readonly DependencyProperty ActiveTabBorderBrushProperty =
            DependencyProperty.Register(nameof(ActiveTabBorderBrush), typeof(Brush), typeof(RibbonTabItem), new PropertyMetadata());

        #endregion

        #region KeyTip

        /// <inheritdoc />
        public string? KeyTip
        {
            get { return (string?)this.GetValue(KeyTipProperty); }
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
        public ScrollViewer GroupsContainer { get; } = new RibbonGroupsContainerScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Disabled };

        /// <summary>
        /// Gets or sets whether ribbon is minimized
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)this.GetValue(IsMinimizedProperty); }
            set { this.SetValue(IsMinimizedProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="IsMinimized"/> dependency property.</summary>
        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register(nameof(IsMinimized), typeof(bool), typeof(RibbonTabItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether ribbon is opened
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)this.GetValue(IsOpenProperty); }
            set { this.SetValue(IsOpenProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="IsOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(RibbonTabItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets reduce order
        /// </summary>
        public string? ReduceOrder
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
            private set { this.SetValue(IsContextualPropertyKey, BooleanBoxes.Box(value)); }
        }

        private static readonly DependencyPropertyKey IsContextualPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsContextual), typeof(bool), typeof(RibbonTabItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="IsContextual"/> dependency property.</summary>
        public static readonly DependencyProperty IsContextualProperty = IsContextualPropertyKey.DependencyProperty;

        #endregion

        /// <summary>
        /// Gets or sets whether tab item is selected
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }

            set
            {
                this.SetValue(IsSelectedProperty, BooleanBoxes.Box(value));
            }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnIsSelectedChanged));

        /// <summary>
        /// Gets ribbon tab control parent
        /// </summary>
        internal RibbonTabControl? TabControlParent => UIHelper.GetParent<RibbonTabControl>(this);

        /// <summary>
        /// Gets or sets the padding for the header.
        /// </summary>
        public Thickness HeaderPadding
        {
            get { return (Thickness)this.GetValue(HeaderPaddingProperty); }
            set { this.SetValue(HeaderPaddingProperty, value); }
        }

        /// <summary>Identifies the <see cref="HeaderPadding"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderPaddingProperty =
            DependencyProperty.Register(nameof(HeaderPadding), typeof(Thickness), typeof(RibbonTabItem), new PropertyMetadata(new Thickness(8, 5, 8, 5)));

        /// <summary>
        /// Gets or sets whether separator is visible
        /// </summary>
        public bool IsSeparatorVisible
        {
            get { return (bool)this.GetValue(IsSeparatorVisibleProperty); }
            set { this.SetValue(IsSeparatorVisibleProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="IsSeparatorVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsSeparatorVisibleProperty =
            DependencyProperty.Register(nameof(IsSeparatorVisible), typeof(bool), typeof(RibbonTabItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets ribbon contextual tab group
        /// </summary>
        public RibbonContextualTabGroup? Group
        {
            get { return (RibbonContextualTabGroup?)this.GetValue(GroupProperty); }
            set { this.SetValue(GroupProperty, value); }
        }

        /// <summary>Identifies the <see cref="Group"/> dependency property.</summary>
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register(nameof(Group), typeof(RibbonContextualTabGroup), typeof(RibbonTabItem), new PropertyMetadata(OnGroupChanged));

        // handles Group property chanhged
        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tab = (RibbonTabItem)d;

            ((RibbonContextualTabGroup?)e.OldValue)?.RemoveTabItem(tab);

            if (e.NewValue is RibbonContextualTabGroup tabGroup)
            {
                tabGroup.AppendTabItem(tab);
                tab.IsContextual = true;
            }
            else
            {
                tab.IsContextual = false;
            }
        }

        /// <summary>
        /// Gets or sets desired width of the tab item.
        /// </summary>
        /// <remarks>This is needed in case the width of <see cref="Group"/> is larger than it's tabs.</remarks>
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
            set { this.SetValue(HasLeftGroupBorderProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="HasLeftGroupBorder"/> dependency property.</summary>
        public static readonly DependencyProperty HasLeftGroupBorderProperty =
            DependencyProperty.Register(nameof(HasLeftGroupBorder), typeof(bool), typeof(RibbonTabItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether tab item has right group border
        /// </summary>
        public bool HasRightGroupBorder
        {
            get { return (bool)this.GetValue(HasRightGroupBorderProperty); }
            set { this.SetValue(HasRightGroupBorderProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="HasRightGroupBorder"/> dependency property.</summary>
        public static readonly DependencyProperty HasRightGroupBorderProperty =
            DependencyProperty.Register(nameof(HasRightGroupBorder), typeof(bool), typeof(RibbonTabItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// get collection of ribbon groups
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonGroupBox> Groups
        {
            get
            {
                if (this.groups is null)
                {
                    this.groups = new ObservableCollection<RibbonGroupBox>();
                    this.groups.CollectionChanged += this.OnGroupsCollectionChanged;
                }

                return this.groups;
            }
        }

        // handles ribbon groups collection changes
        private void OnGroupsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.groupsInnerContainer is null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems?.Count; i++)
                    {
                        var element = (UIElement?)e.NewItems[i];

                        if (element is not null)
                        {
                            this.groupsInnerContainer.Children.Insert(e.NewStartingIndex + i, element);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.NullSafe().OfType<UIElement>())
                    {
                        this.groupsInnerContainer.Children.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.NullSafe().OfType<UIElement>())
                    {
                        this.groupsInnerContainer.Children.Remove(item);
                    }

                    foreach (var item in e.NewItems.NullSafe().OfType<UIElement>())
                    {
                        this.groupsInnerContainer.Children.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.groupsInnerContainer.Children.Clear();

                    foreach (var group in this.Groups)
                    {
                        this.groupsInnerContainer.Children.Add(group);
                    }

                    break;
            }
        }

        #region Header Property

        /// <inheritdoc />
        public object? Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region HeaderTemplate Property

        /// <summary>
        /// Gets or sets header template of tab item.
        /// </summary>
        public DataTemplate? HeaderTemplate
        {
            get { return (DataTemplate?)this.GetValue(HeaderTemplateProperty); }
            set { this.SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(RibbonTabItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region Focusable

        /// <summary>
        /// Handles Focusable changes
        /// </summary>
        private static void OnFocusableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces Focusable
        /// </summary>
        private static object? CoerceFocusable(DependencyObject d, object? basevalue)
        {
            var control = d as RibbonTabItem;
            var ribbon = control?.FindParentRibbon();

            if (ribbon is not null
                && basevalue is bool boolValue)
            {
                return BooleanBoxes.Box(boolValue && ribbon.Focusable);
            }

            return basevalue;
        }

        // Find parent ribbon
        private Ribbon? FindParentRibbon()
        {
            var element = this.Parent;
            while (element is not null)
            {
                if (element is Ribbon ribbon)
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
        static RibbonTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(typeof(RibbonTabItem)));
            FocusableProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(OnFocusableChanged, CoerceFocusable));
            VisibilityProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(OnVisibilityChanged));

            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));

            AutomationProperties.IsOffscreenBehaviorProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(IsOffscreenBehavior.FromClip));
        }

        // Handles visibility changes
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as RibbonTabItem;

            if (item is null)
            {
                return;
            }

            item.Group?.UpdateInnerVisiblityAndGroupBorders();

            if (item.IsSelected
                && (Visibility)e.NewValue == Visibility.Collapsed)
            {
                if (item.TabControlParent is not null)
                {
                    if (item.TabControlParent.IsMinimized)
                    {
                        item.IsSelected = false;
                    }
                    else
                    {
                        item.TabControlParent.SelectedItem = item.TabControlParent.GetFirstVisibleAndEnabledItem();
                    }
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabItem()
        {
            this.AddLogicalChild(this.GroupsContainer);
            this.GroupsContainer.Content = this.groupsInnerContainer;

            // Force redirection of DataContext. This is needed, because we detach the container from the visual tree and attach it to a diffrent one (the popup/dropdown) when the ribbon is minimized.
            this.groupsInnerContainer.SetBinding(DataContextProperty, new Binding(nameof(this.DataContext))
            {
                Source = this
            });

            ContextMenuService.Coerce(this);

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.contentContainer is null)
            {
                return base.MeasureOverride(constraint);
            }

            if (this.IsContextual
                && this.Group?.Visibility == Visibility.Collapsed)
            {
                return Size.Empty;
            }

            var baseConstraint = base.MeasureOverride(constraint);
            var totalWidth = this.contentContainer.DesiredSize.Width - (this.contentContainer.Margin.Left - this.contentContainer.Margin.Right);
            this.contentContainer.Child.Measure(SizeConstants.Infinite);
            var headerWidth = this.contentContainer.Child.DesiredSize.Width;

            if (totalWidth > headerWidth + (this.HeaderPadding.Left + this.HeaderPadding.Right))
            {
                if (DoubleUtil.AreClose(this.desiredWidth, 0) == false)
                {
                    // If header width is larger then tab increase tab width
                    if (constraint.Width > this.desiredWidth
                        && this.desiredWidth > totalWidth)
                    {
                        baseConstraint.Width = this.desiredWidth;
                    }
                    else
                    {
                        baseConstraint.Width = headerWidth + (this.HeaderPadding.Left + this.HeaderPadding.Right) + (this.contentContainer.Margin.Left + this.contentContainer.Margin.Right);
                    }
                }
            }

            if (DoubleUtil.AreClose(this.cachedWidth, baseConstraint.Width) == false
                && this.IsContextual
                && this.Group is not null)
            {
                this.cachedWidth = baseConstraint.Width;

                var contextualTabGroupContainer = UIHelper.GetParent<RibbonContextualGroupsContainer>(this.Group);
                contextualTabGroupContainer?.InvalidateMeasure();

                var ribbonTitleBar = UIHelper.GetParent<RibbonTitleBar>(this.Group);
                ribbonTitleBar?.ScheduleForceMeasureAndArrange();
            }

            return baseConstraint;
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var result = base.ArrangeOverride(arrangeBounds);

            var ribbonTitleBar = UIHelper.GetParent<RibbonTitleBar>(this.Group);
            ribbonTitleBar?.ScheduleForceMeasureAndArrange();

            return result;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.HeaderContentHost = this.GetTemplateChild("PART_HeaderContentHost") as FrameworkElement;

            this.contentContainer = this.GetTemplateChild("PART_ContentContainer") as Border;
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ReferenceEquals(e.Source, this)
                && e.ClickCount == 2)
            {
                e.Handled = true;

                if (this.TabControlParent is not null)
                {
                    var canMinimize = this.TabControlParent.CanMinimize;
                    if (canMinimize)
                    {
                        this.TabControlParent.IsMinimized = !this.TabControlParent.IsMinimized;
                    }
                }
            }
            else if (ReferenceEquals(e.Source, this)
                || this.IsSelected == false)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    if (this.TabControlParent is not null)
                    {
                        var newItem = this.TabControlParent.ItemContainerGenerator.ItemFromContainerOrContainerContent(this);

                        if (ReferenceEquals(this.TabControlParent.SelectedItem, newItem))
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

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonTabItemAutomationPeer(this);

        #endregion

        #region Private methods

        // Handles IsSelected property changes
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = (RibbonTabItem)d;
            var newValue = (bool)e.NewValue;

            if (newValue)
            {
                if (container.TabControlParent?.SelectedTabItem is not null
                    && ReferenceEquals(container.TabControlParent.SelectedTabItem, container) == false)
                {
                    container.TabControlParent.SelectedTabItem.IsSelected = false;
                }

                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }

            // Raise UI automation events on this RibbonTabItem
            if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected)
                || AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
            {
                //SelectorHelper.RaiseIsSelectedChangedAutomationEvent(container.TabControlParent, container, newValue);
                var peer = UIElementAutomationPeer.CreatePeerForElement(container) as RibbonTabItemAutomationPeer;
                peer?.RaiseTabSelectionEvents();
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

            if (this.groups is not null)
            {
                this.groups.CollectionChanged += this.OnGroupsCollectionChanged;
            }
        }

        private void UnSubscribeEvents()
        {
            if (this.groups is not null)
            {
                this.groups.CollectionChanged -= this.OnGroupsCollectionChanged;
            }
        }

        #endregion

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            if (this.TabControlParent?.SelectedItem is RibbonTabItem currentSelectedItem)
            {
                currentSelectedItem.IsSelected = false;
            }

            this.IsSelected = true;

            // This way keytips for delay loaded elements work correctly. Partially fixes #244.
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() => { }));

            return KeyTipPressedResult.Empty;
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
            if (this.TabControlParent is not null
                && this.TabControlParent.IsMinimized)
            {
                this.TabControlParent.IsDropDownOpen = false;
            }
        }

        /// <inheritdoc />
        void ILogicalChildSupport.AddLogicalChild(object child)
        {
            this.AddLogicalChild(child);
        }

        /// <inheritdoc />
        void ILogicalChildSupport.RemoveLogicalChild(object child)
        {
            this.RemoveLogicalChild(child);
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var baseEnumerator = base.LogicalChildren;
                while (baseEnumerator?.MoveNext() == true)
                {
                    yield return baseEnumerator.Current;
                }

                yield return this.GroupsContainer;

                if (this.Header is not null)
                {
                    yield return this.Header;
                }
            }
        }
    }
}