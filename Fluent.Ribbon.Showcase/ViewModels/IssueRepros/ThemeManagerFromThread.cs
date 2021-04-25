namespace FluentTest.ViewModels.IssueRepros
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using ControlzEx.Theming;
    using Fluent.Extensions;
    using FluentTest.Commanding;

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public class ThemeManagerFromThread
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
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
            if (this.cancellationTokenSource is not null)
            {
                ThemeManager.Current.ThemeChanged -= this.ThemeManagerThemeChangedHandler;

                this.cancellationTokenSource.Cancel();
            }
            else
            {
                ThemeManager.Current.ThemeChanged += this.ThemeManagerThemeChangedHandler;

                Task.Factory.StartNew(async () =>
                    {
                        while (this.cancellationTokenSource.IsCancellationRequested == false)
                        {
                            this.ThreadProc();

                            await Task.Delay(TimeSpan.FromSeconds(2));
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
                Application.Current.RunInDispatcherAsync(() => this.ChangeTheme(themeColor));
            }
            else
            {
                var newTheme = ThemeManager.Current.GetTheme("Light." + themeColor.ToString());
                if (newTheme is not null)
                {
                    ThemeManager.Current.ChangeTheme(Application.Current, newTheme);

                    this.Info($"Change theme: NewTheme: {newTheme.Name} Theme changed.");
                }
                else
                {
                    this.Info($"Change theme: Theme not found: {themeColor}.");
                }
            }
        }

        private void ThemeManagerThemeChangedHandler(object sender, ThemeChangedEventArgs e)
        {
            try
            {
                var theme = ThemeManager.Current.DetectTheme(Application.Current);
                this.Info($"Current theme from args: {e.NewTheme.Name}");
                this.Info($"Current theme from detection: {theme.Name}");
            }
            catch (Exception ex)
            {
                this.Info(ex.Message);
            }
        }
    }
}