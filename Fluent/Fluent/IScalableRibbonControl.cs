using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
