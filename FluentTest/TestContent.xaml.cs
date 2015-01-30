namespace FluentTest
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using Fluent;
    using FluentTest.ViewModels;
    using Button = Fluent.Button;

    /// <summary>
    /// Interaction logic for TestContent.xaml
    /// </summary>
    public partial class TestContent
    {
        public TestContent()
        {
            this.InitializeComponent();

            this.Loaded += this.HandleTestContentLoaded;

            //Ribbon.Localization.Culture = new CultureInfo("ru-RU");

            this.buttonBold.Checked += (s, e) => Debug.WriteLine("Checked");
            this.buttonBold.Unchecked += (s, e) => Debug.WriteLine("Unchecked");

            this.DataContext = new MainViewModel();
        }

        private static void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
        {
            Process.Start((string)e.HelpTopic);
        }

        private void HandleTestContentLoaded(object sender, RoutedEventArgs e)
        {
            ScreenTip.HelpPressed += OnScreenTipHelpPressed;
        }

        private void OnLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            var groupBox = (RibbonGroupBox)sender;

            var wnd = new Window
                {
                    Content = string.Format("Launcher-Window for: {0}", groupBox.Header),
                    Owner = Window.GetWindow(this)
                };

            wnd.Show();
        }

        private void OnSplitClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Split Clicked!!!");
        }

        private void OnEnlargeClick(object sender, RoutedEventArgs e)
        {
            this.InRibbonGallery.Enlarge();
        }

        private void OnReduceClick(object sender, RoutedEventArgs e)
        {
            this.InRibbonGallery.Reduce();
        }

        public Button CreateRibbonButton()
        {
            var fooCommand1 = new FooCommand1();

            var button = new Button
            {
                Command = fooCommand1.ItemCommand,
                Header = "Foo",
                Icon = new BitmapImage(new Uri(@"Images\Green.png", UriKind.Relative)),
                LargeIcon = new BitmapImage(new Uri(@"Images\GreenLarge.png", UriKind.Relative)),
            };

            this.CommandBindings.Add(fooCommand1.ItemCommandBinding);
            return button;
        }

        #region Theming

        private enum Theme
        {
            Office2010,
            Office2013,
            Windows8
        }

        private Theme? currentTheme;

        private void OnOffice2013Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2013, "pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml");
        }

        private void OnOffice2010SilverClick(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2010, "pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml");
        }

        private void OnOffice2010BlackClick(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2010, "pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml");
        }

        private void OnOffice2010BlueClick(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2010, "pack://application:,,,/Fluent;component/Themes/Office2010/Blue.xaml");
        }

        private void OnWindows8Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Windows8, "pack://application:,,,/Fluent;component/Themes/Windows8/Silver.xaml");
        }


        private void ChangeTheme(Theme theme, string color)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                var owner = Window.GetWindow(this);
                if (owner != null)
                {
                    owner.Resources.BeginInit();

                    if (owner.Resources.MergedDictionaries.Count > 0)
                    {
                        owner.Resources.MergedDictionaries.RemoveAt(0);
                    }

                    if (string.IsNullOrEmpty(color) == false)
                    {
                        owner.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(color) });
                    }

                    owner.Resources.EndInit();
                }

                if (this.currentTheme != theme)
                {
                    Application.Current.Resources.BeginInit();
                    switch (theme)
                    {
                        case Theme.Office2010:
                            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml") });
                            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                            break;
                        case Theme.Office2013:
                            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
                            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                            break;
                        case Theme.Windows8:
                            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Windows8/Generic.xaml") });
                            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                            break;
                    }

                    this.currentTheme = theme;
                    Application.Current.Resources.EndInit();

                    if (owner is RibbonWindow)
                    {
                        owner.Style = null;
                        owner.Style = owner.FindResource("RibbonWindowStyle") as Style;
                        owner.Style = null;
                    }
                }
            }));
        }

        private void HandleDontUseDwmClick(object sender, RoutedEventArgs e)
        {
            var control = sender as UIElement;

            if (control == null)
            {
                return;
            }

            var window = Window.GetWindow(control) as RibbonWindow;

            if (window == null)
            {
                return;
            }

            window.DontUseDwm = this.DontUseDwm.IsChecked.GetValueOrDefault();
        }

        #endregion Theming

        #region Logical tree

        private void OnShowLogicalTreeClick(object sender, RoutedEventArgs e)
        {
            this.CheckLogicalTree(this.ribbon);
            this.logicalTreeView.Items.Clear();
            this.BuildLogicalTree(this.ribbon, this.logicalTreeView);
        }

        private static string GetDebugInfo(DependencyObject element)
        {
            if (element == null)
            {
                return "NULL";
            }

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
                    Debug.WriteLine(string.Format("Incorrect logical parent for {0}", GetDebugInfo(child)));
                    Debug.WriteLine(string.Format("\tExpected: {0}", GetDebugInfo(root)));
                    Debug.WriteLine(string.Format("\tFound: {0}", GetDebugInfo(LogicalTreeHelper.GetParent(child))));
                }

                this.CheckLogicalTree(child);
            }
        }

        private void BuildLogicalTree(DependencyObject current, ItemsControl parentControl)
        {
            var newItem = new TreeViewItem
            {
                Header = GetDebugInfo(current),
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

            stringBuilder.AppendFormat(" -> {0}\n", GetDebugInfo(current));

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

            if (printDlg.ShowDialog() == true)
            {
                printDlg.PrintVisual(this, "Main Window");
            }
        }

        private void AddRibbonTab_OnClick(object sender, RoutedEventArgs e)
        {
            var tab = new RibbonTabItem
            {
                Header = "Test"
            };

            var group = new RibbonGroupBox();
            for (var i = 0; i < 20; i++)
            {
                group.Items.Add(this.CreateRibbonButton());
            }

            tab.Groups.Add(group);

            this.ribbon.Tabs.Add(tab);
        }

        private void HandleSaveAsClick(object sender, RoutedEventArgs e)
        {
            var w = new Window();
            w.ShowDialog();
        }

        private void OpenMahMetroWindow_OnClick(object sender, RoutedEventArgs e)
        {
            new MahMetroWindow().Show();
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