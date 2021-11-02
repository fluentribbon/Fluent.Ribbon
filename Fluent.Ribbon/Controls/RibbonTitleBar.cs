// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;
    using WindowChrome = ControlzEx.Windows.Shell.WindowChrome;

    /// <summary>
    /// Represents title bar
    /// </summary>
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(RibbonContextualTabGroup))]
    [TemplatePart(Name = "PART_QuickAccessToolbarHolder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_HeaderHolder", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ItemsContainer", Type = typeof(Panel))]
    public class RibbonTitleBar : HeaderedItemsControl
    {
        #region Fields

        // Quick access toolbar holder
        private FrameworkElement? quickAccessToolbarHolder;
        // Header holder
        private FrameworkElement? headerHolder;
        // Items container
        private Panel? itemsContainer;
        // Quick access toolbar rect
        private Rect quickAccessToolbarRect = new Rect(0, 0, 0, 0);
        // Header rect
        private Rect headerRect = new Rect(0, 0, 0, 0);
        // Items rect
        private Rect itemsRect = new Rect(0, 0, 0, 0);

        private Size lastMeasureConstraint;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets quick access toolbar
        /// </summary>
        public FrameworkElement? QuickAccessToolBar
        {
            get { return (FrameworkElement?)this.GetValue(QuickAccessToolBarProperty); }
            set { this.SetValue(QuickAccessToolBarProperty, value); }
        }

        /// <summary>Identifies the <see cref="QuickAccessToolBar"/> dependency property.</summary>
        public static readonly DependencyProperty QuickAccessToolBarProperty =
            DependencyProperty.Register(nameof(QuickAccessToolBar), typeof(FrameworkElement), typeof(RibbonTitleBar), new PropertyMetadata(OnQuickAccessToolBarChanged));

        private static void OnQuickAccessToolBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var titleBar = (RibbonTitleBar)d;
            titleBar.ScheduleForceMeasureAndArrange();
        }

        /// <summary>
        /// Gets or sets header alignment
        /// </summary>
        public HorizontalAlignment HeaderAlignment
        {
            get { return (HorizontalAlignment)this.GetValue(HeaderAlignmentProperty); }
            set { this.SetValue(HeaderAlignmentProperty, value); }
        }

        /// <summary>Identifies the <see cref="HeaderAlignment"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderAlignmentProperty =
            DependencyProperty.Register(nameof(HeaderAlignment), typeof(HorizontalAlignment), typeof(RibbonTitleBar), new FrameworkPropertyMetadata(HorizontalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Defines whether title bar is collapsed
        /// </summary>
        public bool IsCollapsed
        {
            get { return (bool)this.GetValue(IsCollapsedProperty); }
            set { this.SetValue(IsCollapsedProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="IsCollapsed"/> dependency property.</summary>
        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(RibbonTitleBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        private bool isAtLeastOneRequiredControlPresent;

        /// <summary>Identifies the <see cref="HideContextTabs"/> dependency property.</summary>
        public static readonly DependencyProperty HideContextTabsProperty =
            DependencyProperty.Register(nameof(HideContextTabs), typeof(bool), typeof(RibbonTitleBar), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///  Gets or sets whether context tabs are hidden.
        /// </summary>
        public bool HideContextTabs
        {
            get { return (bool)this.GetValue(HideContextTabsProperty); }
            set { this.SetValue(HideContextTabsProperty, BooleanBoxes.Box(value)); }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonTitleBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTitleBar), new FrameworkPropertyMetadata(typeof(RibbonTitleBar)));

            HeaderProperty.OverrideMetadata(typeof(RibbonTitleBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTitleBar()
        {
            WindowChrome.SetIsHitTestVisibleInChrome(this, true);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var baseResult = base.HitTestCore(hitTestParameters);

            if (baseResult is null)
            {
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }

            return baseResult;
        }

        /// <inheritdoc />
        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            if (e.Handled
                || this.IsMouseDirectlyOver == false)
            {
                return;
            }

            WindowSteeringHelper.ShowSystemMenu(this, e);
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.Handled)
            {
                return;
            }

            // Contextual groups shall handle mouse events
            if (e.Source is RibbonContextualGroupsContainer
                || e.Source is RibbonContextualTabGroup)
            {
                return;
            }

            WindowSteeringHelper.HandleMouseLeftButtonDown(e, true, true);
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonContextualTabGroup();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RibbonContextualTabGroup;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.quickAccessToolbarHolder = this.GetTemplateChild("PART_QuickAccessToolbarHolder") as FrameworkElement;
            this.headerHolder = this.GetTemplateChild("PART_HeaderHolder") as FrameworkElement;
            this.itemsContainer = this.GetTemplateChild("PART_ItemsContainer") as Panel;

            this.isAtLeastOneRequiredControlPresent = this.quickAccessToolbarHolder is not null
                                     || this.headerHolder is not null
                                     || this.itemsContainer is not null;

            if (this.quickAccessToolbarHolder is not null)
            {
                WindowChrome.SetIsHitTestVisibleInChrome(this.quickAccessToolbarHolder, true);
            }
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (this.isAtLeastOneRequiredControlPresent == false)
            {
                return base.MeasureOverride(constraint);
            }

            this.lastMeasureConstraint = constraint;

            var resultSize = constraint;

            if (double.IsPositiveInfinity(resultSize.Width)
                || double.IsPositiveInfinity(resultSize.Height))
            {
                resultSize = base.MeasureOverride(resultSize);
            }

            this.Update(resultSize);

            this.itemsContainer?.Measure(this.itemsRect.Size);
            this.headerHolder?.Measure(this.headerRect.Size);
            this.quickAccessToolbarHolder?.Measure(this.quickAccessToolbarRect.Size);

            var maxHeight = Math.Max(Math.Max(this.itemsRect.Height, this.headerRect.Height), this.quickAccessToolbarRect.Height);
            var width = this.quickAccessToolbarRect.Width + this.headerRect.Width + this.itemsRect.Width;

            return new Size(width, maxHeight);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (this.isAtLeastOneRequiredControlPresent == false)
            {
                return base.ArrangeOverride(arrangeBounds);
            }

            // If the last measure constraint and the arrangeBounds are not equal we have to update again.
            // This can happen if the window is set to auto-size it's width.
            // As Update also does some things that are related to an arrange pass we have to update again.
            // It would be way better if Update wouldn't handle parts of the arrange pass, but that would be very difficult to implement...
            if (arrangeBounds.Equals(this.lastMeasureConstraint) == false)
            {
                this.Update(arrangeBounds);

                this.itemsContainer?.Measure(this.itemsRect.Size);
                this.headerHolder?.Measure(this.headerRect.Size);
                this.quickAccessToolbarHolder?.Measure(this.quickAccessToolbarRect.Size);
            }

            this.itemsContainer?.Arrange(this.itemsRect);
            this.headerHolder?.Arrange(this.headerRect);
            this.quickAccessToolbarHolder?.Arrange(this.quickAccessToolbarRect);

            this.EnsureCorrectLayoutAfterArrange();

            return arrangeBounds;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sometimes the relative position only changes after the arrange phase.
        /// To compensate such sitiations we issue a second layout pass by invalidating our measure.
        /// This situation can occur if, for example, the icon of a ribbon window has it's visibility changed.
        /// </summary>
        private void EnsureCorrectLayoutAfterArrange()
        {
            var currentRelativePosition = this.GetCurrentRelativePosition();
            this.RunInDispatcherAsync(() => this.CheckPosition(currentRelativePosition, this.GetCurrentRelativePosition()));
        }

        private void CheckPosition(Point previousRelativePosition, Point currentRelativePositon)
        {
            if (previousRelativePosition != currentRelativePositon)
            {
                this.InvalidateMeasure();
            }
        }

        private Point GetCurrentRelativePosition()
        {
            var parentUIElement = this.Parent as UIElement;

            if (parentUIElement is null)
            {
                return default;
            }

            return this.TranslatePoint(default, parentUIElement);
        }

        // Update items size and positions
        private void Update(Size constraint)
        {
            var visibleGroups = this.Items.OfType<RibbonContextualTabGroup>()
                            .Where(group => group.InnerVisibility == Visibility.Visible && group.Items.Count > 0)
                            .ToList();

            var canRibbonTabControlScroll = false;

            // Defensively try to find out if the RibbonTabControl can scroll
            if (visibleGroups.Count > 0)
            {
                var firstVisibleItem = visibleGroups.First().FirstVisibleItem;

                canRibbonTabControlScroll = UIHelper.GetParent<RibbonTabControl>(firstVisibleItem)?.CanScroll == true;
            }

            if (this.IsCollapsed)
            {
                // Collapse QuickAccessToolbar
                this.quickAccessToolbarRect = new Rect(0, 0, 0, 0);

                // Collapse itemRect
                this.itemsRect = new Rect(0, 0, 0, 0);

                this.headerHolder?.Measure(new Size(constraint.Width, constraint.Height));
                this.headerRect = new Rect(0, 0, this.headerHolder?.DesiredSize.Width ?? default, constraint.Height);
            }
            else if (visibleGroups.Count == 0
                || canRibbonTabControlScroll)
            {
                // Collapse itemRect
                this.itemsRect = new Rect(0, 0, 0, 0);

                // Set quick launch toolbar and header position and size
                this.quickAccessToolbarHolder?.Measure(SizeConstants.Infinite);

                if (this.quickAccessToolbarHolder is not null
                    && constraint.Width <= this.quickAccessToolbarHolder.DesiredSize.Width + 50)
                {
                    this.quickAccessToolbarRect = new Rect(0, 0, Math.Max(0, constraint.Width - 50), this.quickAccessToolbarHolder.DesiredSize.Height);
                    this.quickAccessToolbarHolder.Measure(this.quickAccessToolbarRect.Size);
                }

                if (this.quickAccessToolbarHolder is not null
                    && this.headerHolder is not null
                    && constraint.Width > this.quickAccessToolbarHolder.DesiredSize.Width + 50)
                {
                    this.quickAccessToolbarRect = new Rect(0, 0, this.quickAccessToolbarHolder.DesiredSize.Width, this.quickAccessToolbarHolder.DesiredSize.Height);
                    this.headerHolder.Measure(SizeConstants.Infinite);
                    var allTextWidth = constraint.Width - this.quickAccessToolbarHolder.DesiredSize.Width;

                    if (this.HeaderAlignment == HorizontalAlignment.Left)
                    {
                        this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width, 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
                    }
                    else if (this.HeaderAlignment == HorizontalAlignment.Center)
                    {
                        this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, (allTextWidth / 2) - (this.headerHolder.DesiredSize.Width / 2)), 0, Math.Min(allTextWidth, this.headerHolder.DesiredSize.Width), constraint.Height);
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
                var pointZero = default(Point);

                // get initial StartX value
                var startX = visibleGroups.First().FirstVisibleItem?.TranslatePoint(pointZero, this).X ?? 0;
                var endX = 0D;

                //Get minimum x point (workaround)
                foreach (var group in visibleGroups)
                {
                    var currentStartX = group.FirstVisibleItem?.TranslatePoint(pointZero, this).X ?? 0;

                    if (currentStartX < startX)
                    {
                        startX = currentStartX;
                    }

                    var lastItem = group.LastVisibleItem;
                    var currentEndX = lastItem?.TranslatePoint(new Point(lastItem.DesiredSize.Width, 0), this).X ?? 0;

                    if (currentEndX > endX)
                    {
                        endX = currentEndX;
                    }
                }

                // Ensure that startX and endX are never negative
                startX = Math.Max(0, startX);
                endX = Math.Max(0, endX);

                // Ensure that startX respect min width of QuickAccessToolBar
                startX = Math.Max(startX, this.QuickAccessToolBar?.MinWidth ?? 0);

                // Set contextual groups position and size
                this.itemsContainer?.Measure(SizeConstants.Infinite);
                var itemsRectWidth = Math.Min(this.itemsContainer?.DesiredSize.Width ?? default, Math.Max(0, Math.Min(endX, constraint.Width) - startX));
                this.itemsRect = new Rect(startX, 0, itemsRectWidth, constraint.Height);

                // Set quick launch toolbar position and size
                this.quickAccessToolbarHolder?.Measure(SizeConstants.Infinite);

                var quickAccessToolbarWidth = this.quickAccessToolbarHolder?.DesiredSize.Width ?? default;
                this.quickAccessToolbarRect = new Rect(0, 0, Math.Min(quickAccessToolbarWidth, startX), this.quickAccessToolbarHolder?.DesiredSize.Height ?? default);

                if (quickAccessToolbarWidth > startX
                    && this.quickAccessToolbarHolder is not null)
                {
                    this.quickAccessToolbarHolder.Measure(this.quickAccessToolbarRect.Size);
                    this.quickAccessToolbarRect = new Rect(0, 0, this.quickAccessToolbarHolder.DesiredSize.Width, this.quickAccessToolbarHolder.DesiredSize.Height);
                    quickAccessToolbarWidth = this.quickAccessToolbarHolder.DesiredSize.Width;
                }

                // Set header
                this.headerHolder?.Measure(SizeConstants.Infinite);

                switch (this.HeaderAlignment)
                {
                    case HorizontalAlignment.Left when this.headerHolder is not null:
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

                    case HorizontalAlignment.Center when this.headerHolder is not null && this.quickAccessToolbarHolder is not null:
                        {
                            var allTextWidthRight = Math.Max(0, constraint.Width - endX);
                            var allTextWidthLeft = Math.Max(0, startX - quickAccessToolbarWidth);
                            var fitsRightButNotLeft = allTextWidthRight >= this.headerHolder.DesiredSize.Width && allTextWidthLeft < this.headerHolder.DesiredSize.Width;

                            if (((startX - quickAccessToolbarWidth < 150 || fitsRightButNotLeft) && (startX - quickAccessToolbarWidth > 0) && (startX - quickAccessToolbarWidth < constraint.Width - endX)) || (endX < constraint.Width / 2))
                            {
                                this.headerRect = new Rect(Math.Min(Math.Max(endX, (constraint.Width / 2) - (this.headerHolder.DesiredSize.Width / 2)), constraint.Width), 0, Math.Min(allTextWidthRight, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                            else
                            {
                                this.headerRect = new Rect(this.quickAccessToolbarHolder.DesiredSize.Width + Math.Max(0, (allTextWidthLeft / 2) - (this.headerHolder.DesiredSize.Width / 2)), 0, Math.Min(allTextWidthLeft, this.headerHolder.DesiredSize.Width), constraint.Height);
                            }
                        }

                        break;

                    case HorizontalAlignment.Right when this.headerHolder is not null && this.quickAccessToolbarHolder is not null:
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

            this.headerRect.Width += 2;
        }

        #endregion

        private DispatcherOperation? forceMeasureAndArrangeOperation;

        /// <summary>
        /// Schedules a call to <see cref="FrameworkElementExtensions.ForceMeasureAndArrange"/>.
        /// </summary>
        public void ScheduleForceMeasureAndArrange()
        {
            if (this.forceMeasureAndArrangeOperation is not null)
            {
                return;
            }

            this.forceMeasureAndArrangeOperation = this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(this.PrivateForceMeasureAndArrange));
        }

        private void PrivateForceMeasureAndArrange()
        {
            this.forceMeasureAndArrangeOperation = null;
            this.ForceMeasureAndArrange();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonTitleBarAutomationPeer(this);
    }
}