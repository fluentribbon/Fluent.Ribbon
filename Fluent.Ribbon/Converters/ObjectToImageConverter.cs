namespace Fluent.Converters
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Cache;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Converts string, URI or ImageSource to Image control
    /// </summary>
    [ValueConversion(sourceType: typeof(string), targetType: typeof(Image))]
    [ValueConversion(sourceType: typeof(Uri), targetType: typeof(Image))]
    [ValueConversion(sourceType: typeof(ImageSource), targetType: typeof(Image))]
    public class ObjectToImageConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var desiredSize = double.NaN;

            if (parameter != null)
            {
                desiredSize = System.Convert.ToDouble(parameter);
            }

            var imageSource = value as ImageSource;

            if (imageSource == null)
            {
                var imagePath = value as string;
                if (imagePath != null)
                {
                    imageSource = CreateImageSource(imagePath, desiredSize);
                }
            }

            if (imageSource == null)
            {
                var imageUri = value as Uri;
                if (imageUri != null)
                {
                    imageSource = CreateImageSource(imageUri, desiredSize);
                }
            }

            if (imageSource == null)
            {
                return value;
            }

            var image = new Image
                        {
                            // We have to use a frozen instance. Otherwise we run into trouble if the same instance is used in multiple locations.
                            // In case of BitmapImage it even gets worse when using the same Uri...
                            Source = (ImageSource)ExtractImage(imageSource, desiredSize).GetAsFrozen()
                        };

            return image;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        #endregion

        private static ImageSource CreateImageSource(string imagePath, double desiredSize)
        {
            // Allow things like "Images\Green.png"
            if (imagePath.StartsWith("pack:", StringComparison.OrdinalIgnoreCase) == false)
            {
                // If that file does not exist, try to find it using resource notation
                if (File.Exists(imagePath) == false)
                {
                    var slash = string.Empty;

                    if (imagePath.StartsWith("/") == false)
                    {
                        slash = "/";
                    }

                    imagePath = "pack://application:,,," + slash + imagePath;
                }
            }

            var imageUri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

            return CreateImageSource(imageUri, desiredSize);
        }

        private static ImageSource CreateImageSource(Uri imageUri, double desiredSize)
        {
            if (double.IsNaN(desiredSize) == false
                && imageUri.AbsolutePath.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
            {
                return ExtractImageFromIcoFile(imageUri, desiredSize);
            }

            return new BitmapImage(imageUri, new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore));
        }

        private static ImageSource ExtractImageFromIcoFile(Uri imageUri, double desiredSize)
        {
            var decoder = BitmapDecoder.Create(
                imageUri,
                BitmapCreateOptions.DelayCreation | BitmapCreateOptions.IgnoreImageCache,
                BitmapCacheOption.None);

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
            if (bitmapFrame?.Decoder == null)
            {
                return imageSource;
            }

            return ExtractImage(bitmapFrame.Decoder, desiredSize);
        }

        private static ImageSource ExtractImage(BitmapDecoder decoder, double desiredSize)
        {
            var dpiFactor = 1.0;

            if (Application.Current?.CheckAccess() == true
                && Application.Current.MainWindow?.CheckAccess() == true)
            {
                // dpi.M11 = dpiX, dpi.M22 = dpiY
                var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);

                if (presentationSource?.CompositionTarget != null)
                {
                    dpiFactor = presentationSource.CompositionTarget.TransformToDevice.M11;
                }
            }

            var framesOrderedByWidth = decoder.Frames
                                        .OrderBy(f => f.Width)
                                        .ToList();

            // if there is no matching frame, get the largest frame
            return framesOrderedByWidth
                    .FirstOrDefault(f => f.Width >= desiredSize * dpiFactor)
                   ?? framesOrderedByWidth.Last();
        }
    }
}