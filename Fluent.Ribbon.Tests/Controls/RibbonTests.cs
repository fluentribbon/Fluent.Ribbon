namespace Fluent.Tests.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Markup;
    using Fluent.Tests.Helper;
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
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }

                {
                    window.Language = enUs;
                    window.DataContext = window.Language;

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }

                {
                    window.Language = deDe;
                    window.DataContext = window.Language;

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
                }

                {
                    window.Language = enUs;
                    window.DataContext = window.Language;

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, window);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, window);
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

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }

                {
                    ribbon.Language = enUs;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(ribbon.Language, Is.Not.EqualTo(window.Language), "Language on Ribbon should not match Window.");
                    Assert.That(ribbon.DataContext, Is.Not.EqualTo(window.DataContext), "DataContext on Ribbon should not match Window.");

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }

                {
                    ribbon.Language = deDe;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Ribbon should match Window.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Ribbon should match Window.");

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }

                {
                    ribbon.Language = enUs;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(ribbon.Language, Is.Not.EqualTo(window.Language), "Language on Ribbon should not match Window.");
                    Assert.That(ribbon.DataContext, Is.Not.EqualTo(window.DataContext), "DataContext on Ribbon should not match Window.");

                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.DataContextProperty, ribbon);
                    CheckIfAllElementsHaveSameValue(elemens, FrameworkElement.LanguageProperty, ribbon);
                }
            }
        }

        private static void CheckIfAllElementsHaveSameValue(Dictionary<FrameworkElement, string> elements, DependencyProperty property, FrameworkElement expectedValueSource)
        {
            var expectedValue = expectedValueSource.GetValue(property);

            foreach (var element in elements)
            {
                Assert.That(element.Key.GetValue(property), Is.EqualTo(expectedValue), $"{property.Name} on {element.Value} should match.");
            }
        }

        [Test]
        public void TitleBar_properties_synchronised_with_ribbon()
        {
            var ribbon = new Ribbon { ContextualGroups = { new RibbonContextualTabGroup() } };
            using (new TestRibbonWindow(ribbon))
            {
                ribbon.ApplyTemplate();
                Assert.IsNotNull(ribbon.QuickAccessToolBar);

                var oldTitleBar = ribbon.TitleBar = new RibbonTitleBar();
                Assert.AreEqual(1, oldTitleBar.Items.Count);
                Assert.AreSame(ribbon.QuickAccessToolBar, oldTitleBar.QuickAccessToolBar);

                var newTitleBar = new RibbonTitleBar();
                Assert.AreEqual(0, newTitleBar.Items.Count);
                Assert.IsNull(newTitleBar.QuickAccessToolBar);

                // assign a new title bar, the contextual groups and quick access are transferred across
                ribbon.TitleBar = newTitleBar;
                Assert.AreEqual(0, oldTitleBar.Items.Count);
                Assert.IsNull(oldTitleBar.QuickAccessToolBar);
                Assert.AreEqual(1, newTitleBar.Items.Count);
                Assert.AreSame(ribbon.QuickAccessToolBar, newTitleBar.QuickAccessToolBar);

                // remove the title bar
                ribbon.TitleBar = null;
                Assert.AreEqual(0, oldTitleBar.Items.Count);
                Assert.IsNull(oldTitleBar.QuickAccessToolBar);
                Assert.AreEqual(0, newTitleBar.Items.Count);
                Assert.IsNull(newTitleBar.QuickAccessToolBar);
            }
        }

        [Test]
        public void Test_KeyTipKeys()
        {
            var ribbon = new Ribbon();
            var keyTipService = ribbon.GetFieldValue<KeyTipService>("keyTipService");

            Assert.That(ribbon.KeyTipKeys, Is.Empty);                
            Assert.That(keyTipService.KeyTipKeys, Is.EquivalentTo(KeyTipService.DefaultKeyTipKeys));

            ribbon.KeyTipKeys.Add(Key.A);

            Assert.That(ribbon.KeyTipKeys, Is.EquivalentTo(new[]
                                                           {
                                                               Key.A
                                                           }));

            Assert.That(keyTipService.KeyTipKeys, Is.EquivalentTo(new[]
                                                           {
                                                               Key.A
                                                           }));
        }
    }
}