using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// Base interface for controls supports key tips
    /// </summary>
    public interface IKeyTipedControl
    {
        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        void OnKeyTipPressed();
    }
}
