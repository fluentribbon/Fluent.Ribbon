namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using Fluent.Extensions;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="Ribbon"/>.
    /// </summary>
    public class RibbonAutomationPeer : FrameworkElementAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonAutomationPeer([NotNull] Ribbon owner)
            : base(owner)
        {
            this.OwningRibbon = owner;
        }

        private Ribbon OwningRibbon { get; }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = this.GetLocalizedControlTypeCore();
            }

            return name;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return "Ribbon";
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                    return this;

                case PatternInterface.Scroll:
                {
                    ItemsControl ribbonTabControl = this.OwningRibbon.TabControl;
                    if (ribbonTabControl != null)
                    {
                        var automationPeer = CreatePeerForElement(ribbonTabControl);
                        if (automationPeer != null)
                        {
                            return automationPeer.GetPattern(patternInterface);
                        }
                    }

                    break;
                }
            }

            return base.GetPattern(patternInterface);
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            // If Ribbon is Collapsed, dont show anything in the UIA tree
            if (this.OwningRibbon.IsCollapsed)
            {
                return null;
            }

            var children = new List<AutomationPeer>();
            if (this.OwningRibbon.QuickAccessToolBar != null)
            {
                var automationPeer = CreatePeerForElement(this.OwningRibbon.QuickAccessToolBar);
                if (automationPeer != null)
                {
                    children.Add(automationPeer);
                }
            }

            if (this.OwningRibbon.Menu != null)
            {
                var automationPeer = CreatePeerForElement(this.OwningRibbon.Menu);
                if (automationPeer != null)
                {
                    children.Add(automationPeer);
                }
            }

            // Directly forward the children from the tab control
            if (this.OwningRibbon.TabControl != null)
            {
                var automationPeer = CreatePeerForElement(this.OwningRibbon.TabControl);

                if (automationPeer != null)
                {
                    automationPeer.ResetChildrenCache();

                    var ribbonTabs = automationPeer.GetChildren();
                    children.AddRange(ribbonTabs);
                    children.ForEach(x => x.ResetChildrenCache());
                }
            }

            var toolbarPanel = this.OwningRibbon.TabControl?.ToolbarPanel;
            if (toolbarPanel != null)
            {
                var automationPeer = CreatePeerForElement(toolbarPanel);
                if (automationPeer != null)
                {
                    children.Add(automationPeer);
                }
            }

            return children;
        }

        /// <inheritdoc/>
        protected override bool IsOffscreenCore()
        {
            return this.OwningRibbon.IsCollapsed
                   || base.IsOffscreenCore();
        }

        /// <inheritdoc />
        public void Collapse()
        {
            this.OwningRibbon.IsMinimized = true;
        }

        /// <inheritdoc />
        public void Expand()
        {
            this.OwningRibbon.IsMinimized = false;
        }

        /// <inheritdoc />
        public ExpandCollapseState ExpandCollapseState => this.OwningRibbon.IsMinimized ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                                      oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                                      newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}
