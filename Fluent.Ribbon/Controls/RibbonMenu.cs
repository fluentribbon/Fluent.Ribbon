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

        /// <inheritdoc />
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MenuItem();
        }

        /// <inheritdoc />
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is System.Windows.Controls.MenuItem
                || item is Separator;
        }

        #endregion
    }
}