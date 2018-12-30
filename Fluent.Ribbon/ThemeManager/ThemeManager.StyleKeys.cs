// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that allows for the detection and alteration of a theme.
    /// </summary>
    public static partial class ThemeManager
    {
        // Note: add more checks if these keys aren't sufficient
        private static readonly List<string> styleKeys = new List<string>(new[]
                                                                          {
                                                                              "Fluent.Ribbon.Colors.HighlightColor",
                                                                              "Fluent.Ribbon.Colors.AccentBaseColor",
                                                                              "Fluent.Ribbon.Colors.AccentColor",
                                                                              "Fluent.Ribbon.Colors.AccentColor20",
                                                                              "Fluent.Ribbon.Colors.AccentColor40",
                                                                              "Fluent.Ribbon.Colors.AccentColor60",
                                                                              "Fluent.Ribbon.Colors.AccentColor80",
                                                                              "Fluent.Ribbon.Brushes.HighlightBrush",
                                                                              "Fluent.Ribbon.Brushes.AccentBaseColorBrush",
                                                                              "Fluent.Ribbon.Brushes.AccentColorBrush",
                                                                              "Fluent.Ribbon.Brushes.AccentColorBrush20",
                                                                              "Fluent.Ribbon.Brushes.AccentColorBrush40",
                                                                              "Fluent.Ribbon.Brushes.AccentColorBrush60",
                                                                              "Fluent.Ribbon.Brushes.AccentColorBrush80",
                                                                          });
    }
}