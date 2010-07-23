using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents logical sizes of a ribbon control 
    /// </summary>
    public enum RibbonControlSize
    {
        /// <summary>
        /// Large size of a control
        /// </summary>
        Large = 0,
        /// <summary>
        /// Middle size of a control
        /// </summary>
        Middle,
        /// <summary>
        /// Small size of a control
        /// </summary>
        Small
    }    

    /// <summary>
    /// Base interface for Fluent controls
    /// </summary>
    public interface IRibbonControl: IKeyTipedControl
    {
        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        RibbonControlSize Size { get; set; }

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        string SizeDefinition { get; set; }

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        object Header { get; set; }

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        object Icon { get; set; }

    }
}
