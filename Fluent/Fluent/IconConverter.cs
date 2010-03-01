using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluent
{
    public class IconConverter:IValueConverter
    {
        #region Implementation of IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                var icons = img.Decoder.Frames.Where(x => ((x.Thumbnail.PixelWidth == 16) && (x.PixelHeight == 16) && ((x.Format == PixelFormats.Bgra32) || (x.Format == PixelFormats.Bgr24))));
                if (icons.Count() > 0) return icons.First().Thumbnail;
            }
            return value;
        }

        private static ImageSource GetDefaultIcon(IntPtr hwnd)
        {
            if (hwnd != IntPtr.Zero)
            {
                IntPtr zero = IntPtr.Zero;
                try
                {
                    zero = NativeMethods.SendMessage(hwnd, 0x7f, new IntPtr(2), IntPtr.Zero);
                    if (zero == IntPtr.Zero)
                    {
                        zero = NativeMethods.GetClassLongPtr(hwnd, -34);
                    }
                    if (zero == IntPtr.Zero)
                    {
                        zero = NativeMethods.LoadImage(IntPtr.Zero, 0x7f00, 1, (int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight, 0x8000);
                    }
                    if (zero != IntPtr.Zero)
                    {
                        return BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight)));
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
