namespace FluentTest
{
    using System;
    using System.Linq;
    using System.Windows;
    using ControlzEx.Theming;
    using Fluent;
    using MahApps.Metro.Controls;

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

            ThemeManager.ThemeChanged += this.SyncThemeManagers;
        }

        private void MahMetroWindow_Closed(object sender, EventArgs e)
        {
            ThemeManager.ThemeChanged -= this.SyncThemeManagers;
        }

        private void SyncThemeManagers(object sender, ThemeChangedEventArgs args)
        {
            // Sync Fluent and MahApps ThemeManager
            var fluentRibbonTheme = args?.NewTheme ?? ThemeManager.DetectTheme();
            var newMahAppsMetroTheme = MahApps.Metro.ThemeManager.ChangeTheme(this, fluentRibbonTheme.Name);

            if (newMahAppsMetroTheme.Name != fluentRibbonTheme.Name)
            {
                var mostLikelyMatchingTheme = MahApps.Metro.ThemeManager.Themes.FirstOrDefault(x => x.BaseColorScheme == fluentRibbonTheme.BaseColorScheme
                                                                                                    && x.ShowcaseBrush?.ToString() == fluentRibbonTheme.ShowcaseBrush?.ToString());

                if (mostLikelyMatchingTheme != null)
                {
                    newMahAppsMetroTheme = MahApps.Metro.ThemeManager.ChangeTheme(this, mostLikelyMatchingTheme);
                }
            }

            if (newMahAppsMetroTheme.BaseColorScheme != fluentRibbonTheme.BaseColorScheme)
            {
                MahApps.Metro.ThemeManager.ChangeThemeBaseColor(this, fluentRibbonTheme.BaseColorScheme);
            }
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