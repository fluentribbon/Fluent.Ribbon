using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Fluent.Internal;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using ControlzEx.Microsoft.Windows.Shell;
    using Fluent.Internal.KnownBoxes;

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

        private double? _windowCommandWidth;
        private double WindowCommandWidth
        {
          get
          {
            if (_windowCommandWidth.HasValue)
              return _windowCommandWidth.Value;
            var windowCommands = UIHelper.FindVisualChildByName<FrameworkElement>(Window.GetWindow(this),"PART_WindowCommands");
            if (windowCommands == null || windowCommands.ActualWidth==0)
              return 110;
            _windowCommandWidth = windowCommands.ActualWidth;
            return _windowCommandWidth.Value;
          }
        }

        private double WindowWidth
        {
          get
          {
            var window = Window.GetWindow(this);
            if (window == null)
              return 0;
            return window.Width;
          }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets quick access toolbar
        /// </summary>
        public UIElement QuickAccessToolBar
                {
                    get { return (UIElement)this.GetValue(QuickAccessToolBarProperty); }
                    set { this.SetValue(QuickAccessToolBarProperty, value); }
                }

                /// <summary>
                /// Using a DependencyProperty as the backing store for QuickAccessToolBar.  This enables animation, styling, binding, etc...
                /// </summary>
                public static readonly DependencyProperty QuickAccessToolBarProperty =
                    DependencyProperty.Register(nameof(QuickAccessToolBar), typeof(UIElement), typeof(RibbonTitleBar), new PropertyMetadata(OnQuickAccessToolbarChanged));

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
                    get { return (HorizontalAlignment)this.GetValue(HeaderAlignmentProperty); }
                    set { this.SetValue(HeaderAlignmentProperty, value); }
                }

                /// <summary>
                /// Using a DependencyProperty as the backing store for HeaderAlignment.  This enables animation, styling, binding, etc...
                /// </summary> 
                public static readonly DependencyProperty HeaderAlignmentProperty =
                    DependencyProperty.Register(nameof(HeaderAlignment), typeof(HorizontalAlignment), typeof(RibbonTitleBar), new PropertyMetadata(HorizontalAlignment.Center));

                /// <summary>
                /// Defines whether title bar is collapsed
                /// </summary>
                public bool IsCollapsed
                {
                    get { return (bool)this.GetValue(IsCollapsedProperty); }
                    set { this.SetValue(IsCollapsedProperty, value); }
                }

                /// <summary>
                /// DependencyProperty for <see cref="IsCollapsed"/>
                /// </summary>
                public static readonly DependencyProperty IsCollapsedProperty =
                    DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(RibbonTitleBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

                private bool isAtLeastOneRequiredControlPresent;

                /// <summary>
                /// Using a DependencyProperty as the backing store for HideContextTabs.  This enables animation, styling, binding, etc...
                /// </summary>
                public static readonly DependencyProperty HideContextTabsProperty =
                    DependencyProperty.Register(nameof(HideContextTabs), typeof(bool), typeof(RibbonTitleBar), new PropertyMetadata(BooleanBoxes.FalseBox));

                /// <summary>
                ///  Gets or sets whether context tabs are hidden.
                /// </summary>
                public bool HideContextTabs
                {
                    get { return (bool)this.GetValue(HideContextTabsProperty); }
                    set { this.SetValue(HideContextTabsProperty, value); }
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
            return item is RibbonContextualTabGroup;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes 
        /// call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.quickAccessToolbarHolder = this.GetTemplateChild("PART_QuickAccessToolbarHolder") as FrameworkElement;
            this.headerHolder = this.GetTemplateChild("PART_HeaderHolder") as FrameworkElement;
            this.itemsContainer = this.GetTemplateChild("PART_ItemsContainer") as Panel;

            this.isAtLeastOneRequiredControlPresent = this.quickAccessToolbarHolder != null
                                     || this.headerHolder != null
                                     || this.itemsContainer != null;

            if (this.quickAccessToolbarHolder != null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(this.quickAccessToolbarHolder, true);
            }
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

            var maxHeight = Math.Max(Math.Max(this.itemsRect.Height, this.headerRect.Height), this.quickAccessToolbarRect.Height);
            var width = this.itemsRect.Width + this.headerRect.Width + this.quickAccessToolbarRect.Width;

            return new Size(width, maxHeight);
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

                if (firstVisibleItem?.Parent != null)
                {
                    canRibbonTabControlScroll = ((RibbonTabControl)firstVisibleItem.Parent).CanScroll;
                }
            }

            if (visibleGroups.Count == 0
                || canRibbonTabControlScroll)
            {
                // Collapse itemRect
                this.itemsRect = new Rect(0, 0, 0, 0);
                // Set quick launch toolbar and header position and size
                this.quickAccessToolbarHolder.Measure(infinity);

                if (constraint.Width <= this.quickAccessToolbarHolder.DesiredSize.Width + 50)
                {
                    this.quickAccessToolbarRect = new Rect(0, 0, Math.Max(0, constraint.Width - 50), this.quickAccessToolbarHolder.DesiredSize.Height);
                    this.quickAccessToolbarHolder.Measure(this.quickAccessToolbarRect.Size);
                }

                if (constraint.Width > this.quickAccessToolbarHolder.DesiredSize.Width + 50)
                {
                    this.quickAccessToolbarRect = new Rect(0, 0, this.quickAccessToolbarHolder.DesiredSize.Width, this.quickAccessToolbarHolder.DesiredSize.Height);
                    this.headerHolder.Measure(infinity);
                    var allTextWidth = constraint.Width - this.quickAccessToolbarHolder.DesiredSize.Width;

                    if (this.HeaderAlignment == HorizontalAlignment.Left)
                    {
                        this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width, 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (this.HeaderAlignment == HorizontalAlignment.Center)
                    {
                        double headerSpace = 25; 
                        double windowWidth = WindowWidth;
                        if (windowWidth > (this.quickAccessToolbarHolder.DesiredSize.Width + this.headerHolder.DesiredSize.Width+WindowCommandWidth))
                        {
                          if(this.quickAccessToolbarHolder.DesiredSize.Width + headerSpace < ((windowWidth / 2) - (this.headerHolder.DesiredSize.Width / 2)))
                            this.headerRect = new Rect((windowWidth / 2) - (this.headerHolder.DesiredSize.Width/2), 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                          else
                            this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + headerSpace, 0, Math.Min(allTextWidth -50 , this.headerHolder.DesiredSize.Width), constraint.Height);
                        }
                        else
                          this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + headerSpace + Math.Max(0, (allTextWidth- headerSpace) / 2 - this.headerHolder.DesiredSize.Width / 2), 0, Math.Min(allTextWidth-headerSpace, this.headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (this.HeaderAlignment == HorizontalAlignment.Right)
                    {
                        this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth - this.headerHolder.DesiredSize.Width), 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (this.HeaderAlignment == HorizontalAlignment.Stretch)
                    {
                        this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width, 0, allTextWidth, constraint.Height);
                    }
                }
                else
                {
                    this.headerRect = new Rect(Math.Max(0, constraint.Width - 50), 0, 50, constraint.Height);
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

                // Ensure that startX and endX are never negative
                startX = Math.Max(0, startX);
                endX = Math.Max(0, endX);

                //Looks like thr titlebar things are ordered in an other way
                if (DoubleUtil.AreClose(startX, endX) == false)
                {
                    this.itemsRect = new Rect(startX, 0, Math.Max(0, Math.Min(endX, constraint.Width) - startX), constraint.Height);
                }

                // Set quick launch toolbar position and size
                this.quickAccessToolbarHolder.Measure(infinity);

                var quickAccessToolbarWidth = this.quickAccessToolbarHolder.DesiredSize.Width;
                this.quickAccessToolbarRect = new Rect(0, 0, Math.Min(quickAccessToolbarWidth, startX), this.quickAccessToolbarHolder.DesiredSize.Height);

                if (quickAccessToolbarWidth > startX)
                {
                    this.quickAccessToolbarHolder.Measure(this.quickAccessToolbarRect.Size);
                    this.quickAccessToolbarRect = new Rect(0, 0, this.quickAccessToolbarHolder.DesiredSize.Width, this.quickAccessToolbarHolder.DesiredSize.Height);
                    quickAccessToolbarWidth = this.quickAccessToolbarHolder.DesiredSize.Width;
                }

                // Set header
                this.headerHolder.Measure(infinity);

                switch (this.HeaderAlignment)
                {
                    case HorizontalAlignment.Left:
                        {
                            if (startX - quickAccessToolbarWidth > 150)
                            {
                                var allTextWidth = startX - quickAccessToolbarWidth;
                                this.headerRect = new Rect(this.quickAccessToolbarRect.Width, 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                            else
                            {
                                var allTextWidth = Math.Max(0, constraint.Width - endX);
                                this.headerRect = new Rect(Math.Min(endX, constraint.Width), 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                        }
                        break;

                    case HorizontalAlignment.Center:
                        {
                            var allTextWidthRight = Math.Max(0, constraint.Width - endX);
                            var allTextWidthLeft = Math.Max(0, startX - quickAccessToolbarWidth);
                            var fitsRightButNotLeft = allTextWidthRight >= this.headerHolder.DesiredSize.Width && allTextWidthLeft < this.headerHolder.DesiredSize.Width;

                            if (((startX - quickAccessToolbarWidth < 150 || fitsRightButNotLeft) && (startX - quickAccessToolbarWidth > 0) && (startX - quickAccessToolbarWidth < constraint.Width - endX)) || (endX < constraint.Width / 2))
                            {
                                this.headerRect = new Rect(Math.Min(Math.Max(endX, constraint.Width / 2 - this.headerHolder.DesiredSize.Width / 2), constraint.Width), 0, Math.Min(allTextWidthRight, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                            else
                            {
                                this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidthLeft / 2 - this.headerHolder.DesiredSize.Width / 2), 0, Math.Min(allTextWidthLeft, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                        }
                        break;

                    case HorizontalAlignment.Right:
                        {
                            if (startX - quickAccessToolbarWidth > 150)
                            {
                                var allTextWidth = Math.Max(0, startX - quickAccessToolbarWidth);
                                this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, allTextWidth - this.headerHolder.DesiredSize.Width), 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                            else
                            {
                                var allTextWidth = Math.Max(0, constraint.Width - endX);
                                this.headerRect = new Rect(Math.Min(Math.Max(endX, constraint.Width - this.headerHolder.DesiredSize.Width), constraint.Width), 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                        }
                        break;

                    case HorizontalAlignment.Stretch:
                        {
                            if (startX - quickAccessToolbarWidth > 150)
                            {
                                var allTextWidth = startX - quickAccessToolbarWidth;
                                this.headerRect = new Rect(this.quickAccessToolbarRect.Width, 0, allTextWidth, constraint.Height);
                            }
                            else
                            {
                                var allTextWidth = Math.Max(0, constraint.Width - endX);
                                this.headerRect = new Rect(Math.Min(endX, constraint.Width), 0, allTextWidth, constraint.Height);
                            }
                        }
                        break;
                }
            }

            this.headerRect.Width = this.headerRect.Width + 2;
        }

        #endregion
    }
}