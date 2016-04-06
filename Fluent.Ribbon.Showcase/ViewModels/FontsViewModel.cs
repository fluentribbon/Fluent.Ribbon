using System.Collections.ObjectModel;

namespace FluentTest.ViewModels
{
    public class FontsViewModel : ViewModel
    {
        private readonly ObservableCollection<string> data = new ObservableCollection<string> { "Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol" };

        public ObservableCollection<string> FontsData
        {
            get { return this.data; }
        }
    }
}
