﻿namespace Fluent.Converters;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Fluent.Internal;

#pragma warning disable WPF0072
    
/// <summary>
/// Converts <see cref="string"/>, <see cref="Uri"/>, <see cref="System.Drawing.Icon"/> or <see cref="ImageSource"/> to <see cref="System.Windows.Controls.Image"/> or <see cref="ImageSource"/> (dependent upon target type).
/// When converting you can pass a desired size as the converter parameter.
/// The returned <see cref="ImageSource"/> will be the closest <see cref="BitmapFrame"/> found in the provided image.
/// </summary>
/// <remarks>
/// - This converter is also a <see cref="MarkupExtension"/> to be able to extract the <see cref="DpiScale"/> from the target control.
/// - This converter is also a <see cref="IMultiValueConverter"/>. The order for parameters/values is:
///   0 = value to convert
///   1 = visual target or desired size
///   2 = desired size
///   Index checks are applied during value extraction, so providing null or just the value to convert are considered valid.
/// </remarks>
[MarkupExtensionReturnType(typeof(ImageSource))]
[ValueConversion(sourceType: typeof(string), targetType: typeof(Image))]
[ValueConversion(sourceType: typeof(Uri), targetType: typeof(Image))]
[ValueConversion(sourceType: typeof(System.Drawing.Icon), targetType: typeof(Image))]
[ValueConversion(sourceType: typeof(ImageSource), targetType: typeof(Image))]
[ValueConversion(sourceType: typeof(string), targetType: typeof(ImageSource))]
[ValueConversion(sourceType: typeof(Uri), targetType: typeof(ImageSource))]
[ValueConversion(sourceType: typeof(System.Drawing.Icon), targetType: typeof(ImageSource))]
[ValueConversion(sourceType: typeof(ImageSource), targetType: typeof(ImageSource))]
public class ObjectToImageConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    private static readonly ImageSource imageNotFoundImageSource = (ImageSource)CreateImageNotFoundImageSource().GetAsFrozen();
    private static readonly SizeConverter sizeConverter = new();

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public ObjectToImageConverter()
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="input">The object or binding to which the converter should be applied to.</param>
    public ObjectToImageConverter(object input)
        : this(input, Size.Empty, null)
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="input">The object or binding to which the converter should be applied to.</param>
    /// <param name="desiredSize">The desired size for the image.</param>
    public ObjectToImageConverter(object input, Size desiredSize)
        : this(input, desiredSize, null)
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="input">The object or binding to which the converter should be applied to.</param>
    /// <param name="desiredSize">The desired size for the image.</param>
    public ObjectToImageConverter(object input, object desiredSize)
        : this(input, desiredSize, null)
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="input">The object or binding to which the converter should be applied to.</param>
    /// <param name="desiredSize">The desired size for the image.</param>
    /// <param name="targetVisualBinding">The target visual on which the image/icon should be shown.</param>
    public ObjectToImageConverter(object input, object desiredSize, Binding? targetVisualBinding)
    {
        if (desiredSize is Size desiredSizeValue
            && (desiredSizeValue.IsEmpty
                || DoubleUtil.AreClose(desiredSizeValue.Width, 0)
                || DoubleUtil.AreClose(desiredSizeValue.Height, 0)))
        {
            throw new ArgumentException("DesiredSize must not be empty and width/height must be greater than 0.", nameof(desiredSize));
        }

        this.IconBinding = input as Binding ?? new Binding { Source = input };
        this.DesiredSizeBinding = desiredSize as Binding ?? new Binding { Source = desiredSize };
        this.TargetVisualBinding = targetVisualBinding;
    }

    /// <summary>
    /// The target visual on which the image/icon should be shown.
    /// </summary>
    [ConstructorArgument("targetVisualBinding")]
    public Binding? TargetVisualBinding { get; set; }

    /// <summary>
    /// The binding to which the converter should be applied to.
    /// </summary>
    [ConstructorArgument("iconBinding")]
    public Binding? IconBinding { get; set; }

    /// <summary>
    /// The binding for the desired size for the image.
    /// </summary>
    [ConstructorArgument("desiredSize")]
    public Binding? DesiredSizeBinding { get; set; }

    #region Implementation of IValueConverter

    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        var desiredSize = Size.Empty;

        if (parameter is double
            || parameter is int
            || parameter is string)
        {
            var size = System.Convert.ToDouble(parameter);
            desiredSize = new Size(size, size);
        }
        else if (parameter is Size size)
        {
            desiredSize = size;
        }

        return this.Convert(value, null, desiredSize, targetType);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    /// <summary>
    /// Returns the value to convert.
    /// </summary>
    protected virtual object? GetValueToConvert(object? value, Size desiredSize, Visual? targetVisual)
    {
        return value;
    }

    #endregion

    #region Implementation of IMultiValueConverter

    /// <inheritdoc />
    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var desiredSize = Size.Empty;
        var valuesLength = values.Length;

        // TargetVisual can be at index 1 or index 2, depending on the number of values passed
        var targetVisual = valuesLength == 2
            ? values[1] as Visual
            : null;

        if (targetVisual is null)
        {
            targetVisual = valuesLength == 3
                ? values[2] as Visual
                : null;
        }

        if (valuesLength >= 2
            && values[1] is Size desiredSizeFromIndex1)
        {
            desiredSize = desiredSizeFromIndex1;
        }
        else if (valuesLength >= 2
                 && (values[1] is Visual) == false)
        {
            var possibleDesiredSizeValue = values[1];
            object? convertedValue;

            if (possibleDesiredSizeValue is not null
                && sizeConverter.CanConvertFrom(possibleDesiredSizeValue.GetType())
                && (convertedValue = sizeConverter.ConvertFrom(possibleDesiredSizeValue)) is not null)
            {
                desiredSize = (Size)convertedValue;
            }
            else
            {
                desiredSize = Size.Empty;
            }
        }

        return this.Convert(values[0], targetVisual, desiredSize, targetType);
    }

    /// <inheritdoc />
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    #endregion Implementation of IMultiValueConverter

    #region Implementation of MarkupExtension

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this.CreateMultiBinding(serviceProvider);
    }

    #endregion Implementation of MarkupExtension

    private object CreateMultiBinding(IServiceProvider serviceProvider)
    {
        if (this.IconBinding is null)
        {
            return this;
        }

        var multiBinding = new MultiBinding
        {
            Converter = this
        };

        multiBinding.Bindings.Add(this.IconBinding);

        if (this.DesiredSizeBinding is not null)
        {
            multiBinding.Bindings.Add(this.DesiredSizeBinding);
        }

        if (this.TargetVisualBinding is not null)
        {
            multiBinding.Bindings.Add(this.TargetVisualBinding);
        }

        return multiBinding.ProvideValue(new ServiceProviderWrapper(serviceProvider));
    }

    private class ServiceProviderWrapper : IServiceProvider, IProvideValueTarget
    {
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderWrapper(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        /// <inheritdoc />
        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IProvideValueTarget))
            {
                return this;
            }

            return this.serviceProvider.GetService(serviceType);
        }

        /// <inheritdoc />
        public object? TargetObject => ((IProvideValueTarget)this.serviceProvider).TargetObject is DependencyObject dpo
            ? dpo
            : null;

        /// <inheritdoc />
        public object TargetProperty => ((IProvideValueTarget)this.serviceProvider).TargetProperty;
    }

    private object? Convert(object? value, Visual? targetVisual, Size desiredSize, Type targetType)
    {
        var imageSource = CreateFrozenImageSource(this.GetValueToConvert(value, desiredSize, targetVisual), targetVisual, desiredSize);

        if (imageSource is null)
        {
            return value;
        }

        if (typeof(ImageSource).IsAssignableFrom(targetType))
        {
            return imageSource;
        }

        var image = new Image
        {
            Source = imageSource,
            Stretch = Stretch.Uniform
        };

        if (desiredSize.IsEmpty == false)
        {
            image.Width = desiredSize.Width;
            image.Height = desiredSize.Height;
        }

        return image;
    }

    /// <summary>
    /// Extracts an <see cref="ImageSource"/> from <paramref name="value"/> which closest matches the <paramref name="desiredSize"/>.
    /// </summary>
    /// <param name="value">Value from which the <see cref="ImageSource"/> should be extracted. It can be of type <see cref="ImageSource"/></param>
    /// <param name="desiredSize">The desired size to extract from <paramref name="value"/> .</param>
    /// <returns>An frozen <see cref="ImageSource"/> which closest matches <paramref name="desiredSize"/></returns>
    public static ImageSource? CreateFrozenImageSource(object? value, Size desiredSize)
    {
        // We have to use a frozen instance. Otherwise we run into trouble if the same instance is used in multiple locations.
        // In case of BitmapImage it even gets worse when using the same Uri...
        return GetAsFrozenIfPossible(CreateImageSource(value, desiredSize));
    }

    /// <summary>
    /// Extracts an <see cref="ImageSource"/> from <paramref name="value"/> which closest matches the <paramref name="desiredSize"/>.
    /// </summary>
    /// <param name="value">Value from which the <see cref="ImageSource"/> should be extracted. It can be of type <see cref="ImageSource"/></param>
    /// <param name="targetVisual">The target on which the <see cref="ImageSource"/> will be used.</param>
    /// <param name="desiredSize">The desired size to extract from <paramref name="value"/> .</param>
    /// <returns>An frozen <see cref="ImageSource"/> which closest matches <paramref name="desiredSize"/></returns>
    public static ImageSource? CreateFrozenImageSource(object? value, Visual? targetVisual, Size desiredSize)
    {
        // We have to use a frozen instance. Otherwise we run into trouble if the same instance is used in multiple locations.
        // In case of BitmapImage it even gets worse when using the same Uri...
        return GetAsFrozenIfPossible(CreateImageSource(value, targetVisual, desiredSize));
    }

    /// <summary>
    /// Extracts an <see cref="ImageSource"/> from <paramref name="value"/> which closest matches the <paramref name="desiredSize"/>.
    /// </summary>
    /// <param name="value">Value from which the <see cref="ImageSource"/> should be extracted. It can be of type <see cref="ImageSource"/></param>
    /// <param name="desiredSize">The desired size to extract from <paramref name="value"/> .</param>
    /// <returns>An <see cref="ImageSource"/> which closest matches <paramref name="desiredSize"/></returns>
    public static ImageSource? CreateImageSource(object? value, Size desiredSize)
    {
        return CreateImageSource(value, null, desiredSize);
    }

    /// <summary>
    /// Extracts an <see cref="ImageSource"/> from <paramref name="value"/> which closest matches the <paramref name="desiredSize"/>.
    /// </summary>
    /// <param name="value">Value from which the <see cref="ImageSource"/> should be extracted. It can be of type <see cref="ImageSource"/></param>
    /// /// <param name="targetVisual">The target on which the <see cref="ImageSource"/> will be used.</param>
    /// <param name="desiredSize">The desired size to extract from <paramref name="value"/> .</param>
    /// <returns>An <see cref="ImageSource"/> which closest matches <paramref name="desiredSize"/></returns>
    public static ImageSource? CreateImageSource(object? value, Visual? targetVisual, Size desiredSize)
    {
        if (value is null)
        {
            return null;
        }

        if (desiredSize == default
            || DoubleUtil.AreClose(desiredSize.Width, 0)
            || DoubleUtil.AreClose(desiredSize.Height, 0))
        {
            desiredSize = Size.Empty;
        }

        if (value is ImageSource imageSource)
        {
            return ExtractImageSource(imageSource, targetVisual, desiredSize);
        }

        if (value is string imagePath)
        {
            return CreateImageSource(imagePath, targetVisual, desiredSize);
        }

        if (value is Uri imageUri)
        {
            return CreateImageSource(imageUri, targetVisual, desiredSize);
        }

        if (value is System.Drawing.Icon icon)
        {
            return ExtractImageSource(icon, targetVisual, desiredSize);
        }

        // !!! Danger zone ahead !!!
        // !!! Please do not copy that code somewhere and blame me for failures !!!
        // Hack to get the value from resource expressions
        {
            if (targetVisual is not null // to get values for resource expressions we need a DependencyObject
                && value is Expression expression)
            {
                var type = expression.GetType();
                var method = type.GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var field = type.GetField("_targetObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    ?? type.GetField("targetObject", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method is not null
                    && field is not null)
                {
                    // We have to set the target object. otherwise we might hit debug asserts.
                    if (field.GetValue(expression) is null)
                    {
                        field.SetValue(expression, targetVisual);
                    }

                    var valueFromExpression = method.Invoke(expression, new object[]
                    {
                        targetVisual,
                        // to get values from resource expressions we need a DependencyProperty, so just pass a random one
                        RibbonProperties.SizeProperty
                    });

                    return CreateImageSource(valueFromExpression, targetVisual, desiredSize);
                }
            }

            if (value.GetType().InheritsFrom("DeferredReference"))
            {
                var type = value.GetType();
                var method = type.GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method is not null)
                {
                    var valueFromDeferredReference = method.Invoke(value, new object?[]
                    {
                        null
                    });

                    return CreateImageSource(valueFromDeferredReference, targetVisual, desiredSize);
                }
            }
        }

        return null;
    }

    private static ImageSource CreateImageSource(string imagePath, Visual? targetVisual, Size desiredSize)
    {
        var imageUri = new Uri(imagePath, UriKind.RelativeOrAbsolute);

        // Allow things like "Images\Green.png"
        if (imageUri.IsAbsoluteUri == false)
        {
            // If that file does not exist, try to find it using resource notation
            if (File.Exists(imagePath) == false)
            {
                var slash = string.Empty;

                if (imagePath.StartsWith("/", StringComparison.OrdinalIgnoreCase) == false)
                {
                    slash = "/";
                }

                imageUri = new Uri("pack://application:,,," + slash + imagePath, UriKind.RelativeOrAbsolute);
            }
        }

        return CreateImageSource(imageUri, targetVisual, desiredSize);
    }

    private static ImageSource CreateImageSource(Uri imageUri, Visual? targetVisual, Size desiredSize)
    {
        try
        {
            var decoder = BitmapDecoder.Create(imageUri, BitmapCreateOptions.None, BitmapCacheOption.Default);

            return ExtractImageSource(decoder, targetVisual, desiredSize);
        }
        catch (IOException exception) when (DesignerProperties.GetIsInDesignMode(targetVisual ?? new DependencyObject()))
        {
            Trace.WriteLine(exception);

            return imageNotFoundImageSource;
        }
    }

    private static ImageSource ExtractImageSource(System.Drawing.Icon icon, Visual? targetVisual, Size desiredSize)
    {
        var imageSource = Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        return ExtractImageSource(imageSource, targetVisual, desiredSize);
    }

    private static ImageSource ExtractImageSource(ImageSource imageSource, Visual? targetVisual, Size desiredSize)
    {
        if (desiredSize.IsEmpty)
        {
            return imageSource;
        }

        var bitmapFrame = imageSource as BitmapFrame;

        // We may have some other type of ImageSource
        // that doesn't have a notion of frames or decoder
        if (bitmapFrame?.Decoder is null)
        {
            return imageSource;
        }

        return ExtractImageSource(bitmapFrame.Decoder, targetVisual, desiredSize);
    }

    private static ImageSource ExtractImageSource(BitmapDecoder decoder, Visual? targetVisual, Size desiredSize)
    {
        var scaledDesiredSize = GetScaledDesiredSize(desiredSize, targetVisual);

        var framesOrderedByWidth = decoder.Frames
            .OrderBy(f => f.Width)
            .ThenBy(f => f.Height)
            .ToList();

        // if there is no matching frame, get the largest frame
        return framesOrderedByWidth
                   .FirstOrDefault(f => f.Width >= scaledDesiredSize.Width
                                        && f.Height >= scaledDesiredSize.Height)
               ?? framesOrderedByWidth.Last();
    }

    /// <summary>
    /// Get the scaled desired size.
    /// </summary>
    protected static Size GetScaledDesiredSize(Size desiredSize, Visual? targetVisual)
    {
        return GetScaledDesiredSize(desiredSize, GetDpiScale(targetVisual));
    }

    /// <summary>
    /// Get the scaled desired size.
    /// </summary>
    private static Size GetScaledDesiredSize(Size desiredSize, DpiScale dpiScale)
    {
        if (desiredSize.IsEmpty)
        {
            return desiredSize;
        }

        return new Size(desiredSize.Width * dpiScale.DpiScaleX, desiredSize.Height * dpiScale.DpiScaleY);
    }

    private static DpiScale GetDpiScale(Visual? targetVisual)
    {
        if (targetVisual is not null)
        {
            return VisualTreeHelper.GetDpi(targetVisual);
        }

        if (Application.Current?.CheckAccess() == true
            && Application.Current.MainWindow?.CheckAccess() == true)
        {
            var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);

            if (presentationSource?.CompositionTarget is not null)
            {
                // dpi.M11 = dpiX, dpi.M22 = dpiY
                return new DpiScale(presentationSource.CompositionTarget.TransformToDevice.M11, presentationSource.CompositionTarget.TransformToDevice.M22);
            }
        }

        return new DpiScale(1, 1);
    }

    private static ImageSource CreateImageNotFoundImageSource()
    {
        var drawingGroup = new DrawingGroup
        {
            ClipGeometry = Geometry.Parse("M0,0 V426,667 H426,667 V0 H0 Z")
        };
        var geometryDrawing = new GeometryDrawing(Brushes.Red, new Pen(), Geometry.Parse("F1 M426.667,426.667z M0,0z M213.333,0C95.514,0 0,95.514 0,213.333 0,331.152 95.514,426.666 213.333,426.666 331.152,426.666 426.666,331.152 426.666,213.333 426.666,95.514 331.153,0 213.333,0z M330.995,276.689L276.693,330.995 213.333,267.639 149.973,330.999 95.671,276.689 159.027,213.333 95.671,149.973 149.973,95.671 213.333,159.027 276.693,95.671 330.995,149.973 267.639,213.333 330.995,276.689z"));
        using (drawingGroup.Append())
        {
            drawingGroup.Children.Add(geometryDrawing);
        }

        var image = new DrawingImage
        {
            Drawing = drawingGroup
        };

        return image;
    }

    private static ImageSource? GetAsFrozenIfPossible(ImageSource? imageSource)
    {
        if (imageSource is null)
        {
            return null;
        }

        if (imageSource.CanFreeze)
        {
            return (ImageSource)imageSource.GetAsFrozen();
        }

        return imageSource;
    }
}