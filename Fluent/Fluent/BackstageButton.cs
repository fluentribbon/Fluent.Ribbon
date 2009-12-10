using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Fluent
{
    class BackstageButton:RibbonControl
    {
        #region Fields

        #endregion

        #region Properties

        public Backstage Backstage
        {
            get { return (Backstage)GetValue(BackstageProperty); }
            set { SetValue(BackstageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Backstage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackstageProperty =
            DependencyProperty.Register("Backstage", typeof(object), typeof(BackstageButton), new UIPropertyMetadata(null));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(BackstageButton), new UIPropertyMetadata(false));

        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                list.Add(Backstage);
                return list.GetEnumerator();
            }
        }

        #endregion
       
        #region Initialize

        static BackstageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BackstageButton),
                                                     new FrameworkPropertyMetadata(typeof(BackstageButton)));                       
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public BackstageButton()
        {
            AddHandler(RibbonControl.ClickEvent, new RoutedEventHandler(OnClick));
            Backstage = new Backstage();
            Backstage.Style = Application.Current.Resources["BackstageStyle"] as Style;
            Binding binding = new Binding("Background");
            binding.Source = this;
            Backstage.SetBinding(Backstage.BackgroundProperty, binding);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            IsOpen = !IsOpen;
            e.Handled = true;
        }

        #endregion

        #region Overrides

        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(RibbonControl.ClickEvent,this));            
        }

        public override void OnApplyTemplate()
        {
        }

        #endregion
    }
}
