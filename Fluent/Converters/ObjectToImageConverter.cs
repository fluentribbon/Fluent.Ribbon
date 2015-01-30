namespace Fluent.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net.Cache;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Converts string or ImageSource to Image control
    /// </summary>
    public class ObjectToImageConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var desiredSize = double.NaN;

            if (parameter != null)
            {
                try
                {
                    desiredSize = System.Convert.ToDouble(parameter);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            var imagePath = value as string;
            if (imagePath != null)
            {
                return CreateImage(imagePath, desiredSize);
            }

            var imageSource = value as ImageSource;

            if (imageSource == null)
            {
                return value;
            }

            var image = new Image
            {
                Source = ExtractImage(imageSource, desiredSize)
            };

            return image;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion

        private static Image CreateImage(string imagePath, double desiredSize)
        {
            if (double.IsNaN(desiredSize) == false
                && imagePath.EndsWith(".ico"))
            {
                return new Image
                    {
                        Source = ExtractImageFromIcoFile(imagePath, desiredSize)
                    };
            }

            return new Image
                {
                    Source = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute), new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore))
                };
        }

        private static ImageSource ExtractImageFromIcoFile(string imagePath, double desiredSize)
        {
            var decoder = BitmapDecoder.Create(
                new Uri("pack://application:,,," + imagePath, UriKind.RelativeOrAbsolute),
                BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreImageCache,
                BitmapCacheOption.None
                );

            return ExtractImage(decoder, desiredSize);
        }

        private static ImageSource ExtractImage(ImageSource imageSource, double desiredSize)
        {
            if (double.IsNaN(desiredSize))
            {
                return imageSource;
            }

            var bitmapFrame = imageSource as BitmapFrame;

            // We may have some other type of ImageSource
            // that doesn't have a notion of frames or decoder
            if (bitmapFrame == null
                || bitmapFrame.Decoder == null)
            {
                return imageSource;
            }

            return ExtractImage(bitmapFrame.Decoder, desiredSize);
        }

        private static ImageSource ExtractImage(BitmapDecoder decoder, double desiredSize)
        {
            var dpiFactor = 1.0;

            if (Application.Current.MainWindow != null)
            {
                // dpi.M11 = dpiX, dpi.M22 = dpiY
                var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);

                if (presentationSource != null)
                {
                    if (presentationSource.CompositionTarget != null)
                    {
                        var dpi = presentationSource.CompositionTarget.TransformToDevice;
                        dpiFactor = dpi.M11;
                    }
                }
            }

            var result = decoder.Frames
                .OrderBy(f => f.Width)
                .FirstOrDefault(f => f.Width >= desiredSize * dpiFactor);

            // if there is no matching frame, get the largest frame
            if (ReferenceEquals(result, default(BitmapFrame)))
            {
                result = decoder.Frames.OrderBy(f => f.Width).Last();
            }

            return result;
        }
    }
}