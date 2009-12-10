using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    [ContentProperty("Tabs")]
    [TemplatePart(Name = "PART_BackstageButton", Type = typeof(BackstageButton))]
    public class Ribbon: Control
    {
        #region Fields

        private ObservableCollection<RibbonContextualTabGroup> groups = null;
        private ObservableCollection<RibbonTabItem> tabs = null;
        private ObservableCollection<UIElement> toolbarItems = null;
        private ObservableCollection<UIElement> backstageItems = null;
        private RibbonTitleBar titleBar = null;
        private RibbonTabControl tabControl = null;
        private QuickAccessToolbar quickAccessToolbar = null;

        private Panel layoutRoot = null;

        private BackstageButton backstageButton = null;

        // Handles F10, Alt and so on
        KeyTipService keyTipService = null;

        private ObservableCollection<QuickAccessMenuItem> quickAccessItems = null;

        private BackstageAdorner adorner = null;

        private RibbonTabItem savedTabItem = null;

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
            DependencyProperty.Register("BackstageKeyTipKeys", typeof(string), typeof(Ribbon), new UIPropertyMetadata(""));



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

        public bool IsBackstageOpen
        {
            get { return (bool)GetValue(IsBackstageOpenProperty); }
            set { SetValue(IsBackstageOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBackstageOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBackstageOpenProperty =
            DependencyProperty.Register("IsBackstageOpen", typeof(bool), typeof(Ribbon), new UIPropertyMetadata(false, OnIsBackstageOpenChanged));

        private static void OnIsBackstageOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                (d as Ribbon).ShowBackstage();
            }
            else
            {
                (d as Ribbon).HideBackstage();
            }
        }

        public Brush BackstageBrush
        {
            get { return (Brush)GetValue(BackstageBrushProperty); }
            set { SetValue(BackstageBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BackstageBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackstageBrushProperty =
            DependencyProperty.Register("BackstageBrush", typeof(Brush), typeof(Ribbon), new UIPropertyMetadata(Brushes.Blue));

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
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Contained);
            Focusable = false;
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
                tabControl.SelectionChanged -= OnTabControlSelectionChanged;
            }
            tabControl = GetTemplateChild("PART_RibbonTabControl") as RibbonTabControl;
            if (tabControl != null)
            {
                tabControl.PreviewMouseRightButtonUp += OnTabControlRightButtonUp;
                tabControl.SelectionChanged += OnTabControlSelectionChanged;
            }
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
            if (backstageButton != null)
            {
                backstageButton.Backstage.Items.Clear();
            }
            backstageButton = GetTemplateChild("PART_BackstageButton") as BackstageButton;
            if (backstageButton != null) 
            {
                Binding binding = new Binding("IsBackstageOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                backstageButton.SetBinding(BackstageButton.IsOpenProperty, binding);

                for (int i = 0; i < backstageItems.Count; i++)
                {
                    backstageButton.Backstage.Items.Add(backstageItems[i]);
                }
            }

        }

        private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count>0)
            {
                if(IsBackstageOpen)
                {
                    savedTabItem = e.AddedItems[0] as RibbonTabItem;
                    IsBackstageOpen = false;
                }
            }
        }

        #endregion

        #region Event handling

        private void OnBackstageButtonClick(object sender, RoutedEventArgs e)
        {
            IsBackstageOpen = !IsBackstageOpen;
        }

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

        #region Private methods

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
            if(quickAccessToolbar!=null) quickAccessToolbar.IsEnabled = false;
            if(titleBar!=null) titleBar.IsEnabled = false;
            Window.GetWindow(this).PreviewKeyDown += OnBackstageEscapeKeyDown;
        }        

        private void HideBackstage()
        {
            AdornerLayer layer = GetAdornerLayer(this);
            layer.Remove(adorner);
            if (tabControl != null) tabControl.SelectedItem = savedTabItem;
            if (quickAccessToolbar != null) quickAccessToolbar.IsEnabled = true;
            if (titleBar != null) titleBar.IsEnabled = true;
            Window.GetWindow(this).PreviewKeyDown -= OnBackstageEscapeKeyDown;
        }

        private void OnBackstageEscapeKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Escape)IsBackstageOpen = false;
        }

        #endregion

        #region Static Methods

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
