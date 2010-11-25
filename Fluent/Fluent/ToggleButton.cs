#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Linq;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents toggle button
    /// </summary>
    [ContentProperty("Header")]
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton, IRibbonControl, IQuickAccessItemProvider
    {
        #region Properties

        #region GroupName

        /// <summary>
        /// Gets or sets the name of the group that the toggle button belongs to. 
        /// Use the GroupName property to specify a grouping of toggle buttons to 
        /// create a mutually exclusive set of controls. You can use the GroupName 
        /// property when only one selection is possible from a list of available 
        /// options. When this property is set, only one ToggleButton in the specified
        /// group can be selected at a time.
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(ToggleButton),
            new UIPropertyMetadata(null, OnGroupNameChanged));

        // Group name changed
        static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)d;
            string currentGroupName = (string)e.NewValue;
            string previousGroupName = (string)e.OldValue;

            if (previousGroupName != null) RemoveFromGroup(previousGroupName, toggleButton);
            if (currentGroupName != null) AddToGroup(currentGroupName, toggleButton);
        }

        #region Grouped Button Methods

        // Grouped buttons
        static readonly Dictionary<string, List<WeakReference>> groupedButtons =
            new Dictionary<string, List<WeakReference>>();

        // Remove from group
        static void RemoveFromGroup(string groupName, ToggleButton button)
        {
            List<WeakReference> buttons = null;
            if (!groupedButtons.TryGetValue(groupName, out buttons)) return;

            buttons.RemoveAt(buttons.FindIndex(x => (x.IsAlive && ((ToggleButton)x.Target) == button)));
        }

        // Remove from group
        static void AddToGroup(string groupName, ToggleButton button)
        {
            List<WeakReference> buttons = null;
            if (!groupedButtons.TryGetValue(groupName, out buttons))
            {
                buttons = new List<WeakReference>();
                groupedButtons.Add(groupName, buttons);
            }

            buttons.Add(new WeakReference(button));
        }

        // Gets all buttons in the given group
        static IEnumerable<ToggleButton> GetButtonsInGroup(string groupName)
        {
            List<WeakReference> buttons = null;
            if (!groupedButtons.TryGetValue(groupName, out buttons)) return new List<ToggleButton>();
            return buttons.Where(x => x.IsAlive).Select(x => (ToggleButton)x.Target);
        }

        #endregion

        #endregion

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(ToggleButton));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.AttachSizeDefinition(typeof(ToggleButton));

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ToggleButton));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ToggleButton), new UIPropertyMetadata(null, OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToggleButton element = d as ToggleButton;
            FrameworkElement oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null) element.RemoveLogicalChild(oldElement);
            FrameworkElement newElement = e.NewValue as FrameworkElement;
            if (newElement != null) element.AddLogicalChild(newElement);
        }

        #endregion

        #region LargeIcon

        /// <summary>
        /// Gets or sets button large icon
        /// </summary>
        public object LargeIcon
        {
            get { return GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(object),
            typeof(ToggleButton), new UIPropertyMetadata(null));

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
            DependencyProperty.Register("IsDefinitive", typeof(bool), typeof(ToggleButton), new UIPropertyMetadata(true));

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ToggleButton()
        {
            Type type = typeof(ToggleButton);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            IsCheckedProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(OnIsCheckedChanged, CoerceIsChecked));
            ContextMenuService.Attach(type);
            ToolTipService.Attach(type);
            StyleProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));            
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(ToggleButton));
            }

            return basevalue;
        }

        // Coerce IsChecked
        static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            ToggleButton toggleButton = (ToggleButton)d;
            if (toggleButton.GroupName == null) return basevalue;

            bool baseIsChecked = (bool)basevalue;
            if (!baseIsChecked)
            {
                // We can not allow that there are no one button checked
                foreach (ToggleButton item in GetButtonsInGroup(toggleButton.GroupName))
                {
                    // It's Ok, atleast one checked button exists
                    if (item.IsChecked == true) return false;
                }

                // This button can not be unchecked
                return true;
            }
            return basevalue;
        }

        // Handles isChecked changed
        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool newValue = (bool)e.NewValue;
            ToggleButton button = (ToggleButton)d;

            // Uncheck other toggle buttons
            if (newValue && button.GroupName != null)
            {
                foreach (ToggleButton item in GetButtonsInGroup(button.GroupName))
                    if (item != button) item.IsChecked = false;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ToggleButton()
        {
            ContextMenuService.Coerce(this);
            FocusManager.SetIsFocusScope(this,true);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Called when a <see cref="T:System.Windows.Controls.Button"/> is clicked. 
        /// </summary>
        protected override void OnClick()
        {
            // Close popup on click
            if (IsDefinitive) PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
            base.OnClick();
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            ToggleButton button = new ToggleButton();

            RibbonControl.Bind(this, button, "IsChecked", IsCheckedProperty, BindingMode.TwoWay);
            button.Click += ((sender, e) => RaiseEvent(e));
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ToggleButton), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        #endregion

        #region Implementation of IKeyTipedControl

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            OnClick();
        }

        #endregion
    }
}
