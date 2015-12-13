namespace FluentTest.Commanding
{
    using System;
    using System.Windows.Input;

    public interface IRelayCommand : ICommand
    {
        event EventHandler Executed;
        event EventHandler Executing;
    }
}