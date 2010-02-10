#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents backstage tab item
    /// </summary>
    public class BackstageTabItem : ContentControl
    {
        #region Properties

        /// <summary>
        /// Dependency property for isSelected
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(BackstageTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(BackstageTabItem.OnIsSelectedChanged)));
        
        /// <summary>
        /// Gets or sets whether the tab is selected
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get
            {
                return (bool)base.GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets parent tab control
        /// </summary>
        internal Backstage TabControlParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as Backstage);
            }
        }

        /// <summary>
        /// Gets or sets tab items text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BackstageTabItem), new UIPropertyMetadata(""));



        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static BackstageTabItem()
        {
            StyleProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(typeof(BackstageTabItem)));
        }

        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = (d as FrameworkElement).Resources["BackstageTabItemStyle"] as Style;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageTabItem()
        {
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControlParent != null) if (TabControlParent.SelectedItem is RibbonTabItem)
                    (TabControlParent.SelectedItem as BackstageTabItem).IsSelected = false;
            IsSelected = true;
            e.Handled = true;
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Called when the System.Windows.Controls.ContentControl.Content property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the System.Windows.Controls.ContentControl.Content property.</param>
        /// <param name="newContent">The new value of the System.Windows.Controls.ContentControl.Content property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.IsSelected)
            {
                Backstage tabControlParent = this.TabControlParent;
                if (tabControlParent != null)
                {
                    tabControlParent.SelectedContent = newContent;
                }
            }
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.MouseLeftButtonDown routed event is raised on this element. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e"> The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (((e.Source == this) || !this.IsSelected))
            {
                if (TabControlParent != null) if (TabControlParent.SelectedItem is BackstageTabItem)
                        (TabControlParent.SelectedItem as BackstageTabItem).IsSelected = false;
               
                this.IsSelected = true;
            }
            e.Handled = true;
        }

        #endregion

        #region Private methods

        // Handles IsSelected changed
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackstageTabItem container = d as BackstageTabItem;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        /// <summary>
        /// Handles selected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(e);
        }

        /// <summary>
        /// Handles unselected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(e);
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Handles IsSelected changed
        /// </summary>
        /// <param name="e">The event data.</param>
        private void HandleIsSelectedChanged(RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        #endregion
    }
}
