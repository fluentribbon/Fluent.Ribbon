namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public class ToggleButtonHelper
    {
        // Grouped buttons
        private static readonly Dictionary<string, List<WeakReference>> groupedButtons =
            new Dictionary<string, List<WeakReference>>();

        /// <summary>
        /// Handles changes to <see cref="IToggleButton.GroupName"/>
        /// </summary>
        public static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IToggleButton toggleButton = (IToggleButton)d;
            string currentGroupName = (string)e.NewValue;
            string previousGroupName = (string)e.OldValue;

            if (previousGroupName != null)
            {
                ToggleButtonHelper.RemoveFromGroup(previousGroupName, toggleButton);
            }

            if (currentGroupName != null)
            {
                ToggleButtonHelper.AddToGroup(currentGroupName, toggleButton);
            }
        }

        /// <summary>
        /// Coerce <see cref="IToggleButton.IsChecked"/>
        /// </summary>
        public static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            IToggleButton toggleButton = (IToggleButton)d;
            if (toggleButton.GroupName == null) return basevalue;

            bool baseIsChecked = (bool)basevalue;

            if (!baseIsChecked)
            {
                var buttons = ToggleButtonHelper.GetButtonsInGroup(toggleButton.GroupName);

                // We can not allow that there are no one button checked
                foreach (IToggleButton item in buttons)
                {
                    // It's Ok, atleast one checked button exists
                    // and it's not the current button
                    if (item != toggleButton
                        && item.IsChecked == true)
                    {
                        return false;
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
            bool newValue = (bool)e.NewValue;
            IToggleButton button = (IToggleButton)d;

            // Uncheck other toggle buttons
            if (newValue
                && button.GroupName != null)
            {
                var buttons = ToggleButtonHelper.GetButtonsInGroup(button.GroupName);

                foreach (IToggleButton item in buttons)
                {
                    if (item != button)
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }

        /// <summary>
        /// Remove from group
        /// </summary>
        private static void RemoveFromGroup(string groupName, IToggleButton toggleButton)
        {
            List<WeakReference> buttons = null;

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
            List<WeakReference> buttons = null;

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
            List<WeakReference> buttons = null;

            if (!groupedButtons.TryGetValue(groupName, out buttons))
            {
                return new List<IToggleButton>();
            }

            return buttons.Where(x => x.IsAlive).Select(x => (IToggleButton)x.Target).ToList();
        }
    }
}