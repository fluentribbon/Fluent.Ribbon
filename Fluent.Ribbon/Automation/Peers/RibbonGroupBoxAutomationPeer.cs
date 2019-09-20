namespace Fluent.Automation.Peers
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="RibbonGroupBox"/>.
    /// </summary>
    public class RibbonGroupBoxAutomationPeer : ItemsControlAutomationPeer, IExpandCollapseProvider
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
        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = (this.Owner as IHeaderedControl)?.Header as string;
            }

            return name;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.ExpandCollapse:
                    return this;

                case PatternInterface.Scroll:
                    return null;

                default:
                    return base.GetPattern(patternInterface);
            }
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

        /// <inheritdoc />
        void IExpandCollapseProvider.Expand()
        {
            this.OwningGroup.IsDropDownOpen = true;
        }

        /// <inheritdoc />
        void IExpandCollapseProvider.Collapse()
        {
            this.OwningGroup.IsDropDownOpen = false;
        }

        /// <inheritdoc />
        public ExpandCollapseState ExpandCollapseState => this.OwningGroup.IsDropDownOpen == false ? ExpandCollapseState.Collapsed : ExpandCollapseState.Expanded;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                                           oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                                           newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}