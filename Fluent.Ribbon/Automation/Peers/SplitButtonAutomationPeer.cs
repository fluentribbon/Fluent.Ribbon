namespace Fluent.Automation.Peers
{
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="SplitButton"/>.
    /// </summary>
    public class SplitButtonAutomationPeer : DropDownButtonAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public SplitButtonAutomationPeer([NotNull] SplitButton owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "SplitButton";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.SplitButton;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Invoke)
            {
                //return ((SplitButton)this.Owner).button;
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <inheritdoc />
        public void Invoke()
        {
            if (this.IsEnabled() == false)
            {
                throw new ElementNotEnabledException();
            }

            this.RunInDispatcherAsync(() => ((SplitButton)this.Owner).AutomationButtonClick(), DispatcherPriority.Input);
        }
    }
}