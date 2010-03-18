using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Fluent;

namespace FluentTest
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestWindow : RibbonWindow
    {
        public TestWindow()
        {
            InitializeComponent();
            ScreenTip.HelpPressed += new EventHandler<ScreenTipHelpEventArgs>(OnScreenTipHelpPressed);

            //IView = CollectionViewSource.GetDefaultView();

            //Visibility = Visibility.Hidden;
//            (Content as UIElement).Visibility = Visibility.Hidden;
            Loaded += delegate
            {
                /*tabGroup1.Visibility = System.Windows.Visibility.Visible;
                tabGroup2.Visibility = System.Windows.Visibility.Visible;*/
                /*Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                                                                                               {
                                                                                                   (Content as UIElement)
                                                                                                       .Visibility =
                                                                                                       Visibility.
                                                                                                           Visible;
                                                                                               }));*/
            };
           
            //Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() => { Visibility = Visibility.Visible; }));
        }
        static AdornerLayer GetAdornerLayer(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
            }
        }
        void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            System.Diagnostics.Process.Start((string)e.HelpTopic);
        }

        private void OnLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(/*this,*/"Launcher button pressed!!!");
            TestWindow wnd = new TestWindow();
            wnd.Show();
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (tabGroup1.Visibility == Visibility.Visible)
            {
                tabGroup1.Visibility = System.Windows.Visibility.Collapsed;
                tabGroup2.Visibility = System.Windows.Visibility.Collapsed;
               // tabGroup3.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                tabGroup1.Visibility = System.Windows.Visibility.Visible;
                tabGroup2.Visibility = System.Windows.Visibility.Visible;
                //tabGroup3.Visibility = System.Windows.Visibility.Visible;
            }
            e.Handled = true;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*UIElement control = QuickAccessItemsProvider.PickQuickAccessItem((sender as RibbonTabControl), e.GetPosition(sender as RibbonTabControl));
            if (control != null)
            {               
                if(control is CheckBox)
                {
                    (control as CheckBox).Width = 100;
                    (control as CheckBox).Height = 22;
                }
                if (quickAccessToolbar.Items.Contains(control)) quickAccessToolbar.Items.Remove(control);
                else quickAccessToolbar.Items.Add(control);
            }            */
        }

        private void OnTestClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void OnSomeClick(object sender, RoutedEventArgs e)
        {

        }

        public static RoutedCommand CustomRoutedCommand = new RoutedCommand("lala",typeof(TestWindow));

        private void ExecutedCustomCommand(object sender, ExecutedRoutedEventArgs e)
        {
            /*MessageBox.Show("Custom Command Executed");*/
            canEx = !canEx;
            /*Window dialog = new Window();
            dialog.Owner = Application.Current.MainWindow;
            dialog.ShowDialog();*/
        }

        private bool canEx = true;
        private void CanExecuteCustomCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = canEx;
        }

        private void OnSomeTestClick(object sender, RoutedEventArgs e)
        {
            //A.IsCached = false;
            A.Items.Add(new Fluent.Button() { Text = "fsdfsd" });
            //MessageBox.Show("lala");
            CheckLogicalTree(ribbon);
            logicalTreeView.Items.Clear();
            BuildLogicalTree(ribbon, null);
        }

        void CheckLogicalTree(DependencyObject root)
        {
            var children = LogicalTreeHelper.GetChildren(root);
            foreach (var child in children)
            {
                if (child is DependencyObject)
                {
                    if (LogicalTreeHelper.GetParent(child as DependencyObject) != root)
                    {
                        Debug.WriteLine(string.Format("Incorrect logical parent in element - {0} in {1}", child.ToString(), root.ToString()));
                    }
                    CheckLogicalTree(child as DependencyObject);
                }
            }
        }

        void BuildLogicalTree(DependencyObject root, TreeViewItem item)
        {  

            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = String.Format("[{0}]{1}", root.ToString(), (root is FrameworkElement)?(root as FrameworkElement).Name:"");
            if(item!=null)
            {
                item.Items.Add(newItem);
            }
            else
            {
                logicalTreeView.Items.Add(newItem);
            }
            var children = LogicalTreeHelper.GetChildren(root);
            foreach (var child in children)
            {
                if (child is DependencyObject)
                {
                    BuildLogicalTree(child as DependencyObject, newItem);
                }
            }
        }

        private void OnSilverClick(object sender, RoutedEventArgs e)
        {
            //ThemesManager.SetTheme(this,Themes.Office2010Silver);            
            Application.Current.Resources.BeginInit();
            Application.Current.Resources.Source=new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml");
            Application.Current.Resources.EndInit();
        }

        private void OnBlackClick(object sender, RoutedEventArgs e)
        {
            //ThemesManager.SetTheme(this, Themes.Office2010Black);
            //Application.Current.Resources.BeginInit();
            Application.Current.Resources.Source=new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml");
            //Application.Current.Resources.EndInit();
        }

        private void OnSplitClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Split Clicked!!!");
        }

        private void OnUnfreezeClick(object sender, RoutedEventArgs e)
        {
            Clipboard.IsSnapped = false;
        }

        private void OnFreezeClick(object sender, RoutedEventArgs e)
        {
            Clipboard.IsSnapped = true;
        }

        private void OnEnlargeClick(object sender, RoutedEventArgs e)
        {
            inRibbonGallery.Enlarge();
        }

        private void OnReduceClick(object sender, RoutedEventArgs e)
        {
            inRibbonGallery.Reduce();
        }

        private void OnSplitButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Split");
        }

        private void OnLauncherClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Launcher Click");
        }
    }

}
