namespace Fluent
{
    /// <summary>
    /// Meant to support interop scenarios
    /// </summary>
    public interface IRibbonWindow
    {
        /// <summary>
        /// Gets the height of the titlebar
        /// </summary>
        double TitleBarHeight { get; }

        /// <summary>
        /// Gets the titlebar
        /// </summary>
        RibbonTitleBar TitleBar { get; }
    }
}