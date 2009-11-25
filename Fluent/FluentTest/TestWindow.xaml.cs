using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fluent;

namespace FluentTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();
            ScreenTip.HelpPressed += new EventHandler<ScreenTipHelpEventArgs>(OnScreenTipHelpPressed);


            ribbonTabControl.MouseDown += new MouseButtonEventHandler(OnRibbonTabControlMouseDown);
        }

        void OnRibbonTabControlMouseDown(object sender, MouseButtonEventArgs e)
        {
            QuickAccessItemsProvider.FindAccessedControl(ribbonTabControl, e.GetPosition(ribbonTabControl));
        }

        void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.HelpTopic);
        }

        private void OnLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Launcher button pressed!!!");
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (tabGroup1.Visibility == Visibility.Visible)
            {
                tabGroup1.Visibility = System.Windows.Visibility.Collapsed;
                tabGroup2.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                tabGroup1.Visibility = System.Windows.Visibility.Visible;
                tabGroup2.Visibility = System.Windows.Visibility.Visible;
            }
        }

    }
}
