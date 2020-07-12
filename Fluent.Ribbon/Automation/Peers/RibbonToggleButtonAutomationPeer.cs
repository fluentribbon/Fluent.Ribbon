namespace Fluent.Automation.Peers
{
    using JetBrains.Annotations;

    /// <inheritdoc />
    public class RibbonToggleButtonAutomationPeer : System.Windows.Automation.Peers.ToggleButtonAutomationPeer
    {
        /// <summary>Initializes a new instance of the <see cref="T:ToggleButtonAutomationPeer" /> class.</summary>
        /// <param name="owner">The element associated with this automation peer.</param>
        public RibbonToggleButtonAutomationPeer([NotNull] ToggleButton owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "RibbonToggleButton";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = (this.Owner as IHeaderedControl)?.Header as string;
            }

            return name;
        }
    }
}