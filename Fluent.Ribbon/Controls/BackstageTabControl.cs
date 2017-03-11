// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents Backstage tab control.
    /// </summary>
    public class BackstageTabControl : Selector
    {
        #region Properties

        /// <summary>
        /// Gets or sets the margin which is used to render selected content.
        /// </summary>
        public Thickness SelectedContentMargin
        {
            get { return (Thickness)this.GetValue(SelectedContentMarginProperty); }
            set { this.SetValue(SelectedContentMarginProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="SelectedContentMargin"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedContentMarginProperty =
            DependencyProperty.Register(nameof(SelectedContentMargin), typeof(Thickness), typeof(BackstageTabControl), new PropertyMetadata(default(Thickness)));

        // Dependency property key for SelectedContent
        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContent), typeof(object), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property for <see cref="SelectedContent"/>
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets content for selected tab
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedContent
        {
            get
            {
                return this.GetValue(SelectedContentProperty);
            }
            internal set
            {
                this.SetValue(SelectedContentPropertyKey, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ContentStringFormat"/>
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.Register(nameof(ContentStringFormat), typeof(string), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property for <see cref="ContentTemplate"/>
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property for <see cref="ContentTemplateSelector"/>
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register(nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(BackstageTabControl), new PropertyMetadata());

        private static readonly DependencyPropertyKey SelectedContentStringFormatPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContentStringFormat), typeof(string), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property for <see cref="SelectedContentStringFormat"/>
        /// </summary>
        public static readonly DependencyProperty SelectedContentStringFormatProperty = SelectedContentStringFormatPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContentTemplate), typeof(DataTemplate), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property for <see cref="SelectedContentTemplate"/>
        /// </summary>
        public static readonly DependencyProperty SelectedContentTemplateProperty = SelectedContentTemplatePropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SelectedContentTemplateSelector), typeof(DataTemplateSelector), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Dependency property for <see cref="SelectedContentTemplateSelector"/>
        /// </summary>
        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = SelectedContentTemplateSelectorPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or sets the string format for the content.
        /// </summary>
        public string ContentStringFormat
        {
            get
            {
                return (string)this.GetValue(ContentStringFormatProperty);
            }
            set
            {
                this.SetValue(ContentStringFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> which should be used for the content
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ContentTemplateProperty);
            }
            set
            {
                this.SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ContentTemplateSelector"/> which should be used for the content
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)this.GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                this.SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Get or sets the string format for the selected content.
        /// </summary>
        public string SelectedContentStringFormat
        {
            get
            {
                return (string)this.GetValue(SelectedContentStringFormatProperty);
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
        public DataTemplate SelectedContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(SelectedContentTemplateProperty);
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
        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)this.GetValue(SelectedContentTemplateSelectorProperty);
            }
            internal set
            {
                this.SetValue(SelectedContentTemplateSelectorPropertyKey, value);
            }
        }

        #region ItemsPanelBackground

        /// <summary>
        /// Gets or sets current Backround of the ItemsPanel
        /// </summary>
        public Brush ItemsPanelBackground
        {
            get { return (Brush)this.GetValue(ItemsPanelBackgroundProperty); }
            set { this.SetValue(ItemsPanelBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ItemsPanelBackground"/>
        /// </summary>
        public static DependencyProperty ItemsPanelBackgroundProperty =
            DependencyProperty.Register(nameof(ItemsPanelBackground), typeof(Brush), typeof(BackstageTabControl));

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="ParentBackstage"/>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Backstage ParentBackstage
        {
            get { return (Backstage)this.GetValue(ParentBackstageProperty); }
            set { this.SetValue(ParentBackstageProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ParentBackstage"/>
        /// </summary>
        public static readonly DependencyProperty ParentBackstageProperty =
            DependencyProperty.Register(nameof(ParentBackstage), typeof(Backstage), typeof(BackstageTabControl), new PropertyMetadata());

        /// <summary>
        /// Defines if the <see cref="WindowSteeringHelperControl"/> is enabled in this control
        /// </summary>
        public bool IsWindowSteeringHelperEnabled
        {
            get { return (bool)this.GetValue(IsWindowSteeringHelperEnabledProperty); }
            set { this.SetValue(IsWindowSteeringHelperEnabledProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IsWindowSteeringHelperEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsWindowSteeringHelperEnabledProperty =
            DependencyProperty.Register(nameof(IsWindowSteeringHelperEnabled), typeof(bool), typeof(BackstageTabControl), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static BackstageTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(typeof(BackstageTabControl)));
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
            this.ContextMenu.Opened += delegate { this.ContextMenu.IsOpen = false; };

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

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. 
        /// This method is invoked whenever System.Windows.FrameworkElement.
        /// IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.ItemContainerGenerator.StatusChanged += this.OnGeneratorStatusChanged;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BackstageTabItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BackstageTabItem
                || item is Button
                || item is SeparatorTabItem
                || item is Separator;
        }

        /// <summary>
        /// Updates the current selection when an item in the System.Windows.Controls.Primitives.Selector has changed
        /// </summary>
        /// <param name="e">The event data.</param>
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
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (e.AddedItems.Count > 0)
            {
                this.UpdateSelectedContent();
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets selected <see cref="BackstageTabItem"/>.
        /// If there is no item selected, the first found item is selected and it's container (<see cref="BackstageTabItem"/>) is returned.
        /// </summary>
        /// <returns>The currently selected <see cref="BackstageTabItem"/>. Or null of nothing was selected and nothing could be selected.</returns>
        private BackstageTabItem GetSelectedTabItem()
        {
            var container = this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as BackstageTabItem;
            if (container == null)
            {
                container = this.FindNextTabItem(this.SelectedIndex, 1);

                if (container != null)
                {
                    this.SelectedItem = this.ItemContainerGenerator.ItemFromContainer(container);
                }
            }

            return container;
        }

        // Finds next tab item
        private BackstageTabItem FindNextTabItem(int startIndex, int direction)
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

                var container = this.ItemContainerGenerator.ContainerFromIndex(index) as BackstageTabItem;

                if (container != null
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
                if (selectedTabItem == null)
                {
                    return;
                }

                this.SelectedContent = selectedTabItem.Content;
                if (selectedTabItem.ContentTemplate != null
                    || selectedTabItem.ContentTemplateSelector != null
                    || selectedTabItem.ContentStringFormat != null)
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
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
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
    }
}