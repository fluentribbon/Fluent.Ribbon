namespace Fluent
{
    /// <summary>
    /// Base interface for Fluent controls
    /// </summary>
    public interface IRibbonControl : IKeyTipedControl
    {
        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        RibbonControlSize Size { get; set; }

        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        string SizeDefinition { get; set; }

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        object Header { get; set; }

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        object Icon { get; set; }

    }
}