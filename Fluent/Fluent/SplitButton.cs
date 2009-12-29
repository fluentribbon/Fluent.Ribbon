using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Fluent
{
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class SplitButton:DropDownButton
    {
        #region Fields

        private Button button;

        #endregion

        #region Events

        public event RoutedEventHandler Click;

        #endregion

        #region Constructors

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SplitButton()
        {
            StyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null));            
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = ThemesManager.DefaultSplitButtonStyle;
            return basevalue;
        }

        public SplitButton()
        {

        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            if (button != null) button.Click -= OnButtonClick;
            button = GetTemplateChild("PART_Button") as Button;
            if(button!=null)
            {
                Binding binding = new Binding("Command");
                binding.Source = this;
                binding.Mode = BindingMode.OneTime;
                button.SetBinding(CommandProperty, binding);

                binding = new Binding("CommandTarget");
                binding.Source = this;
                binding.Mode = BindingMode.OneTime;
                button.SetBinding(CommandTargetProperty, binding);
                
                binding = new Binding("CommandParameter");
                binding.Source = this;
                binding.Mode = BindingMode.OneTime;
                button.SetBinding(CommandParameterProperty, binding);
                button.Click += OnButtonClick;
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            if (Click != null) Click(this, e);
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {            
            if(button==null) base.OnMouseLeftButtonDown(e);
            else
            {
                Point position = e.GetPosition(button);
                if (((position.X >= 0.0) && (position.X <= button.ActualWidth)) &&
                    ((position.Y >= 0.0) && (position.Y <= button.ActualHeight))) e.Handled = true;
                else base.OnMouseLeftButtonDown(e);
            }
        }

        #endregion
    }
}
