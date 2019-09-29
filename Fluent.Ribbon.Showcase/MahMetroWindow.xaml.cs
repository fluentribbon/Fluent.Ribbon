namespace FluentTest
{
    using System;
    using System.Linq;
    using System.Windows;
    using Fluent;
    using MahApps.Metro.Controls;

    [CLSCompliant(false)] // Because MetroWindow is not CLSCompliant
    public partial class MahMetroWindow : MetroWindow, IRibbonWindow
    {
        public MahMetroWindow()
        {
            this.InitializeComponent();

            this.TestContent.Backstage.UseHighestAvailableAdornerLayer = false;

            this.Loaded += this.MahMetroWindow_Loaded;
            this.Closed += this.MahMetroWindow_Closed;
        }

        private void MahMetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.TitleBar = this.FindChild<RibbonTitleBar>("RibbonTitleBar");
            this.TitleBar.InvalidateArrange();
            this.TitleBar.UpdateLayout();

            this.SyncThemeManagers(null, null);

            ThemeManager.IsThemeChanged += this.SyncThemeManagers;
        }

        private void MahMetroWindow_Closed(object sender, EventArgs e)
        {
            ThemeManager.IsThemeChanged -= this.SyncThemeManagers;
        }

        private void SyncThemeManagers(object sender, OnThemeChangedEventArgs args)
        {
            // Sync Fluent and MahApps ThemeManager
            var fluentRibbonTheme = args?.Theme ?? ThemeManager.DetectTheme();
            MahApps.Metro.ThemeManager.ChangeTheme(this, fluentRibbonTheme.Name);
        }

        #region TitelBar

        /// <summary>
        /// Gets ribbon titlebar
        /// </summary>
        public RibbonTitleBar TitleBar
        {
            get { return (RibbonTitleBar)this.GetValue(TitleBarProperty); }
            private set { this.SetValue(TitleBarPropertyKey, value); }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly DependencyPropertyKey TitleBarPropertyKey = DependencyProperty.RegisterReadOnly(nameof(TitleBar), typeof(RibbonTitleBar), typeof(MahMetroWindow), new PropertyMetadata());

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="TitleBar"/>.
        /// </summary>
        public static readonly DependencyProperty TitleBarProperty = TitleBarPropertyKey.DependencyProperty;

        #endregion
    }
}