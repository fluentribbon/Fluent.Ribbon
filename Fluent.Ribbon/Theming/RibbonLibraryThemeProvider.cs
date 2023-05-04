namespace Fluent.Theming;

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
    public static readonly RibbonLibraryThemeProvider DefaultInstance = new();

    /// <inheritdoc cref="LibraryThemeProvider" />
    public RibbonLibraryThemeProvider()
        : base(true)
    {
    }

    /// <inheritdoc />
    public override void FillColorSchemeValues(Dictionary<string, string> values, RuntimeThemeColorValues colorValues)
    {
        values.Add("Fluent.Ribbon.Colors.AccentBase", colorValues.AccentColor.ToString());
        values.Add("Fluent.Ribbon.Colors.Accent80", colorValues.AccentColor80.ToString());
        values.Add("Fluent.Ribbon.Colors.Accent60", colorValues.AccentColor60.ToString());
        values.Add("Fluent.Ribbon.Colors.Accent40", colorValues.AccentColor40.ToString());
        values.Add("Fluent.Ribbon.Colors.Accent20", colorValues.AccentColor20.ToString());

        values.Add("Fluent.Ribbon.Colors.Highlight", colorValues.HighlightColor.ToString());
        values.Add("Fluent.Ribbon.Colors.IdealForeground", colorValues.IdealForegroundColor.ToString());
    }
}