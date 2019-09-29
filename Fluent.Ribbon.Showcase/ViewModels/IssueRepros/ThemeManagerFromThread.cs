namespace FluentTest.ViewModels.IssueRepros
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Fluent;
    using FluentTest.Commanding;

    public class ThemeManagerFromThread
    {
        private int currentTheme;
        private CancellationTokenSource cancellationTokenSource;

        public enum ThemeColors
        {
            Red,
            Amber,
            Emerald,
        }

        public ThemeManagerFromThread()
        {
            this.StartStopCommand = new RelayCommand(this.StartStop);
        }

        public RelayCommand StartStopCommand { get; set; }

        private void Info(string info)
        {
            Trace.WriteLine($@"{DateTime.Now} {info}");
        }

        private void StartStop()
        {
            if (this.cancellationTokenSource != null)
            {
                ThemeManager.IsThemeChanged -= this.ThemeManagerIsThemeChangedHandler;

                this.cancellationTokenSource.Cancel();
            }
            else
            {
                ThemeManager.IsThemeChanged += this.ThemeManagerIsThemeChangedHandler;

                Task.Factory.StartNew(() =>
                    {
                        while (this.cancellationTokenSource.IsCancellationRequested == false)
                        {
                            this.ThreadProc();
                            Thread.Sleep(2000);
                        }

                        this.cancellationTokenSource.Dispose();
                        this.cancellationTokenSource = null;
                    }, (this.cancellationTokenSource = new CancellationTokenSource()).Token);
            }
        }

        private void ThreadProc()
        {
            if (this.currentTheme > 2)
            {
                this.currentTheme = 0;
            }

            var newTheme = (ThemeColors)this.currentTheme;

            this.Info("Changing theme to " + newTheme);
            this.ChangeTheme(newTheme);

            this.currentTheme++;
        }

        private void ChangeTheme(ThemeColors themeColor)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<ThemeColors>(this.ChangeTheme), themeColor);
            }
            else
            {
                var newTheme = ThemeManager.GetTheme("Light." + themeColor.ToString());
                if (newTheme != null)
                {
                    ThemeManager.ChangeTheme(Application.Current, newTheme);

                    this.Info($"Change theme: NewTheme: {newTheme.Name} Theme changed.");
                }
                else
                {
                    this.Info($"Change theme: Theme not found: {themeColor}.");
                }
            }
        }

        private void ThemeManagerIsThemeChangedHandler(object sender, OnThemeChangedEventArgs e)
        {
            try
            {
                var theme = ThemeManager.DetectTheme(Application.Current);
                this.Info($"Current theme from args: {e.Theme.Name}");
                this.Info($"Current theme from detection: {theme.Name}");
            }
            catch (Exception ex)
            {
                this.Info(ex.Message);
            }
        }
    }
}