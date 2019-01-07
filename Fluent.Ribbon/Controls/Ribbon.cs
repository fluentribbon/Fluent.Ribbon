// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using Fluent.Internal.KnownBoxes;
    using Fluent.Localization;
    using WindowChrome = ControlzEx.Windows.Shell.WindowChrome;

    // TODO: improve style parts naming & using

    /// <summary>
    /// Represents the main Ribbon control which consists of multiple tabs, each of which
    /// containing groups of controls.  The Ribbon also provides improved context
    /// menus, enhanced screen tips, and keyboard shortcuts.
    /// </summary>
    [ContentProperty(nameof(Tabs))]
    [DefaultProperty(nameof(Tabs))]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_RibbonTabControl", Type = typeof(RibbonTabControl))]
    [TemplatePart(Name = "PART_QuickAccessToolBar", Type = typeof(QuickAccessToolBar))]
    public class Ribbon : Control
    {
        private IRibbonStateStorage ribbonStateStorage;

        /// <summary>
        /// Gets the current instance for storing the state of this control.
        /// </summary>
        public IRibbonStateStorage RibbonStateStorage => this.ribbonStateStorage ?? (this.ribbonStateStorage = this.CreateRibbonStateStorage());

        /// <summary>
        /// Create a new instance for storing the state of this control.
        /// </summary>
        /// <returns>Instance of a state storage class.</returns>
        protected virtual IRibbonStateStorage CreateRibbonStateStorage()
        {
            return new RibbonStateStorage(this);
        }

        #region Constants

        /// <summary>
        /// Minimal width of ribbon parent window
        /// </summary>
        public const double MinimalVisibleWidth = 300;

        /// <summary>
        /// Minimal height of ribbon parent window
        /// </summary>
        public const double MinimalVisibleHeight = 250;

        #endregion

        #region ContextMenu

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsDefaultContextMenuEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsDefaultContextMenuEnabledProperty = DependencyProperty.Register(nameof(IsDefaultContextMenuEnabled), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the default context menu should be enabled/used.
        /// </summary>
        public bool IsDefaultContextMenuEnabled
        {
            get { return (bool)this.GetValue(IsDefaultContextMenuEnabledProperty); }
            set { this.SetValue(IsDefaultContextMenuEnabledProperty, value); }
        }

        private static readonly Dictionary<int, System.Windows.Controls.ContextMenu> contextMenus = new Dictionary<int, System.Windows.Controls.ContextMenu>();

        /// <summary>
        /// Context menu for ribbon in current thread
        /// </summary>
        public static System.Windows.Controls.ContextMenu RibbonContextMenu
        {
            get
            {
                if (!contextMenus.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                {
                    InitRibbonContextMenu();
                }

                return contextMenus[Thread.CurrentThread.ManagedThreadId];
            }
        }

        // Context menu owner ribbon
        private static Ribbon contextMenuOwner;

        // Context menu items
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem AddToQuickAccessMenuItem
        {
            get { return addToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addGroupToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem AddGroupToQuickAccessMenuItem
        {
            get { return addGroupToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addMenuToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem AddMenuToQuickAccessMenuItem
        {
            get { return addMenuToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addGalleryToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem AddGalleryToQuickAccessMenuItem
        {
            get { return addGalleryToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> removeFromQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem RemoveFromQuickAccessMenuItem
        {
            get { return removeFromQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> showQuickAccessToolbarBelowTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem ShowQuickAccessToolbarBelowTheRibbonMenuItem
        {
            get { return showQuickAccessToolbarBelowTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> showQuickAccessToolbarAboveTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem ShowQuickAccessToolbarAboveTheRibbonMenuItem
        {
            get { return showQuickAccessToolbarAboveTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> minimizeTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem MinimizeTheRibbonMenuItem
        {
            get { return minimizeTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> customizeQuickAccessToolbarMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem CustomizeQuickAccessToolbarMenuItem
        {
            get { return customizeQuickAccessToolbarMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> customizeTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();

        private static System.Windows.Controls.MenuItem CustomizeTheRibbonMenuItem
        {
            get { return customizeTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, Separator> firstSeparatorDictionary = new Dictionary<int, Separator>();

        private static Separator FirstSeparator
        {
            get { return firstSeparatorDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        private static readonly Dictionary<int, Separator> secondSeparatorDictionary = new Dictionary<int, Separator>();

        private static Separator SecondSeparator
        {
            get { return secondSeparatorDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        // Initialize ribbon context menu
        private static void InitRibbonContextMenu()
        {
            contextMenus.Add(Thread.CurrentThread.ManagedThreadId, new ContextMenu());
            RibbonContextMenu.Opened += OnContextMenuOpened;
        }

        private static void InitRibbonContextMenuItems()
        {
            // Add to quick access toolbar
            addToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(AddToQuickAccessCommand));
            RibbonContextMenu.Items.Add(AddToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, AddToQuickAccessMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuAddItem), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, AddToQuickAccessMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Add group to quick access toolbar
            addGroupToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(AddToQuickAccessCommand));
            RibbonContextMenu.Items.Add(AddGroupToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, AddGroupToQuickAccessMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuAddGroup), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, AddGroupToQuickAccessMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Add menu item to quick access toolbar
            addMenuToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(AddToQuickAccessCommand));
            RibbonContextMenu.Items.Add(AddMenuToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, AddMenuToQuickAccessMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuAddMenu), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, AddMenuToQuickAccessMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Add gallery to quick access toolbar
            addGalleryToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(AddToQuickAccessCommand));
            RibbonContextMenu.Items.Add(AddGalleryToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, AddGalleryToQuickAccessMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuAddGallery), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, AddGalleryToQuickAccessMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Remove from quick access toolbar
            removeFromQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(RemoveFromQuickAccessCommand));
            RibbonContextMenu.Items.Add(RemoveFromQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, RemoveFromQuickAccessMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuRemoveItem), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, RemoveFromQuickAccessMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Separator
            firstSeparatorDictionary.Add(Thread.CurrentThread.ManagedThreadId, new Separator());
            RibbonContextMenu.Items.Add(FirstSeparator);

            // Customize quick access toolbar
            customizeQuickAccessToolbarMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(CustomizeQuickAccessToolbarCommand));
            RibbonContextMenu.Items.Add(CustomizeQuickAccessToolbarMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, CustomizeQuickAccessToolbarMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuCustomizeQuickAccessToolBar), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, CustomizeQuickAccessToolbarMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Show quick access below the ribbon
            showQuickAccessToolbarBelowTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(ShowQuickAccessBelowCommand));
            RibbonContextMenu.Items.Add(ShowQuickAccessToolbarBelowTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, ShowQuickAccessToolbarBelowTheRibbonMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuShowBelow), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, ShowQuickAccessToolbarBelowTheRibbonMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Show quick access above the ribbon
            showQuickAccessToolbarAboveTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(ShowQuickAccessAboveCommand));
            RibbonContextMenu.Items.Add(ShowQuickAccessToolbarAboveTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, ShowQuickAccessToolbarAboveTheRibbonMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuShowAbove), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, ShowQuickAccessToolbarAboveTheRibbonMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Separator
            secondSeparatorDictionary.Add(Thread.CurrentThread.ManagedThreadId, new Separator());
            RibbonContextMenu.Items.Add(SecondSeparator);

            // Customize the ribbon
            customizeTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(CustomizeTheRibbonCommand));
            RibbonContextMenu.Items.Add(CustomizeTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, CustomizeTheRibbonMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuCustomizeRibbon), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, CustomizeTheRibbonMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Minimize the ribbon
            minimizeTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, CreateMenuItemForContextMenu(ToggleMinimizeTheRibbonCommand));
            RibbonContextMenu.Items.Add(MinimizeTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, MinimizeTheRibbonMenuItem, nameof(RibbonLocalizationBase.RibbonContextMenuMinimizeRibbon), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, MinimizeTheRibbonMenuItem, nameof(System.Windows.Controls.ContextMenu.PlacementTarget), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);
        }

        private static MenuItem CreateMenuItemForContextMenu(ICommand command)
        {
            return new MenuItem
                   {
                       Command = command,
                       CanAddToQuickAccessToolBar = false,
                       ContextMenu = null
                   };
        }

        /// <inheritdoc />
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            contextMenuOwner = this;
            base.OnContextMenuOpening(e);
        }

        /// <inheritdoc />
        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            contextMenuOwner = null;
            base.OnContextMenuClosing(e);
        }

        private void OnQuickAccessContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            this.OnContextMenuOpening(e);
        }

        private void OnQuickAccessContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            this.OnContextMenuClosing(e);
        }

        // Occurs when context menu is opening
        private static void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            var ribbon = contextMenuOwner;

            if (RibbonContextMenu == null
                || ribbon == null)
            {
                return;
            }

            if (ribbon.IsDefaultContextMenuEnabled
                && RibbonContextMenu.Items.Count == 0)
            {
                InitRibbonContextMenuItems();
            }

            if (ribbon.IsDefaultContextMenuEnabled == false)
            {
                foreach (UIElement item in RibbonContextMenu.Items)
                {
                    item.Visibility = Visibility.Collapsed;
                }

                RibbonContextMenu.IsOpen = false;
                return;
            }

            AddToQuickAccessMenuItem.CommandTarget = ribbon;
            AddGroupToQuickAccessMenuItem.CommandTarget = ribbon;
            AddMenuToQuickAccessMenuItem.CommandTarget = ribbon;
            AddGalleryToQuickAccessMenuItem.CommandTarget = ribbon;
            RemoveFromQuickAccessMenuItem.CommandTarget = ribbon;
            CustomizeQuickAccessToolbarMenuItem.CommandTarget = ribbon;
            CustomizeTheRibbonMenuItem.CommandTarget = ribbon;
            MinimizeTheRibbonMenuItem.CommandTarget = ribbon;
            ShowQuickAccessToolbarBelowTheRibbonMenuItem.CommandTarget = ribbon;
            ShowQuickAccessToolbarAboveTheRibbonMenuItem.CommandTarget = ribbon;

            // Hide items for ribbon controls
            AddToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            AddGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            AddMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            AddGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            RemoveFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            FirstSeparator.Visibility = Visibility.Collapsed;

            // Hide customize quick access menu item
            CustomizeQuickAccessToolbarMenuItem.Visibility = Visibility.Collapsed;
            SecondSeparator.Visibility = Visibility.Collapsed;

            // Set minimize the ribbon menu item state
            MinimizeTheRibbonMenuItem.IsChecked = ribbon.IsMinimized;

            // Set minimize the ribbon menu item visibility
            MinimizeTheRibbonMenuItem.Visibility = ribbon.CanMinimize
                                                       ? Visibility.Visible
                                                       : Visibility.Collapsed;

            // Set customize the ribbon menu item visibility
            CustomizeTheRibbonMenuItem.Visibility = ribbon.CanCustomizeRibbon
                                                        ? Visibility.Visible
                                                        : Visibility.Collapsed;

            // Hide quick access position menu items
            ShowQuickAccessToolbarBelowTheRibbonMenuItem.Visibility = Visibility.Collapsed;
            ShowQuickAccessToolbarAboveTheRibbonMenuItem.Visibility = Visibility.Collapsed;

            // If quick access toolbar is visible show
            if (ribbon.IsQuickAccessToolBarVisible)
            {
                // Set quick access position menu items visibility
                if (ribbon.CanQuickAccessLocationChanging)
                {
                    if (ribbon.ShowQuickAccessToolBarAboveRibbon)
                    {
                        ShowQuickAccessToolbarBelowTheRibbonMenuItem.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        ShowQuickAccessToolbarAboveTheRibbonMenuItem.Visibility = Visibility.Visible;
                    }
                }

                if (ribbon.CanCustomizeQuickAccessToolBar)
                {
                    CustomizeQuickAccessToolbarMenuItem.Visibility = Visibility.Visible;
                }

                if (ribbon.CanQuickAccessLocationChanging
                    || ribbon.CanCustomizeQuickAccessToolBar)
                {
                    SecondSeparator.Visibility = Visibility.Visible;
                }
                else
                {
                    SecondSeparator.Visibility = Visibility.Collapsed;
                }

                if (ribbon.CanCustomizeQuickAccessToolBarItems)
                {
                    // Gets control that raise menu opened
                    var control = RibbonContextMenu.PlacementTarget;
                    AddToQuickAccessCommand.CanExecute(null, control);
                    RemoveFromQuickAccessCommand.CanExecute(null, control);

                    //Debug.WriteLine("Menu opened on "+control);
                    if (control != null)
                    {
                        FirstSeparator.Visibility = Visibility.Visible;

                        // Check for value because remove is only possible in the context menu of items in QA which represent the value for QA-items
                        if (ribbon.QuickAccessElements.ContainsValue(control))
                        {
                            // Control is on quick access
                            RemoveFromQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is System.Windows.Controls.MenuItem)
                        {
                            // Control is menu item
                            AddMenuToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is Gallery ||
                                 control is InRibbonGallery)
                        {
                            // Control is gallery
                            AddGalleryToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is RibbonGroupBox)
                        {
                            // Control is group box
                            AddGroupToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is IQuickAccessItemProvider)
                        {
                            // Its other control
                            AddToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            FirstSeparator.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }

            // We have to close the context menu if no items are visible
            if (RibbonContextMenu.Items.OfType<System.Windows.Controls.MenuItem>().All(x => x.Visibility == Visibility.Collapsed))
            {
                RibbonContextMenu.IsOpen = false;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when selected tab has been changed (be aware that SelectedTab can be null)
        /// </summary>
        public event SelectionChangedEventHandler SelectedTabChanged;

        /// <summary>
        /// Occurs when customize the ribbon
        /// </summary>
        public event EventHandler CustomizeTheRibbon;

        /// <summary>
        /// Occurs when customize quick access toolbar
        /// </summary>
        public event EventHandler CustomizeQuickAccessToolbar;

        /// <summary>
        /// Occurs when IsMinimized property is changing
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsMinimizedChanged;

        /// <summary>
        /// Occurs when IsCollapsed property is changing
        /// </summary>
        public event DependencyPropertyChangedEventHandler IsCollapsedChanged;

        #endregion

        #region Fields

        private ObservableCollection<Key> keyTipKeys;

        // Collection of contextual tab groups
        private ObservableCollection<RibbonContextualTabGroup> contextualGroups;

        // Collection of tabs
        private ObservableCollection<RibbonTabItem> tabs;

        // Collection of toolbar items
        private ObservableCollection<UIElement> toolBarItems;

        // Ribbon quick access toolbar

        // Ribbon layout root
        private Panel layoutRoot;

        // Handles F10, Alt and so on
        private readonly KeyTipService keyTipService;

        // Collection of quickaccess menu items
        private ObservableCollection<QuickAccessMenuItem> quickAccessItems;

        // Currently added in QAT items

        private Window ownerWindow;

        #endregion

        #region Properties

        #region Menu

        /// <summary>
        /// Gets or sets file menu control (can be application menu button, backstage button and so on)
        /// </summary>
        public FrameworkElement Menu
        {
            get { return (FrameworkElement)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Menu"/>.
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(FrameworkElement), typeof(Ribbon), new FrameworkPropertyMetadata(default(FrameworkElement), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, OnMenuChanged));

        private static void OnMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddOrRemoveLogicalChildOnPropertyChanged(d, e);
        }

        #endregion

        #region StartScreen

        /// <summary>
        /// Property for defining the start screen.
        /// </summary>
        public StartScreen StartScreen
        {
            get { return (StartScreen)this.GetValue(StartScreenProperty); }
            set { this.SetValue(StartScreenProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="StartScreen"/>
        /// </summary>
        public static readonly DependencyProperty StartScreenProperty =
            DependencyProperty.Register(nameof(StartScreen), typeof(StartScreen), typeof(Ribbon), new FrameworkPropertyMetadata(default(StartScreen), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, OnStartScreenChanged));

        private static void OnStartScreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddOrRemoveLogicalChildOnPropertyChanged(d, e);
        }

        #endregion

        #region QuickAccessToolBar

        /// <summary>
        /// Property for defining the QuickAccessToolBar.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public QuickAccessToolBar QuickAccessToolBar
        {
            get { return (QuickAccessToolBar)this.GetValue(QuickAccessToolBarProperty); }
            private set { this.SetValue(QuickAccessToolBarPropertyKey, value); }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey QuickAccessToolBarPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(QuickAccessToolBar), typeof(QuickAccessToolBar), typeof(Ribbon), new FrameworkPropertyMetadata(default(QuickAccessToolBar), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure, OnQuickAccessToolBarChanged));

        private static void OnQuickAccessToolBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddOrRemoveLogicalChildOnPropertyChanged(d, e);
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="QuickAccessToolBar"/>
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarProperty = QuickAccessToolBarPropertyKey.DependencyProperty;

        #endregion

        #region TabControl

        /// <summary>
        /// Property for defining the TabControl.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RibbonTabControl TabControl
        {
            get { return (RibbonTabControl)this.GetValue(TabControlProperty); }
            private set { this.SetValue(TabControlPropertyKey, value); }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey TabControlPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TabControl), typeof(RibbonTabControl), typeof(Ribbon), new FrameworkPropertyMetadata(default(RibbonTabControl), FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TabControl"/>
        /// </summary>
        public static readonly DependencyProperty TabControlProperty = TabControlPropertyKey.DependencyProperty;

        #endregion

        /// <summary>
        /// Gets or sets selected tab item
        /// </summary>
        public RibbonTabItem SelectedTabItem
        {
            get { return (RibbonTabItem)this.GetValue(SelectedTabItemProperty); }
            set { this.SetValue(SelectedTabItemProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="SelectedTabItem"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register(nameof(SelectedTabItem), typeof(RibbonTabItem), typeof(Ribbon), new PropertyMetadata(OnSelectedTabItemChanged));

        private static void OnSelectedTabItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            if (ribbon.TabControl != null)
            {
                ribbon.TabControl.SelectedItem = e.NewValue;
            }

            if (e.NewValue is RibbonTabItem selectedItem
                && ribbon.Tabs.Contains(selectedItem))
            {
                ribbon.SelectedTabIndex = ribbon.Tabs.IndexOf(selectedItem);
            }
            else
            {
                ribbon.SelectedTabIndex = -1;
            }
        }

        /// <summary>
        /// Gets or sets selected tab index
        /// </summary>
        public int SelectedTabIndex
        {
            get { return (int)this.GetValue(SelectedTabIndexProperty); }
            set { this.SetValue(SelectedTabIndexProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="SelectedTabIndex"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedTabIndexProperty =
            DependencyProperty.Register(nameof(SelectedTabIndex), typeof(int), typeof(Ribbon), new PropertyMetadata(-1, OnSelectedTabIndexChanged));

        private static void OnSelectedTabIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            var selectedIndex = (int)e.NewValue;

            if (ribbon.TabControl != null)
            {
                ribbon.TabControl.SelectedIndex = selectedIndex;
            }

            if (selectedIndex >= 0
                && selectedIndex < ribbon.Tabs.Count)
            {
                ribbon.SelectedTabItem = ribbon.Tabs[selectedIndex];
            }
            else
            {
                ribbon.SelectedTabItem = null;
            }
        }

        private static void AddOrRemoveLogicalChildOnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            if (e.OldValue != null)
            {
                ribbon.RemoveLogicalChild(e.OldValue);
            }

            if (e.NewValue != null)
            {
                ribbon.AddLogicalChild(e.NewValue);
            }
        }

        /// <summary>
        /// Gets the first visible TabItem
        /// </summary>
        public RibbonTabItem FirstVisibleItem => this.GetFirstVisibleItem();

        /// <summary>
        /// Gets the last visible TabItem
        /// </summary>
        public RibbonTabItem LastVisibleItem => this.GetLastVisibleItem();

        /// <summary>
        /// Gets currently active quick access elements.
        /// </summary>
        protected Dictionary<UIElement, UIElement> QuickAccessElements { get; } = new Dictionary<UIElement, UIElement>();

        /// <summary>
        /// Gets a copy of currently active quick access elements.
        /// </summary>
        public IDictionary<UIElement, UIElement> GetQuickAccessElements() => this.QuickAccessElements.ToDictionary(x => x.Key, y => y.Value);

        #region TitelBar

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get { return (RibbonTitleBar)this.GetValue(TitleBarProperty); }
            set { this.SetValue(TitleBarProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = DependencyProperty.Register(nameof(TitleBar), typeof(RibbonTitleBar), typeof(Ribbon), new PropertyMetadata(OnTitleBarChanged));

        private static void OnTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;

            if (e.OldValue is RibbonTitleBar oldValue)
            {
                foreach (var ribbonContextualTabGroup in ribbon.ContextualGroups)
                {
                    oldValue.Items.Remove(ribbonContextualTabGroup);
                }

                // Make sure everything is cleared
                oldValue.Items.Clear();

                ribbon.RemoveQuickAccessToolBarFromTitleBar(oldValue);
            }

            if (e.NewValue is RibbonTitleBar newValue)
            {
                foreach (var contextualTabGroup in ribbon.ContextualGroups)
                {
                    newValue.Items.Add(contextualTabGroup);
                }

                if (ribbon.ShowQuickAccessToolBarAboveRibbon)
                {
                    ribbon.MoveQuickAccessToolBarToTitleBar(newValue);
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets whether quick access toolbar showes above ribbon
        /// </summary>
        public bool ShowQuickAccessToolBarAboveRibbon
        {
            get { return (bool)this.GetValue(ShowQuickAccessToolBarAboveRibbonProperty); }
            set { this.SetValue(ShowQuickAccessToolBarAboveRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAboveRibbon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowQuickAccessToolBarAboveRibbonProperty =
            DependencyProperty.Register(nameof(ShowQuickAccessToolBarAboveRibbon), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox, OnShowQuickAccessToolBarAboveRibbonChanged));

        /// <summary>
        /// Handles ShowQuickAccessToolBarAboveRibbon property changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnShowQuickAccessToolBarAboveRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;

            if (ribbon.TitleBar != null)
            {
                if ((bool)e.NewValue)
                {
                    ribbon.MoveQuickAccessToolBarToTitleBar(ribbon.TitleBar);
                }
                else
                {
                    ribbon.RemoveQuickAccessToolBarFromTitleBar(ribbon.TitleBar);
                }

                ribbon.TitleBar.InvalidateMeasure();
            }

            ribbon.RibbonStateStorage.SaveTemporary();
        }

        /// <summary>
        /// Gets or sets the height which is used to render the window title.
        /// </summary>
        public double QuickAccessToolBarHeight
        {
            get { return (double)this.GetValue(QuickAccessToolBarHeightProperty); }
            set { this.SetValue(QuickAccessToolBarHeightProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="QuickAccessToolBarHeight"/>.
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarHeightProperty =
            DependencyProperty.Register(nameof(QuickAccessToolBarHeight), typeof(double), typeof(Ribbon), new PropertyMetadata(23D));

        /// <summary>
        /// Gets collection of contextual tab groups
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonContextualTabGroup> ContextualGroups
        {
            get
            {
                if (this.contextualGroups == null)
                {
                    this.contextualGroups = new ObservableCollection<RibbonContextualTabGroup>();
                    this.contextualGroups.CollectionChanged += this.OnContextualGroupsCollectionChanged;
                }

                return this.contextualGroups;
            }
        }

        /// <summary>
        /// Handles collection of contextual tab groups ghanges
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnContextualGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.TitleBar?.Items.Insert(e.NewStartingIndex + i, e.NewItems[i]);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.TitleBar?.Items.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        this.TitleBar?.Items.Remove(item);
                    }

                    foreach (var item in e.NewItems)
                    {
                        this.TitleBar?.Items.Add(item);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    this.TitleBar?.Items.Clear();
                    break;
            }
        }

        /// <summary>
        /// gets collection of ribbon tabs
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonTabItem> Tabs
        {
            get
            {
                if (this.tabs == null)
                {
                    this.tabs = new ObservableCollection<RibbonTabItem>();
                    this.tabs.CollectionChanged += this.OnTabsCollectionChanged;
                }

                return this.tabs;
            }
        }

        /// <summary>
        /// Handles collection of ribbon tabs changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnTabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.TabControl == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.TabControl.Items.Insert(e.NewStartingIndex + i, e.NewItems[i]);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.TabControl.Items.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        this.TabControl.Items.Remove(item);
                    }

                    foreach (var item in e.NewItems)
                    {
                        this.TabControl.Items.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.TabControl.Items.Clear();
                    break;
            }
        }

        /// <summary>
        /// Gets collection of toolbar items
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<UIElement> ToolBarItems
        {
            get
            {
                if (this.toolBarItems == null)
                {
                    this.toolBarItems = new ObservableCollection<UIElement>();
                    this.toolBarItems.CollectionChanged += this.OnToolbarItemsCollectionChanged;
                }

                return this.toolBarItems;
            }
        }

        /// <summary>
        /// Handles collection of toolbar items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        this.TabControl?.ToolBarItems.Insert(e.NewStartingIndex + i, (UIElement)e.NewItems[i]);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems)
                    {
                        this.TabControl?.ToolBarItems.Remove(item as UIElement);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems)
                    {
                        this.TabControl?.ToolBarItems.Remove(item as UIElement);
                    }

                    foreach (var item in e.NewItems)
                    {
                        this.TabControl?.ToolBarItems.Add(item as UIElement);
                    }

                    break;
            }
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Menu != null)
                {
                    yield return this.Menu;
                }

                if (this.StartScreen != null)
                {
                    yield return this.StartScreen;
                }

                if (this.QuickAccessToolBar != null)
                {
                    yield return this.QuickAccessToolBar;
                }

                if (this.TabControl?.ToolbarPanel != null)
                {
                    yield return this.TabControl.ToolbarPanel;
                }

                if (this.layoutRoot != null)
                {
                    yield return this.layoutRoot;
                }
            }
        }

        /// <summary>
        /// Gets collection of quick access menu items
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        /// Handles collection of quick access menu items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (var i = 0; i < e.NewItems.Count; i++)
                    {
                        var menuItem = (QuickAccessMenuItem)e.NewItems[i];
                        this.QuickAccessToolBar?.QuickAccessItems.Insert(e.NewStartingIndex + i, menuItem);
                        menuItem.Ribbon = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<QuickAccessMenuItem>())
                    {
                        var menuItem = item;
                        this.QuickAccessToolBar?.QuickAccessItems.Remove(menuItem);
                        menuItem.Ribbon = null;
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.OldItems.OfType<QuickAccessMenuItem>())
                    {
                        var menuItem = item;
                        this.QuickAccessToolBar?.QuickAccessItems.Remove(menuItem);
                        menuItem.Ribbon = null;
                    }

                    foreach (var item in e.NewItems.OfType<QuickAccessMenuItem>())
                    {
                        var menuItem = item;
                        this.QuickAccessToolBar?.QuickAccessItems.Add(menuItem);
                        menuItem.Ribbon = this;
                    }

                    break;
            }
        }

        /// <summary>
        /// Gets or sets whether Customize Quick Access Toolbar menu item is shown
        /// </summary>
        public bool CanCustomizeQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanCustomizeQuickAccessToolBarProperty); }
            set { this.SetValue(CanCustomizeQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeQuickAccessToolBar.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeQuickAccessToolBarProperty =
            DependencyProperty.Register(nameof(CanCustomizeQuickAccessToolBar), typeof(bool),
            typeof(Ribbon), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether items can be added or removed from the quick access toolbar by users.
        /// </summary>
        public bool CanCustomizeQuickAccessToolBarItems
        {
            get { return (bool)this.GetValue(CanCustomizeQuickAccessToolBarItemsProperty); }
            set { this.SetValue(CanCustomizeQuickAccessToolBarItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeQuickAccessToolBarItems.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeQuickAccessToolBarItemsProperty =
            DependencyProperty.Register(nameof(CanCustomizeQuickAccessToolBarItems), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether the QAT Menu-DropDown is visible or not.
        /// </summary>
        public bool IsQuickAccessToolBarMenuDropDownVisible
        {
            get { return (bool)this.GetValue(IsQuickAccessToolBarMenuDropDownVisibleProperty); }
            set { this.SetValue(IsQuickAccessToolBarMenuDropDownVisibleProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsQuickAccessToolBarMenuDropDownVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsQuickAccessToolBarMenuDropDownVisibleProperty =
            DependencyProperty.Register(nameof(IsQuickAccessToolBarMenuDropDownVisible), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether Customize Ribbon menu item is shown
        /// </summary>
        public bool CanCustomizeRibbon
        {
            get { return (bool)this.GetValue(CanCustomizeRibbonProperty); }
            set { this.SetValue(CanCustomizeRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeRibbon.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeRibbonProperty =
            DependencyProperty.Register(nameof(CanCustomizeRibbon), typeof(bool),
            typeof(Ribbon), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets whether ribbon can be minimized
        /// </summary>
        public bool CanMinimize
        {
            get { return (bool)this.GetValue(CanMinimizeProperty); }
            set { this.SetValue(CanMinimizeProperty, value); }
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
        public static readonly DependencyProperty IsMinimizedProperty =
            DependencyProperty.Register(nameof(IsMinimized), typeof(bool),
            typeof(Ribbon), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsMinimizedChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanMinimize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register(nameof(CanMinimize), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        private static void OnIsMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;

            ribbon.IsMinimizedChanged?.Invoke(ribbon, e);
        }

        /// <summary>
        /// Gets or sets the height of the gap between the ribbon and the regular window content
        /// </summary>
        public double ContentGapHeight
        {
            get { return (double)this.GetValue(ContentGapHeightProperty); }
            set { this.SetValue(ContentGapHeightProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="ContentGapHeight"/>
        /// </summary>
        public static readonly DependencyProperty ContentGapHeightProperty =
            DependencyProperty.Register(nameof(ContentGapHeight), typeof(double), typeof(Ribbon), new PropertyMetadata(RibbonTabControl.DefaultContentGapHeight));

        /// <summary>
        /// Gets or sets the height of the ribbon content area
        /// </summary>
        public double ContentHeight
        {
            get { return (double)this.GetValue(ContentHeightProperty); }
            set { this.SetValue(ContentHeightProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ContentHeight"/>
        /// </summary>
        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register(nameof(ContentHeight), typeof(double), typeof(Ribbon), new PropertyMetadata(RibbonTabControl.DefaultContentHeight));

        // todo check if IsCollapsed and IsAutomaticCollapseEnabled should be reduced to one shared property for RibbonWindow and Ribbon

        /// <summary>
        /// Gets whether ribbon is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)this.GetValue(IsCollapsedProperty); }
            set { this.SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="IsCollapsed"/>
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool),
            typeof(Ribbon), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsCollapsedChanged));

        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            ribbon.IsCollapsedChanged?.Invoke(ribbon, e);
        }

        /// <summary>
        /// Defines if the Ribbon should automatically set <see cref="IsCollapsed"/> when the width or height of the owner window drop under <see cref="MinimalVisibleWidth"/> or <see cref="MinimalVisibleHeight"/>
        /// </summary>
        public bool IsAutomaticCollapseEnabled
        {
            get { return (bool)this.GetValue(IsAutomaticCollapseEnabledProperty); }
            set { this.SetValue(IsAutomaticCollapseEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAutomaticCollapseEnabledProperty =
            DependencyProperty.Register(nameof(IsAutomaticCollapseEnabled), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets or sets whether QAT is visible
        /// </summary>
        public bool IsQuickAccessToolBarVisible
        {
            get { return (bool)this.GetValue(IsQuickAccessToolBarVisibleProperty); }
            set { this.SetValue(IsQuickAccessToolBarVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsQuickAccessToolBarVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsQuickAccessToolBarVisibleProperty =
            DependencyProperty.Register(nameof(IsQuickAccessToolBarVisible), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

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
            DependencyProperty.Register(nameof(CanQuickAccessLocationChanging), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="AreTabHeadersVisible"/>.
        /// </summary>
        public static readonly DependencyProperty AreTabHeadersVisibleProperty = DependencyProperty.Register(nameof(AreTabHeadersVisible), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines whether tab headers are visible or not.
        /// </summary>
        public bool AreTabHeadersVisible
        {
            get { return (bool)this.GetValue(AreTabHeadersVisibleProperty); }
            set { this.SetValue(AreTabHeadersVisibleProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsToolBarVisible"/>.
        /// </summary>
        public static readonly DependencyProperty IsToolBarVisibleProperty = DependencyProperty.Register(nameof(IsToolBarVisible), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines whether tab headers are visible or not.
        /// </summary>
        public bool IsToolBarVisible
        {
            get { return (bool)this.GetValue(IsToolBarVisibleProperty); }
            set { this.SetValue(IsToolBarVisibleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="IsMouseWheelScrollingEnabled"/>
        /// </summary>
        public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty = DependencyProperty.Register(nameof(IsMouseWheelScrollingEnabled), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines whether scrolling by mouse wheel is enabled or not.
        /// </summary>
        public bool IsMouseWheelScrollingEnabled
        {
            get { return (bool)this.GetValue(IsMouseWheelScrollingEnabledProperty); }
            set { this.SetValue(IsMouseWheelScrollingEnabledProperty, value); }
        }

        /// <summary>
        /// Checks if any keytips are visible.
        /// </summary>
        public bool AreAnyKeyTipsVisible => this.keyTipService?.AreAnyKeyTipsVisible == true;

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsKeyTipHandlingEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsKeyTipHandlingEnabledProperty = DependencyProperty.Register(nameof(IsKeyTipHandlingEnabled), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox, OnIsKeyTipHandlingEnabledChanged));

        /// <summary>
        /// Defines whether handling of key tips is enabled or not.
        /// </summary>
        public bool IsKeyTipHandlingEnabled
        {
            get { return (bool)this.GetValue(IsKeyTipHandlingEnabledProperty); }
            set { this.SetValue(IsKeyTipHandlingEnabledProperty, value); }
        }

        private static void OnIsKeyTipHandlingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;

            if ((bool)e.NewValue)
            {
                ribbon.keyTipService?.Attach();
            }
            else
            {
                ribbon.keyTipService?.Detach();
            }
        }

        /// <summary>
        /// Defines the keys that are used to activate the key tips.
        /// </summary>
        public ObservableCollection<Key> KeyTipKeys
        {
            get
            {
                if (this.keyTipKeys == null)
                {
                    this.keyTipKeys = new ObservableCollection<Key>();
                    this.keyTipKeys.CollectionChanged += this.HandleKeyTipKeys_CollectionChanged;
                }

                return this.keyTipKeys;
            }
        }

        private void HandleKeyTipKeys_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.keyTipService.KeyTipKeys.Clear();

            foreach (var keyTipKey in this.KeyTipKeys)
            {
                this.keyTipService.KeyTipKeys.Add(keyTipKey);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets add to quick access toolbar command
        /// </summary>
        public static readonly RoutedCommand AddToQuickAccessCommand = new RoutedCommand("AddToQuickAccessCommand", typeof(Ribbon));

        /// <summary>
        /// Gets remove from quick access command
        /// </summary>
        public static readonly RoutedCommand RemoveFromQuickAccessCommand = new RoutedCommand("RemoveFromQuickAccessCommand", typeof(Ribbon));

        /// <summary>
        /// Gets show quick access above command
        /// </summary>
        public static readonly RoutedCommand ShowQuickAccessAboveCommand = new RoutedCommand("ShowQuickAccessAboveCommand", typeof(Ribbon));

        /// <summary>
        /// Gets show quick access below command
        /// </summary>
        public static readonly RoutedCommand ShowQuickAccessBelowCommand = new RoutedCommand("ShowQuickAccessBelowCommand", typeof(Ribbon));

        /// <summary>
        /// Gets toggle ribbon minimize command
        /// </summary>
        public static readonly RoutedCommand ToggleMinimizeTheRibbonCommand = new RoutedCommand("ToggleMinimizeTheRibbonCommand", typeof(Ribbon));

        /// <summary>
        /// Gets customize quick access toolbar command
        /// </summary>
        public static readonly RoutedCommand CustomizeQuickAccessToolbarCommand = new RoutedCommand("CustomizeQuickAccessToolbarCommand", typeof(Ribbon));

        /// <summary>
        /// Gets customize the ribbon command
        /// </summary>
        public static readonly RoutedCommand CustomizeTheRibbonCommand = new RoutedCommand("CustomizeTheRibbonCommand", typeof(Ribbon));

        // Occurs when customize toggle minimize command can execute handles
        private static void OnToggleMinimizeTheRibbonCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is Ribbon ribbon)
            {
                e.CanExecute = ribbon.CanMinimize;
            }
        }

        // Occurs when toggle minimize command executed
        private static void OnToggleMinimizeTheRibbonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is Ribbon ribbon)
            {
                ribbon.IsMinimized = !ribbon.IsMinimized;
            }
        }

        // Occurs when show quick access below command executed
        private static void OnShowQuickAccessBelowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon == null)
            {
                return;
            }

            ribbon.ShowQuickAccessToolBarAboveRibbon = false;
        }

        // Occurs when show quick access above command executed
        private static void OnShowQuickAccessAboveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon == null)
            {
                return;
            }

            ribbon.ShowQuickAccessToolBarAboveRibbon = true;
        }

        // Occurs when remove from quick access command executed
        private static void OnRemoveFromQuickAccessCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon?.QuickAccessToolBar != null)
            {
                var element = ribbon.QuickAccessElements.First(x => ReferenceEquals(x.Value, e.Parameter)).Key;
                ribbon.RemoveFromQuickAccessToolBar(element);
            }
        }

        // Occurs when add to quick access command executed
        private static void OnAddToQuickAccessCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon?.QuickAccessToolBar != null)
            {
                ribbon.AddToQuickAccessToolBar(e.Parameter as UIElement);
            }
        }

        // Occurs when customize quick access command executed
        private static void OnCustomizeQuickAccessToolbarCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            ribbon?.CustomizeQuickAccessToolbar?.Invoke(sender, EventArgs.Empty);
        }

        // Occurs when customize the ribbon command executed
        private static void OnCustomizeTheRibbonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            ribbon?.CustomizeTheRibbon?.Invoke(sender, EventArgs.Empty);
        }

        // Occurs when customize quick access command can execute handles
        private static void OnCustomizeQuickAccessToolbarCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon == null)
            {
                return;
            }

            e.CanExecute = ribbon.CanCustomizeQuickAccessToolBar;
        }

        // Occurs when customize the ribbon command can execute handles
        private static void OnCustomizeTheRibbonCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon == null)
            {
                return;
            }

            e.CanExecute = ribbon.CanCustomizeRibbon;
        }

        // Occurs when remove from quick access command can execute handles
        private static void OnRemoveFromQuickAccessCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is Ribbon ribbon
                && ribbon.IsQuickAccessToolBarVisible)
            {
                e.CanExecute = ribbon.QuickAccessElements.ContainsValue(e.Parameter as UIElement);
            }
            else
            {
                e.CanExecute = false;
            }
        }

        // Occurs when add to quick access command can execute handles
        private static void OnAddToQuickAccessCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is Ribbon ribbon
                && ribbon.IsQuickAccessToolBarVisible
                && QuickAccessItemsProvider.IsSupported(e.Parameter as UIElement)
                && ribbon.IsInQuickAccessToolBar(e.Parameter as UIElement) == false)
            {
                if (e.Parameter is Gallery gallery)
                {
                    e.CanExecute = ribbon.IsInQuickAccessToolBar(FindParentRibbonControl(gallery) as UIElement) == false;
                }
                else
                {
                    e.CanExecute = ribbon.IsInQuickAccessToolBar(e.Parameter as UIElement) == false;
                }
            }
            else
            {
                e.CanExecute = false;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="Ribbon"/> class.
        /// </summary>
        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));

            // Subscribe to menu commands
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(AddToQuickAccessCommand, OnAddToQuickAccessCommandExecuted, OnAddToQuickAccessCommandCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(RemoveFromQuickAccessCommand, OnRemoveFromQuickAccessCommandExecuted, OnRemoveFromQuickAccessCommandCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(ShowQuickAccessAboveCommand, OnShowQuickAccessAboveCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(ShowQuickAccessBelowCommand, OnShowQuickAccessBelowCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(ToggleMinimizeTheRibbonCommand, OnToggleMinimizeTheRibbonCommandExecuted, OnToggleMinimizeTheRibbonCommandCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(CustomizeTheRibbonCommand, OnCustomizeTheRibbonCommandExecuted, OnCustomizeTheRibbonCommandCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(CustomizeQuickAccessToolbarCommand, OnCustomizeQuickAccessToolbarCommandExecuted, OnCustomizeQuickAccessToolbarCommandCanExecute));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ribbon()
        {
            this.VerticalAlignment = VerticalAlignment.Top;
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);

            WindowChrome.SetIsHitTestVisibleInChrome(this, true);

            this.keyTipService = new KeyTipService(this);

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        #endregion

        #region Overrides

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.MaintainIsCollapsed();
        }

        private void MaintainIsCollapsed()
        {
            if (this.IsAutomaticCollapseEnabled == false
                || this.ownerWindow == null)
            {
                return;
            }

            if (this.ownerWindow.ActualWidth < MinimalVisibleWidth
                || this.ownerWindow.ActualHeight < MinimalVisibleHeight)
            {
                this.IsCollapsed = true;
            }
            else
            {
                this.IsCollapsed = false;
            }
        }

        /// <inheritdoc />
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            var ribbonTabItem = (RibbonTabItem)this.TabControl?.SelectedItem;
            ribbonTabItem?.Focus();
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as Panel;

            var selectedTab = this.SelectedTabItem;
            if (this.TabControl != null)
            {
                this.TabControl.SelectionChanged -= this.OnTabControlSelectionChanged;
                selectedTab = this.TabControl.SelectedItem as RibbonTabItem;

                foreach (var ribbonTabItem in this.Tabs)
                {
                    this.TabControl.Items.Remove(ribbonTabItem);
                }

                // Make sure everything is cleared
                this.TabControl.Items.Clear();

                foreach (var toolBarItem in this.ToolBarItems)
                {
                    this.TabControl.ToolBarItems.Remove(toolBarItem);
                }

                // Make sure everything is cleared
                this.TabControl.ToolBarItems.Clear();
            }

            this.TabControl = this.GetTemplateChild("PART_RibbonTabControl") as RibbonTabControl;

            if (this.TabControl != null)
            {
                this.TabControl.SelectionChanged += this.OnTabControlSelectionChanged;

                foreach (var ribbonTabItem in this.Tabs)
                {
                    this.TabControl.Items.Add(ribbonTabItem);
                }

                this.TabControl.SelectedItem = selectedTab;

                foreach (var toolBarItem in this.ToolBarItems)
                {
                    this.TabControl.ToolBarItems.Add(toolBarItem);
                }
            }

            if (this.QuickAccessToolBar != null)
            {
                if (this.AutomaticStateManagement == false
                    || this.RibbonStateStorage.IsLoaded)
                {
                    this.RibbonStateStorage.SaveTemporary();
                }

                this.ClearQuickAccessToolBar();

                this.QuickAccessToolBar.ItemsChanged -= this.OnQuickAccessItemsChanged;

                foreach (var quickAccessMenuItem in this.QuickAccessItems)
                {
                    this.QuickAccessToolBar.QuickAccessItems.Remove(quickAccessMenuItem);
                }
            }

            this.QuickAccessToolBar = this.GetTemplateChild("PART_QuickAccessToolBar") as QuickAccessToolBar;

            if (this.QuickAccessToolBar != null)
            {
                foreach (var quickAccessMenuItem in this.QuickAccessItems)
                {
                    this.QuickAccessToolBar.QuickAccessItems.Add(quickAccessMenuItem);
                }

                this.QuickAccessToolBar.ItemsChanged += this.OnQuickAccessItemsChanged;

                {
                    var binding = new Binding(nameof(this.CanQuickAccessLocationChanging))
                    {
                        Source = this,
                        Mode = BindingMode.OneWay
                    };
                    this.QuickAccessToolBar.SetBinding(QuickAccessToolBar.CanQuickAccessLocationChangingProperty, binding);
                }

                this.QuickAccessToolBar.Loaded += this.OnFirstToolbarLoaded;
            }

            if (this.ShowQuickAccessToolBarAboveRibbon)
            {
                this.MoveQuickAccessToolBarToTitleBar(this.TitleBar);
            }
        }

        private void MoveQuickAccessToolBarToTitleBar(RibbonTitleBar titleBar)
        {
            if (titleBar != null)
            {
                titleBar.QuickAccessToolBar = this.QuickAccessToolBar;
            }

            if (this.QuickAccessToolBar != null)
            {
                // Prevent double add for handler if this method is called multiple times
                this.QuickAccessToolBar.ContextMenuOpening -= this.OnQuickAccessContextMenuOpening;
                this.QuickAccessToolBar.ContextMenuClosing -= this.OnQuickAccessContextMenuClosing;

                this.QuickAccessToolBar.ContextMenuOpening += this.OnQuickAccessContextMenuOpening;
                this.QuickAccessToolBar.ContextMenuClosing += this.OnQuickAccessContextMenuClosing;
            }
        }

        private void RemoveQuickAccessToolBarFromTitleBar(RibbonTitleBar titleBar)
        {
            if (titleBar != null)
            {
                titleBar.QuickAccessToolBar = null;
            }

            if (this.QuickAccessToolBar != null)
            {
                this.QuickAccessToolBar.ContextMenuOpening -= this.OnQuickAccessContextMenuOpening;
                this.QuickAccessToolBar.ContextMenuClosing -= this.OnQuickAccessContextMenuClosing;
            }
        }

        /// <summary>
        /// Called when the <see cref="ownerWindow"/> is closed, so that we set it to null.
        /// </summary>
        private void OnOwnerWindowClosed(object sender, EventArgs e)
        {
            this.DetachFromWindow();
        }

        private void AttachToWindow()
        {
            this.DetachFromWindow();

            this.ownerWindow = Window.GetWindow(this);

            if (this.ownerWindow != null)
            {
                this.ownerWindow.Closed += this.OnOwnerWindowClosed;
                this.ownerWindow.SizeChanged += this.OnSizeChanged;
                this.ownerWindow.KeyDown += this.OnKeyDown;
            }
        }

        private void DetachFromWindow()
        {
            if (this.ownerWindow != null)
            {
                this.RibbonStateStorage.Save();
                this.RibbonStateStorage.Dispose();
                this.ribbonStateStorage = null;

                this.ownerWindow.Closed -= this.OnOwnerWindowClosed;
                this.ownerWindow.SizeChanged -= this.OnSizeChanged;
                this.ownerWindow.KeyDown -= this.OnKeyDown;
            }

            this.ownerWindow = null;
        }

        private void OnFirstToolbarLoaded(object sender, RoutedEventArgs e)
        {
            this.QuickAccessToolBar.Loaded -= this.OnFirstToolbarLoaded;

            this.RibbonStateStorage.LoadTemporary();
        }

        #endregion

        #region Quick Access Items Managment

        /// <summary>
        /// Determines whether the given element is in quick access toolbar
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>True if element in quick access toolbar</returns>
        public bool IsInQuickAccessToolBar(UIElement element)
        {
            if (element == null)
            {
                return false;
            }

            return this.QuickAccessElements.ContainsKey(element);
        }

        /// <summary>
        /// Adds the given element to quick access toolbar
        /// </summary>
        /// <param name="element">Element</param>
        public void AddToQuickAccessToolBar(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            if (element is Gallery)
            {
                element = FindParentRibbonControl(element) as UIElement;
            }

            // Do not add menu items without icon.
            if (element is System.Windows.Controls.MenuItem menuItem && menuItem.Icon == null)
            {
                element = FindParentRibbonControl(element) as UIElement;
            }

            if (element == null)
            {
                return;
            }

            if (QuickAccessItemsProvider.IsSupported(element) == false)
            {
                return;
            }

            if (this.IsInQuickAccessToolBar(element) == false)
            {
                Debug.WriteLine($"Adding \"{element}\" to QuickAccessToolBar.");

                var control = QuickAccessItemsProvider.GetQuickAccessItem(element);

                this.QuickAccessElements.Add(element, control);
                this.QuickAccessToolBar.Items.Add(control);
            }
        }

        private static IRibbonControl FindParentRibbonControl(DependencyObject element)
        {
            var parent = LogicalTreeHelper.GetParent(element);

            while (parent != null)
            {
                if (parent is IRibbonControl control)
                {
                    return control;
                }

                parent = LogicalTreeHelper.GetParent(parent);
            }

            return null;
        }

        /// <summary>
        /// Removes the given elements from quick access toolbar
        /// </summary>
        /// <param name="element">Element</param>
        public void RemoveFromQuickAccessToolBar(UIElement element)
        {
            Debug.WriteLine("Removing \"{0}\" from QuickAccessToolBar.", element);

            if (this.IsInQuickAccessToolBar(element))
            {
                var quickAccessItem = this.QuickAccessElements[element];
                this.QuickAccessElements.Remove(element);
                this.QuickAccessToolBar.Items.Remove(quickAccessItem);
            }
        }

        /// <summary>
        /// Clears quick access toolbar
        /// </summary>
        public void ClearQuickAccessToolBar()
        {
            this.QuickAccessElements.Clear();
            this.QuickAccessToolBar?.Items.Clear();
        }

        #endregion

        #region Event Handling

        // Handles tab control selection changed
        private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ReferenceEquals(e.OriginalSource, this.TabControl) == false)
            {
                return;
            }

            this.SelectedTabItem = this.TabControl.SelectedItem as RibbonTabItem;
            this.SelectedTabIndex = this.TabControl.SelectedIndex;

            this.SelectedTabChanged?.Invoke(this, e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.keyTipService.Attach();

            this.AttachToWindow();

            this.LoadInitialState();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1
                && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                if (this.TabControl.HasItems)
                {
                    if (this.CanMinimize)
                    {
                        this.IsMinimized = !this.IsMinimized;
                    }
                }
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.RibbonStateStorage.Save();

            this.keyTipService.Detach();

            if (this.ownerWindow != null)
            {
                this.ownerWindow.SizeChanged -= this.OnSizeChanged;
                this.ownerWindow.KeyDown -= this.OnKeyDown;
            }
        }

        #endregion

        #region Private methods

        private RibbonTabItem GetFirstVisibleItem()
        {
            return this.Tabs.FirstOrDefault(item => item.Visibility == Visibility.Visible);
        }

        private RibbonTabItem GetLastVisibleItem()
        {
            return this.Tabs.LastOrDefault(item => item.Visibility == Visibility.Visible);
        }

        #endregion

        #region State Management

        private void LoadInitialState()
        {
            if (this.RibbonStateStorage.IsLoaded)
            {
                return;
            }

            this.RibbonStateStorage.Load();

            this.TabControl?.SelectFirstTab();
        }

        // Handles items changing in QAT
        private void OnQuickAccessItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RibbonStateStorage.SaveTemporary();
        }

        /// <summary>
        /// Traverse logical tree and find QAT items, remember paths
        /// </summary>
        public void TraverseLogicalTree(DependencyObject item, string path, IDictionary<FrameworkElement, string> paths)
        {
            // Is this item in QAT
            if (item is FrameworkElement frameworkElement
                && this.QuickAccessElements.ContainsKey(frameworkElement))
            {
                if (paths.ContainsKey(frameworkElement) == false)
                {
                    paths.Add(frameworkElement, path);
                }
            }

            var children = LogicalTreeHelper.GetChildren(item).Cast<object>().ToList();
            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i] as DependencyObject;
                if (child == null)
                {
                    continue;
                }

                this.TraverseLogicalTree(child, path + i + ",", paths);
            }
        }

        #endregion

        #region AutomaticStateManagement Property

        /// <summary>
        /// Gets or sets whether Quick Access ToolBar can
        /// save and load its state automatically
        /// </summary>
        public bool AutomaticStateManagement
        {
            get { return (bool)this.GetValue(AutomaticStateManagementProperty); }
            set { this.SetValue(AutomaticStateManagementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutomaticStateManagement.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutomaticStateManagementProperty =
            DependencyProperty.Register(nameof(AutomaticStateManagement), typeof(bool), typeof(Ribbon), new PropertyMetadata(BooleanBoxes.TrueBox, OnAutomaticStateManagementChanged, CoerceAutomaticStateManagement));

        private static object CoerceAutomaticStateManagement(DependencyObject d, object basevalue)
        {
            var ribbon = (Ribbon)d;
            if (ribbon.RibbonStateStorage.IsLoading)
            {
                return false;
            }

            return basevalue;
        }

        private static void OnAutomaticStateManagementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            if ((bool)e.NewValue)
            {
                ribbon.LoadInitialState();
            }
        }

        #endregion
    }
}