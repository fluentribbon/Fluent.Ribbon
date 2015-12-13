namespace FluentTest.ViewModels
{
    public class FontsViewModel : ViewModel
    {
        private readonly string[] data = { "Tahoma", "Segoe UI", "Arial", "Courier New", "Symbol" };

        public string[] FontsData
        {
            get { return this.data; }
        }
    }
}
