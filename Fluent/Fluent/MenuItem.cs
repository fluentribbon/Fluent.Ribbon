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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
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
    public class MenuItem : System.Windows.Controls.MenuItem, IQuickAccessItemProvider, IRibbonControl
    {
        #region Fields

        private Popup popup;

        private bool ignoreNextMouseLeave;

        #endregion

        #region Properties

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup
        {
            get { return popup; }
        }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(MenuItem));

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.SizeDefinitionProperty.AddOwner(typeof(MenuItem));

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        #endregion

        #region IsDropDownOpen

        /// <summary>
        /// Gets or sets whether popup is opened
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return IsSubmenuOpen; }
            set { IsSubmenuOpen = value; }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static MenuItem()
        {
            Type type = typeof (MenuItem);
            ToolTipService.Attach(type);
            //PopupService.Attach(type);            
            ContextMenuService.Attach(type);
        }
        
        /// <summary>
        /// Default Constructor
        /// </summary>
        public MenuItem()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region QuickAccess

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public FrameworkElement CreateQuickAccessItem()
        {
            Button button = new Button();
            RibbonControl.BindQuickAccessItem(this, button);
            return button;
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddToQuickAccessToolBarProperty); }
            set { SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(MenuItem));

        #endregion

        #region Methods

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent));
        }

        #endregion

        #region Protected

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        protected virtual void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
        }

        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            popup = GetTemplateChild("PART_Popup") as Popup;
        }

        /// <summary>
        /// Called when the submenu of a <see cref="T:System.Windows.Controls.MenuItem"/> is opened. 
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.MenuItem.SubmenuOpened"/> event.</param>
       /* protected override void OnSubmenuOpened(RoutedEventArgs e)
        {
            ignoreNextMouseLeave = true;
            //base.OnSubmenuOpened(e);
            // Mouse.Capture(GetRootDropDownControl() as IInputElement, CaptureMode.SubTree);
        }
        */
        /// <summary>
        /// Called when the submenu of a <see cref="T:System.Windows.Controls.MenuItem"/> is closed. 
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.MenuItem.SubmenuClosed"/> event.</param>
        /*protected override void OnSubmenuClosed(RoutedEventArgs e)
        {            
            //base.OnSubmenuClosed(e);
            //if (Mouse.Captured == GetRootDropDownControl()) Mouse.Capture(null);
        }*/
/*
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            e.Handled = true;
        }*/

       /* protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsKeyboardFocusWithin && !IsHighlighted)
            {
                IsHighlighted = true;
            }
        }*/
        /*
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            e.Handled = true;
        }*/
        /*
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            FocusOrSelect();
            if (HasItems && !IsCheckable) IsSubmenuOpen = true;
            UpdateIsPressed();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if (ignoreNextMouseLeave)
            {
                ignoreNextMouseLeave = false;
            }
            else if (!IsSubmenuOpen)
            {
                base.IsHighlighted = false;
                if (IsKeyboardFocusWithin)
                {
                    ItemsControl control = ItemsControlFromItemContainer(this);
                    if (control != null)
                    {
                        control.Focus();
                    }
                }
            }
            UpdateIsPressed();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsSubmenuOpen)
            {
                UpdateIsPressed();
                e.Handled = true;
            }
            base.OnMouseLeftButtonDown(e);

        }

        private void FocusOrSelect()
        {
            if (!IsKeyboardFocusWithin)
            {
                Focus();
            }
            if (!IsHighlighted)
            {
                IsHighlighted = true;
            }
        }

        private void UpdateIsPressed()
        {
            Rect rect = new Rect(new Point(), base.RenderSize);
            if (((Mouse.LeftButton == MouseButtonState.Pressed) && base.IsMouseOver) && rect.Contains(Mouse.GetPosition(this)))
            {
                base.IsPressed = true;
            }
            else
            {
                base.IsPressed = false;
            }
        }*/

        #endregion

        #region Private Methods

        private IDropDownControl GetRootDropDownControl()
        {
            DependencyObject element = this;
            while (element != null)
            {
                IDropDownControl popup = element as IDropDownControl;
                if (popup != null) return popup;
                DependencyObject elementParent = VisualTreeHelper.GetParent(element);
                if (elementParent == null) element = LogicalTreeHelper.GetParent(element);
                else element = elementParent;
            }
            return null;
        }

        #endregion
    }
}
