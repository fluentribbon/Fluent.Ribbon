// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents specific label to use in particular ribbon controls
    /// </summary>
    [DefaultProperty(nameof(Text))]
    [ContentProperty(nameof(Text))]
    [TemplatePart(Name = "PART_TextRun", Type = typeof(AccessText))]
    [TemplatePart(Name = "PART_TextRun2", Type = typeof(AccessText))]
    public class TwoLineLabel : Control
    {
        #region Fields

        /// <summary>
        /// Run with text
        /// </summary>
        private AccessText textRun;

        private AccessText textRun2;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether label must have two lines
        /// </summary>
        public bool HasTwoLines
        {
            get { return (bool)this.GetValue(HasTwoLinesProperty); }
            set { this.SetValue(HasTwoLinesProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="HasTwoLines"/>.
        /// </summary>
        public static readonly DependencyProperty HasTwoLinesProperty =
            DependencyProperty.Register(nameof(HasTwoLines), typeof(bool), typeof(TwoLineLabel), new PropertyMetadata(BooleanBoxes.TrueBox, OnHasTwoLinesChanged));

        /// <summary>
        /// Handles HasTwoLines property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnHasTwoLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TwoLineLabel)d).UpdateTextRun();
        }

        /// <summary>
        /// Gets or sets whether label has glyph
        /// </summary>
        public bool HasGlyph
        {
            get { return (bool)this.GetValue(HasGlyphProperty); }
            set { this.SetValue(HasGlyphProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="HasGlyph"/>.
        /// </summary>
        public static readonly DependencyProperty HasGlyphProperty =
            DependencyProperty.Register(nameof(HasGlyph), typeof(bool), typeof(TwoLineLabel), new PropertyMetadata(BooleanBoxes.FalseBox, OnHasGlyphChanged));

        /// <summary>
        /// Handles HasGlyph property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnHasGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TwoLineLabel)d).UpdateTextRun();
        }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Text"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
#pragma warning disable WPF0010 // Default value type must match registered type.
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TwoLineLabel), new PropertyMetadata(StringBoxes.Empty, OnTextChanged));
#pragma warning restore WPF0010 // Default value type must match registered type.

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        static TwoLineLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(typeof(TwoLineLabel)));

            FocusableProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            this.textRun = this.GetTemplateChild("PART_TextRun") as AccessText;
            this.textRun2 = this.GetTemplateChild("PART_TextRun2") as AccessText;

            this.UpdateTextRun();
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Handles text property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var label = (TwoLineLabel)d;
            label.UpdateTextRun();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates text runs and adds newline if HasTwoLines == true
        /// </summary>
        private void UpdateTextRun()
        {
            if (this.textRun == null
                || this.textRun2 == null)
            {
                return;
            }

            var text = this.Text?.Trim();

            if (this.HasTwoLines == false
                || string.IsNullOrEmpty(text))
            {
                this.textRun.Text = text;
                this.textRun2.Text = string.Empty;
                return;
            }

            // Find soft hyphen, break at its position and display a normal hyphen.
            var hyphenIndex = text.IndexOf((char)173);

            if (hyphenIndex >= 0)
            {
                this.textRun.Text = text.Substring(0, hyphenIndex) + "-";
                this.textRun2.Text = text.Substring(hyphenIndex) + " ";
            }
            else
            {
                var centerIndex = text.Length / 2;

                // Find spaces nearest to center from left and right
                var leftSpaceIndex = text.LastIndexOf(" ", centerIndex, centerIndex, StringComparison.CurrentCulture);
                var rightSpaceIndex = text.IndexOf(" ", centerIndex, StringComparison.CurrentCulture);

                if (leftSpaceIndex == -1
                    && rightSpaceIndex == -1)
                {
                    this.textRun.Text = text;
                    this.textRun2.Text = string.Empty;
                }
                else if (leftSpaceIndex == -1)
                {
                    // Finds only space from right. New line adds on it
                    this.textRun.Text = text.Substring(0, rightSpaceIndex);
                    this.textRun2.Text = text.Substring(rightSpaceIndex) + " ";
                }
                else if (rightSpaceIndex == -1)
                {
                    // Finds only space from left. New line adds on it
                    this.textRun.Text = text.Substring(0, leftSpaceIndex);
                    this.textRun2.Text = text.Substring(leftSpaceIndex) + " ";
                }
                else
                {
                    // Find nearest to center space and add new line on it
                    if (Math.Abs(centerIndex - leftSpaceIndex) < Math.Abs(centerIndex - rightSpaceIndex))
                    {
                        this.textRun.Text = text.Substring(0, leftSpaceIndex);
                        this.textRun2.Text = text.Substring(leftSpaceIndex) + " ";
                    }
                    else
                    {
                        this.textRun.Text = text.Substring(0, rightSpaceIndex);
                        this.textRun2.Text = text.Substring(rightSpaceIndex) + " ";
                    }
                }
            }
        }

        #endregion
    }
}