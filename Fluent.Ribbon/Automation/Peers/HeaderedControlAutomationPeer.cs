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
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = (this.Owner as IHeaderedControl)?.Header as string;
            }

            return name;
        }
    }
}