// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents specific label to use in particular ribbon controls
    /// </summary>
    [DefaultProperty(nameof(Text))]
    [ContentProperty(nameof(Text))]
    [TemplatePart(Name = "PART_TextRun", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_TextRun2", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Glyph", Type = typeof(InlineUIContainer))]
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
        /// Using a DependencyProperty as the backing store for HasTwoLines.  
        /// This enables animation, styling, binding, etc...
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
        /// Using a DependencyProperty as the backing store for HasGlyph.  This enables animation, styling, binding, etc...
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
        /// Gets or sets labels text
        /// </summary>
        public string Text
        {
            get { return (string)this.GetValue(TextProperty); }
            set { this.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TwoLineLabel), new PropertyMetadata(StringBoxes.Empty, OnTextChanged));

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static TwoLineLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(typeof(TwoLineLabel)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TwoLineLabel()
        {
            this.Focusable = false;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal 
        /// processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.textRun = this.GetTemplateChild("PART_TextRun") as AccessText;
            this.textRun2 = this.GetTemplateChild("PART_TextRun2") as AccessText;
            this.UpdateTextRun();
        }

        protected override Size MeasureOverride(Size constraint)
        {
          this.textRun.Measure(constraint);
          this.textRun2.Measure(constraint);
          var size = base.MeasureOverride(constraint);

          return size;
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
                label?.UpdateTextRun();
            }

            #endregion

        #region Private methods

        /// <summary>
        /// Updates text run adds newline if HasTwoLines == true
        /// </summary>
        private void UpdateTextRun()
        {
            if (this.textRun == null
                || this.textRun2 == null)
            {
                return;
            }

            if (this.HasTwoLines == false
                || string.IsNullOrEmpty(this.Text))
            {
                this.textRun.Text = this.Text;
                this.textRun2.Text = string.Empty;
                return;
            }

            var text = this.Text.Trim();

            // Find soft hyphen, break at its position and display a normal hyphen.
            var hyphenIndex = text.IndexOf((char)173);

            if (hyphenIndex >= 0)
            {
                this.textRun.Text = text.Substring(0, hyphenIndex) + "-";
                this.textRun2.Text = text.Substring(hyphenIndex) + " ";
            }
            else
            {
                var centerIndex = this.Text.Length / 2;
                // Find spaces nearest to center from left and right
                var leftSpaceIndex = text.LastIndexOf(" ", centerIndex, centerIndex, StringComparison.CurrentCulture);
                var rightSpaceIndex = text.IndexOf(" ", centerIndex, StringComparison.CurrentCulture);

                if (leftSpaceIndex == -1
                    && rightSpaceIndex == -1)
                {
                    this.textRun.Text = this.Text;
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