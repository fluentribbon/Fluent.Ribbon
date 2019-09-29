namespace Fluent.Automation.Peers
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Base automation peer for <see cref="IHeaderedControl"/>.
    /// </summary>
    public abstract class HeaderedControlAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        protected HeaderedControlAutomationPeer([NotNull] FrameworkElement owner)
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