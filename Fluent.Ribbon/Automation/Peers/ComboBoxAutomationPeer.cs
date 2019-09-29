namespace Fluent.Automation.Peers
{
    using JetBrains.Annotations;

    /// <inheritdoc />
    public class ComboBoxAutomationPeer : System.Windows.Automation.Peers.ComboBoxAutomationPeer
    {
        /// <summary>Initializes a new instance of the <see cref="T:ComboBoxAutomationPeer" /> class.</summary>
        /// <param name="owner">The element associated with this automation peer.</param>
        public ComboBoxAutomationPeer([NotNull] ComboBox owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            var text = base.GetNameCore();
            var owner = (IHeaderedControl)this.Owner;

            if (string.IsNullOrEmpty(text))
            {
                if (owner.Header is string headerString)
                {
                    return headerString;
                }
            }

            return text;
        }
    }
}