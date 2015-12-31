using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace FluentTest.Converters
{
    public class ConfigurableBoolConverter<T> : MarkupExtension, IValueConverter
    {
        public T True { get; set; }
        public T False { get; set; }

        public ConfigurableBoolConverter( T trueValue, T falseValue )
        {
            True = trueValue;
            False = falseValue;
        }

        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            return this;
        }

        public virtual object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return value is T && EqualityComparer<T>.Default.Equals( (T)value, True );
        }
    }
}

