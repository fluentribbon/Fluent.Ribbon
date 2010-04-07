#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;


namespace Fluent
{
    /// <summary>
    /// An effect that turns the input into shades of a single color.
    /// </summary>
    public class GrayscaleEffect : ShaderEffect
    {
        /// <summary>
        /// Dependency property for Input
        /// </summary>
        public static readonly DependencyProperty InputProperty = 
            RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);

        /// <summary>
        /// Dependency property for FilterColor
        /// </summary>
        public static readonly DependencyProperty FilterColorProperty = 
            DependencyProperty.Register("FilterColor", typeof(Color), typeof(GrayscaleEffect), 
            new UIPropertyMetadata(Color.FromArgb(255, 255, 255, 255), PixelShaderConstantCallback(0)));

        /// <summary>
        /// Default constructor
        /// </summary>
        public GrayscaleEffect()
        {
            PixelShader pixelShader = new PixelShader();
            var prop = DesignerProperties.IsInDesignModeProperty; 

            bool isInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue; 
            if(!isInDesignMode)pixelShader.UriSource = new Uri("/Fluent;component/Themes/Office2010/Effects/Grayscale.ps", UriKind.Relative);
            PixelShader = pixelShader;

            UpdateShaderValue(InputProperty);
            UpdateShaderValue(FilterColorProperty);
        }

        /// <summary>
        /// Impicit input
        /// </summary>
        public Brush Input
        {
            get
            {
                return ((Brush)(GetValue(InputProperty)));
            }
            set
            {
                SetValue(InputProperty, value);
            }
        }

        /// <summary>
        /// The color used to tint the input.
        /// </summary>
        public Color FilterColor
        {
            get
            {
                return ((Color)(GetValue(FilterColorProperty)));
            }
            set
            {
                SetValue(FilterColorProperty, value);
            }
        }
    }
}
