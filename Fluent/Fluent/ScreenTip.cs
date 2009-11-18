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
    /// Includes attached properties for controls 
    /// that want to be in ribbon group
    /// </summary>
    public class ScreenTip
    {
        #region Title Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
          "Title",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("Title",
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Title for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetTitle(UIElement element, string value)
        {
            element.SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Title of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static string GetTitle(UIElement element)
        {
            return (string)element.GetValue(TitleProperty);
        }

        #endregion

        #region Text Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
          "Text",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("Text",
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Text for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetText(UIElement element, string value)
        {
            element.SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Text of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static string GetText(UIElement element)
        {
            return (string)element.GetValue(TextProperty);
        }

        #endregion

        #region DisableReason Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisableReason.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisableReasonProperty = DependencyProperty.RegisterAttached(
          "DisableReason",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("",
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property DisableReason for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetDisableReason(UIElement element, string value)
        {
            element.SetValue(DisableReasonProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Text of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static string GetDisableReason(UIElement element)
        {
            return (string)element.GetValue(DisableReasonProperty);
        }

        #endregion

        #region Help Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Help.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HelpProperty = DependencyProperty.RegisterAttached(
          "Help",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("",
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Help for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetHelp(UIElement element, string value)
        {
            element.SetValue(HelpProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Help of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static string GetHelp(UIElement element)
        {
            return (string)element.GetValue(HelpProperty);
        }

        #endregion

        #region Image Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Image.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached(
          "Image",
          typeof(ImageSource),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata(null,
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Image for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetImage(UIElement element, ImageSource value)
        {
            element.SetValue(ImageProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Image of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static ImageSource GetImage(UIElement element)
        {
            return (ImageSource)element.GetValue(ImageProperty);
        }

        #endregion
    }
}
