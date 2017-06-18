namespace FluentTest.ViewModels
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using Fluent;

    public class ColorViewModel : ViewModel
    {
        private Color standardColor;
        private Color highlightColor;

        public ColorViewModel()
        {
            this.StandardColor = Colors.Black;
            this.HighlightColor = Colors.Yellow;
        }

        public Color StandardColor
        {
            get { return this.standardColor; }
            set
            {
                this.standardColor = value;
                this.OnPropertyChanged(nameof(this.StandardColor));
            }
        }

        public Color HighlightColor
        {
            get { return this.highlightColor; }
            set
            {
                this.highlightColor = value;
                this.OnPropertyChanged(nameof(this.HighlightColor));
            }
        }

        public Color[] ThemeColors { get; } = { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple };

        public Color ThemeColor
        {
            get { return ((SolidColorBrush)Application.Current.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"])?.Color ?? Colors.Pink; }
            set
            {
                Application.Current.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"] = new SolidColorBrush(value);
                this.OnPropertyChanged(nameof(this.ThemeColor));
            }
        }

        public IEnumerable<AppTheme> AppThemes
        {
            get { return ThemeManager.AppThemes; }
        }

        public IEnumerable<Accent> Accents
        {
            get { return ThemeManager.Accents; }
        }

        public AppTheme CurrentAppTheme
        {
            get { return ThemeManager.DetectAppStyle().Item1; }
            set { ThemeManager.ChangeAppStyle(Application.Current, this.CurrentAccent, value); }
        }

        public Accent CurrentAccent
        {
            get { return ThemeManager.DetectAppStyle().Item2; }
            set { ThemeManager.ChangeAppStyle(Application.Current, value, this.CurrentAppTheme); }
        }
    }
}