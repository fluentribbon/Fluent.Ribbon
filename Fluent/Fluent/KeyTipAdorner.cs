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
            if (keys != null)
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
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                UIElement child = VisualTreeHelper.GetChild(element, i) as UIElement;
                if (child != null) FindKeyTips(child);
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Gets element keytipped by keys
        /// </summary>
        /// <param name="keys"Keys></param>
        /// <returns>Element</returns>
        public UIElement GetElement(string keys)
        {
            return  associatedElements[0];

            for(int i = 0; i < keyTips.Count; i++)
            {
                if (keyTips[i].Content == keys) return associatedElements[i];
            }
            return null;
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
            for (int i = 0; i < keyTips.Count; i++)
            {
                keyTipPositions[i] = associatedElements[i].TranslatePoint(new Point(0, 0), AdornedElement);
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
