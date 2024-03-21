namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Fluent.Internal;

[Obsolete(Constants.InternalUsageWarning)]
public class ColorPaletteEntry
{
    public ColorPaletteEntry(Color color, IReadOnlyList<Color> contrastColors)
    {
        this.Color = color;
        this.ContrastColors = contrastColors;

        this.ContrastColor = ColorUtils.ChooseColorForContrast(contrastColors, color);
    }

    public Color Color { get; }

    public IReadOnlyList<Color> ContrastColors { get; }

    public Color ContrastColor { get; }
}