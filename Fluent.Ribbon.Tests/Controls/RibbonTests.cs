namespace Fluent.Tests.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Markup;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonTests
    {
        [Test]
        public void DependencyProperties_and_DataContext_should_be_inherited_from_window()
        {
            var ribbon = new Ribbon
                         {
                             Menu = new Backstage(),
                             StartScreen = new StartScreen()
                         };

            var enUs = XmlLanguage.GetLanguage("en-US");
            var deDe = XmlLanguage.GetLanguage("de-DE");

            using (var window = new TestRibbonWindow(ribbon)
                                {
                                    Language = deDe,
                                    DataContext = deDe
                                })
            {
                ribbon.ApplyTemplate();

                var elemens = new Dictionary<FrameworkElement, string>
                              {
                                  { ribbon, "Ribbon" },
                                  { ribbon.Menu, "Menu" },
                                  { ribbon.StartScreen, "StartScreen" },
                                  { ribbon.QuickAccessToolBar, "QuickAccessToolBar" },
                                  { ribbon.TabControl, "TabControl" },
                                  { (FrameworkElement)ribbon.Template.FindName("PART_LayoutRoot", ribbon), "PART_LayoutRoot" },
                              };

                {
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }

                {
                    window.Language = enUs;
                    window.DataContext = window.Language;

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }

                {
                    window.Language = deDe;
                    window.DataContext = window.Language;

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }

                {
                    window.Language = enUs;
                    window.DataContext = window.Language;

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }
            }
        }

        [Test]
        public void DependencyProperties_and_DataContext_should_be_inherited_from_ribbon()
        {
            var ribbon = new Ribbon
                         {
                             Menu = new Backstage(),
                             StartScreen = new StartScreen()
                         };

            var enUs = XmlLanguage.GetLanguage("en-US");
            var deDe = XmlLanguage.GetLanguage("de-DE");

            using (var window = new TestRibbonWindow(ribbon)
                                {
                                    Language = deDe,
                                    DataContext = deDe
                                })
            {
                ribbon.ApplyTemplate();

                var elemens = new Dictionary<FrameworkElement, string>
                              {
                                  { ribbon, "Ribbon" },
                                  { ribbon.Menu, "Menu" },
                                  { ribbon.StartScreen, "StartScreen" },
                                  { ribbon.QuickAccessToolBar, "QuickAccessToolBar" },
                                  { ribbon.TabControl, "TabControl" },
                                  { (FrameworkElement)ribbon.Template.FindName("PART_LayoutRoot", ribbon), "PART_LayoutRoot" },
                              };

                {
                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }

                {
                    ribbon.Language = enUs;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(ribbon.Language, Is.Not.EqualTo(window.Language), "Language on Ribbon should not match Window.");
                    Assert.That(ribbon.DataContext, Is.Not.EqualTo(window.DataContext), "DataContext on Ribbon should not match Window.");

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }

                {
                    ribbon.Language = deDe;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Ribbon should match Window.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Ribbon should match Window.");

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }

                {
                    ribbon.Language = enUs;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(ribbon.Language, Is.Not.EqualTo(window.Language), "Language on Ribbon should not match Window.");
                    Assert.That(ribbon.DataContext, Is.Not.EqualTo(window.DataContext), "DataContext on Ribbon should not match Window.");

                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    AssertAttElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }
            }
        }

        private static void AssertAttElementsHaveSameValue(Dictionary<FrameworkElement, string> elements, DependencyProperty property, FrameworkElement expectedValueSource)
        {
            var expectedValue = expectedValueSource.GetValue(property);

            foreach (var element in elements)
            {
                Assert.That(element.Key.GetValue(property), Is.EqualTo(expectedValue), $"{property.Name} on {element.Value} should match.");
            }            
        }
    }
}