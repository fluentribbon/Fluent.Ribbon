// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows.Automation.Peers;
    using System.Windows.Controls.Primitives;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for toggle button.
    /// </summary>
    public class FluentToggleButtonAutomationPeer : ToggleButtonAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentToggleButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The class's owner.</param>
        public FluentToggleButtonAutomationPeer([NotNull] ToggleButton owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "FluentToggleButton";
        }

        /// <inheritdoc />
        protected override string GetNameCore()
        {
            string result = base.GetNameCore();
            if (string.IsNullOrEmpty(result))
            {
                AutomationPeer labelAutomationPeer = this.GetLabeledByCore();
                if (labelAutomationPeer != null)
                {
                    result = labelAutomationPeer.GetName();
                }

                if (string.IsNullOrEmpty(result))
                {
                    result = ((IHeaderedControl)this.Owner).Header?.ToString();
                }
            }

            return result ?? string.Empty;
        }
    }
}
