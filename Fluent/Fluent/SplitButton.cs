using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Fluent
{
    [TemplatePart(Name = "PART_Button", Type = typeof(Button))]
    public class SplitButton:DropDownButton
    {
        #region Fields

        private Button button;

        private SplitButton quickAccessButton;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                /*if (contextMenu != null)
                {
                    ArrayList list = new ArrayList();                    
                    if (contextMenu.MenuBar != null) list.Add(contextMenu.MenuBar);
                    else list.Add(contextMenu);
                    if (button != null) list.Add(button);
                    return list.GetEnumerator();                    
                }
                else*/
                {
                    ArrayList list = new ArrayList();                    
                    if(items!=null)  list.AddRange(items);
                    if (button != null) list.Add(button);
                    return list.GetEnumerator();
                }
            }
        }
        /*protected override IEnumerator LogicalChildren
        {
            get
            {
                if (contextMenu != null)
                {
                    ArrayList list = new ArrayList();
                    if (contextMenu.MenuBar != null) list.Add(contextMenu.MenuBar);
                    else list.Add(contextMenu);
                    return list.GetEnumerator();
                }
                else
                {
                    if (items != null) return items.GetEnumerator();
                    else return (new ArrayList()).GetEnumerator();
                }
            }
        }*/
        #endregion

        #region Events

        public event RoutedEventHandler Click;

        #endregion

        #region Constructors

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SplitButton()
        {
            //StyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            FrameworkElement.FocusVisualStyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null));            
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            //if (basevalue == null) basevalue = ThemesManager.DefaultSplitButtonStyle;
            return basevalue;
        }

        public SplitButton()
        {

        }

        #endregion

        #region Overrides

        private void OnClick(object sender, RoutedEventArgs e)
        {            
            e.Handled = true;
        }

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

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            SplitButton button = new SplitButton();
            button.Loaded += OnQuickAccessButtonLoaded;
            
            BindQuickAccessItem(button);
            //button.PreviewMouseLeftButtonDown += OnQuickAccessClick;
            button.MenuOpened += OnQuickAccessClick;
            return button;
        }

        private void OnQuickAccessButtonLoaded(object sender, RoutedEventArgs e)
        {
            SplitButton button = sender as SplitButton;
            if (button.button != null)
            {
                button.Loaded -= OnQuickAccessButtonLoaded;
                button.button.CanAddToQuickAccessToolBar = false;
            }
        }

        private void OnQuickAccessClick(object sender, EventArgs e)
        {
            SplitButton button = sender as SplitButton;
            for (int i = 0; i < Items.Count; i++)
            {
                UIElement item = Items[0];
                Items.Remove(item);
                button.Items.Add(item);
                i--;
            }
            button.MenuClosed += OnQuickAccessMenuClosed;
            quickAccessButton = button;
        }

        private void OnQuickAccessMenuClosed(object sender, EventArgs e)
        {
            quickAccessButton.MenuClosed -= OnQuickAccessMenuClosed;
            for (int i = 0; i < quickAccessButton.Items.Count; i++)
            {
                UIElement item = quickAccessButton.Items[0];
                quickAccessButton.Items.Remove(item);
                Items.Add(item);
                i--;
            }
        }

        /// <summary>
        /// This method must be overriden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            SplitButton button = element as SplitButton;
            Bind(this, button, "MenuResizeMode", MenuResizeModeProperty, BindingMode.Default);
            button.Click += delegate(object sender, RoutedEventArgs e) { RaiseEvent(e); };
            base.BindQuickAccessItem(element);
        }

        #endregion
    }
}
