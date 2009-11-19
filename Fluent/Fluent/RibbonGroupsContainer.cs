using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represent panel with ribbon group.
    /// It is automatically adjusting size of controls
    /// </summary>
    public class RibbonGroupsContainer : Panel
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

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupsContainer(): base()
        {
        }

        #endregion

        #region Layout Overridings

        /// <summary>
        ///   Returns a collection of the panel's UIElements.
        /// </summary>
        /// <param name="logicalParent">The logical parent of the collection to be created.</param>
        /// <returns>Returns an ordered collection of elements that have the specified logical parent.</returns>
        protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, Parent as FrameworkElement);
        }

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
            Size infinitySize = new Size(Double.PositiveInfinity, constraint.Height);
            Size desiredSize = GetChildrenDesiredSize(infinitySize);

            // If the constraint and desired size are equal to those in the cache, skip
            // this layout measure pass.
            if ((constraint != cachedConstraint) || (desiredSize != cachedDesiredSize))
            {
                cachedConstraint = constraint;
                cachedDesiredSize = desiredSize;

                // If we have more available space - try to expand groups
                while (desiredSize.Width <= constraint.Width)
                {
                    bool hasMoreVariants = reduceOrderIndex < reduceOrder.Length - 1;
                    if (!hasMoreVariants) break;

                    // Increase size of another item
                    reduceOrderIndex++;
                    IncreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                    

                    desiredSize = GetChildrenDesiredSize(infinitySize);                    
                }

                // If not enough space - go to next variant
                while (desiredSize.Width > constraint.Width)
                {
                    bool hasMoreVariants = reduceOrderIndex >= 0;
                    if (!hasMoreVariants) break;

                    // Decrease size of another item
                    DecreaseGroupBoxSize(reduceOrder[reduceOrderIndex]);
                    reduceOrderIndex--;

                    desiredSize = GetChildrenDesiredSize(infinitySize);                     
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
            foreach (UIElement child in this.InternalChildren)
            {
                result += child.DesiredSize.Width;
            }
            return result;
        }

        Size GetChildrenDesiredSize(Size availableSize)
        {
            double width = 0;
            double height = 0;
            foreach (UIElement child in this.InternalChildren)
            {
                child.Measure(availableSize);
                width += child.DesiredSize.Width;
                height = Math.Max(height, child.DesiredSize.Height);
            }
            return new Size(width, height);
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


        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(finalSize);

            foreach (UIElement item in InternalChildren)
            {
                finalRect.Width = item.DesiredSize.Width;
                finalRect.Height = Math.Max(finalSize.Height, item.DesiredSize.Height);
                item.Arrange(finalRect);
                finalRect.X += item.DesiredSize.Width;
            }
            return finalSize;
        }

        #endregion
    }
}
