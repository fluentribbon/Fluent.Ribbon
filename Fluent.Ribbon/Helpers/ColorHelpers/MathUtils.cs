// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591

using System;
using System.Windows;
using Fluent.Internal;

[Obsolete(Constants.InternalUsageWarning)]
public static class MathUtils
{
    public static byte ClampToByte(double c)
    {
        if (double.IsNaN(c))
        {
            return 0;
        }

        if (double.IsPositiveInfinity(c))
        {
            return 255;
        }

        if (double.IsNegativeInfinity(c))
        {
            return 0;
        }

        c = Math.Round(c);
        if (c <= 0)
        {
            return 0;
        }

        if (c >= 255)
        {
            return 255;
        }

        return (byte)c;
    }

    public static double ClampToUnit(double c)
    {
        if (double.IsNaN(c))
        {
            return 0;
        }

        if (double.IsPositiveInfinity(c))
        {
            return 1;
        }

        if (double.IsNegativeInfinity(c))
        {
            return 0;
        }

        if (c <= 0)
        {
            return 0;
        }

        if (c >= 1)
        {
            return 1;
        }

        return c;
    }

    public static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180.0);
    }

    public static double RadiansToDegrees(double radians)
    {
        return radians * (180.0 / Math.PI);
    }

    public static double Lerp(double left, double right, double scale)
    {
        if (scale <= 0)
        {
            return left;
        }

        if (scale >= 1)
        {
            return right;
        }

        return left + (scale * (right - left));
    }

    public static byte Lerp(byte left, byte right, double scale)
    {
        if (scale <= 0)
        {
            return left;
        }

        if (scale >= 1)
        {
            return right;
        }

        if (left == right)
        {
            return left;
        }

        double l = left;
        double r = right;
        return (byte)Math.Round(l + (scale * (r - l)));
    }

    public static double LerpAnglesInDegrees(double left, double right, double scale)
    {
        if (scale <= 0)
        {
            return left;
        }

        if (scale >= 1)
        {
            return right;
        }

        var a = (left - right + 360.0) % 360.0;
        var b = (right - left + 360.0) % 360.0;
        if (a <= b)
        {
            return (left - (a * scale) + 360.0) % 360.0;
        }

        return (left + (a * scale) + 360.0) % 360.0;
    }

    public static double Interpolate(double start, double end, double progress, Func<double, double>? easing = null)
    {
        if (progress <= 0)
        {
            return start;
        }

        if (progress >= 1)
        {
            return end;
        }

        if (easing != null)
        {
            progress = easing(progress);
        }

        return start + ((end - start) * progress);
    }

    public static Size Interpolate(in Size start, in Size end, double progress, Func<double, double>? easing = null)
    {
        if (progress <= 0)
        {
            return start;
        }

        if (progress >= 1)
        {
            return end;
        }

        if (easing != null)
        {
            progress = easing(progress);
        }

        return new Size(start.Width + ((end.Width - start.Width) * progress), start.Height + ((end.Height - start.Height) * progress));
    }

    public static Rect Interpolate(in Rect start, in Rect end, double progress, Func<double, double>? easing = null)
    {
        if (progress <= 0)
        {
            return start;
        }

        if (progress >= 1)
        {
            return end;
        }

        if (easing != null)
        {
            progress = easing(progress);
        }

        return new Rect(start.X + ((end.X - start.X) * progress),
            start.Y + ((end.Y - start.Y) * progress),
            start.Width + ((end.Width - start.Width) * progress),
            start.Height + ((end.Height - start.Height) * progress));
    }

    public static double LinearEasing(double input)
    {
        return input;
    }

    public static double QuadraticEasing(double input)
    {
        return input * input;
    }

    public static double CubicEasing(double input)
    {
        return input * input * input;
    }

    public static double CubicBezierEasing(double x, in Point control1, in Point control2, int iterations = 4)
    {
        if (control1.X == control1.Y
            && control2.X == control2.Y)
        {
            return x;
        }

        var t = CubicBezierApproximateT(x, control1.X, control2.X, iterations);
        return CubicBezier(t, control1, control2).Y;
    }

    // Uses Newton Raphson method
    public static double CubicBezierApproximateT(double x, double control1, double control2, int iterations = 4)
    {
        if (x <= 0.0)
        {
            return 0.0;
        }

        if (x >= 1.0)
        {
            return 1.0;
        }

        var t = x;
        for (var i = 0; i < iterations; i++)
        {
            var slope = CubicBezierFirstDerivative(t, control1, control2);
            if (slope == 0.0)
            {
                return t;
            }

            var xp = CubicBezier(t, control1, control2) - x;
            t -= xp / slope;
        }

        return t;
    }

    public static Point CubicBezier(double t, in Point start, in Point control1, in Point control2, in Point end)
    {
        return new Point(CubicBezier(t, start.X, control1.X, control2.X, end.X), CubicBezier(t, start.Y, control1.Y, control2.Y, end.Y));
    }

    // Special case used in easing functions where start = 0,0 and end = 1,1
    public static Point CubicBezier(double t, in Point control1, in Point control2)
    {
        return new Point(CubicBezier(t, control1.X, control2.X), CubicBezier(t, control1.Y, control2.Y));
    }

    public static double CubicBezier(double t, double start, double control1, double control2, double end)
    {
        return (start * Math.Pow(1.0 - t, 3)) + (control1 * 3.0 * t * Math.Pow(1.0 - t, 2)) + (control2 * 3.0 * t * t * (1.0 - t)) + (t * t * t * end);
    }

    // Special case used in easing functions where start = 0 and end = 1
    public static double CubicBezier(double t, double control1, double control2)
    {
        return (control1 * 3.0 * t * Math.Pow(1.0 - t, 2)) + (control2 * 3.0 * t * t * (1.0 - t)) + (t * t * t);
    }

    public static double CubicBezierFirstDerivative(double t, double start, double control1, double control2, double end)
    {
        return (3.0 * Math.Pow(1.0 - t, 2) * (control1 - start)) + (6.0 * t * (1.0 - t) * (control2 - control1)) + (3.0 * t * t * (end - control2));
    }

    // Special case used in easing functions where start = 0 and end = 1
    public static double CubicBezierFirstDerivative(double t, double control1, double control2)
    {
        return (3.0 * Math.Pow(1.0 - t, 2) * control1) + (6.0 * t * (1.0 - t) * (control2 - control1)) + (3.0 * t * t * (1.0 - control2));
    }

    public static double CubicBezierSecondDerivative(double t, double start, double control1, double control2, double end)
    {
        return (6.0 * (1 - t) * (control2 - (2.0 * control1) + start)) + (6.0 * t * (end - (2.0 * control2) + control1));
    }

    // Special case used in easing functions where start = 0 and end = 1
    public static double CubicBezierSecondDerivative(double t, double control1, double control2)
    {
        return (6.0 * (1 - t) * (control2 - (2.0 * control1))) + (6.0 * t * (1.0 - (2.0 * control2) + control1));
    }

    public static double ComputeMean(double[] values, double[]? weights = null)
    {
        if (values == null
            || values.Length == 0)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (weights != null
            && weights.Length != values.Length)
        {
            throw new Exception("The weights array must be either null or the same length as values");
        }

        var totalWeight = 0.0;
        var total = 0.0;
        for (var i = 0; i < values.Length; i++)
        {
            if (weights == null)
            {
                total += values[i];
                totalWeight++;
            }
            else
            {
                total += values[i] * weights[i];
                totalWeight += weights[i];
            }
        }

        if (totalWeight == 0.0)
        {
            return 0.0;
        }

        return total / totalWeight;
    }
}