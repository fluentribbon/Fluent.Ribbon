using System.ComponentModel;
using System.Windows;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Extensibility;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represent logical definition for a control in toolbar
    /// </summary>
    public class RibbonToolBarControlDefinition : DependencyObject, INotifyPropertyChanged, IRibbonSizeChangedSink
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public RibbonToolBarControlDefinition()
        {
            RibbonProperties.SetSize(this, RibbonControlSize.Small);
        }

        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RibbonToolBarControlDefinition));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RibbonToolBarControlDefinition));

        #endregion

        #region Target Property

        /// <summary>
        /// Gets or sets name of the target control
        /// </summary>
        public string Target
        {
            get { return (string)this.GetValue(TargetProperty); }
            set { this.SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ControlName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(string),
            typeof(RibbonToolBarControlDefinition), new PropertyMetadata(OnTargetPropertyChanged));

        private static void OnTargetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = (RibbonToolBarControlDefinition)d;
            definition.Invalidate(nameof(Target));
        }

        #endregion

        #region Width Property

        /// <summary>
        /// Gets or sets width of the target control
        /// </summary>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Width. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(double), typeof(RibbonToolBarControlDefinition), new PropertyMetadata(DoubleBoxes.NaN, OnWidthPropertyChanged));

        private static void OnWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = (RibbonToolBarControlDefinition)d;
            definition.Invalidate(nameof(Width));
        }

        #endregion

        #region Invalidating

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void Invalidate(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Implementation of IRibbonSizeChangedSink

        /// <summary>
        /// Called when the size is changed
        /// </summary>
        /// <param name="previous">Size before change</param>
        /// <param name="current">Size after change</param>
        public void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
        {
            this.Invalidate("Size");
        }

        #endregion
    }
}