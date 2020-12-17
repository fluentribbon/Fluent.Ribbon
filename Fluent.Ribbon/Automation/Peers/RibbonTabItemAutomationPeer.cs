namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer wrapper for <see cref="RibbonTabItem"/>.
    /// </summary>
    public class RibbonTabItemAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTabItemAutomationPeer([NotNull] RibbonTabItem owner)
            : base(owner)
        {
            this.OwningTab = owner;
        }

        private RibbonTabItem OwningTab { get; }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Scroll:
                    var container = this.OwningTab.GroupsContainer;
                    if (container is not null)
                    {
                        var automationPeer = CreatePeerForElement(container);
                        if (automationPeer is not null)
                        {
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
            var children = GetHeaderChildren() ?? new List<AutomationPeer>();

            if (this.OwningTab.IsSelected == false)
            {
                return children;
            }

            foreach (var @group in this.OwningTab.Groups)
            {
                var peer = CreatePeerForElement(@group);

                if (peer is not null)
                {
                    children.Add(peer);
                }
            }

            return children;

            List<AutomationPeer>? GetHeaderChildren()
            {
                if (this.OwningTab.Header is string)
                {
                    return null;
                }

                if (this.OwningTab.HeaderContentHost is not null)
                {
                    return new FrameworkElementAutomationPeer(this.OwningTab.HeaderContentHost).GetChildren();
                }

                return null;
            }
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseTabExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.EventsSource?.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed, newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseTabSelectionEvents()
        {
            var eventsSource = this.EventsSource;
            if (eventsSource is not null)
            {
                if (this.OwningTab.IsSelected)
                {
                    eventsSource.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                }
                else
                {
                    eventsSource.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                }
            }
        }
    }
}