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
    /// Represents custom Fluent UI TextBox
    /// </summary>
    [TemplatePart(Name="PART_TextBox")]
    public class TextBox: RibbonControl
    {
        #region Events

        /// <summary>
        /// Occures when context menu is opened
        /// </summary>
        public event TextChangedEventHandler CurrentTextChanged;

        #endregion

        #region Fields

        // TextBox in template
        System.Windows.Controls.TextBox textBox;
        
        #endregion

        #region Properties

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
        
        #region CurrentText

        /// <summary>
        /// Gets or sets text in TextBox
        /// </summary>
        public string CurrentText
        {
            get { return (string)GetValue(CurrentTextProperty); }
            set { SetValue(CurrentTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentText.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentTextProperty =
            DependencyProperty.Register("CurrentText", typeof(string), typeof(TextBox), 
            new UIPropertyMetadata(""));

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
                
            }
            textBox = GetTemplateChild("PART_TextBox") as System.Windows.Controls.TextBox;
            if(textBox!=null)
            {
               
                textBox.Text = CurrentText;
            }
        }
        
        #endregion

        #region Private methods


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
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            TextBox textBox = (TextBox)element;

            textBox.Width = Width;
            textBox.InputWidth = InputWidth;
            
            Bind(this, textBox, "CurrentText", CurrentTextProperty, BindingMode.TwoWay);
            Bind(this, textBox, "IsReadOnly", IsReadOnlyProperty, BindingMode.OneWay);

            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
