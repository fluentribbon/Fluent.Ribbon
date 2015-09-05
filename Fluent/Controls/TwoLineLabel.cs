﻿#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Fluent
{
    /// <summary>
    /// Represents specific label to use in particular ribbon controls
    /// </summary>
    [TemplatePart(Name = "PART_TextRun", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_TextRun2", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_Glyph", Type = typeof(InlineUIContainer))]
    public class TwoLineLabel: Control
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
            DependencyProperty.Register("HasTwoLines", typeof(bool), typeof(TwoLineLabel), new UIPropertyMetadata(true,OnHasTwoLinesChanged));

        /// <summary>
        /// Handles HasTwoLines property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnHasTwoLinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TwoLineLabel).UpdateTextRun();
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
            DependencyProperty.Register("HasGlyph", typeof(bool), typeof(TwoLineLabel), new UIPropertyMetadata(false, OnHasGlyphChanged));

        /// <summary>
        /// Handles HasGlyph property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnHasGlyphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as TwoLineLabel).UpdateTextRun();
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
            DependencyProperty.Register("Text", typeof(string), typeof(TwoLineLabel), new UIPropertyMetadata("TwoLineLabel", OnTextChanged));

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static TwoLineLabel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(typeof(TwoLineLabel)));
            StyleProperty.OverrideMetadata(typeof(TwoLineLabel), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(TwoLineLabel));
            }

            return basevalue;
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

        #endregion

        #region Event handling

        /// <summary>
        /// Handles text property changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TwoLineLabel label = d as TwoLineLabel;
            label.UpdateTextRun();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Updates text run adds newline if HasTwoLines == true
        /// </summary>
        void UpdateTextRun()
        {
            if ((this.textRun != null)&&(this.textRun2 != null)&&(this.Text != null))
            {
                this.textRun.Text = this.Text;
                this.textRun2.Text = "";
                string text = this.Text.Trim();
                if (this.HasTwoLines)
                {
                    // Find soft hyphen, break at its position and display a normal hyphen.
                    int hyphenIndex = text.IndexOf((char)173);
                    if (hyphenIndex >= 0)
                    {
                        this.textRun.Text = text.Substring(0, hyphenIndex) + "-";
                        this.textRun2.Text = text.Substring(hyphenIndex) + " ";
                    }
                    else
                    {
                        int centerIndex = this.Text.Length / 2;
                        // Find spaces nearest to center from left and right
                        int leftSpaceIndex = text.LastIndexOf(" ", centerIndex, centerIndex);
                        int rightSpaceIndex = text.IndexOf(" ", centerIndex, StringComparison.CurrentCulture);
                        if ((leftSpaceIndex == -1) && (rightSpaceIndex == -1))
                        {
                            // The text can`t be separated. Add new line for glyph
                            //textRun.Text += '\u0085';
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
            }
        }

        #endregion
    }
}
