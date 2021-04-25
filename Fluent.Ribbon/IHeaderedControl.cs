namespace Fluent
{
    /// <summary>
    /// Represents a control that has a header.
    /// </summary>
    public interface IHeaderedControl
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        object? Header { get; set; }
    }
}