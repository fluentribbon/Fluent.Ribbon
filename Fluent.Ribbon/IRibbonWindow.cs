namespace Fluent
{
    /// <summary>
    /// Meant to support interop scenarios
    /// </summary>
    public interface IRibbonWindow
    {
        /// <summary>
        /// Gets the titlebar
        /// </summary>
        RibbonTitleBar TitleBar { get; }
    }
}