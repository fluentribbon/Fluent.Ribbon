// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    ///     Represents custom Fluent UI ComboBox
    /// </summary>
    [TemplatePart(Name = "PART_ToggleButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_SelectedImage", Type = typeof(Image))]
    [TemplatePart(Name = "PART_ContentSite", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_ContentBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [DebuggerDisplay("class{GetType().FullName}: Header = {Header}, Items.Count = {Items.Count}, Size = {Size}, IsSimplified = {IsSimplified}")]
    public class ComboBox : System.Windows.Controls.ComboBox, IQuickAccessItemProvider, IRibbonControl, IDropDownControl, IMediumIconProvider, ISimplifiedRibbonControl
    {
        #region Fields
        private ToggleButton? dropDownButton;

        private IInputElement? focusedElement;

        private Border? contentBorder;

        private ContentPresenter? contentSite;

        // Freezed image (created during snapping)
        private Image? snappedImage;

        // Is visual currently snapped
        private bool isSnapped;

        private ScrollViewer? scrollViewer;

        #endregion

        #region Properties

        #region Size

        /// <inheritdoc />
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region SizeDefinition

        /// <inheritdoc />
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region SimplifiedSizeDefinition

        /// <inheritdoc />
        public RibbonControlSizeDefinition SimplifiedSizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SimplifiedSizeDefinitionProperty); }
            set { this.SetValue(SimplifiedSizeDefinitionProperty, value); }
        }

        /// <summary>Identifies the <see cref="SimplifiedSizeDefinition"/> dependency property.</summary>
        public static readonly DependencyProperty SimplifiedSizeDefinitionProperty = RibbonProperties.SimplifiedSizeDefinitionProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region KeyTip

        /// <inheritdoc />
        public string? KeyTip
        {
            get { return (string?)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>Identifies the <see cref="KeyTip"/> dependency property.</summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(ComboBox));

        #endregion

        /// <inheritdoc />
        public Popup? DropDownPopup { get; private set; }

        /// <inheritdoc />
        public bool IsContextMenuOpened { get; set; }

        #region Header

        /// <inheritdoc />
        public object? Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ComboBox), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region Icon

        /// <inheritdoc />
        public object? Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ComboBox), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region MediumIcon

        /// <inheritdoc />
        public object? MediumIcon
        {
            get { return this.GetValue(MediumIconProperty); }
            set { this.SetValue(MediumIconProperty, value); }
        }

        /// <summary>Identifies the <see cref="MediumIcon"/> dependency property.</summary>
        public static readonly DependencyProperty MediumIconProperty = MediumIconProviderProperties.MediumIconProperty.AddOwner(typeof(ComboBox), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region TopPopupContent

        /// <summary>
        /// Gets or sets content to show on the top side of the Popup.
        /// </summary>
        public object? TopPopupContent
        {
            get { return (object?)this.GetValue(TopPopupContentProperty); }
            set { this.SetValue(TopPopupContentProperty, value); }
        }

        /// <summary>Identifies the <see cref="TopPopupContent"/> dependency property.</summary>
        public static readonly DependencyProperty TopPopupContentProperty =
            DependencyProperty.Register(nameof(TopPopupContent), typeof(object), typeof(ComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        /// <summary>
        /// Gets or sets top content template.
        /// </summary>
        public DataTemplate? TopPopupContentTemplate
        {
            get { return (DataTemplate?)this.GetValue(TopPopupContentTemplateProperty); }
            set { this.SetValue(TopPopupContentTemplateProperty, value); }
        }

        /// <summary>Identifies the <see cref="TopPopupContentTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty TopPopupContentTemplateProperty =
            DependencyProperty.Register(nameof(TopPopupContentTemplate), typeof(DataTemplate), typeof(ComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets top content template selector.
        /// </summary>
        public DataTemplateSelector? TopPopupContentTemplateSelector
        {
            get { return (DataTemplateSelector?)this.GetValue(TopPopupContentTemplateSelectorProperty); }
            set { this.SetValue(TopPopupContentTemplateSelectorProperty, value); }
        }

        /// <summary>Identifies the <see cref="TopPopupContentTemplateSelector"/> dependency property.</summary>
        public static readonly DependencyProperty TopPopupContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(TopPopupContentTemplateSelector), typeof(DataTemplateSelector), typeof(ComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Gets or sets top content template string format.
        /// </summary>
        public string? TopPopupContentStringFormat
        {
            get { return (string?)this.GetValue(TopPopupContentStringFormatProperty); }
            set { this.SetValue(TopPopupContentStringFormatProperty, value); }
        }

        /// <summary>Identifies the <see cref="TopPopupContentStringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty TopPopupContentStringFormatProperty =
            DependencyProperty.Register(nameof(TopPopupContentStringFormat), typeof(string), typeof(ComboBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region Menu

        /// <summary>
        ///     Gets or sets menu to show in combo box bottom
        /// </summary>
        public RibbonMenu? Menu
        {
            get { return (RibbonMenu?)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        /// <summary>Identifies the <see cref="Menu"/> dependency property.</summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register(nameof(Menu), typeof(RibbonMenu), typeof(ComboBox), new PropertyMetadata());

        #endregion

        #region InputWidth

        /// <summary>
        ///     Gets or sets width of the value input part of combobox
        /// </summary>
        public double InputWidth
        {
            get { return (double)this.GetValue(InputWidthProperty); }
            set { this.SetValue(InputWidthProperty, value); }
        }

        /// <summary>Identifies the <see cref="InputWidth"/> dependency property.</summary>
        public static readonly DependencyProperty InputWidthProperty =
            DependencyProperty.Register(nameof(InputWidth), typeof(double), typeof(ComboBox), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region ResizeMode

        /// <summary>
        ///     Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)this.GetValue(ResizeModeProperty); }
            set { this.SetValue(ResizeModeProperty, value); }
        }

        /// <summary>Identifies the <see cref="ResizeMode"/> dependency property.</summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register(nameof(ResizeMode), typeof(ContextMenuResizeMode), typeof(ComboBox), new PropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region Snapping

        /// <summary>
        ///     Snaps / Unsnaps the Visual
        ///     (remove visuals and substitute with freezed image)
        /// </summary>
        private bool IsSnapped
        {
            get { return this.isSnapped; }

            set
            {
                if (value == this.isSnapped)
                {
                    return;
                }

                if (this.snappedImage is null
                    || this.contentSite is null)
                {
                    return;
                }

                if (value && ((int)this.contentSite.ActualWidth > 0) && ((int)this.contentSite.ActualHeight > 0))
                {
                    // Render the freezed image
                    RenderOptions.SetBitmapScalingMode(this.snappedImage, BitmapScalingMode.NearestNeighbor);
                    var renderTargetBitmap = new RenderTargetBitmap((int)this.contentSite.ActualWidth + (int)this.contentSite.Margin.Left + (int)this.contentSite.Margin.Right,
                        (int)this.contentSite.ActualHeight + (int)this.contentSite.Margin.Top + (int)this.contentSite.Margin.Bottom, 96, 96,
                        PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(this.contentSite);
                    this.snappedImage.Source = renderTargetBitmap;
                    this.snappedImage.FlowDirection = this.FlowDirection;
                    /*snappedImage.Width = contentSite.ActualWidth;
                    snappedImage.Height = contentSite.ActualHeight;*/
                    this.snappedImage.Visibility = Visibility.Visible;
                    this.contentSite.Visibility = Visibility.Hidden;
                    this.isSnapped = value;
                }
                else
                {
                    this.snappedImage.Visibility = Visibility.Collapsed;
                    this.contentSite.Visibility = Visibility.Visible;
                    this.isSnapped = value;
                }

                this.InvalidateVisual();
            }
        }

        #endregion

        #region DropDownHeight

        /// <summary>
        ///     Gets or sets initial dropdown height
        /// </summary>
        public double DropDownHeight
        {
            get { return (double)this.GetValue(DropDownHeightProperty); }
            set { this.SetValue(DropDownHeightProperty, value); }
        }

        /// <summary>Identifies the <see cref="DropDownHeight"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(ComboBox), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #region IsSimplified

        /// <summary>
        /// Gets or sets whether or not the ribbon is in Simplified mode
        /// </summary>
        public bool IsSimplified
        {
            get { return (bool)this.GetValue(IsSimplifiedProperty); }
            private set { this.SetValue(IsSimplifiedPropertyKey, BooleanBoxes.Box(value)); }
        }

        private static readonly DependencyPropertyKey IsSimplifiedPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsSimplified), typeof(bool), typeof(ComboBox), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>Identifies the <see cref="IsSimplified"/> dependency property.</summary>
        public static readonly DependencyProperty IsSimplifiedProperty = IsSimplifiedPropertyKey.DependencyProperty;

        #endregion

        #endregion Properties

        #region Constructors

        /// <summary>
        ///     Static constructor
        /// </summary>
        static ComboBox()
        {
            var type = typeof(ComboBox);

            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(OnSelectedItemChanged, CoerceSelectedItem));

            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var combo = (ComboBox)d;
            if (combo.isQuickAccessOpened == false
                && combo.isQuickAccessFocused == false
                && combo.quickAccessCombo is not null)
            {
                combo.UpdateQuickAccessCombo();
            }
        }

        private static object? CoerceSelectedItem(DependencyObject d, object? basevalue)
        {
            var combo = (ComboBox)d;
            if (combo.isQuickAccessOpened
                || combo.isQuickAccessFocused)
            {
                return combo.selectedItem;
            }

            return basevalue;
        }

        /// <summary>
        ///     Default Constructor
        /// </summary>
        public ComboBox()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region QuickAccess

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var combo = new ComboBox();
            RibbonControl.BindQuickAccessItem(this, combo);
            RibbonControl.Bind(this, combo, nameof(this.ActualWidth), MaxWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.InputWidth), InputWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.IsEditable), IsEditableProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.IsReadOnly), IsReadOnlyProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.Text), TextProperty, BindingMode.TwoWay);

            RibbonControl.Bind(this, combo, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.SelectedValuePath), SelectedValuePathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);
            combo.DropDownOpened += this.OnQuickAccessOpened;
            if (this.IsEditable)
            {
                combo.GotFocus += this.OnQuickAccessTextBoxGetFocus;
            }

            this.quickAccessCombo = combo;
            this.UpdateQuickAccessCombo();
            return combo;
        }

        private void OnQuickAccessTextBoxGetFocus(object? sender, RoutedEventArgs e)
        {
            if (this.quickAccessCombo is null)
            {
                return;
            }

            this.isQuickAccessFocused = true;
            if (!this.isQuickAccessOpened)
            {
                this.Freeze();
            }

            this.quickAccessCombo.LostFocus += this.OnQuickAccessTextBoxLostFocus;
        }

        private void OnQuickAccessTextBoxLostFocus(object? sender, RoutedEventArgs e)
        {
            if (this.quickAccessCombo is null)
            {
                return;
            }

            this.quickAccessCombo.LostFocus -= this.OnQuickAccessTextBoxLostFocus;
            if (!this.isQuickAccessOpened)
            {
                this.Unfreeze();
            }

            this.isQuickAccessFocused = false;
        }

        private bool isQuickAccessFocused;
        private bool isQuickAccessOpened;
        private object? selectedItem;
        private ComboBox? quickAccessCombo;

        private void OnQuickAccessOpened(object? sender, EventArgs e)
        {
            if (this.quickAccessCombo is null)
            {
                return;
            }

            this.isQuickAccessOpened = true;
            this.quickAccessCombo.DropDownClosed += this.OnQuickAccessMenuClosed;
            this.quickAccessCombo.UpdateLayout();

            if (this.isQuickAccessFocused == false)
            {
                this.RunInDispatcherAsync(this.FreezeAnBringSelectedItemIntoView);
            }
        }

        private void FreezeAnBringSelectedItemIntoView()
        {
            this.Freeze();
            this.RunInDispatcherAsync(this.BringSelectedItemIntoView, DispatcherPriority.Input);
        }

        private void BringSelectedItemIntoView()
        {
            if (this.quickAccessCombo?.SelectedItem is null)
            {
                return;
            }

            var containerFromItem = this.quickAccessCombo.ItemContainerGenerator.ContainerOrContainerContentFromItem<FrameworkElement>(this.quickAccessCombo.SelectedItem);
            containerFromItem?.BringIntoView();
        }

        private void OnQuickAccessMenuClosed(object? sender, EventArgs e)
        {
            if (this.quickAccessCombo is not null)
            {
                this.quickAccessCombo.DropDownClosed -= this.OnQuickAccessMenuClosed;
            }

            if (!this.isQuickAccessFocused)
            {
                this.Unfreeze();
            }

            this.isQuickAccessOpened = false;
        }

        private void Freeze()
        {
            if (this.quickAccessCombo is null)
            {
                return;
            }

            this.IsSnapped = true;
            this.selectedItem = this.SelectedItem;

            ItemsControlHelper.MoveItemsToDifferentControl(this, this.quickAccessCombo);

            this.SelectedItem = null;
            this.quickAccessCombo.SelectedItem = this.selectedItem;
            this.quickAccessCombo.Menu = this.Menu;
            this.Menu = null;
            this.quickAccessCombo.IsSnapped = false;
        }

        private void Unfreeze()
        {
            if (this.quickAccessCombo is null)
            {
                return;
            }

            var text = this.quickAccessCombo.Text;
            this.selectedItem = this.quickAccessCombo.SelectedItem;
            this.quickAccessCombo.IsSnapped = true;

            ItemsControlHelper.MoveItemsToDifferentControl(this.quickAccessCombo, this);

            this.quickAccessCombo.SelectedItem = null;
            this.SelectedItem = this.selectedItem;
            this.Menu = this.quickAccessCombo.Menu;
            this.quickAccessCombo.Menu = null;
            this.IsSnapped = false;
            this.Text = text;
            this.UpdateLayout();
        }

        private void UpdateQuickAccessCombo()
        {
            if (this.IsLoaded == false)
            {
                this.Loaded += this.OnFirstLoaded;
            }

            if (this.quickAccessCombo is null)
            {
                return;
            }

            if (this.IsEditable == false)
            {
                this.RunInDispatcherAsync(() =>
                                          {
                                              this.quickAccessCombo.IsSnapped = true;
                                              this.IsSnapped = true;
                                              if (this.snappedImage is not null &&
                                                  this.quickAccessCombo.snappedImage is not null)
                                              {
                                                  this.quickAccessCombo.snappedImage.Source = this.snappedImage.Source;
                                                  this.quickAccessCombo.snappedImage.Visibility = Visibility.Visible;
                                                  if (this.quickAccessCombo.IsSnapped == false)
                                                  {
                                                      this.quickAccessCombo.isSnapped = true;
                                                  }
                                              }

                                              this.IsSnapped = false;
                                          }, DispatcherPriority.ApplicationIdle);
            }
        }

        private void OnFirstLoaded(object? sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnFirstLoaded;
            this.UpdateQuickAccessCombo();
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value)); }
        }

        /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ComboBox), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.dropDownButton = this.GetTemplateChild("PART_ToggleButton") as ToggleButton;
            if (this.dropDownButton is ISimplifiedStateControl control)
            {
                control.UpdateSimplifiedState(this.IsSimplified);
            }

            this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

            this.snappedImage = this.GetTemplateChild("PART_SelectedImage") as Image;
            this.contentSite = this.GetTemplateChild("PART_ContentSite") as ContentPresenter;

            if (this.contentBorder is not null)
            {
                this.contentBorder.PreviewMouseDown -= this.OnContentBorderPreviewMouseDown;
            }

            this.contentBorder = this.GetTemplateChild("PART_ContentBorder") as Border;
            if (this.contentBorder is not null)
            {
                this.contentBorder.PreviewMouseDown += this.OnContentBorderPreviewMouseDown;
            }

            this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            base.OnApplyTemplate();
        }

        /// <inheritdoc />
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);

            Mouse.Capture(this, CaptureMode.SubTree);

            if (this.SelectedItem is not null)
            {
                Keyboard.Focus(this.ItemContainerGenerator.ContainerOrContainerContentFromItem<IInputElement>(this.SelectedItem));
            }

            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement is not null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;
            }

            if (this.scrollViewer is not null)
            {
                this.scrollViewer.Width = double.NaN;
                this.scrollViewer.Height = double.NaN;
            }

            var popupChild = this.DropDownPopup?.Child as FrameworkElement;

            var initialHeight = Math.Min(RibbonControl.GetControlWorkArea(this).Height * 2 / 3, this.MaxDropDownHeight);

            if (double.IsNaN(this.DropDownHeight) == false)
            {
                initialHeight = Math.Min(this.DropDownHeight, this.MaxDropDownHeight);
            }

            if (this.scrollViewer?.DesiredSize.Height > initialHeight)
            {
                this.scrollViewer.Height = initialHeight;
            }

            popupChild?.UpdateLayout();
        }

        /// <inheritdoc />
        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            if (ReferenceEquals(Mouse.Captured, this))
            {
                Mouse.Capture(null);
            }

            if (this.focusedElement is not null)
            {
                this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
            }

            this.focusedElement = null;

            if (this.scrollViewer is not null)
            {
                this.scrollViewer.Width = double.NaN;
                this.scrollViewer.Height = double.NaN;
            }
        }

        private void OnFocusedElementLostKeyboardFocus(object? sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.focusedElement is not null)
            {
                this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
            }

            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement is not null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;

                if (this.IsEditable &&
                    this.Items.Contains(this.ItemContainerGenerator.ItemFromContainerOrContainerContent((DependencyObject)Keyboard.FocusedElement)))
                {
                    this.SelectedItem = this.ItemContainerGenerator.ItemFromContainerOrContainerContent((DependencyObject)Keyboard.FocusedElement);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.IsEditable
                && ((e.Key == Key.Down) || (e.Key == Key.Up))
                && !this.IsDropDownOpen)
            {
                this.IsDropDownOpen = true;
                e.Handled = true;
                return;
            }

            base.OnPreviewKeyDown(e);
        }

        /// <inheritdoc />
        protected override void OnKeyDown(KeyEventArgs e)
        {
            var baseKeyDownCalled = false;

            if ((this.Menu is not null && this.Menu.IsKeyboardFocusWithin == false)
                && e.Key != Key.Tab)
            {
                base.OnKeyDown(e);

                baseKeyDownCalled = true;

                if (e.Handled)
                {
                    return;
                }
            }

            if (this.Menu is not null
                && this.Menu.Items.IsEmpty == false)
            {
                if (e.Key == Key.Tab)
                {
                    if (this.Menu.IsKeyboardFocusWithin)
                    {
                        Keyboard.Focus(this.ItemContainerGenerator.ContainerOrContainerContentFromIndex<IInputElement>(0));
                    }
                    else
                    {
                        Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerOrContainerContentFromIndex<IInputElement>(0));
                    }

                    e.Handled = true;
                    return;
                }

                if (this.Menu.Items.Contains(this.Menu.ItemContainerGenerator.ItemFromContainerOrContainerContent((DependencyObject)Keyboard.FocusedElement)))
                {
                    if (e.Key == Key.Down)
                    {
                        var indexOfMenuSelectedItem = this.Menu.ItemContainerGenerator.IndexFromContainer((DependencyObject)Keyboard.FocusedElement);

                        if (indexOfMenuSelectedItem != this.Menu.Items.Count - 1)
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerOrContainerContentFromIndex<IInputElement>(indexOfMenuSelectedItem + 1));
                        }
                        else
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerOrContainerContentFromIndex<IInputElement>(0));
                        }

                        e.Handled = true;
                        return;
                    }

                    if (e.Key == Key.Up)
                    {
                        var indexOfMenuSelectedItem = this.Menu.ItemContainerGenerator.IndexFromContainer((DependencyObject)Keyboard.FocusedElement);

                        if (indexOfMenuSelectedItem != 0)
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerOrContainerContentFromIndex<IInputElement>(indexOfMenuSelectedItem - 1));
                        }
                        else
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerOrContainerContentFromIndex<IInputElement>(this.Menu.Items.Count - 1));
                        }

                        e.Handled = true;
                        return;
                    }
                }
            }

            if (baseKeyDownCalled == false
                && e.Handled == false)
            {
                base.OnKeyDown(e);
            }
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public virtual KeyTipPressedResult OnKeyTipPressed()
        {
            // Edge case: Whole dropdown content is disabled
            if (this.IsKeyboardFocusWithin == false)
            {
                Keyboard.Focus(this);
            }

            if (this.IsEditable == false)
            {
                this.IsDropDownOpen = true;

                return new KeyTipPressedResult(true, true);
            }

            return new KeyTipPressedResult(true, false);
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }

        #endregion

        #region Private methods

        // Prevent reopenning of the dropdown menu (popup)
        private void OnContentBorderPreviewMouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        #endregion

        /// <inheritdoc />
        void ISimplifiedStateControl.UpdateSimplifiedState(bool isSimplified)
        {
            this.IsSimplified = isSimplified;
            if (this.dropDownButton is ISimplifiedStateControl control)
            {
                control.UpdateSimplifiedState(isSimplified);
            }
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

                if (this.Icon is not null)
                {
                    yield return this.Icon;
                }

                if (this.MediumIcon is not null)
                {
                    yield return this.MediumIcon;
                }

                if (this.Header is not null)
                {
                    yield return this.Header;
                }

                if (this.TopPopupContent is not null)
                {
                    yield return this.TopPopupContent;
                }
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonComboBoxAutomationPeer(this);
    }
}