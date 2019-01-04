namespace FluentTest.ViewModels
{
    using System.Diagnostics;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Fluent.Converters;
    using FluentTest.Commanding;

    public class GallerySampleDataItemViewModel : ViewModel
    {
        /// <summary>
        /// Creates new item
        /// </summary>
        /// <param name="icon">Icon</param>
        /// <param name="iconLarge">Large Icon</param>
        /// <param name="text">Text</param>
        /// <param name="group">Group</param>
        /// <returns>Item</returns>
        public static GallerySampleDataItemViewModel Create(string icon, string iconLarge, string text, string group)
        {
            var dataItem = new GallerySampleDataItemViewModel(icon, iconLarge, text, group);

            return dataItem;
        }

        private GallerySampleDataItemViewModel(string icon, string iconLarge, string text, string group)
        {
            this.Icon = (ImageSource)StaticConverters.ObjectToImageConverter.Convert(icon, typeof(BitmapImage), null, null);
            this.IconLarge = (ImageSource)StaticConverters.ObjectToImageConverter.Convert(iconLarge, typeof(BitmapImage), null, null);
            this.Text = text;
            this.Group = group;

            this.Command = new RelayCommand(() => Trace.WriteLine("Command executed"));
        }

        /// <summary>
        /// Gets or sets icon
        /// </summary>
        public ImageSource Icon { get; }

        /// <summary>
        /// Gets or sets large icon
        /// </summary>
        public ImageSource IconLarge { get; }

        /// <summary>
        /// Gets or sets text
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets or sets group name
        /// </summary>
        public string Group { get; }

        public ICommand Command { get; }
    }
}