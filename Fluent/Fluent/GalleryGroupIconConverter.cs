#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Helps associate an icon to a gallery group
    /// </summary>
    public sealed class GalleryGroupIconConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts source values to a value for the binding target.
        /// The data binding engine calls this method when it propagates 
        /// the values from source bindings to the binding target.
        /// </summary>
        /// <returns>A converted value</returns>
        /// <param name="values">The array of values that the source bindings 
        /// in the MultiBinding produces. The value DependencyProperty.UnsetValue 
        /// indicates that the source binding has no value to provide for conversion</param>
        /// <param name="targetType">The type of the binding target property</param>
        /// <param name="parameter">The converter parameter to use</param>
        /// <param name="culture">The culture to use in the converter</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Gallery gallery = VisualTreeHelper.GetParent((DependencyObject)values[1]) as Gallery;
            if (gallery != null)
            {
                string group = gallery.GetItemGroupName(((ReadOnlyObservableCollection<object>)values[0])[0]);
                if (gallery.GroupIcons.Count(x => x.GroupName == group) == 0) return null;
                return gallery.GroupIcons.First(x => x.GroupName == group).Icon;
            }
            return null;
        }

        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
