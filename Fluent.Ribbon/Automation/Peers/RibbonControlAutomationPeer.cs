namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;

    /// <summary>
    /// Automation peer for <see cref="RibbonControl" />.
    /// </summary>
    public class RibbonControlAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonControlAutomationPeer(RibbonControl owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }
    }
}