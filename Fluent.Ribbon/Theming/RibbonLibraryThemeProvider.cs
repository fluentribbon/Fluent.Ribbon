namespace Fluent.Theming
{
    using System.Collections.Generic;
    using System.Windows.Media;
    using ControlzEx.Theming;

#pragma warning disable CS1591

    /// <summary>
    /// Provides theme resources from Fluent.Ribbon.
    /// </summary>
    public class RibbonLibraryThemeProvider : LibraryThemeProvider
    {
        public static readonly RibbonLibraryThemeProvider DefaultInstance = new RibbonLibraryThemeProvider();

        /// <inheritdoc cref="LibraryThemeProvider" />
        public RibbonLibraryThemeProvider()
            : base(true)
        {
        }

        /// <inheritdoc />
        public override void FillColorSchemeValues(Dictionary<string, string> values, Color accentColor, Color accentColor80Percent, Color accentColor60Percent, Color accentColor40Percent, Color accentColor20Percent, Color highlightColor, Color idealForegroundColor)
        {
            values.Add("ControlzEx.Colors.AccentBaseColor", accentColor.ToString());
            values.Add("ControlzEx.Colors.AccentColor80", accentColor.ToString());
            values.Add("ControlzEx.Colors.AccentColor60", accentColor60Percent.ToString());
            values.Add("ControlzEx.Colors.AccentColor40", accentColor40Percent.ToString());
            values.Add("ControlzEx.Colors.AccentColor20", accentColor20Percent.ToString());

            values.Add("ControlzEx.Colors.HighlightColor", highlightColor.ToString());
            values.Add("ControlzEx.Colors.IdealForegroundColor", idealForegroundColor.ToString());
        }
    }
}