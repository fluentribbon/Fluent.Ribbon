using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Fluent
{
    public class BackstageTabItem : ContentControl
    {
        #region Properties

        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(BackstageTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(BackstageTabItem.OnIsSelectedChanged)));
        
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

        internal Backstage TabControlParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as Backstage);
            }
        }



        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BackstageTabItem), new UIPropertyMetadata(""));



        #endregion

        #region Constructors

        static BackstageTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageTabItem), new FrameworkPropertyMetadata(typeof(BackstageTabItem)));
        }

        public BackstageTabItem()
        {
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControlParent != null) if (TabControlParent.SelectedItem is RibbonTabItem)
                    (TabControlParent.SelectedItem as BackstageTabItem).IsSelected = false;
            IsSelected = true;
        }

        #endregion

        #region Overrides

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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (((e.Source == this) || !this.IsSelected))
            {
                if (TabControlParent != null) if (TabControlParent.SelectedItem is BackstageTabItem)
                        (TabControlParent.SelectedItem as BackstageTabItem).IsSelected = false;
                e.Handled = true;
                this.IsSelected = true;
            }
            //base.OnMouseLeftButtonDown(e);
        }

        #endregion

        #region Private methods

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BackstageTabItem container = d as BackstageTabItem;
            bool newValue = (bool)e.NewValue;
            Backstage tabControlParent = container.TabControlParent;

            if (newValue)
            {
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
        }

        protected virtual void OnSelected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(true, e);
        }

        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(false, e);
        }

        #endregion

        #region Event handling

        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        #endregion
    }
}
