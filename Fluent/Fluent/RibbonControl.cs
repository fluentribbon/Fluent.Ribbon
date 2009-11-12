using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Fluent
{
    [TemplatePart(Name = "PART_RibbonPanel", Type = typeof(RibbonPanel))]
    [ContentProperty("Tabs")]
    public class RibbonControl: FrameworkElement, IAddChild
    {
        #region Статический атрибуты

        // Дефолтный стиль для окна
        private static readonly Style DefaultWindowStyle = null;
        // Дефолтный словарь с темами
        private static readonly ResourceDictionary DefaultResourceDictionary = null;

        #endregion

        #region Атрибуты

        private UIElementCollection tabs;

        private RibbonPanel panel;

        #endregion

        #region Свойства

        public UIElementCollection Tabs
        {
            get
            {
                return tabs;
            }
        }

        #endregion

        #region Инициализация

        static RibbonControl()
        {
            StyleProperty.OverrideMetadata(typeof(RibbonControl), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            /*DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonControl), new FrameworkPropertyMetadata(typeof(RibbonControl)));

            DefaultResourceDictionary = new ResourceDictionary();
            DefaultResourceDictionary.BeginInit();
            DefaultResourceDictionary.Source = new Uri("Fluent;component/Styles/DefaultSkinWindowStyle.xaml", UriKind.Relative);
            DefaultResourceDictionary.EndInit();
            DefaultWindowStyle = (Style)DefaultResourceDictionary["DefaultSkinWindowStyle"];*/
        }

        /// <summary>
        /// При необходимости устанавливается ныжный стиль
        /// </summary>
        /// <param name="o">Не используется</param>
        /// <param name="value">Значение из словаря</param>
        /// <returns>Правильный словарь</returns>
        private static object OnCoerceStyle(DependencyObject o, object value)
        {
            if (value == null) value = RibbonControl.DefaultWindowStyle;
            return value;
        }

        /// <summary>
        /// При применении нового стиля
        /// </summary>
        public override void OnApplyTemplate()
        {
            panel = FindName("PART_RibbonPanel") as RibbonPanel;
            if(panel == null) throw new Exception("Incorrect control template.");
        }

        public RibbonControl()
        {
            tabs = new UIElementCollection(this, this);
        }

        #endregion

        #region Реализация IAddChild

        public void AddChild(object value)
        {
            Tabs.Add(value as UIElement);
        }

        public void AddText(string text)
        {
            TextBlock textBox = new TextBlock();
            textBox.Text = text;
            Tabs.Add(textBox);
        }

        #endregion
    }
}
