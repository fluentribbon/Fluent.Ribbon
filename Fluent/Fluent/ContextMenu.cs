#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents context menu resize mode
    /// </summary>
    public enum ContextMenuResizeMode
    {
        /// <summary>
        /// Contet menu can`t resize
        /// </summary>
        None=0,
        /// <summary>
        /// Context menu can only resize vertical
        /// </summary>
        Vertical,
        /// <summary>
        /// Context menu can only vertical and horizontal
        /// </summary>
        Both
    }
    /// <summary>
    /// Represents a pop-up menu that enables a control 
    /// to expose functionality that is specific to the context of the control
    /// </summary>
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        #region Fields

        // Popup for displaing context menu
        private RibbonPopup popup;
        // Initializing flag to prevent context menu closing while initializing
        private bool isInInitializing;

        // Collection of toolbar items
        private ObservableCollection<UIElement> items;

        private ContextMenuBar menuBar;

        #endregion

        #region Properties

        internal RibbonPopup RibbonPopup
        {
            get
            {
                if (popup == null)
                {
                    isInInitializing = true;
                    HookupParentPopup();                    
                    isInInitializing = false;
                    Mouse.Capture(null);
                    popup.Activate();
                }
                return popup;
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
        /// Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode), typeof(ContextMenu), new UIPropertyMetadata(ContextMenuResizeMode.None));

        /// <summary>
        /// Gets collection of menu items
        /// </summary>
        public new ObservableCollection<UIElement> Items
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

        public ContextMenuBar MenuBar
        {
            get { return menuBar; }
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
                        if (MenuBar != null) MenuBar.Items.Add(obj2 as UIElement);
                        else AddLogicalChild(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (MenuBar != null) MenuBar.Items.Remove(obj3 as UIElement);
                        else RemoveLogicalChild(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (MenuBar != null) MenuBar.Items.Remove(obj4 as UIElement);
                        else RemoveLogicalChild(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (MenuBar != null) MenuBar.Items.Add(obj5 as UIElement);
                        else AddLogicalChild(obj5);
                    }
                    break;
            }

        }

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>]
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ContextMenu()
        {
            //StyleProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
            IsOpenProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged, CoerceIsOpen));
            FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(null));            
        }

        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = (d as FrameworkElement).Resources["EmptyContextMenuStyle"] as Style;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContextMenu()
        {
            KeyboardNavigation.SetDirectionalNavigation(this, KeyboardNavigationMode.Cycle);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is FrameworkElement);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.MouseLeftButtonUp routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsOpen = false;
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyDown attached event 
        /// reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                IsOpen = false;
                return;
            }
            if ((e.Key == Key.System) &&
                ((e.SystemKey == Key.LeftAlt) || (e.SystemKey == Key.RightAlt) || (e.SystemKey == Key.F10)))
            {
                if (e.SystemKey != Key.F10)
                {
                    IsOpen = false;
                }
                else e.Handled = true;
                return;
            }
            if (e.Key == Key.Up)
            {
                /*if (Keyboard.FocusedElement == this)
                {
                    if (Items.Count > 0)
                    {
                        FocusManager.SetFocusedElement(this, Items[Items.Count - 1] as IInputElement);
                        //Keyboard.Focus(Items[Items.Count - 1] as IInputElement);
                    }
                }
                else*/
                    (Keyboard.FocusedElement as FrameworkElement).MoveFocus(
                        new TraversalRequest(FocusNavigationDirection.Previous));
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Down)
            {
                /*if (Keyboard.FocusedElement == this)
                {
                    if (Items.Count > 0)
                    {
                        FocusManager.SetFocusedElement(this, Items[0] as IInputElement);
                        //Keyboard.Focus(Items[0] as IInputElement);
                    }
                }
                else*/
                    (Keyboard.FocusedElement as FrameworkElement).MoveFocus(
                        new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
                return;
            }
            if(e.Key==Key.Left)
            {
                /*if(popup.ParentPopup!=null)
                {
                    IsOpen = false;
                    popup.ParentPopup.Activate();
                    Keyboard.Focus(popup.ParentPopup.Child);
                    e.Handled = true;
                    return;
                }*/
            }
        }

        #endregion

        #region Private methods

        // Coerce IsOpen property
        private static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            ContextMenu menu = (ContextMenu)d;
            if(menu.isInInitializing) return true;
            return basevalue;
        }
        // Handles IsOpen property changing
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)d;
            if ((bool)e.NewValue)
            {

                if (menu.popup == null)
                {
                    // Creates new ribbon popup and prevents it`s closing
                    menu.isInInitializing = true;
                    menu.HookupParentPopup();
                    menu.isInInitializing = false;
                    /*Mouse.Capture(null);
                    menu.popup.Activate();*/
                }
                //Keyboard.Focus(menu.MenuBar);
            }


            //else FocusManager.SetFocusedElement(menu,null);
            //TODO: Strange behavior of command when our context menu is opened
        }

        // Creates ribbon popup and removes original System.Windows.Controls.ContextMenu popup
        private void HookupParentPopup()
        {            
            popup = new RibbonPopup();
            this.popup.AllowsTransparency = true;
                
            menuBar = new ContextMenuBar();
            for (int i = 0; i < Items.Count;i++ )
            {
                RemoveLogicalChild(items[i]);
                MenuBar.Items.Add(items[i]);
            }

            Binding binding = new Binding("ResizeMode");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            MenuBar.SetBinding(ContextMenuBar.ResizeModeProperty, binding);

            // Preventing ribbon popup closing
            popup.Child = MenuBar;
            popup.Opened += OnPopupFirstClose;

            popup.Opened += delegate { RaiseEvent(new RoutedEventArgs(OpenedEvent,this)); };
            popup.Closed += delegate { RaiseEvent(new RoutedEventArgs(ClosedEvent, this)); };
        
            // Set ribbon popup bindings
            CreatePopupRoot(this.popup, this);            
        }
        private void OnPopupFirstClose(object sender, EventArgs e)
        {
            popup.Closed -= OnPopupFirstClose;
            // Find original popup
            Popup originalPopup = (typeof(System.Windows.Controls.ContextMenu).
                GetField("_parentPopup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as Popup);
            originalPopup.IsOpen = false;
        }

        // Set popup bindings
        static void CreatePopupRoot(RibbonPopup popup, UIElement child)
        {
            Binding binding = new Binding("PlacementTarget");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.PlacementTargetProperty, binding);
            binding = new Binding("VerticalOffset");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.VerticalOffsetProperty, binding);
            binding = new Binding("HorizontalOffset");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.HorizontalOffsetProperty, binding);
            binding = new Binding("PlacementRectangle");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.PlacementRectangleProperty, binding);
            binding = new Binding("Placement");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.PlacementProperty, binding);
            binding = new Binding("StaysOpen");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.StaysOpenProperty, binding);
            binding = new Binding("CustomPopupPlacementCallback");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.CustomPopupPlacementCallbackProperty, binding);
            binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = child;
            popup.SetBinding(Popup.IsOpenProperty, binding);
        }

        #endregion
    }
}
