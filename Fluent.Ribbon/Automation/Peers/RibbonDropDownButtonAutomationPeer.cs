namespace Fluent.Automation.Peers
{
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="DropDownButton"/>.
    /// </summary>
    public class RibbonDropDownButtonAutomationPeer : RibbonHeaderedControlAutomationPeer, IExpandCollapseProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonDropDownButtonAutomationPeer([NotNull] DropDownButton owner)
            : base(owner)
        {
            this.OwnerDropDownButton = owner;
        }

        private DropDownButton OwnerDropDownButton { get; }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Custom;
        }

        /// <inheritdoc />
        protected override string GetLocalizedControlTypeCore()
        {
            return this.Owner.GetType().Name;
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

        #region IExpandCollapseProvider Members

        /// <inheritdoc />
        void IExpandCollapseProvider.Collapse()
        {
            this.OwnerDropDownButton.IsDropDownOpen = false;
        }

        /// <inheritdoc />
        void IExpandCollapseProvider.Expand()
        {
            this.OwnerDropDownButton.IsDropDownOpen = true;
        }

        /// <inheritdoc />
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState => this.OwnerDropDownButton.IsDropDownOpen == false ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;

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