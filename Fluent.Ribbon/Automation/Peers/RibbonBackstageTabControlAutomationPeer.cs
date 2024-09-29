namespace Fluent.Automation.Peers;

using System.Collections.Generic;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Fluent.Extensions;

/// <summary>
///     Automation peer for <see cref="BackstageTabControl" />.
/// </summary>
public class RibbonBackstageTabControlAutomationPeer : SelectorAutomationPeer, ISelectionProvider
{
    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public RibbonBackstageTabControlAutomationPeer(BackstageTabControl owner)
        : base(owner)
    {
        this.OwningBackstageTabControl = owner;
    }

    private BackstageTabControl OwningBackstageTabControl { get; }

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Tab;
    }

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
    {
        return new RibbonControlDataAutomationPeer(item, this);
    }

    bool ISelectionProvider.IsSelectionRequired => true;

    bool ISelectionProvider.CanSelectMultiple => false;

    /// <inheritdoc />
    protected override List<AutomationPeer> GetChildrenCore()
    {
        var baseResult = base.GetChildrenCore() ?? new List<AutomationPeer>();

        if (this.OwningBackstageTabControl.BackButton is { } backButton)
        {
            var backButtonAutomationPeer = backButton.GetOrCreateAutomationPeer();

            if (backButtonAutomationPeer is not null)
            {
                baseResult.Insert(0, backButtonAutomationPeer);
            }
        }

        return baseResult;
    }
}