namespace FluentTest
{
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using ControlzEx.Theming;

    public partial class ResourcesView
    {
        public ResourcesView()
        {
            this.InitializeComponent();

            this.ThemeResources = new ObservableCollection<ThemeResource>();

            var view = CollectionViewSource.GetDefaultView(this.ThemeResources);

            view.SortDescriptions.Add(new SortDescription(nameof(ThemeResource.Key), ListSortDirection.Ascending));

            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ThemeResource.Theme)));
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ThemeResource.LibraryTheme)));

            this.UpdateThemeAnalyzers(ThemeManager.Current.DetectTheme());

            ThemeManager.Current.ThemeChanged += this.ThemeManager_ThemeChanged;
        }

        public static readonly DependencyProperty ThemeResourcesProperty = DependencyProperty.Register(
            nameof(ThemeResources), typeof(ObservableCollection<ThemeResource>), typeof(ResourcesView), new PropertyMetadata(default(ObservableCollection<ThemeResource>)));

        public ObservableCollection<ThemeResource> ThemeResources
        {
            get => (ObservableCollection<ThemeResource>)this.GetValue(ThemeResourcesProperty);
            set => this.SetValue(ThemeResourcesProperty, value);
        }

        public class ThemeResource
        {
            public ThemeResource(Theme theme, LibraryTheme libraryTheme, ResourceDictionary resourceDictionary, DictionaryEntry dictionaryEntry)
                : this(theme, libraryTheme, resourceDictionary, dictionaryEntry.Key.ToString(), dictionaryEntry.Value)
            {
            }

            public ThemeResource(Theme theme, LibraryTheme libraryTheme, ResourceDictionary resourceDictionary, string key, object value)
            {
                this.Theme = theme;
                this.LibraryTheme = libraryTheme;

                this.Source = resourceDictionary.Source?.ToString() ?? "Runtime";
                this.Key = key;

                this.Value = value switch
                {
                    Color color => new Rectangle { Fill = new SolidColorBrush(color) },
                    Brush brush => new Rectangle { Fill = brush },
                    _ => null
                };

                this.StringValue = value.ToString();
            }

            public Theme Theme { get; }

            public LibraryTheme LibraryTheme { get; }

            public string Source { get; }

            public string Key { get; }

            public object Value { get; }

            public string StringValue { get; }
        }

        private void ThemeManager_ThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            this.UpdateThemeAnalyzers(e.NewTheme);
        }

        private void UpdateThemeAnalyzers(Theme theme)
        {
            this.ThemeResources.Clear();

            if (theme is null)
            {
                return;
            }

            foreach (var libraryTheme in theme.LibraryThemes)
            {
                foreach (var resourceDictionary in libraryTheme.Resources.MergedDictionaries)
                {
                    foreach (DictionaryEntry dictionaryEntry in resourceDictionary)
                    {
                        this.ThemeResources.Add(new ThemeResource(theme, libraryTheme, resourceDictionary, dictionaryEntry));
                    }
                }
            }
        }
    }
}