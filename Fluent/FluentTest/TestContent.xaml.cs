namespace FluentTest
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Fluent;
    using FluentTest.Annotations;
    using Button = Fluent.Button;
    using ComboBox = System.Windows.Controls.ComboBox;

    public enum TstEnum
    {
        Elemen1,
        Elemen2
    }

    /// <summary>
    /// Interaction logic for TestContent.xaml
    /// </summary>
    public partial class TestContent : INotifyPropertyChanged
    {
        public TestContent()
        {
            this.InitializeComponent();

            ScreenTip.HelpPressed += this.OnScreenTipHelpPressed;

            //Ribbon.Localization.Culture = new CultureInfo("ru-RU");

            this.buttonBold.Checked += (s, e) => Debug.WriteLine("Checked");
            this.buttonBold.Unchecked += (s, e) => Debug.WriteLine("Unchecked");

            this.BoundSpinnerValue = 1;

            this.DataContext = this;
        }

        public string Title
        {
            get
            {
                var window = Window.GetWindow(this);
                return window == null
                    ? "Test"
                    : window.Title;
            }
        }

        public int BoundSpinnerValue
        {
            get { return this.boundSpinnerValue; }
            set 
            { 
                this.boundSpinnerValue = value;
                this.OnPropertyChanged("BoundSpinnerValue");
            }
        }

        private readonly string[] data = { "Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol" };

        public string[] FontsData
        {
            get { return this.data; }
        }

        public TstEnum TST
        {
            get { return (TstEnum)this.GetValue(TSTProperty); }
            set { this.SetValue(TSTProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TST.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TSTProperty =
            DependencyProperty.Register("TST", typeof(TstEnum), typeof(TestWindow), new UIPropertyMetadata(TstEnum.Elemen1));

        private readonly Color[] themeColors = { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple };
        private int boundSpinnerValue;

        public Color[] ThemeColors
        {
            get { return this.themeColors; }
        }

        public Array TstArr
        {
            get { return Enum.GetValues(typeof(TstEnum)); }
        }

        private void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            Process.Start((string)e.HelpTopic);
        }

        private void OnLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            var wnd = new Window();
            var box = new ComboBox
                          {
                              ItemsSource = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }
                          };
            wnd.Content = box;
            wnd.Owner = Window.GetWindow(this);
            wnd.Show();
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (this.tabGroup1.Visibility == Visibility.Visible)
            {
                this.tabGroup1.Visibility = Visibility.Collapsed;
                this.tabGroup2.Visibility = Visibility.Collapsed;
                // tabGroup3.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.tabGroup1.Visibility = Visibility.Visible;
                this.tabGroup2.Visibility = Visibility.Visible;
                //tabGroup3.Visibility = System.Windows.Visibility.Visible;
            }

            e.Handled = true;
        }

        public static RoutedCommand CustomRoutedCommand = new RoutedCommand("lala", typeof(TestWindow));

        private void OnSplitClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Split Clicked!!!");
        }

        private void OnUnfreezeClick(object sender, RoutedEventArgs e)
        {
            this.Clipboard.IsSnapped = false;
        }

        private void OnFreezeClick(object sender, RoutedEventArgs e)
        {
            this.Clipboard.IsSnapped = true;
        }

        private void OnEnlargeClick(object sender, RoutedEventArgs e)
        {
            this.inRibbonGallery.Enlarge();
        }

        private void OnReduceClick(object sender, RoutedEventArgs e)
        {
            this.inRibbonGallery.Reduce();
        }

        private void OnLauncherClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Launcher Click");
            this.xxx.Items.Add(this.CreateRibbonButton());
        }

        public Button CreateRibbonButton()
        {
            var button = new Button();
            var fooCommand1 = new FooCommand1();
            button.Command = fooCommand1.ItemCommand;

            button.Header = "Foo";

            this.CommandBindings.Add(fooCommand1.ItemCommandBinding);
            return button;
        }

        #region Theme change

        private void OnSilverClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml") });                
                Application.Current.Resources.EndInit();
            }));
        }

        private void OnMetroClick(object sender, RoutedEventArgs e)
        {
            var metro = new MetroStyle();
            metro.Show();
        }

        private void OnBlackClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml") });                
                Application.Current.Resources.EndInit();
            }));
        }

        private void OnBlueClick(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Blue.xaml") });                
                Application.Current.Resources.EndInit();
            }));
        }

        #endregion Theme change

        #region Logical tree

        private void OnShowLogicalTreeClick(object sender, RoutedEventArgs e)
        {
            this.CheckLogicalTree(this.ribbon);
            this.logicalTreeView.Items.Clear();
            this.BuildLogicalTree(this.ribbon, this.logicalTreeView);
        }

        private string GetDebugInfo(DependencyObject element)
        {
            var header = element is IRibbonControl
                           ? (element as IRibbonControl).Header
                           : string.Empty;

            var name = element is FrameworkElement
                           ? (element as FrameworkElement).Name
                           : string.Empty;

            return string.Format("[{0}] (Header: {1} || Name: {2})", element, header, name);
        }

        private void CheckLogicalTree(DependencyObject root)
        {
            var children = LogicalTreeHelper.GetChildren(root);
            foreach (var child in children.OfType<DependencyObject>())
            {
                if (LogicalTreeHelper.GetParent(child) != root)
                {
                    Debug.WriteLine("Incorrect logical parent for {0}", this.GetDebugInfo(child));
                    Debug.WriteLine("\tExpected: {0}", root.ToString());
                    Debug.WriteLine("\tFound: {0}", LogicalTreeHelper.GetParent(child).ToString());
                }

                this.CheckLogicalTree(child);
            }
        }

        private void BuildLogicalTree(DependencyObject current, ItemsControl parentControl)
        {
            var newItem = new TreeViewItem
            {
                Header = this.GetDebugInfo(current),
                Tag = current
            };

            parentControl.Items.Add(newItem);

            var children = LogicalTreeHelper.GetChildren(current);
            foreach (var child in children.OfType<DependencyObject>())
            {
                this.BuildLogicalTree(child, newItem);
            }
        }

        private void OnTreeDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeView = sender as TreeView;

            if (treeView == null)
            {
                return;
            }

            var item = treeView.SelectedItem as TreeViewItem;
            if (item == null)
            {
                return;
            }

            var stringBuilder = new StringBuilder();
            this.BuildBackLogicalTree(item.Tag as DependencyObject, stringBuilder);

            MessageBox.Show(string.Format("From buttom to top:\n{0}", stringBuilder));
        }

        private void BuildBackLogicalTree(DependencyObject current, StringBuilder stringBuilder)
        {
            if (current == null
                || current == this.ribbon)
            {
                return;
            }

            stringBuilder.AppendFormat(" -> {0}\n", this.GetDebugInfo(current));

            var parent = LogicalTreeHelper.GetParent(current);

            this.BuildBackLogicalTree(parent, stringBuilder);
        }

        #endregion Logical tree

        private void OnFormatPainterClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("FP");
        }

        private void OnHelpClick(object sender, RoutedEventArgs e)
        {
            if (this.tabGroup1.Visibility == Visibility.Visible)
            {
                this.tabGroup1.Visibility = Visibility.Collapsed;
                this.tabGroup2.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tabGroup1.Visibility = Visibility.Visible;
                this.tabGroup2.Visibility = Visibility.Visible;
            }
            //Title = "Long long long title - Fluent Ribbon Control Suite 1.2";
            //homeTabItem.Groups.Add(new RibbonGroupBox() { Header = "Lala" });
            //            ribbon.SelectedTabItem = homeTabItem;
            //Clipboard.Visibility = Visibility.Visible;
            //ribbon.Tabs.RemoveAt(0);
        }

        private void OnSpinnerValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // MessageBox.Show(String.Format("Changed from {0} to {1}", e.OldValue, e.NewValue));
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var wnd = new TestWindow
                      {
                          Owner = Window.GetWindow(this)
                      };
            wnd.Show();
        }

        private void OnPrintVisualClick(object sender, RoutedEventArgs e)
        {
            var printDlg = new PrintDialog();
            printDlg.PrintVisual(this, "Main Window");
        }

        #region threading

        private void OnThreadWindowButtonClick(object sender, RoutedEventArgs e)
        {
            var thread = new Thread(ShowThreadedWindow);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        private static void ShowThreadedWindow()
        {
            var window = new ThreadedWindow();
            window.Show();

            Dispatcher.Run();
        }

        #endregion threading

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class FooCommand1
    {
        public static RoutedCommand TestPresnterCommand = new RoutedCommand("TestPresnterCommand", typeof(FooCommand1));

        public ICommand ItemCommand
        {
            get { return TestPresnterCommand; }
        }

        public CommandBinding ItemCommandBinding
        {
            get { return new CommandBinding(TestPresnterCommand, this.OnTestCommandExecuted, this.CanExecuteTestCommand); }
        }

        private void CanExecuteTestCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnTestCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("Test Module Command");
        }
    }
}
