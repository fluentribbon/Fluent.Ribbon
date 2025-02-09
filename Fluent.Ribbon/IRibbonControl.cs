namespace Fluent;

using System.ComponentModel;
using System.Windows;

/// <summary>
/// Base interface for Fluent controls
/// </summary>
public interface IRibbonControl : IHeaderedControl, IKeyTipedControl, ILogicalChildSupport
{
    /// <summary>
    /// Gets or sets Size for the element
    /// </summary>
    RibbonControlSize Size { get; set; }

    /// <summary>
    /// Gets or sets SizeDefinition for element
    /// </summary>
    RibbonControlSizeDefinition SizeDefinition { get; set; }

    /// <summary>
    /// Gets or sets Icon for the element
    /// </summary>
    [Localizability(LocalizationCategory.NeverLocalize)]
    [Localizable(false)]
    object? Icon { get; set; }
}