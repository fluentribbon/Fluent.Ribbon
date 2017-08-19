// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;

    /// <summary>
    /// Represents menu in combo box and gallery
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class RibbonMenu : MenuBase
    {
        #region Constructors

        static RibbonMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonMenu), new FrameworkPropertyMetadata(typeof(RibbonMenu)));
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns></returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is System.Windows.Controls.MenuItem
                || item is Separator;
        }

        #endregion
    }
}