using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    public class MenuItem: Control
    {
        #region Properies

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(MenuItem), new UIPropertyMetadata(null));

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(MenuItem), new UIPropertyMetadata(null));

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            private set { SetValue(IsPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(MenuItem), new UIPropertyMetadata(false));

        #endregion

        #region Events

        public event RoutedEventHandler Click;

        #endregion

        #region Constructor

        static MenuItem()
        {
            
        }

        public MenuItem()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
        }

        #endregion

        #region Overrides

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            IsPressed = true;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            IsPressed = false;
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            e.Handled = true;
        }

        #endregion

        #region Event Handling

        private void OnClick(object sender, RoutedEventArgs e)
        {
            OnClick(e);
        }

        protected void OnClick(RoutedEventArgs e)
        {
            if (Click != null) Click(this, e);
            e.Handled = true;
            // Close menu
            ExtendedPopup popup = FindParentPopup();
            if(popup!=null) popup.IsOpen = false;
        }

        #endregion

        #region Private methods

        private ExtendedPopup FindParentPopup()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                if (element is ExtendedPopup) return element as ExtendedPopup;
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element as DependencyObject);
                if (parent != null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element as DependencyObject);
            }
            return null;
        }

        #endregion
    }
}
