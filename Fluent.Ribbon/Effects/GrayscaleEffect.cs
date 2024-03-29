﻿// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

/// <summary>
/// An effect that turns the input into shades of a single color.
/// </summary>
public class GrayscaleEffect : ShaderEffect
{
    /// <summary>
    /// Dependency property for Input
    /// </summary>
    public static readonly DependencyProperty InputProperty =
        RegisterPixelShaderSamplerProperty(nameof(Input), typeof(GrayscaleEffect), 0);

    /// <summary>Identifies the <see cref="FilterColor"/> dependency property.</summary>
    public static readonly DependencyProperty FilterColorProperty =
        DependencyProperty.Register(nameof(FilterColor), typeof(Color), typeof(GrayscaleEffect),
            new PropertyMetadata(Color.FromArgb(255, 255, 255, 255), PixelShaderConstantCallback(0)));

    /// <summary>
    /// Default constructor
    /// </summary>
    public GrayscaleEffect()
    {
        this.PixelShader = this.CreatePixelShader();

        this.UpdateShaderValue(InputProperty);
        this.UpdateShaderValue(FilterColorProperty);
    }

    private PixelShader CreatePixelShader()
    {
        var pixelShader = new PixelShader { UriSource = new Uri("pack://application:,,,/Fluent;component/Themes/Effects/Grayscale.ps", UriKind.RelativeOrAbsolute) };

        return pixelShader;
    }

    /// <summary>
    /// Implicit input
    /// </summary>
    public Brush Input
    {
        get => (Brush)this.GetValue(InputProperty);
        set => this.SetValue(InputProperty, value);
    }

    /// <summary>
    /// The color used to tint the input.
    /// </summary>
    public Color FilterColor
    {
        get => (Color)this.GetValue(FilterColorProperty);
        set => this.SetValue(FilterColorProperty, value);
    }
}