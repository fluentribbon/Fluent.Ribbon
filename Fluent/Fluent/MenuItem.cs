using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents menu item
    /// </summary>
    [ContentProperty("Items")]
    public class MenuItem: Button
    {
        #region Fields

        // Context menu
        private ContextMenu contextMenu;

        // Collection of toolbar items
        private ObservableCollection<UIElement> items;

        #endregion

        #region Properies

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (items != null) return items.GetEnumerator();
                else return (new ArrayList()).GetEnumerator();
            }
        }

        /// <summary>
        /// Gets collection of menu items
        /// </summary>
        public ObservableCollection<UIElement> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.items = new ObservableCollection<UIElement>();
                    this.items.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnToolbarItemsCollectionChanged);
                }
                return this.items;
            }
        }

        /// <summary>
        /// handles colection of menu items changes
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
                        if (contextMenu != null)
                        {
                            contextMenu.Items.Add(obj2 as UIElement);                            
                        }
                        HasItems = true;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (contextMenu != null)
                        {
                            contextMenu.Items.Remove(obj3 as UIElement);                            
                        }
                        if (items.Count == 0) HasItems = false;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Add(obj5 as UIElement);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets whether context menu has items
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
            DependencyProperty.Register("HasItems", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether context menu is opened
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(false,OnIsOpenChanged));

        // Handles IsOpen property changes
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuItem menu = d as MenuItem;
            if(menu!=null)
            {
                if(menu.contextMenu==null) menu.CreateMenu();
            }
        }

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode MenuResizeMode
        {
            get { return (ContextMenuResizeMode)GetValue(MenuResizeModeProperty); }
            set { SetValue(MenuResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuResizeModeProperty =
            DependencyProperty.Register("MenuResizeMode", typeof(ContextMenuResizeMode), typeof(MenuItem), new UIPropertyMetadata(ContextMenuResizeMode.None));


        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuItem()
        {

        }

        #endregion

        #region Overrides

        #endregion

        #region Event Handling

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="e">The event data</param>
        protected override void OnClick(RoutedEventArgs e)
        {
            base.OnClick(e);
            if(HasItems)
            {
                IsOpen = true;
                e.Handled = true;
            }
            /*else
            {
                RibbonPopup popup = FindParentPopup();
                if (popup != null) popup.IsOpen = false;    
            }*/            
        }

        #endregion

        #region Private methods

        private RibbonPopup FindParentPopup()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                RibbonPopup ribbonPopup = element as RibbonPopup;
                if (ribbonPopup != null) return ribbonPopup;
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element);
                if (parent != null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element);
            }
            return null;
        }

        // Creates context menu
        private void CreateMenu()
        {
            RibbonPopup parentPopup= FindParentPopup();
            if(parentPopup!=null) parentPopup.IgnoreNextDeactivate = true;
            contextMenu = new ContextMenu();
            foreach (UIElement item in Items)
            {
                contextMenu.Items.Add(item);
            }
            contextMenu.IsOpen = true;
            Binding binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.IsOpenProperty, binding);
            
            Binding resizeModeBinding = new Binding("MenuResizeMode");
            resizeModeBinding.Mode = BindingMode.OneWay;
            resizeModeBinding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.ResizeModeProperty, resizeModeBinding);

            contextMenu.PlacementTarget = this;
            contextMenu.Placement = PlacementMode.Right;

            AddLogicalChild(contextMenu.RibbonPopup);
            if (parentPopup != null)
            {
                parentPopup.IgnoreNextDeactivate = false;
                if (contextMenu.RibbonPopup.ParentPopup == null) contextMenu.RibbonPopup.ParentPopup = parentPopup;
            }
        }

        #endregion
    }
}
