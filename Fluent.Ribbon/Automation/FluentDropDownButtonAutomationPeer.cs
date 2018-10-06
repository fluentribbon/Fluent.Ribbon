// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for dropdown button.
    /// </summary>
    public class FluentDropDownButtonAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentDropDownButtonAutomationPeer"/> class.
        /// </summary>
        /// <param name="owner">The class's owner.</param>
        public FluentDropDownButtonAutomationPeer([NotNull] FrameworkElement owner) 
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "FluentDropDownButton";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
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
