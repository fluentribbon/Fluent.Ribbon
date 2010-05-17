#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Linq;

namespace Fluent
{
    /// <summary>
    /// Represents toggle button
    /// </summary>
    [ContentProperty("Text")]
    public class ToggleButton : Button
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

            buttons.RemoveAt(buttons.FindIndex(x=>(x.IsAlive && ((ToggleButton)x.Target) == button)));
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
            return buttons.Where(x => x.IsAlive).Select(x => (ToggleButton) x.Target);
        }

        #endregion

        #endregion

        #region IsChecked

        /// <summary>
        /// Get or set ToggleButton checked state
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
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleButton),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged, CoerceIsChecked));

        // Coerce IsChecked
        static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            ToggleButton toggleButton = (ToggleButton)d;
            if (toggleButton.GroupName == null) return basevalue;

            bool baseIsChecked = (bool) basevalue;
            if (!baseIsChecked)
            {
                // We can not allow that there are no one button checked
                foreach (ToggleButton item in GetButtonsInGroup(toggleButton.GroupName))
                {
                    // It's Ok, atleast one checked button exists
                    if (item.IsChecked) return false;
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
            bool oldValue = (bool)e.OldValue;
            ToggleButton button = (ToggleButton)d;
            if(oldValue!=newValue)
            {
                if(newValue)
                {
                    if (button.Checked != null) button.Checked(button, EventArgs.Empty);
                }
                else
                {
                    if (button.Unchecked != null) button.Unchecked(button, EventArgs.Empty);
                }
            }

            // Uncheck other toggle buttons
            if (newValue && button.GroupName != null)
            {
                foreach (ToggleButton item in GetButtonsInGroup(button.GroupName))
                    if (item != button) item.IsChecked = false;
            }
        }

        #endregion
        
        #region UseAutoCheck

        /// <summary>
        /// Gets or set a value indicating whether the IsChecked value and the appearance are 
        /// automatically changed when the button is clicked.
        /// </summary>
        public bool UseAutoCheck
        {
            get { return (bool)GetValue(UseAutoCheckProperty); }
            set { SetValue(UseAutoCheckProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UseAutoCheck.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UseAutoCheckProperty =
            DependencyProperty.Register("UseAutoCheck", typeof(bool), 
            typeof(ToggleButton), new UIPropertyMetadata(true));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occured then the toggle button has been checked
        /// </summary>
        public event EventHandler Checked;
        /// <summary>
        /// Occured then the toggle button has been unchecked
        /// </summary>
        public event EventHandler Unchecked;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButton), new FrameworkPropertyMetadata(typeof(ToggleButton)));
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public ToggleButton()
        {
            
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="args">The event data</param>
        protected override void OnClick(RoutedEventArgs args)
        {
            if(UseAutoCheck) IsChecked = !IsChecked;
            base.OnClick(args);
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
            ToggleButton button = new ToggleButton();
            BindQuickAccessItem(button);
            return button;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            Bind(this, element, "IsChecked", IsCheckedProperty, BindingMode.TwoWay);
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
