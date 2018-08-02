namespace FluentTest.ViewModels
{
    using System;
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
            var dataItem = new GallerySampleDataItemViewModel
            {
                Icon = (ImageSource)StaticConverters.ObjectToImageConverter.Convert(icon, typeof(BitmapImage), null, null),
                IconLarge = (ImageSource)StaticConverters.ObjectToImageConverter.Convert(iconLarge, typeof(BitmapImage), null, null),
                Text = text,
                Group = group,
                Command = new RelayCommand(() => Trace.WriteLine("Command executed"))
            };

            return dataItem;
        }

        /// <summary>
        /// Gets or sets icon
        /// </summary>
        public ImageSource Icon { get; set; }

        /// <summary>
        /// Gets or sets large icon
        /// </summary>
        public ImageSource IconLarge { get; set; }

        /// <summary>
        /// Gets or sets text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets group name
        /// </summary>
        public string Group { get; set; }

        public ICommand Command { get; set; }
    }
}