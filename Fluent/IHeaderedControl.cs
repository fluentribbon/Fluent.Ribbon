namespace Fluent
{
    /// <summary>
    /// Represents control that has a header
    /// </summary>
    public interface IHeaderedControl
    {
        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        object Header { get; set; }
    }
}