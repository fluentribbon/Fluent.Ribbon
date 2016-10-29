namespace Fluent.Extensions
{
    using System.Windows.Input;
    using Fluent.Internal;

    public static class ICommandSourceExtensions
    {
        /// <summary>
        /// Execute command
        /// </summary>
        public static void ExecuteCommand(this ICommandSource commandSource)
        {
            CommandHelper.Execute(commandSource.Command, commandSource.CommandParameter, commandSource.CommandTarget);
        }

        /// <summary>
        /// Determines whether the Command can be executed
        /// </summary>
        /// <returns>Returns Command CanExecute</returns>
        public static bool CanExecuteCommand(this ICommandSource commandSource)
        {
            return CommandHelper.CanExecute(commandSource.Command, commandSource.CommandParameter, commandSource.CommandTarget);
        }
    }
}