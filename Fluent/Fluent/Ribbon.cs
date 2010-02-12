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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents the main Ribbon control which consists of multiple tabs, each of which
    /// containing groups of controls.  The Ribbon also provides improved context
    /// menus, enhanced screen tips, and keyboard shortcuts.
    /// </summary>
    [ContentProperty("Tabs")]
    [TemplatePart(Name = "PART_BackstageButton", Type = typeof(BackstageButton))]
    public class Ribbon: Control
    {
        #region Consts

        /// <summary>
        /// Minimal width of ribbon parent wndow
        /// </summary>
        public const double MinimalVisibleWidth = 300;
        /// <summary>
        /// Minimal height of ribbon parent wndow
        /// </summary>
        public const double MinimalVisibleHeight = 250;

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
        private BackstageButton backstageButton;

        // Context Menu
        ContextMenu ribbonContextMenu;
        MenuItem addToQuickAccessMenuItem;
        MenuItem addGroupToQuickAccessMenuItem;
        MenuItem addMenuToQuickAccessMenuItem;
        MenuItem addGalleryToQuickAccessMenuItem;
        MenuItem removeFromQuickAccessMenuItem;
        MenuItem customizeQuickAccessToolbarMenuItem;
        MenuItem showQuickAccessToolbarBelowTheRibbonMenuItem;
        MenuItem showQuickAccessToolbarAboveTheRibbonMenuItem;
        MenuItem customizeTheRibbonMenuItem;
        MenuItem minimizeTheRibbonMenuItem;

        // Handles F10, Alt and so on
        KeyTipService keyTipService;
        
        // Collection of quickaccess menu items
        private ObservableCollection<QuickAccessMenuItem> quickAccessItems;
        // Adornet for backstage
        private BackstageAdorner adorner;
        // Saved when backstage opened tab item
        private RibbonTabItem savedTabItem;

        Dictionary<UIElement,UIElement> quickAccessElements = new Dictionary<UIElement, UIElement>();

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets KeyTip.Keys for Backstage
        /// </summary>
        public string BackstageKeyTipKeys
        {
            get { return (string)GetValue(BackstageKeyTipKeysProperty); }
            set { SetValue(BackstageKeyTipKeysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BackstageKeyTipKeys. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageKeyTipKeysProperty =
            DependencyProperty.Register("BackstageKeyTipKeys", typeof(string), typeof(Ribbon), new UIPropertyMetadata("F"));


        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        internal  RibbonTitleBar TitleBar
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
            Ribbon ribbon = d as Ribbon;
            if (ribbon.titleBar != null) ribbon.titleBar.InvalidateMeasure();
        }

        /// <summary>
        /// Gets collection of contextual tab groups
        /// </summary>
        public ObservableCollection<RibbonContextualTabGroup> ContextualGroups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new ObservableCollection<RibbonContextualTabGroup>();
                    this.groups.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupsCollectionChanged);
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
                    foreach (object obj2 in e.NewItems)
                    {                        
                        if (titleBar != null) titleBar.Items.Add(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {                        
                        if (titleBar != null) titleBar.Items.Remove(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {                        
                        if (titleBar != null) titleBar.Items.Remove(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (titleBar != null) titleBar.Items.Add(obj5);
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
                if (this.tabs == null)
                {
                    this.tabs = new ObservableCollection<RibbonTabItem>();
                    this.tabs.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnTabsCollectionChanged);
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
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.Items.Add(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.Items.Remove(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.Items.Remove(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.Items.Add(obj5);
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
                if (this.toolBarItems == null)
                {
                    this.toolBarItems = new ObservableCollection<UIElement>();
                    this.toolBarItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnToolbarItemsCollectionChanged);
                }
                return this.toolBarItems;
            }
        }

        /// <summary>
        /// handles colection of toolbar i8tenms changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolBarItems.Add(obj5 as UIElement);
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
                if(layoutRoot!=null)list.Add(layoutRoot);
                //if(ShowQuickAccessToolBarAboveRibbon)if (quickAccessToolBar != null) list.Add(quickAccessToolBar);
                return list.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets collectionof quick access menu items
        /// </summary>
        public ObservableCollection<QuickAccessMenuItem> QuickAccessItems
        {
            get
            {
                if (this.quickAccessItems == null)
                {
                    this.quickAccessItems = new ObservableCollection<QuickAccessMenuItem>();
                    this.quickAccessItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnQuickAccessItemsCollectionChanged);
                }
                return this.quickAccessItems;
            }
        }
        /// <summary>
        /// Handles collectionof quick access menu items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Add(obj2 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Remove(obj3 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Remove(obj4 as QuickAccessMenuItem);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (quickAccessToolBar != null) quickAccessToolBar.QuickAccessItems.Add(obj5 as QuickAccessMenuItem);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets collection of backstage items
        /// </summary>
        public ObservableCollection<UIElement> BackstageItems
        {
            get
            {
                if (this.backstageItems == null)
                {
                    this.backstageItems = new ObservableCollection<UIElement>();
                    this.backstageItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnBackstageItemsCollectionChanged);
                }
                return this.backstageItems;
            }
        }

        /// <summary>
        /// Handles collection of backstage items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Th event data</param>
        private void OnBackstageItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (backstageButton != null) backstageButton.Backstage.Items.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets or sets whether backstage is opened
        /// </summary>
        public bool IsBackstageOpen
        {
            get { return (bool)GetValue(IsBackstageOpenProperty); }
            set { SetValue(IsBackstageOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsBackstageOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsBackstageOpenProperty =
            DependencyProperty.Register("IsBackstageOpen", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false, OnIsBackstageOpenChanged));

        /// <summary>
        /// Handles IsBackstageOpen property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnIsBackstageOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Ribbon ribbon = d as Ribbon;
            if((bool)e.NewValue)
            {
                ribbon.ShowBackstage();
            }
            else
            {
                ribbon.HideBackstage();
            }
        }

        /// <summary>
        /// Gets or sets backstage brush
        /// </summary>
        public Brush BackstageBrush
        {
            get { return (Brush)GetValue(BackstageBrushProperty); }
            set { SetValue(BackstageBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BackstageBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageBrushProperty =
            DependencyProperty.Register("BackstageBrush", typeof(Brush), typeof(Ribbon), new UIPropertyMetadata(Brushes.Blue));



        /// <summary>
        /// Gets or set whether Customize Quick Access Toolbar menu item is shown
        /// </summary>
        public bool CanCustomizeQuickAccessToolbar
        {
            get { return (bool)GetValue(CanCustomizeQuickAccessToolbarProperty); }
            set { SetValue(CanCustomizeQuickAccessToolbarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeQuickAccessToolbar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeQuickAccessToolbarProperty =
            DependencyProperty.Register("CanCustomizeQuickAccessToolbar", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets or set whether Customize Ribbon menu item is shown
        /// </summary>
        public bool CanCustomizeRibbon
        {
            get { return (bool)GetValue(CanCustomizeRibbonProperty); }
            set { SetValue(CanCustomizeRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanCustomizeRibbon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanCustomizeRibbonProperty =
            DependencyProperty.Register("CanCustomizeRibbon", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets or sets whether ribbon is minimized
        /// </summary>
        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMinimized.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMinimizedProperty =
            DependencyProperty.Register("IsMinimized", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets whether ribbon is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            private set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCollapsed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
               DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false));      

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Ribbon()
        {
            //StyleProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            //if (basevalue == null) basevalue = ThemesManager.DefaultRibbonStyle;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ribbon()
        {
            VerticalAlignment = VerticalAlignment.Top;
            keyTipService = new KeyTipService(this);
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
            Focusable = false;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;            
        }

        #endregion        

        #region Overrides

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.NewSize.Width < MinimalVisibleWidth)||(e.NewSize.Height < MinimalVisibleHeight)) IsCollapsed = true;
            else IsCollapsed = false;
        }

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.UIElement.GotFocus event reaches this element in its route.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (tabControl != null) (tabControl.SelectedItem as RibbonTabItem).Focus();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (layoutRoot!=null) RemoveLogicalChild(layoutRoot);
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
            if((titleBar!=null)&&(groups!=null))
            {
                for(int i=0;i<groups.Count;i++)
                {
                    titleBar.Items.Add(groups[i]);
                }
            }

            if (tabControl != null)
            {                
                tabControl.SelectionChanged -= OnTabControlSelectionChanged;
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
                Binding minimizeBinding = new Binding("IsMinimized");
                minimizeBinding.Source = tabControl;
                minimizeBinding.Mode = BindingMode.TwoWay;
                SetBinding(IsMinimizedProperty, minimizeBinding);
            }
            if ((tabControl != null)&&(tabs!=null))
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabControl.Items.Add(tabs[i]);
                }
            }

            if ((tabControl != null) && (toolBarItems != null))
            {
                for (int i = 0; i < toolBarItems.Count; i++)
                {
                    tabControl.ToolBarItems.Add(toolBarItems[i]);
                }
            }

            if ((quickAccessToolBar != null) && (quickAccessItems != null))
            {
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    quickAccessToolBar.QuickAccessItems.Remove(quickAccessItems[i]);
                }
            }
            quickAccessToolBar = GetTemplateChild("PART_QuickAccessToolBar") as QuickAccessToolBar;
            if ((quickAccessToolBar != null) && (quickAccessItems != null))
            {
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    quickAccessToolBar.QuickAccessItems.Add(quickAccessItems[i]);
                }
            }
            if (backstageButton != null)
            {
                if (backstageItems != null)
                {
                    for (int i = 0; i < backstageItems.Count; i++)
                    {
                        backstageButton.Backstage.Items.Remove(backstageItems[i]);
                    }
                }
            }
            backstageButton = GetTemplateChild("PART_BackstageButton") as BackstageButton;
            if (backstageButton != null) 
            {
                Binding binding = new Binding("IsBackstageOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                backstageButton.SetBinding(BackstageButton.IsOpenProperty, binding);
                if (backstageItems != null)
                {
                    for (int i = 0; i < backstageItems.Count; i++)
                    {
                        backstageButton.Backstage.Items.Add(backstageItems[i]);
                    }
                }
            }
            if (ribbonContextMenu != null) ribbonContextMenu.Opened -= OnContextMenuOpened;
            ribbonContextMenu = GetTemplateChild("PART_RibbonContextMenu") as ContextMenu;
            if (ribbonContextMenu != null) ribbonContextMenu.Opened += OnContextMenuOpened;
            if (addToQuickAccessMenuItem!=null) addToQuickAccessMenuItem.Click -= OnAddToQuickLaunchClick;
            addToQuickAccessMenuItem = GetTemplateChild("PART_AddToQuickAccessMenuItem") as MenuItem;
            if (addToQuickAccessMenuItem != null) addToQuickAccessMenuItem.Click += OnAddToQuickLaunchClick;
            if (addGroupToQuickAccessMenuItem != null) addGroupToQuickAccessMenuItem.Click -= OnAddToQuickLaunchClick;
            addGroupToQuickAccessMenuItem = GetTemplateChild("PART_AddGroupToQuickAccessMenuItem") as MenuItem;
            if (addGroupToQuickAccessMenuItem != null) addGroupToQuickAccessMenuItem.Click += OnAddToQuickLaunchClick;
            if (addMenuToQuickAccessMenuItem != null) addMenuToQuickAccessMenuItem.Click -= OnAddToQuickLaunchClick;
            addMenuToQuickAccessMenuItem = GetTemplateChild("PART_AddMenuToQuickAccessMenuItem") as MenuItem;
            if (addMenuToQuickAccessMenuItem != null) addMenuToQuickAccessMenuItem.Click += OnAddToQuickLaunchClick;
            if (addGalleryToQuickAccessMenuItem != null) addGalleryToQuickAccessMenuItem.Click -= OnAddToQuickLaunchClick;
            addGalleryToQuickAccessMenuItem = GetTemplateChild("PART_AddGalleryToQuickAccessMenuItem") as MenuItem;
            if (addGalleryToQuickAccessMenuItem != null) addGalleryToQuickAccessMenuItem.Click += OnAddToQuickLaunchClick;
            if (removeFromQuickAccessMenuItem != null) removeFromQuickAccessMenuItem.Click -= OnRemoveToQuickLaunchClick;
            removeFromQuickAccessMenuItem = GetTemplateChild("PART_RemoveFromQuickAccessMenuItem") as MenuItem;
            if (removeFromQuickAccessMenuItem != null) removeFromQuickAccessMenuItem.Click += OnRemoveToQuickLaunchClick;
            customizeQuickAccessToolbarMenuItem = GetTemplateChild("PART_CustomizeQuickAccessToolbarMenuItem") as MenuItem;
            if (showQuickAccessToolbarBelowTheRibbonMenuItem != null) showQuickAccessToolbarBelowTheRibbonMenuItem.Click -= OnShowBelowTheRibbonClick;
            showQuickAccessToolbarBelowTheRibbonMenuItem = GetTemplateChild("PART_ShowQuickAccessToolbarBelowTheRibbonMenuItem") as MenuItem;
            if (showQuickAccessToolbarBelowTheRibbonMenuItem != null) showQuickAccessToolbarBelowTheRibbonMenuItem.Click += OnShowBelowTheRibbonClick;
            if (showQuickAccessToolbarAboveTheRibbonMenuItem != null) showQuickAccessToolbarAboveTheRibbonMenuItem.Click -= OnShowAboveTheRibbonClick;
            showQuickAccessToolbarAboveTheRibbonMenuItem = GetTemplateChild("PART_ShowQuickAccessToolbarAboveTheRibbonMenuItem") as MenuItem;
            if (showQuickAccessToolbarAboveTheRibbonMenuItem != null) showQuickAccessToolbarAboveTheRibbonMenuItem.Click += OnShowAboveTheRibbonClick;
            customizeTheRibbonMenuItem = GetTemplateChild("PART_CustomizeTheRibbonMenuItem") as MenuItem;
            if (minimizeTheRibbonMenuItem != null) minimizeTheRibbonMenuItem.Click -= OnMinimizeRibbonClick;
            minimizeTheRibbonMenuItem = GetTemplateChild("PART_MinimizeTheRibbonMenuItem") as MenuItem;
            if (minimizeTheRibbonMenuItem != null) minimizeTheRibbonMenuItem.Click += OnMinimizeRibbonClick;
        }

        private void OnRemoveToQuickLaunchClick(object sender, RoutedEventArgs e)
        {
            if ((quickAccessToolBar != null) && (ribbonContextMenu != null) && (ribbonContextMenu.Tag != null))
            {
                UIElement element = quickAccessElements.First(x => x.Value == ribbonContextMenu.Tag).Key;
                RemoveFromQuickAccessToolbar(ribbonContextMenu.Tag as UIElement);
                quickAccessToolBar.Items.Remove(quickAccessElements[element]);
                quickAccessElements.Remove(element);
                quickAccessToolBar.InvalidateMeasure();
            }
        }

        private void OnMinimizeRibbonClick(object sender, RoutedEventArgs e)
        {
            if (tabControl!=null) tabControl.IsMinimized = !tabControl.IsMinimized;
        }

        private void OnShowAboveTheRibbonClick(object sender, RoutedEventArgs e)
        {
            ShowQuickAccessToolBarAboveRibbon = true;
        }

        private void OnShowBelowTheRibbonClick(object sender, RoutedEventArgs e)
        {
            ShowQuickAccessToolBarAboveRibbon = false;
        }

        private void OnAddToQuickLaunchClick(object sender, RoutedEventArgs e)
        {
            if ((quickAccessToolBar != null) && (ribbonContextMenu != null) && (ribbonContextMenu.Tag != null))
            {
                AddToQuickAccessToolbar(ribbonContextMenu.Tag as UIElement);
            }
        }

        private void OnContextMenuOpened(object sender, RoutedEventArgs e)
        {
            /*if (quickAccessToolBar != null)
            {
                RibbonTabControl ribbonTabControl = sender as RibbonTabControl;
                UIElement control = QuickAccessItemsProvider.FindSupportedControl(ribbonTabControl,
                                                                                 e.GetPosition(ribbonTabControl));
                if (control != null)
                {
                    AddToQuickAccessToolbar(control);
                }
            }*/
            if(ribbonContextMenu!=null)
            {
                FrameworkElement parentElement = RibbonPopup.GetActivePopup();
                if (parentElement == null) parentElement = this;
                else parentElement = (parentElement as RibbonPopup).Child as FrameworkElement;
                
                if (parentElement == null) parentElement = this;
                UIElement control = QuickAccessItemsProvider.FindSupportedControl(parentElement , Mouse.GetPosition(parentElement));
                if (control != null)
                {
                    ribbonContextMenu.Tag = control;
                    if (quickAccessElements.ContainsValue(control))
                    {
                        addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        removeFromQuickAccessMenuItem.Visibility = Visibility.Visible;
                        addMenuToQuickAccessMenuItem.IsEnabled = !IsInQuickAccessToolbar(control);
                    }
                    else if(control is ContextMenu)
                    {
                        addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addMenuToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addMenuToQuickAccessMenuItem.IsEnabled = !IsInQuickAccessToolbar(control);
                    }
                    else if(control is Gallery)
                    {
                        addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGalleryToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGalleryToQuickAccessMenuItem.IsEnabled = !IsInQuickAccessToolbar(control);
                    }
                    else if (control is RibbonGroupBox)
                    {
                        addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGroupToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGroupToQuickAccessMenuItem.IsEnabled = !IsInQuickAccessToolbar(control);
                    }
                    else 
                    {
                        addToQuickAccessMenuItem.Visibility = Visibility.Visible;
                        addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                        addToQuickAccessMenuItem.IsEnabled = !IsInQuickAccessToolbar(control);
                    }
                }
                else
                {
                    addToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                    addGroupToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                    addMenuToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                    addGalleryToQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                    removeFromQuickAccessMenuItem.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Quick Access Items Managment
               
        public bool IsInQuickAccessToolbar(UIElement element)
        {
            if(element==null) return false;
            return quickAccessElements.ContainsKey(element);
        }

        public void AddToQuickAccessToolbar(UIElement element)
        {
            if(!IsInQuickAccessToolbar(element))
            {
                UIElement control = QuickAccessItemsProvider.GetQuickAccessItem(element);
                quickAccessElements.Add(element,control);
                quickAccessToolBar.Items.Add(control);
                quickAccessToolBar.InvalidateMeasure();
            }
        }

        public void RemoveFromQuickAccessToolbar(UIElement element)
        {
            if (IsInQuickAccessToolbar(element))
            {
                quickAccessToolBar.Items.Remove(quickAccessElements[element]);
                quickAccessElements.Remove(element);
                quickAccessToolBar.InvalidateMeasure();
            }

        }

        #endregion

        #region Event handling

        // Handles tab control selection chaged
        private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                if (IsBackstageOpen)
                {
                    savedTabItem = e.AddedItems[0] as RibbonTabItem;
                    IsBackstageOpen = false;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            keyTipService.Attach();
            Window wnd = Window.GetWindow(this);
            if (wnd != null) wnd.SizeChanged += OnSizeChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            keyTipService.Detach();
            Window wnd = Window.GetWindow(this);
            if (wnd != null) wnd.SizeChanged -= OnSizeChanged;
        }

        #endregion

        #region Private methods

        // Show backstage
        private void ShowBackstage()
        {            
            AdornerLayer layer = GetAdornerLayer(this);
            if (adorner == null)
            {                
                UIElement topLevelElement = Window.GetWindow(this).Content as UIElement;
                double topOffset=backstageButton.TranslatePoint(new Point(0, backstageButton.ActualHeight), topLevelElement).Y;
                adorner = new BackstageAdorner(topLevelElement, backstageButton.Backstage, topOffset);
            }            
            layer.Add(adorner);
            if (tabControl != null)
            {
                savedTabItem = tabControl.SelectedItem as RibbonTabItem;
                tabControl.SelectedItem = null;
            }
            if(quickAccessToolBar!=null) quickAccessToolBar.IsEnabled = false;
            if(titleBar!=null) titleBar.IsEnabled = false;
            Window.GetWindow(this).PreviewKeyDown += OnBackstageEscapeKeyDown;
        }        

        // Hide backstage
        private void HideBackstage()
        {
            AdornerLayer layer = GetAdornerLayer(this);
            layer.Remove(adorner);
            if (tabControl != null) tabControl.SelectedItem = savedTabItem;
            if (quickAccessToolBar != null) quickAccessToolBar.IsEnabled = true;
            if (titleBar != null) titleBar.IsEnabled = true;
            Window.GetWindow(this).PreviewKeyDown -= OnBackstageEscapeKeyDown;
        }

        // Handles backstage Esc key keydown
        private void OnBackstageEscapeKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Escape)IsBackstageOpen = false;
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
    }
}
