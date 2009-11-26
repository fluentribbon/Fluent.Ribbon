using System;
using System.Collections;
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

        public UIElement QuickAccessToolbar
        {
            get { return (UIElement)GetValue(QuickAccessToolbarProperty); }
            set { SetValue(QuickAccessToolbarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for QuickAccessToolbar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty QuickAccessToolbarProperty =
            DependencyProperty.Register("QuickAccessToolbar", typeof(UIElement), typeof(RibbonTitleBar), new UIPropertyMetadata(null,OnQuickAccessToolbarChanged));

        private static void OnQuickAccessToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTitleBar titleBar = (RibbonTitleBar)d;

            UIElement oldToolbar = e.OldValue as UIElement;
            UIElement newToolbar = e.NewValue as UIElement;

            // Remove Logical tree link
            if (oldToolbar != null)
            {
                titleBar.RemoveLogicalChild(oldToolbar);
            }

            // Add Logical tree link
            if (newToolbar != null)
            {
                titleBar.AddLogicalChild(newToolbar);
            }        
        }

        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderAlignmentProperty); }
            set { SetValue(HeaderAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderAlignmentProperty =
            DependencyProperty.Register("HeaderAlignment", typeof(HorizontalAlignment), typeof(RibbonTitleBar), new UIPropertyMetadata(HorizontalAlignment.Center));

        #endregion

        #region Protected Properties

        /// <summary>
        ///   Gets an enumerator for the Ribbon's logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get { return new RibbonTitleBarLogicalChildrenEnumerator(this.Header as UIElement, this.QuickAccessToolbar, this.Items); }
        }

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
            Size resultSize = constraint;
            if((double.IsPositiveInfinity(resultSize.Width))||(double.IsPositiveInfinity(resultSize.Height))) resultSize = base.MeasureOverride(resultSize);
            Update(resultSize);
            
            itemsContainer.Measure(itemsRect.Size);
            headerHolder.Measure(headerRect.Size);
            quickAccessToolbarHolder.Measure(quickAccessToolbarRect.Size);

            return resultSize;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if ((quickAccessToolbarHolder == null) || (headerHolder == null) || (itemsContainer == null)) return base.ArrangeOverride(arrangeBounds);
            itemsContainer.Arrange(itemsRect);
            headerHolder.Arrange(headerRect);
            quickAccessToolbarHolder.Arrange(quickAccessToolbarRect);
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

            Size infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);

            if ((visibleGroups.Count == 0)||((visibleGroups[0].Items[0].Parent as RibbonTabControl).CanScroll))
            {
                // Collapse itemRect
                itemsRect = new Rect(0, 0, 0, 0);
                // Set quick launch toolbar and header position and size
                quickAccessToolbarHolder.Measure(infinity);
                if (constraint.Width > quickAccessToolbarHolder.DesiredSize.Width + 50)
                {
                    quickAccessToolbarRect = new Rect(0, 0, quickAccessToolbarHolder.DesiredSize.Width, quickAccessToolbarHolder.DesiredSize.Height);
                    headerHolder.Measure(infinity);
                    double allTextWidth = constraint.Width - quickAccessToolbarHolder.DesiredSize.Width;
                    if (HeaderAlignment == HorizontalAlignment.Left)
                    {
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width, 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (HeaderAlignment == HorizontalAlignment.Center)
                    {
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth / 2 - headerHolder.DesiredSize.Width/2), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (HeaderAlignment == HorizontalAlignment.Right)
                    {
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth - headerHolder.DesiredSize.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (HeaderAlignment == HorizontalAlignment.Stretch)
                    {
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width, 0, allTextWidth, constraint.Height);
                    }                    
                }
                else
                {
                    quickAccessToolbarRect = new Rect(0, 0, Math.Max(0, constraint.Width-50), quickAccessToolbarHolder.DesiredSize.Height);
                    headerRect = new Rect(Math.Max(0, constraint.Width - 50), 0, 50, constraint.Height);
                }
            }
            else
            {
                // Set items container size and position
                RibbonTabItem firstItem = visibleGroups[0].Items[0];
                RibbonTabItem lastItem = visibleGroups[visibleGroups.Count - 1].Items[visibleGroups[visibleGroups.Count - 1].Items.Count - 1];

                double startX = firstItem.TranslatePoint(new Point(0, 0), this).X;
                double endX = lastItem.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X;

                itemsRect = new Rect(startX, 0, Math.Max(0, Math.Min(endX, constraint.Width) - startX), constraint.Height);
                // Set quick launch toolbar position and size
                quickAccessToolbarHolder.Measure(infinity);
                double quickAccessToolbarWidth = quickAccessToolbarHolder.DesiredSize.Width;
                quickAccessToolbarRect = new Rect(0, 0, Math.Min(quickAccessToolbarWidth, startX), quickAccessToolbarHolder.DesiredSize.Height);
                // Set header
                headerHolder.Measure(infinity);
                if(HeaderAlignment==HorizontalAlignment.Left)
                {
                    if(startX-quickAccessToolbarWidth>150)
                    {
                        double allTextWidth = startX - quickAccessToolbarWidth;
                        headerRect = new Rect(quickAccessToolbarRect.Width, 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else
                    {
                        double allTextWidth = Math.Max(0,constraint.Width-endX);
                        headerRect = new Rect(Math.Min(endX,constraint.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                }
                else if(HeaderAlignment==HorizontalAlignment.Center)
                {
                    if (((startX - quickAccessToolbarWidth < 150) && (startX - quickAccessToolbarWidth > 0) && (startX - quickAccessToolbarWidth < constraint.Width - endX)) || (endX < constraint.Width / 2))
                    {
                        double allTextWidth = Math.Max(0, constraint.Width - endX);
                        headerRect = new Rect(Math.Min(Math.Max(endX, constraint.Width / 2 - headerHolder.DesiredSize.Width / 2), constraint.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);                        
                    }
                    else
                    {
                        double allTextWidth = Math.Max(0,startX - quickAccessToolbarWidth);
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth / 2 - headerHolder.DesiredSize.Width / 2), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                }
                else if (HeaderAlignment == HorizontalAlignment.Right)
                {
                    if (startX - quickAccessToolbarWidth > 150)
                    {
                        double allTextWidth = Math.Max(0,startX - quickAccessToolbarWidth);
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth - headerHolder.DesiredSize.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else
                    {
                        double allTextWidth = Math.Max(0, constraint.Width - endX);
                        headerRect = new Rect(Math.Min(Math.Max(endX, constraint.Width - headerHolder.DesiredSize.Width), constraint.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                }
                else if(HeaderAlignment==HorizontalAlignment.Stretch)
                {
                    if(startX-quickAccessToolbarWidth>150)
                    {
                        double allTextWidth = startX - quickAccessToolbarWidth;
                        headerRect = new Rect(quickAccessToolbarRect.Width, 0, allTextWidth, constraint.Height);
                    }
                    else
                    {
                        double allTextWidth = Math.Max(0,constraint.Width-endX);
                        headerRect = new Rect(Math.Min(endX, constraint.Width), 0, allTextWidth, constraint.Height);
                    }
                }
                
            }
        }

        #endregion

        #region RibbonLogicalChildEnumerator Class

        /// <summary>
        ///   An enumerator for the logical children of the Ribbon.
        /// </summary>
        private class RibbonTitleBarLogicalChildrenEnumerator : IEnumerator
        {
            #region Fields

            private UIElement toolbar;

            private UIElement header;

            /// <summary>
            ///   The Ribbon's collection of tabs.
            /// </summary>
            private ItemCollection items;

            /// <summary>
            ///   The current position of enumeration.
            /// </summary>
            private Position postition;

            /// <summary>
            ///   The current tab index if we are currently enumerating the Ribbon's tabs.
            /// </summary>
            private int index = 0;

            #endregion

            #region Constructors

            public RibbonTitleBarLogicalChildrenEnumerator(UIElement header, UIElement toolbar, ItemCollection items)
            {
                postition = Position.None;
                this.toolbar = toolbar;
                this.toolbar = header;
                this.items = items;
            }

            #endregion

            #region Position Enum

            /// <summary>
            ///   An enum indicating the current position of enumeration.
            /// </summary>
            private enum Position
            {
                /// <summary>
                ///   Indicates that the enumeration is not currently within the Ribbon's
                ///   logical children.
                /// </summary>
                None,

                Toolbar,
                Header,

                /// <summary>
                ///   Indicates enumeration is currently at the QuickAccessToolbar.
                /// </summary>
                Items
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///   Gets the object at the enumerators current position.
            /// </summary>
            public object Current
            {
                get
                {
                    switch (postition)
                    {
                        case Position.Toolbar:
                            return toolbar;
                        case Position.Header:
                            return header;
                        case Position.Items:
                            return items[index];
                    }

                    throw new InvalidOperationException();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///   Advances the enumerator to the next logical child of the Ribbon.
            /// </summary>
            /// <returns>True if the enumerator was successfully advanced, false otherwise.</returns>
            public bool MoveNext()
            {
                if (postition == Position.None)
                {
                    postition = Position.Header;
                    return true;                    
                }
                if (postition == Position.Header)
                {
                    postition = Position.Toolbar;                    
                    return true;
                }

                if (postition == Position.Toolbar)
                {
                    postition = Position.Items;
                    if ((items != null)&&(items.Count>0))
                    {
                        return true;
                    }
                }

                if (postition == Position.Items)
                {
                    if (index < items.Count - 2)
                    {
                        index++;
                        return true;
                    }
                }

                this.Reset();

                return false;
            }

            /// <summary>
            ///   Resets the RibbonLogicalChildrenEnumerator.
            /// </summary>
            public void Reset()
            {
                postition = Position.None;
                index = 0;
            }

            #endregion
        }

        #endregion
    }
}
