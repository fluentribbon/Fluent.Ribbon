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

        /// <summary>
        /// Sets <see cref="SizeProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetSize(DependencyObject element, RibbonControlSize value)
        {
            element.SetValue(SizeProperty, value);
        }

        /// <summary>
        /// Gets <see cref="SizeProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static RibbonControlSize GetSize(DependencyObject element)
        {
            return (RibbonControlSize)element.GetValue(SizeProperty);
        }

        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sink = d as IRibbonSizeChangedSink;

            sink?.OnSizePropertyChanged((RibbonControlSize)e.OldValue, (RibbonControlSize)e.NewValue);
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
        /// Sets <see cref="SizeDefinitionProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetSizeDefinition(DependencyObject element, RibbonControlSizeDefinition value)
        {
            element.SetValue(SizeDefinitionProperty, value);
        }

        /// <summary>
        /// Gets <see cref="SizeDefinitionProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static RibbonControlSizeDefinition GetSizeDefinition(DependencyObject element)
        {
            return (RibbonControlSizeDefinition)element.GetValue(SizeDefinitionProperty);
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

        #region AppTheme

        /// <summary>
        /// <see cref="DependencyProperty"/> for specifying AppTheme.
        /// </summary>
        public static readonly DependencyProperty AppThemeProperty = DependencyProperty.RegisterAttached("AppTheme", typeof(string), typeof(RibbonProperties), new PropertyMetadata(default(string)));

        /// <summary>
        /// Sets <see cref="AppThemeProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetAppTheme(DependencyObject element, string value)
        {
            element.SetValue(AppThemeProperty, value);
        }

        /// <summary>
        /// Gets <see cref="AppThemeProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static string GetAppTheme(DependencyObject element)
        {
            return (string)element.GetValue(AppThemeProperty);
        }

        #endregion

        #region MouseOverBackgroundProperty

        /// <summary>
        /// <see cref="DependencyProperty"/> for specifying MouseOverBackground.
        /// </summary>
        public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(RibbonProperties), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Sets <see cref="MouseOverBackgroundProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetMouseOverBackground(DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverBackgroundProperty, value);
        }

        /// <summary>
        /// Gets <see cref="MouseOverBackgroundProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static Brush GetMouseOverBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(MouseOverBackgroundProperty);
        }

        #endregion

        #region MouseOverForegroundProperty

        /// <summary>
        /// <see cref="DependencyProperty"/> for specifying MouseOverForeground.
        /// </summary>
        public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(RibbonProperties), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Sets <see cref="MouseOverForegroundProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetMouseOverForeground(DependencyObject element, Brush value)
        {
            element.SetValue(MouseOverForegroundProperty, value);
        }

        /// <summary>
        /// Gets <see cref="MouseOverForegroundProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static Brush GetMouseOverForeground(DependencyObject element)
        {
            return (Brush)element.GetValue(MouseOverForegroundProperty);
        }

        #endregion

        #region IsSelectedBackgroundProperty

        /// <summary>
        /// <see cref="DependencyProperty"/> for specifying IsSelectedBackground.
        /// </summary>
        public static readonly DependencyProperty IsSelectedBackgroundProperty = DependencyProperty.RegisterAttached("IsSelectedBackground", typeof(Brush), typeof(RibbonProperties), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// Sets <see cref="IsSelectedBackgroundProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static void SetIsSelectedBackground(DependencyObject element, Brush value)
        {
            element.SetValue(IsSelectedBackgroundProperty, value);
        }

        /// <summary>
        /// Gets <see cref="IsSelectedBackgroundProperty"/> for <paramref name="element"/>.
        /// </summary>
        public static Brush GetIsSelectedBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(IsSelectedBackgroundProperty);
        }

        #endregion
    }
}