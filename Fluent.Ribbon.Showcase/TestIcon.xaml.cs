namespace FluentTest;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// <summary>
/// Interaction logic for TestIcon.xaml
/// </summary>
public partial class TestIcon : UserControl
{
    public TestIcon()
    {
        this.InitializeComponent();
    }

    #region IconSize

    /// <summary>
    /// Gets or sets whether ribbon control click must close backstage
    /// </summary>
    public double IconSize
    {
        get => (double)this.GetValue(IconSizeProperty);
        set => this.SetValue(IconSizeProperty, value);
    }

#pragma warning disable WPF0060
    /// <summary>Identifies the <see cref="IconSize"/> dependency property.</summary>
    public static readonly DependencyProperty IconSizeProperty =
        DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(TestIcon), new PropertyMetadata(32.0));
#pragma warning restore WPF0060

    #endregion

    #region IconBorderThickness

    /// <summary>
    /// Gets or sets whether ribbon control click must close backstage
    /// </summary>
    public Thickness IconBorderThickness
    {
        get => (Thickness)this.GetValue(IconBorderThicknessProperty);
        set => this.SetValue(IconBorderThicknessProperty, value);
    }

#pragma warning disable WPF0060
    /// <summary>Identifies the <see cref="IconBorderThickness"/> dependency property.</summary>
    public static readonly DependencyProperty IconBorderThicknessProperty =
        DependencyProperty.Register(nameof(IconBorderThickness), typeof(Thickness), typeof(TestIcon), new PropertyMetadata(new Thickness(0)));
#pragma warning restore WPF0060

    #endregion

    #region IconBrush

    /// <summary>
    /// Gets or sets whether ribbon control click must close backstage
    /// </summary>
    public Brush IconBrush
    {
        get => (Brush)this.GetValue(IconBrushProperty);
        set => this.SetValue(IconBrushProperty, value);
    }

#pragma warning disable WPF0060
    /// <summary>Identifies the <see cref="IconBrush"/> dependency property.</summary>
    public static readonly DependencyProperty IconBrushProperty =
        DependencyProperty.Register(nameof(IconBrush), typeof(Brush), typeof(TestIcon), new PropertyMetadata(Brushes.Red));
#pragma warning restore WPF0060

    #endregion
}