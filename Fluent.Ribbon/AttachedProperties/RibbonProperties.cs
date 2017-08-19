// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Media;
    using Fluent.Extensibility;

    /// <summary>
    /// Attached Properties for the Fluent Ribbon library
    /// </summary>
    public static class RibbonProperties
    {
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
                                              OnSizePropertyChanged));

        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sink = d as IRibbonSizeChangedSink;

            sink?.OnSizePropertyChanged((RibbonControlSize)e.OldValue, (RibbonControlSize)e.NewValue);
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
            DependencyProperty.RegisterAttached("SizeDefinition", typeof(RibbonControlSizeDefinition), typeof(RibbonProperties),
                new FrameworkPropertyMetadata(new RibbonControlSizeDefinition(RibbonControlSize.Large, RibbonControlSize.Middle, RibbonControlSize.Small),
                                              FrameworkPropertyMetadataOptions.AffectsArrange |
                                              FrameworkPropertyMetadataOptions.AffectsMeasure |
                                              FrameworkPropertyMetadataOptions.AffectsRender |
                                              FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                              OnSizeDefinitionPropertyChanged));

        /// <summary>
        /// Gets SizeDefinition for element
        /// </summary>
        public static RibbonControlSizeDefinition GetSizeDefinition(DependencyObject element)
        {
            return (RibbonControlSizeDefinition)element.GetValue(SizeDefinitionProperty);
        }

        /// <summary>
        /// Sets SizeDefinition for element
        /// </summary>
        public static void SetSizeDefinition(DependencyObject element, RibbonControlSizeDefinition value)
        {
            element.SetValue(SizeDefinitionProperty, value);
        }

        // Handles RibbonSizeDefinitionProperty changes
        internal static void OnSizeDefinitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Find parent group box
            var groupBox = FindParentRibbonGroupBox(d);
            var element = (UIElement)d;

            SetAppropriateSize(element, groupBox?.State ?? RibbonGroupBoxState.Large);
        }

        // Finds parent group box
        internal static RibbonGroupBox FindParentRibbonGroupBox(DependencyObject element)
        {
            var currentElement = element;
            RibbonGroupBox groupBox;

            while ((groupBox = currentElement as RibbonGroupBox) == null)
            {
                currentElement = VisualTreeHelper.GetParent(currentElement)
                    ?? LogicalTreeHelper.GetParent(currentElement);

                if (currentElement == null)
                {
                    break;
                }
            }

            return groupBox;
        }

        /// <summary>
        /// Sets appropriate size of the control according to the
        /// given group box state and control's size definition
        /// </summary>
        /// <param name="element">UI Element</param>
        /// <param name="state">Group box state</param>
        public static void SetAppropriateSize(DependencyObject element, RibbonGroupBoxState state)
        {
            SetSize(element, GetSizeDefinition(element).GetSize(state));
        }

        #endregion
    }
}