#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    /// <summary>
    /// Represents title bar
    /// </summary>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RibbonContextualTabGroup))]
    [TemplatePart(Name = "PART_QuickAccessToolbarHolder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_HeaderHolder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ItemsContainer", Type = typeof(Panel))]
    public class RibbonTitleBar : HeaderedItemsControl
    {
        #region Fields

        // Quick access toolbar holder
        private FrameworkElement quickAccessToolbarHolder;
        // Header holder
        private FrameworkElement headerHolder;
        // Items container
        private Panel itemsContainer;
        // Quick access toolbar rect
        private Rect quickAccessToolbarRect;
        // Header rect
        private Rect headerRect;
        // Items rect
        private Rect itemsRect;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets quick access toolbar
        /// </summary>
        public UIElement QuickAccessToolBar
        {
            get { return (UIElement)GetValue(QuickAccessToolBarProperty); }
            set { SetValue(QuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for QuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarProperty =
            DependencyProperty.Register("QuickAccessToolBar", typeof(UIElement), typeof(RibbonTitleBar), new UIPropertyMetadata(null, OnQuickAccessToolbarChanged));

        // Handles QuickAccessToolBar property chages
        private static void OnQuickAccessToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var titleBar = (RibbonTitleBar)d;
            titleBar.InvalidateMeasure();
        }

        /// <summary>
        /// Gets or sets header alignment
        /// </summary>
        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)GetValue(HeaderAlignmentProperty); }
            set { SetValue(HeaderAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HeaderAlignment.  This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty HeaderAlignmentProperty =
            DependencyProperty.Register("HeaderAlignment", typeof(HorizontalAlignment), typeof(RibbonTitleBar), new UIPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Defines whether title bar is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)GetValue(IsCollapsedProperty); }
            set { SetValue(IsCollapsedProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="IsCollapsed"/>
        /// </summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register("IsCollapsed", typeof(bool), typeof(RibbonTitleBar), new PropertyMetadata(false, OnIsCollapsedChanged));

        private bool isAtLeastOneRequiredControlPresent;

        private static void OnIsCollapsedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var titleBar = (RibbonTitleBar)d;

            var ribbonOwnerWindow = Window.GetWindow(titleBar) as RibbonWindow;

            if (ribbonOwnerWindow != null)
            {
                // Setting height to the exact same value as GlassBorderThickness of the owning RibbonWindow
                // We have to do this to fix a problem with the content area
                // to see what this means, comment out the code below and decrease the window size till the ribbon gets collapsed
                // We could set this via Style-Triggers in the ribbon, but this causes other layout problems when the ribbon is collapsed and then the window is maximized
                titleBar.Height = (bool)e.NewValue
                    ? ribbonOwnerWindow.GlassBorderThickness.Top
                    : 25;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonTitleBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTitleBar), new FrameworkPropertyMetadata(typeof(RibbonTitleBar)));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonContextualTabGroup();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item"> The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonContextualTabGroup);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes 
        /// call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.quickAccessToolbarHolder = this.GetTemplateChild("PART_QuickAccessToolbarHolder") as FrameworkElement;
            this.headerHolder = this.GetTemplateChild("PART_HeaderHolder") as FrameworkElement;
            this.itemsContainer = this.GetTemplateChild("PART_ItemsContainer") as Panel;

            this.isAtLeastOneRequiredControlPresent = this.quickAccessToolbarHolder != null
                                     || this.headerHolder != null
                                     || this.itemsContainer != null;
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.isAtLeastOneRequiredControlPresent == false)
            {
                return base.MeasureOverride(constraint);
            }

            if (this.IsCollapsed)
            {
                return base.MeasureOverride(constraint);
            }

            var resultSize = constraint;

            if (double.IsPositiveInfinity(resultSize.Width)
                || double.IsPositiveInfinity(resultSize.Height))
            {
                resultSize = base.MeasureOverride(resultSize);
            }

            this.Update(resultSize);

            this.itemsContainer.Measure(this.itemsRect.Size);
            this.headerHolder.Measure(this.headerRect.Size);
            this.quickAccessToolbarHolder.Measure(this.quickAccessToolbarRect.Size);

            return resultSize;
        }

        /// <summary>
        /// Called to arrange and size the content of a System.Windows.Controls.Control object.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size of the control.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (this.isAtLeastOneRequiredControlPresent == false)
            {
                return base.ArrangeOverride(arrangeBounds);
            }

            if (this.IsCollapsed)
            {
                return base.ArrangeOverride(arrangeBounds);
            }

            this.itemsContainer.Arrange(this.itemsRect);
            this.headerHolder.Arrange(this.headerRect);
            this.quickAccessToolbarHolder.Arrange(this.quickAccessToolbarRect);
            return arrangeBounds;
        }

        #endregion

        #region Private methods

        // Update items size and positions
        private void Update(Size constraint)
        {
            var visibleGroups = this.Items.OfType<RibbonContextualTabGroup>()
                .Where(group => group.InnerVisibility == Visibility.Visible && group.Items.Count > 0)
                .ToList();

            var infinity = new Size(double.PositiveInfinity, double.PositiveInfinity);

            var canRibbonTabControlScroll = false;

            // Defensively try to find out if the RibbonTabControl can scroll
            if (visibleGroups.Count > 0)
            {
                var firstVisibleItem = visibleGroups.First().FirstVisibleItem;

                if (firstVisibleItem != null
                    && firstVisibleItem.Parent != null)
                {
                    canRibbonTabControlScroll = ((RibbonTabControl)firstVisibleItem.Parent).CanScroll;
                }
            }

            if (visibleGroups.Count == 0
                || canRibbonTabControlScroll)
            {
                // Collapse itemRect
                itemsRect = new Rect(0, 0, 0, 0);
                // Set quick launch toolbar and header position and size
                quickAccessToolbarHolder.Measure(infinity);

                if (constraint.Width <= quickAccessToolbarHolder.DesiredSize.Width + 50)
                {
                    quickAccessToolbarRect = new Rect(0, 0, Math.Max(0, constraint.Width - 50), quickAccessToolbarHolder.DesiredSize.Height);
                    quickAccessToolbarHolder.Measure(quickAccessToolbarRect.Size);
                }

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
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth / 2 - headerHolder.DesiredSize.Width / 2), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
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
                    headerRect = new Rect(Math.Max(0, constraint.Width - 50), 0, 50, constraint.Height);
                }
            }
            else
            {
                // Set items container size and position
                var firstItem = visibleGroups.First().FirstVisibleItem;
                var lastItem = visibleGroups.Last().LastVisibleItem;

                var startX = firstItem.TranslatePoint(new Point(0, 0), this).X;
                var endX = lastItem.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X;

                //Get minimum x point (workaround)
                foreach (var group in visibleGroups)
                {
                    firstItem = group.FirstVisibleItem;

                    if (firstItem != null)
                    {
                        if (firstItem.TranslatePoint(new Point(0, 0), this).X < startX)
                        {
                            startX = firstItem.TranslatePoint(new Point(0, 0), this).X;
                        }
                    }

                    lastItem = group.LastVisibleItem;

                    if (lastItem != null)
                    {
                        if (lastItem.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X > endX)
                        {
                            endX = lastItem.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X;
                        }
                    }
                }
                //double startX = firstItem.TranslatePoint(new Point(0, 0), this).X;
                //double endX = lastItem.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X;

                //Looks like thr titlebar things are ordered in an other way

                if (startX != endX)
                {
                    itemsRect = new Rect(startX, 0, Math.Max(0, Math.Min(endX, constraint.Width) - startX), constraint.Height);
                }

                // Set quick launch toolbar position and size
                quickAccessToolbarHolder.Measure(infinity);

                var quickAccessToolbarWidth = quickAccessToolbarHolder.DesiredSize.Width;
                quickAccessToolbarRect = new Rect(0, 0, Math.Min(quickAccessToolbarWidth, startX), quickAccessToolbarHolder.DesiredSize.Height);

                if (quickAccessToolbarWidth > startX)
                {
                    quickAccessToolbarHolder.Measure(quickAccessToolbarRect.Size);
                    quickAccessToolbarRect = new Rect(0, 0, quickAccessToolbarHolder.DesiredSize.Width, quickAccessToolbarHolder.DesiredSize.Height);
                    quickAccessToolbarWidth = quickAccessToolbarHolder.DesiredSize.Width;
                }
                // Set header
                headerHolder.Measure(infinity);
                if (HeaderAlignment == HorizontalAlignment.Left)
                {
                    if (startX - quickAccessToolbarWidth > 150)
                    {
                        var allTextWidth = startX - quickAccessToolbarWidth;
                        headerRect = new Rect(quickAccessToolbarRect.Width, 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else
                    {
                        var allTextWidth = Math.Max(0, constraint.Width - endX);
                        headerRect = new Rect(Math.Min(endX, constraint.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                }
                else if (HeaderAlignment == HorizontalAlignment.Center)
                {
                    if ((startX - quickAccessToolbarWidth < 150 && startX - quickAccessToolbarWidth > 0 && startX - quickAccessToolbarWidth < constraint.Width - endX)
                        || endX < constraint.Width / 2)
                    {
                        var allTextWidth = Math.Max(0, constraint.Width - endX);
                        headerRect = new Rect(Math.Min(Math.Max(endX, constraint.Width / 2 - headerHolder.DesiredSize.Width / 2), constraint.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else
                    {
                        var allTextWidth = Math.Max(0, startX - quickAccessToolbarWidth);
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth / 2 - headerHolder.DesiredSize.Width / 2), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                }
                else if (HeaderAlignment == HorizontalAlignment.Right)
                {
                    if (startX - quickAccessToolbarWidth > 150)
                    {
                        var allTextWidth = Math.Max(0, startX - quickAccessToolbarWidth);
                        headerRect = new Rect(quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth - headerHolder.DesiredSize.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else
                    {
                        var allTextWidth = Math.Max(0, constraint.Width - endX);
                        headerRect = new Rect(Math.Min(Math.Max(endX, constraint.Width - headerHolder.DesiredSize.Width), constraint.Width), 0, Math.Min(allTextWidth, headerHolder.DesiredSize.Width), constraint.Height);
                    }
                }
                else if (HeaderAlignment == HorizontalAlignment.Stretch)
                {
                    if (startX - quickAccessToolbarWidth > 150)
                    {
                        var allTextWidth = startX - quickAccessToolbarWidth;
                        headerRect = new Rect(quickAccessToolbarRect.Width, 0, allTextWidth, constraint.Height);
                    }
                    else
                    {
                        var allTextWidth = Math.Max(0, constraint.Width - endX);
                        headerRect = new Rect(Math.Min(endX, constraint.Width), 0, allTextWidth, constraint.Height);
                    }
                }
            }

            headerRect.Width = headerRect.Width + 2;
        }

        #endregion
    }
}
