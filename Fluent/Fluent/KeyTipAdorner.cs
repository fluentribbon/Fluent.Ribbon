using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
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
    /// <summary>
    /// Represents adorner for KeyTips. 
    /// KeyTipAdorners is chained to produce one from another. 
    /// Detaching root adorner couses detaching all adorners in the chain
    /// </summary>
    internal class KeyTipAdorner : Adorner
    {
        #region Fields
        
        // KeyTips that have been
        // found on this element
        List<KeyTip> keyTips = new List<KeyTip>();
        List<UIElement> associatedElements = new List<UIElement>();
        Point[] keyTipPositions;

        // Parent adorner
        KeyTipAdorner parentAdorner = null;
        KeyTipAdorner childAdorner = null;
        // Focused element
        UIElement focusedElement = null;

        // Is this adorner attached to the adorned element?
        bool attached = false;

        // Current entered chars
        string enteredKeys = "";

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether at least one on the adorners in the chain is alive
        /// </summary>
        public bool IsAdornerChainAlive
        {
            get { return attached || (childAdorner != null && childAdorner.IsAdornerChainAlive); }
        }

        #endregion

        #region Intialization

        /// <summary>
        /// Construcotor
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="parentAdorner">Parent adorner or null</param>
        public KeyTipAdorner(UIElement adornedElement, KeyTipAdorner parentAdorner) : base(adornedElement)
        {
            this.parentAdorner = parentAdorner;

            // Try to find supported elements
            FindKeyTips(adornedElement);
            keyTipPositions = new Point[keyTips.Count];
        }
                
        // Find key tips on the given element
        void FindKeyTips(UIElement element)
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(element);
            foreach (object item in children)
            {
                UIElement child = item as UIElement;
                if (child != null)
                {
                    string keys = KeyTip.GetKeys(child);
                    if ((keys != null) && !((child is RibbonGroupBox) && (child as RibbonGroupBox).State != RibbonGroupBoxState.Collapsed))
                    {
                        // Gotcha!
                        KeyTip keyTip = new KeyTip();
                        keyTip.Content = keys;
                        // Bind IsEnabled property
                        Binding binding = new Binding("IsEnabled");
                        binding.Source = child;
                        binding.Mode = BindingMode.OneWay;
                        keyTip.SetBinding(UIElement.IsEnabledProperty, binding);

                        // Add to list & visual 
                        // children collections
                        keyTips.Add(keyTip);
                        base.AddVisualChild(keyTip);
                        associatedElements.Add(child);
                        continue;
                    }
                    FindKeyTips(child);
                }
            }
        }


        #endregion

        #region Attach & Detach

        /// <summary>
        /// Attaches this adorner to the adorned element
        /// </summary>
        public void Attach()
        {
            if (attached) return;

            FrameworkElement adornedElement = (FrameworkElement)AdornedElement;
            if (!adornedElement.IsLoaded)
            {
                // Delay attaching
                adornedElement.Loaded += OnDelayAttach;
                return;
            }

            focusedElement = Keyboard.FocusedElement as UIElement;
            if (focusedElement != null)
            {
                focusedElement.LostFocus += OnInputActionOccured;                
                focusedElement.PreviewKeyDown += OnPreviewKeyDown;
                focusedElement.PreviewKeyUp += OnPreviewKeyUp;
            }
            GetTopLevelElement(AdornedElement).PreviewMouseDown += OnInputActionOccured;

            // Show this adorner
            GetAdornerLayer(associatedElements[0]).Add(this);
            // Clears previous user input
            enteredKeys = "";
            attached = true;

        }

        void OnDelayAttach(object sender, EventArgs args)
        {
            ((FrameworkElement)AdornedElement).Loaded -= OnDelayAttach;
            Attach();
        }

        /// <summary>
        /// Detaches this adorner from the adorned element
        /// </summary> 
        public void Detach()
        {
            if (childAdorner != null) childAdorner.Detach();

            // Maybe adorner awaiting attaching, cancel it
            ((FrameworkElement)AdornedElement).Loaded -= OnDelayAttach;

            if (!attached) return;
            if (focusedElement != null)
            {
                focusedElement.LostFocus -= OnInputActionOccured;
                focusedElement.PreviewKeyDown -= OnPreviewKeyDown;
                focusedElement.PreviewKeyUp -= OnPreviewKeyUp;
                focusedElement = null;
            }
            GetTopLevelElement(AdornedElement).PreviewMouseDown -= OnInputActionOccured;

            // Show this adorner
            GetAdornerLayer(associatedElements[0]).Remove(this);
            // Clears previous user input
            enteredKeys = "";
            attached = false;
        }

        #endregion

        #region Event Handlers

        void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Back();
            else
            {
                string newKey = (new KeyConverter()).ConvertToString(e.Key);
                if (Char.IsLetterOrDigit(newKey, 0))
                {
                    e.Handled = true;
                    if (IsElementsStartWith(enteredKeys + newKey))
                    {
                        enteredKeys += newKey;
                        UIElement element = TryGetElement(enteredKeys);
                        if (element != null) Forward(element);
                    }
                    else System.Media.SystemSounds.Beep.Play();
                }                
            }
        }

        void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            
        }

        void OnInputActionOccured(object sender, RoutedEventArgs e)
        {
            Detach();
            if (childAdorner != null) childAdorner.Detach();
        }

        #endregion

        #region Static Methods

        static AdornerLayer GetAdornerLayer(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
            }
        }

        static UIElement GetTopLevelElement(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = VisualTreeHelper.GetParent(element) as UIElement;
                if (current == null) return element;
                element = current;
            }
        }

        #endregion

        #region Methods

        // Back to the previous adorner
        void Back()
        {
            Detach();
            if (parentAdorner != null) parentAdorner.Attach();
        }

        // Forward to the next element
        void Forward(UIElement element)
        {
            Detach();

            element.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, null));

            object obj = LogicalTreeHelper.GetChildren(element).Cast<object>().FirstOrDefault();
            if (obj == null) return;

            UIElement child = obj as UIElement;
            if (child == null) return;
            
            if (GetTopLevelElement(child) != GetTopLevelElement(element)) 
                childAdorner = new KeyTipAdorner(child, this);
            else
                childAdorner = new KeyTipAdorner(element, this);

            if (childAdorner.keyTips.Count != 0)
            {                
                childAdorner.Attach();
            }            
        }

        

        /// <summary>
        /// Gets element keytipped by keys
        /// </summary>
        /// <param name="keys"Keys></param>
        /// <returns>Element</returns>
        UIElement TryGetElement(string keys)
        {
            for(int i = 0; i < keyTips.Count; i++)
            {
                if (!keyTips[i].IsEnabled) continue;
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
                if (!keyTips[i].IsEnabled) continue;
                string keysUpper = keys.ToUpper();
                string contentUpper = (keyTips[i].Content as string).ToUpper();
                if (contentUpper.StartsWith(keysUpper)) return true;
            }
            return false;
        }

        #endregion

        #region Layout & Visual Children

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

            ;
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

        #endregion
    }
}
