using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// Represents KeyTip control
    /// </summary>
    public class KeyTip : Label
    {
        #region HotKey Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for HotKey.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeysProperty = DependencyProperty.RegisterAttached(
          "Keys",
          typeof(string),
          typeof(KeyTip),
          new FrameworkPropertyMetadata(null, KeysPropertyChanged)
        );

        static void KeysPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        /// <summary>
        /// Sets value of attached property Keys for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetKeys(UIElement element, string value)
        {
            element.SetValue(KeysProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Keys of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Keys"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Key sequence for the given element")]
        public static string GetKeys(UIElement element)
        {
            return (string)element.GetValue(KeysProperty);
        }

        #endregion

        #region Initialization
        
        // Static constructor
        static KeyTip()
        {
            // Override metadata to allow slyling
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyTip), new FrameworkPropertyMetadata(typeof(KeyTip)));
        }
        
        /// <summary>
        /// Default constrctor
        /// </summary>
        public KeyTip()
        {
        }

        #endregion

        #region Managing

        static IInputElement backupFocusedElement = null;
        static Stack<KeyTipAdorner> steps = new Stack<KeyTipAdorner>();
        static Popup popup = null;

        public static void Show(UIElement element)
        {
            backupFocusedElement = Keyboard.FocusedElement;
            popup = new Popup();
            popup.Focusable = true;
            popup.Child = new Button();
            popup.Width = popup.Height = 0;
            popup.StaysOpen = false;
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;
            popup.KeyUp += OnKeyUp;

            popup.IsOpen = true;

            Forward(element);
        }

        static void Forward(UIElement element)
        {
            if (steps.Count != 0) AdornerLayer.GetAdornerLayer(steps.Peek()).Remove(steps.Peek());
            KeyTipAdorner adorner = new KeyTipAdorner(element);
            AdornerLayer.GetAdornerLayer(element).Add(adorner);
            steps.Push(adorner);
        }

        static void Back()
        {
            KeyTipAdorner adorner = steps.Pop();
            AdornerLayer.GetAdornerLayer(adorner.AdornedElement).Remove(adorner);
            if (steps.Count == 0) { popup.IsOpen = false; return; }            
            AdornerLayer.GetAdornerLayer(steps.Peek().AdornedElement).Add(steps.Peek());            
        }

        static void OnPopupOpened(object sender, EventArgs e)
        {
           (sender as Popup).Child.Focus();
        }

        static void OnPopupClosed(object sender, EventArgs e)
        {
            Popup popup = (Popup)sender;
            popup.Opened -= OnPopupOpened;
            popup.KeyUp -= OnKeyUp;
            popup.Closed -= OnPopupClosed;
            if (backupFocusedElement != null) backupFocusedElement.Focus();
            backupFocusedElement = null;

            while (steps.Count != 0)
            {
                KeyTipAdorner adorner = steps.Pop();
                AdornerLayer.GetAdornerLayer(adorner.AdornedElement).Remove(adorner);
            }

            popup = null;
        }

        static void OnKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (e.Key == Key.Escape) Back();
            else if ((e.Key == Key.System)&&
                ((e.SystemKey == Key.LeftAlt) || 
                (e.SystemKey == Key.RightAlt) || 
                (e.SystemKey == Key.F10))) popup.IsOpen = false;            
            else
            {
                UIElement element = steps.Peek().GetElement("");
                if (element != null) Forward(element);
            }
        }

        #endregion
    }
}
