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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents backstage button
    /// </summary>
    [ContentProperty("Content")]
    public class ApplicationMenu : RibbonControl
    {
        #region Fields

        

        #endregion

        #region Properties

        #region IsOpen

        /// <summary>
        /// Gets or sets whether application menu is shown
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool),
            typeof(ApplicationMenu), new UIPropertyMetadata(false, OnIsOpenChanged));

        static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO: show popup?
        }

        #endregion

        #region Content

        /// <summary>
        /// Gets or sets content of the application menu
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Content.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(ApplicationMenu),
            new UIPropertyMetadata(null, OnContentChanged));

        static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationMenu backstage = (ApplicationMenu)d;
            if (e.OldValue != null) backstage.RemoveLogicalChild(e.OldValue);
            if (e.NewValue != null) backstage.AddLogicalChild(e.NewValue);
        }

        #endregion

        #region LogicalChildren

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (Content != null) list.Add(Content);
                return list.GetEnumerator();
            }
        }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ApplicationMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ApplicationMenu), new FrameworkPropertyMetadata(typeof(ApplicationMenu)));
            // Disable QAT for this control
            CanAddToQuickAccessToolBarProperty.OverrideMetadata(typeof(ApplicationMenu), new FrameworkPropertyMetadata(false));
            // Make default header
            HeaderProperty.OverrideMetadata(typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, null, CoerceHeader));
            KeyTip.KeysProperty.AddOwner(typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, null, CoerceKeyTipKeys));
        }

        static object CoerceHeader(DependencyObject d, object basevalue)
        {
            return basevalue;// ?? Ribbon.Localization.BackstageButtonText;
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
            CoerceValue(HeaderProperty);
            CoerceValue(KeyTip.KeysProperty);
        }

        #endregion

        #region Methods

        // Handles click event
        void Click()
        {
            IsOpen = !IsOpen;
        }

        #region Show / Hide
        

        #endregion

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data.
        ///  The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            Click();
        }

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public override void OnKeyTipPressed()
        {
            Click();
            base.OnKeyTipPressed();
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
