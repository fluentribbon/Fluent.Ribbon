// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591

using System;
using System.Collections.Generic;
using System.Windows.Media;
using Fluent.Internal;

[Obsolete(Constants.InternalUsageWarning)]
public static class ColorUtils
{
    public const int DefaultRoundingPrecision = 5;

    // This ignores the Alpha channel of the input color
    public static HSL RGBToHSL(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToHSL(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static HSL RGBToHSL(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
        var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
        var delta = max - min;

        double hue = 0;
        if (delta == 0)
        {
            hue = 0;
        }
        else if (max == rgb.R)
        {
            hue = 60 * (((rgb.G - rgb.B) / delta) % 6);
        }
        else if (max == rgb.G)
        {
            hue = 60 * (((rgb.B - rgb.R) / delta) + 2);
        }
        else
        {
            hue = 60 * (((rgb.R - rgb.G) / delta) + 4);
        }

        if (hue < 0)
        {
            hue += 360;
        }

        var lit = (max + min) / 2;

        double sat = 0;
        if (delta != 0)
        {
            sat = delta / (1 - Math.Abs((2 * lit) - 1));
        }

        return new HSL(hue, sat, lit, round, roundingPrecision);
    }

    public static NormalizedRGB HSLToRGB(in HSL hsl, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var c = (1 - Math.Abs((2 * hsl.L) - 1)) * hsl.S;
        var x = c * (1 - Math.Abs(((hsl.H / 60) % 2) - 1));
        var m = hsl.L - (c / 2);

        double r = 0, g = 0, b = 0;
        if (hsl.H < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (hsl.H < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (hsl.H < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (hsl.H < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (hsl.H < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else if (hsl.H < 360)
        {
            r = c;
            g = 0;
            b = x;
        }

        return new NormalizedRGB(r + m, g + m, b + m, round, roundingPrecision);
    }

    // This ignores the Alpha channel of the input color
    public static HSV RGBToHSV(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToHSV(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static HSV RGBToHSV(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var max = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
        var min = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
        var delta = max - min;

        double hue = 0;
        if (delta == 0)
        {
            hue = 0;
        }
        else if (max == rgb.R)
        {
            hue = 60 * (((rgb.G - rgb.B) / delta) % 6);
        }
        else if (max == rgb.G)
        {
            hue = 60 * (((rgb.B - rgb.R) / delta) + 2);
        }
        else
        {
            hue = 60 * (((rgb.R - rgb.G) / delta) + 4);
        }

        if (hue < 0)
        {
            hue += 360;
        }

        double sat = 0;
        if (max != 0)
        {
            sat = delta / max;
        }

        var val = max;

        return new HSV(hue, sat, val, round, roundingPrecision);
    }

    public static NormalizedRGB HSVToRGB(in HSV hsv, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var c = hsv.S * hsv.V;
        var x = c * (1 - Math.Abs(((hsv.H / 60) % 2) - 1));
        var m = hsv.V - c;

        double r = 0, g = 0, b = 0;
        if (hsv.H < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (hsv.H < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (hsv.H < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (hsv.H < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (hsv.H < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else if (hsv.H < 360)
        {
            r = c;
            g = 0;
            b = x;
        }

        return new NormalizedRGB(r + m, g + m, b + m, round, roundingPrecision);
    }

    public static LAB LCHToLAB(in LCH lch, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        // LCH lit == LAB lit
        double a = 0;
        double b = 0;
        // In chroma.js this case is handled by having the rgb -> lch conversion special case h == 0. In that case it changes h to NaN. Which then requires some NaN checks elsewhere.
        // it seems preferable to handle the case of h = 0 here
        if (lch.H != 0)
        {
            a = Math.Cos(MathUtils.DegreesToRadians(lch.H)) * lch.C;
            b = Math.Sin(MathUtils.DegreesToRadians(lch.H)) * lch.C;
        }

        return new LAB(lch.L, a, b, round, roundingPrecision);
    }

    // This discontinuity in the C parameter at 0 means that floating point errors will often result in values near 0 giving unpredictable results. 
    // EG: 0.0000001 gives a very different result than -0.0000001
    public static LCH LABToLCH(in LAB lab, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        // LCH lit == LAB lit
        var h = (MathUtils.RadiansToDegrees(Math.Atan2(lab.B, lab.A)) + 360) % 360;
        var c = Math.Sqrt((lab.A * lab.A) + (lab.B * lab.B));

        return new LCH(lab.L, c, h, round, roundingPrecision);
    }

    // This conversion uses the D65 constants for 2 degrees. That determines the constants used for the pure white point of the XYZ space of 0.95047, 1.0, 1.08883
    public static XYZ LABToXYZ(in LAB lab, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var y = (lab.L + 16.0) / 116.0;
        var x = y + (lab.A / 500.0);
        var z = y - (lab.B / 200.0);

        double LABToXYZHelper(double i)
        {
            if (i > 0.206896552)
            {
                return Math.Pow(i, 3);
            }

            return 0.12841855 * (i - 0.137931034);
        }

        x = 0.95047 * LABToXYZHelper(x);
        y = LABToXYZHelper(y);
        z = 1.08883 * LABToXYZHelper(z);

        return new XYZ(x, y, z, round, roundingPrecision);
    }

    // This conversion uses the D65 constants for 2 degrees. That determines the constants used for the pure white point of the XYZ space of 0.95047, 1.0, 1.08883
    public static LAB XYZToLAB(in XYZ xyz, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        double XYZToLABHelper(double i)
        {
            if (i > 0.008856452)
            {
                return Math.Pow(i, 1.0 / 3.0);
            }

            return (i / 0.12841855) + 0.137931034;
        }

        var x = XYZToLABHelper(xyz.X / 0.95047);
        var y = XYZToLABHelper(xyz.Y);
        var z = XYZToLABHelper(xyz.Z / 1.08883);

        var l = (116.0 * y) - 16.0;
        var a = 500.0 * (x - y);
        var b = -200.0 * (z - y);

        return new LAB(l, a, b, round, roundingPrecision);
    }

    // This ignores the Alpha channel of the input color
    public static XYZ RGBToXYZ(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToXYZ(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static XYZ RGBToXYZ(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        double RGBToXYZHelper(double i)
        {
            if (i <= 0.04045)
            {
                return i / 12.92;
            }

            return Math.Pow((i + 0.055) / 1.055, 2.4);
        }

        var r = RGBToXYZHelper(rgb.R);
        var g = RGBToXYZHelper(rgb.G);
        var b = RGBToXYZHelper(rgb.B);

        var x = (r * 0.4124564) + (g * 0.3575761) + (b * 0.1804375);
        var y = ((r * 0.2126729) + (g * 0.7151522)) + (b * 0.0721750);
        var z = ((r * 0.0193339) + (g * 0.1191920)) + (b * 0.9503041);

        return new XYZ(x, y, z, round, roundingPrecision);
    }

    public static NormalizedRGB XYZToRGB(in XYZ xyz, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        double XYZToRGBHelper(double i)
        {
            if (i <= 0.0031308)
            {
                return i * 12.92;
            }

            return (1.055 * Math.Pow(i, 1 / 2.4)) - 0.055;
        }

        var r = XYZToRGBHelper(((xyz.X * 3.2404542) - (xyz.Y * 1.5371385)) - (xyz.Z * 0.4985314));
        var g = XYZToRGBHelper((xyz.X * -0.9692660) + (xyz.Y * 1.8760108) + (xyz.Z * 0.0415560));
        var b = XYZToRGBHelper((xyz.X * 0.0556434) - (xyz.Y * 0.2040259) + (xyz.Z * 1.0572252));

        return new NormalizedRGB(MathUtils.ClampToUnit(r), MathUtils.ClampToUnit(g), MathUtils.ClampToUnit(b), round, roundingPrecision);
    }

    // This ignores the Alpha channel of the input color
    public static LAB RGBToLAB(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToLAB(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static LAB RGBToLAB(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var xyz = RGBToXYZ(rgb, false);
        return XYZToLAB(xyz, round, roundingPrecision);
    }

    public static NormalizedRGB LABToRGB(in LAB lab, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var xyz = LABToXYZ(lab, false);
        return XYZToRGB(xyz, round, roundingPrecision);
    }

    // This ignores the Alpha channel of the input color
    public static LCH RGBToLCH(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToLCH(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static LCH RGBToLCH(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        // The discontinuity near 0 in LABToLCH means we should round here even if the bound param is false
        var lab = RGBToLAB(rgb, true, 4);

        // This appears redundant but is actually nescessary in order to prevent floating point rounding errors from throwing off the Atan2 computation in LABToLCH
        // https://msdn.microsoft.com/en-us/library/system.math.atan2(v=vs.110).aspx
        // For the RGB value 255,255,255 what happens is the a value appears to be rounded to 0 but is still treated as negative by Atan2 which then returns PI instead of 0

        var l = lab.L == 0
            ? 0
            : lab.L;
        var a = lab.A == 0
            ? 0
            : lab.A;
        var b = lab.B == 0
            ? 0
            : lab.B;

        return LABToLCH(new LAB(l, a, b, false), round, roundingPrecision);
    }

    public static NormalizedRGB LCHToRGB(in LCH lch, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var lab = LCHToLAB(lch, false);
        return LABToRGB(lab, round, roundingPrecision);
    }

    public static NormalizedRGB TemperatureToRGB(double tempKelvin, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        // The constants I could find assumed a decimal range of [0,255] for each channel. Just going to put a /255.0 at the end
        double r = 0.0, g = 0.0, b = 0.0;

        if (tempKelvin < 6600.0)
        {
            r = 255.0;

            g = (tempKelvin / 100.0) - 2.0;
            g = (-155.25485562709179 - (0.44596950469579133 * g)) + (104.49216199393888 * Math.Log(g));
        }
        else
        {
            r = (tempKelvin / 100.0) - 55.0;
            r = 351.97690566805693 + (0.114206453784165 * r) - (40.25366309332127 * Math.Log(r));

            g = (tempKelvin / 100.0) - 50.0;
            g = (325.4494125711974 + (0.07943456536662342 * g)) - (28.0852963507957 * Math.Log(g));
        }

        if (tempKelvin >= 6600.0)
        {
            b = 255.0;
        }
        else if (tempKelvin < 2000.0)
        {
            b = 0.0;
        }
        else
        {
            b = (tempKelvin / 100.0) - 10;
            b = -254.76935184120902 + (0.8274096064007395 * b) + (115.67994401066147 * Math.Log(b));
        }

        return new NormalizedRGB(r / 255.0, g / 255.0, b / 255.0, round, roundingPrecision);
    }

    // This ignores the Alpha channel of the input color
    public static double RGBToTemperature(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToTemperature(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static double RGBToTemperature(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        double t = 0;
        var min = 1000.0;
        var max = 40000.0;
        while (max - min > 0.4)
        {
            t = (max + min) / 2.0;
            var testColor = TemperatureToRGB(t, false);
            if (testColor.B / testColor.R >= rgb.B / rgb.R)
            {
                max = t;
            }
            else
            {
                min = t;
            }
        }

        if (round)
        {
            return Math.Round(t, roundingPrecision);
        }

        return t;
    }

    // This ignores the Alpha channel of the input color
    public static double RGBToLuminance(Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return RGBToLuminance(new NormalizedRGB(rgb, false), round, roundingPrecision);
    }

    public static double RGBToLuminance(in NormalizedRGB rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        double LuminanceHelper(double i)
        {
            if (i <= 0.03928)
            {
                return i / 12.92;
            }

            return Math.Pow((i + 0.055) / 1.055, 2.4);
        }

        var r = LuminanceHelper(rgb.R);
        var g = LuminanceHelper(rgb.G);
        var b = LuminanceHelper(rgb.B);

        // More accurate constants would be helpful here
        var l = (r * 0.2126) + (g * 0.7152) + (b * 0.0722);

        if (round)
        {
            return Math.Round(l, roundingPrecision);
        }

        return l;
    }

    // This ignores the Alpha channel of both input colors
    public static double ContrastRatio(Color a, Color b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        return ContrastRatio(new NormalizedRGB(a, false), new NormalizedRGB(b, false), round, roundingPrecision);
    }

    public static double ContrastRatio(in NormalizedRGB a, in NormalizedRGB b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        var la = RGBToLuminance(a, false);
        var lb = RGBToLuminance(b, false);
        double retVal = 0;
        if (la > lb)
        {
            retVal = (la + 0.05) / (lb + 0.05);
        }
        else
        {
            retVal = (lb + 0.05) / (la + 0.05);
        }

        if (round)
        {
            return Math.Round(retVal, roundingPrecision);
        }

        return retVal;
    }

    // Returns the Color from colorOptions which has the best contrast ratio with background
    // in the case of ties the first color to appear in the list will be chosen
    // alpha channels are ignored
    public static Color ChooseColorForContrast(IEnumerable<Color> colorOptions, Color background)
    {
        if (colorOptions == null)
        {
            throw new ArgumentNullException(nameof(colorOptions));
        }

        var normalizedBackground = new NormalizedRGB(background, false);

        var bestColor = default(Color);
        double bestRatio = 0;
        foreach (var color in colorOptions)
        {
            var normalizedColor = new NormalizedRGB(color, false);
            var ratio = ContrastRatio(normalizedColor, normalizedBackground, false);
            if (ratio > bestRatio)
            {
                bestColor = color;
                bestRatio = ratio;
            }
        }

        return bestColor;
    }

    public static Color InterpolateRGB(Color left, Color right, double position)
    {
        if (position <= 0)
        {
            return left;
        }

        if (position >= 1)
        {
            return right;
        }

        return Color.FromArgb(MathUtils.Lerp(left.A, right.A, position), MathUtils.Lerp(left.R, right.R, position), MathUtils.Lerp(left.G, right.G, position), MathUtils.Lerp(left.B, right.B, position));
    }

    public static NormalizedRGB InterpolateRGB(in NormalizedRGB left, in NormalizedRGB right, double position)
    {
        if (position <= 0)
        {
            return left;
        }

        if (position >= 1)
        {
            return right;
        }

        return new NormalizedRGB(MathUtils.Lerp(left.R, right.R, position), MathUtils.Lerp(left.G, right.G, position), MathUtils.Lerp(left.B, right.B, position), false);
    }

    // Generally looks better than RGB for interpolation
    public static LAB InterpolateLAB(in LAB left, in LAB right, double position)
    {
        if (position <= 0)
        {
            return left;
        }

        if (position >= 1)
        {
            return right;
        }

        return new LAB(MathUtils.Lerp(left.L, right.L, position), MathUtils.Lerp(left.A, right.A, position), MathUtils.Lerp(left.B, right.B, position), false);
    }

    // Possibly a better choice than LAB for very dark colors
    public static XYZ InterpolateXYZ(in XYZ left, in XYZ right, double position)
    {
        if (position <= 0)
        {
            return left;
        }

        if (position >= 1)
        {
            return right;
        }

        return new XYZ(MathUtils.Lerp(left.X, right.X, position), MathUtils.Lerp(left.Y, right.Y, position), MathUtils.Lerp(left.Z, right.Z, position), false);
    }

    public static NormalizedRGB InterpolateColor(in NormalizedRGB left, in NormalizedRGB right, double position, ColorScaleInterpolationMode mode)
    {
        switch (mode)
        {
            case ColorScaleInterpolationMode.LAB:
                var leftLAB = RGBToLAB(left, false);
                var rightLAB = RGBToLAB(right, false);
                return LABToRGB(InterpolateLAB(leftLAB, rightLAB, position));
            case ColorScaleInterpolationMode.XYZ:
                var leftXYZ = RGBToXYZ(left, false);
                var rightXYZ = RGBToXYZ(right, false);
                return XYZToRGB(InterpolateXYZ(leftXYZ, rightXYZ, position));
            default:
                return InterpolateRGB(left, right, position);
        }
    }
}