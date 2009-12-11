using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// This interface must be implemented for controls
    /// which are intended to insert to quick access toolbar
    /// </summary>
    public interface IQuickAccessItemProvider
    {
        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        UIElement CreateQuickAccessShortcut();
    }
    
    /// <summary>
    /// Peresents quick access shortcut to another control
    /// </summary>
    [ContentProperty("Shortcut")]
    public class QuickAccessMenuItem : MenuItem
    {
        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public QuickAccessMenuItem()
        {
        }

        #endregion

        #region Shortcut Property

        /// <summary>
        /// Gets or sets shortcut control
        /// </summary>
        public Control Shortcut
        {
            get { return (Control)GetValue(ShortcutProperty); }
            set { SetValue(ShortcutProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for shortcut. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShortcutProperty =
            DependencyProperty.Register("Shortcut", typeof(Control), typeof(QuickAccessMenuItem), new UIPropertyMetadata(null, OnShortcutChanged));


        static void OnShortcutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles click events
        /// </summary>
        /// <param name="e">Event args</param>
        protected override void OnClick(RoutedEventArgs e)
        {
            base.OnClick(e);
            QuickAccessToolbar toolbar = FindQuickAccessToolbar();
            if (toolbar != null)
            {
                toolbar.Items.Add(QuickAccessItemsProvider.GetQuickAccessItem(Shortcut));
                toolbar.InvalidateMeasure();
            }
        }

        #endregion

        #region Private Methods

        QuickAccessToolbar FindQuickAccessToolbar()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                if (element is QuickAccessToolbar) return (element as QuickAccessToolbar);
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element as DependencyObject);
                if (parent != null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element as DependencyObject);
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// The class responds to mine controls for QuickAccessToolbar
    /// </summary>
    internal static class QuickAccessItemsProvider
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the given control can provide a quick access toolbar item
        /// </summary>
        /// <param name="element">Control</param>
        /// <returns>True if this control is able to provide
        /// a quick access toolbar item, false otherwise</returns>
        public static bool IsSupported(UIElement element)
        {
            if (element is IQuickAccessItemProvider) return true;
            if ((element is Button) ||
                (element is CheckBox) ||
                (element is RadioButton) ||
                (element is ComboBox) ||
                (element is TextBox)) return true;
            else return false;
        }

        /// <summary>
        /// Gets control which represents quick access toolbar item
        /// </summary>
        /// <param name="element">Host control</param>
        /// <returns>Control which represents quick access toolbar item</returns>
        public static UIElement GetQuickAccessItem(UIElement element)
        {
            UIElement result = null;

            // If control supports the interface just return what it provides            
            if (element is IQuickAccessItemProvider) result = (element as IQuickAccessItemProvider).CreateQuickAccessShortcut();

            // Predefined controls            
            else if (element is TextBox) result = GetTextBoxQuickAccessItem(element as TextBox);
            else if (element is ComboBox) result = GetComboBoxQuickAccessItem(element as ComboBox);
            else if (element is ToggleButton) result = GetToggleButtonQuickAccessItem(element as ToggleButton);
            else if (element is Button) result = GetButtonQuickAccessItem(element as Button);            

            // The control isn't supported
            if (result == null) throw new ArgumentException("The contol " + element.GetType().Name + " is not able to provide a quick access toolbar item");
            
            return result;
        }
        
        /// <summary>
        /// Finds the top supported control and gets quick access item from it
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static UIElement PickQuickAccessItem(Visual visual, Point point)
        {
            UIElement element = FindSupportedControl(visual, point);
            if (element != null) return GetQuickAccessItem(element);
            else return null;
        }

        /// <summary>
        /// Finds the top supported control
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static UIElement FindSupportedControl(Visual visual, Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(visual, point);
            if (result == null) return null;
            UIElement element = result.VisualHit as UIElement;
            while (element != null)
            {
                if(IsSupported(element)) return element;
                element = VisualTreeHelper.GetParent(element) as UIElement;
            }
            return null;
        }

        #endregion

        #region Buttons

        static UIElement GetButtonQuickAccessItem(Button button)
        {
           

            Button item = new Button();
            item.Focusable = false;


            // Copy common properties
            BindControlProperties(button, item);
            // Bind small icon
            if (button.Icon != null)
                Bind(button, item, "Icon",RibbonControl.IconProperty, BindingMode.OneWay);
            if (button.LargeIcon != null)
                Bind(button, item, "LargeIcon", Button.LargeIconProperty, BindingMode.OneWay);

            // TODO: check, maybe copy style is not required for quick access toolbar items
            Bind(button, item, "Style", Button.StyleProperty, BindingMode.OneWay);
            item.Size = RibbonControlSize.Small;

            
            // Syncronization
            item.Click += delegate(object sender, RoutedEventArgs e) { button.RaiseEvent(e); };

            
            return item;
        }

        #endregion

        #region CheckBoxes & RadioButtons (ToggleButtons)

        
        static UIElement GetToggleButtonQuickAccessItem(ToggleButton toggleButton)
        {
           

            ToggleButton item = new ToggleButton();
            item.Focusable = false;
            //item.Content = (toggleButton.Content != null) ? toggleButton.Content.ToString() : null;
            item.Text = toggleButton.Text;
            item.Size = RibbonControlSize.Small;
            
            // Copy common properties
            BindControlProperties(toggleButton, item);

            // Copy small icon
            if (toggleButton.Icon != null) 
                Bind(toggleButton, item, "Icon", 
                    RibbonControl.IconProperty, BindingMode.OneWay);
            
            // TODO: check, maybe copy style is not required for quick access toolbar items
            Bind(toggleButton, item, "Style", ToggleButton.StyleProperty, BindingMode.OneWay);

            // Syncronization            
            Bind(toggleButton, item, "IsChecked", ToggleButton.IsCheckedProperty, BindingMode.TwoWay);

            return item;
        }

        #endregion

        #region ComboBoxes

        static UIElement GetComboBoxQuickAccessItem(ComboBox comboBox)
        {


            ComboBox item = new ComboBox();
            item.Focusable = false;
            

            // Copy common properties
            BindControlProperties(comboBox, item);
            
            // Copy small icon
            /*if (RibbonControl.GetSmallIcon(comboBox) != null)
                Bind(comboBox, item, "(Fluent:RibbonControl.SmallIcon)",
                    RibbonControl.SmallIconProperty, BindingMode.OneWay);*/


            Bind(comboBox, item, "Width", ComboBox.WidthProperty, BindingMode.OneWay);
            Bind(comboBox, item, "Heigth", ComboBox.HeightProperty, BindingMode.OneWay);
            Bind(comboBox, item, "MaxDropDownHeight", ComboBox.MaxDropDownHeightProperty, BindingMode.OneWay);
            Bind(comboBox, item, "StaysOpenOnEdit", ComboBox.StaysOpenOnEditProperty, BindingMode.OneWay);
            

            Bind(comboBox, item, "IsEditable", ComboBox.IsEditableProperty, BindingMode.OneWay);
            Bind(comboBox, item, "IsReadOnly", ComboBox.IsReadOnlyProperty, BindingMode.TwoWay);
            Bind(comboBox, item, "Text", ComboBox.TextProperty, BindingMode.TwoWay);

            item.ItemsSource = comboBox.Items;            
            Bind(comboBox, item, "Items", ComboBox.ItemsSourceProperty, BindingMode.OneWay);

            return item;
        }

        #endregion

        #region TextBoxes
                
        static UIElement GetTextBoxQuickAccessItem(TextBox textBox)
        {
           

            TextBox item = new TextBox();
            //item.Focusable = false;
            Bind(textBox, item, "Width", Control.WidthProperty, BindingMode.OneWay);
            Bind(textBox, item, "Heigth", Control.HeightProperty, BindingMode.OneWay);

            // Copy common properties
            BindControlProperties(textBox, item);
            
            // Copy small icon
            /*if (RibbonControl.GetSmallIcon(textBox) != null)
                Bind(textBox, item, "(Fluent:RibbonControl.SmallIcon)",
                    RibbonControl.SmallIconProperty, BindingMode.OneWay);*/
                        
            Bind(textBox, item, "IsReadOnly", TextBox.IsReadOnlyProperty, BindingMode.OneWay);
            //Bind(textBox, item, "Text", TextBox.TextProperty, BindingMode.OneWay);
            Bind(textBox, item, "CharacterCasing", TextBox.CharacterCasingProperty, BindingMode.OneWay);
            Bind(textBox, item, "MaxLength", TextBox.MaxLengthProperty, BindingMode.OneWay);
            Bind(textBox, item, "MaxLines", TextBox.MaxLinesProperty, BindingMode.OneWay);
            Bind(textBox, item, "MinLines", TextBox.MinLinesProperty, BindingMode.OneWay);
            Bind(textBox, item, "TextAlignment", TextBox.TextAlignmentProperty, BindingMode.OneWay);
            Bind(textBox, item, "TextDecorations", TextBox.TextDecorationsProperty, BindingMode.OneWay);
            Bind(textBox, item, "TextWrapping", TextBox.TextWrappingProperty, BindingMode.OneWay);

            // Binding for Text we have to do manually, 
            // because the binding doesn't work properly 
            // if focus will be remain in one of the controls
            item.Text = textBox.Text;
            textBox.TextChanged += delegate { item.Text = textBox.Text; };
            item.TextChanged += delegate { textBox.Text = item.Text; };

           
            return item;
        }

        #endregion
        
        #region Common Stuff
               
        static void BindControlProperties(Control source, Control target)
        {
            if (source is ButtonBase)
            {
                Bind(source, target, "Command", ButtonBase.CommandProperty, BindingMode.OneWay);
                Bind(source, target, "CommandParameter", ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                Bind(source, target, "CommandTarget", ButtonBase.CommandTargetProperty, BindingMode.OneWay);
                Bind(source, target, "Command", ButtonBase.CommandProperty, BindingMode.OneWay);
            }

            Bind(source, target, "ToolTip", Control.ToolTipProperty, BindingMode.OneWay);
            Bind(source, target, "ContextMenu", Control.ContextMenuProperty, BindingMode.OneWay);

            Bind(source, target, "FontFamily", Control.FontFamilyProperty, BindingMode.OneWay);
            Bind(source, target, "FontSize", Control.FontSizeProperty, BindingMode.OneWay);
            Bind(source, target, "FontStretch", Control.FontStretchProperty, BindingMode.OneWay);
            Bind(source, target, "FontStyle", Control.FontStyleProperty, BindingMode.OneWay);
            Bind(source, target, "FontWeight", Control.FontWeightProperty, BindingMode.OneWay);

            Bind(source, target, "Foreground", Control.ForegroundProperty, BindingMode.OneWay);
            Bind(source, target, "IsEnabled", Control.IsEnabledProperty, BindingMode.OneWay);
            Bind(source, target, "Opacity", Control.OpacityProperty, BindingMode.OneWay);
            Bind(source, target, "SnapsToDevicePixels", Control.SnapsToDevicePixelsProperty, BindingMode.OneWay);
            Bind(source, target, "Visibility", Control.VisibilityProperty, BindingMode.OneWay);            
        }

        #endregion

        #region Binding

        static void Bind(FrameworkElement source, FrameworkElement target, string path, DependencyProperty property)
        {
            Bind(source, target, path, property, BindingMode.OneWay);
        }

        static void Bind(FrameworkElement source, FrameworkElement target, string path, DependencyProperty property, BindingMode mode)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(path);
            binding.Source = source;
            binding.Mode = mode;
            target.SetBinding(property, binding);
        }

        #endregion
    }
}
