namespace FluentTest.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;
    using ControlzEx.Theming;

    public static class ThemeHelper
    {
        public static Tuple<string, Theme> CreateTheme(string baseColorScheme, Color accentBaseColor, bool changeImmediately = false)
        {
            var theme = RuntimeThemeGenerator.Current.GenerateRuntimeTheme(baseColorScheme, accentBaseColor, false);

            // Apply theme
            if (changeImmediately)
            {
                var changedTheme = ThemeManager.Current.ChangeTheme(Application.Current, theme);

                Debug.Assert(changedTheme == theme, "Theme must have been changed.");
            }

            return new Tuple<string, Theme>(string.Join(Environment.NewLine, theme.GetAllResources().Select(GetResourceDictionaryContent)), theme);
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
    }
}