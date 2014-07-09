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
    public class RibbonProperties
    {
        #region TitleBarHeight Property

        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.RegisterAttached("TitleBarHeight", typeof(double), typeof(RibbonProperties),
                new FrameworkPropertyMetadata(25D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        public static void SetTitleBarHeight(UIElement element, double value)
        {
            element.SetValue(TitleBarHeightProperty, value);
        }

        public static double GetTitleBarHeight(UIElement element)
        {
            return (double)element.GetValue(TitleBarHeightProperty);
        }

        #endregion

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached("Size", typeof(RibbonControlSize), typeof(RibbonProperties),
                new FrameworkPropertyMetadata(RibbonControlSize.Large,
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender |
                                              FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                              OnSizePropertyChanged)
        );

        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        public static void SetSize(DependencyObject element, RibbonControlSize value)
        {
            element.SetValue(SizeProperty, value);
        }

        /// <summary>
        /// Gets SizeDefinition for element
        /// </summary>
        public static RibbonControlSize GetSize(DependencyObject element)
        {
            return (RibbonControlSize)element.GetValue(SizeProperty);
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty =
            DependencyProperty.RegisterAttached("SizeDefinition", typeof(string), typeof(RibbonProperties),
                new FrameworkPropertyMetadata("Large, Middle, Small",
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender |
                                              FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                              OnSizeDefinitionPropertyChanged));

        /// <summary>
        /// Sets SizeDefinition for element
        /// </summary>
        public static void SetSizeDefinition(DependencyObject element, string value)
        {
            element.SetValue(SizeDefinitionProperty, value);
        }

        /// <summary>
        /// Gets SizeDefinition for element
        /// </summary>
        public static string GetSizeDefinition(DependencyObject element)
        {
            return (string)element.GetValue(SizeDefinitionProperty);
        }

        // Handles RibbonSizeDefinitionProperty changes
        internal static void OnSizeDefinitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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

            SetSize(element, GetThreeRibbonSizeDefinition(element)[index]);
        }

        /// <summary>
        /// Gets value of the attached property SizeDefinition of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static RibbonControlSize[] GetThreeRibbonSizeDefinition(DependencyObject element)
        {
            var sizeDefinition = GetSizeDefinition(element);

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