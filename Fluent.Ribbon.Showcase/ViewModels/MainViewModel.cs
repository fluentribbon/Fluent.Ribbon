﻿namespace FluentTest.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Fluent;
using FluentTest.Commanding;
#if MahApps_Metro

#endif

#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class MainViewModel : ViewModel
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly Timer memoryTimer;

    private int boundSpinnerValue;
    private ColorViewModel colorViewModel = null!;
    private MenuViewModel menuViewModel = null!;
    private FontsViewModel fontsViewModel = null!;
    private GalleryViewModel galleryViewModel = null!;

    private double zoom;

    private bool? isCheckedToggleButton3 = true;

    private bool areContextGroupsVisible = true;
    private bool isBackstageOpen;

    public MainViewModel()
    {
        this.Zoom = 1.0;

        this.BoundSpinnerValue = 1;
        this.IsCheckedToggleButton3 = true;

        this.ColorViewModel = new ColorViewModel();
        this.MenuViewModel = new MenuViewModel();
        this.FontsViewModel = new FontsViewModel();
        this.GalleryViewModel = new GalleryViewModel();
        this.IssueReprosViewModel = new IssueReprosViewModel();

        this.ExitCommand = new RelayCommand(Application.Current.Shutdown, () => this.BoundSpinnerValue > 0);
        this.TestCommand = new RelayCommand(() => MessageBox.Show("Test-Command"));
        this.PreviewCommand = new RelayCommand<GalleryItem>(Preview);
        this.CancelPreviewCommand = new RelayCommand<GalleryItem>(CancelPreview);

        this.DataItems = new(
        [
            GallerySampleDataItemViewModel.Create("Images\\Blue.png", "Images\\BlueLarge.png", "Blue", "Group A"),
            GallerySampleDataItemViewModel.Create("Images\\Brown.png", "Images\\BrownLarge.png", "Brown", "Group A"),
            GallerySampleDataItemViewModel.Create("Images\\Gray.png", "Images\\GrayLarge.png", "Gray", "Group A"),
            GallerySampleDataItemViewModel.Create("Images\\Green.png", "Images\\GreenLarge.png", "Green", "Group A"),
            GallerySampleDataItemViewModel.Create("Images\\Orange.png", "Images\\OrangeLarge.png", "Orange", "Group A"),
            GallerySampleDataItemViewModel.Create("Images\\Pink.png", "Images\\PinkLarge.png", "Pink", "Group B"),
            GallerySampleDataItemViewModel.Create("Images\\Red.png", "Images\\RedLarge.png", "Red", "Group B"),
            GallerySampleDataItemViewModel.Create("Images\\Yellow.png", "Images\\YellowLarge.png", "Yellow", "Group B")
        ]);
        this.ManyItems = GenerateStrings(5000);
        this.StringItems = GenerateStrings(25);
        
        this.GroupByAdvancedSample = x => ((GallerySampleDataItemViewModel)x).Text.Substring(0, 1);

        this.memoryTimer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds);
        this.memoryTimer.Elapsed += this.HandleMemoryTimer_Elapsed;
        this.memoryTimer.Start();
    }

    #region Properties

    public long UsedMemory => GC.GetTotalMemory(true) / 1014;

    public double Zoom
    {
        get => this.zoom;

        set
        {
            if (value.Equals(this.zoom))
            {
                return;
            }

            this.zoom = value;
            this.OnPropertyChanged();
        }
    }

    public bool AreContextGroupsVisible
    {
        get => this.areContextGroupsVisible;
        set
        {
            if (value == this.areContextGroupsVisible)
            {
                return;
            }

            this.areContextGroupsVisible = value;
            this.OnPropertyChanged();
        }
    }

    public bool IsBackstageOpen
    {
        get => this.isBackstageOpen;
        set
        {
            if (value == this.isBackstageOpen)
            {
                return;
            }

            this.isBackstageOpen = value;
            this.OnPropertyChanged();
        }
    }

    public ColorViewModel ColorViewModel
    {
        get => this.colorViewModel;

        private set
        {
            if (Equals(value, this.colorViewModel))
            {
                return;
            }

            this.colorViewModel = value;
            this.OnPropertyChanged();
        }
    }

    public MenuViewModel MenuViewModel
    {
        get => this.menuViewModel;

        private set
        {
            if (Equals(value, this.menuViewModel))
            {
                return;
            }

            this.menuViewModel = value;
            this.OnPropertyChanged();
        }
    }

    public FontsViewModel FontsViewModel
    {
        get => this.fontsViewModel;

        private set
        {
            if (Equals(value, this.fontsViewModel))
            {
                return;
            }

            this.fontsViewModel = value;
            this.OnPropertyChanged();
        }
    }

    public GalleryViewModel GalleryViewModel
    {
        get => this.galleryViewModel;

        private set
        {
            if (Equals(value, this.galleryViewModel))
            {
                return;
            }

            this.galleryViewModel = value;
            this.OnPropertyChanged();
        }
    }

    public IssueReprosViewModel IssueReprosViewModel { get; }

    /// <summary>
    /// Gets data items (uses as DataContext)
    /// </summary>
    public ReadOnlyObservableCollection<GallerySampleDataItemViewModel> DataItems { get; }

    public Func<object, string> GroupByAdvancedSample { get; private set; }

    public IList<string> ManyItems { get; }

    public IList<string> StringItems { get; }

    public bool? IsCheckedToggleButton3
    {
        get => this.isCheckedToggleButton3;

        set
        {
            if (value == this.isCheckedToggleButton3)
            {
                return;
            }

            this.isCheckedToggleButton3 = value;
            this.OnPropertyChanged();
        }
    }

    public ICommand PreviewCommand { get; private set; }

    public ICommand CancelPreviewCommand { get; private set; }

    public int BoundSpinnerValue
    {
        get => this.boundSpinnerValue;

        set
        {
            if (value == this.boundSpinnerValue)
            {
                return;
            }

            this.boundSpinnerValue = value;
            this.OnPropertyChanged();
        }
    }

    #region Exit

    /// <summary>
    /// Exit from the application
    /// </summary>
    public ICommand ExitCommand { get; }

    #endregion

    public ICommand TestCommand { get; }

    #endregion Properties

    private static void Preview(GalleryItem? galleryItem)
    {
        Trace.WriteLine($"Preview: {galleryItem}");
    }

    private static void CancelPreview(GalleryItem? galleryItem)
    {
        Trace.WriteLine($"CancelPreview: {galleryItem}");
    }

    private static IList<string> GenerateStrings(int count)
    {
        return Enumerable.Range(0, count)
            .Select(x => "Item " + (x + 1).ToString())
            .ToList();
    }

    private void HandleMemoryTimer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        this.OnPropertyChanged(nameof(this.UsedMemory));
    }
}