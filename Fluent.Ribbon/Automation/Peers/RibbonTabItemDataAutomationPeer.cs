namespace Fluent.Automation.Peers
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    /// <summary>
    /// Automation peer for <see cref="RibbonTabItem"/>.
    /// </summary>
    public class RibbonTabItemDataAutomationPeer : SelectorItemAutomationPeer, ISelectionItemProvider
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
                if (wrapper?.Header is string)
                {
                    return nameCore;
                }
            }

            return nameCore;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TabItem;
        }

        /// <inheritdoc />
        protected override List<AutomationPeer> GetChildrenCore()
        {
            var automationPeerList = base.GetChildrenCore();

            if (this.GetWrapper() is RibbonTabItem wrapper
                && wrapper.IsSelected)
            {
                if (this.ItemsControlAutomationPeer.Owner is RibbonTabControl owner)
                {
                    var contentPresenter = owner.SelectedContentPresenter;

                    if (contentPresenter != null)
                    {
                        var children = new FrameworkElementAutomationPeer(contentPresenter).GetChildren();
                        if (children != null)
                        {
                            if (automationPeerList == null)
                            {
                                automationPeerList = children;
                            }
                            else
                            {
                                automationPeerList.AddRange(children);
                            }
                        }
                    }
                }
            }

            return automationPeerList;
        }

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