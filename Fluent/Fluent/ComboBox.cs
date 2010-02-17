using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace Fluent
{
    public class ComboBox: RibbonItemsControl
    {
        #region Fields

        private ContextMenu contextMenu;
        private Gallery gallery = new Gallery();
        private TextBox textBox;

        private bool updatingText;
        private bool updatingSelectedItem;

        private int textBoxSelectionStart;

        private bool isInitializing;

        #endregion

        #region Properties

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

        public string GroupBy
        {
            get { return (string)GetValue(GroupByProperty); }
            set { SetValue(GroupByProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupBy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupByProperty =
            DependencyProperty.Register("GroupBy", typeof(string), typeof(ComboBox), new UIPropertyMetadata(null));

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
            if ((d as ComboBox).gallery != null)
            {
                //(d as InRibbonGallery).gallery.Orientation = (Orientation) e.NewValue;
                if ((Orientation)e.NewValue == Orientation.Horizontal)
                {
                    ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel)));
                    template.Seal();
                    (d as ComboBox).ItemsPanel = template;
                }
                else
                {
                    ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
                    template.Seal();
                    (d as ComboBox).ItemsPanel = template;
                }
            }
        }

        #endregion

        #region SelectedIndex

        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(ComboBox), new UIPropertyMetadata(-1, null));

        #endregion

        #region SelectedItem

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(ComboBox), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ComboBox).SelectedItemUpdated();
        }

        internal void SelectedItemUpdated()
        {
            try
            {
                updatingSelectedItem = true;
                if (!updatingText)
                {
                    string primaryTextFromItem = GetItemText(SelectedItem);
                    if ((this.CurrentText != primaryTextFromItem) && (primaryTextFromItem!=null))
                    {
                        this.CurrentText = primaryTextFromItem;
                    }
                }
            }
            finally
            {
                updatingSelectedItem = false;
            }
        }


        #endregion

        #region IsOpen



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(ComboBox), new UIPropertyMetadata(false, OnIsOpenChanged, CoerceIsOpen));

        // Coerce IsOpen
        private static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            if ((d as ComboBox).isInitializing) return true;
            return basevalue;
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((d as ComboBox).contextMenu == null) (d as ComboBox).CreateMenu();
            //if ((bool)e.NewValue) Keyboard.Focus((d as ComboBox).textBox);
                //FocusManager.SetFocusedElement((d as ComboBox).textBox, (d as ComboBox).textBox);//(d as ComboBox).textBox.Focus();
            if ((d as ComboBox).IsOpen) (d as ComboBox).IsHitTestVisible = false;
            else (d as ComboBox).IsHitTestVisible = true;                     
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

        // Using a DependencyProperty as the backing store for MenuMinWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MenuMinWidthProperty =
            DependencyProperty.Register("MenuMinWidth", typeof(double), typeof(ComboBox), new UIPropertyMetadata(0.0));

        #endregion

        #endregion

        #region Events

        public event EventHandler MenuOpened;
        public event EventHandler MenuClosed;

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
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
            FocusManager.SetIsFocusScope(this, false);
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

            gallery.SelectedIndex = SelectedIndex;

            binding = new Binding("SelectedIndex");
            binding.Source = gallery;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.SetBinding(SelectedIndexProperty, binding);

            binding = new Binding("SelectedItem");
            binding.Source = gallery;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            this.SetBinding(SelectedItemProperty, binding);

            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;

            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            //IsOpen = true;
            if (textBox != null)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                                (ThreadStart)(() =>
                                {
                                    textBox.SelectAll();
                                    textBox.Focus();
                                })); 
            }
            e.Handled = true;
        }

        #endregion

        #region Overrides

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
        }

        private void OnTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            if(textBox!=null)textBoxSelectionStart = textBox.SelectionStart;
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextUpdated(textBox.Text, true);

            /*if(!updatingText)
            {
                updatingText = true;                
                int index = FindMatchingPrefix(textBox.Text);                
                if ((index != -1) && (!string.IsNullOrEmpty(textBox.Text)))
                {
                    int length = textBox.Text.Length;
                    SelectedItem = null;
                    SelectedItem = gallery.GetItems()[index];
                    textBox.SelectionStart =length;
                    textBox.SelectionLength = textBox.Text.Length - length;
                }
                else SelectedItem = null;
                updatingText = false;
            }*/

           /* bool textBoxUpdated = true;
            string newText = textBox.Text;
            if (!updatingText)
            {
                try
                {
                    updatingText = true;
                    if ((base.IsTextSearchEnabled)&&(!string.IsNullOrEmpty(newText)))
                    {
                        int num = FindMatchingPrefix(newText);
                        if (num >= 0)
                        {
                            if (textBoxUpdated)
                            {
                                int selectionStart = textBox.SelectionStart;
                                if ((selectionStart == newText.Length)/* && (selectionStart > this._textBoxSelectionStart)*//*)
                                {
                                    string primaryTextFromItem = GetItemText(gallery.GetItems()[num]);
                                    textBox.Text = primaryTextFromItem;
                                    textBox.SelectionStart = newText.Length;
                                    textBox.SelectionLength = primaryTextFromItem.Length -
                                                                               newText.Length;
                                    newText = primaryTextFromItem;
                                }
                            }
                            else
                            {
                                string b = GetItemText(Items[num]);
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
                        this.Text = newText;
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
            }*/
        }

        private void TextUpdated(string newText, bool textBoxUpdated)
        {
            if (!updatingText/* && !updatingSelectedItem*/)
            {
                try
                {
                    updatingText = true;
                    if (base.IsTextSearchEnabled)
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


        private int FindMatchingPrefix(string prefix)
        {
            IEnumerable items = GetItems();
            if (items == null) return -1;
            int i = 0;
            foreach (var item in items)
            {
                if (GetItemText(item).ToLower().StartsWith(prefix.ToLower()))
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

        internal string GetItemText(object obj)
        {
            if(obj==null) return null;
            if (!string.IsNullOrEmpty(DisplayMemberPath)) return obj.GetType().GetProperty(DisplayMemberPath, BindingFlags.Public | BindingFlags.Instance).GetValue(obj, null).ToString();
            else return obj.ToString();
        }

        protected override void OnItemsCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {            
            base.OnItemsCollectionChanged(e);
            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;
        }

        protected override void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {            
            base.OnItemsSourceChanged(e);
            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;
        }

        
        #endregion

        #region Private methods

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Mouse.Capture(this);
            
            IsOpen = true;
            e.Handled = true;
        }

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

        private void CreateMenu()
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
            if (MenuClosed != null) MenuClosed(this, e);
        }

        private void OnMenuOpened(object sender, EventArgs e)
        {
            gallery.MinWidth = Math.Max(MenuMinWidth,ActualWidth);
            gallery.MinHeight = 2*ItemHeight;
            if (MenuOpened != null) MenuOpened(this, e);
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override UIElement CreateQuickAccessItem()
        {
            ComboBox combo = new ComboBox();
            BindQuickAccessItem(combo);
            combo.MenuOpened += OnQuickAccesMenuOpened;
            combo.MenuClosed += OnQuickAccesMenuClosed;
            return combo;
        }

        private void OnQuickAccesMenuOpened(object sender, EventArgs e)
        {
            int selectedIndex = gallery.SelectedIndex;
            gallery.ItemsSource = null;
            if (ItemsSource == null) (sender as ComboBox).ItemsSource = Items;
            else (sender as ComboBox).ItemsSource = ItemsSource;
            (sender as ComboBox).SelectedIndex = selectedIndex;
        }

        private void OnQuickAccesMenuClosed(object sender, EventArgs e)
        {
            int selectedIndex = (sender as ComboBox).SelectedIndex;
            (sender as ComboBox).ItemsSource = null;
            if (ItemsSource == null) gallery.ItemsSource = Items;
            else gallery.ItemsSource = ItemsSource;
            gallery.SelectedIndex = selectedIndex;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            ComboBox nox = element as ComboBox;
            Bind(this, nox, "Width", WidthProperty, BindingMode.Default);
            Bind(this, nox, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            Bind(this, nox, "ItemBindingGroup", ItemBindingGroupProperty, BindingMode.Default);
            Bind(this, nox, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.Default);
            Bind(this, nox, "ItemContainerStyleSelector", ItemContainerStyleSelectorProperty, BindingMode.Default);
            Bind(this, nox, "ItemsPanel", ItemsPanelProperty, BindingMode.Default);
            Bind(this, nox, "ItemTemplate", ItemTemplateProperty, BindingMode.Default);
            Bind(this, nox, "ItemTemplateSelector", ItemTemplateSelectorProperty, BindingMode.Default);
            Bind(this, nox, "VerticalScrollBarVisibility", VerticalScrollBarVisibilityProperty, BindingMode.Default);
            Bind(this, nox, "HorizontalScrollBarVisibility", HorizontalScrollBarVisibilityProperty, BindingMode.Default);
            Bind(this, nox, "GroupBy", GroupByProperty, BindingMode.Default);
            Bind(this, nox, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.Default);
            Bind(this, nox, "IsTextSearchEnabled", IsTextSearchEnabledProperty, BindingMode.Default);
            Bind(this, nox, "CurrentText", CurrentTextProperty, BindingMode.TwoWay);
            Bind(this, nox, "MenuMinWidth", MenuMinWidthProperty, BindingMode.OneWay);
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
