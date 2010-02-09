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
using System.Windows.Data;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represents ribbon items control
    /// </summary>
    [ContentProperty("Items")]
    public class RibbonItemsControl:RibbonControl, IAddChild
    {
        #region Fields

        private ObservableCollection<DependencyObject> items;                      // Cache for Items property

        #endregion

        #region Properties

        #region DisplayMemberPath

        /// <summary>
        /// Gets or sets a path to a value on the source object to serve as the visual representation of the object. This is a dependency property. 
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisplayMemberPath.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region HasItems

        /// <summary>
        /// Gets a value that indicates whether the ItemsControl contains items. This is a dependency property. 
        /// </summary>
        public bool HasItems
        {
            get { return (bool)GetValue(HasItemsProperty); }
            private set { SetValue(HasItemsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasItems.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register("HasItems", typeof(bool), typeof(RibbonItemsControl), new UIPropertyMetadata(false));

        #endregion

        #region IsGrouping

        /// <summary>
        /// Gets a value that indicates whether the control is using grouping. This is a dependency property.
        /// </summary>
        public bool IsGrouping
        {
            get { return (bool)GetValue(IsGroupingProperty); }
            private set { SetValue(IsGroupingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsGrouping.  This enables animation, styling, binding, etc...
        /// </summary>

        public static readonly DependencyProperty IsGroupingProperty =
            DependencyProperty.Register("IsGrouping", typeof(bool), typeof(RibbonItemsControl), new UIPropertyMetadata(false));

        #endregion

        #region ItemBindingGroup

        /// <summary>
        /// Gets or sets the BindingGroup that is copied to each item in the ItemsControl.
        /// </summary>
        public BindingGroup ItemBindingGroup
        {
            get { return (BindingGroup)GetValue(ItemBindingGroupProperty); }
            set { SetValue(ItemBindingGroupProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemBindingGroup.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemBindingGroupProperty =
            DependencyProperty.Register("ItemBindingGroup", typeof(BindingGroup), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region ItemContainerGenerator

        /// <summary>
        /// Gets the ItemContainerGenerator that is associated with the control. 
        /// </summary>
        public ItemContainerGenerator ItemContainerGenerator
        {
            get { return (ItemContainerGenerator)GetValue(ItemContainerGeneratorProperty); }
            private set { SetValue(ItemContainerGeneratorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerGenerator.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerGeneratorProperty =
            DependencyProperty.Register("ItemContainerGenerator", typeof(ItemContainerGenerator), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region ItemContainerStyle

        /// <summary>
        /// Gets or sets the Style that is applied to the container element generated for each item. This is a dependency property. 
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region ItemContainerStyleSelector

        /// <summary>
        /// Gets or sets custom style-selection logic for a style that can be applied to each generated container element. This is a dependency property. 
        /// </summary>
        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValue(ItemContainerStyleSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemContainerStyleSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register("ItemContainerStyleSelector", typeof(StyleSelector), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region Items

        /// <summary> 
        ///     Items is the collection of data that is used to generate the content 
        ///     of this control.
        /// </summary> 
        [Bindable(true)]
        public ObservableCollection<DependencyObject> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ObservableCollection<DependencyObject>();
                    items.CollectionChanged += OnItemsCollectionChanged;
                }

                return items;
            }
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnItemsCollectionChanged(e);
        }
        protected virtual void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        AddLogicalChild(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        RemoveLogicalChild(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        RemoveLogicalChild(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        AddLogicalChild(obj5);
                    }
                    break;
            }
        }

        #endregion

        #region ItemsPanel

        /// <summary>
        /// Gets or sets the template that defines the panel that controls the layout of items. This is a dependency property. 
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(RibbonItemsControl), new UIPropertyMetadata(GetDefaultItemsPanelTemplate()));

        private static ItemsPanelTemplate GetDefaultItemsPanelTemplate()
        {
            ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
            template.Seal();
            return template;
        }

        #endregion

        #region ItemsSource

        /// <summary>
        /// Gets or sets a collection used to generate the content of the ItemsControl. This is a dependency property. 
        /// </summary>
        [Bindable(true)]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemsSource .  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(RibbonItemsControl), new UIPropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RibbonItemsControl).OnItemsSourceChanged(e);
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion

        #region ItemStringFormat

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the items in the ItemsControl if they are displayed as strings.
        /// </summary>
        public string ItemStringFormat
        {
            get { return (string)GetValue(ItemStringFormatProperty); }
            set { SetValue(ItemStringFormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemStringFormat.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemStringFormatProperty =
            DependencyProperty.Register("ItemStringFormat", typeof(string), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region ItemTemplate

        /// <summary>
        /// Gets or sets the DataTemplate used to display each item. This is a dependency property. 
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region ItemTemplateSelector

        /// <summary>
        /// Gets or sets the custom logic for choosing a template used to display each item. This is a dependency property. 
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemTemplateSelector.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(RibbonItemsControl), new UIPropertyMetadata(null));

        #endregion

        #region IsTextSearchEnabled

        /// <summary>
        /// Gets or sets a value that indicates whether TextSearch is enabled on the ItemsControl instance. This is a dependency property. 
        /// </summary>
        public bool IsTextSearchEnabled
        {
            get { return (bool)GetValue(IsTextSearchEnabledProperty); }
            set { SetValue(IsTextSearchEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsTextSearchEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsTextSearchEnabledProperty =
            DependencyProperty.Register("IsTextSearchEnabled", typeof(bool), typeof(RibbonItemsControl), new UIPropertyMetadata(false));

        #endregion        

        #region LogicalChildren

        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (ItemsSource == null) return Items.GetEnumerator();
                return ItemsSource.GetEnumerator();
            }
        }

        #endregion        

        #endregion

        public override UIElement CreateQuickAccessItem()
        {
            return null;
        }

        #region Implementation of IAddChild

        public void AddChild(object value)
        {
            Items.Add(value as DependencyObject);
        }

        public void AddText(string text)
        {
            GalleryItem item = new GalleryItem();
            item.Content = text;
            Items.Add(item);
        }

        #endregion
    }
}
