using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Fluent
{
    public class GalleryGroupIconConverter : IMultiValueConverter
    {
        /*#region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Gallery gallery = ((value as ReadOnlyObservableCollection<object>)[0] as FrameworkElement).Parent as Gallery;
            if (gallery != null)
            {
                string group = gallery.GetItemGroupName((value as ReadOnlyObservableCollection<object>)[0]);
                if (gallery.GroupIcons.Count(x => x.GroupName == group) == 0) return null;
                return gallery.GroupIcons.First(x => x.GroupName == group).Icon;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion*/

        #region Implementation of IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Gallery gallery = VisualTreeHelper.GetParent(values[1] as DependencyObject) as Gallery;
            if (gallery != null)
            {
                string group = gallery.GetItemGroupName((values[0] as ReadOnlyObservableCollection<object>)[0]);
                if (gallery.GroupIcons.Count(x => x.GroupName == group) == 0) return null;
                return gallery.GroupIcons.First(x => x.GroupName == group).Icon;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
