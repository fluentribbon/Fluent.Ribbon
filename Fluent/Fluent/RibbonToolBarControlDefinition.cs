using System;
using System.Windows;

namespace Fluent
{
    /// <summary>
    /// Represent logical definition for a control in toolbar
    /// </summary>
    public class RibbonToolBarControlDefinition : DependencyObject
    {
        #region Fields


        #endregion

        #region Properties

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
          "Size",
          typeof(RibbonControlSize),
          typeof(RibbonToolBarControlDefinition),
          new FrameworkPropertyMetadata(RibbonControlSize.Small)
        );

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region Target Property
        
        /// <summary>
        /// Gets or sets name of the target control
        /// </summary>
        public string Target
        {
            get { return (string)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ControlName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(string),
            typeof(RibbonToolBarControlDefinition), new UIPropertyMetadata(null));

        #endregion

        #region Width Property
        
        /// <summary>
        /// Gets or sets width of the target control
        /// </summary>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Width. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(RibbonToolBarControlDefinition), new UIPropertyMetadata(Double.NaN));

        #endregion

        #endregion

        internal void Invalidate()
        {
            throw new NotImplementedException();
        }
    }
}