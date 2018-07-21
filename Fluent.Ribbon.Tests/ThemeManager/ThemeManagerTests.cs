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

    [TestFixture]
    public class ThemeManagerTest
    {
        [Test]
        public void ChangeAppStyleForAppShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle((Application)null, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("BaseLight")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("UnknownTheme")));
            Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("UnknownAccentColor"), ThemeManager.GetAppTheme("BaseLight")));
        }

        [Test]
        public void ChangeAppStyleForWindowShouldThrowArgumentNullException()
        {
            using (var window = new TestRibbonWindow())
            {
                Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle((Window)null, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("BaseLight")));
                Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current.MainWindow, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme("UnknownTheme")));
                Assert.Throws<ArgumentNullException>(() => ThemeManager.ChangeAppStyle(Application.Current.MainWindow, ThemeManager.GetAccent("UnknownAccentColor"), ThemeManager.GetAppTheme("BaseLight")));
            }
        }

        [Test]
        public void CanAddAccentBeforeGetterIsCalled()
        {
            Assert.True(ThemeManager.AddAccent("TestAccent", new Uri("pack://application:,,,/Fluent;component/Themes/Accents/Blue.xaml")));
        }

        [Test]
        public void CanAddAppThemeBeforeGetterIsCalled()
        {
            Assert.True(ThemeManager.AddAppTheme("TestTheme", new Uri("pack://application:,,,/Fluent;component/Themes/Colors/BaseDark.xaml")));
        }

        [Test]
        public void ChangesWindowTheme()
        {
            using (var window = new TestRibbonWindow())
            {
                Accent expectedAccent = ThemeManager.Accents.First(x => x.Name == "Teal");
                AppTheme expectedTheme = ThemeManager.GetAppTheme("BaseDark");
                ThemeManager.ChangeAppStyle(Application.Current, expectedAccent, expectedTheme);

                var theme = ThemeManager.DetectAppStyle(window);

                Assert.That(theme.Item1, Is.EqualTo(expectedTheme));
                Assert.That(theme.Item2, Is.EqualTo(expectedAccent));
            }
        }

        [Test]
        public void GetInverseAppThemeReturnsDarkTheme()
        {
            AppTheme theme = ThemeManager.GetInverseAppTheme(ThemeManager.GetAppTheme("BaseLight"));

            Assert.That(theme.Name, Is.EqualTo("BaseDark"));
        }

        [Test]
        public void GetInverseAppThemeReturnsLightTheme()
        {
            AppTheme theme = ThemeManager.GetInverseAppTheme(ThemeManager.GetAppTheme("BaseDark"));

            Assert.That(theme.Name, Is.EqualTo("BaseLight"));
        }

        [Test]
        public void GetInverseAppThemeReturnsNullForMissingTheme()
        {
            var appTheme = new AppTheme("TestTheme", new Uri("pack://application:,,,/Fluent;component/Themes/Colors/BaseDark.xaml"));

            AppTheme theme = ThemeManager.GetInverseAppTheme(appTheme);

            Assert.Null(theme);
        }

        [Test]
        public void GetAppThemeIsCaseInsensitive()
        {
            AppTheme theme = ThemeManager.GetAppTheme("basedark");

            Assert.NotNull(theme);
            Assert.That(theme.Resources.Source, Is.EqualTo(new Uri("pack://application:,,,/Fluent;component/Themes/Colors/BaseDark.xaml")));
        }

        [Test]
        public void GetAppThemeWithUriIsCaseInsensitive()
        {
            var dic = new ResourceDictionary
                      {
                          Source = new Uri("pack://application:,,,/Fluent;component/Themes/Colors/basedark.xaml")
                      };

            AppTheme detected = ThemeManager.GetAppTheme(dic);

            Assert.NotNull(detected);
            Assert.That(detected.Name, Is.EqualTo("BaseDark"));
        }

        [Test]
        public void GetAccentIsCaseInsensitive()
        {
            Accent accent = ThemeManager.GetAccent("blue");

            Assert.NotNull(accent);
            Assert.That(accent.Resources.Source, Is.EqualTo(new Uri("pack://application:,,,/Fluent;component/Themes/Accents/Blue.xaml")));
        }

        [Test]
        public void GetAccentWithUriIsCaseInsensitive()
        {
            var dic = new ResourceDictionary
                      {
                          Source = new Uri("pack://application:,,,/Fluent;component/Themes/Accents/blue.xaml")
                      };

            Accent detected = ThemeManager.GetAccent(dic);

            Assert.NotNull(detected);
            Assert.That(detected.Name, Is.EqualTo("Blue"));
        }

        [Test]
        public void CreateDynamicAccentWithColor()
        {
            var applicationTheme = ThemeManager.DetectAppStyle(Application.Current);

            Assert.That(() => ThemeHelper.CreateAppStyleBy(Colors.Red, "CustomAccentRed", changeImmediately: true), Throws.Nothing);

            var detected = ThemeManager.DetectAppStyle(Application.Current);
            Assert.NotNull(detected);
            Assert.That(detected.Item2.Name, Is.EqualTo("CustomAccentRed"));

            Assert.That(() => ThemeHelper.CreateAppStyleBy(Colors.Green, "CustomAccentGreen", changeImmediately: true), Throws.Nothing);

            detected = ThemeManager.DetectAppStyle(Application.Current);
            Assert.NotNull(detected);
            Assert.That(detected.Item2.Name, Is.EqualTo("CustomAccentGreen"));

            ThemeManager.ChangeAppStyle(Application.Current, applicationTheme.Item2, applicationTheme.Item1);
        }

        [Test]
        [TestCase("pack://application:,,,/Fluent;component/Themes/Accents/blue.xaml", "#FF2B579A")]
        [TestCase("pack://application:,,,/Fluent;component/Themes/Accents/green.xaml", "#FF60A917")]
        public void CompareGeneratedAppStyleWithShipped(string source, string color)
        {
            var dic = new ResourceDictionary
                      {
                          Source = new Uri(source)
                      };

            var newDic = ThemeHelper.CreateAppStyleBy((Color)ColorConverter.ConvertFromString(color));

            var ignoredKeyValues = new[]
                                   {
                                       "Fluent.Ribbon.Colors.HighlightColor", // Ignored because it's hand crafted
                                       "Fluent.Ribbon.Brushes.HighlightBrush", // Ignored because it's hand crafted
                                       "Fluent.Ribbon.Brushes.ToggleButton.Checked.BorderBrush", // Ignored because it's based on highlight color
                                       "Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemMouseOverForeground.Light", // Ignored because it's based on highlight color
                                       "Fluent.Ribbon.Brushes.RibbonTabItem.MouseOver.Foreground.Light", // Ignored because it's based on highlight color
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