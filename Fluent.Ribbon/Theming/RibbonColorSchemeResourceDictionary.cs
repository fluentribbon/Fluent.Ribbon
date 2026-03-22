namespace Fluent.Theming;

using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;

/// <summary>
/// Custom resource dictionary for Ribbon runtime generated color schemes.
/// </summary>
public class RibbonColorSchemeResourceDictionary : ResourceDictionary
{
    /// <summary>
    /// Gets or sets the options used to generate the color scheme.
    /// </summary>
    public RibbonColorSchemeResourceDictionaryOptions? Options
    {
        get;

        init
        {
            field = value;

            this.UpdateResources();
        }
    }

    private void UpdateResources()
    {
        this.MergedDictionaries.Clear();

        if (this.Options is { } options)
        {
            this.MergedDictionaries.Add(RuntimeThemeGenerator.Current.GenerateRuntimeLibraryTheme(options.BaseColor, options.PrimaryAccentColor, options.IsHighContrast, RibbonLibraryThemeProvider.DefaultInstance)!.Resources);
        }
    }
}

/// <summary>
/// Options used to generate the color scheme in <see cref="RibbonColorSchemeResourceDictionary"/>.
/// </summary>
public class RibbonColorSchemeResourceDictionaryOptions
{
    /// <summary>
    /// The base color of the color scheme.
    /// </summary>
    public required string BaseColor { get; init; } = null!;

    /// <summary>
    /// The primary accent color of the color scheme.
    /// </summary>
    public required Color PrimaryAccentColor { get; init; }

    /// <summary>
    /// Defines whether the color scheme is high contrast.
    /// </summary>
    public bool IsHighContrast { get; init; }
}