namespace Fluent.Automation.Peers;

using System.Windows;
using System.Windows.Automation.Peers;

/// <summary>
/// Base automation peer for <see cref="IHeaderedControl"/>.
/// </summary>
public abstract class RibbonHeaderedControlAutomationPeer : FrameworkElementAutomationPeer
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    protected RibbonHeaderedControlAutomationPeer(FrameworkElement owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
    {
        return this.Owner.GetType().Name;
    }

    /// <inheritdoc />
    protected override string? GetNameCore()
    {
        // Explicit name, then the text header, then a name of the derived peer's own, then the ScreenTip title (for icon-only controls).
        return AutomationPeerHelper.GetName(this.Owner, base.GetNameCore(), this.GetOwnNameFallback());
    }

    /// <summary>
    /// Gets a name the derived peer finds itself. It is used when there is no explicit name and no text header, before the ScreenTip title.
    /// </summary>
    private protected virtual string? GetOwnNameFallback()
    {
        return null;
    }

    /// <inheritdoc />
    protected override string? GetAccessKeyCore()
    {
        // Expose the KeyTip so screen readers can announce the keyboard shortcut.
        return AutomationPeerHelper.GetAccessKey(this.Owner, base.GetAccessKeyCore());
    }

    /// <inheritdoc />
    protected override string GetHelpTextCore()
    {
        // Expose the ScreenTip text, which the default implementation can't read.
        return AutomationPeerHelper.GetHelpText(this.Owner, base.GetHelpTextCore());
    }
}