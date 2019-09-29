namespace Fluent.Automation.Peers
{
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    ///     Automation peer for <see cref="ScreenTip" />.
    /// </summary>
    public class ScreenTipAutomationPeer : ToolTipAutomationPeer
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public ScreenTipAutomationPeer([NotNull] ScreenTip owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override bool IsContentElementCore()
        {
            return true;
        }
    }
}