namespace Fluent.Converters
{
    /// <summary>
    /// Hold static instances of several commonly used converters.
    /// </summary>
    public static class StaticConverters
    {
        /// <summary>
        /// Get a static instance of <see cref="InvertNumericConverter"/>
        /// </summary>
        public static readonly InvertNumericConverter InvertNumericConverter = new InvertNumericConverter();

        /// <summary>
        /// Get a static instance of <see cref="ThicknessConverter"/>
        /// </summary>
        public static readonly ThicknessConverter ThicknessConverter = new ThicknessConverter();

        /// <summary>
        /// Get a static instance of <see cref="ObjectToImageConverter"/>
        /// </summary>
        public static readonly ObjectToImageConverter ObjectToImageConverter = new ObjectToImageConverter();

        /// <summary>
        /// Get a static instance of <see cref="ColorToSolidColorBrushValueConverter"/>
        /// </summary>
        public static readonly ColorToSolidColorBrushValueConverter ColorToSolidColorBrushValueConverter = new ColorToSolidColorBrushValueConverter();

        /// <summary>
        /// Get a static instance of <see cref="EqualsToVisibilityConverter"/>
        /// </summary>
        public static readonly EqualsToVisibilityConverter EqualsToVisibilityConverter = new EqualsToVisibilityConverter();
    }
}