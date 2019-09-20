namespace Fluent.Automation.Peers
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Automation peer for <see cref="RibbonTabItem"/>.
    /// </summary>
    public class RibbonTabItemDataAutomationPeer : SelectorItemAutomationPeer, ISelectionItemProvider, IScrollItemProvider, IExpandCollapseProvider
    {
        private readonly object item;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTabItemDataAutomationPeer(object item, RibbonTabControlAutomationPeer tabControlAutomationPeer)
            : base(item, tabControlAutomationPeer)
        {
            this.item = item;
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
            if (wrapperTab != null)
            {
                var tabControl = wrapperTab.TabControlParent;
                if (tabControl != null &&
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
            if (wrapperTab != null)
            {
                var tabControl = wrapperTab.TabControlParent;
                if (tabControl != null &&
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
                if (wrapperTab != null)
                {
                    var tabControl = wrapperTab.TabControlParent;
                    if (tabControl != null &&
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

        #region ISelectionItemProvider Members

        void ISelectionItemProvider.RemoveFromSelection()
        {
            if (this.IsEnabled() == false)
            {
                throw new ElementNotEnabledException();
            }

            if (this.GetWrapper() is RibbonTabItem wrapper
                && wrapper.IsSelected)
            {
                throw new InvalidOperationException("Cannot perform operation.");
            }
        }

        void ISelectionItemProvider.AddToSelection()
        {
            if (this.IsEnabled() == false)
            {
                throw new ElementNotEnabledException();
            }

            var parentSelector = (Selector)this.ItemsControlAutomationPeer.Owner;
            if (parentSelector == null)
            {
                var wrapperTab = this.GetWrapper() as RibbonTabItem;
                if (wrapperTab != null)
                {
                    wrapperTab.IsSelected = true;
                }
            }
        }

        #endregion

        #region IScrollItemProvider Members

        void IScrollItemProvider.ScrollIntoView()
        {
            var wrapperTab = this.GetWrapper() as RibbonTabItem;
            if (wrapperTab != null)
            {
                wrapperTab.BringIntoView();
            }
        }

        #endregion

        internal UIElement GetWrapper()
        {
            var uiElement = (UIElement)null;
            var controlAutomationPeer = this.ItemsControlAutomationPeer;
            var owner = (RibbonTabControl)controlAutomationPeer?.Owner;

            if (owner != null)
            {
                uiElement = owner.IsItemItsOwnContainer(this.item) == false
                                ? owner.ItemContainerGenerator.ContainerFromItem(this.item) as UIElement
                                : this.item as UIElement;
            }

            return uiElement;
        }
    }
}