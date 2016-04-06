namespace FluentTest.ViewModels
{
    public class GalleryItemViewModel : ViewModel
    {
        private string text;
        private string group;

        public GalleryItemViewModel(string group, string text)
        {
            this.Group = group;
            this.Text = text;
        }

        public string Text
        {
            get { return this.text; }

            set
            {
                if (value == this.text)
                {
                    return;
                }
                this.text = value;
                this.OnPropertyChanged(nameof(this.Text));
            }
        }

        public string Group
        {
            get { return this.group; }

            set
            {
                if (value == this.group)
                {
                    return;
                }
                this.group = value;
                this.OnPropertyChanged(nameof(this.Group));
            }
        }
    }
}