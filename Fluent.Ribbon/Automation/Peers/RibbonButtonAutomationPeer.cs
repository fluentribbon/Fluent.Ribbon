namespace Fluent.Automation.Peers;

using System.Windows.Automation.Peers;

/// <inheritdoc />
public class RibbonButtonAutomationPeer : ButtonAutomationPeer
{
    /// <summary>Initializes a new instance of the <see cref="T:ButtonAutomationPeer" /> class.</summary>
    /// <param name="owner">The element associated with this automation peer.</param>
    public RibbonButtonAutomationPeer(Button owner)
        : base(owner)
    {
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
    {
        return "RibbonButton";
    }

    /// <inheritdoc />
    protected override string? GetNameCore()
    {
        // Explicit name, then the text header, then the ScreenTip title (for icon-only controls).
        return AutomationPeerHelper.GetName(this.Owner, base.GetNameCore());
    }

    /// <inheritdoc />
    protected override string? GetAccessKeyCore()
    {
        // Unlike other peers (see AutomationPeerHelper.GetAccessKey) the KeyTip is preferred here over the base value.
        // Kept that way to not change the existing behavior of buttons.
        var text = ((Button)this.Owner).KeyTip;

        if (string.IsNullOrEmpty(text))
        {
            text = base.GetAccessKeyCore();
        }

        return text;
    }

    /// <inheritdoc />
    protected override string GetHelpTextCore()
    {
        // Expose the ScreenTip text, which the default implementation can't read.
        return AutomationPeerHelper.GetHelpText(this.Owner, base.GetHelpTextCore());
    }
}