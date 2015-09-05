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

namespace Fluent
{
    /// <summary>
    /// Represents backstage button
    /// </summary>
    public class ApplicationMenu : DropDownButton
    {
        #region Properties

        /// <summary>
        /// Gets or sets width of right content
        /// </summary>
        public double RightPaneWidth
        {
            get { return (double)this.GetValue(RightPaneWidthProperty); }
            set { this.SetValue(RightPaneWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RightContentWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightPaneWidthProperty =
            DependencyProperty.Register("RightPaneWidth", typeof(double), typeof(ApplicationMenu), new UIPropertyMetadata(300.0));        

        /// <summary>
        /// Gets or sets application menu right pane content
        /// </summary>
        public object RightPaneContent
        {
            get { return this.GetValue(RightPaneContentProperty); }
            set { this.SetValue(RightPaneContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RightContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RightPaneContentProperty =
            DependencyProperty.Register("RightPaneContent", typeof(object), typeof(ApplicationMenu), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets application menu bottom pane content
        /// </summary>
        public object FooterPaneContent
        {
            get { return this.GetValue(FooterPaneContentProperty); }
            set { this.SetValue(FooterPaneContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BottomContent.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty FooterPaneContentProperty =
            DependencyProperty.Register("FooterPaneContent", typeof(object), typeof(ApplicationMenu), new UIPropertyMetadata(null));
        
        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ApplicationMenu()
        {
            var type = typeof (ApplicationMenu);

            // Override style metadata
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            // Disable QAT for this control
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false));
            // Make default KeyTip
            KeyTipProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, null, CoerceKeyTipKeys));
            StyleProperty.OverrideMetadata(typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, OnCoerceStyle));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = ((FrameworkElement) d).TryFindResource(typeof(ApplicationMenu));
            }

            return basevalue;
        }
        
        static object CoerceKeyTipKeys(DependencyObject d, object basevalue)
        {
            return basevalue ?? Ribbon.Localization.BackstageButtonKeyTip;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationMenu()
        {
            this.CoerceValue(KeyTipProperty);
        }

        #endregion
       
        #region Quick Access Toolbar

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}