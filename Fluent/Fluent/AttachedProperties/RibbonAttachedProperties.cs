namespace Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using Fluent.Extensibility;

    /// <summary>
    /// Attached Properties for the Fluent Ribbon library
    /// </summary>
    public class RibbonAttachedProperties
    {
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.RegisterAttached("TitleBarHeight", typeof(double), typeof(RibbonAttachedProperties),
                new FrameworkPropertyMetadata(25D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        public static void SetTitleBarHeight(UIElement element, double value)
        {
            element.SetValue(TitleBarHeightProperty, value);
        }

        public static double GetTitleBarHeight(UIElement element)
        {
            return (double)element.GetValue(TitleBarHeightProperty);
        }

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonSizeProperty =
            DependencyProperty.RegisterAttached("RibbonSize", typeof(RibbonControlSize), typeof(RibbonAttachedProperties),
                new FrameworkPropertyMetadata(RibbonControlSize.Large,
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender |
                                              FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                              OnRibbonSizePropertyChanged)
        );

        private static void OnRibbonSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sink = d as IRibbonSizeChangedSink;

            if (sink == null)
            {
                return;
            }

            sink.OnSizePropertyChanged((RibbonControlSize)e.OldValue, (RibbonControlSize)e.NewValue);
        }

        /// <summary>
        /// Sets SizeDefinition for element
        /// </summary>
        public static void SetRibbonSize(DependencyObject element, RibbonControlSize value)
        {
            element.SetValue(RibbonSizeProperty, value);
        }

        /// <summary>
        /// Gets SizeDefinition for element
        /// </summary>
        public static RibbonControlSize GetRibbonSize(DependencyObject element)
        {
            return (RibbonControlSize)element.GetValue(RibbonSizeProperty);
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonSizeDefinitionProperty =
            DependencyProperty.RegisterAttached("RibbonSizeDefinition", typeof(string), typeof(RibbonAttachedProperties),
                new FrameworkPropertyMetadata("Large, Middle, Small",
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender |
                                              FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                              OnRibbonSizeDefinitionPropertyChanged));

        /// <summary>
        /// Sets SizeDefinition for element
        /// </summary>
        public static void SetRibbonSizeDefinition(DependencyObject element, string value)
        {
            element.SetValue(RibbonSizeDefinitionProperty, value);
        }

        /// <summary>
        /// Gets SizeDefinition for element
        /// </summary>
        public static string GetRibbonSizeDefinition(DependencyObject element)
        {
            return (string)element.GetValue(RibbonSizeDefinitionProperty);
        }

        // Handles RibbonSizeDefinitionProperty changes
        internal static void OnRibbonSizeDefinitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Find parent group box
            var groupBox = FindParentRibbonGroupBox(d);
            var element = (UIElement)d;

            if (groupBox != null)
            {
                SetAppropriateSize(element, groupBox.State);
            }
            else
            {
                SetAppropriateSize(element, RibbonGroupBoxState.Large);
            }
        }

        // Finds parent group box
        [SuppressMessage("Microsoft.Performance", "CA1800")]
        internal static RibbonGroupBox FindParentRibbonGroupBox(DependencyObject o)
        {
            while (!(o is RibbonGroupBox))
            {
                o = VisualTreeHelper.GetParent(o) ?? LogicalTreeHelper.GetParent(o);
                if (o == null)
                {
                    break;
                }
            }

            return (RibbonGroupBox)o;
        }

        /// <summary>
        /// Sets appropriate size of the control according to the 
        /// given group box state and control's size definition
        /// </summary>
        /// <param name="element">UI Element</param>
        /// <param name="state">Group box state</param>
        public static void SetAppropriateSize(DependencyObject element, RibbonGroupBoxState state)
        {
            var index = (int)state;
            if (state == RibbonGroupBoxState.Collapsed)
            {
                index = 0;
            }

            SetRibbonSize(element, GetThreeRibbonSizeDefinition(element)[index]);
        }

        /// <summary>
        /// Gets value of the attached property SizeDefinition of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static RibbonControlSize[] GetThreeRibbonSizeDefinition(DependencyObject element)
        {
            var sizeDefinition = GetRibbonSizeDefinition(element);

            if (string.IsNullOrEmpty(sizeDefinition))
            {
                return new[] { RibbonControlSize.Large, RibbonControlSize.Large, RibbonControlSize.Large };
            }

            var splitted = sizeDefinition.Split(new[] { ' ', ',', ';', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);

            var count = Math.Min(splitted.Length, 3);
            if (count == 0)
            {
                return new[] { RibbonControlSize.Large, RibbonControlSize.Large, RibbonControlSize.Large };
            }

            var sizes = new RibbonControlSize[3];
            for (var i = 0; i < count; i++)
            {
                switch (splitted[i])
                {
                    case "Large":
                        sizes[i] = RibbonControlSize.Large;
                        break;

                    case "Middle":
                        sizes[i] = RibbonControlSize.Middle;
                        break;

                    case "Small":
                        sizes[i] = RibbonControlSize.Small;
                        break;

                    default:
                        sizes[i] = RibbonControlSize.Large;
                        break;
                }
            }

            for (var i = count; i < 3; i++)
            {
                sizes[i] = sizes[count - 1];
            }

            return sizes;
        }

        #endregion
    }
}