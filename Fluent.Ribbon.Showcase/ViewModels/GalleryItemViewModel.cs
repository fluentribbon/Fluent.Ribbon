namespace FluentTest.ViewModels;

public class GalleryItemViewModel : ViewModel
{
    private string text = null!;
    private string group = null!;

    public GalleryItemViewModel(string group, string text)
    {
        this.Group = group;
        this.Text = text;
    }

    public string Text
    {
        get => this.text;

        set
        {
            if (value == this.text)
            {
                return;
            }

            this.text = value;
            this.OnPropertyChanged();
        }
    }

    public string Group
    {
        get => this.group;

        set
        {
            if (value == this.group)
            {
                return;
            }

            this.group = value;
            this.OnPropertyChanged();
        }
    }
}