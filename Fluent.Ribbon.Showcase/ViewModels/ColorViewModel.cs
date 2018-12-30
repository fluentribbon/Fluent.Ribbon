#pragma warning disable SA1402 // File may only contain a single class

namespace FluentTest.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;
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

            CollectionViewSource.GetDefaultView(ThemeManager.Themes).GroupDescriptions.Add(new PropertyGroupDescription(nameof(Theme.BaseColorScheme)));
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
            get => ((SolidColorBrush)Application.Current.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"])?.Color ?? Colors.Pink;

            set
            {
                var solidColorBrush = new SolidColorBrush(value);
                solidColorBrush.Freeze();
                Application.Current.Resources["Fluent.Ribbon.Brushes.AccentBaseColorBrush"] = solidColorBrush;
                this.OnPropertyChanged(nameof(this.ThemeColor));
            }
        }

        public string CurrentBaseColor
        {
            get => this.CurrentTheme.BaseColorScheme;

            set
            {
                ThemeManager.ChangeThemeBaseColor(Application.Current, value);
                this.OnPropertyChanged(nameof(this.CurrentBaseColor));
                this.OnPropertyChanged(nameof(this.CurrentTheme));
            }
        }

        public Theme CurrentTheme
        {
            get => ThemeManager.DetectTheme(Application.Current);

            set
            {
                ThemeManager.ChangeTheme(Application.Current, value);
                this.OnPropertyChanged(nameof(this.CurrentTheme));
                this.OnPropertyChanged(nameof(this.CurrentBaseColor));
            }
        }
    }
}