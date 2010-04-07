#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluent
{
    /// <summary>
    /// Icon converter provides default icon if user-defined is not present
    /// </summary>
    public sealed class IconConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapFrame img = value as BitmapFrame;
            if (value == null)
            {
                if (Application.Current.MainWindow != null)
                {
                    return GetDefaultIcon((new WindowInteropHelper(Application.Current.MainWindow)).Handle) as BitmapFrame;
                }
            }
            if(img!=null)
            {
                var icons = img.Decoder.Frames.Where(x => ((x.Thumbnail != null) && (x.Thumbnail.PixelWidth == 16) && (x.PixelHeight == 16) && ((x.Format == PixelFormats.Bgra32) || (x.Format == PixelFormats.Bgr24))));
                if (icons.Count() > 0) return icons.First().Thumbnail;
            }
            return value;
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        static ImageSource GetDefaultIcon(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                try
                {
                    IntPtr zero = NativeMethods.SendMessage(hwnd, 0x7f, new IntPtr(2), IntPtr.Zero);
                    if (zero == IntPtr.Zero)
                    {
                        zero = NativeMethods.GetClassLongPtr(hwnd, -34);
                    }
                    if (zero == IntPtr.Zero)
                    {
                        zero = NativeMethods.LoadImage(IntPtr.Zero, new IntPtr(0x7f00), 1, (int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight, 0x8000);
                    }
                    if (zero != IntPtr.Zero)
                    {
                        return BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight)));
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
