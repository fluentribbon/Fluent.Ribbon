using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents gallery item
    /// </summary>
    public class GalleryItem : ListBoxItem
    {
        #region Fields

        private Dispatcher dispatcher;

        #endregion

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
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(GalleryItem), new UIPropertyMetadata(false));



        /// <summary>
        /// Gets or sets GalleryItem group
        /// </summary>
        public string Group
        {
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Group.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("Group", typeof(string), typeof(GalleryItem), new UIPropertyMetadata(null));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(typeof(GalleryItem)));
            IsSelectedProperty.AddOwner(typeof (GalleryItem), new FrameworkPropertyMetadata(false,FrameworkPropertyMetadataOptions.None, OnIsSelectedPropertyChanged));
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                //(d as GalleryItem).dispatcher.Invoke(DispatcherPriority.ContextIdle, new ThreadStart(delegate{RibbonPopup.CloseAll();}));
                
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryItem()
        {
            dispatcher = Dispatcher.CurrentDispatcher;
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
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
            IsPressed = true;
            Mouse.Capture(this);
            e.Handled = true;
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
            if (Mouse.Captured == this) Mouse.Capture(null);
            Point position = Mouse.PrimaryDevice.GetPosition(this);
            if (((position.X >= 0.0) && (position.X <= ActualWidth)) && ((position.Y >= 0.0) && (position.Y <= ActualHeight)) && (e.ClickCount == 1))
            {
                RoutedEventArgs ee = new RoutedEventArgs(RibbonControl.ClickEvent, this);
                RaiseEvent(ee);
                e.Handled = true;
            }
            e.Handled = true;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="e">The event data</param>
        protected virtual void OnClick(RoutedEventArgs e)
        {
            IsSelected = true;
            RibbonPopup.CloseAll();
            e.Handled = true;
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
    }
}
