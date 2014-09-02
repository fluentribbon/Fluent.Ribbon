#region Copyright and License Information

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
        #region Fields

        

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets width of right content
        /// </summary>
        public double RightPaneWidth
        {
            get { return (double)GetValue(RightPaneWidthProperty); }
            set { SetValue(RightPaneWidthProperty, value); }
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
            get { return (object)GetValue(RightPaneContentProperty); }
            set { SetValue(RightPaneContentProperty, value); }
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
            get { return (object)GetValue(FooterPaneContentProperty); }
            set { SetValue(FooterPaneContentProperty, value); }
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
            Type type = typeof (ApplicationMenu);

            // Override style metadata
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            // Disable QAT for this control
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false));
            // Make default KeyTip
            KeyTipProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, null, CoerceKeyTipKeys));
            StyleProperty.OverrideMetadata(typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(ApplicationMenu));
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
            CoerceValue(KeyTipProperty);
        }

        #endregion
       
        #region Methods


        #endregion

        #region Overrides
        /*/// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            Border buttonBorder = GetTemplateChild("PART_ButtonBorder") as Border;
            if (buttonBorder.IsMouseOver && IsDropDownOpen) IsDropDownOpen = false;
            else base.OnPreviewMouseLeftButtonDown(e);
        }
        */
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