using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Fluent
{
    /// <summary>
    /// Represents context menu resize mode
    /// </summary>
    public enum ContextMenuResizeMode
    {
        /// <summary>
        /// Contet menu can`t resize
        /// </summary>
        None=0,
        /// <summary>
        /// Context menu can only resize vertical
        /// </summary>
        Vertical,
        /// <summary>
        /// Context menu can only vertical and horizontal
        /// </summary>
        Both
    }
    /// <summary>
    /// Represents a pop-up menu that enables a control 
    /// to expose functionality that is specific to the context of the control
    /// </summary>
    [TemplatePart(Name = "PART_ResizeBothThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ResizeVerticalThumb", Type = typeof(Thumb))]
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        #region Fields

        // Popup for displaing context menu
        private RibbonPopup popup;
        // Initializing flag to prevent context menu closing while initializing
        private bool isInInitializing;

        // Thumb to resize in both directions
        private Thumb resizeBothThumb;
        // Thumb to resize vertical
        private Thumb resizeVerticalThumb;

        #endregion

        #region Properties        

        internal RibbonPopup RibbonPopup
        {
            get { return popup;}
        }

        /// <summary>
        /// Gets or sets context menu resize mode
        /// </summary>
        public ContextMenuResizeMode ResizeMode
        {
            get { return (ContextMenuResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.Register("ResizeMode", typeof(ContextMenuResizeMode), typeof(ContextMenu), new UIPropertyMetadata(ContextMenuResizeMode.None));

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor
        /// </summary>
        static ContextMenu()
        {
            IsOpenProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsOpenChanged,CoerceIsOpen));
            FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(ContextMenu), new FrameworkPropertyMetadata(null));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContextMenu()
        {
            
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RibbonTabItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return true;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            if(resizeVerticalThumb!=null)
            {
                resizeVerticalThumb.DragDelta -= OnResizeVerticalDelta;
            }
            resizeVerticalThumb = GetTemplateChild("PART_ResizeVerticalThumb") as Thumb;
            if (resizeVerticalThumb != null)
            {
                resizeVerticalThumb.DragDelta += OnResizeVerticalDelta;
            }

            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta -= OnResizeBothDelta;
            }
            resizeBothThumb = GetTemplateChild("PART_ResizeBothThumb") as Thumb;
            if (resizeBothThumb != null)
            {
                resizeBothThumb.DragDelta += OnResizeBothDelta;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsOpen = false;
            e.Handled = true;
        }

        #endregion

        #region Private methods

        // Handles resize both drag
        private void OnResizeBothDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(Width)) Width = ActualWidth;
            if (double.IsNaN(Height)) Height = ActualHeight;
            Width = Math.Max(0, Width + e.HorizontalChange);
            Height = Math.Max(0, Height + e.VerticalChange);
        }
        // Handles resize vertical drag
        private void OnResizeVerticalDelta(object sender, DragDeltaEventArgs e)
        {
            if (double.IsNaN(Height)) Height = ActualHeight;
            Height = Math.Max(0, Height + e.VerticalChange);
        }

        // Coerce IsOpen property
        private static object CoerceIsOpen(DependencyObject d, object basevalue)
        {
            ContextMenu menu = (ContextMenu)d;
            if(menu.isInInitializing) return true;
            return basevalue;
        }
        // Handles IsOpen property changing
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContextMenu menu = (ContextMenu)d;
            if ((bool)e.NewValue)
            {
                if (menu.popup == null)
                {
                    // Creates new ribbon popup and prevents it`s closing
                    menu.isInInitializing = true;
                    menu.HookupParentPopup();
                    menu.IsOpen = true;                    
                    Mouse.Capture(null);
                    menu.isInInitializing = false;
                    menu.popup.IgnoreNextDeactivate = false;
                    menu.popup.Activate();
                }                
            }
            //TODO: Strange behavior of command when our context menu is opened
        }
        // Creates ribbon popup and removes original System.Windows.Controls.ContextMenu popup
        private void HookupParentPopup()
        {
            // Find original popup
            Popup originalPopup = (typeof(System.Windows.Controls.ContextMenu).
                GetField("_parentPopup", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this) as Popup);
            // Remove context menu from original popup logical children
            try
            {
                originalPopup.Child = null;
            }
            catch (InvalidOperationException){}
            // Remove context menu from original popup visual children
            try
            {              
                typeof(FrameworkElement).
                    GetMethod("RemoveVisualChild", BindingFlags.NonPublic | BindingFlags.Instance).
                    Invoke(VisualTreeHelper.GetParent(this), new object[] { this });
            }
            catch(TargetException){}
            catch (TargetInvocationException) { }
            // Create new ribbon popup
            popup = new RibbonPopup();            
            this.popup.AllowsTransparency = true;
            // Replace original popup with ribbon popup
            typeof(System.Windows.Controls.ContextMenu).
                GetField("_parentPopup", BindingFlags.NonPublic | BindingFlags.Instance).
                SetValue(this, popup);
            // Preventing ribbon popup closing
            popup.IgnoreNextDeactivate = true;
            // Set ribbon popup bindings
            CreatePopupRoot(this.popup, this);
            // Closing original popup
            originalPopup.IsOpen = false;
        }
        // Set popup bindings
        static void CreatePopupRoot(RibbonPopup popup, UIElement child)
        {
            Binding binding = new Binding("PlacementTarget");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.PlacementTargetProperty, binding);
            popup.Child = child;
            binding = new Binding("VerticalOffset");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.VerticalOffsetProperty, binding);
            binding = new Binding("HorizontalOffset");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.HorizontalOffsetProperty, binding);
            binding = new Binding("PlacementRectangle");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.PlacementRectangleProperty, binding);
            binding = new Binding("Placement");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.PlacementProperty, binding);
            binding = new Binding("StaysOpen");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.StaysOpenProperty, binding);
            binding = new Binding("CustomPopupPlacementCallback");
            binding.Mode = BindingMode.OneWay;
            binding.Source = child;
            popup.SetBinding(Popup.CustomPopupPlacementCallbackProperty, binding);
            binding = new Binding("IsOpen");
            binding.Mode = BindingMode.TwoWay;
            binding.Source = child;
            popup.SetBinding(Popup.IsOpenProperty, binding);
        }

        #endregion
    }
}
