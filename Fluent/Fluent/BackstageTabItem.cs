#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents backstage tab item
    /// </summary>
    public class BackstageTabItem : ContentControl, IKeyTipedControl
    {
        #region Properties

        /// <summary>
        /// Dependency property for isSelected
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(BackstageTabItem),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.Journal |
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                OnIsSelectedChanged));

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
        internal BackstageTabControl TabControlParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as BackstageTabControl);
            }
        }

        /// <summary>
        /// Gets or sets tab items text
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object),
            typeof(BackstageTabItem), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static BackstageTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabItem),
                new FrameworkPropertyMetadata(typeof(BackstageTabItem)));
            StyleProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(BackstageTabItem));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageTabItem()
        {

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
            if (IsSelected && TabControlParent != null)
            {
                TabControlParent.SelectedContent = newContent;
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
            if (((e.Source == this) || !IsSelected))
            {
                if (TabControlParent != null && TabControlParent.SelectedItem is BackstageTabItem)
                    ((BackstageTabItem)TabControlParent.SelectedItem).IsSelected = false;

                IsSelected = true;
            }
            e.Handled = true;
        }

        #endregion

        #region Private methods

        // Handles IsSelected changed
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackstageTabItem container = (BackstageTabItem)d;
            bool newValue = (bool)e.NewValue;

            if (newValue)
            {
                BackstageTabControl backstage = container.Parent as BackstageTabControl;
                if ((backstage != null) && (backstage.SelectedItem != container))
                {
                    if (backstage.SelectedItem is BackstageTabItem) (backstage.SelectedItem as BackstageTabItem).IsSelected = false;
                    backstage.SelectedItem = container;
                }
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
            HandleIsSelectedChanged(e);
        }

        /// <summary>
        /// Handles unselected event
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            HandleIsSelectedChanged(e);
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Handles IsSelected changed
        /// </summary>
        /// <param name="e">The event data.</param>
        private void HandleIsSelectedChanged(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        #endregion

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            if (TabControlParent != null && TabControlParent.SelectedItem is RibbonTabItem)
                ((BackstageTabItem)TabControlParent.SelectedItem).IsSelected = false;

            IsSelected = true;
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
        }
    }
}
