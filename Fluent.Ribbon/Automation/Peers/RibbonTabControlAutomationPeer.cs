namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="RibbonTabControl"/>.
    /// </summary>
    public class RibbonTabControlAutomationPeer : SelectorAutomationPeer, ISelectionProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTabControlAutomationPeer([NotNull] RibbonTabControl owner)
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
        public override object GetPattern(PatternInterface patternInterface)
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

                    var ribbonTabsContainer = this.OwningRibbonTabControl.TabsContainer as RibbonTabsContainer;
                    if (ribbonTabsContainer is not null
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

            var minimizeButton = this.OwningRibbonTabControl.MinimizeButton;

            if (minimizeButton is not null)
            {
                var automationPeer = CreatePeerForElement(minimizeButton);

                if (automationPeer is not null)
                {
                    children.Add(automationPeer);
                }
            }

            var toolbarPanel = this.OwningRibbonTabControl.ToolbarPanel;
            if (toolbarPanel is not null)
            {
                var automationPeer = new RibbonToolbarPanelAutomationPeer(toolbarPanel);
                children.Add(automationPeer);
            }

            return children;
        }
    }
}