namespace Fluent.Internal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;

    internal static class DpiHelper
    {
        private static Matrix transformToDevice;
        private static Matrix transformToDip;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DpiHelper()
        {
            IntPtr desktop = NativeMethods.GetDC(IntPtr.Zero);

            // Can get these in the static constructor.  They shouldn't vary window to window,
            // and changing the system DPI requires a restart.
            int pixelsPerInchX = NativeMethods.GetDeviceCaps(desktop, 88);
            int pixelsPerInchY = NativeMethods.GetDeviceCaps(desktop, 90);

            transformToDip = Matrix.Identity;
            transformToDip.Scale(96d / pixelsPerInchX, 96d / pixelsPerInchY);
            transformToDevice = Matrix.Identity;
            transformToDevice.Scale(pixelsPerInchX / 96d, pixelsPerInchY / 96d);
            NativeMethods.ReleaseDC(IntPtr.Zero, desktop);
        }

        public static Thickness LogicalThicknessToDevice(Thickness logicalThickness)
        {
            Point point = DpiHelper.LogicalPixelsToDevice(new Point(logicalThickness.Left, logicalThickness.Top));
            Point point2 = DpiHelper.LogicalPixelsToDevice(new Point(logicalThickness.Right, logicalThickness.Bottom));
            return new Thickness(point.X, point.Y, point2.X, point2.Y);
        }

        /// <summary>
        /// Convert a point in device independent pixels (1/96") to a point in the system coordinates.
        /// </summary>
        /// <param name="logicalPoint">A point in the logical coordinate system.</param>
        /// <returns>Returns the parameter converted to the system's coordinates.</returns>
        public static Point LogicalPixelsToDevice(Point logicalPoint)
        {
            return transformToDevice.Transform(logicalPoint);
        }

        /// <summary>
        /// Convert a point in system coordinates to a point in device independent pixels (1/96").
        /// </summary>
        /// <param name="devicePoint">A point in the physical coordinate system.</param>
        /// <returns>Returns the parameter converted to the device independent coordinate system.</returns>
        public static Point DevicePixelsToLogical(Point devicePoint)
        {
            return transformToDip.Transform(devicePoint);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Rect LogicalRectToDevice(Rect logicalRectangle)
        {
            Point topLeft = LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top));
            Point bottomRight = LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        public static Rect DeviceRectToLogical(Rect deviceRectangle)
        {
            Point topLeft = DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top));
            Point bottomRight = DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom));

            return new Rect(topLeft, bottomRight);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Size LogicalSizeToDevice(Size logicalSize)
        {
            Point pt = LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height));

            return new Size { Width = pt.X, Height = pt.Y };
        }

        public static Size DeviceSizeToLogical(Size deviceSize)
        {
            Point pt = DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height));

            return new Size(pt.X, pt.Y);
        }
    }
}