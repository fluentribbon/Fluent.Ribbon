using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents gallery control.
    /// </summary>
    [ContentProperty("Groups")]
    public class Gallery:RibbonControl
    {
        #region Fields        

        // Collection of toolbar items
        private ObservableCollection<GalleryGroup> groups;

        private Panel itemsPanel;

        private ObservableCollection<GalleryGroupFilter> filters;

        private DropDownButton filterButton;

        #endregion

        #region Properties

        #region ScrollBarsVisibility

        /// <summary> 
        /// HorizonalScollbarVisibility is a Windows.Controls.ScrollBarVisibility that
        /// determines if a horizontal scrollbar is shown. 
        /// </summary> 
        [Bindable(true), Category("Appearance")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(GalleryGroup), new UIPropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary> 
        /// VerticalScrollBarVisibility is a System.Windows.Controls.ScrollBarVisibility that 
        /// determines if a vertical scrollbar is shown.
        /// </summary> 
        [Bindable(true), Category("Appearance")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(GalleryGroup), new UIPropertyMetadata(ScrollBarVisibility.Visible));

        #endregion

        #region Groups

        /// <summary>
        /// Gets collection of groups
        /// </summary>
        /// <summary>
        /// Gets collection of groups
        /// </summary>
        public ObservableCollection<GalleryGroup> Groups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new ObservableCollection<GalleryGroup>();
                    this.groups.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupsCollectionChanged);
                }
                return this.groups;
            }
        }
        // Handle toolbar iitems changes
        private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (itemsPanel != null) itemsPanel.Children.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (itemsPanel != null) itemsPanel.Children.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (itemsPanel != null) itemsPanel.Children.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (itemsPanel != null) itemsPanel.Children.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        #endregion

        #region Filters

        /// <summary>
        /// Gets collection of filters
        /// </summary>
        /// <summary>
        /// Gets collection of filters
        /// </summary>
        public ObservableCollection<GalleryGroupFilter> Filters
        {
            get
            {
                if (this.filters == null)
                {
                    this.filters = new ObservableCollection<GalleryGroupFilter>();
                    this.filters.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnFilterCollectionChanged);
                }
                return this.filters;
            }
        }


        // Handle toolbar iitems changes
        private void OnFilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (filterButton != null) CreateMenuItem(obj2 as GalleryGroupFilter);
                    }
                    break;

                /*case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (filterButton != null) itemsPanel.Children.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (filterButton != null) itemsPanel.Children.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (filterButton != null) itemsPanel.Children.Add(obj5 as UIElement);
                    }
                    break;*/
            }

        }

        /// <summary>
        /// Gets or sets selected filter
        /// </summary>               
        public GalleryGroupFilter SelectedFilter
        {
            get { return (GalleryGroupFilter)GetValue(SelectedFilterProperty); }
            set { SetValue(SelectedFilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(GalleryGroupFilter), typeof(Gallery), new UIPropertyMetadata(null, OnFilterChanged));

        // Handles filter property changed
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null)
            {
                (d as Gallery).HasFilter = true;
            }
            else
            {
                (d as Gallery).HasFilter = false;
            }
            (d as Gallery).SetFilter(e.NewValue as GalleryGroupFilter);
        }

        /// <summary>
        /// Gets selected filter title
        /// </summary>
        public string SelectedFilterTitle
        {
            get { return (string)GetValue(SelectedFilterTitleProperty); }
            private set { SetValue(SelectedFilterTitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedFilterTitle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedFilterTitleProperty =
            DependencyProperty.Register("SelectedFilterTitle", typeof(string), typeof(Gallery), new UIPropertyMetadata(null));



        /// <summary>
        /// Gets whether gallery has selected filter
        /// </summary>       
        public bool HasFilter
        {
            get { return (bool)GetValue(HasFilterProperty); }
            private set { SetValue(HasFilterProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasFilter.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasFilterProperty =
            DependencyProperty.Register("HasFilter", typeof(bool), typeof(Gallery), new UIPropertyMetadata(false));

        #endregion

        #region SelectedIndex

        /// <summary>
        /// Gets or sets the index of the first item in the current selection or returns negative one (-1) if the selection is empty.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Gallery), new UIPropertyMetadata(-1));

        #endregion

        #region SelectedItem

        /// <summary>
        /// Gets or sets the first item in the current selection or returns null if the selection is empty 
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Gallery), new UIPropertyMetadata(null));

        #endregion

        #region LogicalChildren

        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (itemsPanel != null) return (new ArrayList() { itemsPanel }).GetEnumerator();
                else return groups.GetEnumerator();
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static Gallery()
        {
            //StyleProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            //if (basevalue == null) basevalue = ThemesManager.DefaultGalleryStyle;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Gallery()
        {
            
        }

        #endregion

        #region Overrides

        /// <summary>
        ///     This is the virtual that sub-classes must override if they wish to get
        ///     notified that the template tree has been created. 
        /// </summary>
        /// <remarks> 
        ///     This virtual is called after the template tree has been generated and it is invoked only 
        ///     if the call to ApplyTemplate actually caused the template tree to be generated.
        /// </remarks> 
        public override void OnApplyTemplate()
        {
           if(itemsPanel!=null)
           {
               for (int i = itemsPanel.Children.Count - 1; i >= 0; i--)
               {
                   itemsPanel.Children.Remove(itemsPanel.Children[i]);
               }
           }
           itemsPanel = GetTemplateChild("PART_ItemsPanel") as Panel;
           if((itemsPanel!=null)&&(groups!=null))
           {
               for (int i = 0; i < groups.Count; i++)
                   itemsPanel.Children.Add(groups[i]);
           }

            if(filterButton!=null)filterButton.Items.Clear();
            filterButton = GetTemplateChild("PART_DropDownButton") as DropDownButton;
            if((filterButton!=null)&&(filters!=null))
            {
                for(int i=0;i<filters.Count;i++)
                {
                    CreateMenuItem(filters[i]);
                }
            }
       }

        private void CreateMenuItem(GalleryGroupFilter filter)
        {
            MenuItem item = new MenuItem();
            item.Text = filter.Title;
            item.Tag = filter;
            item.Click += OnFilterMenuItemClick;
            filterButton.Items.Add(item);
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override UIElement CreateQuickAccessItem()
        {
            DropDownButton button = new DropDownButton();
            BindQuickAccessItem(button);
            return button;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            DropDownButton button = element as DropDownButton;
            button.Click += delegate(object sender, RoutedEventArgs e) { RaiseEvent(e); };
            base.BindQuickAccessItem(element);
        }

        #endregion

        #region Private Methods

        private void OnFilterMenuItemClick(object sender, RoutedEventArgs e)
        {
            SelectedFilter = (sender as MenuItem).Tag as GalleryGroupFilter;
        }

        // Set filter
        private void SetFilter(GalleryGroupFilter galleryGroupFilter)
        {            
            if(galleryGroupFilter==null)
            {
                SelectedFilterTitle = null;
                for (int i = 0; i < Groups.Count; i++)
                {
                    Groups[i].Visibility = Visibility.Visible;
                }
            }
            else
            {
                SelectedFilterTitle = galleryGroupFilter.Title;
                string[] groupsNames = galleryGroupFilter.Groups.Split(",".ToCharArray());
                for (int i = 0; i < Groups.Count; i++)
                {
                    if (groupsNames.Contains(Groups[i].Name)) Groups[i].Visibility = Visibility.Visible;
                    else Groups[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
