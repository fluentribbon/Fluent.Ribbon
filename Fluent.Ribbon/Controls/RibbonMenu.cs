// ReSharper disable once CheckNamespace
namespace Fluent;

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
        if (this.UsesItemContainerTemplate)
        {
            return base.GetContainerForItemOverride();
        }

        return new MenuItem();
    }

    #endregion
}