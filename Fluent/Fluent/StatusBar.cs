using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Fluent;

namespace Fluent
{
    /// <summary>
    /// Represents ribbon status bar
    /// </summary>
    public class StatusBar : System.Windows.Controls.Primitives.StatusBar
    {
        #region Fields

        // Context menu
        private Fluent.ContextMenu contextMenu = new Fluent.ContextMenu();

        private System.Windows.Window ownerWindow;

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
            if (ownerWindow == null) ownerWindow = Window.GetWindow(this);
            if (ownerWindow != null)
            {
                ownerWindow.StateChanged += OnWindowStateChanged;
                if ((ownerWindow.ResizeMode==ResizeMode.CanResizeWithGrip) && (ownerWindow.WindowState == WindowState.Maximized)) IsWindowMaximized = true;
                else IsWindowMaximized = false;
            }
        }

        private void OnWindowStateChanged(object sender, EventArgs e)
        {
            if ((ownerWindow.ResizeMode==ResizeMode.CanResizeWithGrip) && (ownerWindow.WindowState == WindowState.Maximized)) IsWindowMaximized = true;
            else IsWindowMaximized = false;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StatusBarItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is StatusBarItem) || (item is Separator);
        }

        /// <summary>
        /// Invoked when the <see cref="P:System.Windows.Controls.ItemsControl.Items"/> property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            switch(e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        for (int i = 0; i < e.NewItems.Count;i++ )
                        {
                            StatusBarItem item = e.NewItems[i] as StatusBarItem;
                            if (item != null)
                            {
                                item.Checked += OnItemChecked;
                                item.Unchecked += OnItemUnchecked;
                                contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new StatusBarMenuItem(item));
                            }
                            else contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new Separator());
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            object menuItem = contextMenu.Items[e.OldStartingIndex + 1];
                            contextMenu.Items.RemoveAt(e.OldStartingIndex + 1);
                            contextMenu.Items.Insert(e.NewStartingIndex + i + 1, menuItem);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            StatusBarMenuItem menuItem = contextMenu.Items[e.OldStartingIndex + 1] as StatusBarMenuItem;
                            if(menuItem!=null)
                            {
                                menuItem.StatusBarItem.Checked += OnItemChecked;
                                menuItem.StatusBarItem.Unchecked += OnItemUnchecked;
                            }
                            contextMenu.Items.RemoveAt(e.OldStartingIndex+1);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        for (int i = 0; i < e.OldItems.Count; i++)
                        {
                            StatusBarMenuItem menuItem = contextMenu.Items[e.OldStartingIndex + 1] as StatusBarMenuItem;
                            if (menuItem != null)
                            {
                                menuItem.StatusBarItem.Checked += OnItemChecked;
                                menuItem.StatusBarItem.Unchecked += OnItemUnchecked;
                            }
                            contextMenu.Items.RemoveAt(e.OldStartingIndex + 1);
                        }
                        for (int i = 0; i < e.NewItems.Count; i++)
                        {
                            StatusBarItem item = e.NewItems[i] as StatusBarItem;
                            if (item != null)
                            {
                                item.Checked += OnItemChecked;
                                item.Unchecked += OnItemUnchecked;
                                contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new StatusBarMenuItem(item));
                            }
                            else contextMenu.Items.Insert(e.NewStartingIndex + i + 1, new Separator());
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
            RibbonControl.Bind(Ribbon.Localization, contextMenu.Items[0] as FrameworkElement, "CustomizeStatusBar", MenuItem.HeaderProperty, BindingMode.OneWay);            
            for(int i=0;i<Items.Count;i++)
            {
                StatusBarItem item = Items[i] as StatusBarItem;
                if (item != null)
                {
                    item.Checked += OnItemChecked;
                    item.Unchecked += OnItemUnchecked;
                    contextMenu.Items.Add(new StatusBarMenuItem(item));
                }
                else contextMenu.Items.Add(new Separator());
            }

            UpdateSeparartorsVisibility();
        }

        // Updates separators visibility, to not duplicate
        private void UpdateSeparartorsVisibility()
        {
            bool isPrevSeparator = false;
            bool isFirstVsible = true;
            for (int i = 0; i < Items.Count; i++)
            {
                Separator item = Items[i] as Separator;
                if (item != null)
                {
                    if (isPrevSeparator || isFirstVsible) item.Visibility = Visibility.Collapsed;
                    else item.Visibility = Visibility.Visible;
                    isPrevSeparator = true;
                    isFirstVsible = false;
                }
                else if (Items[i] is StatusBarItem) if ((Items[i] as StatusBarItem).Visibility == Visibility.Visible)
                {
                    isPrevSeparator = false;
                    isFirstVsible = false;
                }
            }
        }

        #endregion
    }
}
