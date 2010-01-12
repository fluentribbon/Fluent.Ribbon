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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents drop down button
    /// </summary>
    [ContentProperty("Items")]
    public class DropDownButton: RibbonControl
    {
        #region Fields

        // Context menu
        private ContextMenu contextMenu;

        // Collection of toolbar items
        private ObservableCollection<UIElement> items;

        private bool isInitializing;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets button large icon
        /// </summary>
        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(DropDownButton), new UIPropertyMetadata(null));

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
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(false,OnIsOpenChanged, CoerceIsOpen));

        // Coerce IsOpen
        private static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            if((d as DropDownButton).isInitializing) return true;
            return basevalue;
        }

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
                    if (contextMenu.RibbonPopup != null) list.Add(contextMenu.RibbonPopup);
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
                        if (contextMenu != null) contextMenu.Items.Add(obj2 as UIElement);
                        else AddLogicalChild(obj2);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (contextMenu != null) contextMenu.Items.Remove(obj3 as UIElement);
                        else RemoveLogicalChild(obj3);
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
            DependencyProperty.Register("MenuResizeMode", typeof(ContextMenuResizeMode), typeof(DropDownButton), new UIPropertyMetadata(ContextMenuResizeMode.None));


        #endregion

        #region Events

        public event EventHandler MenuOpened;
        public event EventHandler MenuClosed;

        #endregion


        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static DropDownButton()
        {
            StyleProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
                                                     new FrameworkPropertyMetadata(typeof(DropDownButton)));                       


        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = ThemesManager.DefaultDropDownButtonStyle;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DropDownButton()
        {            
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
            e.Handled = true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsOpen)
            {
                /*IsOpen = !IsOpen;                
                e.Handled = true;*/
            }
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
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton ribbon = (DropDownButton)d;

            if (ribbon.contextMenu == null)
            {
                ribbon.CreateMenu();
            }

            if (ribbon.IsOpen)
            {                
                ribbon.IsHitTestVisible = false;
            }
            else
            {
                ribbon.IsHitTestVisible = true;
            }
        }

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
            isInitializing = true;
            RibbonPopup parentPopup = FindParentPopup();
            if (parentPopup != null) parentPopup.IgnoreNextDeactivate = true;
            contextMenu = new ContextMenu();
            foreach (UIElement item in Items)
            {
                RemoveLogicalChild(item);
                contextMenu.Items.Add(item);
            }
            contextMenu.IsOpen = true;
            contextMenu.RibbonPopup.Opened += OnMenuOpened;
            contextMenu.RibbonPopup.Closed += OnMenuClosed;

            Binding binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.IsOpenProperty, binding);

            Binding resizeModeBinding = new Binding("MenuResizeMode");
            resizeModeBinding.Mode = BindingMode.OneWay;
            resizeModeBinding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.ResizeModeProperty, resizeModeBinding);

            contextMenu.PlacementTarget = this;
            contextMenu.Placement = PlacementMode.Bottom;            

            AddLogicalChild(contextMenu.RibbonPopup);
            isInitializing = false;
            IsOpen = true;
            Mouse.Capture(contextMenu.RibbonPopup);
            if (parentPopup != null)
            {
                parentPopup.IgnoreNextDeactivate = false;
                if (contextMenu.RibbonPopup.ParentPopup == null) contextMenu.RibbonPopup.ParentPopup = parentPopup;
            }
            contextMenu.IsOpen = true;
        }

        private void OnMenuClosed(object sender, EventArgs e)
        {            
            if (MenuClosed != null) MenuClosed(this, e);
        }

        private void OnMenuOpened(object sender, EventArgs e)
        {
            if (MenuOpened != null) MenuOpened(this, e);
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
            if (LargeIcon != null) Bind(button, button, "LargeIcon", Button.LargeIconProperty, BindingMode.OneWay);
            button.Click += delegate(object sender, RoutedEventArgs e) { RaiseEvent(e); };
            base.BindQuickAccessItem(element);
        }

        #endregion
    }


}
