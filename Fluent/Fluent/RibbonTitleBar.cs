using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RibbonContextualTabGroup))]
    [TemplatePart(Name = "PART_QuickAccessToolbarHolder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_HeaderHolder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ItemsContainer", Type = typeof(Panel))]
    public class RibbonTitleBar: HeaderedItemsControl
    {
        #region Fields

        private FrameworkElement quickAccessToolbarHolder = null;
        private FrameworkElement headerHolder = null;
        private Panel itemsContainer = null;

        #endregion

        #region Properties

        public UIElement QuickLaunchToolbar
        {
            get { return (UIElement)GetValue(QuickLaunchToolbarProperty); }
            set { SetValue(QuickLaunchToolbarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuickLaunchToolbar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuickLaunchToolbarProperty =
            DependencyProperty.Register("QuickLaunchToolbar", typeof(UIElement), typeof(RibbonTitleBar), new UIPropertyMetadata(null));


        #endregion

        #region Initialize

        static RibbonTitleBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTitleBar), new FrameworkPropertyMetadata(typeof(RibbonTitleBar)));
        }

        public RibbonTitleBar()
        {
            
        }

        #endregion

        #region Overrides

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonContextualTabGroup();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonContextualTabGroup);
        }

        public override void OnApplyTemplate()
        {
            quickAccessToolbarHolder = GetTemplateChild("PART_QuickAccessToolbarHolder") as FrameworkElement;
            headerHolder = GetTemplateChild("PART_HeaderHolder") as FrameworkElement;
            itemsContainer = GetTemplateChild("PART_ItemsContainer") as Panel;
        }
        
        #endregion
    }
}
