namespace FluentTest.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;
    using Fluent;
    using FluentTest.Commanding;

    public class MainViewModel : ViewModel
    {
        private int boundSpinnerValue;
        private ColorViewModel colorViewModel;
        private FontsViewModel fontsViewModel;
        private GalleryViewModel galleryViewModel;

        private GallerySampleDataItemViewModel[] dataItems;

        public MainViewModel()
        {
            this.Title = string.Format("Fluent Ribbon Control Suite {0}", GetVersionText());
            this.Zoom = 1.0;

            this.BoundSpinnerValue = 1;

            this.ColorViewModel = new ColorViewModel();
            this.FontsViewModel = new FontsViewModel();
            this.GalleryViewModel = new GalleryViewModel();

            this.PreviewCommand = new RelayCommand<GalleryItem>(this.Preview);
            this.CancelPreviewCommand = new RelayCommand<GalleryItem>(this.CancelPreview);
        }

        private static string GetVersionText()
        {
            var version = typeof(Ribbon).Assembly.GetName().Version;

            var attributes = typeof(Ribbon).Assembly
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                    as AssemblyInformationalVersionAttribute[];

            var attrib = attributes.FirstOrDefault();

            return string.Format("{0} ({1})", version, attrib.InformationalVersion);
        }

        private void Preview(GalleryItem galleryItem)
        {
            Trace.WriteLine(string.Format("Preview: {0}", galleryItem));
        }

        private void CancelPreview(GalleryItem galleryItem)
        {
            Trace.WriteLine(string.Format("CancelPreview: {0}", galleryItem));
        }

        public string Title { get; private set; }

        public double Zoom
        {
            get { return this.zoom; }
            set
            {
                if (value.Equals(this.zoom)) return;
                this.zoom = value;
                this.OnPropertyChanged("Zoom");
            }
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

        /// <summary>
        /// Gets data items (uses as DataContext)
        /// </summary>
        public GallerySampleDataItemViewModel[] DataItems
        {
            get
            {
                return this.dataItems ?? (this.dataItems = new[]
                {
                    GallerySampleDataItemViewModel.Create("Images\\Blue.png", "Images\\BlueLarge.png", "Blue", "Group A"),
                    GallerySampleDataItemViewModel.Create("Images\\Brown.png", "Images\\BrownLarge.png", "Brown", "Group A"),
                    GallerySampleDataItemViewModel.Create("Images\\Gray.png", "Images\\GrayLarge.png", "Gray", "Group A"),
                    GallerySampleDataItemViewModel.Create("Images\\Green.png", "Images\\GreenLarge.png", "Green", "Group A"),
                    GallerySampleDataItemViewModel.Create("Images\\Orange.png", "Images\\OrangeLarge.png", "Orange", "Group A"),
                    GallerySampleDataItemViewModel.Create("Images\\Pink.png", "Images\\PinkLarge.png", "Pink", "Group B"),
                    GallerySampleDataItemViewModel.Create("Images\\Red.png", "Images\\RedLarge.png", "Red", "Group B"),
                    GallerySampleDataItemViewModel.Create("Images\\Yellow.png", "Images\\YellowLarge.png", "Yellow", "Group B")
                });
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

        #region Exit

        private RelayCommand exitCommand;
        private double zoom;

        /// <summary>
        /// Exit from the application
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                if (this.exitCommand == null)
                {
                    this.exitCommand = new RelayCommand(System.Windows.Application.Current.Shutdown, () => this.BoundSpinnerValue > 0);
                }

                return this.exitCommand;
            }
        }

        #endregion
    }
}