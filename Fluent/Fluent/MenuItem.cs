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

        // Thumb to resize in both directions
        Thumb resizeBothThumb;
        // Thumb to resize vertical
        Thumb resizeVerticalThumb;

        private MenuPanel menuPanel;

        private IInputElement focusedElement;

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

        #region IsDefinitive

        /// <summary>
        /// Gets or sets whether ribbon control click must close backstage
        /// </summary>
        public bool IsDefinitive
        {
            get { return (bool)GetValue(IsDefinitiveProperty); }
            set { SetValue(IsDefinitiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register("IsDefinitive", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(true));

        #endregion

        #region ResizeMode

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
            typeof(MenuItem), new UIPropertyMetadata(ContextMenuResizeMode.None));


        #endregion

        #region MaxDropDownHeight

        /// <summary>
        /// Get or sets max height of drop down popup
        /// </summary>
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxDropDownHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(MenuItem), new UIPropertyMetadata(100.0));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when context menu is opened
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when context menu is closed
        /// </summary>
        public event EventHandler DropDownClosed;

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
            ToolTip = new ToolTip();
            (ToolTip as ToolTip).Template = null;
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

        /// <summary>
        /// Gets or sets style of element on quick access toolbar
        /// </summary>
        public Style QuickAccessElementStyle
        {
            get { return (Style)GetValue(QuickAccessElementStyleProperty); }
            set { SetValue(QuickAccessElementStyleProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for QuickAccessElementStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessElementStyleProperty = RibbonControl.QuickAccessElementStyleProperty.AddOwner(typeof(MenuItem));

        #endregion

        #region Public

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            if (!HasItems) RaiseEvent(new RoutedEventArgs(ClickEvent));
            else IsDropDownOpen = true;
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
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is FrameworkElement);
        }

        /// <summary>
        /// Called when a <see cref="T:System.Windows.Controls.Button"/> is clicked. 
        /// </summary>
        protected override void OnClick()
        {
            // Close popup on click
            if ((IsDefinitive)&&(!HasItems)) PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
            base.OnClick();
        }

        /// <summary>
        /// Called when the template's tree is generated.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (popup != null)
            {
                popup.Opened -= OnDropDownOpened;
                popup.Closed -= OnDropDownClosed;
            }

            popup = GetTemplateChild("PART_Popup") as Popup;

            if (popup != null)
            {
                popup.Opened += OnDropDownOpened;
                popup.Closed += OnDropDownClosed;

                KeyboardNavigation.SetControlTabNavigation(popup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetDirectionalNavigation(popup, KeyboardNavigationMode.Cycle);
                KeyboardNavigation.SetTabNavigation(popup, KeyboardNavigationMode.Cycle);
            }

            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta -= OnResizeVerticalDelta;
            }
            resizeVerticalThumb = GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta += OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta -= OnResizeBothDelta;
            }
            resizeBothThumb = GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta += OnResizeBothDelta;
            }

            menuPanel = GetTemplateChild("PART_MenuPanel") as MenuPanel;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        protected virtual void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
        }

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(menuPanel.Width)) menuPanel.Width = menuPanel.ActualWidth;
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Width = Math.Max(menuPanel.MinWidth, menuPanel.Width + e.HorizontalChange);
            menuPanel.Height = Math.Min(Math.Max(menuPanel.MinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(menuPanel.Height)) menuPanel.Height = menuPanel.ActualHeight;
            menuPanel.Height = Math.Min(Math.Max(menuPanel.MinHeight, menuPanel.Height + e.VerticalChange), MaxDropDownHeight);
        }

        // Handles drop down opened
        void OnDropDownClosed(object sender, EventArgs e)
        {
            if (DropDownClosed != null) DropDownClosed(this, e);
            //if (Mouse.Captured == this) Mouse.Capture(null);
        }

        // Handles drop down closed
        void OnDropDownOpened(object sender, EventArgs e)
        {
            menuPanel.Width = double.NaN;
            menuPanel.Height = double.NaN;
            if (DropDownOpened != null) DropDownOpened(this, e);
            //Mouse.Capture(this, CaptureMode.SubTree);
        }     

        #endregion
    }
}
