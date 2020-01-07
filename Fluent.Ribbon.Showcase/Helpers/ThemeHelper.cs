namespace FluentTest.Helpers
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;
    using ControlzEx.Theming;
    using Fluent;
    using Fluent.Theming;

    public static class ThemeHelper
    {
        public static Tuple<string, Theme> CreateTheme(string baseColorScheme, Color accentBaseColor, Color highlightColor, string name = null, bool changeImmediately = false)
        {
            var theme = RuntimeThemeGenerator.GenerateRuntimeTheme(baseColorScheme, accentBaseColor, RibbonLibraryThemeProvider.DefaultInstance);

            // Apply theme
            if (changeImmediately)
            {
                //ThemeManager.ChangeTheme(Application.Current, newTheme);
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