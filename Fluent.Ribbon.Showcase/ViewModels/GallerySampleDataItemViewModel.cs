namespace FluentTest.ViewModels
{
    using System;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class GallerySampleDataItemViewModel : ViewModel
    {
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
                Icon = new BitmapImage(new Uri(icon, UriKind.Relative)),
                IconLarge = new BitmapImage(new Uri(iconLarge, UriKind.Relative)),
                Text = text,
                Group = group
            };

            return dataItem;
        }
    }
}