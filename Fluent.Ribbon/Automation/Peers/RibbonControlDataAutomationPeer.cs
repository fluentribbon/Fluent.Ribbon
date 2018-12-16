namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;
    using Fluent.Extensions;

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

            if (wrapperPeer != null)
            {
                return wrapperPeer.GetClassName();
            }

            return string.Empty;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            object result = null;
            var wrapperPeer = this.GetWrapperPeer();

            if (wrapperPeer != null)
            {
                result = wrapperPeer.GetPattern(patternInterface);
            }

            return result;
        }
    }
}