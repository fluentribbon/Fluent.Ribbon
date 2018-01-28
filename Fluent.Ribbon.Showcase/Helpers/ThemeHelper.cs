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
        public static string CreateAppStyleBy(Color color, bool changeImmediately = false)
        {
            // create a runtime accent resource dictionary
            var resourceDictionary = new ResourceDictionary();

            resourceDictionary.Add("Fluent.Ribbon.Colors.HighlightColor", color);
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentBaseColor", color);
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor80", Color.FromArgb(204, color.R, color.G, color.B));
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor60", Color.FromArgb(153, color.R, color.G, color.B));
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor40", Color.FromArgb(102, color.R, color.G, color.B));
            resourceDictionary.Add("Fluent.Ribbon.Colors.AccentColor20", Color.FromArgb(51, color.R, color.G, color.B));

            resourceDictionary.Add("Fluent.Ribbon.Brushes.HighlightBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.HighlightColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentBaseColorBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentBaseColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush80", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor80"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush60", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor60"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush40", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.AccentColorBrush20", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));

            resourceDictionary.Add("Fluent.Ribbon.Colors.IdealForegroundColor", IdealTextColor(color));

            resourceDictionary.Add("Fluent.Ribbon.Brushes.IdealForegroundColorBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.IdealForegroundColor"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.IdealForegroundDisabledBrush", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.IdealForegroundColor"], 0.4));

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

            // WindowCommands
            resourceDictionary.Add("FFluent.Ribbon.Brushes.WindowCommands.CaptionButton.MouseOver.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor20"]));
            resourceDictionary.Add("Fluent.Ribbon.Brushes.WindowCommands.CaptionButton.Pressed.Background", GetSolidColorBrush((Color)resourceDictionary["Fluent.Ribbon.Colors.AccentColor40"]));

            // Create theme
            var resDictName = $"ApplicationAccent_{color.ToString().Replace("#", string.Empty)}.xaml";
            var fileName = Path.Combine(Path.GetTempPath(), resDictName);
            using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings
                                                           {
                                                               Indent = true,
                                                               IndentChars = "    "
                                                           }))
            {
                XamlWriter.Save(resourceDictionary, writer);
                writer.Close();
            }

            resourceDictionary = new ResourceDictionary
                                 {
                                     Source = new Uri(fileName, UriKind.Absolute)
                                 };

            var newAccent = new Accent
                            {
                                Name = resDictName,
                                Resources = resourceDictionary
                            };
            ThemeManager.AddAccent(newAccent.Name, newAccent.Resources.Source);

            // Apply theme
            if (changeImmediately)
            {
                var application = Application.Current;
                //var applicationTheme = ThemeManager.AppThemes.First(x => string.Equals(x.Name, "BaseLight"));
                // detect current application theme
                var applicationTheme = ThemeManager.DetectAppStyle(application);
                ThemeManager.ChangeAppStyle(application, newAccent, applicationTheme.Item1);
            }

            return File.ReadAllText(fileName);
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