// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Fluent.Helpers.ColorHelpers;

#pragma warning disable CA1308, CA1815, CA1051, CA2231, CA1051, CS1591
using System;
using System.Windows.Media;
using Fluent.Internal;

#pragma warning disable CA1815, CA1051

[Obsolete(Constants.InternalUsageWarning)]
public readonly struct ColorScaleStop
{
    public ColorScaleStop(Color color, double position)
    {
        this.Color = color;
        this.Position = position;
    }

    public ColorScaleStop(ColorScaleStop source)
    {
        this.Color = source.Color;
        this.Position = source.Position;
    }

    public readonly Color Color;
    public readonly double Position;
}