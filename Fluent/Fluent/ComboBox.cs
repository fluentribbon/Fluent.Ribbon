#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Fluent
{
    /// <summary>
    /// Represents custom Fluent UI ComboBox
    /// </summary>
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    public class ComboBox : System.Windows.Controls.ComboBox, IQuickAccessItemProvider, IRibbonControl, IDropDownControl
    {
        #region Fields

        // Thumb to resize in both directions
        Thumb resizeBothThumb;
        // Thumb to resize vertical
        Thumb resizeVerticalThumb;

        private Popup popup;

        private IInputElement focusedElement;

        private System.Windows.Controls.TextBox editableTextBox;

        private Panel menuPanel;

        private ContentPresenter contentSite;

        // Freezed image (created during snapping)
        Image snappedImage;
        // Visuals which were removed diring snapping
        Visual[] snappedVisuals;
        // Is visual currently snapped
        private bool isSnapped;

        private GalleryPanel galleryPanel;

        private ScrollViewer scrollViewer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup
        {
            get { return popup; }
        }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonControl.SizeProperty.AddOwner(typeof(ComboBox));

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonControl.AttachSizeDefinition(typeof(ComboBox));

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region Menu

        /// <summary>
        /// Gets or sets menu to show in combo box bottom
        /// </summary>
        public RibbonMenu Menu
        {
            get { return (RibbonMenu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Menu.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuProperty =
            DependencyProperty.Register("Menu", typeof(RibbonMenu), typeof(ComboBox), new UIPropertyMetadata(null));

        #endregion

        #region InputWidth

        /// <summary>
        /// Gets or sets width of the value input part of combobox
        /// </summary>               
        public double InputWidth
        {
            get { return (double)GetValue(InputWidthProperty); }
            set { SetValue(InputWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InputWidthProperty =
            DependencyProperty.Register("InputWidth", typeof(double), typeof(ComboBox), new UIPropertyMetadata(double.NaN));

        #endregion

        #region ItemHeight

        /// <summary>
        /// Gets or sets items height
        /// </summary>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(ComboBox), new UIPropertyMetadata(double.NaN));

        #endregion

        #region GroupBy

        /// <summary>
        /// Gets or sets name of property which
        /// will use to group items in the ComboBox.
        /// </summary>
        public string GroupBy
        {
            get { return (string)GetValue(GroupByProperty); }
            set { SetValue(GroupByProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupBy. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(string),
            typeof(ComboBox), new UIPropertyMetadata(null));

        #endregion

        #region ResizeMode

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode), typeof(ComboBox), new UIPropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region Snapping

        /// <summary>
        /// Snaps / Unsnaps the Visual 
        /// (remove visuals and substitute with freezed image)
        /// </summary>
        private bool IsSnapped
        {
            get
            {
                return isSnapped;
            }
            set
            {
                if (value == isSnapped) return;
                if (snappedImage == null) return;
                if ((value) && (((int)contentSite.ActualWidth > 0) && ((int)contentSite.ActualHeight > 0)))
                {
                    // Render the freezed image
                    RenderOptions.SetBitmapScalingMode(snappedImage, BitmapScalingMode.NearestNeighbor);
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)contentSite.ActualWidth + (int)contentSite.Margin.Left + (int)contentSite.Margin.Right,
                                                                                   (int)contentSite.ActualHeight + (int)contentSite.Margin.Top + (int)contentSite.Margin.Bottom, 96, 96,
                                                                                   PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(contentSite);
                    snappedImage.Source = renderTargetBitmap;                    
                    snappedImage.FlowDirection = FlowDirection;
                    /*snappedImage.Width = contentSite.ActualWidth;
                    snappedImage.Height = contentSite.ActualHeight;*/
                    snappedImage.Visibility = Visibility.Visible;
                    contentSite.Visibility = Visibility.Hidden;
                    isSnapped = value;
                }
                else
                {
                    snappedImage.Visibility = Visibility.Collapsed;
                    contentSite.Visibility = Visibility.Visible;
                    isSnapped = value;
                }

                InvalidateVisual();
            }
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ComboBox()
        {
            Type type = typeof(ComboBox);
            ToolTipService.Attach(type);
            PopupService.Attach(type);
            ContextMenuService.Attach(type);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            SelectedItemProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(OnSelectionItemChanged, CoerceSelectedItem));
            StyleProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(ComboBox));
            }

            return basevalue;
        }

        private static void OnSelectionItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboBox combo = d as ComboBox;
            if(!combo.isQuickAccessOpened && !combo.isQuickAccessFocused && (combo.quickAccessCombo!=null)) combo.UpdateQuickAccessCombo();
        }

        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            ComboBox combo = d as ComboBox;
            if (combo.isQuickAccessOpened || combo.isQuickAccessFocused) return combo.selectedItem;
            return basevalue;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ComboBox()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region QuickAccess

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            ComboBox combo = new ComboBox();
            RibbonControl.BindQuickAccessItem(this, combo);
            RibbonControl.Bind(this, combo, "GroupBy", ComboBox.GroupByProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ActualWidth", ComboBox.WidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "InputWidth", ComboBox.InputWidthProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ItemHeight", ComboBox.ItemHeightProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "IsEditable", ComboBox.IsEditableProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "IsReadOnly", ComboBox.IsReadOnlyProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ResizeMode", ComboBox.ResizeModeProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "Text", ComboBox.TextProperty, BindingMode.TwoWay);

            RibbonControl.Bind(this, combo, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "SelectedValuePath", SelectedValuePathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, combo, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.OneWay);
            combo.DropDownOpened += OnQuickAccessOpened;
            if(IsEditable) combo.GotFocus += OnQuickAccessTextBoxGetFocus;            
            quickAccessCombo = combo;
            UpdateQuickAccessCombo();
            return combo;
        }

        private void OnQuickAccessTextBoxGetFocus(object sender, RoutedEventArgs e)
        {
            isQuickAccessFocused = true;
            if (!isQuickAccessOpened) Freeze();
            quickAccessCombo.LostFocus += OnQuickAccessTextBoxLostFocus;
        }

        private void OnQuickAccessTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            quickAccessCombo.LostFocus -= OnQuickAccessTextBoxLostFocus;
            if (!isQuickAccessOpened) Unfreeze();
            isQuickAccessFocused = false;
        }

        private bool isQuickAccessFocused;
        private bool isQuickAccessOpened;
        private object selectedItem;
        private ComboBox quickAccessCombo;
        void OnQuickAccessOpened(object sender, EventArgs e)
        {
            isQuickAccessOpened = true;
            quickAccessCombo.DropDownClosed += OnQuickAccessMenuClosed;
            quickAccessCombo.UpdateLayout();
            if (!isQuickAccessFocused) Dispatcher.BeginInvoke(DispatcherPriority.Normal, ((ThreadStart)(() =>
            { Freeze();
                Dispatcher.BeginInvoke(DispatcherPriority.Input, ((ThreadStart)(() =>
            {
                if (quickAccessCombo.SelectedItem != null) (quickAccessCombo.ItemContainerGenerator.ContainerFromItem(quickAccessCombo.SelectedItem) as ComboBoxItem).BringIntoView();
            }
               )));
            }
               )));    
            
        }

        void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            quickAccessCombo.DropDownClosed -= OnQuickAccessMenuClosed;
            if (!isQuickAccessFocused) Unfreeze();
            isQuickAccessOpened = false;
        }

        private void Freeze()
        {
            IsSnapped = true;
            selectedItem = SelectedItem;            
            if (ItemsSource != null)
            {
                quickAccessCombo.ItemsSource = ItemsSource;
                ItemsSource = null;
            }
            else
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    object item = Items[0];
                    Items.Remove(item);
                    quickAccessCombo.Items.Add(item);
                    i--;
                }
            }
            SelectedItem = null;
            quickAccessCombo.SelectedItem = selectedItem;
            quickAccessCombo.Menu = Menu;
            Menu = null;
            quickAccessCombo.IsSnapped = false;            
        }

        private void Unfreeze()
        {
            string text = quickAccessCombo.Text;
            selectedItem = quickAccessCombo.SelectedItem;            
            quickAccessCombo.IsSnapped = true;
            if (quickAccessCombo.ItemsSource != null)
            {
                ItemsSource = quickAccessCombo.ItemsSource;
                quickAccessCombo.ItemsSource = null;
            }
            else
            {
                for (int i = 0; i < quickAccessCombo.Items.Count; i++)
                {
                    object item = quickAccessCombo.Items[0];
                    quickAccessCombo.Items.Remove(item);
                    Items.Add(item);
                    i--;
                }
            }            
            quickAccessCombo.SelectedItem = null;
            SelectedItem = selectedItem;
            Menu = quickAccessCombo.Menu;
            quickAccessCombo.Menu = null;            
            IsSnapped = false;
            Text = text;
            UpdateLayout();
        }

        private void UpdateQuickAccessCombo()
        {
            if (!IsLoaded) Loaded += OnFirstLoaded;
            if(!IsEditable) Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart) (() =>
                                                                                 {
                                                                                     quickAccessCombo.IsSnapped = true;
                                                                                     IsSnapped = true;
                                                                                     if (snappedImage != null && quickAccessCombo.snappedImage != null)
                                                                                     {
                                                                                         quickAccessCombo.snappedImage.
                                                                                             Source
                                                                                             = snappedImage.Source;
                                                                                         quickAccessCombo.
                                                                                                 snappedImage.
                                                                                                 Visibility =
                                                                                                 Visibility.Visible;
                                                                                         if (!quickAccessCombo.IsSnapped)
                                                                                         {
                                                                                             quickAccessCombo.isSnapped = true;
                                                                                         }
                                                                                     }
                                                                                     IsSnapped = false;
                                                                                 }));

        }

        private void OnFirstLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnFirstLoaded;
            UpdateQuickAccessCombo();
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddToQuickAccessToolBarProperty); }
            set { SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ComboBox), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        /// <summary>
        /// Gets or sets style of element on quick access toolbar
        /// </summary>
        public Style QuickAccessElementStyle
        {
            get { return (Style)GetValue(QuickAccessElementStyleProperty); }
            set { SetValue(QuickAccessElementStyleProperty, value); }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for QuickAccessElementStyle.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessElementStyleProperty = RibbonControl.QuickAccessElementStyleProperty.AddOwner(typeof(ComboBox));

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            popup = GetTemplateChild("PART_Popup") as Popup;
            editableTextBox = GetTemplateChild("PART_EditableTextBox") as System.Windows.Controls.TextBox;

            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta -= OnResizeVerticalDelta;
            }
            resizeVerticalThumb = GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta += OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta -= OnResizeBothDelta;
            }
            resizeBothThumb = GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta += OnResizeBothDelta;
            }

            menuPanel = GetTemplateChild("PART_MenuPanel") as Panel;

            snappedImage = GetTemplateChild("PART_SelectedImage") as Image;
            contentSite = GetTemplateChild("PART_ContentSite") as ContentPresenter;

            galleryPanel = GetTemplateChild("PART_GalleryPanel") as GalleryPanel;
            scrollViewer = GetTemplateChild("PART_ScrollViewer") as ScrollViewer;

            base.OnApplyTemplate();
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            Focus();
            base.OnDropDownOpened(e);
            Mouse.Capture(this, CaptureMode.SubTree);
            if (SelectedItem != null) Keyboard.Focus(ItemContainerGenerator.ContainerFromItem(SelectedItem) as IInputElement);
            focusedElement = Keyboard.FocusedElement;
            focusedElement.LostKeyboardFocus += OnFocusedElementLostKeyboardFocus;
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            if (Mouse.Captured == this) Mouse.Capture(null);
            if (focusedElement != null) focusedElement.LostKeyboardFocus -= OnFocusedElementLostKeyboardFocus;
            focusedElement = null;
        }

        private void OnFocusedElementLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (focusedElement != null) focusedElement.LostKeyboardFocus -= OnFocusedElementLostKeyboardFocus;
            focusedElement = Keyboard.FocusedElement;
            if (focusedElement != null)
            {
                focusedElement.LostKeyboardFocus += OnFocusedElementLostKeyboardFocus;
                if ((IsEditable) &&
                    (Items.Contains(ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject))))
                {
                    SelectedItem = ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject);
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if ((IsEditable) && ((e.Key == Key.Down)||(e.Key == Key.Up)) && (!IsDropDownOpen))
            {
                IsDropDownOpen = true;
                e.Handled = true;
                return;
            }
            if ((IsEditable) && ((e.Key == Key.Enter) || (e.Key == Key.Escape))&& (!IsDropDownOpen))
            {
                // Move Focus
                editableTextBox.Focusable = false;
                Focus();
                editableTextBox.Focusable = true;
                e.Handled = true;
                return;
            }
            base.OnPreviewKeyDown(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            /**/

            if (e.Key == Key.Down)
            {
                Debug.WriteLine("Down pressed. FocusedElement - " + Keyboard.FocusedElement);
                if ((Menu != null) && Menu.Items.Contains(Menu.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
                {
                    int indexOfMSelectedItem = Menu.ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
                    if (indexOfMSelectedItem != Menu.Items.Count - 1)
                    {
                        Keyboard.Focus(Menu.ItemContainerGenerator.ContainerFromIndex(indexOfMSelectedItem + 1) as IInputElement);
                    }
                    else
                    {
                        if ((Items.Count > 0) && (!IsEditable))
                        {
                            Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
                        }
                        else Keyboard.Focus(Menu.Items[0] as IInputElement);
                    }
                }
                else if (Items.Contains(ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
                {
                    int indexOfSelectedItem = ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
                    if (indexOfSelectedItem != Items.Count - 1)
                    {
                        Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(indexOfSelectedItem + 1) as IInputElement);
                    }
                    else
                    {
                        if ((Menu != null) && (Menu.Items.Count > 0) && (!IsEditable)) Keyboard.Focus(Menu.ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
                        else
                        {
                            Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(0) as IInputElement);
                        }
                    }
                }
                else if (SelectedItem != null) Keyboard.Focus(ItemContainerGenerator.ContainerFromItem(SelectedItem) as IInputElement);
                e.Handled = true;
                Debug.WriteLine("FocusedElement - " + Keyboard.FocusedElement);
                return;
            }
            else if (e.Key == Key.Up)
            {
                Debug.WriteLine("Up pressed. FocusedElement - " + Keyboard.FocusedElement);
                if ((Menu != null) && Menu.Items.Contains(Menu.ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
                {
                    int indexOfMSelectedItem = Menu.ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
                    if (indexOfMSelectedItem != 0)
                    {
                        Keyboard.Focus(Menu.ItemContainerGenerator.ContainerFromIndex(indexOfMSelectedItem - 1) as IInputElement);
                    }
                    else
                    {
                        if ((Items.Count > 0) && (!IsEditable))
                        {
                            Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as IInputElement);
                        }
                        else Keyboard.Focus(Menu.Items[Menu.Items.Count - 1] as IInputElement);
                    }
                }
                else if (Items.Contains(ItemContainerGenerator.ItemFromContainer(Keyboard.FocusedElement as DependencyObject)))
                {
                    int indexOfSelectedItem = ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
                    if (indexOfSelectedItem != 0)
                    {
                        Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(indexOfSelectedItem - 1) as IInputElement);
                    }
                    else
                    {
                        if ((Menu != null) && (Menu.Items.Count > 0) && (!IsEditable)) Keyboard.Focus(Menu.ItemContainerGenerator.ContainerFromIndex(Menu.Items.Count - 1) as IInputElement);
                        else
                        {
                            Keyboard.Focus(ItemContainerGenerator.ContainerFromIndex(Items.Count - 1) as IInputElement);
                        }
                    }
                }
                else if (SelectedItem != null) Keyboard.Focus(ItemContainerGenerator.ContainerFromItem(SelectedItem) as IInputElement);
                Debug.WriteLine("FocusedElement - " + Keyboard.FocusedElement);
                e.Handled = true;
                return;
            }
            else if ((e.Key == Key.Return) && (!IsEditable))
            {
                SelectedIndex = ItemContainerGenerator.IndexFromContainer(Keyboard.FocusedElement as DependencyObject);
            }
            base.OnKeyDown(e);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
            if (IsEditable) Focus();
            else IsDropDownOpen = true;
        }

        #endregion

        #region Protected


        #endregion

        #region Private methods

        double minimalGallerylWidth;

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(scrollViewer.Height)) scrollViewer.Height = scrollViewer.ActualHeight;
            scrollViewer.Height = Math.Min(Math.Max(galleryPanel.GetItemSize().Height, scrollViewer.Height + e.VerticalChange), MaxDropDownHeight);

            menuPanel.Width = Double.NaN;
            if (Double.IsNaN(galleryPanel.Width)) galleryPanel.Width = 500;
            galleryPanel.Width = Math.Max(galleryPanel.Width + e.HorizontalChange, minimalGallerylWidth);

        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(scrollViewer.Height)) scrollViewer.Height = scrollViewer.ActualHeight;
            scrollViewer.Height = Math.Min(Math.Max(galleryPanel.GetItemSize().Height, scrollViewer.Height + e.VerticalChange), MaxDropDownHeight);
        }

        #endregion
    }
}
