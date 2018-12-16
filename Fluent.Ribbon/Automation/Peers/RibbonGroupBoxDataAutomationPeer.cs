namespace Fluent.Automation.Peers
{
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Fluent.Extensions;
    using JetBrains.Annotations;

    /// <summary>
    ///     Automation peer wrapper for <see cref="RibbonGroupBox" />.
    /// </summary>
    public class RibbonGroupBoxDataAutomationPeer : ItemAutomationPeer, IScrollItemProvider, IExpandCollapseProvider
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public RibbonGroupBoxDataAutomationPeer(object item, [NotNull] RibbonTabItemAutomationPeer itemsControlPeer)
            : base(item, itemsControlPeer)
        {
        }

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                var ribbonGroup = this.GetWrapper() as RibbonGroupBox;
                if (ribbonGroup != null
                    && ribbonGroup.State == RibbonGroupBoxState.Collapsed)
                {
                    if (!ribbonGroup.IsDropDownOpen)
                    {
                        return ExpandCollapseState.Collapsed;
                    }

                    return ExpandCollapseState.Expanded;
                }

                return ExpandCollapseState.LeafNode;
            }
        }

        /// <inheritdoc />
        void IExpandCollapseProvider.Collapse()
        {
            var ribbonGroup = this.GetWrapper() as RibbonGroupBox;
            if (ribbonGroup != null
                && ribbonGroup.State == RibbonGroupBoxState.Collapsed)
            {
                ribbonGroup.IsDropDownOpen = false;
            }
        }

        /// <inheritdoc />
        void IExpandCollapseProvider.Expand()
        {
            var ribbonGroup = this.GetWrapper() as RibbonGroupBox;
            if (ribbonGroup != null
                && ribbonGroup.State == RibbonGroupBoxState.Collapsed)
            {
                ribbonGroup.IsDropDownOpen = true;
            }
        }

        /// <inheritdoc />
        void IScrollItemProvider.ScrollIntoView()
        {
            (this.GetWrapper() as RibbonGroupBox)?.BringIntoView();
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            object obj = null;
            switch (patternInterface)
            {
                case PatternInterface.ScrollItem:
                    obj = this;
                    break;
                case PatternInterface.ExpandCollapse:
                {
                    var ribbonGroup = this.GetWrapper() as RibbonGroupBox;
                    if (ribbonGroup != null
                        && ribbonGroup.State == RibbonGroupBoxState.Collapsed)
                    {
                        obj = this;
                    }

                    break;
                }
            }

            if (obj == null)
            {
                var wrapperPeer = this.GetWrapperPeer();
                if (wrapperPeer != null)
                {
                    obj = wrapperPeer.GetPattern(patternInterface);
                }
            }

            return obj;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Group;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            var wrapperPeer = this.GetWrapperPeer();
            if (wrapperPeer != null)
            {
                return wrapperPeer.GetClassName();
            }

            return string.Empty;
        }
    }
}