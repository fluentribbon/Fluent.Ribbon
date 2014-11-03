#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;

namespace Fluent
{
    /// <summary>
    /// Repesents scalable ribbon contol
    /// </summary>
    public interface IScalableRibbonControl
    {
        /// <summary>
        /// Enlarge control size
        /// </summary>
        void Enlarge();
        /// <summary>
        /// Reduce control size
        /// </summary>
        void Reduce();

        /// <summary>
        /// Occurs when contol is scaled
        /// </summary>
        event EventHandler Scaled;
    }
}
