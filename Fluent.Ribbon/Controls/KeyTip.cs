using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

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
          new PropertyMetadata(KeysPropertyChanged)
        );

        private static void KeysPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
          new PropertyMetadata(BooleanBoxes.TrueBox)
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
        public new static readonly DependencyProperty HorizontalAlignmentProperty = DependencyProperty.RegisterAttached(
          nameof(HorizontalAlignment),
          typeof(HorizontalAlignment),
          typeof(KeyTip),
          new PropertyMetadata(HorizontalAlignment.Center)
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
        public new static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.RegisterAttached("VerticalAlignment",
            typeof(VerticalAlignment), typeof(KeyTip),
            new PropertyMetadata(VerticalAlignment.Center));

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
        public new static readonly DependencyProperty MarginProperty =
            DependencyProperty.RegisterAttached(nameof(Margin), typeof(Thickness), typeof(KeyTip), new PropertyMetadata(new Thickness()));

        #endregion

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static KeyTip()
        {
            // Override metadata to allow slyling
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyTip), new FrameworkPropertyMetadata(typeof(KeyTip)));
        }
    }
}