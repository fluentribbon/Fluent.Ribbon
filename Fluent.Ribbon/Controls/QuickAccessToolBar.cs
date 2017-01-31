// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
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
    public class QuickAccessToolBar : Control
    {
        #region Events

        /// <summary>
        /// Occured when items are added or removed from Quick Access toolbar
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsChanged = delegate { };

        #endregion

        #region Fields

        private DropDownButton toolBarDownButton;

        private DropDownButton menuDownButton;

        // Show above menu item
        private MenuItem showAbove;

        // Show below menu item
        private MenuItem showBelow;

        // Show quick acess menu item
        private MenuItem showCustimizeQuickAcess;

        // Items of quick access menu
        private ObservableCollection<QuickAccessMenuItem> quickAccessItems;

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

        private double cachedDeltaWidth;

        #endregion

        #region Properties

        #region Items

        /// <summary>
        /// Gets items collection
        /// </summary>
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
            this.ItemsChanged(this, e);
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
        public ObservableCollection<QuickAccessMenuItem> QuickAccessItems
        {
            get
            {
                if (this.quickAccessItems == null)
                {
                    this.quickAccessItems = new ObservableCollection<QuickAccessMenuItem>();
                    this.quickAccessItems.CollectionChanged += this.OnQuickAccessItemsCollectionChanged;
                }

                return this.quickAccessItems;
            }
        }

        /// <summary>
        /// Handles quick access menu items chages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        if (this.menuDownButton != null)
                        {
                            this.menuDownButton.Items.Insert(e.NewStartingIndex + i + 1, e.NewItems[i]);
                        }
                        else
                        {
                            this.AddLogicalChild(e.NewItems[i]);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        if (this.menuDownButton != null)
                        {
                            this.menuDownButton.Items.Remove(item);
                        }
                        else
                        {
                            this.RemoveLogicalChild(item);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        if (this.menuDownButton != null)
                        {
                            this.menuDownButton.Items.Remove(item);
                        }
                        else
                        {
                            this.RemoveLogicalChild(item);
                        }
                    }

                    var ii = 0;
                    foreach (var item in e.NewItems)
                    {
                        if (this.menuDownButton != null)
                        {
                            this.menuDownButton.Items.Insert(e.NewStartingIndex + ii + 1, item);
                        }
                        else
                        {
                            this.AddLogicalChild(item);
                        }

                        ii++;
                    }
                    break;
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

        #region LogicalChildren

        /// <summary>
        /// Gets an enumerator to the logical child elements
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                yield return this.rootPanel;
            }
        }

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

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static QuickAccessToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(typeof(QuickAccessToolBar)));
        }

        #endregion

        #region Override

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
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

            if(this.showCustimizeQuickAcess!=null)
            {
              this.showCustimizeQuickAcess.Click -= this.OnShowCustimizeQuickAcess;
            }

            this.showAbove = this.GetTemplateChild("PART_ShowAbove") as MenuItem;
            this.showBelow = this.GetTemplateChild("PART_ShowBelow") as MenuItem;
            this.showCustimizeQuickAcess = this.GetTemplateChild("PART_CustomizeQuickAccessToolbar") as MenuItem;

            if (this.showAbove != null)
            {
                this.showAbove.Click += this.OnShowAboveClick;
            }

            if (this.showBelow != null)
            {
                this.showBelow.Click += this.OnShowBelowClick;
            }

            if (this.showCustimizeQuickAcess != null)
            {
              var parentRibbon = this.Parent as Ribbon;

              if (parentRibbon != null && parentRibbon.CanCustomizeQuickAccessToolBar)
                this.showCustimizeQuickAcess.Visibility = Visibility.Visible;

              this.showCustimizeQuickAcess.Click += this.OnShowCustimizeQuickAcess;
            }


            if (this.menuDownButton != null)
            {
                foreach (var item in this.QuickAccessItems)
                {
                    this.menuDownButton.Items.Remove(item);
                    item.InvalidateProperty(QuickAccessMenuItem.TargetProperty);
                }
            }
            else if (this.quickAccessItems != null)
            {
                foreach (var item in this.quickAccessItems)
                {
                    this.RemoveLogicalChild(item);
                }
            }

            this.menuDownButton = this.GetTemplateChild("PART_MenuDownButton") as DropDownButton;

            if (this.menuDownButton != null
                && this.quickAccessItems != null)
            {
                for (var i = 0; i < this.quickAccessItems.Count; i++)
                {
                    this.menuDownButton.Items.Insert(i + 1, this.quickAccessItems[i]);
                    this.quickAccessItems[i].InvalidateProperty(QuickAccessMenuItem.TargetProperty);
                }
            }

            if (this.toolBarDownButton != null)
            {
                this.toolBarDownButton.DropDownOpened -= this.OnToolBarDownOpened;
                this.toolBarDownButton.DropDownClosed -= this.OnToolBarDownClosed;
            }

            this.toolBarDownButton = this.GetTemplateChild("PART_ToolbarDownButton") as DropDownButton;

            if (this.toolBarDownButton != null)
            {
                this.toolBarDownButton.DropDownOpened += this.OnToolBarDownOpened;
                this.toolBarDownButton.DropDownClosed += this.OnToolBarDownClosed;
            }

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
            this.cachedDeltaWidth = 0;
            this.cachedNonOverflowItemsCount = this.GetNonOverflowItemsCount(this.ActualWidth);
            this.cachedConstraint = new Size();
        }

        private bool _toolBarDownIsOpen;
        private void OnToolBarDownClosed(object sender, EventArgs e)
        {
            _toolBarDownIsOpen = false;
            this.toolBarOverflowPanel.Children.Clear();
        }

        private void OnToolBarDownOpened(object sender, EventArgs e)
        {
            if (this.toolBarOverflowPanel.Children.Count > 0)
            {
                this.toolBarOverflowPanel.Children.Clear();
            }

            for (var i = this.cachedNonOverflowItemsCount; i < this.Items.Count; i++)
            {
                this.toolBarOverflowPanel.Children.Add(this.Items[i]);
            }
            _toolBarDownIsOpen = true;
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

        private void OnShowCustimizeQuickAcess(object sender, RoutedEventArgs e)
        {
        
          var parentRibbon = this.Parent as Ribbon;

          if (parentRibbon != null)
          {
            Ribbon.CustomizeQuickAccessToolbarCommand.Execute(this, parentRibbon);
          }
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

        /// <summary>
        /// Called to remeasure a control. 
        /// </summary>
        /// <returns>The size of the control, up to the maximum specified by constraint</returns>
        /// <param name="constraint">The maximum size that the method can return</param>
        protected override Size MeasureOverride(Size constraint)
        {
            if ((this.cachedConstraint == constraint && !this.itemsHadChanged) || _toolBarDownIsOpen)
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

            if (this.HasOverflowItems == false)
            {
                this.toolBarOverflowPanel.Children.Clear();
            }

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
            var titleBar = RibbonControl.GetParentRibbon(this)?.TitleBar
                ?? UIHelper.GetParent<RibbonTitleBar>(this);

            titleBar?.InvalidateMeasure();

            titleBar?.UpdateLayout();
        }

        // Updates keys for keytip access
        private void UpdateKeyTips()
        {
            for (var i = 0; i < Math.Min(9, this.Items.Count); i++)
            {
                // 1, 2, 3, ... , 9
                KeyTip.SetKeys(this.Items[i], (i + 1).ToString(CultureInfo.InvariantCulture));
            }

            for (var i = 9; i < Math.Min(18, this.Items.Count); i++)
            {
                // 09, 08, 07, ... , 01
                KeyTip.SetKeys(this.Items[i], "0" + (18 - i).ToString(CultureInfo.InvariantCulture));
            }

            var startChar = 'A';
            for (var i = 18; i < Math.Min(9 + 9 + 26, this.Items.Count); i++)
            {
                // 0A, 0B, 0C, ... , 0Z
                KeyTip.SetKeys(this.Items[i], "0" + startChar++);
            }
        }

        private int GetNonOverflowItemsCount(double width)
        {
            if (DoubleUtil.AreClose(this.cachedDeltaWidth, 0)
                && this.rootPanel != null
                && this.toolBarPanel != null)
            {
                this.rootPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                this.cachedDeltaWidth = this.rootPanel.DesiredSize.Width - this.toolBarPanel.DesiredSize.Width;
            }

            var currentWidth = 0D;
            for (var i = 0; i < this.Items.Count; i++)
            {
                this.Items[i].Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                currentWidth += this.Items[i].DesiredSize.Width;

                if (currentWidth + this.cachedDeltaWidth > width)
                {
                    return i;
                }
            }

            return this.Items.Count;
        }

        #endregion
    }
}