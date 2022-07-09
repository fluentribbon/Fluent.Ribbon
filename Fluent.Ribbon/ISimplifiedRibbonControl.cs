namespace Fluent;

/// <summary>
/// Base interface for controls supports simplified state
/// </summary>
public interface ISimplifiedRibbonControl : ISimplifiedStateControl
{
    /// <summary>
    /// Gets or sets SimplifiedSizeDefinition for element on Simplified mode
    /// </summary>
    RibbonControlSizeDefinition SimplifiedSizeDefinition { get; set; }

    /// <summary>
    /// Gets or sets whether or not the ribbon is in Simplified mode
    /// </summary>
    bool IsSimplified { get; }
}