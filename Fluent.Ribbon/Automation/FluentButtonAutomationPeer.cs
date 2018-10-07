// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for button.
    /// </summary>
    public class FluentButtonAutomationPeer : ButtonAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The class's owner.</param>
        public FluentButtonAutomationPeer([NotNull] System.Windows.Controls.Button owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "FluentButton";
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
