#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
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
        ContextMenu contextMenu;
        // Collection of toolbar items
        ObservableCollection<UIElement> items;

        // TODO: make these timers static
        DispatcherTimer focusTimer;
        DispatcherTimer unfocusTimer;

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
        /// Using a DependencyProperty as the backing store for IsChecked.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), 
            typeof(MenuItem), new UIPropertyMetadata(false,OnIsCheckedChanged));

        static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuItem menuItem = (MenuItem) d;
            if((bool)e.NewValue)
            {
                if (menuItem.Checked != null) menuItem.Checked(d, new RoutedEventArgs());
            }
            else
            {
                if (menuItem.Unchecked != null) menuItem.Unchecked(d, new RoutedEventArgs());
            }
        }

        // TODO: add more details what is IsCheckable

        /// <summary>
        /// Gets a value that indicates whether a MenuItem can be checked. This is a dependency property. 
        /// </summary>
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsCheckable.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), 
            typeof(MenuItem), new UIPropertyMetadata(false));
        
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
                        if (contextMenu != null)
                        {
                            contextMenu.Items.Add(item as UIElement);                            
                        }
                        else AddLogicalChild(item);
                        HasItems = true;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (contextMenu != null)
                        {
                            contextMenu.Items.Remove(item as UIElement);                            
                        }
                        else RemoveLogicalChild(item);
                        if (items.Count == 0) HasItems = false;
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Remove(item as UIElement);
                        else RemoveLogicalChild(item);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Add(item as UIElement);
                        else AddLogicalChild(item);
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
        /// Using a DependencyProperty as the backing store for HasItems.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasItemsProperty =
            DependencyProperty.Register("HasItems", typeof(bool), 
            typeof(MenuItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether context menu is opened
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
            DependencyProperty.Register("IsOpen", typeof(bool), 
            typeof(MenuItem), new UIPropertyMetadata(false, OnIsOpenChanged));

        // Handles IsOpen property changes
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)d;
            if(menuItem.contextMenu == null) menuItem.CreateMenu();
        }
        
        /// <summary>
        /// Gets or sets whether menu item is selected
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), 
            typeof(MenuItem), new UIPropertyMetadata(false));

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a menu item is checked
        /// </summary>
        public event RoutedEventHandler Checked;

        /// <summary>
        /// Occurs when a menu item is unchecked
        /// </summary>
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
            FocusVisualStyleProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(null));
            IsDefinitiveProperty.OverrideMetadata(typeof(MenuItem), new UIPropertyMetadata(true));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public MenuItem()
        {
           Unloaded += (s,e)=>IsSelected=false;
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
            
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && 
                ((position.Y >= 0.0) && (position.Y <= ActualHeight)) && 
                (e.ClickCount == 1))
            {
                RoutedEventArgs eventArgs = new RoutedEventArgs(ClickEvent, this);
                RaiseEvent(eventArgs);
                e.Handled = true;
                return;
            }
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled GotKeyboardFocus attached event reaches 
        /// an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The KeyboardFocusChangedEventArgs that contains the event data</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            FocusManager.SetFocusedElement(this.Parent, this);
            if (unfocusTimer != null) unfocusTimer.Stop();
        }

        /// <summary>
        /// Invoked when an unhandled MouseEnter attached event 
        /// is raised on this element. Implement this method 
        /// to add class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && 
                ((position.Y >= 0.0) && (position.Y <= ActualHeight)))
            {
                if (unfocusTimer != null) unfocusTimer.Stop();
                FocusManager.SetFocusedElement(this.Parent, this);
                if (focusTimer == null)
                {
                    focusTimer = new DispatcherTimer();
                    focusTimer.Interval = TimeSpan.FromSeconds(0.2);
                    focusTimer.Tick += OnFocusTimerTick;
                }
                focusTimer.Start();
            }
        }

        /// <summary>
        /// Invoked whenever an unhandled GotFocus
        /// event reaches this element in its route.
        /// </summary>
        /// <param name="e">The RoutedEventArgs that contains the event data</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (unfocusTimer != null) unfocusTimer.Stop();
            IsSelected = true;
            base.OnGotFocus(e);
        }


        /// <summary>
        /// Raises the LostFocus routed event by using 
        /// the event data that is provided. 
        /// </summary>
        /// <param name="e">A RoutedEventArgs that contains event data. 
        /// This event data must contain the identifier for the LostFocus event</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (focusTimer != null) focusTimer.Stop();
            if (IsOpen && HasItems)
            {
                if (unfocusTimer == null)
                {
                    unfocusTimer = new DispatcherTimer();
                    unfocusTimer.Interval = TimeSpan.FromSeconds(0.3);
                    unfocusTimer.Tick += OnUnFocusTimerTick;
                }
                unfocusTimer.Start();
            }
            IsSelected = false;            
        }


        /// <summary>
        /// Invoked when an unhandled MouseLeave attached event is raised on this element. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains the event data</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && 
                ((position.Y >= 0.0) && (position.Y <= ActualHeight)))
                return;
            if (focusTimer != null) focusTimer.Stop();
            if(!IsOpen) FocusManager.SetFocusedElement(this.Parent, null);
        }


        /// <summary>
        /// Invoked when an unhandled KeyDown attached event reaches an 
        /// element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The KeyEventArgs that contains the event data</param>
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
        /// Handles before click
        /// </summary>
        /// <param name="args"></param>
        protected override void OnBeforeClick(RoutedEventArgs args)
        {
            base.OnBeforeClick(args);            
            if (IsCheckable)
            {
                IsChecked = !IsChecked;
            }
            if (HasItems)
            {
                IsOpen = true;
            }
            else RibbonPopup.CloseAll();
        }


        /// <summary>
        /// Handles click
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClick(RoutedEventArgs args)
        {
            base.OnClick(args);
            ExecuteCommand();
            args.Handled = true;
        }


        void OnFocusTimerTick(object sender, EventArgs e)
        {
            focusTimer.Stop();

            if ((!IsOpen) && (HasItems))
            {
                IsOpen = true;
            }
            IsSelected = true;
            Keyboard.Focus(this);
        }

        void OnUnFocusTimerTick(object sender, EventArgs e)
        {
            unfocusTimer.Stop();
            IsSelected = false;
            if(IsOpen) IsOpen = false;
        }

        #endregion

        #region Private methods

        RibbonPopup FindParentPopup()
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
        void CreateMenu()
        {
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
            
            contextMenu.PlacementTarget = this;
            contextMenu.Placement = PlacementMode.Right;

            AddLogicalChild(contextMenu.RibbonPopup);
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
            Button button = (Button)element;
            button.Click += ((sender, e) => RaiseEvent(e));
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
