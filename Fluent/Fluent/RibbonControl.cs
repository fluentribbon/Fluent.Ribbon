using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// Represents logical sizes of a ribbon control 
    /// </summary>
    public enum RibbonControlSize
    {
        /// <summary>
        /// Large size of a control
        /// </summary>
        Large = 0,
        /// <summary>
        /// Middle size of a control
        /// </summary>
        Middle,
        /// <summary>
        /// Small size of a control
        /// </summary>
        Small
    }

    /// <summary>
    /// Includes attached properties for controls 
    /// that want to be in ribbon group
    /// </summary>
    public class RibbonControl
    {
        #region Size Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached(
          "Size",
          typeof(RibbonControlSize),
          typeof(RibbonControl),
          new FrameworkPropertyMetadata(RibbonControlSize.Large, 
              FrameworkPropertyMetadataOptions.AffectsArrange |
              FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsRender)
        );

        /// <summary>
        /// Sets value of attached property Size for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetSize(UIElement element, RibbonControlSize value)
        {
            element.SetValue(SizeProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Size of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static RibbonControlSize GetSize(UIElement element)
        {
            return (RibbonControlSize)element.GetValue(SizeProperty);
        }

        #endregion

        #region Icons

        #region LargeIcon Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for LargeIcon.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = DependencyProperty.RegisterAttached(
          "LargeIcon",
          typeof(ImageSource),
          typeof(RibbonControl),
          new FrameworkPropertyMetadata(null,
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property LargeIcon for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetLargeIcon(UIElement element, ImageSource value)
        {
            element.SetValue(LargeIconProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property LargeIcon of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static ImageSource GetLargeIcon(UIElement element)
        {
            return (ImageSource)element.GetValue(LargeIconProperty);
        }

        #endregion

        #region SmallIcon Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SmallIconProperty = DependencyProperty.RegisterAttached(
          "SmallIcon",
          typeof(ImageSource),
          typeof(RibbonControl),
          new FrameworkPropertyMetadata(null,
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property SmallIcon for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetSmallIcon(UIElement element, ImageSource value)
        {
            element.SetValue(SmallIconProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property SmallIcon of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static ImageSource GetSmallIcon(UIElement element)
        {
            return (ImageSource)element.GetValue(SmallIconProperty);
        }

        #endregion
        
        #endregion
    }
}
