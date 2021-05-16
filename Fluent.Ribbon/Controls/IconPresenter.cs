#pragma warning disable 1591, WPF0012, WPF0023
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
    using Fluent.Converters;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    public class IconPresenter : ContentPresenter
    {
        /// <summary>Identifies the <see cref="IconSize"/> dependency property.</summary>
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            nameof(IconSize), typeof(IconSize), typeof(IconPresenter), new PropertyMetadata(IconSize.Small, PropertyChangedCallback));

        /// <summary>Identifies the <see cref="SmallSize"/> dependency property.</summary>
        public static readonly DependencyProperty SmallSizeProperty = DependencyProperty.Register(
            nameof(SmallSize), typeof(Size), typeof(IconPresenter), new PropertyMetadata(new Size(16, 16), PropertyChangedCallback));

        /// <summary>Identifies the <see cref="MediumSize"/> dependency property.</summary>
        public static readonly DependencyProperty MediumSizeProperty = DependencyProperty.Register(
            nameof(MediumSize), typeof(Size), typeof(IconPresenter), new PropertyMetadata(new Size(24, 24), PropertyChangedCallback));

        /// <summary>Identifies the <see cref="LargeSize"/> dependency property.</summary>
        public static readonly DependencyProperty LargeSizeProperty = DependencyProperty.Register(
            nameof(LargeSize), typeof(Size), typeof(IconPresenter), new PropertyMetadata(new Size(32, 32), PropertyChangedCallback));

        /// <summary>Identifies the <see cref="CustomSize"/> dependency property.</summary>
        public static readonly DependencyProperty CustomSizeProperty = DependencyProperty.Register(
            nameof(CustomSize), typeof(Size), typeof(IconPresenter), new PropertyMetadata(default(Size)));

        /// <summary>Identifies the <see cref="SmallIcon"/> dependency property.</summary>
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.Register(
            nameof(SmallIcon), typeof(object), typeof(IconPresenter), new PropertyMetadata(default, PropertyChangedCallback));

        /// <summary>Identifies the <see cref="MediumIcon"/> dependency property.</summary>
        public static readonly DependencyProperty MediumIconProperty = DependencyProperty.Register(
            nameof(MediumIcon), typeof(object), typeof(IconPresenter), new PropertyMetadata(default, PropertyChangedCallback));

        /// <summary>Identifies the <see cref="LargeIcon"/> dependency property.</summary>
        public static readonly DependencyProperty LargeIconProperty = DependencyProperty.Register(
            nameof(LargeIcon), typeof(object), typeof(IconPresenter), new PropertyMetadata(default, PropertyChangedCallback));

        /// <summary>Identifies the <see cref="BorderThickness"/> dependency property.</summary>
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            nameof(BorderThickness), typeof(Thickness), typeof(IconPresenter), new PropertyMetadata(default(Thickness), PropertyChangedCallback));

        /// <summary>Identifies the <see cref="BorderBrush"/> dependency property.</summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            nameof(BorderBrush), typeof(Brush), typeof(IconPresenter), new PropertyMetadata(default(Brush), PropertyChangedCallback));

        /// <summary>Identifies the <see cref="OptimalIcon"/> dependency property.</summary>
        public static readonly DependencyProperty OptimalIconProperty = DependencyProperty.Register(
            nameof(OptimalIcon), typeof(object), typeof(IconPresenter), new PropertyMetadata(default));

        [ThreadStatic]
        private static GrayscaleEffect? grayscaleEffect;

        static IconPresenter()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(IconPresenter), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));

            IsEnabledProperty.OverrideMetadata(typeof(IconPresenter), new UIPropertyMetadata(OnIsEnabledChanged));
        }

        public IconPresenter()
        {
            grayscaleEffect ??= new();

            var grid = new Grid();
            var icon = new ContentPresenter();
            var border = new Border();

            grid.Children.Add(icon);
            grid.Children.Add(border);

            var iconBinding = new Binding(nameof(this.OptimalIcon))
            {
                Source = this,
                Converter = new ObjectToImageConverter
                {
                    DesiredSizeBinding = new Binding(nameof(this.IconSize)) { Source = this },
                    TargetVisualBinding = new Binding { Source = this }
                }
            };
            icon.SetBinding(ContentProperty, iconBinding);

            var borderBrushBinding = new Binding(nameof(this.BorderBrush))
            {
                Source = this
            };
            border.SetBinding(Border.BorderBrushProperty, borderBrushBinding);

            var borderThicknessBinding = new Binding(nameof(this.BorderThickness))
            {
                Source = this
            };
            border.SetBinding(Border.BorderThicknessProperty, borderThicknessBinding);

            this.Content = grid;

            this.UpdateSize();
        }

        public Size SmallSize
        {
            get => (Size)this.GetValue(SmallSizeProperty);
            set => this.SetValue(SmallSizeProperty, value);
        }

        public Size MediumSize
        {
            get => (Size)this.GetValue(MediumSizeProperty);
            set => this.SetValue(MediumSizeProperty, value);
        }

        public Size LargeSize
        {
            get => (Size)this.GetValue(LargeSizeProperty);
            set => this.SetValue(LargeSizeProperty, value);
        }

        public Size CustomSize
        {
            get => (Size)this.GetValue(CustomSizeProperty);
            set => this.SetValue(CustomSizeProperty, value);
        }

        public IconSize IconSize
        {
            get => (IconSize)this.GetValue(IconSizeProperty);
            set => this.SetValue(IconSizeProperty, value);
        }

        public object? SmallIcon
        {
            get => this.GetValue(SmallIconProperty);
            set => this.SetValue(SmallIconProperty, value);
        }

        public object? MediumIcon
        {
            get => this.GetValue(MediumIconProperty);
            set => this.SetValue(MediumIconProperty, value);
        }

        public object? LargeIcon
        {
            get => this.GetValue(LargeIconProperty);
            set => this.SetValue(LargeIconProperty, value);
        }

        public Thickness BorderThickness
        {
            get => (Thickness)this.GetValue(BorderThicknessProperty);
            set => this.SetValue(BorderThicknessProperty, value);
        }

        public Brush? BorderBrush
        {
            get => (Brush)this.GetValue(BorderBrushProperty);
            set => this.SetValue(BorderBrushProperty, value);
        }

        public object? OptimalIcon
        {
            get => this.GetValue(OptimalIconProperty);
            set => this.SetValue(OptimalIconProperty, value);
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (IconPresenter)d;
            var newValue = (bool)e.NewValue;

            control.Effect = newValue
                ? null
                : grayscaleEffect;
            control.Opacity = newValue
                ? 1
                : 0.5;
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (IconPresenter)d;
            control.Update();
        }

        private void Update()
        {
            this.UpdateSize();

            var optimalIcon = this.GetOptimalIcon();

            if (this.OptimalIcon != optimalIcon)
            {
                this.SetCurrentValue(OptimalIconProperty, optimalIcon);
            }
        }

        private void UpdateSize()
        {
            var size = this.IconSize switch
            {
                IconSize.Small => this.SmallSize,
                IconSize.Medium => this.MediumSize,
                IconSize.Large => this.LargeSize,
                IconSize.Custom => this.CustomSize,
                _ => throw new ArgumentOutOfRangeException(nameof(this.IconSize), this.IconSize, null)
            };

            if (DoubleUtil.AreClose(this.Width, size.Width) == false)
            {
                this.SetCurrentValue(WidthProperty, size.Width);
            }

            if (DoubleUtil.AreClose(this.Height, size.Height) == false)
            {
                this.SetCurrentValue(HeightProperty, size.Height);
            }
        }

        public object? GetOptimalIcon()
        {
            return this.IconSize switch
            {
                IconSize.Small => this.SmallIcon ?? this.MediumIcon ?? this.LargeIcon,
                IconSize.Medium => this.MediumIcon ?? this.LargeIcon ?? this.SmallIcon,
                IconSize.Large => this.LargeIcon ?? this.MediumIcon ?? this.SmallIcon,
                _ => this.LargeIcon ?? this.MediumIcon ?? this.SmallIcon
            };
        }
    }
}