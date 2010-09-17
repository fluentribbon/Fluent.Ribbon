#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Threading;

namespace Fluent
{
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
        #region Localization

        // Localizable properties
        static readonly RibbonLocalization localization = new RibbonLocalization();

        /// <summary>
        /// Gets localizable properties
        /// </summary>
        public static RibbonLocalization Localization
        {
            get { return localization; }
        }

        #endregion

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

        // Context menu for ribbon
        public static System.Windows.Controls.ContextMenu RibbonContextMenu;

        // Context menu owner ribbon
        private static Ribbon contextMenuOwner;

        // Context menu items
        private static System.Windows.Controls.MenuItem addToQuickAccessMenuItem;
        private static System.Windows.Controls.MenuItem addGroupToQuickAccessMenuItem;
        private static System.Windows.Controls.MenuItem addMenuToQuickAccessMenuItem;
        private static System.Windows.Controls.MenuItem addGalleryToQuickAccessMenuItem;
        private static System.Windows.Controls.MenuItem removeFromQuickAccessMenuItem;
        private static System.Windows.Controls.MenuItem showQuickAccessToolbarBelowTheRibbonMenuItem;
        private static System.Windows.Controls.MenuItem showQuickAccessToolbarAboveTheRibbonMenuItem;
        private static System.Windows.Controls.MenuItem minimizeTheRibbonMenuItem;
        private static System.Windows.Controls.MenuItem customizeQuickAccessToolbarMenuItem;
        private static System.Windows.Controls.MenuItem customizeTheRibbonMenuItem;
        private static Separator firstSeparator;
        private static Separator secondSeparator;

        // Initialize ribbon context menu
        private static void InitRibbonContextMenu()
        {
            RibbonContextMenu = new System.Windows.Controls.ContextMenu();
            RibbonContextMenu.Opened += OnContextMenuOpened;

            // Add to quick access toolbar
            RibbonContextMenu.Items.Add(addToQuickAccessMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.AddToQuickAccessCommand});
            RibbonControl.Bind(Ribbon.Localization, addToQuickAccessMenuItem, "RibbonContextMenuAddItem", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addToQuickAccessMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Add group to quick access toolbar
            RibbonContextMenu.Items.Add(addGroupToQuickAccessMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.AddToQuickAccessCommand});
            RibbonControl.Bind(Ribbon.Localization, addGroupToQuickAccessMenuItem, "RibbonContextMenuAddGroup", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addGroupToQuickAccessMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Add menu item to quick access toolbar
            RibbonContextMenu.Items.Add(addMenuToQuickAccessMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.AddToQuickAccessCommand});
            RibbonControl.Bind(Ribbon.Localization, addMenuToQuickAccessMenuItem, "RibbonContextMenuAddMenu", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addMenuToQuickAccessMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Add gallery to quick access toolbar
            RibbonContextMenu.Items.Add(addGalleryToQuickAccessMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.AddToQuickAccessCommand});
            RibbonControl.Bind(Ribbon.Localization, addGalleryToQuickAccessMenuItem, "RibbonContextMenuAddGallery", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, addGalleryToQuickAccessMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Remove from quick access toolbar
            RibbonContextMenu.Items.Add(removeFromQuickAccessMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.RemoveFromQuickAccessCommand});
            RibbonControl.Bind(Ribbon.Localization, removeFromQuickAccessMenuItem, "RibbonContextMenuRemoveItem", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, removeFromQuickAccessMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Separator
            RibbonContextMenu.Items.Add(firstSeparator = new Separator());

            // Customize quick access toolbar
            RibbonContextMenu.Items.Add(customizeQuickAccessToolbarMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.CustomizeQuickAccessToolbarCommand});
            RibbonControl.Bind(Ribbon.Localization, customizeQuickAccessToolbarMenuItem, "RibbonContextMenuCustomizeQuickAccessToolBar", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, customizeQuickAccessToolbarMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Show quick access below the ribbon
            RibbonContextMenu.Items.Add(showQuickAccessToolbarBelowTheRibbonMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.ShowQuickAccessBelowCommand});
            RibbonControl.Bind(Ribbon.Localization, showQuickAccessToolbarBelowTheRibbonMenuItem, "RibbonContextMenuShowBelow", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, showQuickAccessToolbarBelowTheRibbonMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Show quick access above the ribbon
            RibbonContextMenu.Items.Add(showQuickAccessToolbarAboveTheRibbonMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.ShowQuickAccessAboveCommand});
            RibbonControl.Bind(Ribbon.Localization, showQuickAccessToolbarAboveTheRibbonMenuItem, "RibbonContextMenuShowAbove", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, showQuickAccessToolbarAboveTheRibbonMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Separator
            RibbonContextMenu.Items.Add(secondSeparator = new Separator());

            // Customize the ribbon
            RibbonContextMenu.Items.Add(customizeTheRibbonMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.CustomizeTheRibbonCommand});
            RibbonControl.Bind(Ribbon.Localization, customizeTheRibbonMenuItem, "RibbonContextMenuCustomizeRibbon", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, customizeTheRibbonMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);

            // Minimize the ribbon
            RibbonContextMenu.Items.Add(minimizeTheRibbonMenuItem = new System.Windows.Controls.MenuItem { Command = Ribbon.ToggleMinimizeTheRibbonCommand});
            RibbonControl.Bind(Ribbon.Localization, minimizeTheRibbonMenuItem, "RibbonContextMenuMinimizeRibbon", MenuItem.HeaderProperty, BindingMode.OneWay);
            RibbonControl.Bind(RibbonContextMenu, minimizeTheRibbonMenuItem, "PlacementTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);
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
            Ribbon ribbon = contextMenuOwner;
            if ((RibbonContextMenu != null) && (ribbon != null))
            {
                // Hide items for ribbon controls
                addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                firstSeparator.Visibility = Visibility.Collapsed;
                // Hide customize quick access menu item
                customizeQuickAccessToolbarMenuItem.Visibility = Visibility.Collapsed;
                secondSeparator.Visibility = Visibility.Visible;

                // Set minimize the ribbon menu item state
                minimizeTheRibbonMenuItem.IsChecked = ribbon.IsMinimized;

                // Set customize the ribbon menu item visibility
                if (ribbon.CanCustomizeRibbon) customizeTheRibbonMenuItem.Visibility = Visibility.Visible;
                else customizeTheRibbonMenuItem.Visibility = Visibility.Collapsed;

                // Hide quick access position menu items
                showQuickAccessToolbarBelowTheRibbonMenuItem.Visibility = Visibility.Collapsed;
                showQuickAccessToolbarAboveTheRibbonMenuItem.Visibility = Visibility.Collapsed;

                // If quick access toolbar is visible show 
                if (ribbon.IsQuickAccessToolBarVisible)
                {
                    // Set quick access position menu items visibility
                    if (ribbon.ShowQuickAccessToolBarAboveRibbon) showQuickAccessToolbarBelowTheRibbonMenuItem.Visibility = Visibility.Visible;
                    else showQuickAccessToolbarAboveTheRibbonMenuItem.Visibility = Visibility.Visible;
                    if (ribbon.CanCustomizeQuickAccessToolBar) customizeQuickAccessToolbarMenuItem.Visibility = Visibility.Visible;
                    secondSeparator.Visibility = Visibility.Visible;

                    // Gets control that raise menu opened
                    UIElement control = RibbonContextMenu.PlacementTarget as UIElement;
                    if (control != null)
                    {
                        if (ribbon.quickAccessElements.ContainsValue(control))
                        {
                            // Control is on quick access
                            removeFromQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is System.Windows.Controls.MenuItem)
                        {
                            // Control is menu item
                            addMenuToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is Gallery)
                        {
                            // Control is gallery
                            addGalleryToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else if (control is RibbonGroupBox)
                        {
                            // Control is group box
                            addGroupToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            // Its other control
                            addToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        }
                        firstSeparator.Visibility = Visibility.Visible;
                    }
                }

            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when selected tab has been changed (be aware that SelectedTab can be null)
        /// </summary>
        public event EventHandler SelectedTabChanged;

        /// <summary>
        /// Occurs when customize the ribbon
        /// </summary>
        public event EventHandler CustomizeTheRibbon;

        /// <summary>
        /// Occurs when customize quick access toolbar
        /// </summary>
        public event EventHandler CustomizeQuickAccessToolbar;

        #endregion

        #region Fields

        // Collection of contextual tab groups
        private ObservableCollection<RibbonContextualTabGroup> groups;
        // Collection of tabs
        private ObservableCollection<RibbonTabItem> tabs;
        // Collection of toolbar items
        private ObservableCollection<UIElement> toolBarItems;
        // Collection of backstagetems
        private ObservableCollection<UIElement> backstageItems;
        // Ribbon title bar
        private RibbonTitleBar titleBar;
        // Ribbon tab control
        private RibbonTabControl tabControl;
        // Ribbon quick access toolbar
        private QuickAccessToolBar quickAccessToolBar;
        // Ribbon layout root
        private Panel layoutRoot;
        // Ribbon backstage button
        private Backstage backstageButton;

        // Handles F10, Alt and so on
        readonly KeyTipService keyTipService;

        // Collection of quickaccess menu items
        private ObservableCollection<QuickAccessMenuItem> quickAccessItems;
        // Adorner for backstage
        private BackstageAdorner adorner;
        // Saved when backstage opened tab item
        private RibbonTabItem savedTabItem;
        // Currently added in QAT items
        readonly Dictionary<UIElement, UIElement> quickAccessElements = new Dictionary<UIElement, UIElement>();

        private double savedMinWidth;
        private double savedMinHeight;
        int savedWidth;
        private int savedHeight;

        // Stream to save quickaccesselements on aplytemplate
        MemoryStream quickAccessStream;

        private Window ownerWindow;

        private int selectedTabIndex = -1;
        private RibbonTabItem selectedTabItem;

        #endregion

        #region Properties

        #region Menu
        
        /// <summary>
        /// Gets or sets file menu control (can be application menu button, backstage button and so on)
        /// </summary>
        public UIElement Menu
        {
            get { return (UIElement)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Button. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(UIElement), typeof(Ribbon), new UIPropertyMetadata(null, OnApplicationMenuChanged));

        static void OnApplicationMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = (Ribbon) d;
            if (e.OldValue != null) ribbon.RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null) ribbon.AddLogicalChild(e.NewValue);
        }

        #endregion

        /// <summary>
        /// Window title
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Ribbon), new UIPropertyMetadata("", OnTitleChanged));

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as Ribbon).titleBar != null) (d as Ribbon).titleBar.InvalidateMeasure();
        }


        /// <summary>
        /// Gets or sets selected tab item
        /// </summary>
        public RibbonTabItem SelectedTabItem
        {
            get { return selectedTabItem; }
            set
            {
                if (tabControl != null) tabControl.SelectedItem = value;
                selectedTabItem = value;
            }
        }

        /// <summary>
        /// Gets or sets selected tab index
        /// </summary>
        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                if (tabControl != null) tabControl.SelectedIndex = value;
                selectedTabIndex = value;
            }
        }

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        internal RibbonTitleBar TitleBar
        {
            get { return titleBar; }
        }

        /// <summary>
        /// Gets or sets whether quick access toolbar showes above ribbon
        /// </summary>
        public bool ShowQuickAccessToolBarAboveRibbon
        {
            get { return (bool)GetValue(ShowQuickAccessToolBarAboveRibbonProperty); }
            set { SetValue(ShowQuickAccessToolBarAboveRibbonProperty, value); }
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
            Ribbon ribbon = (Ribbon)d;
            if (ribbon.titleBar != null) ribbon.titleBar.InvalidateMeasure();
            ribbon.SaveState();
        }

        /// <summary>
        /// Gets collection of contextual tab groups
        /// </summary>
        public ObservableCollection<RibbonContextualTabGroup> ContextualGroups
        {
            get
            {
                if (groups == null)
                {
                    groups = new ObservableCollection<RibbonContextualTabGroup>();
                    groups.CollectionChanged += OnGroupsCollectionChanged;
                }
                return this.groups;
            }
        }
        /// <summary>
        /// Handles collection of contextual tab groups ghanges
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (titleBar != null) titleBar.Items.Insert(e.NewStartingIndex + i, e.NewItems[i]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (titleBar != null) titleBar.Items.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (titleBar != null) titleBar.Items.Remove(item);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (titleBar != null) titleBar.Items.Add(item);
                    }
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
                if (tabs == null)
                {
                    tabs = new ObservableCollection<RibbonTabItem>();
                    tabs.CollectionChanged += OnTabsCollectionChanged;
                }
                return tabs;
            }
        }

        /// <summary>
        /// Handles collection of ribbon tabs changed
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnTabsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (tabControl != null) tabControl.Items.Insert(e.NewStartingIndex + i, e.NewItems[i]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (tabControl != null) tabControl.Items.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (tabControl != null) tabControl.Items.Remove(item);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (tabControl != null) tabControl.Items.Add(item);
                    }
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
                if (toolBarItems == null)
                {
                    toolBarItems = new ObservableCollection<UIElement>();
                    toolBarItems.CollectionChanged += OnToolbarItemsCollectionChanged;
                }
                return toolBarItems;
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
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Insert(e.NewStartingIndex + i, (UIElement)e.NewItems[i]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Remove(item as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Remove(item as UIElement);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Add(item as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets quick access toolbar associated with the ribbon
        /// </summary>
        internal QuickAccessToolBar QuickAccessToolBar
        {
            get { return quickAccessToolBar; }
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (layoutRoot != null) list.Add(layoutRoot);
                if (Menu != null) list.Add(Menu);
                if (quickAccessToolBar != null) list.Add(quickAccessToolBar);
                if ((tabControl != null) && (tabControl.ToolbarPanel != null)) list.Add(tabControl.ToolbarPanel);
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets collection of quick access menu items
        /// </summary>
        public ObservableCollection<QuickAccessMenuItem> QuickAccessItems
        {
            get
            {
                if (quickAccessItems == null)
                {
                    quickAccessItems = new ObservableCollection<QuickAccessMenuItem>();
                    quickAccessItems.CollectionChanged += OnQuickAccessItemsCollectionChanged;
                }
                return quickAccessItems;
            }
        }
        /// <summary>
        /// Handles collection of quick access menu items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        QuickAccessMenuItem menuItem = (QuickAccessMenuItem)e.NewItems[i];
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Insert(e.NewStartingIndex + i, menuItem);
                        menuItem.Ribbon = this;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        QuickAccessMenuItem menuItem = (QuickAccessMenuItem)item;
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Remove(menuItem);
                        menuItem.Ribbon = null;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        QuickAccessMenuItem menuItem = (QuickAccessMenuItem)item;
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Remove(menuItem);
                        menuItem.Ribbon = null;
                    }
                    foreach (object item in e.NewItems)
                    {
                        QuickAccessMenuItem menuItem = (QuickAccessMenuItem)item;
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Add(menuItem);
                        menuItem.Ribbon = this;
                    }
                    break;
            }
        }

       

       /* /// <summary>
        /// Handles collection of backstage items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Th event data</param>
        private void OnBackstageItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Add(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Remove(item);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Remove(item);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Add(item);
                    }
                    break;
            }
        }*/

       
        /// <summary>
        /// Gets or set whether Customize Quick Access Toolbar menu item is shown
        /// </summary>
        public bool CanCustomizeQuickAccessToolBar
        {
            get { return (bool)GetValue(CanCustomizeQuickAccessToolBarProperty); }
            set { SetValue(CanCustomizeQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeQuickAccessToolBar.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeQuickAccessToolBarProperty =
            DependencyProperty.Register("CanCustomizeQuickAccessToolBar", typeof(bool),
            typeof(Ribbon), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets or set whether Customize Ribbon menu item is shown
        /// </summary>
        public bool CanCustomizeRibbon
        {
            get { return (bool)GetValue(CanCustomizeRibbonProperty); }
            set { SetValue(CanCustomizeRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeRibbon. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeRibbonProperty =
            DependencyProperty.Register("CanCustomizeRibbon", typeof(bool),
            typeof(Ribbon), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets or sets whether ribbon is minimized
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMinimized. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMinimizedProperty =
            DependencyProperty.Register("IsMinimized", typeof(bool),
            typeof(Ribbon), new UIPropertyMetadata(false, OnIsMinimizedChanged));

        private static void OnIsMinimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = (Ribbon)d;
            ribbon.SaveState();
            if (!(bool)e.NewValue)
            {
                if (ribbon.tabControl.SelectedIndex == -1)
                    ribbon.LayoutUpdated += ribbon.OnIsOpenLayoutUpdated;
                //ribbon.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(delegate { ribbon.tabControl.SelectedIndex = 0; }));
            }
        }

        private void OnIsOpenLayoutUpdated(object sender, EventArgs e)
        {
            LayoutUpdated -= OnIsOpenLayoutUpdated;
            tabControl.SelectedIndex = 0;
        }

        /// <summary>
        /// Gets whether ribbon is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            private set { SetValue(IsCollapsedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsCollapsedPropertyKey =
       DependencyProperty.RegisterReadOnly("IsCollapsed", typeof(bool),
       typeof(Ribbon), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty = IsCollapsedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets whether QAT is visible
        /// </summary>
        public bool IsQuickAccessToolBarVisible
        {
            get { return (bool)GetValue(IsQuickAccessToolBarVisibleProperty); }
            set { SetValue(IsQuickAccessToolBarVisibleProperty, value); }
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
            get { return (bool)GetValue(CanQuickAccessLocationChangingProperty); }
            set { SetValue(CanQuickAccessLocationChangingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanQuickAccessLocationChanging.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanQuickAccessLocationChangingProperty =
            DependencyProperty.Register("CanQuickAccessLocationChanging", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true));



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

        // Occurs whe ntoggle minimize command executed
        private static void OnToggleMinimizeTheRibbonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.tabControl != null) ribbon.tabControl.IsMinimized = !ribbon.tabControl.IsMinimized;
        }

        // Occurs when show quick access below command executed
        private static void OnShowQuickAccessBelowCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            ribbon.ShowQuickAccessToolBarAboveRibbon = false;
        }

        // Occurs when show quick access above command executed
        private static void OnShowQuickAccessAboveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            ribbon.ShowQuickAccessToolBarAboveRibbon = true;
        }

        // Occurs when remove from quick access command executed
        private static void OnRemoveFromQuickAccessCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.quickAccessToolBar != null)
            {
                UIElement element = ribbon.quickAccessElements.First(x => x.Value == e.OriginalSource).Key;
                ribbon.RemoveFromQuickAccessToolBar(element);
            }
        }

        // Occurs when add to quick access command executed
        private static void OnAddToQuickAccessCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.quickAccessToolBar != null)
            {
                ribbon.AddToQuickAccessToolBar(e.OriginalSource as UIElement);
            }
        }

        // Occurs when customize quick access command executed
        private static void OnCustomizeQuickAccessToolbarCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.CustomizeQuickAccessToolbar != null) ribbon.CustomizeQuickAccessToolbar(sender, EventArgs.Empty);
        }

        // Occurs when customize the ribbon command executed
        private static void OnCustomizeTheRibbonCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.CustomizeTheRibbon != null) ribbon.CustomizeTheRibbon(sender, EventArgs.Empty);
        }

        // Occurs when customize quick access command can execute handles
        private static void OnCustomizeQuickAccessToolbarCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as Ribbon).CanCustomizeQuickAccessToolBar;
        }

        // Occurs when customize the ribbon command can execute handles
        private static void OnCustomizeTheRibbonCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (sender as Ribbon).CanCustomizeRibbon;
        }

        // Occurs when remove from quick access command can execute handles
        private static void OnRemoveFromQuickAccessCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.IsQuickAccessToolBarVisible)
            {
                e.CanExecute = ribbon.quickAccessElements.ContainsValue(e.OriginalSource as UIElement);
            }
            else e.CanExecute = false;
        }

        // Occurs when add to quick access command can execute handles
        private static void OnAddToQuickAccessCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Ribbon ribbon = sender as Ribbon;
            if (ribbon.IsQuickAccessToolBarVisible)
            {
                if (e.OriginalSource is Gallery) e.CanExecute = !ribbon.IsInQuickAccessToolBar(FindParentRibbonControl(e.OriginalSource as DependencyObject) as UIElement);
                else e.CanExecute = !ribbon.IsInQuickAccessToolBar(e.OriginalSource as UIElement);
            }
            else e.CanExecute = false;
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
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.AddToQuickAccessCommand, OnAddToQuickAccessCommandExecuted, OnAddToQuickAccessCommandCanExecute));            
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.RemoveFromQuickAccessCommand, OnRemoveFromQuickAccessCommandExecuted, OnRemoveFromQuickAccessCommandCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.ShowQuickAccessAboveCommand, OnShowQuickAccessAboveCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.ShowQuickAccessBelowCommand, OnShowQuickAccessBelowCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.ToggleMinimizeTheRibbonCommand, OnToggleMinimizeTheRibbonCommandExecuted));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.CustomizeTheRibbonCommand, OnCustomizeTheRibbonCommandExecuted, OnCustomizeTheRibbonCommandCanExecute));
            CommandManager.RegisterClassCommandBinding(typeof(Ribbon), new CommandBinding(Ribbon.CustomizeQuickAccessToolbarCommand, OnCustomizeQuickAccessToolbarCommandExecuted, OnCustomizeQuickAccessToolbarCommandCanExecute));

            InitRibbonContextMenu();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ribbon()
        {
            VerticalAlignment = VerticalAlignment.Top;
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            keyTipService = new KeyTipService(this);
        }

        #endregion

        #region Overrides

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.NewSize.Width < MinimalVisibleWidth) || (e.NewSize.Height < MinimalVisibleHeight)) IsCollapsed = true;
            else IsCollapsed = false;
        }

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.UIElement.GotFocus 
        /// event reaches this element in its route.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (tabControl != null)
            {
                RibbonTabItem ribbonTabItem = (RibbonTabItem)tabControl.SelectedItem;
                if (ribbonTabItem != null) ribbonTabItem.Focus();
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502")]
        public override void OnApplyTemplate()
        {
            if (layoutRoot != null) RemoveLogicalChild(layoutRoot);

            layoutRoot = GetTemplateChild("PART_LayoutRoot") as Panel;

            if (layoutRoot != null) AddLogicalChild(layoutRoot);

            if ((titleBar != null) && (groups != null))
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    titleBar.Items.Remove(groups[i]);
                }
            }
            titleBar = GetTemplateChild("PART_RibbonTitleBar") as RibbonTitleBar;
            if ((titleBar != null) && (groups != null))
            {
                for (int i = 0; i < groups.Count; i++)
                {
                    titleBar.Items.Add(groups[i]);
                }
            }
            RibbonTabItem selectedTab = selectedTabItem;
            if (tabControl != null)
            {
                tabControl.SelectionChanged -= OnTabControlSelectionChanged;
                selectedTab = tabControl.SelectedItem as RibbonTabItem;
            }
            if ((tabControl != null) && (tabs != null))
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabControl.Items.Remove(tabs[i]);
                }
            }

            if ((tabControl != null) && (toolBarItems != null))
            {
                for (int i = 0; i < toolBarItems.Count; i++)
                {
                    tabControl.ToolBarItems.Remove(toolBarItems[i]);
                }
            }
            tabControl = GetTemplateChild("PART_RibbonTabControl") as RibbonTabControl;
            if (tabControl != null)
            {
                tabControl.SelectionChanged += OnTabControlSelectionChanged;
                tabControl.IsMinimized = IsMinimized;
                Binding minimizeBinding = new Binding("IsMinimized");
                minimizeBinding.Source = this;
                minimizeBinding.Mode = BindingMode.TwoWay;
                tabControl.SetBinding(RibbonTabControl.IsMinimizedProperty, minimizeBinding);
            }
            if ((tabControl != null) && (tabs != null))
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabControl.Items.Add(tabs[i]);
                }
                tabControl.SelectedItem = selectedTab;
                if (tabControl.SelectedItem == null)
                {
                    //bool isBacstageOpen = IsBackstageOpen;
                    //tabControl.SelectedIndex = selectedTabIndex >= 0 ? selectedTabIndex : 0;
                    //IsBackstageOpen = isBacstageOpen;
                }
            }

            if ((tabControl != null) && (toolBarItems != null))
            {
                for (int i = 0; i < toolBarItems.Count; i++)
                {
                    tabControl.ToolBarItems.Add(toolBarItems[i]);
                }
            }


            if (quickAccessToolBar != null)
            {
                quickAccessStream = new MemoryStream();
                SaveState(quickAccessStream);
                ClearQuickAccessToolBar();
            }

            if (quickAccessToolBar != null)
            {
                quickAccessToolBar.ItemsChanged -= OnQuickAccessItemsChanged;
                if (quickAccessItems != null)
                {
                    for (int i = 0; i < quickAccessItems.Count; i++)
                    {
                        quickAccessToolBar.QuickAccessItems.Remove(quickAccessItems[i]);
                    }
                }
            }

            quickAccessToolBar = GetTemplateChild("PART_QuickAccessToolBar") as QuickAccessToolBar;
            if (quickAccessToolBar != null)
            {
                if (quickAccessItems != null)
                {
                    for (int i = 0; i < quickAccessItems.Count; i++)
                    {
                        quickAccessToolBar.QuickAccessItems.Add(quickAccessItems[i]);
                    }
                }
                quickAccessToolBar.ItemsChanged += OnQuickAccessItemsChanged;

                Binding binding = new Binding("CanQuickAccessLocationChanging");
                binding.Source = this;
                binding.Mode = BindingMode.OneWay;
                quickAccessToolBar.SetBinding(Fluent.QuickAccessToolBar.CanQuickAccessLocationChangingProperty, binding);
            }

            if (quickAccessToolBar != null)
            {
                if (quickAccessToolBar.Parent == null) AddLogicalChild(quickAccessToolBar);
                quickAccessToolBar.Loaded += OnFirstToolbarLoaded;
            }

            if (backstageButton != null)
            {
                if (backstageItems != null)
                {
                    for (int i = 0; i < backstageItems.Count; i++)
                    {
                        //backstageButton.Backstage.Items.Remove(backstageItems[i]);
                    }
                }
            }
            backstageButton = GetTemplateChild("PART_BackstageButton") as Backstage;
            adorner = null;
            if (backstageButton != null)
            {
                Binding binding = new Binding("IsBackstageOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                backstageButton.SetBinding(Backstage.IsOpenProperty, binding);
                if (backstageItems != null)
                {
                    for (int i = 0; i < backstageItems.Count; i++)
                    {
                        //backstageButton.Backstage.Items.Add(backstageItems[i]);
                    }
                }
            }

            if (ownerWindow == null)
            {
                ownerWindow = Window.GetWindow(this);
                Binding binding = new Binding("Title");
                binding.Mode = BindingMode.OneWay;
                binding.Source = ownerWindow;
                SetBinding(TitleProperty, binding);
            }
        }

        private void OnFirstToolbarLoaded(object sender, RoutedEventArgs e)
        {
            quickAccessToolBar.Loaded -= OnFirstToolbarLoaded;
            if (quickAccessStream != null)
            {
                quickAccessStream.Position = 0;
                LoadState(quickAccessStream);
                quickAccessStream.Close();
            }
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
            if (element == null) return false;
            return quickAccessElements.ContainsKey(element);
        }

        /// <summary>
        /// Adds the given element to quick access toolbar
        /// </summary>
        /// <param name="element">Element</param>
        public void AddToQuickAccessToolBar(UIElement element)
        {
            if (element is Gallery) element = FindParentRibbonControl(element) as UIElement;
            if (!QuickAccessItemsProvider.IsSupported(element)) return;
            if (!IsInQuickAccessToolBar(element))
            {
                UIElement control = QuickAccessItemsProvider.GetQuickAccessItem(element);

                quickAccessElements.Add(element, control);
                quickAccessToolBar.Items.Add(control);
                quickAccessToolBar.InvalidateMeasure();
            }
        }

        private static IRibbonControl FindParentRibbonControl(DependencyObject element)
        {
            DependencyObject parent = LogicalTreeHelper.GetParent(element);
            while (parent != null)
            {
                IRibbonControl control = parent as IRibbonControl;
                if (control != null) return control;
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
            Debug.WriteLine(element);
            if (IsInQuickAccessToolBar(element))
            {
                UIElement quickAccessItem = quickAccessElements[element];
                quickAccessElements.Remove(element);
                quickAccessToolBar.Items.Remove(quickAccessItem);
                quickAccessToolBar.InvalidateMeasure();
            }

        }

        /// <summary>
        /// Clears quick access toolbar
        /// </summary>
        public void ClearQuickAccessToolBar()
        {
            quickAccessElements.Clear();
            if (quickAccessToolBar != null) quickAccessToolBar.Items.Clear();
        }

        #endregion

        #region Event Handling

        // Handles tab control selection changed
        void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                //if (IsBackstageOpen)
                //{
                //    savedTabItem = e.AddedItems[0] as RibbonTabItem;
                //    if (savedTabItem != null) IsBackstageOpen = false;
                //}
            }
            if (tabControl != null)
            {
                selectedTabItem = tabControl.SelectedItem as RibbonTabItem;
                selectedTabIndex = tabControl.SelectedIndex;
            }
            if (SelectedTabChanged != null) SelectedTabChanged(this, EventArgs.Empty);
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            keyTipService.Attach();
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.SizeChanged += OnSizeChanged;
                window.KeyDown += OnKeyDown;
            }
            InitialLoadState();

            /*if (IsBackstageOpen) ShowBackstage();
            else*/ if ((tabControl != null) && (tabControl.SelectedIndex == -1) && (!IsMinimized)) tabControl.SelectedIndex = 0;
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1 && ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control))
            {
                IsMinimized = !IsMinimized;
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            keyTipService.Detach();
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.SizeChanged -= OnSizeChanged;
                window.KeyDown -= OnKeyDown;
            }
        }

        #endregion

        #region Private methods

       

        // Handles backstage Esc key keydown
        void OnBackstageEscapeKeyDown(object sender, KeyEventArgs e)
        {
           // if (e.Key == Key.Escape) IsBackstageOpen = false;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Get adorner layer for element
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Adorner layer</returns>
        static AdornerLayer GetAdornerLayer(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
            }
        }

        #endregion

        #region Show / Hide Backstage

        // We have to collapse WindowsFormsHost while Backstate is open
        Dictionary<FrameworkElement, Visibility> collapsedElements =
            new Dictionary<FrameworkElement, Visibility>();

        // Show backstage
        void ShowBackstage()
        {
            if (!IsLoaded) return;

            AdornerLayer layer = GetAdornerLayer(this);
            if (adorner == null)
            {
                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    // TODO: in design mode it is required to use design time adorner
                    FrameworkElement topLevelElement = (FrameworkElement)VisualTreeHelper.GetParent(this);
                    double topOffset = backstageButton.TranslatePoint(new Point(0, backstageButton.ActualHeight), topLevelElement).Y;
                    //adorner = new BackstageAdorner(topLevelElement, backstageButton.Backstage, topOffset);
                }
                else
                {
                    FrameworkElement topLevelElement = (FrameworkElement)Window.GetWindow(this).Content;
                    double topOffset = backstageButton.TranslatePoint(new Point(0, backstageButton.ActualHeight), topLevelElement).Y;
                    //adorner = new BackstageAdorner(topLevelElement, backstageButton.Backstage, topOffset);
                }
            }
            layer.Add(adorner);
            if (tabControl != null)
            {
                savedTabItem = tabControl.SelectedItem as RibbonTabItem;
                if (savedTabItem == null && tabControl.Items.Count > 0)
                    savedTabItem = (RibbonTabItem)tabControl.Items[0];
                tabControl.SelectedItem = null;
            }
            if (quickAccessToolBar != null) quickAccessToolBar.IsEnabled = false;
            if (titleBar != null) titleBar.IsEnabled = false;

            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown += OnBackstageEscapeKeyDown;
                savedMinWidth = window.MinWidth;
                savedMinHeight = window.MinHeight;

                SaveWindowSize(window);

                if (savedMinWidth < 500) window.MinWidth = 500;
                if (savedMinHeight < 400) window.MinHeight = 400;
                window.SizeChanged += OnWindowSizeChanged;

                // We have to collapse WindowsFormsHost while Backstate is open
                CollapseWindowsFormsHosts(window);
            }
        }

        // We have to collapse WindowsFormsHost while Backstate is open
        void CollapseWindowsFormsHosts(DependencyObject parent)
        {
            FrameworkElement frameworkElement = parent as FrameworkElement;
            if (frameworkElement != null)
            {
                if ((parent is WindowsFormsHost || parent is WebBrowser) &&
                    frameworkElement.Visibility != Visibility.Collapsed)
                {
                    collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                    frameworkElement.Visibility = Visibility.Collapsed;
                    return;
                }
            }
            // Traverse visual tree
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
            }
        }

        // Hide backstage
        void HideBackstage()
        {
            if (!IsLoaded || adorner == null) return;

            AdornerLayer layer = GetAdornerLayer(this);
            layer.Remove(adorner);
            if (tabControl != null) tabControl.SelectedItem = savedTabItem;
            if (quickAccessToolBar != null) quickAccessToolBar.IsEnabled = true;
            if (titleBar != null) titleBar.IsEnabled = true;

            Window window = Window.GetWindow(this);
            if (window != null)
            {
                window.PreviewKeyDown -= OnBackstageEscapeKeyDown;
                window.SizeChanged -= OnWindowSizeChanged;

                window.MinWidth = savedMinWidth;
                window.MinHeight = savedMinHeight;
                NativeMethods.SetWindowPos((new WindowInteropHelper(window)).Handle,
                                           new IntPtr(NativeMethods.HWND_NOTOPMOST),
                                           0, 0, savedWidth, savedHeight, NativeMethods.SWP_NOMOVE);
            }

            // Uncollapse elements
            foreach (var element in collapsedElements) element.Key.Visibility = element.Value;
            collapsedElements.Clear();

            if (SelectedTabIndex < 0)
            {
                if ((tabControl != null) && (tabControl.SelectedIndex < 0)) SelectedTabIndex = 0;
                else SelectedTabIndex = tabControl.SelectedIndex;
            }
        }

        #endregion

        void SaveWindowSize(Window wnd)
        {
            NativeMethods.WINDOWINFO info = new NativeMethods.WINDOWINFO();
            info.cbSize = (uint)Marshal.SizeOf(info);
            NativeMethods.GetWindowInfo((new WindowInteropHelper(wnd)).Handle, ref info);
            savedWidth = info.rcWindow.Right - info.rcWindow.Left;
            savedHeight = info.rcWindow.Bottom - info.rcWindow.Top;
        }

        void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Window wnd = Window.GetWindow(this);
            SaveWindowSize(wnd);
        }

        #region State Management

        #region Initial State Loading

        // Initial state loading
        void InitialLoadState()
        {
            LayoutUpdated += OnJustLayoutUpdated;
        }

        void OnJustLayoutUpdated(object sender, EventArgs e)
        {
            LayoutUpdated -= OnJustLayoutUpdated;
            if (!IsStateLoaded) LoadState();
        }

        #endregion

        #region Load / Save to Isolated Storage

        // Handles items changing in QAT
        void OnQuickAccessItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SaveState();
        }

        // Saves to Isolated Storage (in user store for domain)
        void SaveState()
        {
            // Check whether automatic save is valid now
            if (!AutomaticStateManagement || !IsStateLoaded) return;

            IsolatedStorageFile storage = GetIsolatedStorageFile();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("Fluent.Ribbon.State.2.0.dat", FileMode.Create, FileAccess.Write, storage))
            {
                SaveState(stream);
            }
        }

        // Loads from Isolated Storage (in user store for domain)
        void LoadState()
        {
            if (!AutomaticStateManagement) return;

            IsolatedStorageFile storage = GetIsolatedStorageFile();
            if (FileExists(storage, "Fluent.Ribbon.State.2.0.dat"))
            {
                using (IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream("Fluent.Ribbon.State.2.0.dat",
                                                  FileMode.Open, FileAccess.Read, storage))
                {
                    LoadState(stream);
                }
            }

            // Now we can save states
            IsStateLoaded = true;
        }

        // Gets a proper isolated storage file
        static IsolatedStorageFile GetIsolatedStorageFile()
        {
            return AppDomain.CurrentDomain.ActivationContext != null ? 
                IsolatedStorageFile.GetUserStoreForDomain() : 
                IsolatedStorageFile.GetUserStoreForAssembly();
        }

        /// <summary>
        /// Resets automatically saved state
        /// </summary>
        public static void ResetState()
        {
            IsolatedStorageFile storage = GetIsolatedStorageFile();
            string fileName = "Fluent.Ribbon.State.2.0.dat";
            if (FileExists(storage, fileName)) storage.DeleteFile(fileName);
        }

        // Determinates whether the given file exists in the given storage
        static bool FileExists(IsolatedStorageFile storage, string fileName)
        {
            string[] files = storage.GetFileNames(fileName);
            return files.Length != 0;
        }

        #endregion

        #region Save to Stream

        /// <summary>
        /// Saves state to the given stream
        /// </summary>
        /// <param name="stream">Stream</param>
        public void SaveState(Stream stream)
        {
            StringBuilder builder = new StringBuilder();

            // Save Ribbon State
            builder.Append(IsMinimized.ToString(CultureInfo.InvariantCulture));
            builder.Append(',');
            builder.Append(ShowQuickAccessToolBarAboveRibbon.ToString(CultureInfo.InvariantCulture));
            builder.Append('|');

            // Save QAT items
            Dictionary<FrameworkElement, string> paths = new Dictionary<FrameworkElement, string>();
            TraverseLogicalTree(this, "", paths);
            // Foreach items and see whether path is found for the item
            foreach (var element in quickAccessElements)
            {
                if (paths.ContainsKey((FrameworkElement)element.Key))
                {
                    builder.Append(paths[(FrameworkElement)element.Key]);
                    builder.Append(';');
                }
                else
                {
                    // Item is not found in logical tree, output to debug console
                    FrameworkElement control = element.Key as FrameworkElement;
                    string controlName = (control != null && !String.IsNullOrEmpty(control.Name)) ?
                        String.Format(CultureInfo.InvariantCulture, " (name of the control is {0})", control.Name) : "";
                    Debug.WriteLine("Control " + element.Key.GetType().Name + " is not found in logical tree during QAT saving" + controlName);
                }
            }

            StreamWriter writer = new StreamWriter(stream);
            writer.Write(builder.ToString());

            writer.Flush();
        }

        // Traverse logical tree and find QAT items, remember paths
        void TraverseLogicalTree(DependencyObject item, string path, IDictionary<FrameworkElement, string> paths)
        {
            // Is this item in QAT
            FrameworkElement uielement = item as FrameworkElement;
            if (uielement != null && quickAccessElements.ContainsKey(uielement))
            {
                if (!paths.ContainsKey(uielement)) paths.Add(uielement, path);
            }

            object[] children = LogicalTreeHelper.GetChildren(item).Cast<object>().ToArray();
            for (int i = 0; i < children.Length; i++)
            {
                DependencyObject child = children[i] as DependencyObject;
                if (child == null) continue;
                TraverseLogicalTree(child, path + i + ",", paths);
            }
        }

        #endregion

        #region Load from Stream

        /// <summary>
        /// Loads state from the given stream
        /// </summary>
        /// <param name="stream">Stream</param>
        public void LoadState(Stream stream)
        {
            suppressAutomaticStateManagement = true;

            StreamReader reader = new StreamReader(stream);
            string[] splitted = reader.ReadToEnd().Split('|');

            if (splitted.Length != 2) return;

            // Load Ribbon State
            string[] ribbonProperties = splitted[0].Split(',');
            IsMinimized = Boolean.Parse(ribbonProperties[0]);
            ShowQuickAccessToolBarAboveRibbon = Boolean.Parse(ribbonProperties[1]);

            // Load items
            string[] items = splitted[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (quickAccessToolBar != null) quickAccessToolBar.Items.Clear();
            quickAccessElements.Clear();

            for (int i = 0; i < items.Length; i++) ParseAndAddToQuickAccessToolBar(items[i]);

            // Sync QAT menu items
            foreach (QuickAccessMenuItem menuItem in QuickAccessItems)
            {
                menuItem.IsChecked = IsInQuickAccessToolBar(menuItem.Target);
            }

            suppressAutomaticStateManagement = false;
        }

        // Loads item and add to QAT
        void ParseAndAddToQuickAccessToolBar(string data)
        {
            int[] indices = data.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x, CultureInfo.InvariantCulture)).ToArray();

            DependencyObject current = this;
            for (int i = 0; i < indices.Length; i++)
            {
                object[] children = LogicalTreeHelper.GetChildren(current).OfType<object>().ToArray();
                bool indexIsInvalid = children.Length <= indices[i];
                DependencyObject item = indexIsInvalid ? null : children[indices[i]] as DependencyObject;
                if (item == null)
                {
                    // Path is incorrect
                    Debug.WriteLine("Error while QAT items loading: one of the paths is invalid");
                    return;
                }
                current = item;
            }

            UIElement result = current as UIElement;
            if ((result == null) || (!QuickAccessItemsProvider.IsSupported(result)))
            {
                // Item is invalid
                Debug.WriteLine("Error while QAT items loading: an item is not be able to be added to QAT");
                return;
            }

            AddToQuickAccessToolBar(result);
        }

        #endregion

        #region IsStateLoaded

        /// <summary>
        /// Gets or sets whether state is loaded
        /// </summary>
        bool IsStateLoaded
        {
            get;
            set;
        }

        #endregion

        #region AutomaticStateManagement Property

        // To temporary suppress automatic management
        bool suppressAutomaticStateManagement;

        /// <summary>
        /// Gets or sets whether Quick Access ToolBar can 
        /// save and load its state automatically
        /// </summary>
        public bool AutomaticStateManagement
        {
            get { return (bool)GetValue(AutomaticStateManagementProperty); }
            set { SetValue(AutomaticStateManagementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutomaticStateManagement. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutomaticStateManagementProperty =
            DependencyProperty.Register("AutomaticStateManagement", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(true, OnAutoStateManagement, CoerceAutoStateManagement));

        private static object CoerceAutoStateManagement(DependencyObject d, object basevalue)
        {
            Ribbon ribbon = (Ribbon)d;
            if (ribbon.suppressAutomaticStateManagement) return false;
            return basevalue;
        }

        static void OnAutoStateManagement(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = (Ribbon)d;
            if ((bool)e.NewValue) ribbon.InitialLoadState();
        }

        #endregion

        #endregion
    }
}
