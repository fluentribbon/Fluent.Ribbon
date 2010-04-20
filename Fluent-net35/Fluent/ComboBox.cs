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
    public class ComboBox: RibbonItemsControl
    {
        #region Fields

        private ContextMenu contextMenu;
        private Gallery gallery = new Gallery();
        private TextBox textBox;

        private bool updatingText;
        
        private int textBoxSelectionStart;

        private bool isInitializing;

        private GalleryItem selectedGalleryItem;
        private Image fakeImage;

        // Is visual currently snapped
        private bool isSnapped;

        private ComboBox quickAccessCombo;

        #endregion

        #region Properties

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

        #region IsReadOnly

        /// <summary>
        /// Gets or sets a value that enables selection-only mode, in which the contents of the combo box are selectable but not editable. This is a dependency property.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsReadonly.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ComboBox), new UIPropertyMetadata(false));



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
                return isSnapped;
            }
            set
            {
                if (value == isSnapped) return;
                if (fakeImage == null) return;
                if (selectedGalleryItem == null) return;
                if (value)
                {
                    // Render the freezed image
                    RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)selectedGalleryItem.ActualWidth, (int)selectedGalleryItem.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                    renderTargetBitmap.Render((Visual)selectedGalleryItem);
                    fakeImage.Source = renderTargetBitmap;
                    selectedGalleryItem.Visibility = Visibility.Collapsed;
                    fakeImage.Visibility = Visibility.Visible;
                    fakeImage.FlowDirection = FlowDirection;
                }
                else
                {
                    fakeImage.Source = null;
                    selectedGalleryItem.Visibility = Visibility.Visible;
                    fakeImage.Visibility = Visibility.Collapsed;
                }
                isSnapped = value;
                InvalidateVisual();
            }
        }

        #endregion

        #region IsEditable

        /// <summary>
        /// gets or sets wthether cmbobox is editable
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(ComboBox), new UIPropertyMetadata(true));



        #endregion

        #region ScrollBarsVisibility

        /// <summary> 
        /// HorizonalScollbarVisibility is a Windows.Controls.ScrollBarVisibility that
        /// determines if a horizontal scrollbar is shown. 
        /// </summary> 
        [Bindable(true), Category("Appearance")]
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HorizontalScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register("HorizontalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ComboBox), new UIPropertyMetadata(ScrollBarVisibility.Disabled));

        /// <summary> 
        /// VerticalScrollBarVisibility is a System.Windows.Controls.ScrollBarVisibility that 
        /// determines if a vertical scrollbar is shown.
        /// </summary> 
        [Bindable(true), Category("Appearance")]
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VerticalScrollBarVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register("VerticalScrollBarVisibility", typeof(ScrollBarVisibility), typeof(ComboBox), new UIPropertyMetadata(ScrollBarVisibility.Visible));

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

        #region Orientation

        /// <summary>
        /// Gets or sets orientation of gallery
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ComboBox), new UIPropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox) d;
            if (comboBox.gallery != null)
            {
                if ((Orientation)e.NewValue == Orientation.Horizontal)
                {
                    ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
                    template.Seal();
                    comboBox.ItemsPanel = template;
                }
                else
                {
                    ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
                    template.Seal();
                    comboBox.ItemsPanel = template;
                }
            }
        }

        #endregion

        #region SelectedIndex

        /// <summary>
        /// Gets or sets index of currently selected item
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedIndex. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ComboBox), new UIPropertyMetadata(-1, OnSelectedIndexChenged, CoerceSelectedIndex));

        private static object CoerceSelectedIndex(DependencyObject d, object basevalue)
        {
            if (((int)basevalue != -1) && (((ComboBox)d).GetItem((int)basevalue) == null)) return -1;
            return basevalue;
        }

        static void OnSelectedIndexChenged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)d;            
            if (((int)e.NewValue == -1) && (comboBox.SelectedItem != null)) comboBox.SelectedItem = null;
            else
            {
                object selectedItem = comboBox.GetItem((int) e.NewValue);
                if ((selectedItem != comboBox.SelectedItem)&&(selectedItem!=null))
                {
                    comboBox.SelectedItem = selectedItem;
                    comboBox.CurrentText = comboBox.GetItemText(comboBox.SelectedItem);
                }
            }
        }

        #endregion

        #region SelectedItem

        /// <summary>
        /// Gets or sets currently selected item
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedItem.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboBox), new UIPropertyMetadata(null, OnSelectedItemChanged,CoerceSelectedItem));

        private static object CoerceSelectedItem(DependencyObject d, object basevalue)
        {
            if ((basevalue != null) && (((ComboBox)d).GetItemIndex(basevalue) == -1)) return null;
            return basevalue;
        }

        static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            ((ComboBox)d).SelectedItemUpdated();
        }

        internal void SelectedItemUpdated()
        {
            if (!updatingText)
            {
                string primaryTextFromItem = GetItemText(SelectedItem);
                if ((this.CurrentText != primaryTextFromItem) && (primaryTextFromItem != null))
                {
                    this.CurrentText = primaryTextFromItem;
                }
            }
            if (selectedGalleryItem != null)
            {
                selectedGalleryItem.Content = SelectedItem;
                if ((quickAccessCombo != null) && (selectedGalleryItem.Visibility == Visibility.Visible))
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                           {
                               RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)selectedGalleryItem.ActualWidth, (int)selectedGalleryItem.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                               renderTargetBitmap.Render((Visual)selectedGalleryItem);
                               quickAccessCombo.fakeImage.Source = renderTargetBitmap;
                           }));
                }
            }
            int selectedIndex = GetItemIndex(SelectedItem);
            if((selectedIndex!=-1)&&(SelectedItem!=null)) if (selectedIndex != SelectedIndex) SelectedIndex = selectedIndex;
        }


        #endregion

        #region IsOpen

        /// <summary>
        /// Gets or sets whether context menu of the ComboBox is open
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), 
            typeof(ComboBox), new UIPropertyMetadata(false, OnIsOpenChanged, CoerceIsOpen));

        // Coerce IsOpen
        static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            if (((ComboBox)d).isInitializing) return true;
            return basevalue;
        }

        static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox) d;
            if (comboBox.contextMenu == null) comboBox.CreateMenu();
            if (comboBox.IsOpen) comboBox.IsHitTestVisible = false;
            else comboBox.IsHitTestVisible = true;                     
        }

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

        #region CurrentText

        /// <summary>
        /// Gets or sets text in ComboBox
        /// </summary>
        public string CurrentText
        {
            get { return (string)GetValue(CurrentTextProperty); }
            set { SetValue(CurrentTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentTextProperty =
            DependencyProperty.Register("CurrentText", typeof(string), typeof(ComboBox), new UIPropertyMetadata("",OnCurrentTextChanged));

        private static void OnCurrentTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ComboBox).TextUpdated(e.NewValue as string,false);
        }

        #endregion

        #region MenuMinWidth

        /// <summary>
        /// Gets or sets minimal width of dropdown menu
        /// </summary>
        public double MenuMinWidth
        {
            get { return (double)GetValue(MenuMinWidthProperty); }
            set { SetValue(MenuMinWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MenuMinWidth. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MenuMinWidthProperty =
            DependencyProperty.Register("MenuMinWidth", typeof(double), typeof(ComboBox), new UIPropertyMetadata(0.0));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occures when context menu is opened
        /// </summary>
        public event EventHandler Opened;

        /// <summary>
        /// Occures when context menu is closed
        /// </summary>
        public event EventHandler Closed;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ComboBox), new FrameworkPropertyMetadata(typeof(ComboBox)));
            ItemWidthProperty.OverrideMetadata(typeof(ComboBox), new UIPropertyMetadata(Double.NaN));
            ItemHeightProperty.OverrideMetadata(typeof(ComboBox), new UIPropertyMetadata(22.0));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ComboBox()
        {
            Binding binding = new Binding("DisplayMemberPath");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.DisplayMemberPathProperty, binding);
            binding = new Binding("ItemBindingGroup");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemBindingGroupProperty, binding);
            binding = new Binding("ItemContainerStyle");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemContainerStyleProperty, binding);
            binding = new Binding("ItemContainerStyleSelector");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemContainerStyleSelectorProperty, binding);
            binding = new Binding("ItemsPanel");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemsPanelProperty, binding);
            binding = new Binding("ItemStringFormat");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemStringFormatProperty, binding);
            binding = new Binding("ItemTemplate");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemTemplateProperty, binding);
            binding = new Binding("ItemTemplateSelector");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemTemplateSelectorProperty, binding);
            binding = new Binding("ItemWidth");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemWidthProperty, binding);
            binding = new Binding("ItemHeight");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.ItemHeightProperty, binding);
            binding = new Binding("IsTextSearchEnabled");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.IsTextSearchEnabledProperty, binding);

            binding = new Binding("VerticalScrollBarVisibility");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.VerticalScrollBarVisibilityProperty, binding);
            binding = new Binding("HorizontalScrollBarVisibility");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.HorizontalScrollBarVisibilityProperty, binding);

            binding = new Binding("GroupBy");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.GroupByProperty, binding);

            /*binding = new Binding("Orientation");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            gallery.SetBinding(Gallery.OrientationProperty, binding);*/
            gallery.Orientation = Orientation;

            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;

            /*
            binding = new Binding("SelectedIndex");
            binding.Source = gallery;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.SetBinding(SelectedIndexProperty, binding);

            binding = new Binding("SelectedItem");
            binding.Source = gallery;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.SetBinding(SelectedItemProperty, binding); */

            Loaded += delegate
                          {
                              SelectedItemUpdated();
                          };
        }

        /// <summary>
        /// Handles click
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClick(RoutedEventArgs args)
        {
            if (textBox != null)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                (ThreadStart)(() =>
                                {
                                    textBox.SelectAll();
                                    textBox.Focus();
                                }));
            }
            args.Handled = true;
            base.OnClick(args);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever 
        /// application code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (textBox != null)
            {
                textBox.TextChanged -= OnTextBoxTextChanged;
                textBox.PreviewKeyDown -= OnTextBoxPreviewKeyDown;
                textBox.SelectionChanged -= OnTextBoxSelectionChanged;
            }
            textBox = GetTemplateChild("PART_TextBox") as TextBox;
            if(textBox!=null)
            {
                textBox.TextChanged += OnTextBoxTextChanged;
                textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;
                textBox.SelectionChanged += OnTextBoxSelectionChanged;
                textBox.Text = CurrentText;
            }

            selectedGalleryItem = GetTemplateChild("PART_GalleryItem") as GalleryItem;
            fakeImage = GetTemplateChild("PART_FakeImage") as Image;
        }

        void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(textBox!=null)textBoxSelectionStart = textBox.SelectionStart;
        }

        void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextUpdated(textBox.Text, true);
        }

        void TextUpdated(string newText, bool textBoxUpdated)
        {
            if (!updatingText)
            {
                try
                {
                    updatingText = true;
                    if (IsTextSearchEnabled)
                    {
                        int num = FindMatchingPrefix(newText);
                        if (num >= 0)
                        {
                            if (textBoxUpdated)
                            {
                                int selectionStart = textBox.SelectionStart;
                                if ((selectionStart == newText.Length) && (selectionStart > textBoxSelectionStart))
                                {
                                    string primaryTextFromItem = GetItemText(GetItem(num));
                                    textBox.Text = primaryTextFromItem;
                                    textBox.SelectionStart = newText.Length;
                                    textBox.SelectionLength = primaryTextFromItem.Length - newText.Length;
                                    newText = primaryTextFromItem;
                                }
                            }
                            else
                            {
                                string b = GetItemText(GetItem(num));
                                if (!string.Equals(newText, b, StringComparison.CurrentCulture))
                                {
                                    num = -1;
                                }
                            }
                        }
                        if (num != SelectedIndex)
                        {
                            SelectedIndex = num;                            
                        }
                    }
                    if (textBoxUpdated)
                    {
                        CurrentText = newText;
                    }
                    else if (textBox != null)
                    {
                        textBox.Text = newText;
                    }
                }
                finally
                {
                    updatingText = false;
                }
            }
        }


        int FindMatchingPrefix(string prefix)
        {
            IEnumerable items = GetItems();
            if (items == null) return -1;
            int i = 0;
            foreach (var item in items)
            {
                if (GetItemText(item).StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
                {                    
                    return i;
                }
                i++;
            }
            return -1;
        }

        IEnumerable GetItems()
        {
            IEnumerable items = ItemsSource;
            if (items == null) items = Items;
            return items;
        }

        object GetItem(int index)
        {
            IEnumerable items = GetItems();
            if (items == null) return null;
            int i = 0;
            foreach (var item in items)
            {
                if(i==index) return item;
                i++;
            }
            return null;
        }

        int GetItemIndex(object obj)
        {
            IEnumerable items = GetItems();
            if (items == null) return -1;
            int i = 0;
            foreach (var item in items)
            {
                if (item.Equals(obj)) return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Gets a text representation of the given item
        /// </summary>
        /// <param name="obj">Item</param>
        /// <returns>Text</returns>
        internal string GetItemText(object obj)
        {
            if(obj==null) return "";
            if (!string.IsNullOrEmpty(DisplayMemberPath)) return obj.GetType().GetProperty(DisplayMemberPath, BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null).ToString();
            return obj.ToString();
        }

        /// <summary>
        /// Items collection changes hadling 
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnItemsCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CoerceValue(ComboBox.SelectedItemProperty);
            CoerceValue(ComboBox.SelectedIndexProperty);
            base.OnItemsCollectionChanged(e);
            if ((SelectedItem == null) && (SelectedIndex >= 0))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { SelectedItem = GetItem(SelectedIndex); }));
            }
            else if ((SelectedItem != null) && (SelectedIndex == 1))
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate { SelectedIndex = GetItemIndex(SelectedItem); }));
            }
        }

        /// <summary>
        /// ItemsSource property change handling
        /// </summary>
        /// <param name="args"></param>
        protected override void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
        {            
            base.OnItemsSourceChanged(args);
            CoerceValue(ComboBox.SelectedItemProperty);
            CoerceValue(ComboBox.SelectedIndexProperty);
            if((SelectedItem==null)&&(SelectedIndex>=0))
            {
                SelectedItem = GetItem(SelectedIndex);
            }
            else if ((SelectedItem != null) && (SelectedIndex == -1 ))
            {
                SelectedIndex = GetItemIndex(SelectedItem);
            }
        }

        
        #endregion

        #region Private methods

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event is raised on this element. 
        /// Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            
            IsOpen = true;
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonUp routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add 
        /// class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was released</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if(Mouse.Captured==this)Mouse.Capture(null);
        }

        void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter) || (e.Key == Key.Escape))
            {
                // Move Focus
                textBox.Focusable = false;
                Focus();
                textBox.Focusable = true;
            }
            if(e.Key==Key.Down) IsOpen = true;
            base.OnPreviewKeyDown(e);
        }

        void CreateMenu()
        {
            isInitializing = true;
            contextMenu = new ContextMenu();
            contextMenu.Owner = this;
            contextMenu.Items.Add(gallery);
            AddLogicalChild(contextMenu.RibbonPopup);
            contextMenu.IsOpen = true;

            contextMenu.RibbonPopup.Opened += OnMenuOpened;
            contextMenu.RibbonPopup.Closed += OnMenuClosed;
            Binding binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.IsOpenProperty, binding);

            binding = new Binding("ResizeMode");
            binding.Mode = BindingMode.OneWay;
            binding.Source = this;
            contextMenu.SetBinding(Fluent.ContextMenu.ResizeModeProperty, binding);

            contextMenu.PlacementTarget = this;
            contextMenu.Placement = PlacementMode.Bottom;

            isInitializing = false;
            IsOpen = true;
            contextMenu.IsOpen = true;
        }

        private void OnMenuClosed(object sender, EventArgs e)
        {
            CloseMenu();
            if (Closed != null) Closed(this, e);
        }

        private void CloseMenu()
        {                        
            if (!IsEditable)
            {                
                selectedGalleryItem.Content = SelectedItem;
                IsSnapped = false;
            }
            SelectedIndex = gallery.SelectedIndex;
            SelectedItem = gallery.SelectedItem;
            gallery.ItemsSource = null;
        }

        private void OnMenuOpened(object sender, EventArgs e)
        {
            OpenMenu();
            if (Opened != null) Opened(this, e);
        }

        private void OpenMenu()
        {
            if (!IsEditable)
            {
                IsSnapped = true;
                selectedGalleryItem.Content = null;
            }
            int selectedIndex = SelectedIndex;
            object selectedItem = SelectedItem;
            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;
            //
            gallery.SelectedIndex = selectedIndex;
            gallery.SelectedItem = selectedItem;
            gallery.MinWidth = Math.Max(MenuMinWidth,ActualWidth);
            gallery.MinHeight = 2*ItemHeight;
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            ComboBox combo = new ComboBox();

            /*if (contextMenu == null)
            {
                CreateMenu();
                IsOpen = false;
            }*/

            BindQuickAccessItem(combo);
            combo.Opened += OnQuickAccesMenuOpened;
            combo.Closed += OnQuickAccesMenuClosed;
            
            if (!IsEditable)
            {
                       
                combo.Loaded += OnFirstComboLoaded;
            }
            else
            {
                combo.GotKeyboardFocus += OnQuickAccesGotKeyboardFocus;
                combo.LostKeyboardFocus -= OnQuickAccesLostKeyboardFocus;
            }
            quickAccessCombo = combo;
            return combo;
        }

        private void OnQuickAccesLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            quickAccessCombo.ItemsSource = null;
        }

        private void OnQuickAccesGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (ItemsSource == null) quickAccessCombo.ItemsSource = Items;
            else quickAccessCombo.ItemsSource = ItemsSource;
        }

        private void OnFirstComboLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new ThreadStart(delegate
                   {
                       ComboBox combo = (sender as ComboBox);
                       combo.Loaded -= OnFirstComboLoaded;
                       int selectedIndex = SelectedIndex;
                       object selectedItem = SelectedItem;
                       IsSnapped = true;
                       SelectedItem = null;
                       if (ItemsSource == null) combo.ItemsSource = Items;
                       else combo.ItemsSource = ItemsSource;
                       combo.SelectedIndex = selectedIndex;
                       combo.SelectedItem = selectedItem;     
                       /*int selectedIndex = combo.SelectedIndex;
                       object selectedItem = combo.SelectedItem;*/
                       combo.IsSnapped = true;
                       combo.ItemsSource = null;
                       IsSnapped = false;
                       combo.SelectedItem = null;
                       SelectedIndex = selectedIndex;
                       SelectedItem = selectedItem;
                   }));
        }

        private void OnQuickAccesMenuOpened(object sender, EventArgs e)
        {
            ComboBox combo = (sender as ComboBox);
            if (!IsEditable)
            {
                IsSnapped = true;
            }
            int selectedIndex = SelectedIndex;
            object selectedItem = SelectedItem;
            SelectedItem = null;
            if (ItemsSource == null) combo.ItemsSource = Items;
            else combo.ItemsSource = ItemsSource;
            combo.SelectedIndex = selectedIndex;
            combo.SelectedItem = selectedItem;
            combo.OpenMenu();
            if (!IsEditable)
            {
                combo.fakeImage.Source = fakeImage.Source;
            }
        }

        private void OnQuickAccesMenuClosed(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate
                   {
                       ComboBox combo = (sender as ComboBox);
                       int selectedIndex = combo.SelectedIndex;
                       object selectedItem = combo.SelectedItem;
                       if (!IsEditable)
                       {
                           combo.IsSnapped = true;
                       }
                       combo.ItemsSource = null;
                       if (!IsEditable)
                       {
                           IsSnapped = false;
                       }
                       SelectedIndex = selectedIndex;
                       SelectedItem = selectedItem;
                   }));
            /*IsSnapped = true;            
            combo.fakeImage.Source = fakeImage.Source;
            IsSnapped = false;*/
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            ComboBox comboBox = (ComboBox)element;

            comboBox.Width = Width;
            comboBox.InputWidth = InputWidth;

            Bind(this, comboBox, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            Bind(this, comboBox, "ItemBindingGroup", ItemBindingGroupProperty, BindingMode.Default);
            Bind(this, comboBox, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.Default);
            Bind(this, comboBox, "ItemContainerStyleSelector", ItemContainerStyleSelectorProperty, BindingMode.Default);
            Bind(this, comboBox, "ItemsPanel", ItemsPanelProperty, BindingMode.Default);
            Bind(this, comboBox, "ItemTemplate", ItemTemplateProperty, BindingMode.Default);
            Bind(this, comboBox, "ItemTemplateSelector", ItemTemplateSelectorProperty, BindingMode.Default);
            Bind(this, comboBox, "VerticalScrollBarVisibility", VerticalScrollBarVisibilityProperty, BindingMode.Default);
            Bind(this, comboBox, "HorizontalScrollBarVisibility", HorizontalScrollBarVisibilityProperty, BindingMode.Default);
            Bind(this, comboBox, "GroupBy", GroupByProperty, BindingMode.Default);
            Bind(this, comboBox, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.Default);
            Bind(this, comboBox, "IsTextSearchEnabled", IsTextSearchEnabledProperty, BindingMode.Default);
            Bind(this, comboBox, "CurrentText", CurrentTextProperty, BindingMode.TwoWay);
            Bind(this, comboBox, "MenuMinWidth", MenuMinWidthProperty, BindingMode.OneWay);
            Bind(this, comboBox, "IsEditable", IsEditableProperty, BindingMode.OneWay);
            Bind(this, comboBox, "IsReadOnly", IsReadOnlyProperty, BindingMode.OneWay);
            

            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
