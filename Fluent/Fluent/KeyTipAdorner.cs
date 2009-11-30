using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace Fluent
{
    // TODO: make KeyTipAdorner internal class

    /// <summary>
    /// Represents adorner for KeyTips
    /// </summary>
    public class KeyTipAdorner : Adorner
    {
        #region Fields
        
        // KeyTips that have been
        // found on this element
        List<KeyTip> keyTips;
        List<UIElement> associatedElements;
        Point[] keyTipPositions;

        #endregion

        #region Intialization

        /// <summary>
        /// Construcotor
        /// </summary>
        /// <param name="adornedElement"></param>
        public KeyTipAdorner(UIElement adornedElement) : base(adornedElement)
        {
            keyTips = new List<KeyTip>();
            associatedElements = new List<UIElement>();

            // Try to find supported elements
            FindKeyTips(adornedElement);
            keyTipPositions = new Point[keyTips.Count];
        }

        // Find key tips on the given element
        void FindKeyTips(UIElement element)
        {
            string keys = KeyTip.GetKeys(element);
            if ((keys != null) && !((element is RibbonGroupBox)&&(element as RibbonGroupBox).State != RibbonGroupBoxState.Collapsed))
            {
                // Gotcha!
                KeyTip keyTip = new KeyTip();
                keyTip.Content = keys;

                // Add to list & visual 
                // children collections
                keyTips.Add(keyTip);
                base.AddVisualChild(keyTip);
                associatedElements.Add(element);
                return;
            }

            // Check children
            if (element is RibbonTabControl)
            {
                RibbonTabControl ribbonTabControl = (RibbonTabControl)element;
                foreach (UIElement item in ribbonTabControl.Items) FindKeyTips(item);
            }
            else
            {
                if (element is Ribbon) FindKeyTips((element as Ribbon).QuickAccessToolbar);

                int childrenCount = VisualTreeHelper.GetChildrenCount(element);
                for (int i = 0; i < childrenCount; i++)
                {
                    UIElement child = VisualTreeHelper.GetChild(element, i) as UIElement;
                    if (child != null) FindKeyTips(child);
                }
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Gets element keytipped by keys
        /// </summary>
        /// <param name="keys"Keys></param>
        /// <returns>Element</returns>
        public UIElement TryGetElement(string keys)
        {
            for(int i = 0; i < keyTips.Count; i++)
            {
                string keysUpper = keys.ToUpper();
                string contentUpper = (keyTips[i].Content as string).ToUpper();
                if (keysUpper == contentUpper) return associatedElements[i];
            }
            return null;
        }

        /// <summary>
        /// Is one of the elements starts with the given chars
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public bool IsElementsStartWith(string keys)
        {
            for (int i = 0; i < keyTips.Count; i++)
            {
                string keysUpper = keys.ToUpper();
                string contentUpper = (keyTips[i].Content as string).ToUpper();
                if (contentUpper.StartsWith(keysUpper)) return true;
            }
            return false;
        }

        #endregion

        /// <summary>
        /// Positions child elements and determines
        /// a size for the control
        /// </summary>
        /// <param name="finalSize">The final area within the parent 
        /// that this element should use to arrange 
        /// itself and its children</param>
        /// <returns>The actual size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rect = new Rect(finalSize);

            for (int i = 0; i < keyTips.Count; i++)
            {
                keyTips[i].Arrange(new Rect(keyTipPositions[i], keyTips[i].DesiredSize));
            }

            return finalSize;
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.keyTips[index];
        }

        /// <summary>
        /// Measures KeyTips
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {            
            Size infinitySize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
            foreach (KeyTip tip in keyTips) tip.Measure(infinitySize);
            UpdateKeyTipPositions();

            Size result = new Size(0, 0);
            for (int i = 0; i < keyTips.Count; i++)
            {
                double cornerX = keyTips[i].DesiredSize.Width + keyTipPositions[i].X;
                double cornerY = keyTips[i].DesiredSize.Height + keyTipPositions[i].Y;
                if (cornerX > result.Width) result.Width = cornerX;
                if (cornerY > result.Height) result.Height = cornerY;
            }
            return result;
        }

        void UpdateKeyTipPositions()
        {
            if (keyTips.Count == 0) return;

            double[] rows = null;
            if (AdornedElement is RibbonGroupsContainer)
            {
                RibbonGroupsContainer container = (RibbonGroupsContainer)AdornedElement;
                if (container.Children.Count != 0)
                {
                    RibbonGroupBox groupBox = (RibbonGroupBox)container.Children[0];
                    Panel panel = groupBox.GetPanel();
                    if (panel != null)
                    {
                        double height = container.DesiredSize.Height;
                        rows = new double[]
                        {
                            panel.TranslatePoint(new Point(0, 0), AdornedElement).Y,
                            panel.TranslatePoint(new Point(0, panel.DesiredSize.Height / 2.0), AdornedElement).Y,
                            panel.TranslatePoint(new Point(0, panel.DesiredSize.Height), AdornedElement).Y,
                            height
                        };
                    }
                }
            }

            for (int i = 0; i < keyTips.Count; i++)
            {
                if (associatedElements[i] is RibbonTabItem)
                {
                    // Ribbon Tab Item Exclusive Placement
                    Size keyTipSize = keyTips[i].DesiredSize;
                    Size elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                            elementSize.Height - keyTipSize.Height / 2.0), AdornedElement);
                }
                else if (associatedElements[i] is RibbonGroupBox)
                {
                    // Ribbon Group Box Exclusive Placement
                    Size keyTipSize = keyTips[i].DesiredSize;
                    Size elementSize = associatedElements[i].DesiredSize;
                    keyTipPositions[i] = associatedElements[i].TranslatePoint(
                        new Point(elementSize.Width / 2.0 - keyTipSize.Width / 2.0,
                            elementSize.Height + 1), AdornedElement);
                }
                else 
                {
                    if (RibbonControl.GetSize(associatedElements[i]) != RibbonControlSize.Large)
                    {
                        Point translatedPoint = associatedElements[i].TranslatePoint(new Point(8, 8), AdornedElement);
                        // Snapping to rows if it present
                        if (rows != null)
                        {
                            int index = 0;
                            double mindistance = Math.Abs(rows[0] - translatedPoint.Y);
                            for (int j = 1; j < rows.Length; j++)
                            {
                                double distance = Math.Abs(rows[j] - translatedPoint.Y);
                                if (distance < mindistance)
                                {
                                    mindistance = distance;
                                    index = j;
                                }
                            }
                            translatedPoint.Y = rows[index] - keyTips[i].DesiredSize.Height / 2.0;
                        }
                        keyTipPositions[i] = translatedPoint;
                    }
                    else
                    {
                        Point translatedPoint = associatedElements[i].TranslatePoint(new Point(
                            associatedElements[i].DesiredSize.Width / 2.0,
                            associatedElements[i].DesiredSize.Height - 8), AdornedElement);
                        if (rows != null) translatedPoint.Y = rows[2] - keyTips[i].DesiredSize.Height / 2.0;
                        keyTipPositions[i] = translatedPoint;
                    }
                }
            }
        }

        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return this.keyTips.Count;
            }
        }
    }
}
