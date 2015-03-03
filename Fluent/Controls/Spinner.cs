#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

namespace Fluent
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Threading;
    using Fluent.Internal;

    /// <summary>
    /// Represents spinner control
    /// </summary>
    [ContentProperty("Value")]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(System.Windows.Controls.TextBox))]
    [TemplatePart(Name = "PART_ButtonUp", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_ButtonDown", Type = typeof(RepeatButton))]
    public class Spinner : RibbonControl
    {
        #region Events

        /// <summary>
        /// Occurs when value has been changed
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> ValueChanged;

        #endregion

        #region Fields

        // Parts of the control (must be in control template)
        private System.Windows.Controls.TextBox textBox;
        private RepeatButton buttonUp;
        private RepeatButton buttonDown;

        #endregion

        #region Properties

        #region Value

        /// <summary>
        /// Gets or sets current value
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1721")]
        public double Value
        {
            get { return (double)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Value.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ValueProperty;

        private static object CoerceValue(DependencyObject d, object basevalue)
        {
            var spinner = (Spinner)d;
            var value = (double)basevalue;
            value = GetLimitedValue(spinner, value);
            return value;
        }

        private static double GetLimitedValue(Spinner spinner, double value)
        {
            value = Math.Max(spinner.Minimum, value);
            value = Math.Min(spinner.Maximum, value);
            return value;
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spinner = (Spinner)d;
            spinner.ValueToTextBoxText();

            if (spinner.ValueChanged != null)
            {
                spinner.ValueChanged(spinner, new RoutedPropertyChangedEventArgs<double>((double)e.OldValue, (double)e.NewValue));
            }
        }

        private void ValueToTextBoxText()
        {
            if (this.IsTemplateValid())
            {
                this.textBox.Text = this.Value.ToString(this.Format, CultureInfo.CurrentCulture);
                this.Text = this.textBox.Text;
            }
        }

        #endregion

        #region Text

        /// <summary>
        /// Gets current text from the spinner
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            private set { this.SetValue(textPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey textPropertyKey = DependencyProperty.RegisterReadOnly("Text", typeof(string), typeof(Spinner), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty = textPropertyKey.DependencyProperty;

        #endregion

        #region Increment

        /// <summary>
        /// Gets or sets a value added or subtracted from the value property
        /// </summary>
        public double Increment
        {
            get { return (double)this.GetValue(IncrementProperty); }
            set { this.SetValue(IncrementProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Increment.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(Spinner), new UIPropertyMetadata(1.0d));

        #endregion

        #region Minimum

        /// <summary>
        /// Gets or sets minimun value
        /// </summary>
        public double Minimum
        {
            get { return (double)this.GetValue(MinimumProperty); }
            set { this.SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Minimum.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimumProperty;

        static object CoerceMinimum(DependencyObject d, object basevalue)
        {
            var spinner = (Spinner)d;
            var value = (double)basevalue;

            if (spinner.Maximum < value)
            {
                return spinner.Maximum;
            }

            return value;
        }

        static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spinner = (Spinner)d;
            var value = (double)CoerceValue(d, spinner.Value);

            if (DoubleUtil.AreClose(value, spinner.Value) == false)
            {
                spinner.Value = value;
            }
        }

        #endregion

        #region Maximum

        /// <summary>
        /// Gets or sets maximum value
        /// </summary>
        public double Maximum
        {
            get { return (double)this.GetValue(MaximumProperty); }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Maximum.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MaximumProperty;

        static object CoerceMaximum(DependencyObject d, object basevalue)
        {
            var spinner = (Spinner)d;
            var value = (double)basevalue;

            if (spinner.Minimum > value)
            {
                return spinner.Minimum;
            }

            return value;
        }

        static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spinner = (Spinner)d;
            var value = (double)CoerceValue(d, spinner.Value);

            if (DoubleUtil.AreClose(value, spinner.Value) == false)
            {
                spinner.Value = value;
            }
        }

        #endregion

        #region Format

        /// <summary>
        /// Gets or sets string format of value
        /// </summary>
        public string Format
        {
            get { return (string)this.GetValue(FormatProperty); }
            set { this.SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Format.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(Spinner), new UIPropertyMetadata("F1", OnFormatChanged));

        static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spinner = (Spinner)d;
            spinner.ValueToTextBoxText();
        }

        #endregion

        #region Delay

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, 
        /// the Spinner waits while it is pressed before it starts repeating. 
        /// The value must be non-negative. This is a dependency property.
        /// </summary>
        public int Delay
        {
            get { return (int)this.GetValue(DelayProperty); }
            set { this.SetValue(DelayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Delay.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DelayProperty =
            DependencyProperty.Register("Delay", typeof(int), typeof(Spinner),
            new UIPropertyMetadata(400));

        #endregion

        #region Interval

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, 
        /// between repeats once repeating starts. The value must be non-negative. 
        /// This is a dependency property.
        /// </summary>
        public int Interval
        {
            get { return (int)this.GetValue(IntervalProperty); }
            set { this.SetValue(IntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Interval.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(Spinner), new UIPropertyMetadata(80));

        #endregion

        #region InputWidth

        /// <summary>
        /// Gets or sets width of the value input part of spinner
        /// </summary>               
        public double InputWidth
        {
            get { return (double)this.GetValue(InputWidthProperty); }
            set { this.SetValue(InputWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InputWidthProperty =
            DependencyProperty.Register("InputWidth", typeof(double), typeof(Spinner), new UIPropertyMetadata(double.NaN));

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Spinner()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(typeof(Spinner)));

            MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(Spinner), new UIPropertyMetadata(double.MaxValue, OnMaximumChanged, CoerceMaximum));
            MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(Spinner), new UIPropertyMetadata(0.0d, OnMinimumChanged, CoerceMinimum));
            ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Spinner), new FrameworkPropertyMetadata(0.0d, OnValueChanged, CoerceValue) { BindsTwoWayByDefault = true });

            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Spinner), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (this.IsTemplateValid())
            {
                this.buttonUp.Click -= this.OnButtonUpClick;
                this.buttonDown.Click -= this.OnButtonDownClick;
                BindingOperations.ClearAllBindings(this.buttonDown);
                BindingOperations.ClearAllBindings(this.buttonUp);
            }

            // Get template childs
            this.textBox = this.GetTemplateChild("PART_TextBox") as System.Windows.Controls.TextBox;
            this.buttonUp = this.GetTemplateChild("PART_ButtonUp") as RepeatButton;
            this.buttonDown = this.GetTemplateChild("PART_ButtonDown") as RepeatButton;

            // Check template
            if (this.IsTemplateValid() == false)
            {
                Debug.WriteLine("Template for Spinner control is invalid");
                return;
            }

            // Bindings
            Bind(this, this.buttonUp, "Delay", RepeatButton.DelayProperty, BindingMode.OneWay);
            Bind(this, this.buttonDown, "Delay", RepeatButton.DelayProperty, BindingMode.OneWay);
            Bind(this, this.buttonUp, "Interval", RepeatButton.IntervalProperty, BindingMode.OneWay);
            Bind(this, this.buttonDown, "Interval", RepeatButton.IntervalProperty, BindingMode.OneWay);


            // Events subscribing
            this.buttonUp.Click += this.OnButtonUpClick;
            this.buttonDown.Click += this.OnButtonDownClick;
            this.textBox.LostKeyboardFocus += this.OnTextBoxLostKeyboardFocus;
            this.textBox.PreviewKeyDown += this.OnTextBoxPreviewKeyDown;

            this.ValueToTextBoxText();
        }

        bool IsTemplateValid()
        {
            return this.textBox != null
                && this.buttonUp != null
                && this.buttonDown != null;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            if (this.IsTemplateValid() == false)
            {
                return;
            }

            // Use dispatcher to avoid focus moving to backup'ed element 
            // (focused element before keytips processing)
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                (ThreadStart)(() =>
                {
                    this.textBox.SelectAll();
                    this.textBox.Focus();
                }));
            base.OnKeyTipPressed();
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyUp�attached event reaches 
        /// an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Avoid Click invocation (from RibbonControl)
            if (e.Key == Key.Enter
                || e.Key == Key.Space)
            {
                return;
            }

            base.OnKeyUp(e);
        }

        private void OnButtonUpClick(object sender, RoutedEventArgs e)
        {
            this.Value = GetLimitedValue(this, this.Value + this.Increment);
        }

        private void OnButtonDownClick(object sender, RoutedEventArgs e)
        {
            this.Value = GetLimitedValue(this, this.Value - this.Increment);
        }

        private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.TextBoxTextToValue();
        }

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.TextBoxTextToValue();
            }

            if (e.Key == Key.Escape)
            {
                this.ValueToTextBoxText();
            }

            if (e.Key == Key.Enter
                || e.Key == Key.Escape)
            {
                // Move Focus
                this.textBox.Focusable = false;
                this.Focus();
                this.textBox.Focusable = true;
                e.Handled = true;
            }

            if (e.Key == Key.Up)
            {
                this.buttonUp.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }

            if (e.Key == Key.Down)
            {
                this.buttonDown.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void TextBoxTextToValue()
        {
            var text = this.textBox.Text;

            // Remove all except digits, signs and commas
            var stringBuilder = new StringBuilder();

            foreach (var symbol in text)
            {
                if (Char.IsDigit(symbol)
                    || symbol == ','
                    || symbol == '.'
                    || (symbol == '-' && stringBuilder.Length == 0))
                {
                    stringBuilder.Append(symbol);
                }
            }

            text = stringBuilder.ToString();

            double value;

            if (Double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out value))
            {
                this.Value = GetLimitedValue(this, value);
            }

            this.ValueToTextBoxText();
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
            var spinner = new Spinner();
            this.BindQuickAccessItem(spinner);
            return spinner;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected void BindQuickAccessItem(FrameworkElement element)
        {
            var spinner = (Spinner)element;

            BindQuickAccessItem(this, element);

            spinner.Width = this.Width;
            spinner.InputWidth = this.InputWidth;

            Bind(this, spinner, "Value", ValueProperty, BindingMode.TwoWay);
            Bind(this, spinner, "Increment", IncrementProperty, BindingMode.OneWay);
            Bind(this, spinner, "Minimum", MinimumProperty, BindingMode.OneWay);
            Bind(this, spinner, "Maximum", MaximumProperty, BindingMode.OneWay);
            Bind(this, spinner, "Format", FormatProperty, BindingMode.OneWay);
            Bind(this, spinner, "Delay", DelayProperty, BindingMode.OneWay);
            Bind(this, spinner, "Interval", IntervalProperty, BindingMode.OneWay);

            BindQuickAccessItem(this, element);
        }

        #endregion
    }
}