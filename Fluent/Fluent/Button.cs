#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Specifies when the Click event should be raised. 
    /// </summary>
    public enum ClickMode
    {
        /// <summary>
        /// Specifies that the Click event should be raised when a button is pressed and released. 
        /// </summary>
        Release = 0,
        /// <summary>
        /// Specifies that the Click event should be raised as soon as a button is pressed. 
        /// </summary>
        Pressed
    }

    /// <summary>
    /// Represents button
    /// </summary>
    [ContentProperty("Text")]
    public class Button: RibbonControl
    {
        #region Properties
        
        /// <summary>
        /// Gets or sets when the Click event occurs. 
        /// This is a dependency property. 
        /// </summary>
        public ClickMode ClickMode
        {
            get { return (ClickMode)GetValue(ClickModeProperty); }
            set { SetValue(ClickModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ClickMode.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ClickModeProperty =
            DependencyProperty.Register("ClickMode", typeof(ClickMode), 
            typeof(Button), new UIPropertyMetadata(ClickMode.Release));

        /// <summary>
        /// Gets a value that indicates whether a Button is currently 
        /// activated. This is a dependency property.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            private set { SetValue(IsPressedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsPressedPropertyKey = DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool),
            typeof(Button), new UIPropertyMetadata(false));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPressed. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;
        
        /// <summary>
        /// Button large icon
        /// </summary>
        public ImageSource LargeIcon
        {
            get { return (ImageSource)GetValue(LargeIconProperty); }
            set { SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register("LargeIcon", typeof(ImageSource), 
            typeof(Button), new UIPropertyMetadata(null));

        // Gets whether mouse cursor directly over this control
        bool IsMouseStraightOver
        {
            get
            {
                Point position = Mouse.GetPosition(this);
                return (((position.X >= 0.0) && (position.X <= ActualWidth)) &&
                        ((position.Y >= 0.0) && (position.Y <= ActualHeight)));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static Button()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Button), new FrameworkPropertyMetadata(typeof(Button)));
            IsDefinitiveProperty.OverrideMetadata(typeof(Button), new UIPropertyMetadata(true));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Button()
        {
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
            Mouse.Capture(this);
            IsPressed = true;            
            e.Handled = true;            
            if ((ClickMode == ClickMode.Pressed) && (e.ClickCount == 1))
            {
                if (IsMouseStraightOver) RaiseClick();
            }            
        }

        /// <summary>
        /// Invoked when an unhandled LostMouseCapture attached 
        /// event reaches an element in its route that is derived 
        /// from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The MouseEventArgs that contains event data</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            IsPressed = false;
        }

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonUp routed event that occurs 
        /// when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            IsPressed = false;
            e.Handled = true;
            if ((ClickMode == ClickMode.Release) && (e.ClickCount == 1))
            {
                if (IsMouseStraightOver) RaiseClick();
            }
            if (Mouse.Captured == this)
                try
                {
                    Mouse.Capture(null);
                }
                catch (System.Exception)
                {
                    // TODO: On ApplyTemplate grid is removed from ControlTemplate namescope but WPF tries to animate it and throws exception.
                    // May be need to stop all animations on apply template. 
                }
        }


        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Mouse.MouseMove 
        /// attached event reaches an element in its route that is derived 
        /// from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseEventArgs that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((!IsEnabled) || (!IsHitTestVisible)) return;    
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) &&
                ((position.Y >= 0.0) && (position.Y <= ActualHeight)))
            {
                if ((Mouse.Captured == this) && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
                {
                    if (!IsPressed) IsPressed = true;
                }
            }
            else
            {
                if ((Mouse.Captured == this) && (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed))
                {
                    if (IsPressed) IsPressed = false;
                }
            }
            base.OnMouseMove(e);
        }
        
        /// <summary>
        /// Handles click
        /// </summary>
        /// <param name="args"></param>
        protected override void OnClick(RoutedEventArgs args)
        {
            base.OnClick(args);
            ExecuteCommand();
            args.Handled = true;
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
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
            Button button = (Button)element;
            button.Click += ((sender, e) => RaiseEvent(e));
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
