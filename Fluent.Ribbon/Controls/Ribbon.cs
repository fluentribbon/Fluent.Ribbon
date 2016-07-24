using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    using ControlzEx.Microsoft.Windows.Shell;

    // TODO: improve style parts naming & using

    /// <summary>
    /// Represents the main Ribbon control which consists of multiple tabs, each of which
    /// containing groups of controls.  The Ribbon also provides improved context
    /// menus, enhanced screen tips, and keyboard shortcuts.
    /// </summary>
    [ContentProperty("Tabs")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506")]
    [SuppressMessage("Microsoft.Design", "CA1001")]
    public class Ribbon : Control
    {
        private IRibbonStateStorage ribbonStateStorage;

        /// <summary>
        /// Gets the current instance for storing the state of this control.
        /// </summary>
        protected IRibbonStateStorage RibbonStateStorage => this.ribbonStateStorage ?? (this.ribbonStateStorage = this.CreateRibbonStateStorage());

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

        private static readonly Dictionary<int, System.Windows.Controls.ContextMenu> contextMenus = new Dictionary<int, System.Windows.Controls.ContextMenu>();

        /// <summary>
        /// Context menu for ribbon in current thread
        /// </summary>
        public static System.Windows.Controls.ContextMenu RibbonContextMenu
        {
            get
            {
                if (!contextMenus.ContainsKey(Thread.CurrentThread.ManagedThreadId)) InitRibbonContextMenu();
                return contextMenus[Thread.CurrentThread.ManagedThreadId];
            }
        }

        // Context menu owner ribbon
        private static Ribbon contextMenuOwner;

        // Context menu items
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem addToQuickAccessMenuItem
        {
            get { return addToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addGroupToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem addGroupToQuickAccessMenuItem
        {
            get { return addGroupToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addMenuToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem addMenuToQuickAccessMenuItem
        {
            get { return addMenuToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> addGalleryToQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem addGalleryToQuickAccessMenuItem
        {
            get { return addGalleryToQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> removeFromQuickAccessMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem removeFromQuickAccessMenuItem
        {
            get { return removeFromQuickAccessMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> showQuickAccessToolbarBelowTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem showQuickAccessToolbarBelowTheRibbonMenuItem
        {
            get { return showQuickAccessToolbarBelowTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> showQuickAccessToolbarAboveTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem showQuickAccessToolbarAboveTheRibbonMenuItem
        {
            get { return showQuickAccessToolbarAboveTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> minimizeTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem minimizeTheRibbonMenuItem
        {
            get { return minimizeTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> customizeQuickAccessToolbarMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem customizeQuickAccessToolbarMenuItem
        {
            get { return customizeQuickAccessToolbarMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, System.Windows.Controls.MenuItem> customizeTheRibbonMenuItemDictionary = new Dictionary<int, System.Windows.Controls.MenuItem>();
        private static System.Windows.Controls.MenuItem customizeTheRibbonMenuItem
        {
            get { return customizeTheRibbonMenuItemDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, Separator> firstSeparatorDictionary = new Dictionary<int, Separator>();
        private static Separator firstSeparator
        {
            get { return firstSeparatorDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }
        private static readonly Dictionary<int, Separator> secondSeparatorDictionary = new Dictionary<int, Separator>();
        private static Separator secondSeparator
        {
            get { return secondSeparatorDictionary[Thread.CurrentThread.ManagedThreadId]; }
        }

        // Initialize ribbon context menu
        private static void InitRibbonContextMenu()
        {
            contextMenus.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.ContextMenu());
            RibbonContextMenu.Opened += OnContextMenuOpened;

            // Add to quick access toolbar
            addToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = AddToQuickAccessCommand });
            RibbonContextMenu.Items.Add(addToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, addToQuickAccessMenuItem, "RibbonContextMenuAddItem", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addToQuickAccessMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Add group to quick access toolbar
            addGroupToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = AddToQuickAccessCommand });
            RibbonContextMenu.Items.Add(addGroupToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, addGroupToQuickAccessMenuItem, "RibbonContextMenuAddGroup", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addGroupToQuickAccessMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Add menu item to quick access toolbar
            addMenuToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = AddToQuickAccessCommand });
            RibbonContextMenu.Items.Add(addMenuToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, addMenuToQuickAccessMenuItem, "RibbonContextMenuAddMenu", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addMenuToQuickAccessMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Add gallery to quick access toolbar
            addGalleryToQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = AddToQuickAccessCommand });
            RibbonContextMenu.Items.Add(addGalleryToQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, addGalleryToQuickAccessMenuItem, "RibbonContextMenuAddGallery", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addGalleryToQuickAccessMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Remove from quick access toolbar
            removeFromQuickAccessMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = RemoveFromQuickAccessCommand });
            RibbonContextMenu.Items.Add(removeFromQuickAccessMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, removeFromQuickAccessMenuItem, "RibbonContextMenuRemoveItem", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, removeFromQuickAccessMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Separator
            firstSeparatorDictionary.Add(Thread.CurrentThread.ManagedThreadId, new Separator());
            RibbonContextMenu.Items.Add(firstSeparator);

            // Customize quick access toolbar
            customizeQuickAccessToolbarMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = CustomizeQuickAccessToolbarCommand });
            RibbonContextMenu.Items.Add(customizeQuickAccessToolbarMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, customizeQuickAccessToolbarMenuItem, "RibbonContextMenuCustomizeQuickAccessToolBar", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, customizeQuickAccessToolbarMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Show quick access below the ribbon
            showQuickAccessToolbarBelowTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = ShowQuickAccessBelowCommand });
            RibbonContextMenu.Items.Add(showQuickAccessToolbarBelowTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, showQuickAccessToolbarBelowTheRibbonMenuItem, "RibbonContextMenuShowBelow", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, showQuickAccessToolbarBelowTheRibbonMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Show quick access above the ribbon
            showQuickAccessToolbarAboveTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = ShowQuickAccessAboveCommand });
            RibbonContextMenu.Items.Add(showQuickAccessToolbarAboveTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, showQuickAccessToolbarAboveTheRibbonMenuItem, "RibbonContextMenuShowAbove", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, showQuickAccessToolbarAboveTheRibbonMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Separator
            secondSeparatorDictionary.Add(Thread.CurrentThread.ManagedThreadId, new Separator());
            RibbonContextMenu.Items.Add(secondSeparator);

            // Customize the ribbon
            customizeTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = CustomizeTheRibbonCommand });
            RibbonContextMenu.Items.Add(customizeTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, customizeTheRibbonMenuItem, "RibbonContextMenuCustomizeRibbon", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, customizeTheRibbonMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);

            // Minimize the ribbon
            minimizeTheRibbonMenuItemDictionary.Add(Thread.CurrentThread.ManagedThreadId, new System.Windows.Controls.MenuItem { Command = ToggleMinimizeTheRibbonCommand });
            RibbonContextMenu.Items.Add(minimizeTheRibbonMenuItem);
            RibbonControl.Bind(RibbonLocalization.Current.Localization, minimizeTheRibbonMenuItem, "RibbonContextMenuMinimizeRibbon", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, minimizeTheRibbonMenuItem, "PlacementTarget", System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuOpening"/> routed event reaches this class in its route. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            contextMenuOwner = this;
            base.OnContextMenuOpening(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuClosing"/> routed event reaches this class in its route. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            contextMenuOwner = null;
            base.OnContextMenuClosing(e);
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

            addToQuickAccessMenuItem.CommandTarget = ribbon;
            addGroupToQuickAccessMenuItem.CommandTarget = ribbon;
            addMenuToQuickAccessMenuItem.CommandTarget = ribbon;
            addGalleryToQuickAccessMenuItem.CommandTarget = ribbon;
            removeFromQuickAccessMenuItem.CommandTarget = ribbon;
            customizeQuickAccessToolbarMenuItem.CommandTarget = ribbon;
            customizeTheRibbonMenuItem.CommandTarget = ribbon;
            minimizeTheRibbonMenuItem.CommandTarget = ribbon;
            showQuickAccessToolbarBelowTheRibbonMenuItem.CommandTarget = ribbon;
            showQuickAccessToolbarAboveTheRibbonMenuItem.CommandTarget = ribbon;

            // Hide items for ribbon controls
            addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
            firstSeparator.Visibility = Visibility.Collapsed;

            // Hide customize quick access menu item
            customizeQuickAccessToolbarMenuItem.Visibility = Visibility.Collapsed;
            secondSeparator.Visibility = Visibility.Collapsed;

            // Set minimize the ribbon menu item state
            minimizeTheRibbonMenuItem.IsChecked = ribbon.IsMinimized;

            // Set minimize the ribbon menu item visibility
            if (ribbon.CanMinimize)
            {
                minimizeTheRibbonMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                minimizeTheRibbonMenuItem.Visibility = Visibility.Collapsed;
            }

            // Set customize the ribbon menu item visibility
            if (ribbon.CanCustomizeRibbon)
            {
                customizeTheRibbonMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                customizeTheRibbonMenuItem.Visibility = Visibility.Collapsed;
            }

            // Hide quick access position menu items
            showQuickAccessToolbarBelowTheRibbonMenuItem.Visibility = Visibility.Collapsed;
            showQuickAccessToolbarAboveTheRibbonMenuItem.Visibility = Visibility.Collapsed;

            // If quick access toolbar is visible show 
            if (ribbon.IsQuickAccessToolBarVisible)
            {
                // Set quick access position menu items visibility
                if (ribbon.CanQuickAccessLocationChanging)
                {
                    if (ribbon.ShowQuickAccessToolBarAboveRibbon)
                    {
                        showQuickAccessToolbarBelowTheRibbonMenuItem.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        showQuickAccessToolbarAboveTheRibbonMenuItem.Visibility = Visibility.Visible;
                    }
                }

                if (ribbon.CanCustomizeQuickAccessToolBar)
                {
                    customizeQuickAccessToolbarMenuItem.Visibility = Visibility.Visible;
                }

                if (ribbon.CanQuickAccessLocationChanging
                    || ribbon.CanCustomizeQuickAccessToolBar)
                {
                    secondSeparator.Visibility = Visibility.Visible;
                }
                else
                {
                    secondSeparator.Visibility = Visibility.Collapsed;
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
                        firstSeparator.Visibility = Visibility.Visible;

                        // Check for value because remove is only possible in the context menu of items in QA which represent the value for QA-items
                        if (ribbon.QuickAccessElements.ContainsValue(control))
                        {
                            // Control is on quick access
                            removeFromQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is System.Windows.Controls.MenuItem)
                        {
                            // Control is menu item
                            addMenuToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is Gallery ||
                                 control is InRibbonGallery)
                        {
                            // Control is gallery
                            addGalleryToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is RibbonGroupBox)
                        {
                            // Control is group box
                            addGroupToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is IQuickAccessItemProvider)
                        {
                            // Its other control
                            addToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            firstSeparator.Visibility = Visibility.Collapsed;
                        }
                    }
                }
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
        public UIElement Menu
        {
            get { return (UIElement)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Button. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(UIElement), typeof(Ribbon), new UIPropertyMetadata(null, AddOrRemoveLogicalChildOnPropertyChanged));

        #endregion

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
            DependencyProperty.Register(nameof(StartScreen), typeof(StartScreen), typeof(Ribbon), new UIPropertyMetadata(null, AddOrRemoveLogicalChildOnPropertyChanged));

        /// <summary>
        /// Window title
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(Ribbon), new UIPropertyMetadata(string.Empty, OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;

            ribbon?.TitleBar?.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets selected tab item
        /// </summary>
        public RibbonTabItem SelectedTabItem
        {
            get { return (RibbonTabItem)this.GetValue(SelectedTabItemProperty); }
            set { this.SetValue(SelectedTabItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedTabItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedTabItemProperty =
            DependencyProperty.Register(nameof(SelectedTabItem), typeof(RibbonTabItem), typeof(Ribbon), new UIPropertyMetadata(null, OnSelectedTabItemChanged));

        private static void OnSelectedTabItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            if (ribbon.TabControl != null)
            {
                ribbon.TabControl.SelectedItem = e.NewValue;
            }

            var selectedItem = e.NewValue as RibbonTabItem;

            if (selectedItem != null
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
        /// Using a DependencyProperty as the backing store for SelectedTabindex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedTabIndexProperty =
            DependencyProperty.Register("SelectedTabIndex", typeof(int), typeof(Ribbon), new UIPropertyMetadata(-1, OnSelectedTabIndexChanged));

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

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        internal RibbonTitleBar TitleBar { get; private set; }

        /// <summary>
        /// Gets the Ribbon tab control
        /// </summary>
        internal RibbonTabControl TabControl { get; private set; }

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
            DependencyProperty.Register("ShowQuickAccessToolBarAboveRibbon", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true, OnShowQuickAccesToolBarAboveRibbonChanged));

        /// <summary>
        /// Handles ShowQuickAccessToolBarAboveRibbon property changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnShowQuickAccesToolBarAboveRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            ribbon.TitleBar?.InvalidateMeasure();

            ribbon.RibbonStateStorage.SaveTemporary();
        }

        /// <summary>
        /// Gets collection of contextual tab groups
        /// </summary>
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

        /// <summary>
        /// Gets quick access toolbar associated with the ribbon
        /// </summary>
        internal QuickAccessToolBar QuickAccessToolBar { get; private set; }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (this.layoutRoot != null)
                {
                    yield return this.layoutRoot;
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
            }
        }

        /// <summary>
        /// Gets collection of quick access menu items
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
            DependencyProperty.Register("CanCustomizeQuickAccessToolBar", typeof(bool),
            typeof(Ribbon), new UIPropertyMetadata(false));

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
            DependencyProperty.Register("CanCustomizeQuickAccessToolBarItems", typeof(bool), typeof(Ribbon), new PropertyMetadata(true));

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
            DependencyProperty.Register("CanCustomizeRibbon", typeof(bool),
            typeof(Ribbon), new UIPropertyMetadata(false));

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
            DependencyProperty.Register("IsMinimized", typeof(bool),
            typeof(Ribbon), new UIPropertyMetadata(false, OnIsMinimizedChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanMinimize.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register("CanMinimize", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true, OnCanMinimizeChanged));


        private static void OnIsMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;

            ribbon.IsMinimizedChanged?.Invoke(ribbon, e);
        }

        private static void OnCanMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (Ribbon)d;
            if (ribbon.TabControl != null)
            {
                ribbon.TabControl.CanMinimize = ribbon.CanMinimize;
            }
        }

        /// <summary>
        /// Gets or sets the height of the gap between the ribbon and the content
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
            DependencyProperty.Register("ContentGapHeight", typeof(double), typeof(Ribbon), new UIPropertyMetadata(5D));

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
            DependencyProperty.Register("IsCollapsed", typeof(bool),
            typeof(Ribbon), new FrameworkPropertyMetadata(false, OnIsCollapsedChanged));

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
            DependencyProperty.Register("IsAutomaticCollapseEnabled", typeof(bool), typeof(Ribbon), new PropertyMetadata(true));

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
            DependencyProperty.Register("IsQuickAccessToolBarVisible", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true));


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
            DependencyProperty.Register("CanQuickAccessLocationChanging", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true));


        /// <summary>
        /// Checks if any keytips are visible.
        /// </summary>
        public bool AreAnyKeyTipsVisible
        {
            get
            {
                if (this.keyTipService != null)
                {
                    return this.keyTipService.AreAnyKeyTipsVisible;
                }

                return false;
            }
        }
        #endregion

        #region Commands

        /// <summary>
        /// Gets add to quick access toolbar command
        /// </summary>
        public static RoutedCommand AddToQuickAccessCommand = new RoutedCommand("AddToQuickAccessCommand", typeof(Ribbon));

        /// <summary>
        /// Gets remove from quick access command
        /// </summary>
        public static RoutedCommand RemoveFromQuickAccessCommand = new RoutedCommand("RemoveFromQuickAccessCommand", typeof(Ribbon));

        /// <summary>
        /// Gets show quick access above command
        /// </summary>
        public static RoutedCommand ShowQuickAccessAboveCommand = new RoutedCommand("ShowQuickAccessAboveCommand", typeof(Ribbon));

        /// <summary>
        /// Gets show quick access below command
        /// </summary>
        public static RoutedCommand ShowQuickAccessBelowCommand = new RoutedCommand("ShowQuickAccessBelowCommand", typeof(Ribbon));

        /// <summary>
        /// Gets toggle ribbon minimize command
        /// </summary>
        public static RoutedCommand ToggleMinimizeTheRibbonCommand = new RoutedCommand("ToggleMinimizeTheRibbonCommand", typeof(Ribbon));

        /// <summary>
        /// Gets customize quick access toolbar command
        /// </summary>
        public static RoutedCommand CustomizeQuickAccessToolbarCommand = new RoutedCommand("CustomizeQuickAccessToolbarCommand", typeof(Ribbon));

        /// <summary>
        /// Gets customize the ribbon command
        /// </summary>
        public static RoutedCommand CustomizeTheRibbonCommand = new RoutedCommand("CustomizeTheRibbonCommand", typeof(Ribbon));

        // Occurs when customize toggle minimize command can execute handles
        private static void OnToggleMinimizeTheRibbonCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;

            if (ribbon?.TabControl != null)
            {
                e.CanExecute = ribbon.CanMinimize;
            }
        }

        // Occurs when toggle minimize command executed
        private static void OnToggleMinimizeTheRibbonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var ribbon = sender as Ribbon;
            if (ribbon?.TabControl != null)
            {
                ribbon.TabControl.IsMinimized = !ribbon.TabControl.IsMinimized;
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
                var element = ribbon.QuickAccessElements.First(x => x.Value == e.Parameter).Key;
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
            var ribbon = sender as Ribbon;

            if (ribbon == null)
            {
                return;
            }

            if (ribbon.IsQuickAccessToolBarVisible)
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
            var ribbon = sender as Ribbon;

            if (ribbon != null
                && ribbon.IsQuickAccessToolBarVisible
                && QuickAccessItemsProvider.IsSupported(e.Parameter as UIElement)
                && ribbon.IsInQuickAccessToolBar(e.Parameter as UIElement) == false)
            {
                if (e.Parameter is Gallery)
                {
                    e.CanExecute = ribbon.IsInQuickAccessToolBar(FindParentRibbonControl(e.Parameter as DependencyObject) as UIElement) == false;
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
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
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

            InitRibbonContextMenu();
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

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.UIElement.GotFocus 
        /// event reaches this element in its route.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            var ribbonTabItem = (RibbonTabItem)this.TabControl?.SelectedItem;
            ribbonTabItem?.Focus();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        public override void OnApplyTemplate()
        {
            this.layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as Panel;

            if (this.TitleBar != null)
            {
                foreach (var ribbonContextualTabGroup in this.ContextualGroups)
                {
                    this.TitleBar.Items.Remove(ribbonContextualTabGroup);
                }

                // Make sure everything is cleared
                this.TitleBar.Items.Clear();
            }

            this.TitleBar = this.GetTemplateChild("PART_RibbonTitleBar") as RibbonTitleBar;

            if (this.TitleBar != null)
            {
                foreach (var contextualTabGroup in this.ContextualGroups)
                {
                    this.TitleBar.Items.Add(contextualTabGroup);
                }
            }

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

                this.TabControl.CanMinimize = this.CanMinimize;
                this.TabControl.IsMinimized = this.IsMinimized;
                this.TabControl.ContentGapHeight = this.ContentGapHeight;

                this.TabControl.SetBinding(RibbonTabControl.IsMinimizedProperty, new Binding("IsMinimized") { Source = this, Mode = BindingMode.TwoWay });
                this.TabControl.SetBinding(RibbonTabControl.ContentGapHeightProperty, new Binding("ContentGapHeight") { Source = this, Mode = BindingMode.OneWay });

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

                var binding = new Binding("CanQuickAccessLocationChanging")
                {
                    Source = this,
                    Mode = BindingMode.OneWay
                };
                this.QuickAccessToolBar.SetBinding(QuickAccessToolBar.CanQuickAccessLocationChangingProperty, binding);

                this.QuickAccessToolBar.Loaded += this.OnFirstToolbarLoaded;
            }
        }

        /// <summary>
        /// Called when the <see cref="ownerWindow"/> is closed, so that we set it to null and clear the <see cref="TitleProperty"/>
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

                var binding = new Binding("Title")
                {
                    Mode = BindingMode.OneWay,
                    Source = this.ownerWindow
                };
                this.SetBinding(TitleProperty, binding);
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

                BindingOperations.ClearBinding(this, TitleProperty);
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
            var menuItem = element as MenuItem;
            if (menuItem != null && menuItem.Icon == null)
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
                var control = parent as IRibbonControl;
                if (control != null)
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
            if (this.TabControl != null)
            {
                this.SelectedTabItem = this.TabControl.SelectedItem as RibbonTabItem;
                this.SelectedTabIndex = this.TabControl.SelectedIndex;
            }

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

            if (this.TabControl != null
                && this.TabControl.SelectedIndex == -1
                && this.TabControl.IsMinimized == false)
            {
                this.TabControl.SelectedItem = this.TabControl.GetFirstVisibleItem();
            }
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
            var uielement = item as FrameworkElement;
            if (uielement != null
                && this.QuickAccessElements.ContainsKey(uielement))
            {
                if (!paths.ContainsKey(uielement))
                {
                    paths.Add(uielement, path);
                }
            }

            var children = LogicalTreeHelper.GetChildren(item).Cast<object>().ToArray();
            for (var i = 0; i < children.Length; i++)
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
            DependencyProperty.Register("AutomaticStateManagement", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true, OnAutoStateManagement, CoerceAutoStateManagement));

        private static object CoerceAutoStateManagement(DependencyObject d, object basevalue)
        {
            var ribbon = (Ribbon)d;
            if (ribbon.RibbonStateStorage.IsLoading)
            {
                return false;
            }

            return basevalue;
        }

        private static void OnAutoStateManagement(DependencyObject d, DependencyPropertyChangedEventArgs e)
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