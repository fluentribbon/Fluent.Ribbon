namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="RibbonGroupBox"/>.
    /// </summary>
    public class RibbonGroupBoxAutomationPeer : ItemsControlAutomationPeer
    {
        private RibbonGroupHeaderAutomationPeer headerPeer;

        private RibbonGroupBox OwningGroup => (RibbonGroupBox)this.Owner;

        private RibbonGroupHeaderAutomationPeer HeaderPeer
        {
            get
            {
                if (this.headerPeer == null
                    || !this.headerPeer.Owner.IsDescendantOf(this.OwningGroup))
                {
                    if (this.OwningGroup.State == RibbonGroupBoxState.Collapsed)
                    {
                        if (this.OwningGroup.DropDownPopup != null)
                        {
                            this.headerPeer = new RibbonGroupHeaderAutomationPeer(this.OwningGroup.DropDownPopup);
                        }
                    }
                    else if (this.OwningGroup.Header != null)
                    {
                        this.headerPeer = new RibbonGroupHeaderAutomationPeer(this.OwningGroup);
                    }
                }

                return this.headerPeer;
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonGroupBoxAutomationPeer([NotNull] RibbonGroupBox owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var list = base.GetChildrenCore();

            if (this.HeaderPeer != null)
            {
                if (list == null)
                {
                    list = new List<AutomationPeer>(1);
                }

                list.Add(this.HeaderPeer);
            }

            return list;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Scroll)
            {
                return null;
            }

            return base.GetPattern(patternInterface);
        }

        /// <inheritdoc />
        protected override void SetFocusCore()
        {
        }

        /// <inheritdoc />
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new RibbonControlDataAutomationPeer(item, this);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.EventsSource?.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed, newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}