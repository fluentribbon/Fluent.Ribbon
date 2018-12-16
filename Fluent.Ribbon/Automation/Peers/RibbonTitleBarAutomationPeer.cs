namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;

    /// <summary>
    /// Automation peer for <see cref="RibbonTitleBar"/>.
    /// </summary>
    public class RibbonTitleBarAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonTitleBarAutomationPeer(RibbonTitleBar owner)
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
            var contentPresenter = this.Owner as HeaderedContentControl;

            if (contentPresenter?.Header != null)
            {
                return contentPresenter.Header.ToString();
            }

            return base.GetNameCore();
        }
    }
}
