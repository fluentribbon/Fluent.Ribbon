// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Fluent.Internal;

[Obsolete(Constants.InternalUsageWarning)]
public class ColorScale
{
    private readonly ColorScaleStop[] stops;

    // Evenly distributes the colors provided between 0 and 1
    public ColorScale(IReadOnlyCollection<Color> colors)
    {
        if (colors == null)
        {
            throw new ArgumentNullException(nameof(colors));
        }

        var count = colors.Count;
        this.stops = new ColorScaleStop[count];
        var index = 0;
        foreach (var color in colors)
        {
            // Clean up floating point jaggies
            if (index == 0)
            {
                this.stops[index] = new ColorScaleStop(color, 0);
            }
            else if (index == count - 1)
            {
                this.stops[index] = new ColorScaleStop(color, 1);
            }
            else
            {
                this.stops[index] = new ColorScaleStop(color, index * (1.0 / (count - 1)));
            }

            index++;
        }
    }

    public ColorScale(IReadOnlyCollection<ColorScaleStop> stops)
    {
        if (stops == null)
        {
            throw new ArgumentNullException(nameof(stops));
        }

        var count = stops.Count;
        this.stops = new ColorScaleStop[count];
        var index = 0;
        foreach (var stop in stops)
        {
            this.stops[index] = new ColorScaleStop(stop);
            index++;
        }
    }

    public ColorScale(ColorScale source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        this.stops = new ColorScaleStop[source.stops.Length];
        for (var i = 0; i < this.stops.Length; i++)
        {
            this.stops[i] = new ColorScaleStop(source.stops[i]);
        }
    }

    public Color GetColor(double position, ColorScaleInterpolationMode mode = ColorScaleInterpolationMode.RGB)
    {
        if (this.stops.Length == 1)
        {
            return this.stops[0].Color;
        }

        if (position <= 0)
        {
            return this.stops[0].Color;
        }

        if (position >= 1)
        {
            return this.stops[this.stops.Length - 1].Color;
        }

        var lowerIndex = 0;
        for (var i = 0; i < this.stops.Length; i++)
        {
            if (this.stops[i].Position <= position)
            {
                lowerIndex = i;
            }
        }

        var upperIndex = lowerIndex + 1;
        if (upperIndex >= this.stops.Length)
        {
            upperIndex = this.stops.Length - 1;
        }

        var scalePosition = (position - this.stops[lowerIndex].Position) * (1.0 / (this.stops[upperIndex].Position - this.stops[lowerIndex].Position));

        switch (mode)
        {
            case ColorScaleInterpolationMode.LAB:
                var leftLAB = ColorUtils.RGBToLAB(this.stops[lowerIndex].Color, false);
                var rightLAB = ColorUtils.RGBToLAB(this.stops[upperIndex].Color, false);
                var targetLAB = ColorUtils.InterpolateLAB(leftLAB, rightLAB, scalePosition);
                return ColorUtils.LABToRGB(targetLAB, false).Denormalize();
            case ColorScaleInterpolationMode.XYZ:
                var leftXYZ = ColorUtils.RGBToXYZ(this.stops[lowerIndex].Color, false);
                var rightXYZ = ColorUtils.RGBToXYZ(this.stops[upperIndex].Color, false);
                var targetXYZ = ColorUtils.InterpolateXYZ(leftXYZ, rightXYZ, scalePosition);
                return ColorUtils.XYZToRGB(targetXYZ, false).Denormalize();
            default:
                return ColorUtils.InterpolateRGB(this.stops[lowerIndex].Color, this.stops[upperIndex].Color, scalePosition);
        }
    }

    public ColorScale Trim(double lowerBound, double upperBound, ColorScaleInterpolationMode mode = ColorScaleInterpolationMode.RGB)
    {
        if (lowerBound < 0
            || upperBound > 1
            || upperBound < lowerBound)
        {
            throw new ArgumentException("Invalid bounds");
        }

        if (lowerBound == upperBound)
        {
            return new ColorScale(new[]
            {
                this.GetColor(lowerBound, mode)
            });
        }

        var containedStops = new List<ColorScaleStop>(this.stops.Length);

        for (var i = 0; i < this.stops.Length; i++)
        {
            if (this.stops[i].Position >= lowerBound
                && this.stops[i].Position <= upperBound)
            {
                containedStops.Add(this.stops[i]);
            }
        }

        if (containedStops.Count == 0)
        {
            return new ColorScale(new[]
            {
                this.GetColor(lowerBound, mode),
                this.GetColor(upperBound, mode)
            });
        }

        if (containedStops.First().Position != lowerBound)
        {
            containedStops.Insert(0, new ColorScaleStop(this.GetColor(lowerBound, mode), lowerBound));
        }

        if (containedStops.Last().Position != upperBound)
        {
            containedStops.Add(new ColorScaleStop(this.GetColor(upperBound, mode), upperBound));
        }

        var range = upperBound - lowerBound;
        var finalStops = new ColorScaleStop[containedStops.Count];
        for (var i = 0; i < finalStops.Length; i++)
        {
            var adjustedPosition = (containedStops[i].Position - lowerBound) / range;
            finalStops[i] = new ColorScaleStop(containedStops[i].Color, adjustedPosition);
        }

        return new ColorScale(finalStops);
    }
}