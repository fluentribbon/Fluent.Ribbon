// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using Fluent.Localization;

    /// <summary>
    /// Represents ribbon status bar
    /// </summary>
    public class StatusBar : System.Windows.Controls.Primitives.StatusBar
    {
        #region Fields

        // Context menu
        private readonly ContextMenu contextMenu = new ContextMenu();

        private bool waitingForItemContainerGenerator;

        #endregion

        #region Properties

        private object currentItem;

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
            this.RecreateMenu();
            this.ContextMenu = this.contextMenu;

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            this.ItemContainerGenerator.StatusChanged += this.HandleItemContainerGeneratorStatusChanged;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
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
                        return (DependencyObject)dataTemplateContent;
                    }

                    throw new InvalidOperationException("Invalid ItemContainer");
                }
            }

            return new StatusBarItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            var isItemItsOwnContainerOverride = item is StatusBarItem || item is Separator;

            if (isItemItsOwnContainerOverride == false)
            {
                this.currentItem = item;
            }

            return isItemItsOwnContainerOverride;
        }

        private void HandleItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            this.RunInDispatcherAsync(this.RecreateMenu, DispatcherPriority.Loaded);
        }

        /// <inheritdoc />
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (this.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated
                || this.waitingForItemContainerGenerator)
            {
                this.waitingForItemContainerGenerator = true;
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    foreach (var newItem in e.NewItems)
                    {
                        var container = this.ItemContainerGenerator.ContainerFromItem(newItem);
                        var containerIndex = this.ItemContainerGenerator.IndexFromContainer(container);

                        if (container is StatusBarItem item)
                        {
                            item.Checked += this.OnItemChecked;
                            item.Unchecked += this.OnItemUnchecked;
                            this.contextMenu.Items.Insert(containerIndex, new StatusBarMenuItem(item));
                        }
                        else
                        {
                            this.contextMenu.Items.Insert(containerIndex, new Separator());
                        }
                    }

                    break;
                }

                case NotifyCollectionChangedAction.Move:
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            var menuItem = this.contextMenu.Items[e.OldStartingIndex + 1];
                            this.contextMenu.Items.Remove(e.OldStartingIndex + 1);
                            this.contextMenu.Items.Insert(e.NewStartingIndex + i + 1, menuItem);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            if (this.contextMenu.Items[e.OldStartingIndex + 1] is StatusBarMenuItem menuItem)
                            {
                                menuItem.StatusBarItem.Checked += this.OnItemChecked;
                                menuItem.StatusBarItem.Unchecked += this.OnItemUnchecked;
                            }

                            this.contextMenu.Items.RemoveAt(e.OldStartingIndex + 1);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            if (this.contextMenu.Items[e.OldStartingIndex + 1] is StatusBarMenuItem menuItem)
                            {
                                menuItem.StatusBarItem.Checked += this.OnItemChecked;
                                menuItem.StatusBarItem.Unchecked += this.OnItemUnchecked;
                            }

                            this.contextMenu.Items.RemoveAt(e.OldStartingIndex + 1);
                        }

                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            if (this.ItemContainerGenerator.ContainerFromItem(e.NewItems[i]) is StatusBarItem item)
                            {
                                item.Checked += this.OnItemChecked;
                                item.Unchecked += this.OnItemUnchecked;
                                this.contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new StatusBarMenuItem(item));
                            }
                            else
                            {
                                this.contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new Separator());
                            }
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    {
                        this.RecreateMenu();
                        break;
                    }
            }
        }

        private void OnItemUnchecked(object sender, RoutedEventArgs e)
        {
            this.UpdateSeparartorsVisibility();
        }

        private void OnItemChecked(object sender, RoutedEventArgs e)
        {
            this.UpdateSeparartorsVisibility();
        }

        #endregion

        #region Private Methods

        // Creates menu
        private void RecreateMenu()
        {
            this.contextMenu.Items.Clear();

            // Adding header separator
            this.contextMenu.Items.Add(new GroupSeparatorMenuItem());
            RibbonControl.Bind(RibbonLocalization.Current.Localization, this.contextMenu.Items[0] as FrameworkElement, nameof(RibbonLocalizationBase.CustomizeStatusBar), HeaderedItemsControl.HeaderProperty, BindingMode.OneWay);

            for (var i = 0; i < this.Items.Count; i++)
            {
                if (this.ItemContainerGenerator.ContainerFromItem(this.Items[i]) is StatusBarItem item)
                {
                    item.Checked += this.OnItemChecked;
                    item.Unchecked += this.OnItemUnchecked;
                    this.contextMenu.Items.Add(new StatusBarMenuItem(item));
                }
                else
                {
                    this.contextMenu.Items.Add(new Separator());
                }
            }

            this.UpdateSeparartorsVisibility();

            this.waitingForItemContainerGenerator = false;
        }

        // Updates separators visibility, to not duplicate
        private void UpdateSeparartorsVisibility()
        {
            var isPrevSeparator = false;
            var isFirstVsible = true;

            foreach (var item in this.Items)
            {
                var containerFromItem = this.ItemContainerGenerator.ContainerFromItem(item);

                if (containerFromItem is Separator separator)
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
                else
                {
                    if ((containerFromItem as StatusBarItem)?.Visibility == Visibility.Visible)
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