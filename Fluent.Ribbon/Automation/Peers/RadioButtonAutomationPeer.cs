namespace Fluent.Automation.Peers
{
    using JetBrains.Annotations;

    /// <inheritdoc />
    public class RadioButtonAutomationPeer : System.Windows.Automation.Peers.RadioButtonAutomationPeer
    {
        /// <summary>Initializes a new instance of the <see cref="T:RadioButtonAutomationPeer" /> class.</summary>
        /// <param name="owner">The element associated with this automation peer.</param>
        public RadioButtonAutomationPeer([NotNull] RadioButton owner)
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