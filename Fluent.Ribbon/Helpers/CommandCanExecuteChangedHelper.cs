namespace Fluent.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    internal class CommandCanExecuteChangedHelper
    {
        private IReadOnlyControl ReadOnlyControl { get; }

        private ICommand Command { get; }

        internal CommandCanExecuteChangedHelper(IReadOnlyControl ribbonControl, ICommand command)
        {
            this.ReadOnlyControl = ribbonControl;
            this.Command = command;
        }

        internal void RegisterCommand()
        {
            this.SetIsReadOnlyFromCommand(this.Command);

            WeakEventManager<ICommand, EventArgs>.AddHandler(this.Command, nameof(ICommand.CanExecuteChanged), this.CanExecuteChanged);
        }

        internal void UnRegisterCommand()
        {
            this.ReadOnlyControl.IsReadOnly = false;

            WeakEventManager<ICommand, EventArgs>.RemoveHandler(this.Command, nameof(ICommand.CanExecuteChanged), this.CanExecuteChanged);
        }

        private void CanExecuteChanged(object? sender, EventArgs e)
        {
            if (sender is not ICommand command)
            {
                return;
            }

            this.SetIsReadOnlyFromCommand(command);
        }

        private void SetIsReadOnlyFromCommand(ICommand command)
        {
            if (this.ReadOnlyControl is not UIElement control)
            {
                return;
            }

            bool isReadOnly;

            if (command is RoutedCommand routedCommand)
            {
                isReadOnly = !routedCommand.CanExecute(null, control);
            }
            else
            {
                isReadOnly = !command.CanExecute(null);
            }

            control.SetCurrentValue(RibbonProperties.IsReadOnlyProperty, isReadOnly);
        }
    }
}
