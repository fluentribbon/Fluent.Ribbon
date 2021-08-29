namespace FluentTest
{
    public partial class SimplifiedRibbonWindow
    {
        public SimplifiedRibbonWindow()
        {
            this.InitializeComponent();
        }

        private void Click_AddToggleButton1(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ResizeTab11.Items.Add(new Fluent.ToggleButton() { Header = "test" });
        }

        private void Click_AddToggleButton2(object sender, System.Windows.RoutedEventArgs e)
        {
            this.ResizeTab21.Items.Add(new Fluent.ToggleButton() { Header = "test" });
        }
    }
}
