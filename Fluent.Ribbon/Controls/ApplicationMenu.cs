// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents backstage button
    /// </summary>
    public class ApplicationMenu : DropDownButton
    {
        private static readonly PropertyInfo targetElementPropertyInfo = typeof(ContextMenuEventArgs).GetProperty("TargetElement", BindingFlags.Instance | BindingFlags.NonPublic);

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
        public static readonly DependencyProperty RightPaneWidthProperty = DependencyProperty.Register(nameof(RightPaneWidth), typeof(double), typeof(ApplicationMenu), new PropertyMetadata(300.0));

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
        public static readonly DependencyProperty RightPaneContentProperty = DependencyProperty.Register(nameof(RightPaneContent), typeof(object), typeof(ApplicationMenu), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

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
        public static readonly DependencyProperty FooterPaneContentProperty = DependencyProperty.Register(nameof(FooterPaneContent), typeof(object), typeof(ApplicationMenu), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
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
        }

        #endregion

        /// <inheritdoc />
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            if (ReferenceEquals(e.Source, this))
            {
                var targetElement = targetElementPropertyInfo?.GetValue(e);
                if (targetElement == null
                    || ReferenceEquals(targetElement, this))
                {
                    e.Handled = true;
                    return;
                }
            }

            base.OnContextMenuOpening(e);
        }

        #region Quick Access Toolbar

        /// <inheritdoc />
        public override FrameworkElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var baseEnumerator = base.LogicalChildren;
                while (baseEnumerator?.MoveNext() == true)
                {
                    yield return baseEnumerator.Current;
                }

                if (this.RightPaneContent != null)
                {
                    yield return this.RightPaneContent;
                }

                if (this.FooterPaneContent != null)
                {
                    yield return this.FooterPaneContent;
                }
            }
        }
    }
}