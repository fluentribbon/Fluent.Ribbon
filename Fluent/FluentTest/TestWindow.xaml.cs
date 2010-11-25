using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
using Microsoft.Win32;
using Button = Fluent.Button;
using ComboBox = Fluent.ComboBox;
using MenuItem = Fluent.MenuItem;

namespace FluentTest
{
    public enum TstEnum
    {
        Elemen1, Elemen2
    }
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestWindow : RibbonWindow
    {
        

        private string[] data = new string[] {"Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol"};
        public string[] FontsData
        {
            get { return data; }
        }

        public TstEnum TST
        {
            get { return (TstEnum)GetValue(TSTProperty); }
            set { SetValue(TSTProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TST.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TSTProperty =
            DependencyProperty.Register("TST", typeof(TstEnum), typeof(TestWindow), new UIPropertyMetadata(TstEnum.Elemen1));

        private Color[] themeColors = new Color[]{Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple};
        public Color[] ThemeColors
        {
            get { return themeColors; }
        }

        public Array TstArr
        {
            get { return Enum.GetValues(typeof (TstEnum)); }
        }
        
        public TestWindow()
        {
            InitializeComponent();

            //ribbon.IsBackstageOpen = true;

            ScreenTip.HelpPressed += new EventHandler<ScreenTipHelpEventArgs>(OnScreenTipHelpPressed);
            
            //Ribbon.Localization.Culture = new CultureInfo("ru-RU");
            //IView = CollectionViewSource.GetDefaultView();
            
            //Visibility = Visibility.Hidden;
//            (Content as UIElement).Visibility = Visibility.Hidden;
            /*Loaded += delegate
            {*/
                /*tabGroup1.Visibility = System.Windows.Visibility.Visible;
                tabGroup2.Visibility = System.Windows.Visibility.Visible;*/
                /*Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() =>
                                                                                               {
                                                                                                   (Content as UIElement)
                                                                                                       .Visibility =
                                                                                                       Visibility.
                                                                                                           Visible;
                                                                                               }));*/
           // };
           
            //Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new ThreadStart(() => { Visibility = Visibility.Visible; }));
            buttonBold.Checked += (s, e) => Debug.WriteLine("Checked");
            buttonBold.Unchecked += (s, e) => Debug.WriteLine("Unchecked");
            ribbon.DataContext = this;
            DataContext = this;
            /*DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) => { Debug.WriteLine("FocusedElement - " + Keyboard.FocusedElement); };
            timer.Start();*/
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
           /* MessageBox.Show(this,"Launcher button pressed!!!");
            
            Thread thread = new Thread(()=>
                                           {
                                            TestWindow wnd = new TestWindow();
                                            wnd.ShowDialog();
                                           });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();*/
            Window wnd = new Window();
            System.Windows.Controls.ComboBox box = new System.Windows.Controls.ComboBox();
            box.ItemsSource = new string[] {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};
            wnd.Content = box;
            wnd.Owner = this;
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
        {/*
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
         //   canEx = !canEx;
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
            //A.Items.Add(new Fluent.Button() { Header = "fsdfsd" });
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
            newItem.Tag = root;
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

       

        private void OnLauncherClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Launcher Click");
            xxx.Items.Add(CreateRibbonButton());
        }
        
        public Fluent.Button CreateRibbonButton()
        {
            Fluent.Button button = new Fluent.Button();
            FooCommand1 fooCommand1 = new FooCommand1();
            button.Command = fooCommand1.ItemCommand;
            
            button.Header = "Foo";
            
            this.CommandBindings.Add(fooCommand1.ItemCommandBinding);
            return button;
        }

        void OnExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void OnSilverClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml") });
                Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                Application.Current.Resources.EndInit();
            }));
        }

        private void OnBlackClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml") });
                Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                Application.Current.Resources.EndInit();
            }));
        }

        private void OnBlueClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Blue.xaml") });
                Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                Application.Current.Resources.EndInit();
            }));
        }
        
        private void OnFormatPainterClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FP");
            /*OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog(this);*/
            /*if (Font.Visibility == Visibility.Collapsed) Font.Visibility = Visibility.Visible;
            else Font.Visibility = Visibility.Collapsed;*/
        }
        
        private void OnHelpClick(object sender, RoutedEventArgs e)
        {
            if(tabGroup1.Visibility==Visibility.Visible)
            {
                tabGroup1.Visibility = Visibility.Collapsed;
                tabGroup2.Visibility = Visibility.Collapsed;
            }
            else
            {
                tabGroup1.Visibility = Visibility.Visible;
                tabGroup2.Visibility = Visibility.Visible;
            }
           //Title = "Long long long title - Fluent Ribbon Control Suite 1.2";
            //homeTabItem.Groups.Add(new RibbonGroupBox() { Header = "Lala" });
//            ribbon.SelectedTabItem = homeTabItem;
            //Clipboard.Visibility = Visibility.Visible;
            //ribbon.Tabs.RemoveAt(0);
        }

        void OnSpinnerValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           // MessageBox.Show(String.Format("Changed from {0} to {1}", e.OldValue, e.NewValue));
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            TestWindow wnd = new TestWindow();
            wnd.Owner = this;
            wnd.Show();
        }
        

        private void OnTreeDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = ((sender as TreeView).SelectedItem as TreeViewItem);
            if(item==null) return;
            Debug.Write("//");            
            BuildBackLogicalTree(item.Tag as DependencyObject);
            Debug.WriteLine("//");
        }
        void BuildBackLogicalTree(DependencyObject root)
        {

            if (root == null) return;
            Debug.Write(" - " + String.Format("[{0}]{1}", root.ToString(), (root is FrameworkElement) ? (root as FrameworkElement).Name : ""));

            var parent = LogicalTreeHelper.GetParent(root);

            BuildBackLogicalTree(parent as DependencyObject);
                
            
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Executed");
        }

        private void New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
     
        }

        static RoutedCommand new1 = new RoutedCommand("New1", typeof(TestWindow));

        static public RoutedCommand New1
        {
            get { return new1; }
        }
    }

    public class FooCommand1
    {

        public static RoutedCommand TestPresnterCommand = new RoutedCommand("TestPresnterCommand", typeof(FooCommand1));

        public System.Windows.Input.ICommand ItemCommand
        {
            get { return TestPresnterCommand; }
        }

        public System.Windows.Input.CommandBinding ItemCommandBinding
        {
            get { return new CommandBinding(TestPresnterCommand, OnTestCommandExecuted, CanExecuteTestCommand); }
        }

        public FooCommand1()
        {

        }


        private void CanExecuteTestCommand(object sender,
       CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnTestCommandExecuted(object sender,
        ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Test Module Command");
        }

    }
    
}
