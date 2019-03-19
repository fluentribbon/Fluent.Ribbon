namespace FluentTest.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;
    using Fluent;
    using Newtonsoft.Json;
    using XamlColorSchemeGenerator;

    public static class ThemeHelper
    {
        public static Tuple<string, ResourceDictionary> CreateTheme(string baseColorScheme, Color accentBaseColor, Color highlightColor, string name = null, bool changeImmediately = false)
        {
            name = name ?? $"RuntimeTheme_{baseColorScheme}_{accentBaseColor.ToString().Replace("#", string.Empty)}";

            var generatorParameters = GetGeneratorParameters();
            var themeTemplateContent = GetThemeTemplateContent();

            var variant = generatorParameters.BaseColorSchemes.First(x => x.Name == baseColorScheme);
            var colorScheme = new XamlColorSchemeGenerator.ColorScheme
                              {
                                  Name = accentBaseColor.ToString().Replace("#", string.Empty)
                              };
            var values = colorScheme.Values;
            values.Add("Fluent.Ribbon.Colors.AccentBaseColor", accentBaseColor.ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor80", Color.FromArgb(204, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor60", Color.FromArgb(153, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor40", Color.FromArgb(102, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor20", Color.FromArgb(51, accentBaseColor.R, accentBaseColor.G, accentBaseColor.B).ToString());

            values.Add("Fluent.Ribbon.Colors.HighlightColor", highlightColor.ToString());
            values.Add("Fluent.Ribbon.Colors.IdealForegroundColor", IdealTextColor(accentBaseColor).ToString());

            var xamlContent = new ColorSchemeGenerator().GenerateColorSchemeFileContent(generatorParameters, variant, colorScheme, themeTemplateContent, name, name);

            var resourceDictionary = (ResourceDictionary)XamlReader.Parse(xamlContent);

            var newTheme = new Theme(resourceDictionary);

            ThemeManager.AddTheme(newTheme.Resources);

            // Apply theme
            if (changeImmediately)
            {
                ThemeManager.ChangeTheme(Application.Current, newTheme);
            }

            return new Tuple<string, ResourceDictionary>(xamlContent, resourceDictionary);
        }

        public static string GetResourceDictionaryContent(ResourceDictionary resourceDictionary)
        {
            using (var sw = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sw, new XmlWriterSettings
                                                               {
                                                                   Indent = true,
                                                                   IndentChars = "    "
                                                               }))
                {
                    XamlWriter.Save(resourceDictionary, writer);

                    return sw.ToString();
                }
            }
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

        private static string GetThemeTemplateContent()
        {
            using (var stream = typeof(ThemeManager).Assembly.GetManifestResourceStream("Fluent.Themes.Themes.Theme.Template.xaml"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static GeneratorParameters GetGeneratorParameters()
        {
            return JsonConvert.DeserializeObject<GeneratorParameters>(GetGeneratorParametersJson());
        }

        private static string GetGeneratorParametersJson()
        {
            using (var stream = typeof(ThemeManager).Assembly.GetManifestResourceStream("Fluent.Themes.Themes.GeneratorParameters.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}