// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591
using System;
using System.Collections.Generic;
using System.Windows.Media;
using Fluent.Internal;

// https://learn.microsoft.com/en-us/windows/apps/design/style/color
[Obsolete(Constants.InternalUsageWarning)]
public class ColorPalette
{
    private ColorPaletteEntry[] palette;

    public ColorPalette(Color baseColor)
        : this(baseColor, new[] { Colors.White, Colors.Black })
    {
    }

    public ColorPalette(Color baseColor, IReadOnlyList<Color> contrastColors)
    {
        this.BaseColor = baseColor;
        this.ContrastColors = contrastColors;
        this.palette = new ColorPaletteEntry[this.Steps];

        this.UpdatePaletteColors();
    }

    public Color BaseColor { get; set; }

    public int Steps { get; set; } = 11;

    public double ClipDark { get; set; } = 0.160;

    public double ClipLight { get; set; } = 0.185;

    public ColorScaleInterpolationMode InterpolationMode { get; set; } = ColorScaleInterpolationMode.RGB;

    public double MultiplyDark { get; set; } = 0.0;

    public double MultiplyLight { get; set; } = 0.0;

    public double OverlayDark { get; set; } = 0.25;

    public double OverlayLight { get; set; } = 0.0;

    public double SaturationAdjustmentCutoff { get; set; } = 0.05;

    public double SaturationDark { get; set; } = 1.25;

    public double SaturationLight { get; set; } = 0.35;

    public Color ScaleColorDark { get; set; } = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);

    public Color ScaleColorLight { get; set; } = Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF);

    public IReadOnlyList<Color> ContrastColors { get; set; }

    public IReadOnlyList<ColorPaletteEntry> Palette => this.palette;

    public ColorScale GetPaletteScale()
    {
        var baseColorRGB = this.BaseColor;
        var baseColorHSL = ColorUtils.RGBToHSL(baseColorRGB);
        var baseColorNormalized = new NormalizedRGB(baseColorRGB);

        var baseScale = new ColorScale(new[]
        {
            this.ScaleColorLight,
            baseColorRGB,
            this.ScaleColorDark
        });

        var trimmedScale = baseScale.Trim(this.ClipLight, 1.0 - this.ClipDark);
        var trimmedLight = new NormalizedRGB(trimmedScale.GetColor(0, this.InterpolationMode));
        var trimmedDark = new NormalizedRGB(trimmedScale.GetColor(1, this.InterpolationMode));

        var adjustedLight = trimmedLight;
        var adjustedDark = trimmedDark;

        if (baseColorHSL.S >= this.SaturationAdjustmentCutoff)
        {
            adjustedLight = ColorBlending.SaturateViaLCH(adjustedLight, this.SaturationLight);
            adjustedDark = ColorBlending.SaturateViaLCH(adjustedDark, this.SaturationDark);
        }

        if (this.MultiplyLight != 0)
        {
            var multiply = ColorBlending.Blend(baseColorNormalized, adjustedLight, ColorBlendMode.Multiply);
            adjustedLight = ColorUtils.InterpolateColor(adjustedLight, multiply, this.MultiplyLight, this.InterpolationMode);
        }

        if (this.MultiplyDark != 0)
        {
            var multiply = ColorBlending.Blend(baseColorNormalized, adjustedDark, ColorBlendMode.Multiply);
            adjustedDark = ColorUtils.InterpolateColor(adjustedDark, multiply, this.MultiplyDark, this.InterpolationMode);
        }

        if (this.OverlayLight != 0)
        {
            var overlay = ColorBlending.Blend(baseColorNormalized, adjustedLight, ColorBlendMode.Overlay);
            adjustedLight = ColorUtils.InterpolateColor(adjustedLight, overlay, this.OverlayLight, this.InterpolationMode);
        }

        if (this.OverlayDark != 0)
        {
            var overlay = ColorBlending.Blend(baseColorNormalized, adjustedDark, ColorBlendMode.Overlay);
            adjustedDark = ColorUtils.InterpolateColor(adjustedDark, overlay, this.OverlayDark, this.InterpolationMode);
        }

        var finalScale = new ColorScale(new[]
        {
            adjustedLight.Denormalize(),
            baseColorRGB,
            adjustedDark.Denormalize()
        });
        return finalScale;
    }

    public void UpdatePaletteColors()
    {
        this.palette = new ColorPaletteEntry[this.Steps];
        var scale = this.GetPaletteScale();

        for (var i = 0; i < this.Steps; i++)
        {
            var color = scale.GetColor(i / (double)(this.Steps - 1), this.InterpolationMode);
            this.palette[i] = new ColorPaletteEntry(color, this.ContrastColors);
        }
    }
}