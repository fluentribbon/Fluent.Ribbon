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
        public MahMetroWindow()
        {
            this.InitializeComponent();

            this.Loaded += this.MahMetroWindow_Loaded;
        }

        private void MahMetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.TitleBar = this.FindChild<RibbonTitleBar>("RibbonTitleBar");
            this.TitleBar.InvalidateArrange();
            this.TitleBar.UpdateLayout();
        }

        #region TitelBar

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get { return (RibbonTitleBar)this.GetValue(TitleBarProperty); }
            private set { this.SetValue(titleBarPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey titleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(MahMetroWindow), new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = titleBarPropertyKey.DependencyProperty;

        #endregion
    }
}