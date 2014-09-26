namespace FluentTest.ViewModels
{
    using System.Diagnostics;
    using System.Windows.Input;
    using Fluent;
    using FluentTest.Commanding;

    public class MainViewModel : ViewModel
    {
        public static MainViewModel DesignTimeInstance 
        {
            get 
            { 
                var result = new MainViewModel();
                result.BoundSpinnerValue = 100;
                return result;
            }
        }

        public MainViewModel()
        {
            this.BoundSpinnerValue = 1;

            this.ColorViewModel = new ColorViewModel();
            this.FontsViewModel = new FontsViewModel();
            this.GalleryViewModel = new GalleryViewModel();

            this.PreviewCommand = new RelayCommand<GalleryItem>(this.Preview);
            this.CancelPreviewCommand = new RelayCommand<GalleryItem>(this.CancelPreview);
        }

        private void Preview(GalleryItem galleryItem)
        {
            Trace.WriteLine(string.Format("Preview: {0}", galleryItem));
        }

        private void CancelPreview(GalleryItem galleryItem)
        {
            Trace.WriteLine(string.Format("CancelPreview: {0}", galleryItem));
        }

        public ColorViewModel ColorViewModel
        {
            get { return this.colorViewModel; }
            private set
            {
                if (Equals(value, this.colorViewModel)) return;
                this.colorViewModel = value;
                this.OnPropertyChanged("ColorViewModel");
            }
        }

        public FontsViewModel FontsViewModel
        {
            get { return this.fontsViewModel; }
            private set
            {
                if (Equals(value, this.fontsViewModel)) return;
                this.fontsViewModel = value;
                this.OnPropertyChanged("FontsViewModel");
            }
        }

        public GalleryViewModel GalleryViewModel
        {
            get { return this.galleryViewModel; }
            private set
            {
                if (Equals(value, this.galleryViewModel)) return;
                this.galleryViewModel = value;
                this.OnPropertyChanged("GalleryViewModel");
            }
        }        

        public ICommand PreviewCommand { get; private set; }

        public ICommand CancelPreviewCommand { get; private set; }

        public int BoundSpinnerValue
        {
            get { return this.boundSpinnerValue; }
            set
            {
                this.boundSpinnerValue = value;
                this.OnPropertyChanged("BoundSpinnerValue");
            }
        }

        private int boundSpinnerValue;
        private ColorViewModel colorViewModel;
        private FontsViewModel fontsViewModel;
        private GalleryViewModel galleryViewModel;
    }
}