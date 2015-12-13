namespace FluentTest.ViewModels
{
    public class GalleryItemViewModel
    {
        public GalleryItemViewModel(string group, string text)
        {
            this.Group = group;
            this.Text = text;
        }

        public string Text { get; set; }

        public string Group { get; set; }
    }
}