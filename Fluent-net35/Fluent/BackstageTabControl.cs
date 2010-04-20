using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Tab control on backstage
    /// </summary>
    public class BackstageTabControl:TabControl
    {
        /// <summary>
        /// Static constructor
        /// </summary>
        static BackstageTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(typeof(BackstageTabControl)));
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own ItemContainer. 
        /// </summary>
        /// <returns>
        /// Returns true if the item is its own ItemContainer; otherwise, false.
        /// </returns>
        /// <param name="item">Specified item.</param>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return ((item is TabItem) || (item is Separator));
        }
    }
}
