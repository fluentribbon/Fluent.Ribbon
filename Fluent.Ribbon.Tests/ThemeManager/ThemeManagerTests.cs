namespace Fluent.Tests.ThemeManager
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using Fluent;
    using Fluent.Tests.TestClasses;
    using FluentTest.Helpers;
    using NUnit.Framework;

    [CLSCompliant(false)]
    [TestFixture]
    public class ThemeManagerTest
    {
        [SetUp]
        public void SetUp()
        {
            ThemeManager.themes = null;
        }

        [TearDown]
        public void TearDown()
        {
            ThemeManager.themes = null;
        }

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
            Assert.That(theme.Resources.Source.ToString(), Is.EqualTo("pack://application:,,,/Fluent;component/Themes/Themes/Dark.Blue.xaml").IgnoreCase);
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
            var expectedThemes = new[]
                                 {
                                     "Amber (Dark)",
                                     "Amber (Light)",
                                     "Blue (Colorful)",
                                     "Blue (Dark)",
                                     "Blue (Light)",
                                     "Brown (Dark)",
                                     "Brown (Light)",
                                     "Cobalt (Dark)",
                                     "Cobalt (Light)",
                                     "Crimson (Dark)",
                                     "Gray (Colorful)",
                                     "Green (Dark)",
                                     "Green (Light)",
                                     "Indigo (Dark)",
                                     "Indigo (Light)",
                                     "Lime (Dark)",
                                     "Lime (Light)",
                                     "Magenta (Dark)",
                                     "Magenta (Light)",
                                     "Mauve (Dark)",
                                     "Crimson (Light)",
                                     "Cyan (Dark)",
                                     "Cyan (Light)",
                                     "Emerald (Dark)",
                                     "Emerald (Light)",
                                     "Mauve (Light)",
                                     "Olive (Dark)",
                                     "Olive (Light)",
                                     "Orange (Dark)",
                                     "Orange (Light)",
                                     "Pink (Dark)",
                                     "Pink (Light)",
                                     "Purple (Dark)",
                                     "Purple (Light)",
                                     "Red (Dark)",
                                     "Red (Light)",
                                     "Sienna (Dark)",
                                     "Sienna (Light)",
                                     "Steel (Dark)",
                                     "Steel (Light)",
                                     "Taupe (Dark)",
                                     "Taupe (Light)",
                                     "Teal (Dark)",
                                     "Teal (Light)",
                                     "Violet (Dark)",
                                     "Violet (Light)",
                                     "Yellow (Dark)",
                                     "Yellow (Light)"
                                 };
            Assert.That(ThemeManager.Themes.OrderBy(x => x.DisplayName).Select(x => x.DisplayName).ToList(), Is.EquivalentTo(expectedThemes));
        }

        [Test]
        public void CreateDynamicThemeWithColor()
        {
            var applicationTheme = ThemeManager.DetectTheme(Application.Current);

            Assert.That(() => ThemeHelper.CreateTheme("Light", Colors.Red, Colors.Red, "CustomAccentRed", changeImmediately: true), Throws.Nothing);

            var detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.That(detected.Name, Is.EqualTo("CustomAccentRed"));

            Assert.That(() => ThemeHelper.CreateTheme("Dark", Colors.Green, Colors.Green, "CustomAccentGreen", changeImmediately: true), Throws.Nothing);

            detected = ThemeManager.DetectTheme(Application.Current);
            Assert.NotNull(detected);
            Assert.That(detected.Name, Is.EqualTo("CustomAccentGreen"));

            ThemeManager.ChangeTheme(Application.Current, applicationTheme);
        }

        [Test]
        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/dark.blue.xaml", "Dark", "#FF2B579A", "#FF086F9E")]
        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/dark.green.xaml", "Dark", "#FF60A917", "#FF477D11")]

        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/Light.blue.xaml", "Light", "#FF2B579A", "#FF086F9E")]
        [TestCase("pack://application:,,,/Fluent;component/Themes/themes/Light.green.xaml", "Light", "#FF60A917", "#FF477D11")]
        public void CompareGeneratedAppStyleWithShipped(string source, string baseColor, string color, string highlightColor)
        {
            var dic = new ResourceDictionary
            {
                Source = new Uri(source)
            };

            var newDic = ThemeHelper.CreateTheme(baseColor, (Color)ColorConverter.ConvertFromString(color), (Color)ColorConverter.ConvertFromString(highlightColor));

            var ignoredKeyValues = new[]
                                   {
                                       "Theme.Name",
                                       "Theme.DisplayName",
                                       "Theme.ColorScheme",
                                       "Fluent.Ribbon.Colors.HighlightColor", // Ignored because it's hand crafted
                                       "Fluent.Ribbon.Brushes.HighlightBrush", // Ignored because it's hand crafted
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