namespace Fluent
{
    using System.Windows;
    using Fluent.Helpers;

    /// <summary>
    /// Inferface for controls which provide a large icon.
    /// </summary>
    public interface ILargeIconProvider
    {
        /// <summary>
        /// Gets or sets the large icon.
        /// </summary>
        object LargeIcon { get; set; }
    }

    /// <summary>
    /// Provides some <see cref="DependencyProperty"/> for <see cref="ILargeIconProvider"/>.
    /// </summary>
    public class LargeIconProviderProperties : DependencyObject
    {
        private LargeIconProviderProperties()
        {
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="ILargeIconProvider.LargeIcon"/>.
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = DependencyProperty.Register(nameof(ILargeIconProvider.LargeIcon), typeof(object), typeof(LargeIconProviderProperties), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));
    }
}