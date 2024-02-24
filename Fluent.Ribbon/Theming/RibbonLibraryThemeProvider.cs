namespace Fluent.Theming;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using Fluent.Helpers.ColorHelpers;

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

        var accentColor = colorValues.AccentBaseColor;
#pragma warning disable CS0618 // Type or member is obsolete
        var colorPalette = new ColorPalette(accentColor);
        this.UpdateAccentColors(values, accentColor,
            colorPalette.Palette[4], colorPalette.Palette[3], colorPalette.Palette[2],
            colorPalette.Palette[6], colorPalette.Palette[7], colorPalette.Palette[8]);
    }

    private void UpdateAccentColors(Dictionary<string, string> values, Color accentColor,
        ColorPaletteEntry accentColorLight1, ColorPaletteEntry accentColorLight2, ColorPaletteEntry accentColorLight3,
        ColorPaletteEntry accentColorDark1, ColorPaletteEntry accentColorDark2, ColorPaletteEntry accentColorDark3)
    {
        values["AccentLight1"] = accentColorLight1.Color.ToString(CultureInfo.InvariantCulture);
        values["AccentLight1.Foreground"] = accentColorLight1.ContrastColor.ToString(CultureInfo.InvariantCulture);
        values["AccentLight2"] = accentColorLight2.Color.ToString(CultureInfo.InvariantCulture);
        values["AccentLight2.Foreground"] = accentColorLight2.ContrastColor.ToString(CultureInfo.InvariantCulture);
        values["AccentLight3"] = accentColorLight3.Color.ToString(CultureInfo.InvariantCulture);
        values["AccentLight3.Foreground"] = accentColorLight3.ContrastColor.ToString(CultureInfo.InvariantCulture);
        values["AccentDark1"] = accentColorDark1.Color.ToString(CultureInfo.InvariantCulture);
        values["AccentDark1.Foreground"] = accentColorDark1.ContrastColor.ToString(CultureInfo.InvariantCulture);
        values["AccentDark2"] = accentColorDark2.Color.ToString(CultureInfo.InvariantCulture);
        values["AccentDark2.Foreground"] = accentColorDark2.ContrastColor.ToString(CultureInfo.InvariantCulture);
        values["AccentDark3"] = accentColorDark3.Color.ToString(CultureInfo.InvariantCulture);
        values["AccentDark3.Foreground"] = accentColorDark3.ContrastColor.ToString(CultureInfo.InvariantCulture);
    }
#pragma warning restore CS0618 // Type or member is obsolete
}