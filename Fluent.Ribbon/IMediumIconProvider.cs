namespace Fluent
{
    using System.Windows;
    using Fluent.Helpers;

    /// <summary>
    /// Inferface for controls which provide a medium icon.
    /// </summary>
    public interface IMediumIconProvider
    {
        /// <summary>
        /// Gets or sets the medium icon.
        /// </summary>
        object? MediumIcon { get; set; }
    }

    /// <summary>
    /// Provides some <see cref="DependencyProperty"/> for <see cref="IMediumIconProvider"/>.
    /// </summary>
    public class MediumIconProviderProperties : DependencyObject
    {
        private MediumIconProviderProperties()
        {
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="IMediumIconProvider.MediumIcon"/>.
        /// </summary>
        public static readonly DependencyProperty MediumIconProperty = DependencyProperty.Register(nameof(IMediumIconProvider.MediumIcon), typeof(object), typeof(MediumIconProviderProperties), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));
    }
}