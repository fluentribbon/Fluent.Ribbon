// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents backstage tab item
    /// </summary>
    [TemplatePart(Name = "PART_Header", Type = typeof(FrameworkElement))]
    public class BackstageTabItem : ContentControl, IHeaderedControl, IKeyTipedControl, ILogicalChildSupport
    {
        internal FrameworkElement HeaderContentHost { get; private set; }

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="Icon"/>
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(BackstageTabItem), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        /// <inheritdoc />
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="KeyTip"/>
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(BackstageTabItem));

        /// <summary>
        /// Gets or sets a value indicating whether the tab is selected
        /// </summary>
        [Bindable(true)]
        [Category("Appearance")]
        public bool IsSelected
        {
            get { return (bool)this.GetValue(IsSelectedProperty); }
            set { this.SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="IsSelected"/>
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(BackstageTabItem),
            new FrameworkPropertyMetadata(BooleanBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.Journal |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                OnIsSelectedChanged));

        /// <summary>
        /// Gets parent tab control
        /// </summary>
        internal BackstageTabControl TabControlParent
        {
            get
            {
                return ItemsControlHelper.ItemsControlFromItemContainer(this) as BackstageTabControl;
            }
        }

        /// <summary>
        /// Gets or sets tab items text
        /// </summary>
        public object Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(BackstageTabItem), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        /// <summary>
        /// Static constructor
        /// </summary>
        static BackstageTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(typeof(BackstageTabItem)));

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Local));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
        }

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.HeaderContentHost = this.GetTemplateChild("PART_Header") as FrameworkElement;
        }

        /// <inheritdoc />
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (this.IsSelected
                && this.TabControlParent != null)
            {
                this.TabControlParent.SelectedContent = newContent;
            }
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (ReferenceEquals(e.Source, this)
                || this.IsSelected == false)
            {
                this.IsSelected = true;
            }
        }

        /// <inheritdoc />
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((e.Key == Key.Space || e.Key == Key.Enter) 
                && (ReferenceEquals(e.Source, this) || this.IsSelected == false))
            {
                this.IsSelected = true;
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        #endregion

        #region Private methods

        // Handles IsSelected changed
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = (BackstageTabItem)d;
            var newValue = (bool)e.NewValue;

            if (newValue)
            {
                if (container.TabControlParent != null
                    && ReferenceEquals(container.TabControlParent.ItemContainerGenerator.ContainerOrContainerContentFromItem<BackstageTabItem>(container.TabControlParent.SelectedItem), container) == false)
                {
                    UnselectSelectedItem(container.TabControlParent);

                    container.TabControlParent.SelectedItem = container.TabControlParent.ItemContainerGenerator.ItemFromContainerOrContainerContent(container);
                }

                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        private static void UnselectSelectedItem(BackstageTabControl backstageTabControl)
        {
            if (backstageTabControl?.SelectedItem == null)
            {
                return;
            }

            if (backstageTabControl.ItemContainerGenerator.ContainerOrContainerContentFromItem<BackstageTabItem>(backstageTabControl.SelectedItem) is BackstageTabItem backstageTabItem)
            {
                backstageTabItem.IsSelected = false;
            }
        }

        #endregion

        /// <summary>
        /// Handles selected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(e);
        }

        /// <summary>
        /// Handles unselected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(e);
        }

        #region Event handling

        /// <summary>
        /// Handles IsSelected changed
        /// </summary>
        /// <param name="e">The event data.</param>
        private void HandleIsSelectedChanged(RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        #endregion

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            UnselectSelectedItem(this.TabControlParent);

            this.IsSelected = true;

            return KeyTipPressedResult.Empty;
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }

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

                if (this.Icon != null)
                {
                    yield return this.Icon;
                }

                if (this.Header != null)
                {
                    yield return this.Header;
                }
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonBackstageTabItemAutomationPeer(this);
    }
}