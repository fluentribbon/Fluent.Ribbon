using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    [ContentProperty("Tabs")]
    public class Ribbon: Control
    {
        #region Fields

        private ObservableCollection<RibbonContextualTabGroup> groups = null;
        private ObservableCollection<RibbonTabItem> tabs = null;
        private ObservableCollection<UIElement> toolbarItems = null;
        private RibbonTitleBar titleBar = null;
        private RibbonTabControl tabControl = null;
        private QuickAccessToolbar quickAccessToolbar = null;

        private Panel layoutRoot = null;

        // Handles F10, Alt and so on
        KeyTipService keyTipService = null;

        private ObservableCollection<QuickAccessMenuItem> quickAccessItems = null;

        #endregion

        #region Properties

        internal  RibbonTitleBar TitleBar
        {
            get { return titleBar; }
        }

        public bool ShowQuickAccessToolbarAboveRibbon
        {
            get { return (bool)GetValue(ShowQuickAccessToolbarAboveRibbonProperty); }
            set { SetValue(ShowQuickAccessToolbarAboveRibbonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAboveRibbon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowQuickAccessToolbarAboveRibbonProperty =
            DependencyProperty.Register("ShowQuickAccessToolbarAboveRibbon", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false, OnShowQuickAccesToolbarAboveRibbonChanged));

        private static void OnShowQuickAccesToolbarAboveRibbonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((d as Ribbon).titleBar !=null)(d as Ribbon).titleBar.InvalidateMeasure();
        }


        public ObservableCollection<RibbonContextualTabGroup> Groups
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

        public ObservableCollection<UIElement> ToolbarItems
        {
            get
            {
                if (this.toolbarItems == null)
                {
                    this.toolbarItems = new ObservableCollection<UIElement>();
                    this.toolbarItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnToolbarItemsCollectionChanged);
                }
                return this.toolbarItems;
            }
        }

        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolbarItems.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolbarItems.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (tabControl != null) tabControl.ToolbarItems.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (tabControl != null) tabControl.ToolbarItems.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets quick access toolbar associated with the ribbon
        /// </summary>
        internal QuickAccessToolbar QuickAccessToolbar
        {
            get { return quickAccessToolbar; }
        }

        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if(layoutRoot!=null)list.Add(layoutRoot);
                if(ShowQuickAccessToolbarAboveRibbon)if (quickAccessToolbar != null) list.Add(quickAccessToolbar);
                return list.GetEnumerator();
            }
        }


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

        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (quickAccessToolbar != null) quickAccessToolbar.QuickAccessItems.Add(obj2 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (quickAccessToolbar != null) quickAccessToolbar.QuickAccessItems.Remove(obj3 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (quickAccessToolbar != null) quickAccessToolbar.QuickAccessItems.Remove(obj4 as QuickAccessMenuItem);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (quickAccessToolbar != null) quickAccessToolbar.QuickAccessItems.Add(obj5 as QuickAccessMenuItem);
                    }
                    break;
            }

        }

        #endregion

        #region Constructors

        static Ribbon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Ribbon), new FrameworkPropertyMetadata(typeof(Ribbon)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Ribbon()
        {
            keyTipService = new KeyTipService(this);
        }
                              
        #endregion        

        #region Overrides

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (tabControl != null) (tabControl.SelectedItem as RibbonTabItem).Focus();
        }

        public override void OnApplyTemplate()
        {
            if (layoutRoot!=null) RemoveLogicalChild(layoutRoot);
            layoutRoot = GetTemplateChild("PART_LayoutRoot") as Panel;
            if (layoutRoot != null) AddLogicalChild(layoutRoot);
            
            if(titleBar!=null)
            {
                titleBar.Items.Clear();
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
                tabControl.Items.Clear();
                tabControl.ToolbarItems.Clear();
                tabControl.PreviewMouseRightButtonUp -= OnTabControlRightButtonUp;
            }
            tabControl = GetTemplateChild("PART_RibbonTabControl") as RibbonTabControl;
            if (tabControl != null) tabControl.PreviewMouseRightButtonUp += OnTabControlRightButtonUp;
            if ((tabControl != null)&&(tabs!=null))
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabControl.Items.Add(tabs[i]);
                }
            }

            if ((tabControl != null) && (toolbarItems != null))
            {
                for (int i = 0; i < toolbarItems.Count; i++)
                {
                    tabControl.ToolbarItems.Add(toolbarItems[i]);
                }
            }

            if (quickAccessToolbar != null)
            {
                quickAccessToolbar.QuickAccessItems.Clear();
            }
            quickAccessToolbar = GetTemplateChild("PART_QuickAccessToolbar") as QuickAccessToolbar;
            if ((quickAccessToolbar != null) && (quickAccessItems != null))
            {
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    quickAccessToolbar.QuickAccessItems.Add(quickAccessItems[i]);
                }
            }
        }

        #endregion

        #region Event handling

        private void OnTabControlRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (quickAccessToolbar != null)
            {
                UIElement control = QuickAccessItemsProvider.PickQuickAccessItem((sender as RibbonTabControl),
                                                                                 e.GetPosition(
                                                                                     sender as RibbonTabControl));
                if (control != null)
                {
                    if (control is CheckBox)
                    {
                        (control as CheckBox).Width = 100;
                        (control as CheckBox).Height = 22;
                    }
                    if (quickAccessToolbar.Items.Contains(control)) quickAccessToolbar.Items.Remove(control);
                    else quickAccessToolbar.Items.Add(control);
                    quickAccessToolbar.InvalidateMeasure();
                }
            }
        }

        #endregion
    }
}
