namespace Fluent.Tests.Controls;

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using NUnit.Framework;

/// <summary>
/// Tests that a control does not stay alive just because the command it uses is alive.
/// </summary>
/// <remarks>
/// Apps often keep their commands much longer than a single control (in a view model, or as a singleton),
/// and many command implementations raise <see cref="ICommand.CanExecuteChanged"/> from their own event, not through the CommandManager.
/// A control that listens to that event must not be kept alive by the command once the app has thrown the control away
/// (for example a removed tab, or the gallery items of a refreshed ItemsSource), like WPF's own buttons aren't.
/// </remarks>
[TestFixture]
public class CommandCanExecuteChangedLeakTests
{
    // WPF's own button proves that this way of checking for leaks works at all.
    [TestCase("WPF Button")]
    [TestCase("Spinner")]
    [TestCase("RibbonToolBar")]
    [TestCase("GalleryItem")]
    public void Control_is_collected_while_its_command_is_alive(string kind)
    {
        var command = new AppCommand();

        var control = CreateControlUsing(kind, command);

        CollectGarbage();

        Assert.That(control.IsAlive, Is.False, "The command keeps the control alive.");

        GC.KeepAlive(command);
    }

    /// <summary>
    /// Guard: a live control still follows its command after a garbage collection.
    /// </summary>
    [TestCase("Spinner")]
    [TestCase("RibbonToolBar")]
    [TestCase("GalleryItem")]
    public void Live_control_still_follows_its_command_after_garbage_collection(string kind)
    {
        var command = new AppCommand();
        var control = CreateControl(kind);
        control.SetValue(CommandPropertyOf(control), command);

        Assert.That(control.IsEnabled, Is.True, "Precondition: the control is enabled while the command can execute.");

        CollectGarbage();

        command.SetCanExecute(false);
        Assert.That(control.IsEnabled, Is.False, "The control must be disabled when its command can no longer execute.");

        CollectGarbage();

        command.SetCanExecute(true);
        Assert.That(control.IsEnabled, Is.True, "The control must be enabled again when its command can execute again.");
    }

    /// <summary>
    /// Guard: a control no longer follows the command it used before.
    /// </summary>
    [TestCase("Spinner")]
    [TestCase("RibbonToolBar")]
    [TestCase("GalleryItem")]
    public void Control_no_longer_follows_its_previous_command(string kind)
    {
        var previousCommand = new AppCommand();
        var command = new AppCommand();
        var control = CreateControl(kind);
        control.SetValue(CommandPropertyOf(control), previousCommand);
        control.SetValue(CommandPropertyOf(control), command);

        previousCommand.SetCanExecute(false);

        Assert.That(control.IsEnabled, Is.True, "The previous command must no longer disable the control.");
        Assert.That(previousCommand.CanExecuteCalls, Is.Zero, "The control must no longer ask the previous command.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateControlUsing(string kind, ICommand command)
    {
        var control = CreateControl(kind);
        control.SetValue(CommandPropertyOf(control), command);

        Assert.That(control.IsEnabled, Is.True, "Precondition: the control is enabled while the command can execute.");

        return new WeakReference(control);
    }

    private static FrameworkElement CreateControl(string kind)
    {
        switch (kind)
        {
            case "WPF Button":
                return new System.Windows.Controls.Button();
            case "Spinner":
                return new Spinner();
            case "RibbonToolBar":
                return new RibbonToolBar();
            case "GalleryItem":
                return new GalleryItem();
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }
    }

    private static DependencyProperty CommandPropertyOf(FrameworkElement control)
    {
        return control is GalleryItem
            ? GalleryItem.CommandProperty
            : RibbonControl.CommandProperty;
    }

    private static void CollectGarbage()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    // Like most app commands (for example a view model's relay command): it raises CanExecuteChanged from its own event.
    private sealed class AppCommand : ICommand
    {
        private bool canExecute = true;

        public event EventHandler CanExecuteChanged;

        public int CanExecuteCalls { get; private set; }

        public void SetCanExecute(bool value)
        {
            this.CanExecuteCalls = 0;
            this.canExecute = value;
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool CanExecute(object parameter)
        {
            this.CanExecuteCalls++;
            return this.canExecute;
        }

        public void Execute(object parameter)
        {
        }
    }
}
