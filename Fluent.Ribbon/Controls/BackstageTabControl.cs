// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents Backstage tab control.
    /// </summary>
    [TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_ItemsPanelContainer", Type = typeof(UIElement))]
    public class BackstageTabControl : Selector, ILogicalChildSupport
    {
        #region Properties

        internal ContentPresenter? SelectedContentHost { get; private set; }

        internal UIElement? ItemsPanelContainer { get; private set; }

        /// <summary>
        /// Gets or sets the margin which is used to render selected content.
        /// </summary>
        public Thickness SelectedContentMargin
        {
            get { return (Thickness)this.GetValue(SelectedContentMarginProperty); }
            set { this.SetValue(SelectedContentMarginProperty, value); }
        }

        /// <summary>Identifies the <see cref="SelectedContentMargin"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedContentMarginProperty =
            DependencyProperty.Register(nameof(SelectedContentMargin), typeof(Thickness), typeof(BackstageTabControl), new PropertyMetadata(default(Thickness)));

        // Dependency property key for SelectedContent
        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContent), typeof(object), typeof(BackstageTabControl), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        /// <summary>Identifies the <see cref="SelectedContent"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets content for selected tab
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? SelectedContent
        {
            get { return this.GetValue(SelectedContentProperty); }
            internal set { this.SetValue(SelectedContentPropertyKey, value); }
        }

        /// <summary>Identifies the <see cref="ContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.Register(nameof(ContentStringFormat), typeof(string), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>Identifies the <see cref="ContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>Identifies the <see cref="ContentTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(BackstageTabControl), new PropertyMetadata());

        private static readonly DependencyPropertyKey SelectedContentStringFormatPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContentStringFormat), typeof(string), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>Identifies the <see cref="SelectedContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedContentStringFormatProperty = SelectedContentStringFormatPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContentTemplate), typeof(DataTemplate), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>Identifies the <see cref="SelectedContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedContentTemplateProperty = SelectedContentTemplatePropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContentTemplateSelector), typeof(DataTemplateSelector), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>Identifies the <see cref="SelectedContentTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = SelectedContentTemplateSelectorPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or sets the string format for the content.
        /// </summary>
        public string? ContentStringFormat
        {
            get
            {
                return (string?)this.GetValue(ContentStringFormatProperty);
            }

            set
            {
                this.SetValue(ContentStringFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> which should be used for the content
        /// </summary>
        public DataTemplate? ContentTemplate
        {
            get
            {
                return (DataTemplate?)this.GetValue(ContentTemplateProperty);
            }

            set
            {
                this.SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ContentTemplateSelector"/> which should be used for the content
        /// </summary>
        public DataTemplateSelector? ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector?)this.GetValue(ContentTemplateSelectorProperty);
            }

            set
            {
                this.SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Get or sets the string format for the selected content.
        /// </summary>
        public string? SelectedContentStringFormat
        {
            get
            {
                return (string?)this.GetValue(SelectedContentStringFormatProperty);
            }

            internal set
            {
                this.SetValue(SelectedContentStringFormatPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> which should be used for the selected content
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate? SelectedContentTemplate
        {
            get
            {
                return (DataTemplate?)this.GetValue(SelectedContentTemplateProperty);
            }

            internal set
            {
                this.SetValue(SelectedContentTemplatePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ContentTemplateSelector"/> which should be used for the selected content
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplateSelector? SelectedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector?)this.GetValue(SelectedContentTemplateSelectorProperty);
            }

            internal set
            {
                this.SetValue(SelectedContentTemplateSelectorPropertyKey, value);
            }
        }

        #region ItemsPanelMinWidth

        /// <summary>Identifies the <see cref="ItemsPanelMinWidth"/> dependency property.</summary>
        public static readonly DependencyProperty ItemsPanelMinWidthProperty = DependencyProperty.Register(nameof(ItemsPanelMinWidth), typeof(double), typeof(BackstageTabControl), new PropertyMetadata(125d));

        /// <summary>
        /// Gets or sets the MinWidth for the ItemsPanel.
        /// </summary>
        public double ItemsPanelMinWidth
        {
            get { return (double)this.GetValue(ItemsPanelMinWidthProperty); }
            set { this.SetValue(ItemsPanelMinWidthProperty, value); }
        }

        #endregion

        #region ItemsPanelBackground

        /// <summary>
        /// Gets or sets current Backround of the ItemsPanel
        /// </summary>
        public Brush? ItemsPanelBackground
        {
            get { return (Brush?)this.GetValue(ItemsPanelBackgroundProperty); }
            set { this.SetValue(ItemsPanelBackgroundProperty, value); }
        }

        /// <summary>Identifies the <see cref="ItemsPanelBackground"/> dependency property.</summary>
        public static readonly DependencyProperty ItemsPanelBackgroundProperty = DependencyProperty.Register(nameof(ItemsPanelBackground), typeof(Brush), typeof(BackstageTabControl));

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="ParentBackstage"/>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Backstage? ParentBackstage
        {
            get { return (Backstage?)this.GetValue(ParentBackstageProperty); }
            set { this.SetValue(ParentBackstageProperty, value); }
        }

        /// <summary>Identifies the <see cref="ParentBackstage"/> dependency property.</summary>
        public static readonly DependencyProperty ParentBackstageProperty =
            DependencyProperty.Register(nameof(ParentBackstage), typeof(Backstage), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Defines if the <see cref="WindowSteeringHelperControl"/> is enabled in this control
        /// </summary>
        public bool IsWindowSteeringHelperEnabled
        {
            get { return (bool)this.GetValue(IsWindowSteeringHelperEnabledProperty); }
            set { this.SetValue(IsWindowSteeringHelperEnabledProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="IsWindowSteeringHelperEnabled"/> dependency property.</summary>
        public static readonly DependencyProperty IsWindowSteeringHelperEnabledProperty =
            DependencyProperty.Register(nameof(IsWindowSteeringHelperEnabled), typeof(bool), typeof(BackstageTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Defines if the back button is visible or not.
        /// </summary>
        public bool IsBackButtonVisible
        {
            get { return (bool)this.GetValue(IsBackButtonVisibleProperty); }
            set { this.SetValue(IsBackButtonVisibleProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="IsBackButtonVisible"/> dependency property.</summary>
        public static readonly DependencyProperty IsBackButtonVisibleProperty = DependencyProperty.Register(nameof(IsBackButtonVisible), typeof(bool), typeof(BackstageTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static BackstageTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(typeof(BackstageTabControl)));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageTabControl()
        {
            // Fixed incorect menu showing
            this.ContextMenu = new ContextMenu
            {
                Width = 0,
                Height = 0,
                HasDropShadow = false
            };

            this.ContextMenu.Opened += (sender, args) => this.ContextMenu.IsOpen = false;

            this.Loaded += this.HandleLoaded;
            this.Unloaded += this.HandleUnloaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            this.ParentBackstage = UIHelper.GetParent<Backstage>(this);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            this.ParentBackstage = null;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.ItemContainerGenerator.StatusChanged += this.OnGeneratorStatusChanged;
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ItemsPanelContainer = this.GetTemplateChild("PART_ItemsPanelContainer") as UIElement;
            this.SelectedContentHost = this.GetTemplateChild("PART_SelectedContentHost") as ContentPresenter;
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BackstageTabItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BackstageTabItem
                or Button
                or SeparatorTabItem
                or Separator;
        }

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Remove
                && this.SelectedIndex == -1)
            {
                var startIndex = e.OldStartingIndex + 1;
                if (startIndex > this.Items.Count)
                {
                    startIndex = 0;
                }

                var item = this.FindNextTabItem(startIndex, -1);
                if (item is not null)
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <inheritdoc />
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (e.AddedItems.Count > 0)
            {
                this.UpdateSelectedContent();
            }
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
            {
                base.OnKeyDown(e);
                return;
            }

            // Handle [Ctrl][Shift]Tab

            switch (e.Key)
            {
                case Key.F6:
                case Key.Tab when (e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control:
                    {
                        var focusNavigationDirection = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                            ? FocusNavigationDirection.Last
                            : FocusNavigationDirection.First;

                        if (this.SelectedContentHost?.IsKeyboardFocusWithin == true)
                        {
                            e.Handled = this.ItemsPanelContainer?.MoveFocus(new TraversalRequest(focusNavigationDirection)) == true;
                        }
                        else
                        {
                            e.Handled = this.SelectedContentHost?.MoveFocus(new TraversalRequest(focusNavigationDirection)) == true;
                        }
                    }

                    break;
            }

            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets selected <see cref="BackstageTabItem"/>.
        /// If there is no item selected, the first found item is selected and it's container (<see cref="BackstageTabItem"/>) is returned.
        /// </summary>
        /// <returns>The currently selected <see cref="BackstageTabItem"/>. Or null of nothing was selected and nothing could be selected.</returns>
        private BackstageTabItem? GetSelectedTabItem()
        {
            var container = this.ItemContainerGenerator.ContainerOrContainerContentFromItem<BackstageTabItem>(this.SelectedItem);

            if (container is null
                || container.IsEnabled == false
                || container.Visibility != Visibility.Visible)
            {
                container = this.FindNextTabItem(this.SelectedIndex, 1);

                if (container is not null)
                {
                    this.SelectedItem = this.ItemContainerGenerator.ItemFromContainerOrContainerContent(container);
                }
            }

            return container;
        }

        // Finds next tab item
        private BackstageTabItem? FindNextTabItem(int startIndex, int direction)
        {
            if (direction == 0)
            {
                return null;
            }

            var index = startIndex;
            for (var i = 0; i < this.Items.Count; i++)
            {
                index += direction;

                if (index >= this.Items.Count)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = this.Items.Count - 1;
                }

                var container = this.ItemContainerGenerator.ContainerOrContainerContentFromIndex<BackstageTabItem>(index);
                if (container is not null
                    && container.IsEnabled
                    && container.Visibility == Visibility.Visible)
                {
                    return container;
                }
            }

            return null;
        }

        // Updates selected content
        private void UpdateSelectedContent()
        {
            if (this.SelectedIndex < 0)
            {
                this.SelectedContent = null;
            }
            else
            {
                var selectedTabItem = this.GetSelectedTabItem();
                if (selectedTabItem is null)
                {
                    return;
                }

                this.SelectedContent = selectedTabItem.Content;

                if (selectedTabItem.ContentTemplate is not null
                    || selectedTabItem.ContentTemplateSelector is not null
                    || selectedTabItem.ContentStringFormat is not null)
                {
                    this.SelectedContentTemplate = selectedTabItem.ContentTemplate;
                    this.SelectedContentTemplateSelector = selectedTabItem.ContentTemplateSelector;
                    this.SelectedContentStringFormat = selectedTabItem.ContentStringFormat;
                }
                else
                {
                    this.SelectedContentTemplate = this.ContentTemplate;
                    this.SelectedContentTemplateSelector = this.ContentTemplateSelector;
                    this.SelectedContentStringFormat = this.ContentStringFormat;
                }

                this.UpdateLayout();
            }
        }

        #endregion

        #region Event handling

        // Handles GeneratorStatusChange
        private void OnGeneratorStatusChanged(object? sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            if (this.HasItems
                && this.SelectedIndex == -1)
            {
                this.SelectedIndex = 0;
            }

            this.UpdateSelectedContent();
        }

        #endregion

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonBackstageTabControlAutomationPeer(this);

        /// <inheritdoc />
        void ILogicalChildSupport.AddLogicalChild(object child)
        {
            this.AddLogicalChild(child);
        }

        /// <inheritdoc />
        void ILogicalChildSupport.RemoveLogicalChild(object child)
        {
            this.RemoveLogicalChild(child);
        }

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

                if (this.SelectedContent is not null)
                {
                    yield return this.SelectedContent;
                }
            }
        }
    }
}