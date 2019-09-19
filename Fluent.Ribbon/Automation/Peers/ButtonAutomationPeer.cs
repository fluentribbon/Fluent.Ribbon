namespace Fluent.Automation.Peers
{
    using Fluent.Extensions;
    using JetBrains.Annotations;

    /// <inheritdoc />
    public class ButtonAutomationPeer : System.Windows.Automation.Peers.ButtonAutomationPeer
    {
        /// <summary>Initializes a new instance of the <see cref="T:ButtonAutomationPeer" /> class.</summary>
        /// <param name="owner">The element associated with this automation peer.</param>
        public ButtonAutomationPeer([NotNull] Button owner)
            : base(owner)
        {
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