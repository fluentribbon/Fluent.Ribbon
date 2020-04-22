namespace Fluent.Automation.Peers
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;

    /// <summary>
    /// Automation peer for ribbon control items.
    /// </summary>
    public class RibbonControlDataAutomationPeer : ItemAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonControlDataAutomationPeer(object item, ItemsControlAutomationPeer itemsControlPeer)
            : base(item, itemsControlPeer)
        {
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            var wrapperPeer = this.GetWrapperPeer();
            return wrapperPeer?.GetClassName() ?? string.Empty;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            // Doesnt implement any patterns of its own, so just forward to the wrapper peer. 
            var wrapperPeer = this.GetWrapperPeer();

            return wrapperPeer?.GetPattern(patternInterface);
        }

        private UIElement GetWrapper()
        {
            var itemsControlAutomationPeer = this.ItemsControlAutomationPeer;

            var owner = (ItemsControl)itemsControlAutomationPeer?.Owner;
            return owner?.ItemContainerGenerator.ContainerFromItem(this.Item) as UIElement;
        }

        private AutomationPeer GetWrapperPeer()
        {
            var wrapper = this.GetWrapper();
            if (wrapper is null)
            {
                return null;
            }

            var wrapperPeer = UIElementAutomationPeer.CreatePeerForElement(wrapper);
            if (!(wrapperPeer is null))
            {
                return wrapperPeer;
            }

            if (wrapper is FrameworkElement element)
            {
                return new FrameworkElementAutomationPeer(element);
            }

            return new UIElementAutomationPeer(wrapper);
        }
    }
}