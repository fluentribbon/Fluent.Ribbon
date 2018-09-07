// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents context menu resize mode
    /// </summary>
    public enum ContextMenuResizeMode
    {
        /// <summary>
        /// Context menu can not be resized
        /// </summary>
        None = 0,

        /// <summary>
        /// Context menu can be only resized vertically
        /// </summary>
        Vertical,

        /// <summary>
        /// Context menu can be resized vertically and horizontally
        /// </summary>
        Both
    }

    /// <summary>
    /// Represents a pop-up menu that enables a control
    /// to expose functionality that is specific to the context of the control
    /// </summary>
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        #region Fields

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;
        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)this.GetValue(ResizeModeProperty); }
            set { this.SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register(nameof(ResizeMode), typeof(ContextMenuResizeMode),
            typeof(ContextMenu), new PropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>]
        static ContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(typeof(ContextMenu)));
            FocusVisualStyleProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata());
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta -= this.OnResizeVerticalDelta;
            }

            this.resizeVerticalThumb = this.GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (this.resizeVerticalThumb != null)
            {
                this.resizeVerticalThumb.DragDelta += this.OnResizeVerticalDelta;
            }

            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta -= this.OnResizeBothDelta;
            }

            this.resizeBothThumb = this.GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (this.resizeBothThumb != null)
            {
                this.resizeBothThumb.DragDelta += this.OnResizeBothDelta;
            }
        }

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameworkElement;
        }

        #endregion

        #region Private methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(this.Width))
            {
                this.Width = this.ActualWidth;
            }

            if (double.IsNaN(this.Height))
            {
                this.Height = this.ActualHeight;
            }

            this.Width = Math.Max(this.MinWidth, this.Width + e.HorizontalChange);
            this.Height = Math.Max(this.MinHeight, this.Height + e.VerticalChange);
        }

        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(this.Height))
            {
                this.Height = this.ActualHeight;
            }

            this.Height = Math.Max(this.MinHeight, this.Height + e.VerticalChange);
        }

        #endregion
    }
}