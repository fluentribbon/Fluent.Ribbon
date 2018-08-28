// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using ControlzEx.Standard;
    using Fluent.Converters;
    using Fluent.Internal;

    /// <summary>
    /// Icon converter provides window or application default icon if user-defined is not present.
    /// </summary>
    [ValueConversion(sourceType: typeof(string), targetType: typeof(Image))]
    [ValueConversion(sourceType: typeof(Uri), targetType: typeof(Image))]
    [ValueConversion(sourceType: typeof(System.Drawing.Icon), targetType: typeof(Image))]
    [ValueConversion(sourceType: typeof(ImageSource), targetType: typeof(Image))]
    [ValueConversion(sourceType: typeof(string), targetType: typeof(ImageSource))]
    [ValueConversion(sourceType: typeof(Uri), targetType: typeof(ImageSource))]
    [ValueConversion(sourceType: typeof(System.Drawing.Icon), targetType: typeof(ImageSource))]
    [ValueConversion(sourceType: typeof(ImageSource), targetType: typeof(ImageSource))]
    public sealed class IconConverter : ObjectToImageConverter
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="iconBinding">The binding to which the converter should be applied to.</param>
        public IconConverter(Binding iconBinding)
            : base(iconBinding, new Size(SystemParameters.SmallIconWidth, SystemParameters.SmallIconHeight))
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="iconBinding">The binding to which the converter should be applied to.</param>
        /// <param name="targetVisualBinding">The target visual on which the image/icon should be shown.</param>
        public IconConverter(Binding iconBinding, Binding targetVisualBinding)
            : base(iconBinding, new Size(SystemParameters.SmallIconWidth, SystemParameters.SmallIconHeight), targetVisualBinding)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="iconBinding">The binding to which the converter should be applied to.</param>
        /// <param name="desiredSize">The desired size for the image.</param>
        /// <param name="targetVisualBinding">The target visual on which the image/icon should be shown.</param>
        public IconConverter(Binding iconBinding, Size desiredSize, Binding targetVisualBinding)
            : base(iconBinding, desiredSize, targetVisualBinding)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="desiredSize">The desired size for the image.</param>
        public IconConverter(Size desiredSize)
            : base(desiredSize)
        {
            if (desiredSize.IsEmpty
                || DoubleUtil.AreClose(desiredSize.Width, 0)
                || DoubleUtil.AreClose(desiredSize.Height, 0))
            {
                throw new ArgumentException("DesiredSize must not be empty and width/height must be greater than 0.", nameof(desiredSize));
            }
        }

        /// <inheritdoc />
        protected override object GetValueToConvert(object value, Size desiredSize, Visual targetVisual)
        {
            if (value == null)
            {
                var defaultIcon = GetDefaultIcon(targetVisual, desiredSize);

                if (defaultIcon != null)
                {
                    return defaultIcon;
                }
            }

            return base.GetValueToConvert(value, desiredSize, targetVisual);
        }

        private static ImageSource GetDefaultIcon(DependencyObject targetVisual, Size desiredSize)
        {
            IntPtr windowHandle;

            if (targetVisual != null)
            {
                var window = Window.GetWindow(targetVisual);

                if (window != null
                    && (windowHandle = new WindowInteropHelper(window).Handle) != IntPtr.Zero)
                {
                    return GetDefaultIcon(windowHandle, desiredSize);
                }
            }

            if (Application.Current != null
                && Application.Current.CheckAccess()
                && Application.Current.MainWindow != null
                && Application.Current.MainWindow.CheckAccess()
                && (windowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle) != IntPtr.Zero)
            {
                return GetDefaultIcon(windowHandle, desiredSize);
            }

            using (var p = Process.GetCurrentProcess())
            {
                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    return GetDefaultIcon(p.MainWindowHandle, desiredSize);
                }
            }

            return null;
        }

        private static ImageSource GetDefaultIcon(IntPtr hwnd, Size desiredSize)
        {
#pragma warning disable CS0219 // Variable is assigned but its value is never used

            // Retrieve the small icon for the window.
            const int ICON_SMALL = 0;
            // Retrieve the large icon for the window.
            const int ICON_BIG = 1;
            // Retrieves the small icon provided by the application. If the application does not provide one, the system uses the system-generated icon for that window.
            const int ICON_SMALL2 = 2;

            // Shares the image handle if the image is loaded multiple times. If LR_SHARED is not set, a second call to LoadImage for the same resource will load the image again and return a different handle.
            const int LR_SHARED = 0x00008000;

            const int IDI_APPLICATION = 0x7f00;

#pragma warning restore CS0219 // Variable is assigned but its value is never used

            try
            {
#pragma warning disable 618
                var iconPtr = IntPtr.Zero;

                if (hwnd != IntPtr.Zero)
                {
                    iconPtr = NativeMethods.SendMessage(hwnd, WM.GETICON, new IntPtr(ICON_SMALL2), IntPtr.Zero);

                    if (iconPtr == IntPtr.Zero)
                    {
                        iconPtr = NativeMethods.GetClassLong(hwnd, GCLP.HICONSM);
                    }
                }

                if (iconPtr == IntPtr.Zero)
                {
                    iconPtr = NativeMethods.LoadImage(IntPtr.Zero, new IntPtr(IDI_APPLICATION), 1, (int)desiredSize.Width, (int)desiredSize.Height, LR_SHARED);
                }

                if (iconPtr != IntPtr.Zero)
                {
                    var bitmapFrame = BitmapFrame.Create(Imaging.CreateBitmapSourceFromHIcon(iconPtr, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight((int)desiredSize.Width, (int)desiredSize.Height)));
                    return bitmapFrame;
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