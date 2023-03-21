namespace FluentTest;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

public partial class ImagesViewer
{
    public ImagesViewer()
    {
        this.InitializeComponent();
    }
}

public class ImagesViewerDesignTimeData
{
    public ImagesViewerDesignTimeData()
    {
        var dictionary = new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Images.xaml", UriKind.Absolute) };

        foreach (var key in dictionary.Keys.Cast<string>().OrderBy(x => x))
        {
#pragma warning disable CA1307
            this.Data.Add(new KeyValuePair<string, DrawingImage>(key.Replace("Fluent.Ribbon.Images.", string.Empty), (DrawingImage)dictionary[key]));
#pragma warning restore CA1307
        }
    }

    public ObservableCollection<KeyValuePair<string, DrawingImage>> Data { get; } = new();
}