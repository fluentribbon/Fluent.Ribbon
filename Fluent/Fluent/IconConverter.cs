#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            if (value == null)
            {
                if (Application.Current != null
                    && Application.Current.MainWindow != null)
                {
                    try
                    {
                        return GetDefaultIcon((new WindowInteropHelper(Application.Current.MainWindow)).Handle) as BitmapFrame;
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                }

                var p = Process.GetCurrentProcess();
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    return GetDefaultIcon(p.MainWindowHandle/*(new WindowInteropHelper(Application.Current.MainWindow)).Handle*/) as BitmapFrame;
                }
            }

            var bitmapFrame = value as BitmapFrame;

            if (bitmapFrame == null
                || bitmapFrame.Decoder == null)
            {
                return null;
            }

            foreach (var frame in bitmapFrame.Decoder.Frames)
            {
                var source = GetThumbnail(frame);

                if (source != null)
                {
                    return source;
                }
            }

            return value;
        }

        /// <summary>
        /// ThumbnailExceptionWorkArround when image cause a format exception by accessing the Thumbnail
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        static BitmapSource GetThumbnail(BitmapSource frame)
        {
            try
            {
                if (frame != null
                    && frame.PixelWidth == 16
                    && frame.PixelHeight == 16
                    && (frame.Format == PixelFormats.Bgra32 || frame.Format == PixelFormats.Bgr24))
                {
                    return frame;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031")]
        static ImageSource GetDefaultIcon(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                try
                {
                    var zero = NativeMethods.SendMessage(hwnd, 0x7f, new IntPtr(2), IntPtr.Zero);

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
            return Binding.DoNothing;
        }

        #endregion
    }
}