// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents gallery group filter definition
    /// </summary>
    public class GalleryGroupFilter : DependencyObject
    {
        /// <summary>
        /// Gets or sets title of filter
        /// </summary>
        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string),
            typeof(GalleryGroupFilter), new PropertyMetadata("GalleryGroupFilter"));

        /// <summary>
        /// Gets or sets list pf groups splitted by comma
        /// </summary>
        public string Groups
        {
            get { return (string)this.GetValue(GroupsProperty); }
            set { this.SetValue(GroupsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Groups.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupsProperty =
#pragma warning disable WPF0010 // Default value type must match registered type.
            DependencyProperty.Register(nameof(Groups), typeof(string), typeof(GalleryGroupFilter), new PropertyMetadata(StringBoxes.Empty));
#pragma warning restore WPF0010 // Default value type must match registered type.
    }
}