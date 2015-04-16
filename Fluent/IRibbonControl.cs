namespace Fluent
{
    /// <summary>
    /// Base interface for Fluent controls
    /// </summary>
    public interface IRibbonControl : IHeaderedControl, IKeyTipedControl
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
        object Icon { get; set; }
    }
}