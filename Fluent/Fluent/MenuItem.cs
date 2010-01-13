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
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents menu item
    /// </summary>
    [ContentProperty("Items")]
    public class MenuItem: RibbonControl
    {
        #region Fields

        // Context menu
        private ContextMenu contextMenu;

        // Collection of toolbar items
        private ObservableCollection<UIElement> items;

        private DispatcherTimer focusTimer;

        #endregion

        #region Properies

        /// <summary>
        /// Gets or sets whether MenuItem is checked
        /// </summary>
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(false,OnIsCheckedChanged));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                if((d as MenuItem).Checked!=null)(d as MenuItem).Checked(d,new RoutedEventArgs());
            }
            else
            {
                if ((d as MenuItem).Unchecked != null) (d as MenuItem).Unchecked(d, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// Gets or sets whether MenuItem can auto check
        /// </summary>
        public bool CanAutoCheck
        {
            get { return (bool)GetValue(CanAutoCheckProperty); }
            set { SetValue(CanAutoCheckProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAutoCheck.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAutoCheckProperty =
            DependencyProperty.Register("CanAutoCheck", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(false));



        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (contextMenu != null)
                {
                    ArrayList list = new ArrayList();
                    if (contextMenu.MenuBar != null) list.Add(contextMenu.MenuBar);
                    else list.Add(contextMenu);
                    return list.GetEnumerator();
                }
                else
                {
                    if (items != null) return items.GetEnumerator();
                    else return (new ArrayList()).GetEnumerator();
                }
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
                        else AddLogicalChild(obj2);
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
                        else RemoveLogicalChild(obj3);
                        if (items.Count == 0) HasItems = false;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Remove(obj4 as UIElement);
                        else RemoveLogicalChild(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Add(obj5 as UIElement);
                        else AddLogicalChild(obj5);
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

        #region Events

        public event RoutedEventHandler Checked;
        public event RoutedEventHandler Unchecked;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static MenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(typeof(MenuItem)));
            FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(null));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuItem()
        {
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonUp routed event that occurs 
        /// when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            //if (Mouse.Captured == this) Mouse.Capture(null);
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && ((position.Y >= 0.0) && (position.Y <= ActualHeight)) && (e.ClickCount == 1))
            {
                RoutedEventArgs ee = new RoutedEventArgs(RibbonControl.ClickEvent, this);
                RaiseEvent(ee);
                e.Handled = true;
                return;
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            Keyboard.Focus(this.Parent as IInputElement);
            FocusManager.SetFocusedElement(this.Parent, this);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            FocusManager.SetFocusedElement(this.Parent, this);
            if(focusTimer==null)
            {
                focusTimer = new DispatcherTimer();
                focusTimer.Interval = TimeSpan.FromSeconds(0.5);
                focusTimer.Tick += OnFocusTimerTick;
            }
            focusTimer.Start();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if(focusTimer!=null)focusTimer.Stop();
            if (IsOpen) IsOpen = false;
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (focusTimer != null) focusTimer.Stop();
            if(!IsOpen) FocusManager.SetFocusedElement(this.Parent, null);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if(HasItems)
            {
                if (e.Key == Key.Right)
                {
                    RaiseEvent(new RoutedEventArgs(ClickEvent, this));
                    e.Handled = true;
                    return;
                }
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="e">The event data</param>
        protected virtual void OnClick(RoutedEventArgs e)
        {
            ExecuteCommand();            
            if(CanAutoCheck)
            {
                IsChecked = !IsChecked;
            }
            if (HasItems)
            {
                RibbonPopup parentPopup = null;
                if (Parent is ContextMenu)
                {
                    parentPopup = (Parent as ContextMenu).RibbonPopup;
                }
                //if (parentPopup != null) parentPopup.IgnoreNextDeactivate = true;
                IsOpen = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            OnClick(e);
        }

        private void OnFocusTimerTick(object sender, EventArgs e)
        {
            if (IsFocused)
            {
                if ((!IsOpen) && (HasItems))
                {
                    RibbonPopup parentPopup = null;
                    if (Parent is ContextMenu)
                    {
                        parentPopup = (Parent as ContextMenu).RibbonPopup;
                    }
                    //if (parentPopup != null) parentPopup.IgnoreNextDeactivate = true;
                    IsOpen = true;
                }                
            }
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
            //if(parentPopup!=null) parentPopup.IgnoreNextDeactivate = true;
            contextMenu = new ContextMenu();
            foreach (UIElement item in Items)
            {
                RemoveLogicalChild(item);
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
                //parentPopup.IgnoreNextDeactivate = false;
                //if (contextMenu.RibbonPopup.ParentPopup == null) contextMenu.RibbonPopup.ParentPopup = parentPopup;
            }
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
            Button button = new Button();
            BindQuickAccessItem(button);
            return button;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            Button button = element as Button;
            button.Click += delegate(object sender, RoutedEventArgs e) { RaiseEvent(e); };
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
