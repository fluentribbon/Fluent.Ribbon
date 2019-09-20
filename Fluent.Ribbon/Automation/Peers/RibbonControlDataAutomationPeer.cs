namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;

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
            return AutomationControlType.DataItem;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "ItemsControlItem";
        }
    }
}