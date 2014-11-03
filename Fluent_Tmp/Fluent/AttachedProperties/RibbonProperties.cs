namespace Fluent
{
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for TitleBarHeight.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleBarHeightProperty =
            DependencyProperty.RegisterAttached("TitleBarHeight", typeof(double), typeof(RibbonProperties),
                new FrameworkPropertyMetadata(25D, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Sets TitleBarHeight for element
        /// </summary>
        public static void SetTitleBarHeight(UIElement element, double value)
        {
            element.SetValue(TitleBarHeightProperty, value);
        }

        /// <summary>
        /// Gets TitleBarHeight for element
        /// </summary>
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
            SetSize(element, GetSizeDefinition(element).GetSize(state));
        }

        #endregion
    }
}