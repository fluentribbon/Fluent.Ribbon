using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
              FrameworkPropertyMetadataOptions.AffectsRender |
              FrameworkPropertyMetadataOptions.AffectsParentArrange |
              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
              OnSizePropertyChanged)
        );

        ///     When the ControlSizeDefinition property changes we need to invalidate the parent chain measure so that
        ///     the RibbonGroupsContainer can calculate the new size within the same MeasureOverride call.  This property
        ///     usually changes from RibbonGroupsContainer.MeasureOverride.
        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Visual visual = d as Visual;
            while (visual != null)
            {
                UIElement uiElement = visual as UIElement;
                if (uiElement != null)
                {
                    if (uiElement is RibbonGroupsContainer)
                    {
                        break;
                    }

                    uiElement.InvalidateMeasure();
                }

                visual = VisualTreeHelper.GetParent(visual) as Visual;
            }
        }

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
        [System.ComponentModel.Browsable(false)]
        public static RibbonControlSize GetSize(UIElement element)
        {
            return (RibbonControlSize)element.GetValue(SizeProperty);
        }

        #endregion
        
        #region SizeDefinition Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = DependencyProperty.RegisterAttached(
          "SizeDefinition",
          typeof(string),
          typeof(RibbonControl),
          new FrameworkPropertyMetadata("Large, Middle, Small",
              FrameworkPropertyMetadataOptions.AffectsArrange |
              FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsRender |
              FrameworkPropertyMetadataOptions.AffectsParentArrange |
              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
              OnSizeDefinitionPropertyChanged)
        );

        static void OnSizeDefinitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            // Find parent group box
            RibbonGroupBox groupBox = FindParentRibbonGroupBox(d);
            if (groupBox != null)
            {
                SetAppropriateSize((UIElement) d, groupBox.State);
            }
            else SetAppropriateSize((UIElement)d, RibbonGroupBoxState.Large);
        }

        // Finds parent group box
        static RibbonGroupBox FindParentRibbonGroupBox(DependencyObject o)
        {
            while (!(o is RibbonGroupBox)) { o = VisualTreeHelper.GetParent(o); if (o == null) break; }
            return o == null ? null : (RibbonGroupBox)o;
        }

        /// <summary>
        /// Sets appropriate size of the control according to the 
        /// given group box state and control's size definition
        /// </summary>
        /// <param name="element">UI Element</param>
        /// <param name="state">Group box state</param>
        public static void SetAppropriateSize(UIElement element, RibbonGroupBoxState state)
        {
            int index = (int)state;
            if (state == RibbonGroupBoxState.Collapsed) index = 0;
            SetSize(element, GetThreeSizeDefinition(element)[index]);
        }

        /// <summary>
        /// Sets value of attached property SizeDefinition for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetSizeDefinition(UIElement element, string value)
        {
            element.SetValue(SizeDefinitionProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property SizeDefinition of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Size Definition"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Ribbon Control Properties"),
        System.ComponentModel.Description("Enumerate using comma what sizes need to be used from bigger to larger. For example: Large, Middle, Small")]
        public static string GetSizeDefinition(UIElement element)
        {
            return (string)element.GetValue(SizeDefinitionProperty);
        }

        /// <summary>
        /// Gets value of the attached property SizeDefinition of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static RibbonControlSize[] GetThreeSizeDefinition(UIElement element)
        {
            string[] splitted = (GetSizeDefinition(element)).Split(new char[] { ' ', ',', ';', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
            
            int count = Math.Min(splitted.Length, 3);
            if (count == 0) return new RibbonControlSize[] { RibbonControlSize.Large, RibbonControlSize.Large, RibbonControlSize.Large };

            RibbonControlSize[] sizes = new RibbonControlSize[3];
            for (int i = 0; i < count; i++)
            {
                switch(splitted[i])
                {
                    case "Large": sizes[i] = RibbonControlSize.Large; break;
                    case "Middle": sizes[i] = RibbonControlSize.Middle; break;
                    case "Small": sizes[i] = RibbonControlSize.Small; break;
                    default: sizes[i] = RibbonControlSize.Large; break;
                }
            }
            for (int i = count; i < 3; i++)
            {
                sizes[i] = sizes[count - 1];
            }
            return sizes;
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
          new FrameworkPropertyMetadata(new BitmapImage(new Uri("pack://application:,,,/Fluent;component/Images/DefaultSmallIcon.png")),
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
        [System.ComponentModel.DisplayName("Large Icon"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Ribbon Control Properties"),
        System.ComponentModel.Description("Large Icon of the control to use it in a style")]
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
          new FrameworkPropertyMetadata(new BitmapImage(new Uri("pack://application:,,,/Fluent;component/Images/DefaultSmallIcon.png")),
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
        [System.ComponentModel.DisplayName("Small Icon"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Ribbon Control Properties"),
        System.ComponentModel.Description("Small Icon of the control to use it in a style")]
        public static ImageSource GetSmallIcon(UIElement element)
        {
            return (ImageSource)element.GetValue(SmallIconProperty);
        }

        #endregion
        
        #endregion
    }
}
