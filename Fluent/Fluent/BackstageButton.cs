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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents backstage button
    /// </summary>
    public class BackstageButton:RibbonControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets Backstage
        /// </summary>
        public Backstage Backstage
        {
            get { return (Backstage)GetValue(BackstageProperty); }
            set { SetValue(BackstageProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Backstage.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageProperty =
            DependencyProperty.Register("Backstage", typeof(object), typeof(BackstageButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets whether backstage is shown
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BackstageButton), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(Backstage);
                return list.GetEnumerator();
            }
        }

        #endregion
       
        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static BackstageButton()
        {
            //StyleProperty.OverrideMetadata(typeof(BackstageButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageButton),
                                                     new FrameworkPropertyMetadata(typeof(BackstageButton)));                       
        }

        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = ThemesManager.DefaultBackstageButtonStyle;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageButton()
        {
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
            Backstage = new Backstage();
            Binding binding = new Binding("Background");
            binding.Source = this;
            Backstage.SetBinding(Backstage.BackgroundProperty, binding);
            AddLogicalChild(Backstage);
        }

        /// <summary>
        /// handles click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            IsOpen = !IsOpen;
            e.Handled = true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data.
        ///  The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(RibbonControl.ClickEvent,this));            
        }

        #endregion

        public override UIElement CreateQuickAccessItem()
        {
            throw new NotImplementedException();
        }

        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            throw new NotImplementedException();
        }
    }
}
