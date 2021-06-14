namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    ///     Automation peer for <see cref="BackstageTabControl" />.
    /// </summary>
    public class RibbonBackstageTabControlAutomationPeer : SelectorAutomationPeer, ISelectionProvider
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public RibbonBackstageTabControlAutomationPeer([NotNull] BackstageTabControl owner)
            : base(owner)
        {
            this.OwningBackstageTabControl = owner;
        }

        private BackstageTabControl OwningBackstageTabControl { get; }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Tab;
        }

        /// <inheritdoc />
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new RibbonControlDataAutomationPeer(item, this);
        }

        bool ISelectionProvider.IsSelectionRequired => true;

        bool ISelectionProvider.CanSelectMultiple => false;
    }
}