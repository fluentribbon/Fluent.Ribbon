using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represent panel with ribbon group.
    /// It is automatically adjusting size of controls
    /// </summary>
    public class RibbonGroupsContainer : StackPanel
    {
        #region Reduce Order
        
        /// <summary>
        /// Gets or sets reduce order of group in the ribbon panel.
        /// It must be enumerated with comma from the first to reduce to 
        /// the last to reduce (use Control.Name as group name in the enum)
        /// </summary>
        public string ReduceOrder
        {
            get { return (string)GetValue(ReduceOrderProperty); }
            set { SetValue(ReduceOrderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ReduceOrder.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ReduceOrderProperty =
            DependencyProperty.Register("ReduceOrder", typeof(string), typeof(RibbonGroupsContainer), new UIPropertyMetadata(ReduceOrderPropertyChanged));


        static void ReduceOrderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupsContainer ribbonPanel = (RibbonGroupsContainer)d;
            ribbonPanel.cachedConstraint = ribbonPanel.cachedDesiredSize = new Size();
            ribbonPanel.reduceOrder = ((string)e.NewValue).Split(new char[] {',',' '}, StringSplitOptions.RemoveEmptyEntries);
            ribbonPanel.reduceOrderIndex = ribbonPanel.reduceOrder.Length - 1;

            // TODO: maybe InvalidateMeasure?
            ribbonPanel.InvalidateArrange();
        }

        #endregion

        #region Fields

        // A cached copy of the constraint from the previous layout pass.
        Size cachedConstraint;
        // A cached copy of the desired size from the previous layout pass.
        Size cachedDesiredSize;
        string[] reduceOrder = new string[0];
        int reduceOrderIndex = 0;

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
                    bool hasMoreVariants = reduceOrderIndex < reduceOrder.Length - 1;
                    if (!hasMoreVariants) break;

                    // Increase size of another item
                    reduceOrderIndex++;
                    IncreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                    

                    desiredSize = base.MeasureOverride(constraint);                    
                }

                // If not enough space - go to next variant
                while (GetChildrenWidth() > constraint.Width)
                {
                    bool hasMoreVariants = reduceOrderIndex >= 0;
                    if (!hasMoreVariants) break;

                    // Decrease size of another item
                    DecreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                    reduceOrderIndex--;

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

        

        // Increase size of the item
        void IncreaseGroupBoxSize(string name)
        {
            object item = FindName(name);
            if (item == null) return;
            RibbonGroupBox groupBox = (RibbonGroupBox)item;
            if(groupBox.State != RibbonGroupBoxState.Large) groupBox.State = groupBox.State - 1;
        }

        // Decrease size of the item
        void DecreaseGroupBoxSize(string name)
        {
            object item = FindName(name);
            if (item == null) return;
            RibbonGroupBox groupBox = (RibbonGroupBox)item;
            if(groupBox.State != RibbonGroupBoxState.Collapsed) groupBox.State = groupBox.State + 1;
        }

        #endregion
    }
}
