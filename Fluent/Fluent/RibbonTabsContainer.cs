using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represent panel with ribbon tab items.
    /// It is automatically adjusting size of tabs
    /// </summary>
    public class RibbonTabsContainer : StackPanel
    {
        #region Fields

        // A cached copy of the constraint from the previous layout pass.
        Size cachedConstraint;
        // A cached copy of the desired size from the previous layout pass.
        Size cachedDesiredSize;
        string[] reduceOrder = new string[0];
        int reduceOrderIndex = 0;

        #endregion
                
        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabsContainer() : base()
        {
            Orientation = Orientation.Horizontal;
        }

        #endregion

        #region Layout Overridings

        /// <summary>
        /// Measures all of the RibbonGroupBox, and resize them appropriately
        /// to fit within the available room
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize = base.MeasureOverride(constraint);

            // If the constraint and desired size are equal to those in the cache, skip
            // this layout measure pass.
            if ((constraint != cachedConstraint) || (desiredSize != cachedDesiredSize))
            {
                cachedConstraint = constraint;
                cachedDesiredSize = desiredSize;

                // If we have more available space - try to expand groups
                while (GetChildrenWidth() <= constraint.Width)
                {
                    
                    

                    desiredSize = base.MeasureOverride(constraint);                    
                }

                // If not enough space - go to next variant
                while (GetChildrenWidth() > constraint.Width)
                {
                    

                    desiredSize = base.MeasureOverride(constraint);                    
                }                
            }

            return desiredSize;
        }

        /// <summary>
        /// Calculates the total width of all children
        /// </summary>
        /// <returns>Returns the total width</returns>
        double GetChildrenWidth()
        {
            double result = 0;
            foreach (UIElement child in this.Children)
            {
                result += child.DesiredSize.Width;
            }
            return result + 3;
        }
        
        #endregion
    }
}
