using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Fluent
{
    /// <summary>
    /// Represents additional toltip functionality
    /// </summary>
    public static class ToolTipService
    {
        /// <summary>
        /// Attach ooltip properties to control
        /// </summary>
        /// <param name="type">Control type</param>
        public static void Attach(Type type)
        {
            System.Windows.Controls.ToolTipService.ShowOnDisabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(true));
            System.Windows.Controls.ToolTipService.InitialShowDelayProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(900));
            System.Windows.Controls.ToolTipService.BetweenShowDelayProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(0));
            System.Windows.Controls.ToolTipService.ShowDurationProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(20000));    
        }
    }
}
