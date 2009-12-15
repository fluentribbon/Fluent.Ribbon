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
    /// Represents a pop-up menu that enables a control 
    /// to expose functionality that is specific to the context of the control
    /// </summary>
    public class ContextMenu : System.Windows.Controls.ContextMenu
    {
        #region Fields

        // Popup for displaing context menu
        private RibbonPopup popup;
        // Initializing flag to prevent context menu closing while initializing
        private bool isInInitializing;

        #endregion

        #region Properties        

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

        #endregion

        #region Private methods

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
