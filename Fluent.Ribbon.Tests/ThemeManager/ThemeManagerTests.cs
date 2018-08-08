namespace Fluent.Tests.ThemeManager
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using System.Windows;
    using System.Windows.Media;
    using Fluent;
    using Fluent.Tests.TestClasses;
    using FluentTest.Helpers;
    using NUnit.Framework;

    [TestFixture]
    public class ThemeManagerTest
    {
        [Test]
        public void ChangeThemeForAppShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme((Application)null, ThemeManager.GetTheme("Light.Red")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme(Application.Current, ThemeManager.GetTheme("UnknownTheme")));
        }

        [Test]
        public void ChangeThemeForWindowShouldThrowArgumentNullException()
        {
            using (var window = new TestRibbonWindow())
            {
                Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme((Window)null, ThemeManager.GetTheme("Light.Red")));
                Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeTheme(Application.Current.MainWindow, ThemeManager.GetTheme("UnknownTheme")));
            }
        }

        [Test]
        public void CanAddThemeBeforeGetterIsCalled()
        {
            Assert.False(ThemeManager.AddTheme(new Uri("pack://application:,,,/Fluent;component/Themes/Themes/Dark.Cobalt.xaml")));

            var resource = new ResourceDictionary
                           {
                               {
                                   "Theme.Name", "Runtime"
                               },
                               {
                                   "Theme.DisplayName", "Runtime"
                               }
                           };
            Assert.True(ThemeManager.AddTheme(resource));
        }

        [Test]
        public void ChangesWindowTheme()
        {
            using (var window = new TestRibbonWindow())
            {
                var expectedTheme = ThemeManager.GetTheme("Dark.Teal");
                ThemeManager.ChangeTheme(Application.Current, expectedTheme);

                var theme = ThemeManager.DetectTheme(window);

                Assert.That(theme, Is.EqualTo(expectedTheme));
            }
        }

        [Test]
        public void GetInverseThemeReturnsDarkTheme()
        {
            var theme = ThemeManager.GetInverseTheme(ThemeManager.GetTheme("Light.Blue"));

            Assert.That(theme.Name, Is.EqualTo("Dark.Blue"));
        }

        [Test]
        public void GetInverseThemeReturnsLightTheme()
        {
            var theme = ThemeManager.GetInverseTheme(ThemeManager.GetTheme("Dark.Blue"));

            Assert.That(theme.Name, Is.EqualTo("Light.Blue"));
        }

        [Test]
        public void GetInverseThemeReturnsNullForMissingTheme()
        {
            var resource = new ResourceDictionary
                           {
                               {
                                   "Theme.Name", "Runtime"
                               },
                               {
                                   "Theme.DisplayName", "Runtime"
                               }
                           };
            var theme = new Theme(resource);

            var inverseTheme = ThemeManager.GetInverseTheme(theme);

            Assert.Null(inverseTheme);
        }

        [Test]
        public void GetThemeIsCaseInsensitive()
        {
            var theme = ThemeManager.GetTheme("Dark.Blue");

            Assert.NotNull(theme);
            Assert.That(theme.Resources.Source, Is.EqualTo(new Uri("pack://application:,,,/Fluent;component/Themes/Themes/Dark.Blue.xaml")));
        }

        [Test]
        public void GetThemeWithUriIsCaseInsensitive()
        {
            var dic = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Fluent;component/Themes/Themes/daRK.Blue.xaml")
            };

            var theme = ThemeManager.GetTheme(dic);

            Assert.NotNull(theme);
            Assert.That(theme.Name, Is.EqualTo("Dark.Blue"));
        }

        [Test]
        public void GetThemes()
        {
            var assembly = typeof(ThemeManager).Assembly;
            var resourceDictionaries = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceDictionaries.Where(x => x.EndsWith(".g.resources")))
            {
                var info = assembly.GetManifestResourceInfo(resourceName);
                if (info.ResourceLocation != ResourceLocation.ContainedInAnotherAssembly)
                {
                    var resourceStream = assembly.GetManifestResourceStream(resourceName);
                    using (var reader = new ResourceReader(resourceStream))
                    {
                        foreach (DictionaryEntry entry in reader)
                        {
                            System.Diagnostics.Trace.WriteLine(entry.Key);
                            //Here you can see all your ResourceDictionaries
                            //entry is your ResourceDictionary from assembly
                        }
                    }
                }
            }
        }

        [Test]
        public void CreateDynamicThemeWithColor()
        {
            var applicationTheme = ThemeManager.DetectTheme(Application.Current);

            Assert.That(() => ThemeHelper.CreateTheme("Light", Colors.Red, "CustomAccentRed", changeImmediately: true), Throws.Nothing);

            var detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.That(detected.Name, Is.EqualTo("CustomAccentRed"));

            Assert.That(() => ThemeHelper.CreateTheme("Dark", Colors.Green, "CustomAccentGreen", changeImmediately: true), Throws.Nothing);

            detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.That(detected.Name, Is.EqualTo("CustomAccentGreen"));

            ThemeManager.ChangeTheme(Application.Current, applicationTheme);
        }

        [Test]
        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/dark.blue.xaml", "Dark", "#FF2B579A")]
        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/dark.green.xaml", "Dark", "#FF60A917")]

        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/Light.blue.xaml", "Light", "#FF2B579A")]
        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/Light.green.xaml", "Light", "#FF60A917")]
        public void CompareGeneratedAppStyleWithShipped(string source, string baseColor, string color)
        {
            var dic = new ResourceDictionary
            {
                Source = new Uri(source)
            };

            var newDic = ThemeHelper.CreateTheme(baseColor, (Color)ColorConverter.ConvertFromString(color));

            var ignoredKeyValues = new[]
                                   {
                                       "Theme.Name",
                                       "Theme.DisplayName",
                                       "Theme.ColorScheme",
                                       "Fluent.Ribbon.Colors.HighlightColor", // Ignored because it's hand crafted
                                       "Fluent.Ribbon.Brushes.HighlightBrush", // Ignored because it's hand crafted
                                       "Fluent.Ribbon.Brushes.ToggleButton.Checked.BorderBrush", // Ignored because it's based on highlight color
                                       "Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemMouseOverForeground", // Ignored because it's based on highlight color
                                       "Fluent.Ribbon.Brushes.RibbonTabItem.MouseOver.Foreground", // Ignored because it's based on highlight color
                                       "Fluent.Ribbon.Brushes.RibbonTabItem.Selected.MouseOver.Foreground", // Ignored because it's based on highlight color
                                   };
            CompareResourceDictionaries(dic, newDic, ignoredKeyValues);
            CompareResourceDictionaries(newDic, dic, ignoredKeyValues);
        }

        private static void CompareResourceDictionaries(ResourceDictionary first, ResourceDictionary second, params string[] ignoredKeyValues)
        {
            foreach (var key in first.Keys)
            {
                if (second.Contains(key) == false)
                {
                    throw new Exception($"Key \"{key}\" is missing from {second.Source}.");
                }

                if (ignoredKeyValues.Contains(key) == false)
                {
                    Assert.That(second[key].ToString(), Is.EqualTo(first[key].ToString()), $"Values for {key} should be equal.");
                }
            }
        }
    }
}