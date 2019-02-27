#pragma warning disable SA1402 // File may only contain a single class

namespace FluentTest.ViewModels
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
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

        public IEnumerable<AppTheme> AppThemes { get; } = ThemeManager.AppThemes;

        public IEnumerable<AccentItem> Accents { get; } = ThemeManager.Accents.Select(x => new AccentItem(x)).ToList();

        public AppTheme CurrentAppTheme
        {
            get { return ThemeManager.DetectAppStyle(Application.Current).Item1; }
            set { ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.DetectAppStyle().Item2, value); }
        }

        public AccentItem CurrentAccent
        {
            get
            {
                var accent = ThemeManager.DetectAppStyle(Application.Current).Item2;
                return this.Accents.FirstOrDefault(x => x.Accent.Name == accent.Name);
            }

            set { ThemeManager.ChangeAppStyle(Application.Current, value.Accent, this.CurrentAppTheme); }
        }
    }

    [DebuggerDisplay("accent={Accent.Name}")]
    public class AccentItem
    {
        public AccentItem(Accent accent)
        {
            this.Accent = accent;
            this.AccentBaseColor = (Brush)accent.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"];
        }

        public Accent Accent { get; }

        public Brush AccentBaseColor { get; }
    }
}