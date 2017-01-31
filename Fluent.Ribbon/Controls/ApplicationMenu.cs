using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents backstage button
    /// </summary>
    public class ApplicationMenu : DropDownButton
    {
        #region Properties

        /// <summary>
        /// Gets or sets width of right content
        /// </summary>
        public double RightPaneWidth
        {
            get { return (double)this.GetValue(RightPaneWidthProperty); }
            set { this.SetValue(RightPaneWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RightContentWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightPaneWidthProperty =
            DependencyProperty.Register(nameof(RightPaneWidth), typeof(double), typeof(ApplicationMenu), new PropertyMetadata(300.0));

        /// <summary>
        /// Gets or sets application menu right pane content
        /// </summary>
        public object RightPaneContent
        {
            get { return this.GetValue(RightPaneContentProperty); }
            set { this.SetValue(RightPaneContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RightContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightPaneContentProperty =
            DependencyProperty.Register(nameof(RightPaneContent), typeof(object), typeof(ApplicationMenu), new PropertyMetadata());

        /// <summary>
        /// Gets or sets application menu bottom pane content
        /// </summary>
        public object FooterPaneContent
        {
            get { return this.GetValue(FooterPaneContentProperty); }
            set { this.SetValue(FooterPaneContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BottomContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FooterPaneContentProperty =
            DependencyProperty.Register(nameof(FooterPaneContent), typeof(object), typeof(ApplicationMenu), new PropertyMetadata());

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ApplicationMenu()
        {
            var type = typeof(ApplicationMenu);

            // Override style metadata
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            // Disable QAT for this control
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
            // Make default KeyTip
            KeyTipProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, CoerceKeyTipKeys));
        }

        private static object CoerceKeyTipKeys(DependencyObject d, object basevalue)
        {
            return basevalue ?? RibbonLocalization.Current.Localization.BackstageButtonKeyTip;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationMenu()
        {
            this.CoerceValue(KeyTipProperty);
      this.Loaded += ApplicationMenu_Loaded;
        }

    private void ApplicationMenu_Loaded(object sender, RoutedEventArgs e)
    {
      this.ApplyTemplate();
      this.UpdateLayout();
    }

    #endregion

    #region Quick Access Toolbar

    /// <summary>
    /// Gets control which represents shortcut item.
    /// This item MUST be syncronized with the original 
    /// and send command to original one control.
    /// </summary>
    /// <returns>Control which represents shortcut item</returns>
    public override FrameworkElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        #endregion

      
    }
}