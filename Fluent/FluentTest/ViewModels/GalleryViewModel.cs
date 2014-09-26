namespace FluentTest.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class GalleryViewModel : ViewModel
    {
        private ObservableCollection<GalleryItemViewModel> items;

        public GalleryViewModel()
        {
            this.Items = new ObservableCollection<GalleryItemViewModel> 
                {
                    new GalleryItemViewModel("Group 1", "1"),
                    new GalleryItemViewModel("Group 1", "2"),
                    new GalleryItemViewModel("Group 1", "3"),
                    new GalleryItemViewModel("Group 1", "4"),
                    new GalleryItemViewModel("Group 1", "5"),
                    new GalleryItemViewModel("Group 1", "6"),

                    new GalleryItemViewModel("Group 2", "10"),
                    new GalleryItemViewModel("Group 2", "20"),
                    new GalleryItemViewModel("Group 2", "30"),
                    new GalleryItemViewModel("Group 2", "40"),
                    new GalleryItemViewModel("Group 2", "50"),
                    new GalleryItemViewModel("Group 2", "60"),
                };
        }

        public ObservableCollection<GalleryItemViewModel> Items
        {
            get { return this.items; }
            private set
            {
                if (Equals(value, this.items)) return;
                this.items = value;
                this.OnPropertyChanged("Items");
            }
        }
    }
}