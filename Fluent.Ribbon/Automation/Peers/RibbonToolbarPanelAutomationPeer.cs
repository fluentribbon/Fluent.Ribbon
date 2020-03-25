namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="RibbonTabControl.ToolbarPanel"/>.
    /// </summary>
    public class RibbonToolbarPanelAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonToolbarPanelAutomationPeer([NotNull] Panel owner)
            : base(owner)
        {
            this.OwnerPanel = owner;
        }

        private Panel OwnerPanel { get; }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "ToolbarPanel";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                // Todo: localization
                name = "Ribbon toolbar";
            }

            return name;
        }
    }
}