namespace FluentTest
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Fluent;

    public enum TstEnum
    {
        Elemen1,
        Elemen2
    }

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestWindow
    {
        private readonly string[] data = new[] { "Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol" };
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

        private readonly Color[] themeColors = new[] { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple };
        public Color[] ThemeColors
        {
            get { return themeColors; }
        }

        public Array TstArr
        {
            get { return Enum.GetValues(typeof(TstEnum)); }
        }

        public TestWindow()
        {
            this.InitializeComponent();

            //ribbon.IsBackstageOpen = true;

            ScreenTip.HelpPressed += this.OnScreenTipHelpPressed;

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

        private static AdornerLayer GetAdornerLayer(UIElement element)
        {
            UIElement current = element;
            while (true)
            {
                current = (UIElement)VisualTreeHelper.GetParent(current);
                if (current is AdornerDecorator) return AdornerLayer.GetAdornerLayer((UIElement)VisualTreeHelper.GetChild(current, 0));
            }
        }

        private void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            Process.Start((string)e.HelpTopic);
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
            var wnd = new Window();
            var box = new System.Windows.Controls.ComboBox
                          {
                              ItemsSource = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" }
                          };
            wnd.Content = box;
            wnd.Owner = this;
            wnd.Show();
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            if (tabGroup1.Visibility == Visibility.Visible)
            {
                tabGroup1.Visibility = Visibility.Collapsed;
                tabGroup2.Visibility = Visibility.Collapsed;
                // tabGroup3.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                tabGroup1.Visibility = Visibility.Visible;
                tabGroup2.Visibility = Visibility.Visible;
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

        public static RoutedCommand CustomRoutedCommand = new RoutedCommand("lala", typeof(TestWindow));

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

        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Theme change

        private void OnSilverClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
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
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                Application.Current.Resources.BeginInit();
                Application.Current.Resources.MergedDictionaries.RemoveAt(1);
                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml") });                
                Application.Current.Resources.EndInit();
            }));
        }

        private void OnBlueClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
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
                    Debug.WriteLine(string.Format("Incorrect logical parent for {0}", this.GetDebugInfo(child)));
                    Debug.WriteLine(string.Format("\tExpected: {0}", root));
                    Debug.WriteLine(string.Format("\tFound: {0}", LogicalTreeHelper.GetParent(child)));
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
            /*OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog(this);*/
            /*if (Font.Visibility == Visibility.Collapsed) Font.Visibility = Visibility.Visible;
            else Font.Visibility = Visibility.Collapsed;*/
        }

        private void OnHelpClick(object sender, RoutedEventArgs e)
        {
            if (tabGroup1.Visibility == Visibility.Visible)
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

        private void OnSpinnerValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // MessageBox.Show(String.Format("Changed from {0} to {1}", e.OldValue, e.NewValue));
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            TestWindow wnd = new TestWindow();
            wnd.Owner = this;
            wnd.Show();
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