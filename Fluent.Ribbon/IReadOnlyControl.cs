namespace Fluent
{
    /// <summary>
    ///  Marks a control as implementing the IsReadOnly scheme (accessibly by keyboard even when it is not enabled) 
    /// </summary>
    public interface IReadOnlyControl
    {
        /// <summary>
        /// Gets or sets IsReadOnly for the element
        /// </summary>
        bool IsReadOnly { get; set; }
    }
}