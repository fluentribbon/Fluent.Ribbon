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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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

        void RaiseTextChanged(TextChangedEventArgs args)
        {
            if (TextChanged != null) TextChanged(this, args);
        }

        /// <summary>
        /// Occurs when selection changed
        /// </summary>
        public event EventHandler SelectionChanged;

        void RaiseSelectionChanged()
        {
            if (SelectionChanged != null) SelectionChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Fields

        // TextBox in template
        System.Windows.Controls.TextBox textBoxTemplated;
        // Local TextBox
        System.Windows.Controls.TextBox textBox = new System.Windows.Controls.TextBox();
        // Content when the textbox got focus
        string textBoxContentWhenGotFocus = null;

        #endregion

        #region Properties (Dependency)

        #region InputWidth

        /// <summary>
        /// Gets or sets width of the value input part of textbox
        /// </summary>               
        public double InputWidth
        {
            get { return (double)GetValue(InputWidthProperty); }
            set { SetValue(InputWidthProperty, value); }
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
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
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
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
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
            get { return (CharacterCasing)GetValue(CharacterCasingProperty); }
            set { SetValue(CharacterCasingProperty, value); }
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
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MaxLength. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBox), new UIPropertyMetadata(Int32.MaxValue));

        #endregion

        #region TextAlignment

        /// <summary>
        /// Gets or sets horizontal text alignment of the content
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return (TextAlignment)GetValue(TextAlignmentProperty); }
            set { SetValue(TextAlignmentProperty, value); }
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
            get { return (TextDecorationCollection)GetValue(TextDecorationsProperty); }
            set { SetValue(TextDecorationsProperty, value); }
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
            get { return (bool)GetValue(IsUndoEnabledProperty); }
            set { SetValue(IsUndoEnabledProperty, value); }
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
            get { return (int)GetValue(UndoLimitProperty); }
            set { SetValue(UndoLimitProperty, value); }
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
            get { return (bool)GetValue(AutoWordSelectionProperty); }
            set { SetValue(AutoWordSelectionProperty, value); }
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
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
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
            get { return (double)GetValue(SelectionOpacityProperty); }
            set { SetValue(SelectionOpacityProperty, value); }
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
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
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
            get { return textBox.SelectionStart; }
            set
            {
                if (textBoxTemplated != null) textBoxTemplated.SelectionStart = value;
                else textBox.SelectionStart = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicating the number of characters in the current selection
        /// </summary>
        public int SelectionLength
        {
            get { return textBox.SelectionLength; }
            set
            {
                if (textBoxTemplated != null) textBoxTemplated.SelectionLength = value;
                else textBox.SelectionLength = value;
            }
        }

        /// <summary>
        /// Gets or sets content of the selection
        /// </summary>
        public string SelectedText
        {
            get { return textBox.SelectedText; }
            set
            {
                if (textBoxTemplated != null) textBoxTemplated.SelectedText = value;
                else textBox.SelectedText = value;
            }
        }

        /// <summary>
        /// Gets a value that indicating whether actions can be undo
        /// </summary>
        public bool CanUndo
        {
            get { return textBoxTemplated == null ? false : textBoxTemplated.CanUndo; }
        }

        /// <summary>
        /// Gets a value that indicating whether actions can be redo
        /// </summary>
        public bool CanRedo
        {
            get { return textBoxTemplated == null ? false : textBoxTemplated.CanRedo; }
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
            StyleProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(TextBox));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TextBox()
        {
            Focusable = true;
            textBox.SelectionChanged += (s, e) => RaiseSelectionChanged();
            textBox.TextChanged += (s, e) => RaiseTextChanged(e);

            Binding binding = new Binding("Text");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            textBox.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);

            Bind(textBox, this, "CharacterCasing", System.Windows.Controls.TextBox.CharacterCasingProperty, BindingMode.TwoWay);
            Bind(textBox, this, "MaxLength", System.Windows.Controls.TextBox.MaxLengthProperty, BindingMode.TwoWay);
            Bind(textBox, this, "TextAlignment", System.Windows.Controls.TextBox.TextAlignmentProperty, BindingMode.TwoWay);
            Bind(textBox, this, "TextDecorations", System.Windows.Controls.TextBox.TextDecorationsProperty, BindingMode.TwoWay);
            Bind(textBox, this, "IsUndoEnabled", System.Windows.Controls.TextBox.IsUndoEnabledProperty, BindingMode.TwoWay);
            Bind(textBox, this, "UndoLimit", System.Windows.Controls.TextBox.UndoLimitProperty, BindingMode.TwoWay);
            Bind(textBox, this, "AutoWordSelection", System.Windows.Controls.TextBox.AutoWordSelectionProperty, BindingMode.TwoWay);
            Bind(textBox, this, "SelectionBrush", System.Windows.Controls.TextBox.SelectionBrushProperty, BindingMode.TwoWay);
            Bind(textBox, this, "SelectionOpacity", System.Windows.Controls.TextBox.SelectionOpacityProperty, BindingMode.TwoWay);
            Bind(textBox, this, "CaretBrush", System.Windows.Controls.TextBox.CaretBrushProperty, BindingMode.TwoWay);
            Bind(textBox, this, "IsReadOnly", System.Windows.Controls.TextBox.IsReadOnlyProperty, BindingMode.TwoWay);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Appends text
        /// </summary>
        /// <param name="text">Text</param>
        public void AppendText(string text)
        {
            if (textBoxTemplated != null) textBoxTemplated.AppendText(text);
            else textBox.AppendText(text);
        }

        /// <summary>
        /// Copies selected content to the clipboard
        /// </summary>
        public void Copy()
        {
            if (textBoxTemplated != null) textBoxTemplated.Copy();
            else textBox.Copy();
        }

        /// <summary>
        /// Cuts selected content to the clipboard
        /// </summary>
        public void Cut()
        {
            if (textBoxTemplated != null) textBoxTemplated.Cut();
            else textBox.Cut();
        }

        /// <summary>
        /// Pastes content from the clipboard
        /// </summary>
        public void Paste()
        {
            if (textBoxTemplated != null) textBoxTemplated.Paste();
            else textBox.Paste();
        }

        /// <summary>
        /// Undoes the most recent undo command
        /// </summary>
        /// <returns></returns>
        public bool Undo()
        {
            if (textBoxTemplated != null) return textBoxTemplated.Undo();
            return false;
        }

        /// <summary>
        /// Redoes the most recent undo command
        /// </summary>
        /// <returns></returns>
        public bool Redo()
        {
            if (textBoxTemplated != null) return textBoxTemplated.Redo();
            return false;
        }

        /// <summary>
        /// Selects all the contents
        /// </summary>
        public void SelectAll()
        {
            if (textBoxTemplated != null) textBoxTemplated.SelectAll();
            else textBox.SelectAll();
        }

        /// <summary>
        /// Selects contents
        /// </summary>
        /// <param name="start">Start</param>
        /// <param name="length">Length</param>
        public void Select(int start, int length)
        {
            if (textBoxTemplated != null) textBoxTemplated.Select(start, length);
            else textBox.Select(start, length);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever 
        /// application code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (textBoxTemplated != null)
            {
                textBoxTemplated.PreviewKeyDown -= OnTextBoxTemplatedKeyDown;
                textBoxTemplated.SelectionChanged -= OnTextBoxTemplatedSelectionChanged;
                textBoxTemplated.LostFocus -= OnTextBoxTemplatedLostFocus;
                textBoxTemplated.GotKeyboardFocus -= OnTextBoxTemplatedGotKeyboardFocus;
                textBoxTemplated.TextChanged -= OnTextBoxTemplatedTextChanged;
                BindingOperations.ClearAllBindings(textBoxTemplated);
            }
            textBoxTemplated = GetTemplateChild("PART_TextBox") as System.Windows.Controls.TextBox;


            // Check template
            if (!IsTemplateValid())
            {
                Debug.WriteLine("Template for TextBox control is invalid");
                return;
            }

            textBoxTemplated.Text = Text;
            textBoxTemplated.Select(textBox.SelectionStart, textBox.SelectionLength);

            // Bindings
            BindingOperations.ClearAllBindings(textBox);

            Binding binding = new Binding("Text");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            textBoxTemplated.SetBinding(System.Windows.Controls.TextBox.TextProperty, binding);

            Bind(this, textBoxTemplated, "CharacterCasing", System.Windows.Controls.TextBox.CharacterCasingProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "MaxLength", System.Windows.Controls.TextBox.MaxLengthProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "TextAlignment", System.Windows.Controls.TextBox.TextAlignmentProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "TextDecorations", System.Windows.Controls.TextBox.TextDecorationsProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "IsUndoEnabled", System.Windows.Controls.TextBox.IsUndoEnabledProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "UndoLimit", System.Windows.Controls.TextBox.UndoLimitProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "AutoWordSelection", System.Windows.Controls.TextBox.AutoWordSelectionProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "SelectionBrush", System.Windows.Controls.TextBox.SelectionBrushProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "SelectionOpacity", System.Windows.Controls.TextBox.SelectionOpacityProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "CaretBrush", System.Windows.Controls.TextBox.CaretBrushProperty, BindingMode.TwoWay);
            Bind(this, textBoxTemplated, "IsReadOnly", System.Windows.Controls.TextBox.IsReadOnlyProperty, BindingMode.TwoWay);

            textBoxTemplated.PreviewKeyDown += OnTextBoxTemplatedKeyDown;
            textBoxTemplated.SelectionChanged += OnTextBoxTemplatedSelectionChanged;
            textBoxTemplated.LostFocus += OnTextBoxTemplatedLostFocus;
            textBoxTemplated.GotKeyboardFocus += OnTextBoxTemplatedGotKeyboardFocus;
            textBoxTemplated.TextChanged += OnTextBoxTemplatedTextChanged;
        }

        void OnTextBoxTemplatedTextChanged(object sender, TextChangedEventArgs e)
        {
            RaiseTextChanged(e);
        }

        void OnTextBoxTemplatedGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            textBoxContentWhenGotFocus = textBoxTemplated.Text;
        }

        void OnTextBoxTemplatedLostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxContentWhenGotFocus != textBoxTemplated.Text) ExecuteCommand();
        }

        void OnTextBoxTemplatedSelectionChanged(object sender, RoutedEventArgs e)
        {
            textBox.Select(textBoxTemplated.SelectionStart, textBoxTemplated.SelectionLength);
        }

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            if (!IsTemplateValid()) return;

            // Use dispatcher to avoid focus moving to backup'ed element 
            // (focused element before keytips processing)
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                (ThreadStart)(() =>
                {
                    textBoxTemplated.SelectAll();
                    textBoxTemplated.Focus();
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

        void OnTextBoxTemplatedKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Move Focus
                textBoxTemplated.Focusable = false;
                Focus();
                textBoxTemplated.Focusable = true;
            }
        }

        #endregion

        #region Private methods

        bool IsTemplateValid()
        {
            return textBoxTemplated != null;
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
            TextBox textBox = new TextBox();

            BindQuickAccessItem(textBox);


            return textBox;
        }


        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);

            TextBox textBoxQAT = (TextBox)element;

            textBoxQAT.Width = Width;
            textBoxQAT.InputWidth = InputWidth;

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
