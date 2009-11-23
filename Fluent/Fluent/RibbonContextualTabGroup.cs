using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RibbonTabItem))]    
    [TemplatePart(Name = "PART_BorderTop", Type = typeof(Border))]
    [TemplatePart(Name = "PART_BorderBottom", Type = typeof(Border))]
    public class RibbonContextualTabGroup : HeaderedItemsControl
    {
        #region Fields

        private Border borderTop = null;
        private Border borderBottom = null;

        #endregion

        #region Initialize

        static RibbonContextualTabGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(typeof(RibbonContextualTabGroup)));
        }

        public RibbonContextualTabGroup()
        {
            
        }

        #endregion

        #region Overrides

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTabItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonTabItem);
        }

        public override void OnApplyTemplate()
        {
            borderTop = GetTemplateChild("PART_BorderTop") as Border;
            borderBottom = GetTemplateChild("PART_BorderBottom") as Border;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            if ((borderTop == null) || (borderBottom == null)) return base.MeasureOverride(constraint);
            Size infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);
            borderTop.Measure(infinity);
            borderBottom.Measure(infinity);
            double widthTop = borderTop.DesiredSize.Width;
            double widthBottom = borderBottom.DesiredSize.Width;
            double width = Math.Min(Math.Max(widthTop, widthBottom),constraint.Width);
            //TODO: need to conside width of caption buttons for top border
            borderTop.Measure(new Size(width,double.PositiveInfinity));
            borderBottom.Measure(new Size(width, double.PositiveInfinity));
            return new Size(width, borderTop.DesiredSize.Height + borderBottom.DesiredSize.Height);
        }

        #endregion
    }
}
