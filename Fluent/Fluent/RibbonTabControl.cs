using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    [TemplatePart(Name = "PART_SelectedContentHost", Type = typeof(ContentPresenter)), StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(RibbonTabItem))]
    public class RibbonTabControl: Selector
    {
        #region Constants
        
        private const string SelectedContentHostTemplateName = "PART_SelectedContentHost";

        #endregion

        #region Dependency propeties
        
        private static readonly DependencyPropertyKey SelectedContentPropertyKey = DependencyProperty.RegisterReadOnly("SelectedContent", typeof(object), typeof(RibbonTabControl), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty SelectedContentProperty = SelectedContentPropertyKey.DependencyProperty;

        #endregion

        #region Attrinutes

        #endregion

        #region Свойства

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

        #endregion

        #region Инициализация

        static RibbonTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabControl), new FrameworkPropertyMetadata(typeof(RibbonTabControl)));
        }                

        public RibbonTabControl()
        {

        }

        #endregion

        #region Overrides

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //base.CanSelectMultiple = false;
            base.ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTabItem();
        }

        /// <summary>
        /// On new style applying
        /// </summary>
        public override void OnApplyTemplate()
        {
            /*contentPresenter = FindName(SelectedContentHostTemplateName) as ContentPresenter;
            if (contentPresenter == null) throw new Exception("Incorrect control template.");*/
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is RibbonTabItem);
        }

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
                RibbonTabItem item = this.FindNextTabItem(startIndex, -1);
                if (item != null)
                {
                    item.IsSelected = true;
                }
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);            
            this.UpdateSelectedContent();
        }

        #endregion

        #region Private methods

        // Get selected ribbon tab item
        private RibbonTabItem GetSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            RibbonTabItem item = selectedItem as RibbonTabItem;
            if (item == null)
            {
                item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as RibbonTabItem;
            }
            return item;
        }

        private RibbonTabItem FindNextTabItem(int startIndex, int direction)
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
                    RibbonTabItem item2 = base.ItemContainerGenerator.ContainerFromIndex(index) as RibbonTabItem;
                    if (((item2 != null) && item2.IsEnabled) && (item2.Visibility == Visibility.Visible))
                    {
                        return item2;
                    }
                }
            }
            return null;
        }

        private void UpdateSelectedContent()
        {
            if (base.SelectedIndex < 0)
            {
                this.SelectedContent = null;
            }
            else
            {
                RibbonTabItem selectedTabItem = this.GetSelectedTabItem();
                if (selectedTabItem != null)
                {
                    FrameworkElement parent = VisualTreeHelper.GetParent(selectedTabItem) as FrameworkElement;
                    this.SelectedContent = selectedTabItem.Content;
                }
            }
        }

        #endregion

        #region Event handling

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (base.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                //if (base.HasItems && (base._selectedItems.Count == 0))
                if (base.HasItems && (base.SelectedIndex == -1))
                {
                    base.SelectedIndex = 0;
                }
                this.UpdateSelectedContent();
            }
        }

        #endregion
    }
}
