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

        public IEnumerable<Theme> Themes { get; } = ThemeManager.Themes.ToList();

        public Theme CurrentTheme
        {
            get
            {
                var theme = ThemeManager.DetectTheme(Application.Current);
                return this.Themes.FirstOrDefault(x => x.Name == theme.Name);
            }

            set => ThemeManager.ChangeTheme(Application.Current, value);
        }
    }
}