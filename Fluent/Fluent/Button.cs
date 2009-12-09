using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    [ContentProperty("Text")]
    public class Button: RibbonControl
    {
        #region Properties

        /// <summary>
        /// Gets a value that indicates whether a Button is currently activated. This is a dependency property.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            private set { SetValue(IsPressedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(Button), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets a value that indicates whether the mouse pointer is located over this element (including visual children elements that are inside its bounds).
        /// </summary>
        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            private set { SetValue(IsMouseOverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        public static readonly new DependencyProperty IsMouseOverProperty =
            DependencyProperty.Register("IsMouseOver", typeof(bool), typeof(Button), new UIPropertyMetadata(false));

        /// <summary>
        /// Button large icon
        /// </summary>
        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(Button), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Button()
        {
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonDown routed event that occurs when the left mouse button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if((!IsEnabled)||(!IsHitTestVisible)) return;
            IsPressed = true;
            Mouse.Capture(this);
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonUp routed event that occurs when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsPressed = false;
            Mouse.Capture(null);
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && ((position.Y >= 0.0) && (position.Y <= ActualHeight)) && (e.ClickCount == 1))
            {
                RoutedEventArgs ee = new RoutedEventArgs(RibbonControl.ClickEvent, this);
                RaiseEvent(ee);
                e.Handled = true;
            }            
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;    
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) &&
                ((position.Y >= 0.0) && (position.Y <= ActualHeight)))
            {
                if(!IsMouseOver) IsMouseOver = true;
                if ((Mouse.Captured == this) && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
                {
                    if (!IsPressed)
                    {
                        IsPressed = true;
                    }
                }
            }
            else
            {
                if(IsMouseOver) IsMouseOver = false;
                if ((Mouse.Captured == this) && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
                {
                    if (IsPressed)
                    {
                        IsPressed = false;
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsMouseOver = false;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsMouseOver = true;
        }

        #endregion

        #region Protected methods

        protected virtual void OnClick(RoutedEventArgs e)
        {
            ExecuteCommand();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;            
        }

        #endregion

        #region Private methods

        

        #endregion
    }
}
