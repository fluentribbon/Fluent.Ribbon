using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// This interface must be implemented for controls
    /// which are intended to insert to quick access toolbar
    /// </summary>
    public interface IQuickAccessItemProvider
    {
        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        FrameworkElement CreateQuickAccessItem();

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        bool CanAddToQuickAccessToolBar { get; set; }
    }
    
    /// <summary>
    /// Peresents quick access shortcut to another control
    /// </summary>
    [ContentProperty(nameof(Target))]
    public class QuickAccessMenuItem : MenuItem
    {


    #region Fields

    internal Ribbon Ribbon;

        #endregion

        #region Initialization

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static QuickAccessMenuItem()
        {
            IsCheckableProperty.AddOwner(typeof(QuickAccessMenuItem), new FrameworkPropertyMetadata(BooleanBoxes.TrueBox));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public QuickAccessMenuItem()
        {
            this.Checked += this.OnChecked;
            this.Unchecked += this.OnUnchecked;
            this.Loaded += this.OnFirstLoaded;
            this.Loaded += this.OnItemLoaded;            
        }

    #endregion

    #region Target Property


    public string TargetName
    {
      get { return (string)GetValue(TargetNameProperty); }
      set { SetValue(TargetProperty, value); }
    }

    /// <summary>
    /// Using a DependencyProperty as the backing store for shortcut. 
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty TargetNameProperty =
        DependencyProperty.Register("TargetName", typeof(string), typeof(QuickAccessMenuItem));


    /// <summary>
    /// Gets or sets shortcut to the target control
    /// </summary>
    public Control Target
        {
            get { return (Control)this.GetValue(TargetProperty); }
            set { this.SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for shortcut. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(Control), typeof(QuickAccessMenuItem), new PropertyMetadata(OnTargetChanged));

        private static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var quickAccessMenuItem = (QuickAccessMenuItem)d;
            var ribbonControl = e.NewValue as IRibbonControl;

            if (quickAccessMenuItem.Header == null 
                && ribbonControl != null)
            {
                // Set Default Text Value
                RibbonControl.Bind(ribbonControl, quickAccessMenuItem, nameof(IRibbonControl.Header), HeaderProperty, BindingMode.OneWay);
            }
            if (quickAccessMenuItem.Icon == null
                && ribbonControl != null)
            {
              // Set Default Text Value
              RibbonControl.Bind(ribbonControl, quickAccessMenuItem, "Icon", IconProperty, BindingMode.OneWay);
            }

            if (ribbonControl != null)
            {
                var parent = LogicalTreeHelper.GetParent((DependencyObject)ribbonControl);
                if (parent == null)
                {
                    quickAccessMenuItem.AddLogicalChild(ribbonControl);
                }
            }

            var oldRibbonControl = e.OldValue as IRibbonControl;

            if (oldRibbonControl!=null)
            {
                var parent = LogicalTreeHelper.GetParent((DependencyObject)oldRibbonControl);
                if (ReferenceEquals(parent, quickAccessMenuItem))
                {
                    quickAccessMenuItem.RemoveLogicalChild(oldRibbonControl);
                }
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Target != null)
                {
                    var parent = LogicalTreeHelper.GetParent(this.Target);
                    if (ReferenceEquals(parent, this))
                    {
                        var list = new ArrayList { this.Target };
                        return list.GetEnumerator();
                    }
                }

                return base.LogicalChildren;
            }
        }

        #endregion

        #region Event Handlers

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (this.Ribbon != null)
            {
                this.Ribbon.AddToQuickAccessToolBar(this.Target);
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            if (this.Ribbon != null)
            {
                this.Ribbon.RemoveFromQuickAccessToolBar(this.Target);
            }
        }

        private void OnItemLoaded(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded)
            {
                return;
            }

            if (this.Ribbon != null)
            {
                this.IsChecked = this.Ribbon.IsInQuickAccessToolBar(this.Target);
            }
        }

        private void OnFirstLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnFirstLoaded;
            if (this.IsChecked 
                && this.Ribbon != null)
            {
                this.Ribbon.AddToQuickAccessToolBar(this.Target);
            }
        }

        #endregion
    }

    /// <summary>
    /// The class responds to mine controls for QuickAccessToolBar
    /// </summary>
    internal static class QuickAccessItemsProvider
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the given control can provide a quick access toolbar item
        /// </summary>
        /// <param name="element">Control</param>
        /// <returns>True if this control is able to provide
        /// a quick access toolbar item, false otherwise</returns>
        public static bool IsSupported(UIElement element)
        {
            var provider = element as IQuickAccessItemProvider;
            if (provider != null 
                && provider.CanAddToQuickAccessToolBar)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets control which represents quick access toolbar item
        /// </summary>
        /// <param name="element">Host control</param>
        /// <returns>Control which represents quick access toolbar item</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800")]
        public static FrameworkElement GetQuickAccessItem(UIElement element)
        {
            FrameworkElement result = null;

            // If control supports the interface just return what it provides 
            var provider = element as IQuickAccessItemProvider;
            if (provider != null 
                && provider.CanAddToQuickAccessToolBar)
            {
                result = ((IQuickAccessItemProvider)element).CreateQuickAccessItem();
            }

            // The control isn't supported
            if (result == null)
            {
                throw new ArgumentException("The contol " + element.GetType().Name + " is not able to provide a quick access toolbar item");
            }

            if (BindingOperations.IsDataBound(result, UIElement.VisibilityProperty) == false)
            {
                RibbonControl.Bind(element, result, nameof(UIElement.Visibility), UIElement.VisibilityProperty, BindingMode.OneWay);
            }

            if (BindingOperations.IsDataBound(result, UIElement.IsEnabledProperty) == false)
            {
                RibbonControl.Bind(element, result, nameof(UIElement.IsEnabled), UIElement.IsEnabledProperty, BindingMode.OneWay);
            }

            return result;
        }

        /// <summary>
        /// Finds the top supported control
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static FrameworkElement FindSupportedControl(Visual visual, Point point)
        {
            var result = VisualTreeHelper.HitTest(visual, point);
            if (result == null)
            {
                return null;
            }

            // Try to find in visual (or logical) tree
            var element = result.VisualHit as FrameworkElement;
            while (element != null)
            {
                if (IsSupported(element))
                {
                    return element;
                }

                var visualParent = VisualTreeHelper.GetParent(element) as FrameworkElement;
                var logicalParent = LogicalTreeHelper.GetParent(element) as FrameworkElement;
                element = visualParent ?? logicalParent;
            }

            return null;
        }

        #endregion
    }
}