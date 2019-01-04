namespace Fluent.Tests.ThemeManager
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
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
            ThemeManager.ClearThemes();
        }

        [TearDown]
        public void TearDown()
        {
            ThemeManager.ClearThemes();
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
                                   Theme.ThemeNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemeDisplayNameKey, "Runtime"
                               }
                           };
            Assert.True(ThemeManager.AddTheme(resource));
        }

        [Test]
        public void NewThemeAddsNewBaseColorAndColorScheme()
        {
            var resource = new ResourceDictionary
                           {
                               {
                                   Theme.ThemeNameKey, "Runtime"
                               },
                               {
                                   Theme.ThemeDisplayNameKey, "Runtime"
                               },
                               {
                                    Theme.ThemeBaseColorSchemeKey, "Foo"
                               },
                               {
                                   Theme.ThemeColorSchemeKey, "Bar"
                               },
                           };

            Assert.True(ThemeManager.AddTheme(resource));
            Assert.That(ThemeManager.BaseColors, Is.EqualTo(new[] { ThemeManager.BaseColorLight, ThemeManager.BaseColorDark, "Colorful", "Foo" }));
            Assert.That(ThemeManager.ColorSchemes.Select(x => x.Name), Does.Contain("Bar"));
        }

        [Test]
        public void ChangingAppThemeChangesWindowTheme()
        {
            using (var window = new TestRibbonWindow())
            {
                var expectedTheme = ThemeManager.GetTheme("Dark.Teal");
                ThemeManager.ChangeTheme(Application.Current, expectedTheme);

                Assert.That(ThemeManager.DetectTheme(Application.Current), Is.EqualTo(expectedTheme));
                Assert.That(ThemeManager.DetectTheme(window), Is.EqualTo(expectedTheme));
            }
        }

        [Test]
        public void ChangeBaseColor()
        {
            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.That(currentTheme, Is.Not.Null);
                ThemeManager.ChangeThemeBaseColor(Application.Current, ThemeManager.GetInverseTheme(currentTheme).BaseColorScheme);

                Assert.That(ThemeManager.DetectTheme(Application.Current).BaseColorScheme, Is.Not.EqualTo(currentTheme.BaseColorScheme));
                Assert.That(ThemeManager.DetectTheme(Application.Current).ColorScheme, Is.EqualTo(currentTheme.ColorScheme));
            }

            {
                using (var window = new TestRibbonWindow())
                {
                    var currentTheme = ThemeManager.DetectTheme(window);

                    Assert.That(currentTheme, Is.Not.Null);
                    ThemeManager.ChangeThemeBaseColor(window, ThemeManager.GetInverseTheme(currentTheme).BaseColorScheme);

                    Assert.That(ThemeManager.DetectTheme(window).BaseColorScheme, Is.Not.EqualTo(currentTheme.BaseColorScheme));
                    Assert.That(ThemeManager.DetectTheme(window).ColorScheme, Is.EqualTo(currentTheme.ColorScheme));
                }
            }

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.That(currentTheme, Is.Not.Null);

                var control = new Control();
                ThemeManager.ChangeThemeBaseColor(control.Resources, currentTheme, ThemeManager.GetInverseTheme(currentTheme).BaseColorScheme);

                Assert.That(ThemeManager.DetectTheme(control.Resources).BaseColorScheme, Is.Not.EqualTo(currentTheme.BaseColorScheme));
                Assert.That(ThemeManager.DetectTheme(control.Resources).ColorScheme, Is.EqualTo(currentTheme.ColorScheme));
            }
        }

        [Test]
        public void ChangeColorScheme()
        {
            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.That(currentTheme, Is.Not.Null);
                ThemeManager.ChangeThemeColorScheme(Application.Current, "Yellow");

                Assert.That(ThemeManager.DetectTheme(Application.Current).BaseColorScheme, Is.EqualTo(currentTheme.BaseColorScheme));
                Assert.That(ThemeManager.DetectTheme(Application.Current).ColorScheme, Is.EqualTo("Yellow"));
            }

            {
                using (var window = new TestRibbonWindow())
                {
                    var currentTheme = ThemeManager.DetectTheme(window);

                    Assert.That(currentTheme, Is.Not.Null);
                    ThemeManager.ChangeThemeColorScheme(window, "Green");

                    Assert.That(ThemeManager.DetectTheme(window).BaseColorScheme, Is.EqualTo(currentTheme.BaseColorScheme));
                    Assert.That(ThemeManager.DetectTheme(window).ColorScheme, Is.EqualTo("Green"));
                }
            }

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.That(currentTheme, Is.Not.Null);

                var control = new Control();
                ThemeManager.ChangeThemeColorScheme(control.Resources, currentTheme, "Red");

                Assert.That(ThemeManager.DetectTheme(control.Resources).BaseColorScheme, Is.EqualTo(currentTheme.BaseColorScheme));
                Assert.That(ThemeManager.DetectTheme(control.Resources).ColorScheme, Is.EqualTo("Red"));
            }

            Assert.That(ThemeManager.DetectTheme(Application.Current).ColorScheme, Is.EqualTo("Yellow"));
        }

        [Test]
        public void ChangeBaseColorAndColorScheme()
        {
            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.That(currentTheme, Is.Not.Null);
                ThemeManager.ChangeTheme(Application.Current, ThemeManager.BaseColorDark, "Yellow");

                Assert.That(ThemeManager.DetectTheme(Application.Current).BaseColorScheme, Is.EqualTo(ThemeManager.BaseColorDark));
                Assert.That(ThemeManager.DetectTheme(Application.Current).ColorScheme, Is.EqualTo("Yellow"));
            }

            {
                using (var window = new TestRibbonWindow())
                {
                    var currentTheme = ThemeManager.DetectTheme(window);

                    Assert.That(currentTheme, Is.Not.Null);
                    ThemeManager.ChangeTheme(window, ThemeManager.BaseColorLight, "Green");

                    Assert.That(ThemeManager.DetectTheme(window).BaseColorScheme, Is.EqualTo(ThemeManager.BaseColorLight));
                    Assert.That(ThemeManager.DetectTheme(window).ColorScheme, Is.EqualTo("Green"));
                }
            }

            {
                var currentTheme = ThemeManager.DetectTheme(Application.Current);

                Assert.That(currentTheme, Is.Not.Null);

                var control = new Control();
                ThemeManager.ChangeTheme(control.Resources, currentTheme, ThemeManager.BaseColorDark, "Red");

                Assert.That(ThemeManager.DetectTheme(control.Resources).BaseColorScheme, Is.EqualTo(ThemeManager.BaseColorDark));
                Assert.That(ThemeManager.DetectTheme(control.Resources).ColorScheme, Is.EqualTo("Red"));
            }

            Assert.That(ThemeManager.DetectTheme(Application.Current).ColorScheme, Is.EqualTo("Yellow"));
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
                                     "Crimson (Light)",
                                     "Cyan (Dark)",
                                     "Cyan (Light)",
                                     "Emerald (Dark)",
                                     "Emerald (Light)",
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
            Assert.That(CollectionViewSource.GetDefaultView(ThemeManager.Themes).Cast<Theme>().Select(x => x.DisplayName).ToList(), Is.EqualTo(expectedThemes));
        }

        [Test]
        public void GetBaseColors()
        {
            ThemeManager.ClearThemes();

            Assert.That(ThemeManager.BaseColors, Is.Not.Empty);
        }

        [Test]
        public void GetColorSchemes()
        {
            ThemeManager.ClearThemes();

            Assert.That(ThemeManager.ColorSchemes, Is.Not.Empty);
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

            var newTheme = ThemeHelper.CreateTheme(baseColor, (Color)ColorConverter.ConvertFromString(color), (Color)ColorConverter.ConvertFromString(highlightColor));

            var ignoredKeyValues = new[]
                                   {
                                       "Theme.Name",
                                       "Theme.DisplayName",
                                       "Theme.ColorScheme",
                                       "Fluent.Ribbon.Colors.HighlightColor", // Ignored because it's hand crafted
                                       "Fluent.Ribbon.Brushes.HighlightBrush", // Ignored because it's hand crafted
                                   };
            CompareResourceDictionaries(dic, newTheme.Item2, ignoredKeyValues);
            CompareResourceDictionaries(newTheme.Item2, dic, ignoredKeyValues);
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