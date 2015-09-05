﻿#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents Backstage tab control.
    /// </summary>
    public class BackstageTabControl : Selector
    {
        #region Properties

        // Dependency property key for SelectedContent
        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContent", typeof(object), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency property SelectedContent
        /// </summary>
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets content for selected tab
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SelectedContent
        {
            get
            {
                return this.GetValue(SelectedContentProperty);
            }
            internal set
            {
                this.SetValue(SelectedContentPropertyKey, value);
            }
        }

        /// <summary>
        /// Dependency property ContentStringFormat
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency property ContentTemplate
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency property ContentTemplateSelector
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        private static readonly DependencyPropertyKey SelectedContentStringFormatPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentStringFormat", typeof(string), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency property SelectedContentStringFormat
        /// </summary>
        public static readonly DependencyProperty SelectedContentStringFormatProperty = SelectedContentStringFormatPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentTemplate", typeof(DataTemplate), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency property SelectedContentTemplate
        /// </summary>
        public static readonly DependencyProperty SelectedContentTemplateProperty = SelectedContentTemplatePropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentTemplateSelector", typeof(DataTemplateSelector), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Dependency property SelectedContentTemplateSelector
        /// </summary>
        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = SelectedContentTemplateSelectorPropertyKey.DependencyProperty;

        /// <summary>
        /// Get or sets the string format for the content.
        /// </summary>
        public string ContentStringFormat
        {
            get
            {
                return (string)this.GetValue(ContentStringFormatProperty);
            }
            set
            {
                this.SetValue(ContentStringFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> which should be used for the content
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ContentTemplateProperty);
            }
            set
            {
                this.SetValue(ContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ContentTemplateSelector"/> which should be used for the content
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)this.GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                this.SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Get or sets the string format for the selected content.
        /// </summary>
        public string SelectedContentStringFormat
        {
            get
            {
                return (string)this.GetValue(SelectedContentStringFormatProperty);
            }
            internal set
            {
                this.SetValue(SelectedContentStringFormatPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> which should be used for the selected content
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate SelectedContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(SelectedContentTemplateProperty);
            }
            internal set
            {
                this.SetValue(SelectedContentTemplatePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ContentTemplateSelector"/> which should be used for the selected content
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)this.GetValue(SelectedContentTemplateSelectorProperty);
            }
            internal set
            {
                this.SetValue(SelectedContentTemplateSelectorPropertyKey, value);
            }
        }

        #region ItemsPanelBackground

        /// <summary>
        /// Gets or sets current Backround of the ItemsPanel
        /// </summary>
        public Brush ItemsPanelBackground
        {
            get { return (Brush)this.GetValue(ItemsPanelBackgroundProperty); }
            set { this.SetValue(ItemsPanelBackgroundProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Foreground.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static DependencyProperty ItemsPanelBackgroundProperty =
            DependencyProperty.Register("ItemsPanelBackground", typeof(Brush), typeof(BackstageTabControl));

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static BackstageTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(typeof(BackstageTabControl)));
            StyleProperty.OverrideMetadata(typeof(BackstageTabControl), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(BackstageTabControl));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BackstageTabControl()
        {
            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            // Fixed incoreect menu showing
            this.ContextMenu = new ContextMenu
            {
                Width = 0,
                Height = 0,
                HasDropShadow = false
            };
            this.ContextMenu.Opened += delegate { ContextMenu.IsOpen = false; };
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Raises the System.Windows.FrameworkElement.Initialized event. 
        /// This method is invoked whenever System.Windows.FrameworkElement.
        /// IsInitialized is set to true internally.
        /// </summary>
        /// <param name="e">The System.Windows.RoutedEventArgs that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.ItemContainerGenerator.StatusChanged += this.OnGeneratorStatusChanged;
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BackstageTabItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BackstageTabItem
                || item is Button
                || item is SeparatorTabItem
                || item is Separator;
        }

        /// <summary>
        /// Updates the current selection when an item in the System.Windows.Controls.Primitives.Selector has changed
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if ((e.Action == NotifyCollectionChangedAction.Remove) && (this.SelectedIndex == -1))
            {
                var startIndex = e.OldStartingIndex + 1;
                if (startIndex > this.Items.Count)
                {
                    startIndex = 0;
                }

                var item = this.FindNextTabItem(startIndex, -1);
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Called when the selection changes.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if (e.AddedItems.Count > 0)
            {
                this.UpdateSelectedContent();
            }
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled MouseLeftButtonDown routed event 
        /// is raised on this element. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            e.Handled = true;
        }

        #endregion

        #region Private methods

        // Gets selected ribbon tab item
        BackstageTabItem GetSelectedTabItem()
        {
            object selectedItem = this.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            BackstageTabItem item = selectedItem as BackstageTabItem;
            if (item == null)
            {
                item = this.FindNextTabItem(this.SelectedIndex, 1);
                this.SelectedItem = item;
            }
            return item;
        }

        // Finds next tab item
        private BackstageTabItem FindNextTabItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                int index = startIndex;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    index += direction;
                    if (index >= this.Items.Count)
                    {
                        index = 0;
                    }
                    else if (index < 0)
                    {
                        index = this.Items.Count - 1;
                    }
                    BackstageTabItem item2 = this.ItemContainerGenerator.ContainerFromIndex(index) as BackstageTabItem;
                    if (((item2 != null) && item2.IsEnabled) && (item2.Visibility == Visibility.Visible))
                    {
                        return item2;
                    }
                }
            }
            return null;
        }

        // Updates selected content
        private void UpdateSelectedContent()
        {
            if (this.SelectedIndex < 0)
            {

                this.SelectedContent = null;
            }
            else
            {
                BackstageTabItem selectedTabItem = this.GetSelectedTabItem();
                if (selectedTabItem != null)
                {
                    this.SelectedContent = selectedTabItem.Content;
                    if (((selectedTabItem.ContentTemplate != null) || (selectedTabItem.ContentTemplateSelector != null)) || (selectedTabItem.ContentStringFormat != null))
                    {
                        this.SelectedContentTemplate = selectedTabItem.ContentTemplate;
                        this.SelectedContentTemplateSelector = selectedTabItem.ContentTemplateSelector;
                        this.SelectedContentStringFormat = selectedTabItem.ContentStringFormat;
                    }
                    else
                    {
                        this.SelectedContentTemplate = this.ContentTemplate;
                        this.SelectedContentTemplateSelector = this.ContentTemplateSelector;
                        this.SelectedContentStringFormat = this.ContentStringFormat;
                    }

                    this.UpdateLayout();
                }
            }
        }

        #endregion

        #region Event handling

        // Handles GeneratorStatusChange
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (this.HasItems && (this.SelectedIndex == -1))
                {
                    this.SelectedIndex = 0;
                }
                this.UpdateSelectedContent();
            }
        }

        private void OnPopupDismiss(object sender, DismissPopupEventArgs e)
        {
            var backstage = LogicalTreeHelper.GetParent(this);

            if (backstage != null)
            {
                PopupService.RaiseDismissPopupEvent(backstage, DismissPopupMode.Always);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.AddHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)this.OnPopupDismiss);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.RemoveHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)this.OnPopupDismiss);
        }

        #endregion
    }
}