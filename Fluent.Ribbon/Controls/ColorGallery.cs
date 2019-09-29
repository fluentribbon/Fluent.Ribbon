// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media;
    using ControlzEx.Native;
    using ControlzEx.Standard;
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents color gallery modes
    /// </summary>
    public enum ColorGalleryMode
    {
        /// <summary>
        /// Color gallery displays only fixed highlight colors
        /// </summary>
        HighlightColors = 0,

        /// <summary>
        /// Color gallery displays only fixed standart colors
        /// </summary>
        StandardColors,

        /// <summary>
        /// Color gallery displays theme colors
        /// </summary>
        ThemeColors
    }

    /// <summary>
    /// Date template selector for gradients
    /// </summary>
    public class ColorGradientItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        /// <param name="item">The data object for which to select the template.</param><param name="container">The data-bound object.</param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return null;
            }

            ListBox listBox = null;
            var parent = container;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                listBox = parent as ListBox;
                if (listBox != null)
                {
                    break;
                }
            }

            if (listBox == null)
            {
                return null;
            }

            ColorGallery colorGallery = null;
            while (parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
                colorGallery = parent as ColorGallery;
                if (colorGallery != null)
                {
                    break;
                }
            }

            if (colorGallery == null)
            {
                return null;
            }

            var index = listBox.Items.IndexOf(item);
            if (index < colorGallery.Columns)
            {
                return listBox.TryFindResource("GradientColorTopDataTemplate") as DataTemplate;
            }

            if (index >= listBox.Items.Count - colorGallery.Columns)
            {
                return listBox.TryFindResource("GradientColorBottomDataTemplate") as DataTemplate;
            }

            return listBox.TryFindResource("GradientColorCenterDataTemplate") as DataTemplate;
        }
    }

    /// <summary>
    /// More colors event args
    /// </summary>
    public class MoreColorsExecutingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets choosed color
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether more colors is canceled
        /// </summary>
        public bool Canceled { get; set; }
    }

    /// <summary>
    /// Represents color gallery
    /// </summary>
    [ContentProperty(nameof(ThemeColors))]
    [TemplatePart(Name = "PART_MoreColors", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_NoColor", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_AutomaticColor", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_ThemeColorsListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_ThemeGradientColorsListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_StandardColorsListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_StandardGradientColorsListBox", Type = typeof(ListBox))]
    [TemplatePart(Name = "PART_RecentColorsListBox", Type = typeof(ListBox))]
    public class ColorGallery : Control
    {
        #region Constants

        /// <summary>
        /// Hightlight colors array
        /// </summary>
        public static readonly Color[] HighlightColors = new Color[]
        {
            Color.FromRgb(0xFF, 0xFF, 0x00),
            Color.FromRgb(0x00, 0xFF, 0x00),
            Color.FromRgb(0x00, 0xFF, 0xFF),

            Color.FromRgb(0xFF, 0x00, 0xFF),
            Color.FromRgb(0x00, 0x00, 0xFF),
            Color.FromRgb(0xFF, 0x00, 0x00),

            Color.FromRgb(0x00, 0x00, 0x80),
            Color.FromRgb(0x00, 0x80, 0x80),
            Color.FromRgb(0x00, 0x80, 0x00),

            Color.FromRgb(0x80, 0x00, 0x80),
            Color.FromRgb(0x80, 0x00, 0x00),
            Color.FromRgb(0x80, 0x80, 0x00),

            Color.FromRgb(0x80, 0x80, 0x80),
            Color.FromRgb(0xC0, 0xC0, 0xC0),
            Color.FromRgb(0x00, 0x00, 0x00),
        };

        /// <summary>
        /// Standard colors array
        /// </summary>
        public static readonly Color[] StandardColors = new Color[]
        {
              Color.FromRgb(0xFF, 0xFF, 0xFF),
              Color.FromRgb(0xFF, 0x00, 0x00),
              Color.FromRgb(0xC0, 0x50, 0x4D),
              Color.FromRgb(0xD1, 0x63, 0x49),
              Color.FromRgb(0xDD, 0x84, 0x84),

              Color.FromRgb(0xCC, 0xCC, 0xCC),
              Color.FromRgb(0xFF, 0xC0, 0x00),
              Color.FromRgb(0xF7, 0x96, 0x46),
              Color.FromRgb(0xD1, 0x90, 0x49),
              Color.FromRgb(0xF3, 0xA4, 0x47),

              Color.FromRgb(0xA5, 0xA5, 0xA5),
              Color.FromRgb(0xFF, 0xFF, 0x00),
              Color.FromRgb(0x9B, 0xBB, 0x59),
              Color.FromRgb(0xCC, 0xB4, 0x00),
              Color.FromRgb(0xDF, 0xCE, 0x04),

              Color.FromRgb(0x66, 0x66, 0x66),
              Color.FromRgb(0x00, 0xB0, 0x50),
              Color.FromRgb(0x4B, 0xAC, 0xC6),
              Color.FromRgb(0x8F, 0xB0, 0x8C),
              Color.FromRgb(0xA5, 0xB5, 0x92),

              Color.FromRgb(0x33, 0x33, 0x33),
              Color.FromRgb(0x00, 0x4D, 0xBB),
              Color.FromRgb(0x4F, 0x81, 0xBD),
              Color.FromRgb(0x64, 0x6B, 0x86),
              Color.FromRgb(0x80, 0x9E, 0xC2),

              Color.FromRgb(0x00, 0x00, 0x00),
              Color.FromRgb(0x9B, 0x00, 0xD3),
              Color.FromRgb(0x80, 0x64, 0xA2),
              Color.FromRgb(0x9E, 0x7C, 0x7C),
              Color.FromRgb(0x9C, 0x85, 0xC0),
        };

        /// <summary>
        /// Standard colors array in ThemeColor mode
        /// </summary>
        public static readonly Color[] StandardThemeColors = new Color[]
        {
            Color.FromRgb(0xC0, 0x00, 0x00),
            Color.FromRgb(0xFF, 0x00, 0x00),
            Color.FromRgb(0xFF, 0xC0, 0x00),
            Color.FromRgb(0xFF, 0xFF, 0x00),
            Color.FromRgb(0x92, 0xD0, 0x50),
            Color.FromRgb(0x00, 0xB0, 0x50),
            Color.FromRgb(0x00, 0xB0, 0xF0),
            Color.FromRgb(0x00, 0x70, 0xC0),
            Color.FromRgb(0x00, 0x20, 0x60),
            Color.FromRgb(0x70, 0x30, 0xA0),
        };

        #endregion

        #region RecentItems

        private static ObservableCollection<Color> recentColors;

        /// <summary>
        /// Gets recent colors collection
        /// </summary>
        public static ObservableCollection<Color> RecentColors
        {
            get { return recentColors ?? (recentColors = new ObservableCollection<Color>()); }
        }

        #endregion

        #region Fields

        private MenuItem noColorButton;
        private MenuItem automaticButton;

        private MenuItem moreColorsButton;

        private ListBox themeColorsListBox;
        private ListBox themeGradientsListBox;
        private ListBox standardColorsListBox;
        private ListBox standardGradientsListBox;
        private ListBox recentColorsListBox;

        private readonly List<ListBox> listBoxes = new List<ListBox>();

        private bool isSelectionChanging;

        private bool isTemplateApplied;

        #endregion

        #region Properties

        #region Mode

        /// <summary>
        /// Gets or sets color gallery mode
        /// </summary>
        public ColorGalleryMode Mode
        {
            get { return (ColorGalleryMode)this.GetValue(ModeProperty); }
            set { this.SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(nameof(Mode), typeof(ColorGalleryMode), typeof(ColorGallery), new PropertyMetadata(ColorGalleryMode.StandardColors, OnModeChanged));

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorGallery)d).UpdateGradients();
        }

        #endregion

        #region Chip Size

        /// <summary>
        /// Gets or sets chip width
        /// </summary>
        public double ChipWidth
        {
            get { return (double)this.GetValue(ChipWidthProperty); }
            set { this.SetValue(ChipWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ChipWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ChipWidthProperty =
            DependencyProperty.Register(nameof(ChipWidth), typeof(double), typeof(ColorGallery), new PropertyMetadata(13.0, null, CoerceChipSize));

        private static object CoerceChipSize(DependencyObject d, object basevalue)
        {
            var value = (double)basevalue;
            if (value < 0)
            {
                return 0;
            }

            return basevalue;
        }

        /// <summary>
        /// Gets or sets chip height
        /// </summary>
        public double ChipHeight
        {
            get { return (double)this.GetValue(ChipHeightProperty); }
            set { this.SetValue(ChipHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ChipHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ChipHeightProperty =
            DependencyProperty.Register(nameof(ChipHeight), typeof(double), typeof(ColorGallery), new PropertyMetadata(13.0, null, CoerceChipSize));

        #endregion

        #region IsAutomaticColorButtonVisible

        /// <summary>
        /// Gets or sets a value indicating whether Automatic button is visible
        /// </summary>
        public bool IsAutomaticColorButtonVisible
        {
            get { return (bool)this.GetValue(IsAutomaticColorButtonVisibleProperty); }
            set { this.SetValue(IsAutomaticColorButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsAutomaticColorButtonVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAutomaticColorButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsAutomaticColorButtonVisible), typeof(bool), typeof(ColorGallery), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region IsNoColorButtonVisible

        /// <summary>
        /// Gets or sets a value indicating whether No color button is visible
        /// </summary>
        public bool IsNoColorButtonVisible
        {
            get { return (bool)this.GetValue(IsNoColorButtonVisibleProperty); }
            set { this.SetValue(IsNoColorButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsNoColorButtonVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsNoColorButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsNoColorButtonVisible), typeof(bool), typeof(ColorGallery), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region IsMoreColorsButtonVisible

        /// <summary>
        /// Gets or sets a value indicating whether More Colors button is visible
        /// </summary>
        public bool IsMoreColorsButtonVisible
        {
            get { return (bool)this.GetValue(IsMoreColorsButtonVisibleProperty); }
            set { this.SetValue(IsMoreColorsButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMoreColorsButtonVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsMoreColorsButtonVisibleProperty =
            DependencyProperty.Register(nameof(IsMoreColorsButtonVisible), typeof(bool), typeof(ColorGallery), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #region Columns

        /// <summary>
        /// Gets or sets number of color gallery columns. It works only when Mode is ThemeColors
        /// </summary>
        public int Columns
        {
            get { return (int)this.GetValue(ColumnsProperty); }
            set { this.SetValue(ColumnsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(nameof(Columns), typeof(int), typeof(ColorGallery), new PropertyMetadata(10, OnColumnsChanged, CoerceColumns));

        private static object CoerceColumns(DependencyObject d, object basevalue)
        {
            var value = (int)basevalue;
            if (value < 1)
            {
                return 1;
            }

            return basevalue;
        }

        private static void OnColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorGallery)d).UpdateGradients();
        }

        #endregion

        #region StandardColorGridRows

        /// <summary>
        /// Gets or set number of standard color rows. Work only when Mode is ThemeColors
        /// </summary>
        public int StandardColorGridRows
        {
            get { return (int)this.GetValue(StandardColorGridRowsProperty); }
            set { this.SetValue(StandardColorGridRowsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StandardColorGridRows.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StandardColorGridRowsProperty =
            DependencyProperty.Register(nameof(StandardColorGridRows), typeof(int), typeof(ColorGallery), new PropertyMetadata(IntBoxes.Zero, OnStandardColorGridRowsChanged, CoeceGridRows));

        private static object CoeceGridRows(DependencyObject d, object basevalue)
        {
            var value = (int)basevalue;
            if (value < 0)
            {
                return 0;
            }

            return basevalue;
        }

        private static void OnStandardColorGridRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorGallery)d).UpdateGradients();
        }

        #endregion

        #region ThemeColorGridRows

        /// <summary>
        /// Gets or set number of theme color rows. Work only when Mode is ThemeColors
        /// </summary>
        public int ThemeColorGridRows
        {
            get { return (int)this.GetValue(ThemeColorGridRowsProperty); }
            set { this.SetValue(ThemeColorGridRowsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ThemeColorGridRows.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ThemeColorGridRowsProperty =
            DependencyProperty.Register(nameof(ThemeColorGridRows), typeof(int), typeof(ColorGallery), new PropertyMetadata(IntBoxes.Zero, OnThemeColorGridRowsChanged, CoeceGridRows));

        private static void OnThemeColorGridRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorGallery)d).UpdateGradients();
        }

        #endregion

        #region SelectedColor

        /// <summary>
        /// Gets or sets selected color
        /// </summary>
        public Color? SelectedColor
        {
            get { return (Color?)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color?), typeof(ColorGallery), new PropertyMetadata(OnSelectedColorChanged));

        // Handles selected color changed
        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gallery = d as ColorGallery;

            if (gallery == null)
            {
                return;
            }

            // Set color in gallery
            var color = (Color?)e.NewValue;
            gallery.UpdateSelectedColor(color);

            // Raise event
            gallery.RunInDispatcherAsync(() => gallery.RaiseEvent(new RoutedEventArgs(SelectedColorChangedEvent)));
        }

        private void UpdateSelectedColor(Color? color)
        {
            if (this.isSelectionChanging
                || this.isTemplateApplied == false)
            {
                return;
            }

            this.isSelectionChanging = true;
            var isSetted = false;

            // Check menu items
            if (color.HasValue == false)
            {
                isSetted = true;
                this.automaticButton.IsChecked = true;
                this.noColorButton.IsChecked = false;
            }
            else if (color.Value == Colors.Transparent)
            {
                isSetted = true;
                this.automaticButton.IsChecked = false;
                this.noColorButton.IsChecked = true;
            }
            else
            {
                this.automaticButton.IsChecked = false;
                this.noColorButton.IsChecked = false;
            }

            // Remove selection from others
            foreach (var listBox in this.listBoxes)
            {
                if (isSetted == false
                    && listBox.Visibility == Visibility.Visible)
                {
                    if (listBox.Items.Contains(color.Value))
                    {
                        listBox.SelectedItem = color.Value;
                        isSetted = true;
                    }
                }
                else
                {
                    listBox.SelectedItem = null;
                }
            }

            this.isSelectionChanging = false;
        }

        #endregion

        #region ThemeColors

        private ObservableCollection<Color> themeColors;

        /// <summary>
        /// Gets collection of theme colors
        /// </summary>
        public ObservableCollection<Color> ThemeColors
        {
            get
            {
                if (this.themeColors == null)
                {
                    this.themeColors = new ObservableCollection<Color>();
                    this.themeColors.CollectionChanged += this.OnThemeColorsChanged;
                }

                return this.themeColors;
            }
        }

        private void OnThemeColorsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateGradients();
        }

        /// <summary>
        /// Gets or sets theme colors source
        /// </summary>
        public IEnumerable<Color> ThemeColorsSource
        {
            get { return (IEnumerable<Color>)this.GetValue(ThemeColorsSourceProperty); }
            set { this.SetValue(ThemeColorsSourceProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ThemeColorsSource.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ThemeColorsSourceProperty =
            DependencyProperty.Register(nameof(ThemeColorsSource), typeof(IEnumerable<Color>), typeof(ColorGallery), new PropertyMetadata(OnThemeColorsSourceChanged));

        private static void OnThemeColorsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gal = (ColorGallery)d;
            gal.ThemeColors.Clear();

            if (e.NewValue is IEnumerable<Color> colors)
            {
                foreach (var color in colors)
                {
                    gal.ThemeColors.Add(color);
                }
            }
        }

        #endregion

        #region ThemeGradients

        /// <summary>
        /// Gets theme gradients collection
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Values get regenerated.")]
        public Color[] ThemeGradients
        {
            get { return (Color[])this.GetValue(ThemeGradientsProperty); }
            private set { this.SetValue(ThemeGradientsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey ThemeGradientsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ThemeGradients), typeof(Color[]), typeof(ColorGallery), new PropertyMetadata());

        /// <summary>
        /// Using a DependencyProperty as the backing store for ThemeGradients.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ThemeGradientsProperty = ThemeGradientsPropertyKey.DependencyProperty;

        #endregion

        #region StandardGradients

        /// <summary>
        /// Gets standart gradients collection
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Values get regenerated.")]
        public Color[] StandardGradients
        {
            get { return (Color[])this.GetValue(StandardGradientsProperty); }
            private set { this.SetValue(StandardGradientsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey StandardGradientsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(StandardGradients), typeof(Color[]), typeof(ColorGallery), new PropertyMetadata());

        /// <summary>
        /// Using a DependencyProperty as the backing store for ThemeGradients.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StandardGradientsProperty = StandardGradientsPropertyKey.DependencyProperty;

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when selection color is changed
        /// </summary>
        public event RoutedEventHandler SelectedColorChanged
        {
            add
            {
                this.AddHandler(SelectedColorChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(SelectedColorChangedEvent, value);
            }
        }

        /// <summary>
        /// Identifies the SelectedColorChanged routed event.
        /// </summary>
        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectedColorChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorGallery));

        /// <summary>
        /// Occurs whether more colors menu item is clicked
        /// </summary>
        public event EventHandler<MoreColorsExecutingEventArgs> MoreColorsExecuting;

        #endregion

        #region Initializing

        /// <summary>
        /// Static constructor
        /// </summary>
        static ColorGallery()
        {
            var type = typeof(ColorGallery);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            ContextMenuService.Attach(type);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.moreColorsButton != null)
            {
                this.moreColorsButton.Click += this.OnMoreColorsClick;
            }

            this.moreColorsButton = this.GetTemplateChild("PART_MoreColors") as MenuItem;
            if (this.moreColorsButton != null)
            {
                this.moreColorsButton.Click += this.OnMoreColorsClick;
            }

            if (this.noColorButton != null)
            {
                this.noColorButton.Click -= this.OnNoColorClick;
            }

            this.noColorButton = this.GetTemplateChild("PART_NoColor") as MenuItem;
            if (this.noColorButton != null)
            {
                this.noColorButton.Click += this.OnNoColorClick;
            }

            if (this.automaticButton != null)
            {
                this.automaticButton.Click -= this.OnAutomaticClick;
            }

            this.automaticButton = this.GetTemplateChild("PART_AutomaticColor") as MenuItem;
            if (this.automaticButton != null)
            {
                this.automaticButton.Click += this.OnAutomaticClick;
            }

            // List boxes
            this.listBoxes.Clear();

            if (this.themeColorsListBox != null)
            {
                this.themeColorsListBox.SelectionChanged -= this.OnListBoxSelectedChanged;
            }

            this.themeColorsListBox = this.GetTemplateChild("PART_ThemeColorsListBox") as ListBox;
            this.listBoxes.Add(this.themeColorsListBox);
            if (this.themeColorsListBox != null)
            {
                this.themeColorsListBox.SelectionChanged += this.OnListBoxSelectedChanged;
            }

            if (this.themeGradientsListBox != null)
            {
                this.themeGradientsListBox.SelectionChanged -= this.OnListBoxSelectedChanged;
            }

            this.themeGradientsListBox = this.GetTemplateChild("PART_ThemeGradientColorsListBox") as ListBox;
            this.listBoxes.Add(this.themeGradientsListBox);
            if (this.themeGradientsListBox != null)
            {
                this.themeGradientsListBox.SelectionChanged += this.OnListBoxSelectedChanged;
            }

            if (this.standardColorsListBox != null)
            {
                this.standardColorsListBox.SelectionChanged -= this.OnListBoxSelectedChanged;
            }

            this.standardColorsListBox = this.GetTemplateChild("PART_StandardColorsListBox") as ListBox;
            this.listBoxes.Add(this.standardColorsListBox);
            if (this.standardColorsListBox != null)
            {
                this.standardColorsListBox.SelectionChanged += this.OnListBoxSelectedChanged;
            }

            if (this.standardGradientsListBox != null)
            {
                this.standardGradientsListBox.SelectionChanged -= this.OnListBoxSelectedChanged;
            }

            this.standardGradientsListBox = this.GetTemplateChild("PART_StandardGradientColorsListBox") as ListBox;
            this.listBoxes.Add(this.standardGradientsListBox);
            if (this.standardGradientsListBox != null)
            {
                this.standardGradientsListBox.SelectionChanged += this.OnListBoxSelectedChanged;
            }

            if (this.recentColorsListBox != null)
            {
                this.recentColorsListBox.SelectionChanged -= this.OnListBoxSelectedChanged;
            }

            this.recentColorsListBox = this.GetTemplateChild("PART_RecentColorsListBox") as ListBox;
            this.listBoxes.Add(this.recentColorsListBox);
            if (this.recentColorsListBox != null)
            {
                this.recentColorsListBox.SelectionChanged += this.OnListBoxSelectedChanged;
            }

            this.isTemplateApplied = true;

            this.UpdateSelectedColor(this.SelectedColor);
        }

        #endregion

        #region Private Methods

        private static IntPtr customColors = IntPtr.Zero;
        private readonly int[] colorsArray = new int[16];

        private void OnMoreColorsClick(object sender, RoutedEventArgs e)
        {
            if (this.MoreColorsExecuting != null)
            {
                var args = new MoreColorsExecutingEventArgs();
                this.MoreColorsExecuting(this, args);
                if (!args.Canceled)
                {
                    var color = args.Color;
                    if (RecentColors.Contains(color))
                    {
                        RecentColors.Remove(color);
                    }

                    RecentColors.Insert(0, color);
                    this.recentColorsListBox.SelectedIndex = 0;
                }
            }
            else
            {
#pragma warning disable 618
                var chooseColor = new NativeMethods.CHOOSECOLOR();
                var wnd = Window.GetWindow(this);
                if (wnd != null)
                {
                    chooseColor.hwndOwner = new WindowInteropHelper(wnd).Handle;
                }

                chooseColor.Flags = Constants.CC_ANYCOLOR;
                if (customColors == IntPtr.Zero)
                {
                    // Set custom colors)
                    for (var i = 0; i < this.colorsArray.Length; i++)
                    {
                        this.colorsArray[i] = 0x00FFFFFF;
                    }

                    customColors = GCHandle.Alloc(this.colorsArray, GCHandleType.Pinned).AddrOfPinnedObject();
                }

                chooseColor.lpCustColors = customColors;
                if (NativeMethods.ChooseColor(chooseColor))
                {
                    var color = ConvertFromWin32Color(chooseColor.rgbResult);
                    if (RecentColors.Contains(color))
                    {
                        RecentColors.Remove(color);
                    }

                    RecentColors.Insert(0, color);
                    this.recentColorsListBox.SelectedIndex = 0;
                }
#pragma warning restore 618
            }
        }

        private static Color ConvertFromWin32Color(int color)
        {
            var r = color & 0x000000FF;
            var g = (color & 0x0000FF00) >> 8;
            var b = (color & 0x00FF0000) >> 16;
            return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
        }

        private void OnAutomaticClick(object sender, RoutedEventArgs e)
        {
            this.isSelectionChanging = true;
            this.noColorButton.IsChecked = false;
            this.automaticButton.IsChecked = true;
            // Remove selection from listboxes
            foreach (var listBox in this.listBoxes)
            {
                listBox.SelectedItem = null;
            }

            this.SelectedColor = null;
            this.isSelectionChanging = false;
        }

        private void OnNoColorClick(object sender, RoutedEventArgs e)
        {
            this.isSelectionChanging = true;
            this.noColorButton.IsChecked = true;
            this.automaticButton.IsChecked = false;
            // Remove selection from listboxes
            foreach (var listBox in this.listBoxes)
            {
                listBox.SelectedItem = null;
            }

            this.SelectedColor = Colors.Transparent;
            this.isSelectionChanging = false;
        }

        private void OnListBoxSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isSelectionChanging)
            {
                return;
            }

            this.isSelectionChanging = true;

            if (e.AddedItems != null
                && e.AddedItems.Count > 0)
            {
                // Remove selection from others
                this.noColorButton.IsChecked = false;
                this.automaticButton.IsChecked = false;

                foreach (var listBox in this.listBoxes)
                {
                    if (ReferenceEquals(listBox, sender) == false)
                    {
                        listBox.SelectedItem = null;
                    }
                }

                this.SelectedColor = (Color)e.AddedItems[0];
                PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
            }

            this.isSelectionChanging = false;
        }

        private void UpdateGradients()
        {
            if ((this.Mode == ColorGalleryMode.ThemeColors) && (this.Columns > 0))
            {
                if (this.ThemeColorGridRows > 0)
                {
                    this.ThemeGradients = this.GenerateThemeGradients();
                }
                else
                {
                    this.ThemeGradients = null;
                }

                if (this.StandardColorGridRows > 0)
                {
                    this.StandardGradients = this.GenerateStandardGradients();
                }
                else
                {
                    this.StandardGradients = null;
                }
            }
            else
            {
                this.StandardGradients = null;
                this.ThemeGradients = null;
            }
        }

        private Color[] GenerateStandardGradients()
        {
            var count = Math.Min(this.Columns, StandardThemeColors.Length);
            var result = new Color[this.Columns * this.StandardColorGridRows];
            for (var i = 0; i < count; i++)
            {
                var colors = GetGradient(StandardThemeColors[i], this.StandardColorGridRows);
                for (var j = 0; j < this.StandardColorGridRows; j++)
                {
                    result[i + (j * this.Columns)] = colors[j];
                }
            }

            return result;
        }

        private Color[] GenerateThemeGradients()
        {
            var count = Math.Min(this.Columns, this.ThemeColors.Count);
            var result = new Color[this.Columns * this.ThemeColorGridRows];
            for (var i = 0; i < count; i++)
            {
                var colors = GetGradient(this.ThemeColors[i], this.ThemeColorGridRows);
                for (var j = 0; j < this.ThemeColorGridRows; j++)
                {
                    result[i + (j * this.Columns)] = colors[j];
                }
            }

            return result;
        }

        #endregion

        #region Gradient Generation

        /// <summary>
        /// Returns brightness of the given color from 0..1
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Brightness of the given color from 0..1</returns>
        private static double GetBrightness(Color color)
        {
            var summ = (double)color.R + color.G + color.B;
            return summ / (255.0 * 3.0);
        }

        // Makes the given color lighter
        private static Color Lighter(Color color, double power)
        {
            var totalAvailability = (255.0 * 3.0) - color.R + color.G + color.B;
            double redAvailability;
            double greenAvailability;
            double blueAvailability;
            double needToBeAdded;

            if (color.R + color.G + color.B == 0)
            {
                redAvailability = 1.0 / 3.0;
                greenAvailability = 1.0 / 3.0;
                blueAvailability = 1.0 / 3.0;
                needToBeAdded = power * 255.0 * 3.0;
            }
            else
            {
                redAvailability = (255.0 - color.R) / totalAvailability;
                greenAvailability = (255.0 - color.G) / totalAvailability;
                blueAvailability = (255.0 - color.B) / totalAvailability;
                needToBeAdded = ((double)color.R + color.G + color.B) * (power - 1);
            }

            var result = Color.FromRgb(
                (byte)(color.R + (byte)(redAvailability * needToBeAdded)),
                (byte)(color.G + (byte)(greenAvailability * needToBeAdded)),
                (byte)(color.B + (byte)(blueAvailability * needToBeAdded)));

            return result;
        }

        // Makes the given color darker
        private static Color Darker(Color color, double power)
        {
            var totalAvailability = (double)color.R + color.G + color.B;
            var redAvailability = color.R / totalAvailability;
            var greenAvailability = color.G / totalAvailability;
            var blueAvailability = color.B / totalAvailability;

            var needToBeAdded = (double)color.R + color.G + color.B;
            needToBeAdded = needToBeAdded - (needToBeAdded * power);

            var result = Color.FromRgb(
                (byte)(color.R - (byte)(redAvailability * needToBeAdded)),
                (byte)(color.G - (byte)(greenAvailability * needToBeAdded)),
                (byte)(color.B - (byte)(blueAvailability * needToBeAdded)));

            return result;
        }

        // Makes a new color from the given with new brightness
        private static Color Rebright(Color color, double newBrightness)
        {
            var currentBrightness = GetBrightness(color);
            var power = DoubleUtil.AreClose(currentBrightness, 0.0) == false
                ? newBrightness / currentBrightness
                : 1.0 + newBrightness;

            // TODO: round power to make nice numbers
            // ...

            if (power > 1.0)
            {
                return Lighter(color, power);
            }

            return Darker(color, power);
        }

        /// <summary>
        /// Makes gradient colors from lighter to darker
        /// </summary>
        /// <param name="color">Base color</param>
        /// <param name="count">Count of items in the gradient</param>
        /// <returns>Colors from lighter to darker</returns>
        private static Color[] GetGradient(Color color, int count)
        {
            const double lowBrightness = 0.15;
            const double highBrightness = 0.85;
            var result = new Color[count];

            for (var i = 0; i < count; i++)
            {
                var brightness = lowBrightness + (i * (highBrightness - lowBrightness) / count);
                result[count - i - 1] = Rebright(color, brightness);
            }

            return result;
        }

        #endregion
    }
}