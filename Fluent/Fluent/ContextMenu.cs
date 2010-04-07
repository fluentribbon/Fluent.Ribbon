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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents context menu resize mode
    /// </summary>
    public enum ContextMenuResizeMode
    {
        /// <summary>
        /// Context menu can not be resized
        /// </summary>
        None=0,
        /// <summary>
        /// Context menu can be only resized vertically
        /// </summary>
        Vertical,
        /// <summary>
        /// Context menu can be resized vertically and horizontally
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
        RibbonPopup popup;
        // Initializing flag to prevent context menu closing while initializing
        bool isInInitializing;

        // Collection of context menu items
        ObservableCollection<UIElement> items;
        // Host for context menu items
        ContextMenuBar menuBar;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying ribbon popup
        /// </summary>
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
        /// Using a DependencyProperty as the backing store for ResizeMode. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode),
            typeof(ContextMenu), new UIPropertyMetadata(ContextMenuResizeMode.None));

        /// <summary>
        /// Gets collection of menu items
        /// </summary>
        public new ObservableCollection<UIElement> Items
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
        /// Gets host of context menu items
        /// </summary>
        public ContextMenuBar MenuBar
        {
            get { return menuBar; }
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
                        if (MenuBar != null) MenuBar.Items.Add(item as UIElement);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        if (MenuBar != null) MenuBar.Items.Remove(item as UIElement);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        if (MenuBar != null) MenuBar.Items.Remove(item as UIElement);
                    }
                    foreach (object item in e.NewItems)
                    {
                        if (MenuBar != null) MenuBar.Items.Add(item as UIElement);
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets or sets owner of contextmenu
        /// </summary>
        public object Owner
        {
            get { return GetValue(OwnerProperty); }
            set { SetValue(OwnerProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Owner.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OwnerProperty =
            DependencyProperty.Register("Owner", typeof(object),
            typeof(ContextMenu), new UIPropertyMetadata(null));
        
        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>]
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
            IsOpenProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged, CoerceIsOpen));
            FocusVisualStyleProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(null));            
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
            // Context menu closing
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

            // Keyboard navigation
            FrameworkElement control = Keyboard.FocusedElement as FrameworkElement;
            if (control != null)
            {
                if (e.Key == Key.Up)
                {
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                    e.Handled = true;
                    return;
                }
                if (e.Key == Key.Down)
                {
                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                    return;
                }

                // TODO: implement left / right arrows
            }
        }

        #endregion

        #region Private methods

        // Coerce IsOpen property
        static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            ContextMenu menu = (ContextMenu)d;
            if(menu.isInInitializing) return true;
            return basevalue;
        }

        // Handles IsOpen property changing
        static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
                }
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { menu.popup.Activate(); }));
            }
        }

        // Creates ribbon popup and removes original 
        // System.Windows.Controls.ContextMenu popup
        void HookupParentPopup()
        {            
            popup = new RibbonPopup();
            popup.AllowsTransparency = true;
            menuBar = new ContextMenuBar();            

            for (int i = 0; i < Items.Count;i++ )
            {
                MenuBar.Items.Add(items[i]);
            }

            Binding binding = new Binding("ResizeMode");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            MenuBar.SetBinding(ContextMenuBar.ResizeModeProperty, binding);
            MenuBar.ParentContextMenu = this;
            MenuBar.FlowDirection = FlowDirection;

            // Preventing ribbon popup closing
            popup.Child = MenuBar;
            popup.HorizontalAlignment = HorizontalAlignment.Left;
            popup.VerticalAlignment = VerticalAlignment.Top;
            popup.Opened += OnPopupFirstClose;

            popup.Opened += ((sender, args) => RaiseEvent(new RoutedEventArgs(OpenedEvent, this)));
            popup.Closed += ((sender, args) => RaiseEvent(new RoutedEventArgs(ClosedEvent, this)));
        
            // Set ribbon popup bindings
            CreatePopupRoot(this.popup, this);            
        }
        
        void OnPopupFirstClose(object sender, EventArgs e)
        {
            popup.Closed -= OnPopupFirstClose;
            // Find original popup
            Popup originalPopup = (Popup)(typeof(System.Windows.Controls.ContextMenu).
                GetField("_parentPopup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this));
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
