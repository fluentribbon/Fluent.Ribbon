using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fluent
{
    public class GalleryItem:ContentControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether item is selected
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GalleryItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets whether mouse button is pressed on control
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

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(typeof(GalleryItem)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryItem()
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
            if ((!IsEnabled) || (!IsHitTestVisible)) return;
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
            if (Mouse.Captured == this) Mouse.Capture(null);
            base.OnMouseLeftButtonUp(e);
        }

        #endregion
    }
}
