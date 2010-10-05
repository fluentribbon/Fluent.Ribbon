#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represents KeyTip control
    /// </summary>
    public class KeyTip : Label
    {
        #region Keys Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeysProperty = DependencyProperty.RegisterAttached(
          "Keys",
          typeof(string),
          typeof(KeyTip),
          new FrameworkPropertyMetadata(null, KeysPropertyChanged)
        );

        static void KeysPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Sets value of attached property Keys for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetKeys(DependencyObject element, string value)
        {
            element.SetValue(KeysProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Keys of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Keys"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Key sequence for the given element")]
        public static string GetKeys(DependencyObject element)
        {
            return (string)element.GetValue(KeysProperty);
        }

        #endregion

        #region AutoPlacement Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoPlacement.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoPlacementProperty = DependencyProperty.RegisterAttached(
          "AutoPlacement",
          typeof(bool),
          typeof(KeyTip),
          new FrameworkPropertyMetadata(true)
        );


        /// <summary>
        /// Sets whether key tip placement is auto 
        /// or defined by alignment and margin properties
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetAutoPlacement(DependencyObject element, bool value)
        {
            element.SetValue(AutoPlacementProperty, value);
        }

        /// <summary>
        /// Gets whether key tip placement is auto 
        /// or defined by alignment and margin properties
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("AutoPlacement"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Whether key tip placement is auto or defined by alignment and margin properties")]
        public static bool GetAutoPlacement(DependencyObject element)
        {
            return (bool)element.GetValue(AutoPlacementProperty);
        }

        #endregion

        #region HorizontalAlignment Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalAlignment.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static new readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
          "HorizontalAlignment",
          typeof(HorizontalAlignment),
          typeof(KeyTip),
          new FrameworkPropertyMetadata(HorizontalAlignment.Center)
        );


        /// <summary>
        /// Sets Horizontal Alignment of the key tip
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetHorizontalAlignment(DependencyObject element, HorizontalAlignment value)
        {
            element.SetValue(HorizontalAlignmentProperty, value);
        }

        /// <summary>
        /// Gets Horizontal alignment of the key tip
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("HorizontalAlignment"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Horizontal alignment of the key tip")]
        public static HorizontalAlignment GetHorizontalAlignment(DependencyObject element)
        {
            return (HorizontalAlignment)element.GetValue(HorizontalAlignmentProperty);
        }

        #endregion

        #region VerticalAlignment Attached Property
        
        /// <summary>
        /// Gets vertical alignment of the key tip
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("VerticalAlignment"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Vertical alignment of the key tip")]
        public static VerticalAlignment GetVerticalAlignment(DependencyObject element)
        {
            return (VerticalAlignment)element.GetValue(VerticalAlignmentProperty);
        }

        /// <summary>
        /// Sets vertical alignment of the key tip
        /// </summary>
        /// <param name="obj">The given element</param>
        /// <param name="value">Value</param>
        public static void SetVerticalAlignment(DependencyObject obj, VerticalAlignment value)
        {
            obj.SetValue(VerticalAlignmentProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalAlignment.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static new readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.RegisterAttached("VerticalAlignment", 
            typeof(VerticalAlignment), typeof(KeyTip), 
            new UIPropertyMetadata(VerticalAlignment.Center));
        
        #endregion

        #region Margin Attached Property

        /// <summary>
        /// Gets margin of the key tip
        /// </summary>
        /// <param name="obj">The key tip</param>
        /// <returns>Margin</returns>
        [System.ComponentModel.DisplayName("Margin"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Margin of the key tip")]
        public static Thickness GetMargin(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(MarginProperty);
        }

        /// <summary>
        /// Sets margin of the key tip
        /// </summary>
        /// <param name="obj">The key tip</param>
        /// <param name="value">Value</param>
        public static void SetMargin(DependencyObject obj, Thickness value)
        {
            obj.SetValue(MarginProperty, value);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Margin. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static new readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(KeyTip), new UIPropertyMetadata(new Thickness()));

        #endregion

        #region Initialization

        // Static constructor
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static KeyTip()
        {
            // Override metadata to allow slyling
            //StyleProperty.OverrideMetadata(typeof(KeyTip), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyTip), new FrameworkPropertyMetadata(typeof(KeyTip)));
            StyleProperty.OverrideMetadata(typeof(KeyTip), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(KeyTip));
            }

            return basevalue;
        }
        
        /// <summary>
        /// Default constrctor
        /// </summary>
        public KeyTip()
        {
        }

        #endregion
    }
}
