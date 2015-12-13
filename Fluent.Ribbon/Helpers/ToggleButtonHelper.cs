namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// Helper-Class for switching states in ToggleButton-Groups
    /// </summary>
    public class ToggleButtonHelper
    {
        // Grouped buttons
        private static readonly Dictionary<string, List<WeakReference>> groupedButtons = new Dictionary<string, List<WeakReference>>();

        /// <summary>
        /// Handles changes to <see cref="IToggleButton.GroupName"/>
        /// </summary>
        public static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleButton = (IToggleButton)d;
            var currentGroupName = (string)e.NewValue;
            var previousGroupName = (string)e.OldValue;

            if (previousGroupName != null)
            {
                RemoveFromGroup(previousGroupName, toggleButton);
            }

            if (currentGroupName != null)
            {
                AddToGroup(currentGroupName, toggleButton);
            }
        }

        /// <summary>
        /// Coerce <see cref="IToggleButton.IsChecked"/>
        /// </summary>
        public static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            var toggleButton = (IToggleButton)d;

            // If the button does not belong to any group
            // or the button/control is not loaded
            // we don't have to do any checks and can directly return the requested basevalue
            if (toggleButton.GroupName == null
                || toggleButton.IsLoaded == false)
            {
                return basevalue;
            }

            var baseIsChecked = (bool?)basevalue;

            if (baseIsChecked.HasValue == false
                || baseIsChecked.Value == false)
            {
                var buttons = GetButtonsInGroup(toggleButton.GroupName);

                // We can not allow that there is no button checked
                foreach (var item in buttons)
                {
                    // It's Ok, atleast one checked button exists
                    // and it's not the current button
                    if (ReferenceEquals(item, toggleButton) == false
                        && item.IsChecked == true)
                    {
                        return basevalue;
                    }
                }

                // This button can not be unchecked
                return true;
            }

            return basevalue;
        }

        /// <summary>
        /// Handles changes to <see cref="IToggleButton.IsChecked"/>
        /// </summary>
        public static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var newValue = (bool?)e.NewValue;
            var button = (IToggleButton)d;

            // Uncheck other toggle buttons
            if (!newValue.HasValue || !newValue.Value || button.GroupName == null)
            {
                return;
            }

            var buttons = GetButtonsInGroup(button.GroupName);

            foreach (var item in buttons.Where(item => item != button))
            {
                item.IsChecked = false;
            }
        }

        /// <summary>
        /// Remove from group
        /// </summary>
        private static void RemoveFromGroup(string groupName, IToggleButton toggleButton)
        {
            List<WeakReference> buttons;

            if (!groupedButtons.TryGetValue(groupName, out buttons))
            {
                return;
            }

            buttons.RemoveAt(buttons.FindIndex(x => (x.IsAlive && ((IToggleButton)x.Target) == toggleButton)));
        }

        /// <summary>
        /// Add to group
        /// </summary>
        private static void AddToGroup(string groupName, IToggleButton toggleButton)
        {
            List<WeakReference> buttons;

            if (!groupedButtons.TryGetValue(groupName, out buttons))
            {
                buttons = new List<WeakReference>();
                groupedButtons.Add(groupName, buttons);
            }

            buttons.Add(new WeakReference(toggleButton));
        }

        /// <summary>
        /// Gets all buttons in the given group
        /// </summary>
        private static IEnumerable<IToggleButton> GetButtonsInGroup(string groupName)
        {
            List<WeakReference> buttons;

            if (!groupedButtons.TryGetValue(groupName, out buttons))
            {
                return new List<IToggleButton>();
            }

            return buttons.Where(x => x.IsAlive).Select(x => (IToggleButton)x.Target).ToList();
        }
    }
}