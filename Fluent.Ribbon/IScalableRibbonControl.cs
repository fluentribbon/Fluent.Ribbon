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
