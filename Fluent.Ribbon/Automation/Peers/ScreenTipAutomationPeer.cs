namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;
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
        protected override string GetClassNameCore()
        {
            return "ScreenTip";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = ((ScreenTip)this.Owner).Title;
            }

            return name;
        }
    }
}