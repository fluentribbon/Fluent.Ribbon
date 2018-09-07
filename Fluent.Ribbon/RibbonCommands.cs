namespace Fluent
{
    using System.Windows.Input;

    /// <summary>
    /// Class for several commands belonging to the Ribbon
    /// </summary>
    public static class RibbonCommands
    {
        /// <summary>
        /// Gets the value that represents the Open Backstage command
        /// </summary>
        public static readonly RoutedCommand OpenBackstage = new RoutedUICommand("Open backstage", nameof(OpenBackstage), typeof(RibbonCommands));
    }
}