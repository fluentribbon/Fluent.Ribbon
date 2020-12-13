namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Extracts right content presenter of application menu converter
    /// </summary>
    public class ApplicationMenuRightScrollViewerExtractorConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <inheritdoc />
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ApplicationMenu menu)
            {
                return menu.Template.FindName("PART_ScrollViewer", menu) as UIElement;
            }

            return value;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}