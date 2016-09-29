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
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Fluent;
    using FluentTest.ViewModels;
    using Button = Fluent.Button;

    public partial class TestContent
    {
        private readonly MainViewModel viewModel;

        public TestContent()
        {
            this.InitializeComponent();

            //RibbonLocalization.Current.Localization.Culture = new CultureInfo("ru-RU");

            this.HookEvents();

            this.viewModel = new MainViewModel();
            this.DataContext = this.viewModel;
        }

        private void HookEvents()
        {
            this.Loaded += this.HandleTestContentLoaded;

            this.buttonBold.Checked += (s, e) => Debug.WriteLine("Checked");
            this.buttonBold.Unchecked += (s, e) => Debug.WriteLine("Unchecked");

            this.PreviewMouseWheel += this.OnPreviewMouseWheel;
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
                Content = $"Launcher-Window for: {groupBox.Header}",
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
            if (this.InRibbonGallery.IsLoaded)
            {
                this.InRibbonGallery.Enlarge();
            }
        }

        private void OnReduceClick(object sender, RoutedEventArgs e)
        {
            if (this.InRibbonGallery.IsLoaded)
            {
                this.InRibbonGallery.Reduce();
            }
        }

        public Button CreateRibbonButton()
        {
            var fooCommand1 = new TestRoutedCommand();

            var button = new Button
            {
                Command = fooCommand1.ItemCommand,
                Header = "Foo",
                Icon = new BitmapImage(new Uri("pack://application:,,,/Fluent.Ribbon.Showcase;component/Images/Green.png", UriKind.Absolute)),
                LargeIcon = new BitmapImage(new Uri("pack://application:,,,/Fluent.Ribbon.Showcase;component/Images/GreenLarge.png", UriKind.Absolute)),
            };

            this.CommandBindings.Add(fooCommand1.ItemCommandBinding);
            return button;
        }

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

            var ribbonControl = element as IHeaderedControl;

            var header = ribbonControl != null
                           ? ribbonControl.Header
                           : string.Empty;

            var frameworkElement = element as FrameworkElement;
            var name = frameworkElement != null
                           ? frameworkElement.Name
                           : string.Empty;

            return $"[{element}] (Header: {header} || Name: {name})";
        }

        private void CheckLogicalTree(DependencyObject root)
        {
            var children = LogicalTreeHelper.GetChildren(root);
            foreach (var child in children.OfType<DependencyObject>())
            {
                if (ReferenceEquals(LogicalTreeHelper.GetParent(child), root) == false)
                {
                    Debug.WriteLine($"Incorrect logical parent for {GetDebugInfo(child)}");
                    Debug.WriteLine($"\tExpected: {GetDebugInfo(root)}");
                    Debug.WriteLine($"\tFound: {GetDebugInfo(LogicalTreeHelper.GetParent(child))}");
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

            MessageBox.Show($"From buttom to top:\n{stringBuilder}");
        }

        private void BuildBackLogicalTree(DependencyObject current, StringBuilder stringBuilder)
        {
            if (current == null
                || ReferenceEquals(current, this.ribbon))
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

        private void OpenRegularWindow_OnClick(object sender, RoutedEventArgs e)
        {
            new RegularWindow().Show();
        }

        private void OpenMahMetroWindow_OnClick(object sender, RoutedEventArgs e)
        {
            new MahMetroWindow().Show();
        }

        private void OpenRibbonWindowWithoutVisibileRibbon_OnClick(object sender, RoutedEventArgs e)
        {
            new RibbonWindowWithoutVisibleRibbon().Show();
        }

        private void OpenRibbonWindowWithoutRibbon_OnClick(object sender, RoutedEventArgs e)
        {
            new RibbonWindowWithoutRibbon().Show();
        }

        private void ZoomSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var textFormattingMode = e.NewValue > 1.0 || Math.Abs(e.NewValue - 1.0) < double.Epsilon ? TextFormattingMode.Ideal : TextFormattingMode.Display;
            TextOptions.SetTextFormattingMode(this, textFormattingMode);
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) == false
                && Keyboard.IsKeyDown(Key.RightCtrl) == false)
            {
                return;
            }

            this.zoomSlider.Value += e.Delta > 0 ? 0.1 : -0.1;

            e.Handled = true;
        }

        private void SleepButton_OnClick(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));
        }

        private void OpenModalRibbonWindow_OnClick(object sender, RoutedEventArgs e)
        {
            new TestWindow().ShowDialog();
        }

        private void ShowStartScreen_OnClick(object sender, RoutedEventArgs e)
        {
            this.startScreen.Shown = false;
            this.startScreen.IsOpen = true;
        }

        private void HandleAddItemToFontsClick(object sender, RoutedEventArgs e)
        {
            this.viewModel.FontsViewModel.FontsData.Add($"Added item {this.viewModel.FontsViewModel.FontsData.Count}");
        }
    }

    public class TestRoutedCommand
    {
        public static RoutedCommand TestPresenterCommand = new RoutedCommand("TestPresenterCommand", typeof(TestRoutedCommand));

        public ICommand ItemCommand
        {
            get { return TestPresenterCommand; }
        }

        public CommandBinding ItemCommandBinding
        {
            get { return new CommandBinding(TestPresenterCommand, this.OnTestCommandExecuted, this.CanExecuteTestCommand); }
        }

        private void CanExecuteTestCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnTestCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("TestPresenterCommand");
        }
    }
}