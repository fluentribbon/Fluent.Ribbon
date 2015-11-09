﻿namespace FluentTest.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using FluentTest.Commanding;

    public class GalleryViewModel : ViewModel
    {
        private ObservableCollection<GalleryItemViewModel> items;

        public GalleryViewModel()
        {
            this.Items = new ObservableCollection<GalleryItemViewModel>();
            this.RefreshCommand = new RelayCommand(this.Refresh);

            this.Refresh();
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

        public ICommand RefreshCommand { get; private set; }

        public void Refresh()
        {
            this.Items.Clear();

            this.Items.Add(new GalleryItemViewModel("Group 1", "1"));
            this.Items.Add(new GalleryItemViewModel("Group 1", "2"));
            this.Items.Add(new GalleryItemViewModel("Group 1", "3"));
            this.Items.Add(new GalleryItemViewModel("Group 1", "4"));
            this.Items.Add(new GalleryItemViewModel("Group 1", "5"));
            this.Items.Add(new GalleryItemViewModel("Group 1", "6"));
            this.Items.Add(new GalleryItemViewModel("Group 2", "10"));
            this.Items.Add(new GalleryItemViewModel("Group 2", "20"));
            this.Items.Add(new GalleryItemViewModel("Group 2", "30"));
            this.Items.Add(new GalleryItemViewModel("Group 2", "40"));
            this.Items.Add(new GalleryItemViewModel("Group 2", "50"));
            this.Items.Add(new GalleryItemViewModel("Group 2", "60"));
        }
    }
}