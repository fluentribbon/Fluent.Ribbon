namespace Fluent.Automation.Peers;

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Fluent.Extensions;
using Fluent.Internal;

/// <summary>
/// Automation peer for <see cref="RibbonTabControl"/>.
/// </summary>
public class RibbonTabControlAutomationPeer : SelectorAutomationPeer, ISelectionProvider
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public RibbonTabControlAutomationPeer(RibbonTabControl owner)
        : base(owner)
    {
        this.OwningRibbonTabControl = owner;
    }

    private RibbonTabControl OwningRibbonTabControl { get; }

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
    {
        return new RibbonTabItemDataAutomationPeer(item, this);
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
    {
        return this.Owner.GetType().Name;
    }

    /// <inheritdoc />
    protected override Point GetClickablePointCore()
    {
        return new Point(double.NaN, double.NaN);
    }

    bool ISelectionProvider.IsSelectionRequired => true;

    bool ISelectionProvider.CanSelectMultiple => false;

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        switch (patternInterface)
        {
            case PatternInterface.Scroll:
                var ribbonTabsContainerPanel = this.OwningRibbonTabControl.TabsContainer;
                if (ribbonTabsContainerPanel is not null)
                {
                    var automationPeer = CreatePeerForElement(ribbonTabsContainerPanel);
                    if (automationPeer is not null)
                    {
                        return automationPeer.GetPattern(patternInterface);
                    }
                }

                if (this.OwningRibbonTabControl.TabsContainer is RibbonTabsContainer ribbonTabsContainer
                    && ribbonTabsContainer.ScrollOwner is not null)
                {
                    var automationPeer = CreatePeerForElement(ribbonTabsContainer.ScrollOwner);
                    if (automationPeer is not null)
                    {
                        automationPeer.EventsSource = this;
                        return automationPeer.GetPattern(patternInterface);
                    }
                }

                break;
        }

        return base.GetPattern(patternInterface);
    }

    /// <inheritdoc />
    protected override List<AutomationPeer> GetChildrenCore()
    {
        var children = base.GetChildrenCore() ?? new List<AutomationPeer>();

        if (this.OwningRibbonTabControl.Menu is { } menu)
        {
            var automationPeer = menu.GetOrCreateAutomationPeer();

            if (automationPeer is null)
            {
                var child = UIHelper.GetVisualChildren(menu)
                    .OfType<UIElement>()
                    .FirstOrDefault(x => x.IsVisible);

                if (child is not null)
                {
                    automationPeer = child.GetOrCreateAutomationPeer();
                }
            }

            if (automationPeer is not null)
            {
                children.Insert(0, automationPeer);
            }
        }

        if (this.OwningRibbonTabControl.ToolbarPanel is { } toolbarPanel)
        {
            foreach (UIElement? child in toolbarPanel.Children)
            {
                if (child is null)
                {
                    continue;
                }

                var automationPeer = child.GetOrCreateAutomationPeer();

                if (automationPeer is not null)
                {
                    children.Add(automationPeer);
                }
            }
        }

        if (this.OwningRibbonTabControl.DisplayOptionsControl is { } displayOptionsButton)
        {
            var automationPeer = displayOptionsButton.GetOrCreateAutomationPeer();

            if (automationPeer is not null)
            {
                children.Add(automationPeer);
            }
        }

        return children;
    }
}