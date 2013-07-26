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

namespace FluentTest
{
    /// <summary>
    /// Interaktionslogik für MetroStyle.xaml
    /// </summary>
    public partial class MetroStyle
    {
        public MetroStyle()
        {
            InitializeComponent();
            ScreenTip.HelpPressed += this.OnScreenTipHelpPressed;
            ribbon.DataContext = this;
            DataContext = this;
        }

        private readonly string[] data = new[] { "Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol" };
        public string[] FontsData
        {
            get { return data; }
        }

        private readonly Color[] themeColors = new[] { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple };
        public Color[] ThemeColors
        {
            get { return themeColors; }
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
            this.Close();
        }

        private void OnBlackClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnBlueClick(object sender, RoutedEventArgs e)
        {
            this.Close();
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
}
