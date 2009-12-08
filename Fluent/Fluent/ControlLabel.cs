using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Fluent
{
    [TemplatePart(Name = "PART_TextRun", Type = typeof(Run))]
    [TemplatePart(Name = "PART_TextBlock", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Glyph", Type = typeof(InlineUIContainer))]
    public class ControlLabel: Control
    {
        #region Fields

        private Run textRun = null;
        private TextBlock textBlock = null;
        private InlineUIContainer glyph = null;

        #endregion

        #region Properties

        public bool HasTwoLines
        {
            get { return (bool)GetValue(HasTwoLinesProperty); }
            set { SetValue(HasTwoLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasTwoLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasTwoLinesProperty =
            DependencyProperty.Register("HasTwoLines", typeof(bool), typeof(ControlLabel), new UIPropertyMetadata(true,OnHasTwoLinesChanged));

        private static void OnHasTwoLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ControlLabel).UpdateTextRun();
        }

        public bool HasGlyph
        {
            get { return (bool)GetValue(HasGlyphProperty); }
            set { SetValue(HasGlyphProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasGlyph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasGlyphProperty =
            DependencyProperty.Register("HasGlyph", typeof(bool), typeof(ControlLabel), new UIPropertyMetadata(false, OnHasGlyphChanged));

        private static void OnHasGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ControlLabel).UpdateTextRun();
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ControlLabel), new UIPropertyMetadata("ControlLabel", OnTextChanged));

        #endregion

        #region Initialize

        static ControlLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ControlLabel), new FrameworkPropertyMetadata(typeof(ControlLabel)));
        }
        
        public ControlLabel()
        {
            this.Focusable = false;
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            textBlock = GetTemplateChild("PART_TextBlock") as TextBlock;
            textRun = GetTemplateChild("PART_TextRun") as Run;
            UpdateTextRun();

            glyph = GetTemplateChild("PART_Glyph") as InlineUIContainer;
        }

       /* protected override Size MeasureOverride(Size constraint)
        {
            if (textBlock == null) return base.MeasureOverride(constraint);
            textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double desiredWidth = textBlock.DesiredSize.Width;
            double desiredHeight = textBlock.DesiredSize.Height;
            if (HasTwoLines)
            {                
                Size size = new Size(desiredWidth / 2, double.PositiveInfinity);
                textBlock.Measure(size);                
                double height = textBlock.DesiredSize.Height;
                while ((height > desiredHeight * 2)/* || (size.Width < width)*///)
                /*{
                    size.Width ++;
                    textBlock.Measure(size);
                    height = textBlock.DesiredSize.Height;
                }
                desiredWidth = size.Width;
                desiredHeight = height;*/
/*
                    int i = 0;
                    do
                    {
                        // Set the TextBlock's width to half of its desired with to spread it
                        // to two lines, and re-run layout.
                        _textBlock.Width = Math.Max(LabelMinWidth, width / 2.0) + (arrowWidth * i++);
                        _textBlock.Measure(infinity);
                        _textBlock.Arrange(new Rect(_textBlock.DesiredSize));

                        // Expand the TextBlock's width to keep from clipping text when we
                        // naively resized to DesiredWidth / 2.
                        TextPointer tp = _textBlock.ContentStart.GetLineStartPosition(1);
                        if (tp != null)
                        {
                            Rect r = tp.GetCharacterRect(LogicalDirection.Backward);
                            Rect r2 = _textBlock.ContentEnd.GetCharacterRect(LogicalDirection.Backward);
                            _textBlock.Width = Math.Max(r.X, r2.X);
                            _textBlock.Measure(infinity);
                        }

                        // Continue to expand the width of the label while its height exceeds
                        // the allowable maximum height.
                        height = _textBlock.DesiredSize.Height;
                    } while (height > TwoLineLabel.LabelMaxHeight);
                    */
                /*textBlock.Measure(new Size(desiredWidth, desiredHeight * 2));
                desiredWidth = textBlock.DesiredSize.Width;
                desiredHeight = textBlock.DesiredSize.Height;   */             
           /* }
            return new Size(desiredWidth, desiredHeight);
        }*/

        #endregion

        #region Event handling

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ControlLabel label = d as ControlLabel;
            label.UpdateTextRun();
        }

        #endregion

        #region Private methods

        void UpdateTextRun()
        {
            if (textRun != null)
            {
                textRun.Text = Text;
                if (HasTwoLines)
                {
                    int centerIndex = Text.Length / 2;
                    int leftSpaceIndex = Text.LastIndexOf(' ', centerIndex, centerIndex);
                    int rightSpaceIndex = Text.IndexOf(" ", centerIndex);
                    if ((leftSpaceIndex == -1) && (rightSpaceIndex == -1))
                    {
                        textRun.Text += '\u0085';
                    }
                    else if (leftSpaceIndex == -1)
                    {
                        textRun.Text = textRun.Text.Remove(rightSpaceIndex, 1);
                        textRun.Text = textRun.Text.Insert(rightSpaceIndex, "\u0085");
                    }
                    else if (rightSpaceIndex == -1)
                    {
                        textRun.Text = textRun.Text.Remove(leftSpaceIndex, 1);
                        textRun.Text = textRun.Text.Insert(leftSpaceIndex, "\u0085");
                    }
                    else
                    {
                        if (Math.Abs(centerIndex - leftSpaceIndex) < Math.Abs(centerIndex - rightSpaceIndex))
                        {
                            textRun.Text = textRun.Text.Remove(leftSpaceIndex, 1);
                            textRun.Text=textRun.Text.Insert(leftSpaceIndex, "\u0085");
                        }
                        else
                        {
                            textRun.Text=textRun.Text.Remove(rightSpaceIndex, 1);
                            textRun.Text=textRun.Text.Insert(rightSpaceIndex, "\u0085");
                        }
                    }
                }
            }
        }

        #endregion
    }
}
