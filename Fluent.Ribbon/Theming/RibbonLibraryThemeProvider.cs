namespace Fluent.Theming
{
    using System.Collections.Generic;
    using ControlzEx.Theming;

    /// <summary>
    /// Provides theme resources from Fluent.Ribbon.
    /// </summary>
    public class RibbonLibraryThemeProvider : LibraryThemeProvider
    {
        /// <summary>
        /// Gets the default instance of this class.
        /// </summary>
        public static readonly RibbonLibraryThemeProvider DefaultInstance = new RibbonLibraryThemeProvider();

        /// <inheritdoc cref="LibraryThemeProvider" />
        public RibbonLibraryThemeProvider()
            : base(true)
        {
        }

        /// <inheritdoc />
        public override void FillColorSchemeValues(Dictionary<string, string> values, RuntimeThemeColorValues colorValues)
        {
            values.Add("Fluent.Ribbon.Colors.AccentBaseColor", colorValues.AccentColor.ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor80", colorValues.AccentColor80.ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor60", colorValues.AccentColor60.ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor40", colorValues.AccentColor40.ToString());
            values.Add("Fluent.Ribbon.Colors.AccentColor20", colorValues.AccentColor20.ToString());

            values.Add("Fluent.Ribbon.Colors.HighlightColor", colorValues.HighlightColor.ToString());
            values.Add("Fluent.Ribbon.Colors.IdealForegroundColor", colorValues.IdealForegroundColor.ToString());
        }
    }
}