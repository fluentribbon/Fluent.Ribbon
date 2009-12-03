using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
using Fluent;

namespace FluentTest
{
    /// <summary>
    /// Represents adorner for KeyTips. 
    /// KeyTipAdorners is chained to produce one from another. 
    /// Detaching root adorner couses detaching all adorners in the chain
    /// </summary>
    internal class SampleAdorner : Adorner
    {

        public Button Grid = new Button();

        /// <summary>
        /// Construcotor
        /// </summary>
        /// <param name="adornedElement"></param>
        /// <param name="parentAdorner">Parent adorner or null</param>
        public SampleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            Grid.Content = "LLL";
        }



        /// <summary>
        /// Positions child elements and determines
        /// a size for the control
        /// </summary>
        /// <param name="finalSize">The final area within the parent 
        /// that this element should use to arrange 
        /// itself and its children</param>
        /// <returns>The actual size used</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Grid.Arrange(new Rect(0, 0, 30, 30));
            return new Size(30, 30);
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index)
        {

            return Grid;
        }

        /// <summary>
        /// Measures KeyTips
        /// </summary>
        /// <param name="constraint">The available size that this element can give to child elements.</param>
        /// <returns>The size that the groups container determines it needs during 
        /// layout, based on its calculations of child element sizes.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            Grid.Measure(new Size(30, 30));
            return new Size(30, 30);
        }


        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }
    }
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        public TestWindow()
        {
            InitializeComponent();

           ScreenTip.HelpPressed += new EventHandler<ScreenTipHelpEventArgs>(OnScreenTipHelpPressed);
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
            MessageBox.Show("Launcher button pressed!!!");
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
            testDropButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            SampleAdorner ad = new SampleAdorner(testButton);
            GetAdornerLayer(testButton).Add(ad);
        }
    }
}
