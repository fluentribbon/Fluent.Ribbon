using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents ribbon status bar
    /// </summary>
    public class StatusBar : System.Windows.Controls.Primitives.StatusBar
    {
        #region Fields

        // Context menu
        private readonly ContextMenu contextMenu = new ContextMenu();

        private Window ownerWindow;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets whether window is maximized
        /// </summary>
        public bool IsWindowMaximized
        {
            get { return (bool)GetValue(IsWindowMaximizedProperty); }
            set { SetValue(IsWindowMaximizedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsWindowMaximized.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsWindowMaximizedProperty =
            DependencyProperty.Register("IsWindowMaximized", typeof(bool), typeof(StatusBar), new UIPropertyMetadata(false));

#if NET45
        private object currentItem;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public StatusBar()
        {
            RecreateMenu();
            ContextMenu = contextMenu;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            this.ItemContainerGenerator.StatusChanged += HandleItemContainerGeneratorStatusChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (ownerWindow != null)
            {
                ownerWindow.StateChanged -= OnWindowStateChanged;
                ownerWindow = null;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (ownerWindow == null)
            {
                ownerWindow = Window.GetWindow(this);
            }

            if (ownerWindow != null)
            {
                ownerWindow.StateChanged += OnWindowStateChanged;
                if ((ownerWindow.ResizeMode == ResizeMode.CanResizeWithGrip) && (ownerWindow.WindowState == WindowState.Maximized))
                {
                    IsWindowMaximized = true;
                }
                else
                {
                    IsWindowMaximized = false;
                }
            }
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            if ((ownerWindow.ResizeMode == ResizeMode.CanResizeWithGrip) && (ownerWindow.WindowState == WindowState.Maximized))
            {
                IsWindowMaximized = true;
            }
            else
            {
                IsWindowMaximized = false;
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
#if NET45
            var item = this.currentItem;
            this.currentItem = null;

            if (this.UsesItemContainerTemplate
                && item != null)
            {
                var dataTemplate = this.ItemContainerTemplateSelector.SelectTemplate(item, this);
                if (dataTemplate != null)
                {
                    var dataTemplateContent = (object)dataTemplate.LoadContent();
                    if (dataTemplateContent is StatusBarItem
                        || dataTemplateContent is Separator)
                    {
                        return dataTemplateContent as DependencyObject;
                    }

                    throw new InvalidOperationException("Invalid ItemContainer");
                }
            }
#endif
            return new StatusBarItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var isItemItsOwnContainerOverride = item is StatusBarItem || item is Separator;

#if NET45
            if (isItemItsOwnContainerOverride == false)
            {
                this.currentItem = item;
            }
#endif

            return isItemItsOwnContainerOverride;
        }

        private void HandleItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Delegate)new Action(RecreateMenu));
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var container = this.ItemContainerGenerator.ContainerFromItem(e.NewItems[i]);
                            var item = container as StatusBarItem;

                            if (item != null)
                            {
                                item.Checked += OnItemChecked;
                                item.Unchecked += OnItemUnchecked;
                                contextMenu.Items.Insert(this.ItemContainerGenerator.IndexFromContainer(container), new StatusBarMenuItem(item));
                            }
                            else
                            {
                                contextMenu.Items.Insert(this.ItemContainerGenerator.IndexFromContainer(container), new Separator());
                            }
                        }
                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var menuItem = contextMenu.Items[e.OldStartingIndex + 1];
                            contextMenu.Items.Remove(e.OldStartingIndex + 1);
                            contextMenu.Items.Insert(e.NewStartingIndex + i + 1, menuItem);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var menuItem = contextMenu.Items[e.OldStartingIndex + 1] as StatusBarMenuItem;
                            if (menuItem != null)
                            {
                                menuItem.StatusBarItem.Checked += OnItemChecked;
                                menuItem.StatusBarItem.Unchecked += OnItemUnchecked;
                            }
                            contextMenu.Items.RemoveAt(e.OldStartingIndex + 1);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            var menuItem = contextMenu.Items[e.OldStartingIndex + 1] as StatusBarMenuItem;
                            if (menuItem != null)
                            {
                                menuItem.StatusBarItem.Checked += OnItemChecked;
                                menuItem.StatusBarItem.Unchecked += OnItemUnchecked;
                            }

                            contextMenu.Items.RemoveAt(e.OldStartingIndex + 1);
                        }

                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var item = this.ItemContainerGenerator.ContainerFromItem(e.NewItems[i]) as StatusBarItem;
                            if (item != null)
                            {
                                item.Checked += OnItemChecked;
                                item.Unchecked += OnItemUnchecked;
                                contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new StatusBarMenuItem(item));
                            }
                            else
                            {
                                contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new Separator());
                            }
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        RecreateMenu();
                        break;
                    }
            }
        }

        private void OnItemUnchecked(object sender, RoutedEventArgs e)
        {
            UpdateSeparartorsVisibility();
        }

        private void OnItemChecked(object sender, RoutedEventArgs e)
        {
            UpdateSeparartorsVisibility();
        }

        #endregion

        #region Private Methods

        // Creates menu
        private void RecreateMenu()
        {
            contextMenu.Items.Clear();

            // Adding header separator
            contextMenu.Items.Add(new GroupSeparatorMenuItem());
            RibbonControl.Bind(Ribbon.Localization, contextMenu.Items[0] as FrameworkElement, "CustomizeStatusBar", HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);

            for (var i = 0; i < this.Items.Count; i++)
            {
                var item = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) as StatusBarItem;
                if (item != null)
                {
                    item.Checked += OnItemChecked;
                    item.Unchecked += OnItemUnchecked;
                    contextMenu.Items.Add(new StatusBarMenuItem(item));
                }
                else
                {
                    contextMenu.Items.Add(new Separator());
                }
            }

            UpdateSeparartorsVisibility();
        }

        // Updates separators visibility, to not duplicate
        private void UpdateSeparartorsVisibility()
        {
            var isPrevSeparator = false;
            var isFirstVsible = true;

            for (var i = 0; i < this.Items.Count; i++)
            {
                var item = this.ItemContainerGenerator.ContainerFromItem(this.Items[i]);
                var separator = item as Separator;

                if (separator != null)
                {
                    if (isPrevSeparator || isFirstVsible)
                    {
                        separator.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        separator.Visibility = Visibility.Visible;
                    }

                    isPrevSeparator = true;
                    isFirstVsible = false;
                }
                else if (item is StatusBarItem)
                {
                    if ((item as StatusBarItem).Visibility == Visibility.Visible)
                    {
                        isPrevSeparator = false;
                        isFirstVsible = false;
                    }
                }
            }
        }

        #endregion
    }
}