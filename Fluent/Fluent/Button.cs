#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents button
    /// </summary>
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPressed.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(Button), new UIPropertyMetadata(false));


        /// <summary>
        /// Gets a value that indicates whether the mouse pointer is located over this element 
        /// (including visual children elements that are inside its bounds).
        /// </summary>
        public new bool IsMouseOver
        {
            get { return (bool)GetValue(IsMouseOverProperty); }
            private set { SetValue(IsMouseOverProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsMouseOver.  This enables animation, styling, binding, etc...
        /// </summary>
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

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), typeof(Button), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Button()
        {
            StyleProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));            
        }

        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = ThemesManager.DefaultButtonStyle;
            return basevalue;
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
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonDown routed event that occurs 
        /// when the left mouse button is pressed while the mouse pointer is over this control.
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
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonUp routed event that occurs 
        /// when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsPressed = false;
            if(Mouse.Captured==this)Mouse.Capture(null);
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && ((position.Y >= 0.0) && (position.Y <= ActualHeight)) && (e.ClickCount == 1))
            {
                RoutedEventArgs ee = new RoutedEventArgs(RibbonControl.ClickEvent, this);
                RaiseEvent(ee);
                e.Handled = true;
            }            
            base.OnMouseLeftButtonDown(e);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseMove attached event reaches an element 
        /// in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseEventArgs that contains the event data.</param>
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

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseLeave attached event is raised on this element. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseEventArgs that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsMouseOver = false;
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseEnter attached event is raised on this element. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e"> The System.Windows.Input.MouseEventArgs that contains the event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsMouseOver = true;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="e">The event data</param>
        protected virtual void OnClick(RoutedEventArgs e)
        {
            ExecuteCommand();
        }

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {                                                
            OnClick(e);
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override UIElement CreateQuickAccessItem()
        {
            Button button = new Button();
            BindQuickAccessItem(button);
            return button;
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            Button button = element as Button;
            if (LargeIcon != null) Bind(button, button, "LargeIcon", Button.LargeIconProperty, BindingMode.OneWay);
            button.Click += delegate(object sender, RoutedEventArgs e) { RaiseEvent(e); };
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
