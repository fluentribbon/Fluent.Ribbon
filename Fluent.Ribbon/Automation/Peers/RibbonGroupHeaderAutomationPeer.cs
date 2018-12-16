namespace Fluent.Automation.Peers
{
    using System.Windows;
    using System.Windows.Automation.Peers;

    /// <summary>
    /// Automation peer for the header of <see cref="RibbonGroupBox"/>.
    /// </summary>
    public class RibbonGroupHeaderAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonGroupHeaderAutomationPeer(FrameworkElement owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Header;
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return false;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var parent = this.GetParent();
            if (parent != null)
            {
                return parent.GetName();
            }

            return string.Empty;
        }
    }
}
