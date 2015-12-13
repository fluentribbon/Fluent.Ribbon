namespace FluentTest.ViewModels
{
    using System.Windows.Media;

    public class ColorViewModel : ViewModel
    {
        private Color standardColor;
        private Color highlightColor;

        private readonly Color[] themeColors = { Colors.Red, Colors.Green, Colors.Blue, Colors.White, Colors.Black, Colors.Purple };
        private Color themeColor;

        public ColorViewModel()
        {
            this.StandardColor = Colors.Black;
            this.HighlightColor = Colors.Yellow;
            this.ThemeColor = Colors.Blue;
        }

        public Color StandardColor
        {
            get { return this.standardColor; }
            set
            {
                this.standardColor = value;
                this.OnPropertyChanged("StandardColor");
            }
        }

        public Color HighlightColor
        {
            get { return this.highlightColor; }
            set
            {
                this.highlightColor = value;
                this.OnPropertyChanged("HighlightColor");
            }
        }

        public Color[] ThemeColors
        {
            get { return this.themeColors; }
        }

        public Color ThemeColor
        {
            get { return this.themeColor; }
            set
            {
                this.themeColor = value;
                this.OnPropertyChanged("ThemeColor");
            }
        }
    }
}