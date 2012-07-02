#region Copyright and License Information
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
        static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContent", typeof(object), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));

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
                return base.GetValue(SelectedContentProperty);
            }
            internal set
            {
                base.SetValue(SelectedContentPropertyKey, value);
            }
        }

        public static readonly DependencyProperty ContentStringFormatProperty = DependencyProperty.Register("ContentStringFormat", typeof(string), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ContentTemplateSelectorProperty = DependencyProperty.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));


        private static readonly DependencyPropertyKey SelectedContentStringFormatPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentStringFormat", typeof(string), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedContentStringFormatProperty = SelectedContentStringFormatPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplatePropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentTemplate", typeof(DataTemplate), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedContentTemplateProperty = SelectedContentTemplatePropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey SelectedContentTemplateSelectorPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContentTemplateSelector", typeof(DataTemplateSelector), typeof(BackstageTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty = SelectedContentTemplateSelectorPropertyKey.DependencyProperty;

        public string ContentStringFormat
        {
            get
            {
                return (string)base.GetValue(ContentStringFormatProperty);
            }
            set
            {
                base.SetValue(ContentStringFormatProperty, value);
            }
        }

        public DataTemplate ContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(ContentTemplateProperty);
            }
            set
            {
                base.SetValue(ContentTemplateProperty, value);
            }
        }

        public DataTemplateSelector ContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(ContentTemplateSelectorProperty);
            }
            set
            {
                base.SetValue(ContentTemplateSelectorProperty, value);
            }
        }

        public string SelectedContentStringFormat
        {
            get
            {
                return (string)base.GetValue(SelectedContentStringFormatProperty);
            }
            internal set
            {
                base.SetValue(SelectedContentStringFormatPropertyKey, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplate SelectedContentTemplate
        {
            get
            {
                return (DataTemplate)base.GetValue(SelectedContentTemplateProperty);
            }
            internal set
            {
                base.SetValue(SelectedContentTemplatePropertyKey, value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)base.GetValue(SelectedContentTemplateSelectorProperty);
            }
            internal set
            {
                base.SetValue(SelectedContentTemplateSelectorPropertyKey, value);
            }
        }

        #region ItemsPanelBackground

        /// <summary>
        /// Gets or sets current Backround of the ItemsPanel
        /// </summary>
        public Brush ItemsPanelBackground
        {
            get { return (Brush)GetValue(ItemsPanelBackgroundProperty); }
            set { SetValue(ItemsPanelBackgroundProperty, value); }
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
            ContextMenu = new ContextMenu
            {
                Width = 0,
                Height = 0,
                HasDropShadow = false
            };
            ContextMenu.Opened += delegate { ContextMenu.IsOpen = false; };
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
            base.ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
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
            return ((item is BackstageTabItem) || (item is Button) || (item is SeparatorTabItem));
        }

        /// <summary>
        /// Updates the current selection when an item in the System.Windows.Controls.Primitives.Selector has changed
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if ((e.Action == NotifyCollectionChangedAction.Remove) && (base.SelectedIndex == -1))
            {
                int startIndex = e.OldStartingIndex + 1;
                if (startIndex > base.Items.Count)
                {
                    startIndex = 0;
                }
                BackstageTabItem item = FindNextTabItem(startIndex, -1);
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
                UpdateSelectedContent();
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
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            BackstageTabItem item = selectedItem as BackstageTabItem;
            if (item == null)
            {
                item = FindNextTabItem(base.SelectedIndex, 1);
                base.SelectedItem = item;
            }
            return item;
        }

        // Finds next tab item
        private BackstageTabItem FindNextTabItem(int startIndex, int direction)
        {
            if (direction != 0)
            {
                int index = startIndex;
                for (int i = 0; i < base.Items.Count; i++)
                {
                    index += direction;
                    if (index >= base.Items.Count)
                    {
                        index = 0;
                    }
                    else if (index < 0)
                    {
                        index = base.Items.Count - 1;
                    }
                    BackstageTabItem item2 = base.ItemContainerGenerator.ContainerFromIndex(index) as BackstageTabItem;
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
            if (base.SelectedIndex < 0)
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

                    UpdateLayout();
                }
            }
        }

        #endregion

        #region Event handling

        // Handles GeneratorStatusChange
        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (base.HasItems && (base.SelectedIndex == -1))
                {
                    base.SelectedIndex = 0;
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
            AddHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)OnPopupDismiss);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            RemoveHandler(PopupService.DismissPopupEvent, (DismissPopupEventHandler)OnPopupDismiss);
        }

        #endregion
    }
}
