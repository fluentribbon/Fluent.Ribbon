// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// RibbonGroup represents a logical group of controls as they appear on
    /// a RibbonTab.  These groups can resize its content
    /// </summary>
    [TemplatePart(Name = "PART_DialogLauncherButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_UpPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_ParentPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_SnappedImage", Type = typeof(Image))]
    public class RibbonGroupBox : HeaderedItemsControl, IQuickAccessItemProvider, IDropDownControl, IKeyTipedControl, IHeaderedControl, ILogicalChildSupport
    {
        #region Fields

        // up part
        private Panel upPanel;

        private Panel parentPanel;

        // Freezed image (created during snapping)
        private Image snappedImage;

        // Is visual currently snapped
        private bool isSnapped;

        private readonly ItemContainerGeneratorAction updateChildSizesItemContainerGeneratorAction;

        #endregion

        #region Properties

        #region KeyTip

        /// <inheritdoc />
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(RibbonGroupBox));

        #endregion

        #region Header-Options

        /// <summary>
        /// <see cref="DependencyProperty"/> for IsCollapsedHeaderContentPresenter.
        /// </summary>
        public static readonly DependencyProperty IsCollapsedHeaderContentPresenterProperty = DependencyProperty.RegisterAttached("IsCollapsedHeaderContentPresenter", typeof(bool), typeof(RibbonGroupBox), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Sets the value of <see cref="IsCollapsedHeaderContentPresenterProperty"/>.
        /// </summary>
        public static void SetIsCollapsedHeaderContentPresenter(DependencyObject element, bool value)
        {
            element.SetValue(IsCollapsedHeaderContentPresenterProperty, value);
        }

        /// <summary>
        /// Gets the value of <see cref="IsCollapsedHeaderContentPresenterProperty"/>.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(RibbonGroupBox))]
        public static bool GetIsCollapsedHeaderContentPresenter(DependencyObject element)
        {
            return (bool)element.GetValue(IsCollapsedHeaderContentPresenterProperty);
        }

        #endregion

        /// <inheritdoc />
        public Popup DropDownPopup { get; private set; }

        /// <inheritdoc />
        public bool IsContextMenuOpened { get; set; }

        #region State

        /// <summary>
        /// Gets or sets the current state of the group
        /// </summary>
        public RibbonGroupBoxState State
        {
            get { return (RibbonGroupBoxState)this.GetValue(StateProperty); }
            set { this.SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for State.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(RibbonGroupBoxState), typeof(RibbonGroupBox), new PropertyMetadata(RibbonGroupBoxState.Large, OnStateChanged));

        /// <summary>
        /// On state property changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbonGroupBox = (RibbonGroupBox)d;
            ribbonGroupBox.updateChildSizesItemContainerGeneratorAction.QueueAction();
        }

        private void UpdateChildSizes()
        {
            var groupBoxState = this.State == RibbonGroupBoxState.QuickAccess
                            ? RibbonGroupBoxState.Collapsed
                            : this.State;

            foreach (var item in this.Items)
            {
                var element = this.ItemContainerGenerator.ContainerFromItem(item);

                if (element == null)
                {
                    continue;
                }

                var targetElement = element;

                if (targetElement is ContentPresenter)
                {
                    targetElement = UIHelper.GetFirstVisualChild(targetElement) ?? targetElement;
                }

                RibbonProperties.SetAppropriateSize(targetElement, groupBoxState);
            }
        }

        #endregion

        #region Scale

        // Current scale index
        private int scale;

        /// <summary>
        /// Gets or sets scale index (for internal IRibbonScalableControl)
        /// </summary>
        internal int Scale
        {
            get { return this.scale; }

            set
            {
                var difference = value - this.scale;
                this.scale = value;

                for (var i = 0; i < Math.Abs(difference); i++)
                {
                    if (difference > 0)
                    {
                        this.IncreaseScalableElement();
                    }
                    else
                    {
                        this.DecreaseScalableElement();
                    }
                }
            }
        }

        // Finds and increase size of all scalable elements in the given group box
        private void IncreaseScalableElement()
        {
            foreach (var item in this.Items)
            {
                var scalableRibbonControl = item as IScalableRibbonControl;

                scalableRibbonControl?.Enlarge();
            }
        }

        private void OnScalableControlScaled(object sender, EventArgs e)
        {
            this.TryClearCache();
        }

        /// <summary>
        /// Gets or sets whether to reset cache when scalable control is scaled
        /// </summary>
        internal bool SuppressCacheReseting { get; set; }

        // Finds and decrease size of all scalable elements in the given group box
        private void DecreaseScalableElement()
        {
            foreach (var item in this.Items)
            {
                var scalableRibbonControl = item as IScalableRibbonControl;

                scalableRibbonControl?.Reduce();
            }
        }

        private void UpdateScalableControlSubscribing()
        {
            this.UpdateScalableControlSubscribing(true);
        }

        private void UpdateScalableControlSubscribing(bool registerEvents)
        {
            foreach (var scalableRibbonControl in this.Items.OfType<IScalableRibbonControl>())
            {
                // Always unregister first to ensure that we don't subscribe twice
                scalableRibbonControl.Scaled -= this.OnScalableControlScaled;

                if (registerEvents)
                {
                    scalableRibbonControl.Scaled += this.OnScalableControlScaled;
                }
            }
        }

        #endregion

        #region IsLauncherVisible

        /// <summary>
        /// Gets or sets dialog launcher button visibility
        /// </summary>
        public bool IsLauncherVisible
        {
            get { return (bool)this.GetValue(IsLauncherVisibleProperty); }
            set { this.SetValue(IsLauncherVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLauncherVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLauncherVisibleProperty =
            DependencyProperty.Register(nameof(IsLauncherVisible), typeof(bool), typeof(RibbonGroupBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        #endregion

        #region LauncherKeys

        /// <summary>
        /// Gets or sets key tip for dialog launcher button
        /// </summary>
        [DisplayName("DialogLauncher Keys")]
        [Category("KeyTips")]
        [Description("Key tip keys for dialog launcher button")]
        public string LauncherKeys
        {
            get { return (string)this.GetValue(LauncherKeysProperty); }
            set { this.SetValue(LauncherKeysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for
        /// LauncherKeys.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherKeysProperty =
            DependencyProperty.Register(nameof(LauncherKeys),
            typeof(string), typeof(RibbonGroupBox), new PropertyMetadata(OnLauncherKeysChanged));

        private static void OnLauncherKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbonGroupBox = (RibbonGroupBox)d;
            if (ribbonGroupBox.LauncherButton != null)
            {
                ribbonGroupBox.LauncherButton.KeyTip = (string)e.NewValue;
            }
        }

        #endregion

        #region LauncherIcon

        /// <summary>
        /// Gets or sets launcher button icon
        /// </summary>
        public object LauncherIcon
        {
            get { return this.GetValue(LauncherIconProperty); }
            set { this.SetValue(LauncherIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherIconProperty =
            DependencyProperty.Register(nameof(LauncherIcon), typeof(object), typeof(RibbonGroupBox), new PropertyMetadata(OnLauncherIconChanged));

        private static void OnLauncherIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddOrRemoveLogicalChild(d, e);
        }

        #endregion

        #region LauncherIcon

        /// <summary>
        /// Gets or sets launcher button text
        /// </summary>
        public string LauncherText
        {
            get { return (string)this.GetValue(LauncherTextProperty); }
            set { this.SetValue(LauncherTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherTextProperty =
            DependencyProperty.Register(nameof(LauncherText), typeof(string), typeof(RibbonGroupBox), new PropertyMetadata());

        #endregion

        #region LauncherCommand

        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed. This is a dependency property.
        /// </summary>
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Bindable(true)]
        public ICommand LauncherCommand
        {
            get
            {
                return (ICommand)this.GetValue(LauncherCommandProperty);
            }

            set
            {
                this.SetValue(LauncherCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the System.Windows.Controls.Primitives.ButtonBase.Command property. This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Category("Action")]
        public object LauncherCommandParameter
        {
            get
            {
                return this.GetValue(LauncherCommandParameterProperty);
            }

            set
            {
                this.SetValue(LauncherCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the element on which to raise the specified command. This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Action")]
        public IInputElement LauncherCommandTarget
        {
            get
            {
                return (IInputElement)this.GetValue(LauncherCommandTargetProperty);
            }

            set
            {
                this.SetValue(LauncherCommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.Primitives.ButtonBase.CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherCommandParameterProperty = DependencyProperty.Register(nameof(LauncherCommandParameter), typeof(object), typeof(RibbonGroupBox), new PropertyMetadata());

        /// <summary>
        /// Identifies the routed System.Windows.Controls.Primitives.ButtonBase.Command dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherCommandProperty = DependencyProperty.Register(nameof(LauncherCommand), typeof(ICommand), typeof(RibbonGroupBox), new PropertyMetadata());

        /// <summary>
        /// Identifies the System.Windows.Controls.Primitives.ButtonBase.CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherCommandTargetProperty = DependencyProperty.Register(nameof(LauncherCommandTarget), typeof(IInputElement), typeof(RibbonGroupBox), new PropertyMetadata());

        #endregion

        #region LauncherToolTip

        /// <summary>
        /// Gets or sets launcher button tooltip
        /// </summary>
        public object LauncherToolTip
        {
            get { return this.GetValue(LauncherToolTipProperty); }
            set { this.SetValue(LauncherToolTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherToolTipProperty =
            DependencyProperty.Register(nameof(LauncherToolTip), typeof(object), typeof(RibbonGroupBox), new PropertyMetadata());

        #endregion

        #region IsLauncherEnabled

        /// <summary>
        /// Gets or sets whether launcher button is enabled
        /// </summary>
        public bool IsLauncherEnabled
        {
            get { return (bool)this.GetValue(IsLauncherEnabledProperty); }
            set { this.SetValue(IsLauncherEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLauncherEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLauncherEnabledProperty =
            DependencyProperty.Register(nameof(IsLauncherEnabled), typeof(bool), typeof(RibbonGroupBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region LauncherButton

        /// <summary>
        /// Gets launcher button
        /// </summary>
        public Button LauncherButton
        {
            get { return (Button)this.GetValue(LauncherButtonProperty); }
            private set { this.SetValue(LauncherButtonPropertyKey, value); }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey LauncherButtonPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(LauncherButton), typeof(Button), typeof(RibbonGroupBox), new PropertyMetadata());

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherButtonProperty = LauncherButtonPropertyKey.DependencyProperty;

        #endregion

        #region IsOpen

        /// <inheritdoc />
        public bool IsDropDownOpen
        {
            get { return (bool)this.GetValue(IsDropDownOpenProperty); }
            set { this.SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(RibbonGroupBox), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsDropDownOpenChanged, CoerceIsDropDownOpen));

        private static object CoerceIsDropDownOpen(DependencyObject d, object basevalue)
        {
            var box = (RibbonGroupBox)d;

            if ((box.State != RibbonGroupBoxState.Collapsed)
                && (box.State != RibbonGroupBoxState.QuickAccess))
            {
                return false;
            }

            return basevalue;
        }

        #endregion

        #region LogicalChildren

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                foreach (var item in this.Items)
                {
                    yield return item;
                }

                if (this.LauncherButton != null)
                {
                    yield return this.LauncherButton;
                }
            }
        }

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets icon
        /// </summary>
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(RibbonGroupBox), new PropertyMetadata(RibbonControl.OnIconChanged));

        #endregion

        #region IsSeparatorVisible

        /// <summary>
        /// Gets or sets whether the groupbox shows a separator.
        /// </summary>
        public bool IsSeparatorVisible
        {
            get { return (bool)this.GetValue(IsSeparatorVisibleProperty); }
            set { this.SetValue(IsSeparatorVisibleProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="IsSeparatorVisible"/>
        /// </summary>
        public static readonly DependencyProperty IsSeparatorVisibleProperty =
          DependencyProperty.Register(nameof(IsSeparatorVisible), typeof(bool), typeof(RibbonGroupBox), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Dialog launcher btton click event
        /// </summary>
        public event RoutedEventHandler LauncherClick;

        /// <summary>
        /// Occurs when context menu is opened
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when context menu is closed
        /// </summary>
        public event EventHandler DropDownClosed;

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes static members of the <see cref="RibbonGroupBox"/> class.
        /// </summary>
        static RibbonGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(typeof(RibbonGroupBox)));
            VisibilityProperty.AddOwner(typeof(RibbonGroupBox), new PropertyMetadata(OnVisibilityChanged));
            FontSizeProperty.AddOwner(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(OnFontSizeChanged));
            FontFamilyProperty.AddOwner(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(OnFontFamilyChanged));

            PopupService.Attach(typeof(RibbonGroupBox));

            ContextMenuService.Attach(typeof(RibbonGroupBox));
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (RibbonGroupBox)d;
            box.ClearCache();
        }

        private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (RibbonGroupBox)d;
            box.ClearCache();
        }

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (RibbonGroupBox)d;
            box.ClearCache();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupBox()
        {
            this.ToolTip = new ToolTip();
            ((ToolTip)this.ToolTip).Template = null;
            this.CoerceValue(ContextMenuProperty);
            this.Focusable = false;

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            this.updateChildSizesItemContainerGeneratorAction = new ItemContainerGeneratorAction(this.ItemContainerGenerator, this.UpdateChildSizes);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();

            this.UpdateScalableControlSubscribing();

            if (this.LauncherButton != null)
            {
                this.LauncherButton.Click += this.OnDialogLauncherButtonClick;
            }

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened += this.OnPopupOpened;
                this.DropDownPopup.Closed += this.OnPopupClosed;
            }
        }

        private void UnSubscribeEvents()
        {
            this.UpdateScalableControlSubscribing(false);

            if (this.LauncherButton != null)
            {
                this.LauncherButton.Click -= this.OnDialogLauncherButtonClick;
            }

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened -= this.OnPopupOpened;
                this.DropDownPopup.Closed -= this.OnPopupClosed;
            }
        }

        #endregion

        #region Methods

        private static void AddOrRemoveLogicalChild(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (RibbonGroupBox)d;

            if (e.OldValue is FrameworkElement oldElement)
            {
                element.RemoveLogicalChild(oldElement);
            }

            if (e.NewValue is FrameworkElement newElement)
            {
                element.AddLogicalChild(newElement);
            }
        }

        /// <summary>
        /// Gets a panel with items
        /// </summary>
        /// <returns></returns>
        internal Panel GetPanel()
        {
            return this.upPanel;
        }

        /// <summary>
        /// Gets cmmon layout root for popup and groupbox
        /// </summary>
        /// <returns></returns>
        internal Panel GetLayoutRoot()
        {
            return this.parentPanel;
        }

        #endregion

        #region Snapping

        /// <summary>
        /// Snaps / Unsnaps the Visual
        /// (remove visuals and substitute with freezed image)
        /// </summary>
        public bool IsSnapped
        {
            get
            {
                return this.isSnapped;
            }

            set
            {
                if (value == this.isSnapped)
                {
                    return;
                }

                if (value)
                {
                    if (this.IsVisible)
                    {
                        // Render the freezed image
                        var renderTargetBitmap = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                        renderTargetBitmap.Render((Visual)VisualTreeHelper.GetChild(this, 0));
                        this.snappedImage.FlowDirection = this.FlowDirection;
                        this.snappedImage.Source = renderTargetBitmap;
                        this.snappedImage.Width = this.ActualWidth;
                        this.snappedImage.Height = this.ActualHeight;
                        this.snappedImage.Visibility = Visibility.Visible;
                        this.isSnapped = true;
                    }
                }
                else if (this.snappedImage != null)
                {
                    // Clean up
                    this.snappedImage.Visibility = Visibility.Collapsed;
                    this.isSnapped = false;
                }

                this.InvalidateVisual();
            }
        }

        #endregion

        #region Caching

#pragma warning disable 414
        // Pair of chached states
        private struct StateScale
        {
            public RibbonGroupBoxState State;
            public int Scale;
        }
#pragma warning restore 414

        // Cache
        private readonly Dictionary<StateScale, Size> cachedMeasures = new Dictionary<StateScale, Size>();

        /// <summary>
        /// Gets or sets intermediate state of the group box
        /// </summary>
        internal RibbonGroupBoxState StateIntermediate { get; set; }

        /// <summary>
        /// Gets or sets intermediate scale of the group box
        /// </summary>
        internal int ScaleIntermediate { get; set; }

        /// <summary>
        /// Gets intermediate desired size
        /// </summary>
        internal Size DesiredSizeIntermediate
        {
            get
            {
                var stateScale = this.GetCurrentIntermediateStateScale();

                if (this.cachedMeasures.TryGetValue(stateScale, out var result) == false)
                {
                    var contentHeight = UIHelper.GetParent<RibbonTabControl>(this)?.ContentHeight ?? RibbonTabControl.DefaultContentHeight;

                    this.SuppressCacheReseting = true;
                    this.UpdateScalableControlSubscribing();

                    // Get desired size for these values
                    var backupState = this.State;
                    var backupScale = this.Scale;
                    this.State = this.StateIntermediate;
                    this.Scale = this.ScaleIntermediate;
                    this.InvalidateLayout();
                    this.Measure(new Size(double.PositiveInfinity, contentHeight));
                    this.cachedMeasures.Remove(stateScale);
                    this.cachedMeasures.Add(stateScale, this.DesiredSize);
                    result = this.DesiredSize;

                    // Rollback changes
                    this.State = backupState;
                    this.Scale = backupScale;
                    this.InvalidateLayout();
                    this.Measure(new Size(double.PositiveInfinity, contentHeight));

                    this.SuppressCacheReseting = false;
                }

                return result;
            }
        }

        private void TryClearCache()
        {
            if (this.SuppressCacheReseting == false)
            {
                this.ClearCache();
            }
        }

        /// <summary>
        /// Clears cache
        /// </summary>
        public void ClearCache()
        {
            this.cachedMeasures.Clear();
        }

        /// <summary>
        /// Invalidates layout (with children)
        /// </summary>
        internal void InvalidateLayout()
        {
            InvalidateMeasureRecursive(this);
        }

        private static void InvalidateMeasureRecursive(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            element.InvalidateMeasure();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;

                if (child == null)
                {
                    continue;
                }

                InvalidateMeasureRecursive(child);
            }
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.UnSubscribeEvents();

            // Clear cache
            this.ClearCache();

            this.LauncherButton = this.GetTemplateChild("PART_DialogLauncherButton") as Button;

            if (this.LauncherButton != null)
            {
                if (this.LauncherKeys != null)
                {
                    this.LauncherButton.KeyTip = this.LauncherKeys;
                }
            }

            this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

            this.upPanel = this.GetTemplateChild("PART_UpPanel") as Panel;
            this.parentPanel = this.GetTemplateChild("PART_ParentPanel") as Panel;

            this.snappedImage = this.GetTemplateChild("PART_SnappedImage") as Image;

            this.SubscribeEvents();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            this.DropDownOpened?.Invoke(this, e);
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            this.DropDownClosed?.Invoke(this, e);
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (ReferenceEquals(e.Source, this) == false
                || this.DropDownPopup == null)
            {
                return;
            }

            if (this.State == RibbonGroupBoxState.Collapsed
                || this.State == RibbonGroupBoxState.QuickAccess)
            {
                e.Handled = true;

                if (!this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = true;
                }
                else
                {
                    PopupService.RaiseDismissPopupEventAsync(this, DismissPopupMode.MouseNotOver);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);

            this.cachedMeasures.Remove(this.GetCurrentIntermediateStateScale());
        }

        private StateScale GetCurrentIntermediateStateScale()
        {
            var stateScale = new StateScale
            {
                Scale = this.ScaleIntermediate,
                State = this.StateIntermediate
            };
            return stateScale;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Dialog launcher button click handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">the event data</param>
        private void OnDialogLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            this.LauncherClick?.Invoke(this, e);
        }

        /// <summary>
        /// Handles IsOpen propertyu changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (RibbonGroupBox)d;

            ribbon.OnIsDropDownOpenChanged();
        }

        private void OnIsDropDownOpenChanged()
        {
            if (this.IsDropDownOpen)
            {
                this.OnRibbonGroupBoxPopupOpening();
            }
            else
            {
                this.OnRibbonGroupBoxPopupClosing();
            }
        }

        // Handles popup closing
        private void OnRibbonGroupBoxPopupClosing()
        {
            //IsHitTestVisible = true;
            if (ReferenceEquals(Mouse.Captured, this))
            {
                Mouse.Capture(null);
            }
        }

        // handles popup opening
        private void OnRibbonGroupBoxPopupOpening()
        {
            //IsHitTestVisible = false;
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        #endregion

        #region Quick Access Item Creating

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var groupBox = new RibbonGroupBox();

            RibbonControl.BindQuickAccessItem(this, groupBox);

            groupBox.DropDownOpened += this.OnQuickAccessOpened;
            groupBox.DropDownClosed += this.OnQuickAccessClosed;

            groupBox.State = RibbonGroupBoxState.QuickAccess;

            RibbonControl.Bind(this, groupBox, nameof(this.ItemTemplateSelector), ItemTemplateSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.ItemsSource), ItemsSourceProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherCommandParameter), LauncherCommandParameterProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherCommand), LauncherCommandProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherCommandTarget), LauncherCommandTargetProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherIcon), LauncherIconProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherText), LauncherTextProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherToolTip), LauncherToolTipProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.IsLauncherEnabled), IsLauncherEnabledProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.IsLauncherVisible), IsLauncherVisibleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, nameof(this.LauncherKeys), LauncherKeysProperty, BindingMode.OneWay);
            groupBox.LauncherClick += this.LauncherClick;

            if (this.Icon != null)
            {
                if (this.Icon is Visual iconVisual)
                {
                    var rect = new Rectangle
                    {
                        Width = 16,
                        Height = 16,
                        Fill = new VisualBrush(iconVisual)
                    };
                    groupBox.Icon = rect;
                }
                else
                {
                    RibbonControl.Bind(this, groupBox, nameof(this.Icon), RibbonControl.IconProperty, BindingMode.OneWay);
                }
            }

            return groupBox;
        }

        private void OnQuickAccessOpened(object sender, EventArgs e)
        {
            if (this.IsDropDownOpen == false
                && this.IsSnapped == false)
            {
                var groupBox = (RibbonGroupBox)sender;
                // Save state
                this.IsSnapped = true;

                if (this.ItemsSource == null)
                {
                    for (var i = 0; i < this.Items.Count; i++)
                    {
                        var item = this.Items[0];
                        this.Items.Remove(item);
                        groupBox.Items.Add(item);
                        i--;
                    }
                }
            }
        }

        private void OnQuickAccessClosed(object sender, EventArgs e)
        {
            var groupBox = (RibbonGroupBox)sender;

            if (this.ItemsSource == null)
            {
                for (var i = 0; i < groupBox.Items.Count; i++)
                {
                    var item = groupBox.Items[0];
                    groupBox.Items.Remove(item);
                    this.Items.Add(item);
                    i--;
                }
            }

            this.IsSnapped = false;
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty =
            DependencyProperty.Register(nameof(CanAddToQuickAccessToolBar), typeof(bool), typeof(RibbonGroupBox), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        #endregion

        #region Implementation of IKeyTipedControl

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            if (this.State == RibbonGroupBoxState.Collapsed
                || this.State == RibbonGroupBoxState.QuickAccess)
            {
                this.IsDropDownOpen = true;

                if (this.DropDownPopup?.Child != null)
                {
                    Keyboard.Focus(this.DropDownPopup.Child);
                    this.DropDownPopup.Child.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));

                    return new KeyTipPressedResult(true, true);
                }

                return new KeyTipPressedResult(false, true);
            }

            return KeyTipPressedResult.Empty;
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
            this.IsDropDownOpen = false;
        }

        #endregion

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
    }
}