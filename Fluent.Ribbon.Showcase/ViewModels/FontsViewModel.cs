namespace FluentTest.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;

    public class FontsViewModel : ViewModel
    {
        public ObservableCollection<string> FontsData { get; } = new ObservableCollection<string>(System.Windows.Media.Fonts.SystemFontFamilies.Select(fontFamily => fontFamily.ToString()));
    }
}
