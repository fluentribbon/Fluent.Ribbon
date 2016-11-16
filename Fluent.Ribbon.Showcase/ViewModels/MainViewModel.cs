namespace FluentTest.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Timers;
    using System.Windows;
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

        private RelayCommand exitCommand;
        private double zoom;
        private ICommand testCommand;
        private IList<string> manyItems;
        private bool? isCheckedToggleButton3;

        private readonly Timer memoryTimer;

        public MainViewModel()
        {
            this.Title = $"Fluent.Ribbon {GetVersionText()}";
            this.Zoom = 1.0;

            this.BoundSpinnerValue = 1;
            this.IsCheckedToggleButton3 = true;

            this.ColorViewModel = new ColorViewModel();
            this.FontsViewModel = new FontsViewModel();
            this.GalleryViewModel = new GalleryViewModel();

            this.PreviewCommand = new RelayCommand<GalleryItem>(Preview);
            this.CancelPreviewCommand = new RelayCommand<GalleryItem>(CancelPreview);

            this.GroupByAdvancedSample = x => ((GallerySampleDataItemViewModel)x).Text.Substring(0, 1);

            this.memoryTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
            this.memoryTimer.Elapsed += this.HandleMemoryTimer_Elapsed;
            this.memoryTimer.Start();

            ////string.Format("{0:##,000}", this.UsedMemory)
        }

        #region Properties

        public string Title { get; private set; }

        public long UsedMemory => GC.GetTotalMemory(true) / 1014;

        public double Zoom
        {
            get { return this.zoom; }
            set
            {
                if (value.Equals(this.zoom)) return;
                this.zoom = value;
                this.OnPropertyChanged(nameof(this.Zoom));
            }
        }

        public ColorViewModel ColorViewModel
        {
            get { return this.colorViewModel; }
            private set
            {
                if (Equals(value, this.colorViewModel)) return;
                this.colorViewModel = value;
                this.OnPropertyChanged(nameof(this.ColorViewModel));
            }
        }

        public FontsViewModel FontsViewModel
        {
            get { return this.fontsViewModel; }
            private set
            {
                if (Equals(value, this.fontsViewModel)) return;
                this.fontsViewModel = value;
                this.OnPropertyChanged(nameof(this.FontsViewModel));
            }
        }

        public GalleryViewModel GalleryViewModel
        {
            get { return this.galleryViewModel; }
            private set
            {
                if (Equals(value, this.galleryViewModel)) return;
                this.galleryViewModel = value;
                this.OnPropertyChanged(nameof(this.GalleryViewModel));
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

        public Func<object, string> GroupByAdvancedSample { get; private set; }

        public IList<string> ManyItems
        {
            get { return this.manyItems ?? (this.manyItems = this.GenerateStrings(5000)); }
        }

        public bool? IsCheckedToggleButton3
        {
            get { return this.isCheckedToggleButton3; }
            set
            {
                if (this.isCheckedToggleButton3 != value)
                {
                    this.isCheckedToggleButton3 = value;
                    this.OnPropertyChanged(nameof(this.IsCheckedToggleButton3));
                }
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
                this.OnPropertyChanged(nameof(this.BoundSpinnerValue));
            }
        }

        #region Exit

        /// <summary>
        /// Exit from the application
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                if (this.exitCommand == null)
                {
                    this.exitCommand = new RelayCommand(Application.Current.Shutdown, () => this.BoundSpinnerValue > 0);
                }

                return this.exitCommand;
            }
        }

        #endregion

        public ICommand TestCommand
        {
            get
            {
                if (this.testCommand == null)
                {
                    this.testCommand = new RelayCommand(() => MessageBox.Show("Test-Command"));
                }

                return this.testCommand;
            }
        }

        #endregion Properties

        private static string GetVersionText()
        {
            var version = typeof(Ribbon).Assembly.GetName().Version;

            var attributes = typeof(Ribbon).Assembly
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                    as AssemblyInformationalVersionAttribute[];

            var attrib = attributes.FirstOrDefault();

            return $"{version} ({attrib.InformationalVersion})";
        }

        private static void Preview(GalleryItem galleryItem)
        {
            Trace.WriteLine($"Preview: {galleryItem}");
        }

        private static void CancelPreview(GalleryItem galleryItem)
        {
            Trace.WriteLine($"CancelPreview: {galleryItem}");
        }

        private IList<string> GenerateStrings(int count)
        {
            return Enumerable.Repeat("Test", count).ToList();
        }

        private void HandleMemoryTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.OnPropertyChanged(nameof(this.UsedMemory));
        }
    }
}