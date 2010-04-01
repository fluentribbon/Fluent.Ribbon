#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents drop down button
    /// </summary>
    [ContentProperty("Items")]
    public class DropDownButton : RibbonControl
    {
        #region Fields

        // Internal context menu of the button
        ContextMenu dropDownMenu;

        // Collection of cotext menu items
        ObservableCollection<UIElement> items;
        
        // QAT clone of this button
        DropDownButton quickAccessButton;
        // Prevents menu closing while initializing 
        bool isInitializing;

        private bool isMenuCreated = false;

        #endregion

        #region Properties

        /// <summary>
        /// Button drop down menu
        /// </summary>
        public ContextMenu DropDownMenu
        {
            get 
            { 
                if(dropDownMenu==null)
                {
                    dropDownMenu = new ContextMenu();
                    dropDownMenu.Opened += OnFirstMenuOpened;
                }
                return dropDownMenu;
            } 
        }

        private void OnFirstMenuOpened(object sender, RoutedEventArgs e)
        {
            dropDownMenu.Opened -= OnFirstMenuOpened;
            if (!isMenuCreated) CreateMenu();
        }

        /// <summary>
        /// Gets or sets button large icon
        /// </summary>
        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), 
            typeof(DropDownButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets whether button has triangle
        /// </summary>
        public bool HasTriangle
        {
            get { return (bool) GetValue(HasTriangleProperty); }
            set { SetValue(HasTriangleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasTriangle. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasTriangleProperty =
            DependencyProperty.Register(
                "HasTriangle", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownButton), 
            new UIPropertyMetadata(false,OnIsOpenChanged, CoerceIsOpen));

        // Coerce IsOpen
        static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            if(((DropDownButton)d).isInitializing) return true;
            return basevalue;
        }

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (items != null) return items.GetEnumerator();
                return (new ArrayList()).GetEnumerator();
            }
        }
        
        /// <summary>
        /// Gets collection of menu items
        /// </summary>
        public ObservableCollection<UIElement> Items
        {
            get
            {
                if (items == null)
                {
                    items = new ObservableCollection<UIElement>();
                    items.CollectionChanged += OnToolbarItemsCollectionChanged;
                }
                return items;
            }
        }

        /// <summary>
        /// Handles colection of menu items changes
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnToolbarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object item in e.NewItems)
                    {
                        if (dropDownMenu != null) dropDownMenu.Items.Add(item as UIElement);
                        else AddLogicalChild(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (dropDownMenu != null) dropDownMenu.Items.Remove(item as UIElement);
                        else RemoveLogicalChild(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (dropDownMenu != null) dropDownMenu.Items.Remove(item as UIElement);
                        else RemoveLogicalChild(item);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (dropDownMenu != null) dropDownMenu.Items.Add(item as UIElement);
                        else AddLogicalChild(item);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode), 
            typeof(DropDownButton), new UIPropertyMetadata(ContextMenuResizeMode.None));


        #endregion

        #region Events

        /// <summary>
        /// Occures when context menu is opened
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Occures when context menu is closed
        /// </summary>
        public event EventHandler Closed;

        #endregion
        
        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));                       
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DropDownButton()
        {            
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
            e.Handled = true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent,this));
            e.Handled = true;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Handles IsOpen property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton ribbon = (DropDownButton)d;

            if (!ribbon.isMenuCreated) ribbon.CreateMenu();
            ribbon.IsHitTestVisible = !ribbon.IsOpen;
        }

        internal void DoCreateMenu()
        {
            if (!isMenuCreated)
            {
                CreateMenu();
                IsOpen = false;
            }
        }

        // Creates context menu
        void CreateMenu()
        {
            isInitializing = true;
            isMenuCreated = true;
            if (dropDownMenu==null) dropDownMenu = new ContextMenu();            
            foreach (UIElement item in Items)
            {
                RemoveLogicalChild(item);
                dropDownMenu.Items.Add(item);
            }          
            AddLogicalChild(dropDownMenu.RibbonPopup);
            dropDownMenu.IsOpen = true;
            dropDownMenu.RibbonPopup.Opened += OnMenuOpened;
            dropDownMenu.RibbonPopup.Closed += OnMenuClosed;

            Binding binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            dropDownMenu.SetBinding(Fluent.ContextMenu.IsOpenProperty, binding);

            Binding resizeModeBinding = new Binding("ResizeMode");
            resizeModeBinding.Mode = BindingMode.OneWay;
            resizeModeBinding.Source = this;
            dropDownMenu.SetBinding(Fluent.ContextMenu.ResizeModeProperty, resizeModeBinding);

            dropDownMenu.PlacementTarget = this;
            dropDownMenu.Placement = PlacementMode.Bottom;            
            
            isInitializing = false;
            Mouse.Capture(null);
            IsOpen = true;
            dropDownMenu.IsOpen = true;
        }

        void OnMenuClosed(object sender, EventArgs e)
        {            
            if (Closed != null) Closed(this, e);
        }

        void OnMenuOpened(object sender, EventArgs e)
        {
            if (Opened != null) Opened(this, e);
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            DropDownButton button = new DropDownButton();
            button.Size = RibbonControlSize.Small;
            BindQuickAccessItem(button);
            button.PreviewMouseLeftButtonDown += OnQuickAccessClick;
            return button;
        }

        void OnQuickAccessClick(object sender, MouseButtonEventArgs e)
        {
            DropDownButton button = (DropDownButton)sender;
            for(int i=0;i<Items.Count;i++)
            {
                UIElement item = Items[0];
                Items.Remove(item);
                button.Items.Add(item);
                i--;
            }            
            button.Closed += OnQuickAccessMenuClosed;
            quickAccessButton = button;
        }

        void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            quickAccessButton.Closed -= OnQuickAccessMenuClosed;
            for (int i = 0; i < quickAccessButton.Items.Count; i++)
            {
                UIElement item = quickAccessButton.Items[0];
                quickAccessButton.Items.Remove(item);
                Items.Add(item);
                i--;
            }
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            Bind(this, element, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            base.BindQuickAccessItem(element);
        }

        #endregion
    }


}
