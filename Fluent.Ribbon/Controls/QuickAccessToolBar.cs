// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using Fluent.Collections;
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents quick access toolbar
    /// </summary>
    [TemplatePart(Name = "PART_ShowAbove", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_ShowBelow", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_RootPanel", Type = typeof(Panel))]
    [ContentProperty(nameof(QuickAccessItems))]
    [TemplatePart(Name = "PART_MenuDownButton", Type = typeof(DropDownButton))]
    [TemplatePart(Name = "PART_ToolbarDownButton", Type = typeof(DropDownButton))]
    [TemplatePart(Name = "PART_ToolBarPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_ToolBarOverflowPanel", Type = typeof(Panel))]
    public class QuickAccessToolBar : Control, ILogicalChildSupport
    {
        #region Events

        /// <summary>
        /// Occured when items are added or removed from Quick Access toolbar
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsChanged;

        #endregion

        #region Fields

        private DropDownButton toolBarDownButton;

        internal DropDownButton MenuDownButton { get; private set; }

        // Show above menu item
        private MenuItem showAbove;

        // Show below menu item
        private MenuItem showBelow;

        // Items of quick access menu
        private ItemCollectionWithLogicalTreeSupport<QuickAccessMenuItem> quickAccessItems;

        // Root panel
        private Panel rootPanel;

        // ToolBar panel
        private Panel toolBarPanel;

        // ToolBar overflow panel
        private Panel toolBarOverflowPanel;

        // Items of quick access menu
        private ObservableCollection<UIElement> items;

        private Size cachedConstraint;
        private int cachedNonOverflowItemsCount = -1;

        // Itemc collection was changed
        private bool itemsHadChanged;

        private double cachedMenuDownButtonWidth;
        private double cachedOverflowDownButtonWidth;

        #endregion

        #region Properties

        #region Items

        /// <summary>
        /// Gets items collection
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ObservableCollection<UIElement> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new ObservableCollection<UIElement>();
                    this.items.CollectionChanged += this.OnItemsCollectionChanged;
                }

                return this.items;
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.cachedNonOverflowItemsCount = this.GetNonOverflowItemsCount(this.DesiredSize.Width);

            this.UpdateHasOverflowItems();
            this.itemsHadChanged = true;

            this.Refresh();

            this.UpdateKeyTips();

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<FrameworkElement>())
                {
                    item.SizeChanged -= this.OnChildSizeChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<FrameworkElement>())
                {
                    item.SizeChanged += this.OnChildSizeChanged;
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in this.Items.OfType<FrameworkElement>())
                {
                    item.SizeChanged -= this.OnChildSizeChanged;
                }
            }

            // Raise items changed event
            this.ItemsChanged?.Invoke(this, e);

            if (this.Items.Count == 0
                && this.toolBarDownButton != null)
            {
                this.toolBarDownButton.IsDropDownOpen = false;
            }
        }

        private void OnChildSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateMeasureOfTitleBar();
        }

        #endregion

        #region HasOverflowItems

        /// <summary>
        /// Gets whether QuickAccessToolBar has overflow items
        /// </summary>
        public bool HasOverflowItems
        {
            get { return (bool)this.GetValue(HasOverflowItemsProperty); }
            private set { this.SetValue(HasOverflowItemsPropertyKey, value); }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey HasOverflowItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasOverflowItems), typeof(bool), typeof(QuickAccessToolBar), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasOverflowItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;

        #endregion

        #region QuickAccessItems

        /// <summary>
        /// Gets quick access menu items
        /// </summary>
        public ItemCollectionWithLogicalTreeSupport<QuickAccessMenuItem> QuickAccessItems
        {
            get
            {
                if (this.quickAccessItems == null)
                {
                    this.quickAccessItems = new ItemCollectionWithLogicalTreeSupport<QuickAccessMenuItem>(this);
                }

                return this.quickAccessItems;
            }
        }

        #endregion

        #region ShowAboveRibbon

        /// <summary>
        /// Gets or sets whether quick access toolbar showes above ribbon
        /// </summary>
        public bool ShowAboveRibbon
        {
            get { return (bool)this.GetValue(ShowAboveRibbonProperty); }
            set { this.SetValue(ShowAboveRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAboveRibbon.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAboveRibbonProperty =
            DependencyProperty.Register(nameof(ShowAboveRibbon), typeof(bool),
            typeof(QuickAccessToolBar), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region CanQuickAccessLocationChanging

        /// <summary>
        /// Gets or sets whether user can change location of QAT
        /// </summary>
        public bool CanQuickAccessLocationChanging
        {
            get { return (bool)this.GetValue(CanQuickAccessLocationChangingProperty); }
            set { this.SetValue(CanQuickAccessLocationChangingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanQuickAccessLocationChanging.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanQuickAccessLocationChangingProperty =
            DependencyProperty.Register(nameof(CanQuickAccessLocationChanging), typeof(bool), typeof(QuickAccessToolBar), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region DropDownVisibility

        /// <summary>
        /// Gets or sets whether the Menu-DropDown is visible or not.
        /// </summary>
        public bool IsMenuDropDownVisible
        {
            get { return (bool)this.GetValue(IsMenuDropDownVisibleProperty); }
            set { this.SetValue(IsMenuDropDownVisibleProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsMenuDropDownVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsMenuDropDownVisibleProperty =
            DependencyProperty.Register(nameof(IsMenuDropDownVisible), typeof(bool), typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, OnIsMenuDropDownVisibleChanged));

        private static void OnIsMenuDropDownVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (QuickAccessToolBar)d;

            if ((bool)e.NewValue == false)
            {
                control.cachedMenuDownButtonWidth = 0;
            }
        }

        #endregion DropDownVisibility

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        static QuickAccessToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(typeof(QuickAccessToolBar)));
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public QuickAccessToolBar()
        {
            this.Loaded += (sender, args) => this.InvalidateMeasureOfTitleBar();
        }

        #endregion

        #region Override

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.showAbove != null)
            {
                this.showAbove.Click -= this.OnShowAboveClick;
            }

            if (this.showBelow != null)
            {
                this.showBelow.Click -= this.OnShowBelowClick;
            }

            this.showAbove = this.GetTemplateChild("PART_ShowAbove") as MenuItem;
            this.showBelow = this.GetTemplateChild("PART_ShowBelow") as MenuItem;

            if (this.showAbove != null)
            {
                this.showAbove.Click += this.OnShowAboveClick;
            }

            if (this.showBelow != null)
            {
                this.showBelow.Click += this.OnShowBelowClick;
            }

            if (this.MenuDownButton != null)
            {
                foreach (var item in this.QuickAccessItems)
                {
                    this.MenuDownButton.Items.Remove(item);
                    item.InvalidateProperty(QuickAccessMenuItem.TargetProperty);
                }

                this.QuickAccessItems.AquireLogicalOwnership();
            }

            this.MenuDownButton = this.GetTemplateChild("PART_MenuDownButton") as DropDownButton;

            if (this.MenuDownButton != null)
            {
                this.QuickAccessItems.ReleaseLogicalOwnership();

                for (var i = 0; i < this.quickAccessItems.Count; i++)
                {
                    this.MenuDownButton.Items.Insert(i + 1, this.quickAccessItems[i]);
                    this.quickAccessItems[i].InvalidateProperty(QuickAccessMenuItem.TargetProperty);
                }
            }

            this.toolBarDownButton = this.GetTemplateChild("PART_ToolbarDownButton") as DropDownButton;

            // ToolBar panels
            this.toolBarPanel = this.GetTemplateChild("PART_ToolBarPanel") as Panel;
            this.toolBarOverflowPanel = this.GetTemplateChild("PART_ToolBarOverflowPanel") as Panel;

            if (this.rootPanel != null)
            {
                this.RemoveLogicalChild(this.rootPanel);
            }

            this.rootPanel = this.GetTemplateChild("PART_RootPanel") as Panel;

            if (this.rootPanel != null)
            {
                this.AddLogicalChild(this.rootPanel);
            }

            // Clears cache
            this.cachedMenuDownButtonWidth = 0;
            this.cachedOverflowDownButtonWidth = 0;
            this.cachedNonOverflowItemsCount = this.GetNonOverflowItemsCount(this.ActualWidth);
            this.cachedConstraint = default;
        }

        /// <summary>
        /// Handles show below menu item click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnShowBelowClick(object sender, RoutedEventArgs e)
        {
            this.ShowAboveRibbon = false;
        }

        /// <summary>
        /// Handles show above menu item click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnShowAboveClick(object sender, RoutedEventArgs e)
        {
            this.ShowAboveRibbon = true;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.IsLoaded == false)
            {
                return base.MeasureOverride(constraint);
            }

            if ((this.cachedConstraint == constraint)
                && !this.itemsHadChanged)
            {
                return base.MeasureOverride(constraint);
            }

            var nonOverflowItemsCount = this.GetNonOverflowItemsCount(constraint.Width);

            if (this.itemsHadChanged == false
                && nonOverflowItemsCount == this.cachedNonOverflowItemsCount)
            {
                return base.MeasureOverride(constraint);
            }

            this.cachedNonOverflowItemsCount = nonOverflowItemsCount;
            this.UpdateHasOverflowItems();
            this.cachedConstraint = constraint;

            // Clear overflow panel to prevent items from having a visual/logical parent
            this.toolBarOverflowPanel.Children.Clear();

            if (this.itemsHadChanged)
            {
                // Refill toolbar
                this.toolBarPanel.Children.Clear();

                for (var i = 0; i < this.cachedNonOverflowItemsCount; i++)
                {
                    this.toolBarPanel.Children.Add(this.Items[i]);
                }

                this.itemsHadChanged = false;
            }
            else
            {
                if (this.cachedNonOverflowItemsCount > this.toolBarPanel.Children.Count)
                {
                    // Add needed items
                    var savedCount = this.toolBarPanel.Children.Count;
                    for (var i = savedCount; i < this.cachedNonOverflowItemsCount; i++)
                    {
                        this.toolBarPanel.Children.Add(this.Items[i]);
                    }
                }
                else
                {
                    // Remove nonneeded items
                    for (var i = this.toolBarPanel.Children.Count - 1; i >= this.cachedNonOverflowItemsCount; i--)
                    {
                        this.toolBarPanel.Children.Remove(this.Items[i]);
                    }
                }
            }

            // Move overflowing items to overflow panel
            for (var i = this.cachedNonOverflowItemsCount; i < this.Items.Count; i++)
            {
                this.toolBarOverflowPanel.Children.Add(this.Items[i]);
            }

            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// We have to use this function because setting a <see cref="DependencyProperty"/> very frequently is quite expensive
        /// </summary>
        private void UpdateHasOverflowItems()
        {
            var newValue = this.cachedNonOverflowItemsCount < this.Items.Count;

            // ReSharper disable RedundantCheckBeforeAssignment
            if (this.HasOverflowItems != newValue)
            // ReSharper restore RedundantCheckBeforeAssignment
            {
                // todo: code runs very often on startup
                this.HasOverflowItems = newValue;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// First calls <see cref="UIElement.InvalidateMeasure"/> and then <see cref="InvalidateMeasureOfTitleBar"/>
        /// </summary>
        public void Refresh()
        {
            this.InvalidateMeasure();

            this.InvalidateMeasureOfTitleBar();
        }

        private void InvalidateMeasureOfTitleBar()
        {
            if (this.IsLoaded == false)
            {
                return;
            }

            var titleBar = RibbonControl.GetParentRibbon(this)?.TitleBar
                           ?? UIHelper.GetParent<RibbonTitleBar>(this);

            titleBar?.ForceMeasureAndArrange();
        }

        /// <summary>
        /// Gets or sets a custom action to generate KeyTips for items in this control.
        /// </summary>
        public Action<QuickAccessToolBar> UpdateKeyTipsAction
        {
            get { return (Action<QuickAccessToolBar>)this.GetValue(UpdateKeyTipsActionProperty); }
            set { this.SetValue(UpdateKeyTipsActionProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="UpdateKeyTipsAction"/>.
        /// </summary>
        public static readonly DependencyProperty UpdateKeyTipsActionProperty =
            DependencyProperty.Register(nameof(UpdateKeyTipsAction), typeof(Action<QuickAccessToolBar>), typeof(QuickAccessToolBar), new PropertyMetadata(OnUpdateKeyTipsActionChanged));

        private static void OnUpdateKeyTipsActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var quickAccessToolBar = (QuickAccessToolBar)d;
            quickAccessToolBar.UpdateKeyTips();
        }

        private void UpdateKeyTips()
        {
            if (this.UpdateKeyTipsAction == null)
            {
                DefaultUpdateKeyTips(this);
                return;
            }

            this.UpdateKeyTipsAction(this);
        }

        // Updates keys for keytip access
        private static void DefaultUpdateKeyTips(QuickAccessToolBar quickAccessToolBar)
        {
            for (var i = 0; i < Math.Min(9, quickAccessToolBar.Items.Count); i++)
            {
                // 1, 2, 3, ... , 9
                KeyTip.SetKeys(quickAccessToolBar.Items[i], (i + 1).ToString(CultureInfo.InvariantCulture));
            }

            for (var i = 9; i < Math.Min(18, quickAccessToolBar.Items.Count); i++)
            {
                // 09, 08, 07, ... , 01
                KeyTip.SetKeys(quickAccessToolBar.Items[i], "0" + (18 - i).ToString(CultureInfo.InvariantCulture));
            }

            var startChar = 'A';
            for (var i = 18; i < Math.Min(9 + 9 + 26, quickAccessToolBar.Items.Count); i++)
            {
                // 0A, 0B, 0C, ... , 0Z
                KeyTip.SetKeys(quickAccessToolBar.Items[i], "0" + startChar++);
            }
        }

        private int GetNonOverflowItemsCount(in double width)
        {
            // Cache width of menuDownButton
            if (DoubleUtil.AreClose(this.cachedMenuDownButtonWidth, 0)
                && this.rootPanel != null
                && this.MenuDownButton != null
                && this.IsMenuDropDownVisible)
            {
                this.rootPanel.Measure(SizeConstants.Infinite);
                this.cachedMenuDownButtonWidth = this.MenuDownButton.DesiredSize.Width;
            }

            // Cache width of toolBarDownButton
            if (DoubleUtil.AreClose(this.cachedOverflowDownButtonWidth, 0)
                && this.rootPanel != null
                && this.MenuDownButton != null)
            {
                this.rootPanel.Measure(SizeConstants.Infinite);
                this.cachedOverflowDownButtonWidth = this.toolBarDownButton.DesiredSize.Width;
            }

            // If IsMenuDropDownVisible is true we have less width available
            var widthReductionWhenNotCompressed = this.IsMenuDropDownVisible ? this.cachedMenuDownButtonWidth : 0;

            return CalculateNonOverflowItems(this.Items, width, widthReductionWhenNotCompressed, this.cachedOverflowDownButtonWidth);
        }

        private static int CalculateNonOverflowItems(IList<UIElement> items, double maxAvailableWidth, double widthReductionWhenNotCompressed, double widthReductionWhenCompressed)
        {
            // Calculate how many items we can fit into the available width
            var maxPossibleItems = GetMaxPossibleItems(maxAvailableWidth - widthReductionWhenNotCompressed, true);

            if (maxPossibleItems < items.Count)
            {
                // If we can't fit all items into the available width
                // we have to reduce the available width as the overflow button also needs space.
                var availableWidth = maxAvailableWidth - widthReductionWhenCompressed;

                return GetMaxPossibleItems(availableWidth, false);
            }

            return items.Count;

            int GetMaxPossibleItems(double availableWidth, bool measureItems)
            {
                var currentWidth = 0D;

                for (var i = 0; i < items.Count; i++)
                {
                    var currentItem = items[i];

                    if (measureItems)
                    {
                        currentItem.Measure(SizeConstants.Infinite);
                    }

                    currentWidth += currentItem.DesiredSize.Width;

                    if (currentWidth > availableWidth)
                    {
                        return i;
                    }
                }

                return items.Count;
            }
        }

        #endregion

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonQuickAccessToolBarAutomationPeer(this);

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

                yield return this.rootPanel;

                foreach (var item in this.QuickAccessItems.GetLogicalChildren())
                {
                    yield return item;
                }
            }
        }
    }
}