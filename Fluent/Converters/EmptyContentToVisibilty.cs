namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.Net.Cache;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class EmptyContentToVisibilty : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            if (!(value is string))
                return Visibility.Visible;
            string str = (string)value;
            if (!string.IsNullOrEmpty(str) && str.Length > 0)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}