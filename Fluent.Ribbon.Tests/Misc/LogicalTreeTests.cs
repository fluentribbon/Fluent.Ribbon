namespace Fluent.Tests.Misc
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Fluent.Helpers;
    using NUnit.Framework;
    using Ribbon = Fluent.Ribbon;
    using RibbonControl = Fluent.RibbonControl;

    [TestFixture]
    public class LogicalTreeTests
    {
        [Test]
        [TestCaseSource(nameof(GetTypesWithImplementedInterface), new object[]
        {
            typeof(IRibbonControl)
        })]
        public void LogicalTreeShouldWorkForIcon(Type controlType)
        {
            if (typeof(MenuItem).IsAssignableFrom(controlType))
            {
                TestLogicalTree(controlType, MenuItem.IconProperty);
            }
            else
            {
                TestLogicalTree(controlType, RibbonControl.IconProperty);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetTypesWithImplementedInterface), new object[]
        {
            typeof(ILargeIconProvider)
        })]
        public void LogicalTreeShouldWorkForLargeIcon(Type controlType)
        {
            TestLogicalTree(controlType, LargeIconProviderProperties.LargeIconProperty);
        }

        [Test]
        [TestCaseSource(nameof(GetTypesWithImplementedInterface), new object[]
        {
            typeof(IHeaderedControl)
        })]
        public void LogicalTreeShouldWorkForHeader(Type controlType)
        {
            if (typeof(HeaderedItemsControl).IsAssignableFrom(controlType))
            {
                TestLogicalTree(controlType, HeaderedItemsControl.HeaderProperty);
            }
            else
            {
                TestLogicalTree(controlType, RibbonControl.HeaderProperty);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetTypesThatMustHaveLogicalChildSupport))]
        public void CheckLogicalChildSupport(KeyValuePair<Type, DependencyProperty> item)
        {
            var controlType = item.Key;
            var dependencyProperty = item.Value;

            var control = (DependencyObject)Activator.CreateInstance(controlType, true);

            Assert.That(control, Is.Not.Null);

            var metadata = dependencyProperty.GetMetadata(control);

            Assert.That(metadata.PropertyChangedCallback == LogicalChildSupportHelper.OnLogicalChildPropertyChanged);

            Assert.That(controlType.GetInterfaces(), Does.Contain(typeof(ILogicalChildSupport)));

            if (dependencyProperty.ReadOnly)
            {
                var dependencyPropertykeyFieldName = dependencyProperty.Name + "PropertyKey";
                var dependencyPropertyKeyField = controlType.GetField(dependencyPropertykeyFieldName, BindingFlags.Static | BindingFlags.NonPublic);

                Assert.That(dependencyPropertyKeyField, Is.Not.Null, $"Field \"{dependencyPropertykeyFieldName}\" must exist.");

                var dependencyPropertyKey = (DependencyPropertyKey)dependencyPropertyKeyField.GetValue(null);

                TestLogicalTree(controlType, dependencyProperty, dependencyPropertyKey);
            }
            else
            {
                TestLogicalTree(controlType, dependencyProperty);
            }
        }

        private static IEnumerable<Type> GetTypesWithImplementedInterface(Type type)
        {
            return typeof(Ribbon).Assembly.GetTypes()
                .Where(x => type.IsAssignableFrom(x) && x.IsAbstract == false);
        }

        private static readonly Type[] excludedTypesForLogicalChildSupportTest =
        {
            typeof(LargeIconProviderProperties)
        };

        private static readonly DependencyProperty[] excludedPropertiesForLogicalChildSupportTest =
        {
            GalleryItem.CommandParameterProperty,
            RibbonGroupBox.LauncherCommandParameterProperty,
            RibbonGroupBox.LauncherToolTipProperty,
            SplitButton.CommandParameterProperty,
            SplitButton.DropDownToolTipProperty
        };

        private static IEnumerable<KeyValuePair<Type, DependencyProperty>> GetTypesThatMustHaveLogicalChildSupport()
        {
            foreach (var keyValuePair in GetDependencyPropertiesWithPropertyTypeObject())
            {
                if (excludedTypesForLogicalChildSupportTest.Contains(keyValuePair.Key))
                {
                    continue;
                }

                if (excludedPropertiesForLogicalChildSupportTest.Contains(keyValuePair.Value))
                {
                    continue;
                }

                yield return keyValuePair;
            }
        }

        private static IEnumerable<KeyValuePair<Type, DependencyProperty>> GetDependencyPropertiesWithPropertyTypeObject()
        {
            var types = typeof(Ribbon).Assembly.GetTypes()
                .Where(x => typeof(DependencyObject).IsAssignableFrom(x) && x.IsAbstract == false);
            foreach (var type in types)
            {
                var properties = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => typeof(DependencyProperty).IsAssignableFrom(x.FieldType))
                    .Select(x => x.GetValue(null))
                    .Cast<DependencyProperty>();

                foreach (var dependencyProperty in properties)
                {
                    if (dependencyProperty.PropertyType == typeof(object))
                    {
                        yield return new KeyValuePair<Type, DependencyProperty>(type, dependencyProperty);
                    }
                }
            }
        }

        private static void TestLogicalTree(Type controlType, DependencyProperty property, DependencyPropertyKey propertyKey = null)
        {
            var control = (DependencyObject)Activator.CreateInstance(controlType, true);

            Assert.That(control, Is.Not.Null);

            {
                var value = new object();
                SetValue(value);

                {
                    var children = LogicalTreeHelper.GetChildren(control);

                    Assert.That(children, Does.Contain(value), "Logical children must contain the value.");
                }

                SetValue(null);

                {
                    var children = LogicalTreeHelper.GetChildren(control);

                    Assert.That(children, Does.Not.Contain(value), "Logical children must NOT contain the value.");
                }
            }

            {
                var value = new DependencyObject();
                SetValue(value);

                {
                    var children = LogicalTreeHelper.GetChildren(control);

                    Assert.That(children, Does.Contain(value), "Logical children must contain the value.");
                }

                {
                    var parent = LogicalTreeHelper.GetParent(value);
                    Assert.That(parent, Is.Null, "Dependency-Objects don't support logical parents.");
                }

                SetValue(null);

                {
                    var children = LogicalTreeHelper.GetChildren(control);

                    Assert.That(children, Does.Not.Contain(value), "Logical children must NOT contain the value.");
                }
            }

            {
                var value = new FrameworkElement();
                SetValue(value);

                {
                    var children = LogicalTreeHelper.GetChildren(control);

                    Assert.That(children, Does.Contain(value), "Logical children must contain the value.");
                }

                {
                    var parent = LogicalTreeHelper.GetParent(value);
                    Assert.That(parent, Is.EqualTo(control), "Parent should match.");
                }

                SetValue(null);

                {
                    var children = LogicalTreeHelper.GetChildren(control);

                    Assert.That(children, Does.Not.Contain(value), "Logical children must NOT contain the value.");
                }
            }

            void SetValue(object value)
            {
                if (propertyKey is null)
                {
                    control.SetValue(property, value);
                }
                else
                {
                    control.SetValue(propertyKey, value);
                }
            }
        }
    }
}