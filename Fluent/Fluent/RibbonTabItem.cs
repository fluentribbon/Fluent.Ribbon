using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Fluent
{
    public class RibbonTabItem:HeaderedContentControl
    {
        #region Dependency properties

        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(RibbonTabItem.OnIsSelectedChanged)));

        #endregion

        #region Fields

        #endregion

        #region Properties

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

        private RibbonTabControl TabControlParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as RibbonTabControl);
            }
        }



        public double Whitespace
        {
            get { return (double)GetValue(WhitespaceProperty); }
            set { SetValue(WhitespaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WhitespaceProperty =
            DependencyProperty.Register("Whitespace", typeof(double), typeof(RibbonTabItem), new UIPropertyMetadata((double)12.0));



        public bool IsSeparatorVisible
        {
            get { return (bool)GetValue(IsSeparatorVisibleProperty); }
            set { SetValue(IsSeparatorVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSeparatorVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSeparatorVisibleProperty =
            DependencyProperty.Register("IsSeparatorVisible", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));
        


        #endregion

        #region Events

        public event RoutedEventHandler TabChanged;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(typeof(RibbonTabItem)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabItem()
        {
            
        }

        #endregion

        #region Overrides

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.IsSelected)
            {
                RibbonTabControl tabControlParent = this.TabControlParent;
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
                if(TabControlParent.SelectedItem is RibbonTabItem)
                    (TabControlParent.SelectedItem as RibbonTabItem).IsSelected = false;
                e.Handled = true;
                this.IsSelected = true;
            }
            base.OnMouseLeftButtonDown(e);
        }

        #endregion

        #region Private methods

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabItem container = d as RibbonTabItem;
            bool newValue = (bool)e.NewValue;
            RibbonTabControl tabControlParent = container.TabControlParent;
            
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
