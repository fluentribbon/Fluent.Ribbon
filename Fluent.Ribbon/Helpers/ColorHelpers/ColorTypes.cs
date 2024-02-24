// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591

using System;
using System.Windows.Media;
using Fluent.Internal;

/// <summary>
/// Valid values for each channel are ∈ [0.0,1.0]
/// But sometimes it is useful to allow values outside that range during calculations as long as they are clamped eventually 
/// </summary>
[Obsolete(Constants.InternalUsageWarning)]
public readonly struct NormalizedRGB : IEquatable<NormalizedRGB>
{
    public const int DefaultRoundingPrecision = 5;

    public NormalizedRGB(double r, double g, double b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.R = Math.Round(r, roundingPrecision);
            this.G = Math.Round(g, roundingPrecision);
            this.B = Math.Round(b, roundingPrecision);
        }
        else
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }
    }

    public NormalizedRGB(in Color rgb, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.R = Math.Round(rgb.R / 255.0, roundingPrecision);
            this.G = Math.Round(rgb.G / 255.0, roundingPrecision);
            this.B = Math.Round(rgb.B / 255.0, roundingPrecision);
        }
        else
        {
            this.R = rgb.R / 255.0;
            this.G = rgb.G / 255.0;
            this.B = rgb.B / 255.0;
        }
    }

    public Color Denormalize(byte a = 255)
    {
        return Color.FromArgb(a, MathUtils.ClampToByte(this.R * 255.0), MathUtils.ClampToByte(this.G * 255.0), MathUtils.ClampToByte(this.B * 255.0));
    }

    public readonly double R;
    public readonly double G;
    public readonly double B;

    #region IEquatable<NormalizedRGB>

    public bool Equals(NormalizedRGB other)
    {
        return this.R == other.R && this.G == other.G && this.B == other.B;
    }

    #endregion

    #region Equals

    public override bool Equals(object? obj)
    {
        if (obj is NormalizedRGB other)
        {
            return this.Equals(other);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.R.GetHashCode() ^ this.G.GetHashCode() ^ this.B.GetHashCode();
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        return $"{this.R},{this.G},{this.B}";
    }

    #endregion
}

// H ∈ [0.0,360.0]
// S ∈ [0.0,1.0]
// L ∈ [0.0,1.0]
[Obsolete(Constants.InternalUsageWarning)]
public readonly struct HSL : IEquatable<HSL>
{
    public const int DefaultRoundingPrecision = 5;

    public HSL(double h, double s, double l, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.H = Math.Round(h, roundingPrecision);
            this.S = Math.Round(s, roundingPrecision);
            this.L = Math.Round(l, roundingPrecision);
        }
        else
        {
            this.H = h;
            this.S = s;
            this.L = l;
        }
    }

    public readonly double H;
    public readonly double S;
    public readonly double L;

    #region IEquatable<HSL>

    public bool Equals(HSL other)
    {
        return this.H == other.H && this.S == other.S && this.L == other.L;
    }

    #endregion

    #region Equals

    public override bool Equals(object? obj)
    {
        if (obj is HSL other)
        {
            return this.Equals(other);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.H.GetHashCode() ^ this.S.GetHashCode() ^ this.L.GetHashCode();
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        return $"{this.H},{this.S},{this.L}";
    }

    #endregion
}

// H ∈ [0.0,360.0]
// S ∈ [0.0,1.0]
// V ∈ [0.0,1.0]
[Obsolete(Constants.InternalUsageWarning)]
public readonly struct HSV : IEquatable<HSV>
{
    public const int DefaultRoundingPrecision = 5;

    public HSV(double h, double s, double v, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.H = Math.Round(h, roundingPrecision);
            this.S = Math.Round(s, roundingPrecision);
            this.V = Math.Round(v, roundingPrecision);
        }
        else
        {
            this.H = h;
            this.S = s;
            this.V = v;
        }
    }

    public readonly double H;
    public readonly double S;
    public readonly double V;

    #region IEquatable<HSV>

    public bool Equals(HSV other)
    {
        return this.H == other.H && this.S == other.S && this.V == other.V;
    }

    #endregion

    #region Equals

    public override bool Equals(object? obj)
    {
        if (obj is HSV other)
        {
            return this.Equals(other);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.H.GetHashCode() ^ this.S.GetHashCode() ^ this.V.GetHashCode();
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        return $"{this.H},{this.S},{this.V}";
    }

    #endregion
}

[Obsolete(Constants.InternalUsageWarning)]
public readonly struct LAB : IEquatable<LAB>
{
    public const int DefaultRoundingPrecision = 5;

    public LAB(double l, double a, double b, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.L = Math.Round(l, roundingPrecision);
            this.A = Math.Round(a, roundingPrecision);
            this.B = Math.Round(b, roundingPrecision);
        }
        else
        {
            this.L = l;
            this.A = a;
            this.B = b;
        }
    }

    public readonly double L;
    public readonly double A;
    public readonly double B;

    #region IEquatable<LAB>

    public bool Equals(LAB other)
    {
        return this.L == other.L && this.A == other.A && this.B == other.B;
    }

    #endregion

    #region Equals

    public override bool Equals(object? obj)
    {
        if (obj is LAB other)
        {
            return this.Equals(other);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.L.GetHashCode() ^ this.A.GetHashCode() ^ this.B.GetHashCode();
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        return $"{this.L},{this.A},{this.B}";
    }

    #endregion
}

[Obsolete(Constants.InternalUsageWarning)]
public readonly struct LCH : IEquatable<LCH>
{
    public const int DefaultRoundingPrecision = 5;

    public LCH(double l, double c, double h, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.L = Math.Round(l, roundingPrecision);
            this.C = Math.Round(c, roundingPrecision);
            this.H = Math.Round(h, roundingPrecision);
        }
        else
        {
            this.L = l;
            this.C = c;
            this.H = h;
        }
    }

    public readonly double L;
    public readonly double C;
    public readonly double H;

    #region IEquatable<LCH>

    public bool Equals(LCH other)
    {
        return this.L == other.L && this.C == other.C && this.H == other.H;
    }

    #endregion

    #region Equals

    public override bool Equals(object? obj)
    {
        if (obj is LCH other)
        {
            return this.Equals(other);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.L.GetHashCode() ^ this.C.GetHashCode() ^ this.H.GetHashCode();
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        return $"{this.L},{this.C},{this.H}";
    }

    #endregion
}

[Obsolete(Constants.InternalUsageWarning)]
public readonly struct XYZ : IEquatable<XYZ>
{
    public const int DefaultRoundingPrecision = 5;

    public XYZ(double x, double y, double z, bool round = true, int roundingPrecision = DefaultRoundingPrecision)
    {
        if (round)
        {
            this.X = Math.Round(x, roundingPrecision);
            this.Y = Math.Round(y, roundingPrecision);
            this.Z = Math.Round(z, roundingPrecision);
        }
        else
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

    public readonly double X;
    public readonly double Y;
    public readonly double Z;

    #region IEquatable<XYZ>

    public bool Equals(XYZ other)
    {
        return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
    }

    #endregion

    #region Equals

    public override bool Equals(object? obj)
    {
        if (obj is XYZ other)
        {
            return this.Equals(other);
        }

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
    }

    #endregion

    #region ToString

    public override string ToString()
    {
        return $"{this.X},{this.Y},{this.Z}";
    }

    #endregion
}