using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    [ContentProperty("PopupContent")]
    public class DropDownButton: Control
    {
        #region Fields

        private ExtendedPopup popup = null;

        #endregion

        #region Properties

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DropDownButton), new UIPropertyMetadata(""));


        public ImageSource SmallIcon
        {
            get { return (ImageSource)GetValue(SmallIconProperty); }
            set { SetValue(SmallIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallIconProperty =
            DependencyProperty.Register("SmallIcon", typeof(ImageSource), typeof(DropDownButton), new UIPropertyMetadata(null));

        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(DropDownButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Есть ли треугольник
        /// </summary>
        public bool HasTriangle
        {
            get { return (bool) GetValue(HasTriangleProperty); }
            set { SetValue(HasTriangleProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(false,OnIsOpenChanged));

        public object PopupContent
        {
            get { return (object)GetValue(PopupContentProperty); }
            set { SetValue(PopupContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PopupContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PopupContentProperty =
            DependencyProperty.Register("PopupContent", typeof(object), typeof(DropDownButton), new UIPropertyMetadata(null));


        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(popup);
                return list.GetEnumerator();
            }
        }

        #endregion
       
        #region Dependency properties
        
        /// <summary>
        /// Есть ли треугольник в низ
        /// </summary>
        public static readonly DependencyProperty HasTriangleProperty =
            DependencyProperty.Register(
                "HasTriangle", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        #endregion

        #region Initialize

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
                                                     new FrameworkPropertyMetadata(typeof(DropDownButton)));                       


        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public DropDownButton()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }

        #endregion

        #region Overrides

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((popup != null)&&(!popup.IsOpen))
            {
                /*popup.StaysOpen = true;
                popup.PlacementTarget = this;
                popup.Placement = PlacementMode.Bottom;*/
                popup.IsOpen = !popup.IsOpen;
                if (IsOpen) Mouse.Capture(popup, CaptureMode.Element);
                e.Handled = true;
            }
        }

        public override void OnApplyTemplate()
        {
            if (popup != null) RemoveLogicalChild(popup);
            popup = GetTemplateChild("PART_Popup") as ExtendedPopup;
            if(popup!=null)
            {
                if (popup.Parent != null) (popup.Parent as Panel).Children.Remove(popup);
                AddLogicalChild(popup);
                Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                popup.SetBinding(Popup.IsOpenProperty, binding);
            }
        }

        #endregion

        #region Private methods

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton ribbon = (DropDownButton)d;

            if (ribbon.IsOpen)
            {                
                ribbon.IsHitTestVisible = false;
            }
            else
            {
                ribbon.IsHitTestVisible = true;
            }
        }

        #endregion
    }


}
