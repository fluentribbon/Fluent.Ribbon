using System.Collections.ObjectModel;

namespace FluentTest.ViewModels
{
    public class FontsViewModel : ViewModel
    {
        public ObservableCollection<string> FontsData { get; } = new ObservableCollection<string> { "Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol" };
    }
}
