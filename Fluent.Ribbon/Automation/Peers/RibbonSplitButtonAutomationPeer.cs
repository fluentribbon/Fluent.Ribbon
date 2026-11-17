namespace Fluent.Automation.Peers;

using System;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Threading;
using Fluent.Extensions;
using Fluent.Internal;

/// <summary>
/// Automation peer for <see cref="SplitButton"/>.
/// </summary>
public class RibbonSplitButtonAutomationPeer : RibbonDropDownButtonAutomationPeer, IInvokeProvider, IToggleProvider
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public RibbonSplitButtonAutomationPeer(SplitButton owner)
        : base(owner)
    {
        this.SplitButtonOnwer = owner;
    }

    private SplitButton SplitButtonOnwer { get; }

    /// <inheritdoc />
    protected override string GetClassNameCore()
    {
        return this.Owner.GetType().Name;
    }

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.SplitButton;
    }

    /// <inheritdoc />
    public override object GetPattern(PatternInterface patternInterface)
    {
        switch (patternInterface)
        {
            case PatternInterface.Invoke:
                return this;

            // Only a checkable split button is a toggle (like "Bold" with a drop down); a screen reader then reports its state.
            case PatternInterface.Toggle when this.SplitButtonOnwer.IsCheckable:
                return this;

            default:
                return base.GetPattern(patternInterface);
        }
    }

    /// <inheritdoc />
    protected override string GetAutomationIdCore()
    {
        var id = base.GetAutomationIdCore();

        if (string.IsNullOrEmpty(id))
        {
            if (this.SplitButtonOnwer.Command is RoutedCommand routedCommand
                && string.IsNullOrEmpty(routedCommand.Name) == false)
            {
                id = routedCommand.Name;
            }
        }

        return id ?? string.Empty;
    }

    /// <inheritdoc />
    protected override string? GetNameCore()
    {
        // The command text and the button's content (see GetOwnNameFallback) come before the ScreenTip title, as they were the name before it was considered.
        return base.GetNameCore();
    }

    /// <inheritdoc />
    private protected override string? GetOwnNameFallback()
    {
        if (this.SplitButtonOnwer.Command is RoutedUICommand routedUiCommand
            && string.IsNullOrEmpty(routedUiCommand.Text) == false)
        {
            return routedUiCommand.Text;
        }

        if (this.SplitButtonOnwer.Button?.Content is string buttonContent)
        {
            return AccessTextHelper.RemoveAccessKeyMarker(buttonContent);
        }

        return null;
    }

    /// <inheritdoc />
    public void Invoke()
    {
        // Invoke clicks the button part, which can be disabled on its own
        // (IsButtonEnabled = false, or a command that can't execute).
        if (this.IsEnabled() == false
            || this.SplitButtonOnwer.Button is { IsEnabled: false })
        {
            throw new ElementNotEnabledException();
        }

        this.RunInDispatcherAsync(() => this.SplitButtonOnwer.AutomationButtonClick(), DispatcherPriority.Input);
    }

    internal void RaiseToggleStateChanged(bool? oldValue, bool? newValue)
    {
        this.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ToToggleState(oldValue), ToToggleState(newValue));
    }

    private static ToggleState ToToggleState(bool? value) => value switch
    {
        true => ToggleState.On,
        false => ToggleState.Off,
        _ => ToggleState.Indeterminate
    };

    /// <inheritdoc />
    public ToggleState ToggleState => ToToggleState(this.SplitButtonOnwer.IsChecked);

    /// <inheritdoc />
    public void Toggle()
    {
        if (this.SplitButtonOnwer.IsCheckable == false)
        {
            throw new InvalidOperationException("The split button is not checkable.");
        }

        // Toggle does what a click on the button part does (like Invoke): it respects a disabled button part
        // (IsButtonEnabled = false or a command that can't execute), runs the app's command and keeps the GroupName rule.
        if (this.IsEnabled() == false
            || this.SplitButtonOnwer.Button is { IsEnabled: false })
        {
            throw new ElementNotEnabledException();
        }

        this.RunInDispatcherAsync(() => this.SplitButtonOnwer.AutomationButtonClick(), DispatcherPriority.Input);
    }
}