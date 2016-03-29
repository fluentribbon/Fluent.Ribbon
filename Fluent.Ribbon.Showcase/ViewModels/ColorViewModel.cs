namespace FluentTest.ViewModels
{
    using System.Windows;
    using System.Windows.Media;

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
            get { return ((SolidColorBrush)Application.Current.Resources["RibbonThemeColorBrush"])?.Color ?? Colors.Pink; }
            set
            {
                Application.Current.Resources["RibbonThemeColorBrush"] = new SolidColorBrush(value);
                this.OnPropertyChanged(nameof(this.ThemeColor));
            }
        }
    }
}