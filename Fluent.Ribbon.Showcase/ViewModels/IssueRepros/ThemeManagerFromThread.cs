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

        public enum ThemeAccents
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

            var newTheme = (ThemeAccents)this.currentTheme;

            this.Info("Changing theme to " + newTheme);
            this.ChangeTheme(newTheme);

            this.currentTheme++;
        }

        private void ChangeTheme(ThemeAccents themeAccent)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<ThemeAccents>(this.ChangeTheme), themeAccent);
            }
            else
            {
                var newAccent = ThemeManager.GetAccent(themeAccent.ToString());
                if (newAccent != null)
                {
                    var theme = ThemeManager.DetectAppStyle(Application.Current);
                    ThemeManager.ChangeAppStyle(Application.Current, newAccent, theme.Item1);

                    this.Info($"Change theme: NewTheme: {newAccent.Name} Theme changed.");
                }
                else
                {
                    this.Info($"Change theme: Theme not found: {themeAccent}.");
                }
            }
        }

        private void ThemeManagerIsThemeChangedHandler(object sender, OnThemeChangedEventArgs e)
        {
            try
            {
                var theme = ThemeManager.DetectAppStyle(Application.Current);
                this.Info($"Current theme from args: {e.Accent.Name}");
                this.Info($"Current theme from detection: {theme.Item2.Name}");
            }
            catch (Exception ex)
            {
                this.Info(ex.Message);
            }
        }
    }
}