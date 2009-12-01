using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Fluent
{
    public class DropDownButton: ContentControl
    {
        
        #region Properties

        /// <summary>
        /// Есть ли треугольник
        /// </summary>
        public bool HasTriangle
        {
            get { return (bool) GetValue(HasTriangleProperty); }
            set { SetValue(HasTriangleProperty, value); }
        }

        /// <summary>
        /// Выпадающий список
        /// </summary>
        public Popup Popup
        {
            get { return (Popup) GetValue(PopupProperty); }
            set { SetValue(PopupProperty, value); }
        }



        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(false,OnIsOpenChanged));



        #endregion
       
        #region Dependency properties
        
        /// <summary>
        /// Есть ли треугольник в низ
        /// </summary>
        public static readonly DependencyProperty HasTriangleProperty =
            DependencyProperty.Register(
                "HasTriangle", typeof(bool), typeof(DropDownButton), new UIPropertyMetadata(true));

        /// <summary>
        /// Выпадающий список
        /// </summary>
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register("Popup", typeof (Popup), typeof (DropDownButton),
                                        new UIPropertyMetadata(null,OnPopupChanged));

        #endregion

        #region Initialize

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton),
                                                     new FrameworkPropertyMetadata(typeof(DropDownButton)));

            EventManager.RegisterClassHandler(typeof(DropDownButton), Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
            EventManager.RegisterClassHandler(typeof(DropDownButton), Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
            
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public DropDownButton()
        {

        }

        #endregion

        #region Overrides

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Popup != null)
            {
                Popup.StaysOpen = true;
                Popup.PlacementTarget = this;
                Popup.Placement = PlacementMode.Bottom;
                //(Popup as ExtendedPopup).IngnoreFirstClose = true;
                Popup.IsOpen = !Popup.IsOpen;
                if (IsOpen) Mouse.Capture(Popup, CaptureMode.Element/*SubTree*/);
                //Mouse.Capture(this, CaptureMode.Element/*SubTree*/);
                e.Handled = true;
            }
            //base.OnMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            //base.OnMouseLeftButtonUp(e);
            /*if (Popup != null)
            {
                if (Mouse.Captured == this)
                {
                    Mouse.Capture(null);
                }
                e.Handled = true;
                IsOpen = true;
                //Popup.StaysOpen = false;
            }*/
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            /*if (Popup != null)
            {
                if (Mouse.Captured != this)
                {
                    UIElement selectedPopupChild = Popup.Child;
                    if (e.OriginalSource == this)
                    {
                        // If Ribbon loses capture because something outside popup is clicked - close the popup
                        if (Mouse.Captured == null ||
                            !selectedPopupChild.IsAncestorOf(Mouse.Captured as DependencyObject))
                        {
                            IsOpen = false;
                        }
                    }
                    else
                    {
                        // If control inside Ribbon loses capture - restore capture to Ribbon
                        if (selectedPopupChild.IsAncestorOf(e.OriginalSource as DependencyObject))
                        {
                            if (IsOpen && Mouse.Captured == null)
                            {
                                Mouse.Capture(this, CaptureMode.SubTree);
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            IsOpen = false;
                        }
                    }
                }
            }
            base.OnLostMouseCapture(e);*/
        }

        #endregion

        #region Private methods

        private void OnPopupClosing()
        {
            
        }

        private void OnPopupOpening()
        {
            
        }

        private static void OnClickThroughThunk(object sender, MouseButtonEventArgs e)
        {
            /*DropDownButton button = (DropDownButton)sender;
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                if (Mouse.Captured == button)
                {
                    button.IsOpen = false;                    
                    Mouse.Capture(null);
                }
            }*/
        }

        private static void OnPopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                /*(e.OldValue as Popup).Opened -= (d as DropDownButton).OnPopupOpening;
                (e.OldValue as Popup).Closed -= (d as DropDownButton).OnPopupClosing;*/
            }

            if (e.NewValue != null)
            {
                /*(e.NewValue as Popup).Opened += (d as DropDownButton).OnPopupOpening;
                (e.NewValue as Popup).Closed += (d as DropDownButton).OnPopupClosing;*/
                /*(e.NewValue as Popup).PlacementTarget = (d as DropDownButton);
                (e.NewValue as Popup).Placement = PlacementMode.Bottom;*/

                /*Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = (e.NewValue as Popup);
                (d as DropDownButton).SetBinding(IsOpenProperty, binding);*/
                Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = (d as DropDownButton);
                (e.NewValue as Popup).SetBinding(Popup.IsOpenProperty, binding);
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DropDownButton ribbon = (DropDownButton)d;

            if (ribbon.IsOpen)
            {
                //ribbon.OnPopupOpening();
                ribbon.IsHitTestVisible = false;
            }
            else
            {
                //ribbon.OnPopupClosing();
                ribbon.IsHitTestVisible = true;
            }
        }

        #endregion
    }
}
