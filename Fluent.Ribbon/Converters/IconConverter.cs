// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using ControlzEx.Standard;
    using Fluent.Converters;

    /// <summary>
    /// Icon converter provides application default icon if user-defined is not present.
    /// </summary>
    [ValueConversion(sourceType: typeof(string), targetType: typeof(ImageSource))]
    [ValueConversion(sourceType: typeof(Uri), targetType: typeof(ImageSource))]
    [ValueConversion(sourceType: typeof(ImageSource), targetType: typeof(ImageSource))]
    public sealed class IconConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if (Application.Current != null
                    && Application.Current.CheckAccess()
                    && Application.Current.MainWindow != null
                    && Application.Current.MainWindow.CheckAccess())
                {
                    try
                    {
                        return GetDefaultIcon(new WindowInteropHelper(Application.Current.MainWindow).Handle);
                    }
                    catch (InvalidOperationException)
                    {
                        return null;
                    }
                }

                var p = Process.GetCurrentProcess();
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    return GetDefaultIcon(p.MainWindowHandle);
                }
            }

            return ObjectToImageConverter.CreateImageSource(value, new Size(SystemParameters.SmallIconWidth, SystemParameters.SmallIconHeight));
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion

        private static ImageSource GetDefaultIcon(IntPtr hwnd)
        {
            // Retrieve the small icon for the window.
            const int ICON_SMALL = 0;
            // Retrieve the large icon for the window.
            const int ICON_BIG = 1;
            // Retrieves the small icon provided by the application. If the application does not provide one, the system uses the system-generated icon for that window.
            const int ICON_SMALL2 = 2;

            // Retrieves a handle to the icon associated with the class.
            const int GCL_HICON = -14;
            // Retrieves a handle to the small icon associated with the class.
            const int GCL_HICONSM = -34;

            // Shares the image handle if the image is loaded multiple times. If LR_SHARED is not set, a second call to LoadImage for the same resource will load the image again and return a different handle.
            const int LR_SHARED = 0x00008000;

            if (hwnd == IntPtr.Zero)
            {
                return null;
            }

            try
            {
#pragma warning disable 618
                var iconPtr = NativeMethods.SendMessage(hwnd, WM.GETICON, new IntPtr(ICON_SMALL), IntPtr.Zero);

                if (iconPtr == IntPtr.Zero)
                {
                    iconPtr = NativeMethods.GetClassLong(hwnd, GCL_HICONSM);
                }

                if (iconPtr == IntPtr.Zero)
                {
                    iconPtr = NativeMethods.LoadImage(IntPtr.Zero, new IntPtr(0x7f00) /*IDI_APPLICATION*/, 1, (int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight, LR_SHARED);
                }

                if (iconPtr != IntPtr.Zero)
                {
                    var bitmapFrame = BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(iconPtr, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)SystemParameters.SmallIconWidth, (int)SystemParameters.SmallIconHeight)));
                    return (ImageSource)bitmapFrame.GetAsFrozen();
                }
#pragma warning restore 618
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}