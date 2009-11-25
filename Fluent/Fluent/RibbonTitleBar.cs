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

        private Rect quickAccessToolbarRect;
        private Rect headerRect;
        private Rect itemsRect;

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

        protected override Size MeasureOverride(Size constraint)
        {
            if ((quickAccessToolbarHolder == null) || (headerHolder == null) || (itemsContainer == null)) return base.MeasureOverride(constraint);
            Update(constraint);
            
            itemsContainer.Measure(itemsRect.Size);

            return constraint;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if ((quickAccessToolbarHolder == null) || (headerHolder == null) || (itemsContainer == null)) return base.ArrangeOverride(arrangeBounds);
            itemsContainer.Arrange(itemsRect);
            return arrangeBounds;
        }

        #endregion

        #region Private methods

        private void Update(Size constraint)
        {
            List<RibbonContextualTabGroup> visibleGroups = new List<RibbonContextualTabGroup>();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] is RibbonContextualTabGroup)
                {
                    RibbonContextualTabGroup group = Items[i] as RibbonContextualTabGroup;
                    if ((group.Visibility == Visibility.Visible) && (group.Items.Count > 0)) visibleGroups.Add(group);
                }
            }

            if ((visibleGroups.Count == 0)||((visibleGroups[0].Items[0].Parent as RibbonTabControl).CanScroll))
            {
                itemsRect = new Rect(0,0,0,0);
            }
            else
            {
                RibbonTabItem firstItem = visibleGroups[0].Items[0];
                RibbonTabItem lastItem = visibleGroups[visibleGroups.Count - 1].Items[visibleGroups[visibleGroups.Count - 1].Items.Count - 1];

                double startX = firstItem.TranslatePoint(new Point(0, 0), this).X;
                double endX = lastItem.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X;

                itemsRect = new Rect(startX, 0, Math.Max(0, Math.Min(endX, constraint.Width) - startX), constraint.Height);
            }
        }

        #endregion
    }
}
