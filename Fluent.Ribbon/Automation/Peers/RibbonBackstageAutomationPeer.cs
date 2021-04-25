namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="Backstage"/>.
    /// </summary>
    public class RibbonBackstageAutomationPeer : RibbonControlAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonBackstageAutomationPeer([NotNull] Backstage owner)
            : base(owner)
        {
            this.OwningBackstage = owner;
        }

        private Backstage OwningBackstage { get; }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Menu;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                    return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var children = new List<AutomationPeer>();

            if (this.OwningBackstage.Content is not null)
            {
                var automationPeer = CreatePeerForElement(this.OwningBackstage.Content);

                if (automationPeer is not null)
                {
                    children.Add(automationPeer);
                }
            }

            return children;
        }

        #region IExpandCollapseProvider Members

        /// <inheritdoc />
        void IExpandCollapseProvider.Collapse()
        {
            this.OwningBackstage.IsOpen = false;
        }

        /// <inheritdoc />
        void IExpandCollapseProvider.Expand()
        {
            this.OwningBackstage.IsOpen = true;
        }

        /// <inheritdoc />
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState => this.OwningBackstage.IsOpen == false ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                                           oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                                           newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }

        #endregion
    }
}