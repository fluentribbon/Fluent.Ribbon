namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Net.Cache;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Converts string or ImageSource to Image control
    /// </summary>
    public class ObjectToImageConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(value as string, UriKind.RelativeOrAbsolute), new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore))
                };
                return image;
            }

            var imageSource = value as ImageSource;

            if (imageSource != null)
            {
                var image = new Image
                {
                    Source = imageSource
                };
                return image;
            }
            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}