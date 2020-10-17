// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Represents a <see cref="ScrollViewer"/> specific to <see cref="RibbonGroupsContainer"/>.
    /// </summary>
    public class RibbonGroupsContainerScrollViewer : ScrollViewer
    {
        static RibbonGroupsContainerScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupsContainerScrollViewer), new FrameworkPropertyMetadata(typeof(RibbonGroupsContainerScrollViewer)));
        }
    }
}