﻿namespace Fluent;

using System;

/// <summary>
/// Repesents scalable ribbon contol
/// </summary>
public interface IScalableRibbonControl
{
    /// <summary>
    /// Resets the scale.
    /// </summary>
    void ResetScale();

    /// <summary>
    /// Enlarge control size.
    /// </summary>
    void Enlarge();

    /// <summary>
    /// Reduce control size.
    /// </summary>
    void Reduce();

    /// <summary>
    /// Occurs when contol is scaled.
    /// </summary>
    event EventHandler Scaled;
}