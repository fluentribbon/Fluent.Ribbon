// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Helper-Class for switching states in ToggleButton-Groups
    /// </summary>
    public class ToggleButtonHelper
    {
        // Grouped buttons
        [ThreadStatic]
        private static Dictionary<string, List<WeakReference>> groupedButtons;

        private static Dictionary<string, List<WeakReference>> GroupedButtons
        {
            get { return groupedButtons ?? (groupedButtons = new Dictionary<string, List<WeakReference>>()); }
        }

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
                var buttons = GetButtonsInGroup(toggleButton);

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
            if (newValue.HasValue == false
                || newValue.Value == false
                || button.GroupName == null)
            {
                return;
            }

            List<WeakReference> buttons;

            if (GroupedButtons.TryGetValue(button.GroupName, out buttons) == false)
            {
                return;
            }

            var rootScope = PresentationSource.FromVisual((Visual)button);

            // Get all elements bound to this key and remove this element
            for (var i = 0; i < buttons.Count;)
            {
                var weakReference = buttons[i];
                var currentButton = weakReference.Target as IToggleButton;
                if (currentButton == null)
                {
                    // Remove dead instances
                    buttons.RemoveAt(i);
                }
                else
                {
                    // Uncheck all checked RadioButtons different from the current one
                    if (currentButton != button
                        && currentButton.IsChecked == true
                        && rootScope != null
                        && PresentationSource.FromVisual((Visual)currentButton) != null
                        && rootScope == PresentationSource.FromVisual((Visual)currentButton))
                    {
                        currentButton.IsChecked = false;
                    }

                    i++;
                }
            }
        }

        /// <summary>
        /// Remove from group
        /// </summary>
        private static void RemoveFromGroup(string groupName, IToggleButton toggleButton)
        {
            List<WeakReference> buttons;

            if (GroupedButtons.TryGetValue(groupName, out buttons) == false)
            {
                return;
            }

            PurgeDead(buttons, toggleButton);

            if (buttons.Count == 0)
            {
                GroupedButtons.Remove(groupName);
            }
        }

        /// <summary>
        /// Add to group
        /// </summary>
        private static void AddToGroup(string groupName, IToggleButton toggleButton)
        {
            List<WeakReference> buttons;

            if (GroupedButtons.TryGetValue(groupName, out buttons) == false)
            {
                buttons = new List<WeakReference>();
                GroupedButtons.Add(groupName, buttons);
            }
            else
            {
                PurgeDead(buttons, null);
            }

            buttons.Add(new WeakReference(toggleButton));
        }

        /// <summary>
        /// Gets all buttons in the given group
        /// </summary>
        private static IEnumerable<IToggleButton> GetButtonsInGroup(IToggleButton button)
        {
            List<WeakReference> buttons;

            if (GroupedButtons.TryGetValue(button.GroupName, out buttons) == false)
            {
                return Enumerable.Empty<IToggleButton>();
            }

            PurgeDead(buttons, null);

            var rootScope = PresentationSource.FromVisual((Visual)button);

            return buttons
                .Where(x => rootScope == PresentationSource.FromVisual((Visual)x.Target))
                .Select(x => (IToggleButton)x.Target).ToList();
        }

        private static void PurgeDead(List<WeakReference> elements, object elementToRemove)
        {
            for (var i = 0; i < elements.Count;)
            {
                var weakReference = elements[i];
                var element = weakReference.Target;

                if (element == null
                    || element == elementToRemove)
                {
                    elements.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}