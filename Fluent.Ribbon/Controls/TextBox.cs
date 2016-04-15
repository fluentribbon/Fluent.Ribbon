using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents custom Fluent UI TextBox
    /// </summary>
    [TemplatePart(Name = "PART_TextBox")]
    [ContentProperty("Text")]
    public class TextBox : RibbonControl
    {
        #region Events

        /// <summary>
        /// Occurs when text is changed
        /// </summary>
        public event TextChangedEventHandler TextChanged;

        private void RaiseTextChanged(TextChangedEventArgs args)
        {
            if (this.TextChanged != null)
                this.TextChanged(this, args);
        }

        /// <summary>
        /// Occurs when selection changed
        /// </summary>
        public event EventHandler SelectionChanged;

        private void RaiseSelectionChanged()
        {
            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Fields

        // TextBox in template
        private System.Windows.Controls.TextBox textBoxTemplated;
        // Local TextBox
        private readonly System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
        // Content when the textbox got focus
        private string textBoxContentWhenGotFocus = null;

        #endregion

        #region Properties (Dependency)

        #region InputWidth

        /// <summary>
        /// Gets or sets width of the value input part of textbox
        /// </summary>               
        public double InputWidth
        {
            get { return (double)this.GetValue(InputWidthProperty); }
            set { this.SetValue(InputWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputWidth.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InputWidthProperty =
            DependencyProperty.Register("InputWidth", typeof(double), typeof(TextBox), new UIPropertyMetadata(double.NaN));

        #endregion

        #region Text

        /// <summary>
        /// Gets or sets text content of the textbox
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                null, null, true, UpdateSourceTrigger.LostFocus));

        #endregion

        #region IsReadOnly

        /// <summary>
        /// Gets or sets whether text can be edited. This is a dependency property.
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)this.GetValue(IsReadOnlyProperty); }
            set { this.SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsReadonly.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(TextBox), new UIPropertyMetadata(false));

        #endregion

        #region CharacterCasing

        /// <summary>
        /// Gets or sets how characters are cased
        /// </summary>
        public CharacterCasing CharacterCasing
        {
            get { return (CharacterCasing)this.GetValue(CharacterCasingProperty); }
            set { this.SetValue(CharacterCasingProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CharacterCasing.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CharacterCasingProperty =
            DependencyProperty.Register("CharacterCasing", typeof(CharacterCasing), typeof(TextBox),
            new UIPropertyMetadata(CharacterCasing.Normal));

        #endregion

        #region MaxLength

        /// <summary>
        /// Gets or sets how many characters can be entered manually into the textbox
        /// </summary>
        public int MaxLength
        {
            get { return (int)this.GetValue(MaxLengthProperty); }
            set { this.SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxLength. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBox), new UIPropertyMetadata(int.MaxValue));

        #endregion

        #region TextAlignment

        /// <summary>
        /// Gets or sets horizontal text alignment of the content
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)this.GetValue(TextAlignmentProperty); }
            set { this.SetValue(TextAlignmentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TextAlignment.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(TextBox),
            new UIPropertyMetadata(TextAlignment.Left));

        #endregion

        #region TextDecorations

        /// <summary>
        /// Gets or sets the text decorations to apply to the text box
        /// </summary>
        public TextDecorationCollection TextDecorations
        {
            get { return (TextDecorationCollection)this.GetValue(TextDecorationsProperty); }
            set { this.SetValue(TextDecorationsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TextDecorations.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextDecorationsProperty =
            DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(TextBox),
            new UIPropertyMetadata(new TextDecorationCollection()));

        #endregion

        #region IsUndoEnabled

        /// <summary>
        /// Gets or sets a value that indicates whether undo support is enabled
        /// </summary>
        public bool IsUndoEnabled
        {
            get { return (bool)this.GetValue(IsUndoEnabledProperty); }
            set { this.SetValue(IsUndoEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsUndoEnabled.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsUndoEnabledProperty =
            DependencyProperty.Register("IsUndoEnabled", typeof(bool), typeof(TextBox),
            new UIPropertyMetadata(true));

        #endregion

        #region UndoLimit

        /// <summary>
        /// Gets or sets the number of actions stored in undo queue
        /// </summary>
        public int UndoLimit
        {
            get { return (int)this.GetValue(UndoLimitProperty); }
            set { this.SetValue(UndoLimitProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for UndoLimit.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty UndoLimitProperty =
            DependencyProperty.Register("UndoLimit", typeof(int), typeof(TextBox), new UIPropertyMetadata(1000));

        #endregion

        #region AutoWordSelection

        /// <summary>
        /// Gets or sets whether auto word selection feature is enabled
        /// </summary>
        public bool AutoWordSelection
        {
            get { return (bool)this.GetValue(AutoWordSelectionProperty); }
            set { this.SetValue(AutoWordSelectionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AutoWordSelection.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AutoWordSelectionProperty =
            DependencyProperty.Register("AutoWordSelection", typeof(bool), typeof(TextBox), new UIPropertyMetadata(false));

        #endregion

        #region SelectionBrush

        /// <summary>
        /// Gets or sets the brush that highlights the selected text
        /// </summary>
        public Brush SelectionBrush
        {
            get { return (Brush)this.GetValue(SelectionBrushProperty); }
            set { this.SetValue(SelectionBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionBrush.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(TextBox),
            new UIPropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x33, 0x99, 0xFF))));

        #endregion

        #region SelectionOpacity

        /// <summary>
        /// Gets or sets opacity of the selection brush
        /// </summary>
        public double SelectionOpacity
        {
            get { return (double)this.GetValue(SelectionOpacityProperty); }
            set { this.SetValue(SelectionOpacityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectionOpacity.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectionOpacityProperty =
            DependencyProperty.Register("SelectionOpacity", typeof(double), typeof(TextBox), new UIPropertyMetadata(0.4d));

        #endregion

        #region CaretBrush

        /// <summary>
        /// Gets or sets the caret brush that is used 
        /// to paint the caret of the text box
        /// </summary>
        public Brush CaretBrush
        {
            get { return (Brush)this.GetValue(CaretBrushProperty); }
            set { this.SetValue(CaretBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CaretBrush.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CaretBrushProperty =
            DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(TextBox),
            new UIPropertyMetadata(null));

        #endregion

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets character index of the beginning of the current selection
        /// </summary>
        public int SelectionStart
        {
            get { return this.textBox.SelectionStart; }
            set
            {
                if (this.textBoxTemplated != null)
                    this.textBoxTemplated.SelectionStart = value;
                else
                    this.textBox.SelectionStart = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicating the number of characters in the current selection
        /// </summary>
        public int SelectionLength
        {
            get { return this.textBox.SelectionLength; }
            set
            {
                if (this.textBoxTemplated != null)
                    this.textBoxTemplated.SelectionLength = value;
                else
                    this.textBox.SelectionLength = value;
            }
        }

        /// <summary>
        /// Gets or sets content of the selection
        /// </summary>
        public string SelectedText
        {
            get { return this.textBox.SelectedText; }
            set
            {
                if (this.textBoxTemplated != null)
                    this.textBoxTemplated.SelectedText = value;
                else
                    this.textBox.SelectedText = value;
            }
        }

        /// <summary>
        /// Gets a value that indicating whether actions can be undo
        /// </summary>
        public bool CanUndo
        {
            get { return this.textBoxTemplated == null ? false : this.textBoxTemplated.CanUndo; }
        }

        /// <summary>
        /// Gets a value that indicating whether actions can be redo
        /// </summary>
        public bool CanRedo
        {
            get { return this.textBoxTemplated == null ? false : this.textBoxTemplated.CanRedo; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TextBox()
        {
            this.Focusable = true;
            this.textBox.SelectionChanged += (s, e) => this.RaiseSelectionChanged();
            this.textBox.TextChanged += (s, e) => this.RaiseTextChanged(e);

            var binding = new Binding("Text");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            this.textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);

            Bind(this.textBox, this, "CharacterCasing", System.Windows.Controls.TextBox.CharacterCasingProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "MaxLength", System.Windows.Controls.TextBox.MaxLengthProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "TextAlignment", System.Windows.Controls.TextBox.TextAlignmentProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "TextDecorations", System.Windows.Controls.TextBox.TextDecorationsProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "IsUndoEnabled", TextBoxBase.IsUndoEnabledProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "UndoLimit", TextBoxBase.UndoLimitProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "AutoWordSelection", TextBoxBase.AutoWordSelectionProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "SelectionBrush", TextBoxBase.SelectionBrushProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "SelectionOpacity", TextBoxBase.SelectionOpacityProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "CaretBrush", TextBoxBase.CaretBrushProperty, BindingMode.TwoWay);
            Bind(this.textBox, this, "IsReadOnly", TextBoxBase.IsReadOnlyProperty, BindingMode.TwoWay);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Appends text
        /// </summary>
        /// <param name="text">Text</param>
        public void AppendText(string text)
        {
            if (this.textBoxTemplated != null)
                this.textBoxTemplated.AppendText(text);
            else
                this.textBox.AppendText(text);
        }

        /// <summary>
        /// Copies selected content to the clipboard
        /// </summary>
        public void Copy()
        {
            if (this.textBoxTemplated != null)
                this.textBoxTemplated.Copy();
            else
                this.textBox.Copy();
        }

        /// <summary>
        /// Cuts selected content to the clipboard
        /// </summary>
        public void Cut()
        {
            if (this.textBoxTemplated != null)
                this.textBoxTemplated.Cut();
            else
                this.textBox.Cut();
        }

        /// <summary>
        /// Pastes content from the clipboard
        /// </summary>
        public void Paste()
        {
            if (this.textBoxTemplated != null)
                this.textBoxTemplated.Paste();
            else
                this.textBox.Paste();
        }

        /// <summary>
        /// Undoes the most recent undo command
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if (this.textBoxTemplated != null) return this.textBoxTemplated.Undo();
            return false;
        }

        /// <summary>
        /// Redoes the most recent undo command
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if (this.textBoxTemplated != null) return this.textBoxTemplated.Redo();
            return false;
        }

        /// <summary>
        /// Selects all the contents
        /// </summary>
        public void SelectAll()
        {
            if (this.textBoxTemplated != null)
                this.textBoxTemplated.SelectAll();
            else
                this.textBox.SelectAll();
        }

        /// <summary>
        /// Selects contents
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="length">Length</param>
        public void Select(int start, int length)
        {
            if (this.textBoxTemplated != null)
                this.textBoxTemplated.Select(start, length);
            else
                this.textBox.Select(start, length);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever 
        /// application code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.textBoxTemplated != null)
            {
                this.textBoxTemplated.PreviewKeyDown -= this.OnTextBoxTemplatedKeyDown;
                this.textBoxTemplated.SelectionChanged -= this.OnTextBoxTemplatedSelectionChanged;
                this.textBoxTemplated.LostFocus -= this.OnTextBoxTemplatedLostFocus;
                this.textBoxTemplated.GotKeyboardFocus -= this.OnTextBoxTemplatedGotKeyboardFocus;
                this.textBoxTemplated.TextChanged -= this.OnTextBoxTemplatedTextChanged;
                BindingOperations.ClearAllBindings(this.textBoxTemplated);
            }
            this.textBoxTemplated = this.GetTemplateChild("PART_TextBox") as System.Windows.Controls.TextBox;


            // Check template
            if (!this.IsTemplateValid())
            {
                Debug.WriteLine("Template for TextBox control is invalid");
                return;
            }

            this.textBoxTemplated.Text = this.Text;
            this.textBoxTemplated.Select(this.textBox.SelectionStart, this.textBox.SelectionLength);

            // Bindings
            BindingOperations.ClearAllBindings(this.textBox);

            var binding = new Binding("Text");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            this.textBoxTemplated.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);

            Bind(this, this.textBoxTemplated, "CharacterCasing", System.Windows.Controls.TextBox.CharacterCasingProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "MaxLength", System.Windows.Controls.TextBox.MaxLengthProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "TextAlignment", System.Windows.Controls.TextBox.TextAlignmentProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "TextDecorations", System.Windows.Controls.TextBox.TextDecorationsProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "IsUndoEnabled", TextBoxBase.IsUndoEnabledProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "UndoLimit", TextBoxBase.UndoLimitProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "AutoWordSelection", TextBoxBase.AutoWordSelectionProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "SelectionBrush", TextBoxBase.SelectionBrushProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "SelectionOpacity", TextBoxBase.SelectionOpacityProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "CaretBrush", TextBoxBase.CaretBrushProperty, BindingMode.TwoWay);
            Bind(this, this.textBoxTemplated, "IsReadOnly", TextBoxBase.IsReadOnlyProperty, BindingMode.TwoWay);

            this.textBoxTemplated.PreviewKeyDown += this.OnTextBoxTemplatedKeyDown;
            this.textBoxTemplated.SelectionChanged += this.OnTextBoxTemplatedSelectionChanged;
            this.textBoxTemplated.LostFocus += this.OnTextBoxTemplatedLostFocus;
            this.textBoxTemplated.GotKeyboardFocus += this.OnTextBoxTemplatedGotKeyboardFocus;
            this.textBoxTemplated.TextChanged += this.OnTextBoxTemplatedTextChanged;
        }

        private void OnTextBoxTemplatedTextChanged(object sender, TextChangedEventArgs e)
        {
            this.RaiseTextChanged(e);
        }

        private void OnTextBoxTemplatedGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.textBoxContentWhenGotFocus = this.textBoxTemplated.Text;
        }

        private void OnTextBoxTemplatedLostFocus(object sender, RoutedEventArgs e)
        {
            if (this.textBoxContentWhenGotFocus != this.textBoxTemplated.Text)
                this.ExecuteCommand();
        }

        private void OnTextBoxTemplatedSelectionChanged(object sender, RoutedEventArgs e)
        {
            this.textBox.Select(this.textBoxTemplated.SelectionStart, this.textBoxTemplated.SelectionLength);
        }

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            if (!this.IsTemplateValid()) return;

            // Use dispatcher to avoid focus moving to backup'ed element 
            // (focused element before keytips processing)
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                (ThreadStart)(() =>
                {
                    this.textBoxTemplated.SelectAll();
                    this.textBoxTemplated.Focus();
                }));
        }

        /// <summary>
        /// Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus"/> event reaches this element in its route.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (this.textBoxTemplated != null)
            {
                this.textBoxTemplated.Focus();
            }
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyUp�attached event reaches 
        /// an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Avoid Click invocation (from RibbonControl)
            if (e.Key == Key.Enter || e.Key == Key.Space) return;
            base.OnKeyUp(e);
        }

        private void OnTextBoxTemplatedKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Move Focus
                this.textBoxTemplated.Focusable = false;
                this.Focus();
                this.textBoxTemplated.Focusable = true;
            }
        }

        #endregion

        #region Private methods

        private bool IsTemplateValid()
        {
            return this.textBoxTemplated != null;
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
            var textBox = new TextBox();

            this.BindQuickAccessItem(textBox);


            return textBox;
        }

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);

            var textBoxQAT = (TextBox)element;

            textBoxQAT.Width = this.Width;
            textBoxQAT.InputWidth = this.InputWidth;

            Bind(this, textBoxQAT, "Text", TextProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "IsReadOnly", IsReadOnlyProperty, BindingMode.OneWay);
            Bind(this, textBoxQAT, "CharacterCasing", CharacterCasingProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "MaxLength", MaxLengthProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "TextAlignment", TextAlignmentProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "TextDecorations", TextDecorationsProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "IsUndoEnabled", IsUndoEnabledProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "UndoLimit", UndoLimitProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "AutoWordSelection", AutoWordSelectionProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "SelectionBrush", SelectionBrushProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "SelectionOpacity", SelectionOpacityProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "CaretBrush", CaretBrushProperty, BindingMode.TwoWay);
            Bind(this, textBoxQAT, "IsReadOnly", IsReadOnlyProperty, BindingMode.TwoWay);

            RibbonControl.BindQuickAccessItem(this, element);
        }

        #endregion
    }
}