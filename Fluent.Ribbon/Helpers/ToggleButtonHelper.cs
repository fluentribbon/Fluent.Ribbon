// ReSharper disable once CheckNamespace

namespace Fluent
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    ///     Helper-Class for switching states in ToggleButton-Groups
    /// </summary>
    public static class ToggleButtonHelper
    {
        private static readonly MethodInfo getVisualRootMethodInfo = typeof(KeyboardNavigation).GetMethod("GetVisualRoot", BindingFlags.NonPublic | BindingFlags.Static);

        // Grouped buttons
        [ThreadStatic]
        private static Hashtable groupNameToElements;

        /// <summary>
        ///     Handles changes to <see cref="IToggleButton.GroupName" />
        /// </summary>
        public static void OnGroupNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggleButton = (IToggleButton)d;
            var newValue = e.NewValue as string;
            var oldValue = e.OldValue as string;

            if (newValue == oldValue)
            {
                return;
            }

            if (string.IsNullOrEmpty(oldValue) == false)
            {
                Unregister(oldValue, toggleButton);
            }

            if (string.IsNullOrEmpty(newValue))
            {
                return;
            }

            Register(newValue, toggleButton);
        }

        private static void Register(string groupName, IToggleButton toggleButton)
        {
            if (groupNameToElements == null)
            {
                groupNameToElements = new Hashtable(1);
            }

            var elements = (ArrayList)groupNameToElements[groupName];
            if (elements == null)
            {
                elements = new ArrayList(1);
                groupNameToElements[groupName] = elements;
            }
            else
            {
                PurgeDead(elements, null);
            }

            elements.Add(new WeakReference(toggleButton));
        }

        private static void Unregister(string groupName, IToggleButton toggleButton)
        {
            if (groupNameToElements == null)
            {
                return;
            }

            var groupNameToElement = (ArrayList)groupNameToElements[groupName];
            if (groupNameToElement != null)
            {
                PurgeDead(groupNameToElement, toggleButton);
                if (groupNameToElement.Count == 0)
                {
                    groupNameToElements.Remove(groupName);
                }
            }
        }

        private static void PurgeDead(ArrayList elements, object elementToRemove)
        {
            var index = 0;
            while (index < elements.Count)
            {
                var target = ((WeakReference)elements[index]).Target;
                if (target == null
                    || target == elementToRemove)
                {
                    elements.RemoveAt(index);
                }
                else
                {
                    ++index;
                }
            }
        }

        /// <summary>
        ///     Updates the states of all buttons inside the group which <paramref name="toggleButton" /> belongs to.
        /// </summary>
        public static void UpdateButtonGroup(IToggleButton toggleButton)
        {
            var groupName = toggleButton.GroupName;

            if (string.IsNullOrEmpty(groupName) == false)
            {
                var visualRoot = getVisualRootMethodInfo.Invoke(null, new object[] { (DependencyObject)toggleButton });
                if (groupNameToElements == null)
                {
                    groupNameToElements = new Hashtable(1);
                }

                var groupNameToElement = (ArrayList)groupNameToElements[groupName];
                var index = 0;
                while (index < groupNameToElement.Count)
                {
                    var target = ((WeakReference)groupNameToElement[index]).Target as IToggleButton;
                    if (target == null)
                    {
                        groupNameToElement.RemoveAt(index);
                    }
                    else
                    {
                        if (target != toggleButton)
                        {
                            var isCheckedValue = GetIsCheckedValue(target);

                            if (isCheckedValue != 0
                                && visualRoot == getVisualRootMethodInfo.Invoke(null, new object[] { (DependencyObject)target }))
                            {
                                UncheckToggleButton(target);
                            }
                        }

                        ++index;
                    }
                }
            }

            //else
            //{
            //    var parent = toggleButton.Parent;
            //    if (parent == null)
            //    {
            //        return;
            //    }

            //    foreach (var child in LogicalTreeHelper.GetChildren(parent))
            //    {
            //        var childAsToggleButton = child as IToggleButton;

            //        if (childAsToggleButton != null
            //            && childAsToggleButton != toggleButton
            //            && string.IsNullOrEmpty(childAsToggleButton.GroupName))
            //        {
            //            var isCheckedValue = GetIsCheckedValue(childAsToggleButton);

            //            if (isCheckedValue != 0)
            //            {
            //                UncheckToggleButton(childAsToggleButton);
            //            }
            //        }
            //    }
            //}
        }

        private static void UncheckToggleButton(IToggleButton toggleButton)
        {
            var dependencyObject = toggleButton as DependencyObject;

            if (dependencyObject == null)
            {
                return;
            }

            if (toggleButton is System.Windows.Controls.MenuItem)
            {
                dependencyObject.SetCurrentValue(System.Windows.Controls.MenuItem.IsCheckedProperty, BooleanBoxes.FalseBox);
            }
            else
            {
                dependencyObject.SetCurrentValue(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, BooleanBoxes.FalseBox);
            }
        }

        private static int GetIsCheckedValue(IToggleButton childAsToggleButton)
        {
            var isChecked = childAsToggleButton.IsChecked;

            var isCheckedValue = isChecked.GetValueOrDefault()
                                     ? (isChecked.HasValue
                                            ? 1
                                            : 0)
                                     : 0;
            return isCheckedValue;
        }

        /// <summary>
        ///     Handles changes to <see cref="IToggleButton.IsChecked" />
        /// </summary>
        public static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                UpdateButtonGroup((IToggleButton)d);
            }
        }
    }
}