using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    public class GroupingPanel : Panel
    {
        #region Fields

        GroupHeader header = new GroupHeader();

        List<List<object>> objectsGroups = new List<List<object>>();

        #endregion

        #region Properties

        #region GroupBy

        /// <summary>
        /// Gets or sets property name to group items
        /// </summary>
        public string GroupBy
        {
            get { return (string)GetValue(GroupByProperty); }
            set { SetValue(GroupByProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupBy.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(string), typeof(GroupingPanel), new UIPropertyMetadata(null));
       
        #endregion

        #region Orientation

        /// <summary>
        /// Gets or sets panel orientation
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(GroupingPanel), new UIPropertyMetadata(System.Windows.Controls.Orientation.Horizontal));
                
        #endregion

        #region GroupHeaderStyle

        /// <summary>
        /// Gets or sets group header style
        /// </summary>
        public Style GroupHeaderStyle
        {
            get { return (Style)GetValue(GroupHeaderStyleProperty); }
            set { SetValue(GroupHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupHeaderStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupHeaderStyleProperty =
            DependencyProperty.Register("GroupHeaderStyle", typeof(Style), typeof(GroupingPanel), new UIPropertyMetadata(null));
        
        #endregion

        #endregion

        #region Overrides

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            
            return base.MeasureOverride(availableSize);
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(System.Windows.Media.DrawingContext dc)
        {
            base.OnRender(dc);
        }

        #endregion
    }
}
