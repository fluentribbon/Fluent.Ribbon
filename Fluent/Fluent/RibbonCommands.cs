namespace Fluent
{
    using System.Windows.Input;

    public static class RibbonCommands
    {
        private static readonly RoutedUICommand openBackstage = new RoutedUICommand("Open backstage", "OpenBackstage", typeof(RibbonCommands));

        /// <summary>
        /// Gets the value that represents the Open Backstage command
        /// </summary>
        public static RoutedCommand OpenBackstage
        {
            get { return openBackstage; }
        }
    }
}