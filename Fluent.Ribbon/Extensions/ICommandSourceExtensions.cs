namespace Fluent.Extensions
{
    using System.Windows.Input;
    using Fluent.Internal;

    /// <summary>
    /// Extensions for <see cref="ICommandSource"/>.
    /// </summary>
    public static class ICommandSourceExtensions
    {
        /// <summary>
        /// Execute <see cref="ICommandSource.Command"/> using <see cref="ICommandSource.CommandParameter"/> and <see cref="ICommandSource.CommandTarget"/>.
        /// </summary>
        public static void ExecuteCommand(this ICommandSource commandSource)
        {
            CommandHelper.Execute(commandSource.Command, commandSource.CommandParameter, commandSource.CommandTarget);
        }

        /// <summary>
        /// Determines whether the <see cref="ICommandSource.Command"/> can be executed using <see cref="ICommandSource.CommandParameter"/> and <see cref="ICommandSource.CommandTarget"/>.
        /// </summary>
        /// <returns>Returns the commands result of CanExecute.</returns>
        public static bool CanExecuteCommand(this ICommandSource commandSource)
        {
            return CommandHelper.CanExecute(commandSource.Command, commandSource.CommandParameter, commandSource.CommandTarget);
        }
    }
}