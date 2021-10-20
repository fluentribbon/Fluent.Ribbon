// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows.Data;
    using JetBrains.Annotations;

    public static class IConverterExtensions
    {
        [CanBeNull]
        public static object Convert(this IValueConverter converter, [CanBeNull] object value)
        {
            return converter.Convert(value, null, null, null);
        }

        [CanBeNull]
        public static object ConvertBack(this IValueConverter converter, [CanBeNull] object value)
        {
            return converter.ConvertBack(value, null, null, null);
        }
    }
}