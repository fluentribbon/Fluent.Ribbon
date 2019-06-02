// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
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
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    ///     Represents custom Fluent UI ComboBox
    /// </summary>
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_SelectedImage", Type = typeof(Image))]
    [TemplatePart(Name = "PART_ContentSite", Type = typeof(ContentPresenter))]
    [TemplatePart(Name = "PART_ContentBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    [TemplatePart(Name = "PART_DropDownBorder", Type = typeof(Border))]
    public class ComboBox : System.Windows.Controls.ComboBox, IQuickAccessItemProvider, IRibbonControl, IDropDownControl
    {
        #region Fields

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;
        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        private IInputElement focusedElement;

        private Panel menuPanel;

        private Border dropDownBorder;
        private Border contentBorder;

        private ContentPresenter contentSite;

        // Freezed image (created during snapping)
        private Image snappedImage;

        // Is visual currently snapped
        private bool isSnapped;

        private ScrollViewer scrollViewer;

        private bool canSizeY;

        #endregion

        #region Properties

        #region Size

        /// <inheritdoc />
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for Size.
        ///     This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region SizeDefinition

        /// <inheritdoc />
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for SizeDefinition.
        ///     This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region KeyTip

        /// <inheritdoc />
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for Keys.
        ///     This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(ComboBox));

        #endregion

        /// <inheritdoc />
        public Popup DropDownPopup { get; private set; }

        /// <inheritdoc />
        public bool IsContextMenuOpened { get; set; }

        #region Header

        /// <inheritdoc />
        public object Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for Header.
        ///     This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region Icon

        /// <inheritdoc />
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ComboBox), new PropertyMetadata(RibbonControl.OnIconChanged));

        #endregion

        #region Menu

        /// <summary>
        ///     Gets or sets menu to show in combo box bottom
        /// </summary>
        public RibbonMenu Menu
        {
            get { return (RibbonMenu)this.GetValue(MenuProperty); }
            set { this.SetValue(MenuProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        /// </summary>
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

        /// <summary>
        ///     Using a DependencyProperty as the backing store for InputWidth.  This enables animation, styling, binding, etc...
        /// </summary>
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

        /// <summary>
        ///     Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
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

                if (this.snappedImage == null)
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

        /// <summary>
        ///     /Using a DependencyProperty as the backing store for DropDownHeight.  This enables animation, styling, binding,
        ///     etc...
        /// </summary>
        public static readonly DependencyProperty DropDownHeightProperty =
            DependencyProperty.Register(nameof(DropDownHeight), typeof(double), typeof(ComboBox), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #endregion

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
                && combo.quickAccessCombo != null)
            {
                combo.UpdateQuickAccessCombo();
            }
        }

        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
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

        private void OnQuickAccessTextBoxGetFocus(object sender, RoutedEventArgs e)
        {
            this.isQuickAccessFocused = true;
            if (!this.isQuickAccessOpened)
            {
                this.Freeze();
            }

            this.quickAccessCombo.LostFocus += this.OnQuickAccessTextBoxLostFocus;
        }

        private void OnQuickAccessTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            this.quickAccessCombo.LostFocus -= this.OnQuickAccessTextBoxLostFocus;
            if (!this.isQuickAccessOpened)
            {
                this.Unfreeze();
            }

            this.isQuickAccessFocused = false;
        }

        private bool isQuickAccessFocused;
        private bool isQuickAccessOpened;
        private object selectedItem;
        private ComboBox quickAccessCombo;

        private void OnQuickAccessOpened(object sender, EventArgs e)
        {
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
            if (this.quickAccessCombo.SelectedItem == null)
            {
                return;
            }

            var containerFromItem = this.quickAccessCombo.ItemContainerGenerator.ContainerFromItem(this.quickAccessCombo.SelectedItem) as FrameworkElement;
            containerFromItem?.BringIntoView();
        }

        private void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            this.quickAccessCombo.DropDownClosed -= this.OnQuickAccessMenuClosed;
            if (!this.isQuickAccessFocused)
            {
                this.Unfreeze();
            }

            this.isQuickAccessOpened = false;
        }

        private void Freeze()
        {
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

            if (this.IsEditable == false)
            {
                this.RunInDispatcherAsync(() =>
                                          {
                                              this.quickAccessCombo.IsSnapped = true;
                                              this.IsSnapped = true;
                                              if (this.snappedImage != null &&
                                                  this.quickAccessCombo.snappedImage != null)
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

        private void OnFirstLoaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= this.OnFirstLoaded;
            this.UpdateQuickAccessCombo();
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        ///     Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling,
        ///     binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ComboBox), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta -= this.OnResizeVerticalDelta;
            }

            this.resizeVerticalThumb = this.GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta += this.OnResizeVerticalDelta;
            }

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta -= this.OnResizeBothDelta;
            }

            this.resizeBothThumb = this.GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta += this.OnResizeBothDelta;
            }

            this.menuPanel = this.GetTemplateChild("PART_MenuPanel") as Panel;

            this.snappedImage = this.GetTemplateChild("PART_SelectedImage") as Image;
            this.contentSite = this.GetTemplateChild("PART_ContentSite") as ContentPresenter;

            if (this.contentBorder != null)
            {
                this.contentBorder.PreviewMouseDown -= this.OnContentBorderPreviewMouseDown;
            }

            this.contentBorder = this.GetTemplateChild("PART_ContentBorder") as Border;
            if (this.contentBorder != null)
            {
                this.contentBorder.PreviewMouseDown += this.OnContentBorderPreviewMouseDown;
            }

            this.scrollViewer = this.GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            this.dropDownBorder = this.GetTemplateChild("PART_DropDownBorder") as Border;

            base.OnApplyTemplate();
        }

        /// <inheritdoc />
        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);

            Mouse.Capture(this, CaptureMode.SubTree);

            if (this.SelectedItem != null)
            {
                Keyboard.Focus(this.ItemContainerGenerator.ContainerFromItem(this.SelectedItem) as IInputElement);
            }

            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;
            }

            this.canSizeY = true;

            this.scrollViewer.Width = double.NaN;
            this.scrollViewer.Height = double.NaN;

            var popupChild = this.DropDownPopup.Child as FrameworkElement;

            var initialHeight = Math.Min(RibbonControl.GetControlWorkArea(this).Height * 2 / 3, this.MaxDropDownHeight);

            if (double.IsNaN(this.DropDownHeight) == false)
            {
                initialHeight = Math.Min(this.DropDownHeight, this.MaxDropDownHeight);
            }

            if (this.scrollViewer.DesiredSize.Height > initialHeight)
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

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
            }

            this.focusedElement = null;
            this.scrollViewer.Width = double.NaN;
            this.scrollViewer.Height = double.NaN;
        }

        private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus -= this.OnFocusedElementLostKeyboardFocus;
            }

            this.focusedElement = Keyboard.FocusedElement;

            if (this.focusedElement != null)
            {
                this.focusedElement.LostKeyboardFocus += this.OnFocusedElementLostKeyboardFocus;

                if (this.IsEditable &&
                    this.Items.Contains(this.ItemContainerGenerator.ItemFromContainer((DependencyObject)Keyboard.FocusedElement)))
                {
                    this.SelectedItem = this.ItemContainerGenerator.ItemFromContainer((DependencyObject)Keyboard.FocusedElement);
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

            if ((this.Menu != null && this.Menu.IsKeyboardFocusWithin == false)
                && e.Key != Key.Tab)
            {
                base.OnKeyDown(e);

                baseKeyDownCalled = true;

                if (e.Handled)
                {
                    return;
                }
            }

            if (this.Menu != null
                && this.Menu.Items.IsEmpty == false)
            {
                if (e.Key == Key.Tab)
                {
                    if (this.Menu.IsKeyboardFocusWithin)
                    {
                        Keyboard.Focus(this.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
                    }
                    else
                    {
                        Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
                    }

                    e.Handled = true;
                    return;
                }

                if (this.Menu.Items.Contains(this.Menu.ItemContainerGenerator.ItemFromContainer((DependencyObject)Keyboard.FocusedElement)))
                {
                    if (e.Key == Key.Down)
                    {
                        var indexOfMenuSelectedItem = this.Menu.ItemContainerGenerator.IndexFromContainer((DependencyObject)Keyboard.FocusedElement);

                        if (indexOfMenuSelectedItem != this.Menu.Items.Count - 1)
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(indexOfMenuSelectedItem + 1) as IInputElement);
                        }
                        else
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
                        }

                        e.Handled = true;
                        return;
                    }

                    if (e.Key == Key.Up)
                    {
                        var indexOfMenuSelectedItem = this.Menu.ItemContainerGenerator.IndexFromContainer((DependencyObject)Keyboard.FocusedElement);

                        if (indexOfMenuSelectedItem != 0)
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(indexOfMenuSelectedItem - 1) as IInputElement);
                        }
                        else
                        {
                            Keyboard.Focus(this.Menu.ItemContainerGenerator.ContainerFromIndex(this.Menu.Items.Count - 1) as IInputElement);
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
        private void OnContentBorderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsDropDownOpen)
            {
                this.IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            // Set height
            this.SetDragHeight(e);

            // Set width
            this.menuPanel.Width = double.NaN;
            if (double.IsNaN(this.scrollViewer.Width))
            {
                this.scrollViewer.Width = this.scrollViewer.ActualWidth;
            }

            var monitorRight = RibbonControl.GetControlMonitor(this).Right;
            var popupChild = this.DropDownPopup.Child as FrameworkElement;

            if (popupChild == null)
            {
                return;
            }

            var delta = monitorRight - this.PointToScreen(default).X - popupChild.ActualWidth - e.HorizontalChange;
            var deltaX = popupChild.ActualWidth - this.scrollViewer.ActualWidth;
            var deltaBorders = this.dropDownBorder.ActualWidth - this.scrollViewer.ActualWidth;

            if (delta > 0)
            {
                this.scrollViewer.Width = Math.Max(0, Math.Max(this.scrollViewer.Width + e.HorizontalChange, this.ActualWidth - deltaBorders));
            }
            else
            {
                this.scrollViewer.Width = Math.Max(0, Math.Max(monitorRight - this.PointToScreen(default).X - deltaX, this.ActualWidth - deltaBorders));
            }
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            this.SetDragHeight(e);
        }

        private void SetDragHeight(DragDeltaEventArgs e)
        {
            if (this.canSizeY == false)
            {
                return;
            }

            if (double.IsNaN(this.scrollViewer.Height))
            {
                this.scrollViewer.Height = this.scrollViewer.ActualHeight;
            }

            this.scrollViewer.Height = Math.Max(0, this.scrollViewer.Height + e.VerticalChange);
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

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.ComboBoxAutomationPeer(this);
    }
}