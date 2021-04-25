namespace Fluent.Automation.Peers
{
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Fluent.Extensions;

    /// <summary>
    /// Automation peer for <see cref="RibbonTabItem"/>.
    /// </summary>
    public class RibbonTabItemDataAutomationPeer : SelectorItemAutomationPeer, IScrollItemProvider, IExpandCollapseProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTabItemDataAutomationPeer(object item, RibbonTabControlAutomationPeer tabControlAutomationPeer)
            : base(item, tabControlAutomationPeer)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "RibbonTabItem";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var nameCore = base.GetNameCore();

            if (string.IsNullOrEmpty(nameCore) == false)
            {
                var wrapper = this.GetWrapper() as RibbonTabItem;
                if (wrapper?.Header is string headerString)
                {
                    return headerString;
                }
            }

            return nameCore;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TabItem;
        }

        #region IExpandCollapseProvider Members

        /// <summary>
        /// If Ribbon.IsMinimized then set Ribbon.IsDropDownOpen to false
        /// </summary>
        void IExpandCollapseProvider.Collapse()
        {
            var wrapperTab = this.GetWrapper() as RibbonTabItem;
            if (wrapperTab is not null)
            {
                var tabControl = wrapperTab.TabControlParent;
                if (tabControl is not null &&
                    tabControl.IsMinimized)
                {
                    tabControl.IsDropDownOpen = false;
                }
            }
        }

        /// <summary>
        /// If Ribbon.IsMinimized then set Ribbon.IsDropDownOpen to true
        /// </summary>
        void IExpandCollapseProvider.Expand()
        {
            var wrapperTab = this.GetWrapper() as RibbonTabItem;

            // Select the tab and display popup
            if (wrapperTab is not null)
            {
                var tabControl = wrapperTab.TabControlParent;
                if (tabControl is not null &&
                    tabControl.IsMinimized)
                {
                    wrapperTab.IsSelected = true;
                    tabControl.IsDropDownOpen = true;
                }
            }
        }

        /// <summary>
        /// Return Ribbon.IsDropDownOpen
        /// </summary>
        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
        {
            get
            {
                var wrapperTab = this.GetWrapper() as RibbonTabItem;
                if (wrapperTab is not null)
                {
                    var tabControl = wrapperTab.TabControlParent;
                    if (tabControl is not null &&
                        tabControl.IsMinimized)
                    {
                        if (wrapperTab.IsSelected && tabControl.IsDropDownOpen)
                        {
                            return ExpandCollapseState.Expanded;
                        }
                        else
                        {
                            return ExpandCollapseState.Collapsed;
                        }
                    }
                }

                // When not minimized
                return ExpandCollapseState.Expanded;
            }
        }

        #endregion

        #region IScrollItemProvider Members

        void IScrollItemProvider.ScrollIntoView()
        {
            var wrapperTab = this.GetWrapper() as RibbonTabItem;
            if (wrapperTab is not null)
            {
                wrapperTab.BringIntoView();
            }
        }

        #endregion
    }
}