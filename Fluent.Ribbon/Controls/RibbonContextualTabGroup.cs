// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents contextual tab group
    /// </summary>
    public class RibbonContextualTabGroup : Control
    {
        #region Properties

        /// <summary>
        /// Gets or sets group header
        /// </summary>
        public string Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(string), typeof(RibbonContextualTabGroup),
            new PropertyMetadata("RibbonContextualTabGroup", OnHeaderChanged));

        /// <summary>
        /// Handles header chages
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Gets collection of tab items
        /// </summary>
        public List<RibbonTabItem> Items { get; } = new List<RibbonTabItem>();

        /// <summary>
        /// Gets or sets the visibility this group for internal use (this enables us to hide this group when all items in this group are hidden)
        /// </summary>
        public Visibility InnerVisibility
        {
            get { return (Visibility)this.GetValue(InnerVisibilityProperty); }
            private set { this.SetValue(InnerVisibilityPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey InnerVisibilityPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(InnerVisibility), typeof(Visibility), typeof(RibbonContextualTabGroup), new PropertyMetadata(VisibilityBoxes.Visible, OnInnerVisibilityChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for InnerVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InnerVisibilityProperty = InnerVisibilityPropertyKey.DependencyProperty;

        private static void OnInnerVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var contextGroup = (RibbonContextualTabGroup)d;

            ForceRedraw(contextGroup);
        }

        /// <summary>
        /// Gets the first visible TabItem in this group
        /// </summary>
        public RibbonTabItem FirstVisibleItem => this.GetFirstVisibleItem();

        /// <summary>
        /// Gets the last visible TabItem in this group
        /// </summary>
        public RibbonTabItem LastVisibleItem => this.GetLastVisibleItem();

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonContextualTabGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new FrameworkPropertyMetadata(typeof(RibbonContextualTabGroup)));
            VisibilityProperty.OverrideMetadata(typeof(RibbonContextualTabGroup), new PropertyMetadata(VisibilityBoxes.Collapsed, OnVisibilityChanged));
        }

        /// <summary>
        /// Handles visibility prioperty changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = (RibbonContextualTabGroup)d;

            group.UpdateInnerVisiblityAndGroupBorders();

            ForceRedraw(group);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonContextualTabGroup()
        {
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
            this.UpdateInnerVisibility();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();
        }

        private void UnSubscribeEvents()
        {
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Appends tab item
        /// </summary>
        /// <param name="item">Ribbon tab item</param>
        internal void AppendTabItem(RibbonTabItem item)
        {
            this.Items.Add(item);
            this.UpdateInnerVisiblityAndGroupBorders();
        }

        /// <summary>
        /// Removes tab item
        /// </summary>
        /// <param name="item">Ribbon tab item</param>
        internal void RemoveTabItem(RibbonTabItem item)
        {
            this.Items.Remove(item);
            this.UpdateInnerVisiblityAndGroupBorders();
        }

        private RibbonTabItem GetFirstVisibleItem()
        {
            return this.Items.FirstOrDefault(item => item.Visibility == Visibility.Visible);
        }

        private RibbonTabItem GetLastVisibleItem()
        {
            return this.Items.LastOrDefault(item => item.Visibility == Visibility.Visible);
        }

        /// <summary>
        /// Updates the group border
        /// </summary>
        public void UpdateInnerVisiblityAndGroupBorders()
        {
            this.UpdateInnerVisibility();

            var leftset = false;
            var rightset = false;

            for (var i = 0; i < this.Items.Count; i++)
            {
                //if (i == 0) items[i].HasLeftGroupBorder = true;
                //else items[i].HasLeftGroupBorder = false;
                //if (i == items.Count - 1) items[i].HasRightGroupBorder = true;
                //else items[i].HasRightGroupBorder = false;

                //Workaround so you can have inivisible Tabs on a Group
                if (this.Items[i].Visibility == Visibility.Visible
                    && leftset == false)
                {
                    this.Items[i].HasLeftGroupBorder = true;
                    leftset = true;
                }
                else
                {
                    this.Items[i].HasLeftGroupBorder = false;
                }

                if (this.Items[this.Items.Count - 1 - i].Visibility == Visibility.Visible
                    && rightset == false)
                {
                    this.Items[this.Items.Count - 1 - i].HasRightGroupBorder = true;
                    rightset = true;
                }
                else
                {
                    this.Items[this.Items.Count - 1 - i].HasRightGroupBorder = false;
                }
            }
        }

        #endregion

        #region Override

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            var firstVisibleItem = this.FirstVisibleItem;

            if (e.ClickCount == 1
                && firstVisibleItem != null)
            {
                var currentSelectedItem = firstVisibleItem.TabControlParent?.SelectedItem as RibbonTabItem;

                if (currentSelectedItem != null)
                {
                    currentSelectedItem.IsSelected = false;
                }

                e.Handled = true;

                if (firstVisibleItem.TabControlParent != null)
                {
                    if (firstVisibleItem.TabControlParent.IsMinimized)
                    {
                        firstVisibleItem.TabControlParent.IsMinimized = false;
                    }

                    firstVisibleItem.IsSelected = true;
                }
            }

            base.OnMouseLeftButtonUp(e);
        }

        #endregion

        /// <summary>
        /// Updates the Visibility of the inner container
        /// </summary>
        private void UpdateInnerVisibility()
        {
            this.InnerVisibility = this.Visibility == Visibility.Visible && this.Items.Any(item => item.Visibility == Visibility.Visible) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        private static void ForceRedraw(RibbonContextualTabGroup contextGroup)
        {
            contextGroup.ForceMeasure();

            var ribbonTitleBar = UIHelper.GetParent<RibbonTitleBar>(contextGroup);
            ribbonTitleBar?.ForceMeasureAndArrange();
        }
    }
}