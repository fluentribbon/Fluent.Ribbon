namespace FluentTest.Helpers
{
    using System;
    using System.IO;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;
    using Fluent;

    public class ThemeHelper
    {
        public static ResourceDictionary CreateAppStyleBy(Color accentBaseColor, string accentName = null, bool changeImmediately = false)
        {
            // create a runtime accent resource dictionary
            var resourceDictionary = new ResourceDictionary();

            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentBaseColor", accentBaseColor);
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor80", Color.FromArgb(204, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B));
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor60", Color.FromArgb(153, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B));
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor40", Color.FromArgb(102, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B));
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor20", Color.FromArgb(51, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B));

            resourceDictionary.Add("Fluent.Ribbon.Colors.HighlightColor", accentBaseColor);

            resourceDictionary.Add("Fluent.Ribbon.Colors.IdealForegroundColor", IdealTextColor(accentBaseColor));
            resourceDictionary.Add("Fluent.Ribbon.Colors.DarkIdealForegroundDisabledColor", (Color)ColorConverter.ConvertFromString("#ADADAD"));

            resourceDictionary.Add("Fluent.Ribbon.Colors.ExtremeHighlightColor", (Color)ColorConverter.ConvertFromString("#FFFFD232"));
            resourceDictionary.Add("Fluent.Ribbon.Colors.DarkExtremeHighlightColor", (Color)ColorConverter.ConvertFromString("#FFF29536"));

            // Accent brushes
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentBaseColorBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentBaseColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush80", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor80"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush60", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor60"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush40", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush20", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));

            resourceDictionary.Add("Fluent.Ribbon.Brushes.HighlightBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.HighlightColor"]));

            // Misc brushes
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ExtremeHighlightBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.ExtremeHighlightColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.DarkExtremeHighlightBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.DarkExtremeHighlightColor"]));

            // Foreground
            resourceDictionary.Add("Fluent.Ribbon.Brushes.IdealForegroundColorBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.IdealForegroundColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.IdealForegroundDisabledBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.IdealForegroundColor"], 0.4));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.DarkIdealForegroundDisabledBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.DarkIdealForegroundDisabledColor"]));

            // Button
            resourceDictionary.Add("Fluent.Ribbon.Brushes.Button.MouseOver.BorderBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.Button.MouseOver.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor60"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.Button.Pressed.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));

            // ToggleButton
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ToggleButton.Checked.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ToggleButton.Checked.BorderBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.HighlightColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ToggleButton.CheckedMouseOver.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ToggleButton.CheckedMouseOver.BorderBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor60"]));

            // GalleryItem
            resourceDictionary.Add("Fluent.Ribbon.Brushes.GalleryItem.MouseOver", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.GalleryItem.Selected", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.GalleryItem.Pressed", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor60"]));

            // MenuItem
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ApplicationMenuItem.CheckBox.Background", (Color)ColorConverter.ConvertFromString("#FFFCF1C2"));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.ApplicationMenuItem.CheckBox.BorderBrush", (Color)ColorConverter.ConvertFromString("#FFF29536"));

            // WindowCommands
            resourceDictionary.Add("Fluent.Ribbon.Brushes.WindowCommands.CaptionButton.MouseOver.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.WindowCommands.CaptionButton.Pressed.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));

            // Create theme
            accentName = accentName ?? $"ApplicationAccent_{accentBaseColor.ToString().Replace("#", string.Empty)}";
            var resDictName = accentName;

            if (resDictName.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase) == false)
            {
                resDictName += ".xaml";
            }

            var fileName = Path.Combine(Path.GetTempPath(), resDictName);
            using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings
                                                           {
                                                               Indent = true,
                                                               IndentChars = "    "
                                                           }))
            {
                XamlWriter.Save(resourceDictionary, writer);
            }

            resourceDictionary = new ResourceDictionary
                                 {
                                     Source = new Uri(fileName, UriKind.Absolute)
                                 };

            var newAccent = new Accent
                            {
                                Name = accentName,
                                Resources = resourceDictionary
                            };
            ThemeManager.AddAccent(newAccent.Name, newAccent.Resources.Source);

            // Apply theme
            if (changeImmediately)
            {
                var application = Application.Current;
                // detect current application theme
                var applicationTheme = ThemeManager.DetectAppStyle(application);
                ThemeManager.ChangeAppStyle(application, newAccent, applicationTheme.Item1);
            }

            return resourceDictionary;
        }

        /// <summary>
        ///     Determining Ideal Text Color Based on Specified Background Color
        ///     http://www.codeproject.com/KB/GDI-plus/IdealTextColor.aspx
        /// </summary>
        /// <param name="color">The bg.</param>
        /// <returns></returns>
        private static Color IdealTextColor(Color color)
        {
            const int nThreshold = 105;
            var bgDelta = Convert.ToInt32((color.R * 0.299) + (color.G * 0.587) + (color.B * 0.114));
            var foreColor = 255 - bgDelta < nThreshold
                                ? Colors.Black
                                : Colors.White;
            return foreColor;
        }

        private static SolidColorBrush GetSolidColorBrush(Color color, double opacity = 1d)
        {
            var brush = new SolidColorBrush(color)
                        {
                            Opacity = opacity
                        };
            brush.Freeze();
            return brush;
        }
    }
}