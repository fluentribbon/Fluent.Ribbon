namespace FluentTest
{
    using System;
    using System.Windows;
    using Fluent;
    using MahApps.Metro.Controls;

    /// <summary>
    /// Interaction logic for MahMetroWindow.xaml
    /// </summary>
    [CLSCompliant(false)] // Because MetroWindow is not CLSCompliant
    public partial class MahMetroWindow : IRibbonWindow
    {
        private RibbonTitleBar titleBar;

        public MahMetroWindow()
        {
            this.InitializeComponent();

            //this.TitleBar = this.RibbonTitleBar;

            this.Loaded += MahMetroWindow_Loaded;
        }

        private void MahMetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        public double TitleBarHeight { get; } = 28d;

        public RibbonTitleBar TitleBar
        {
            get { return this.titleBar ?? (this.titleBar = this.FindChild<RibbonTitleBar>("RibbonTitleBar")); }
        }
    }
}