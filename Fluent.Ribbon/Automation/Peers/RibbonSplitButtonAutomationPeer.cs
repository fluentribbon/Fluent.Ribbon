namespace Fluent.Automation.Peers
{
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Fluent.Extensions;
    using Fluent.Internal;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="SplitButton"/>.
    /// </summary>
    public class RibbonSplitButtonAutomationPeer : RibbonDropDownButtonAutomationPeer, IInvokeProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonSplitButtonAutomationPeer([NotNull] SplitButton owner)
            : base(owner)
        {
            this.SplitButtonOnwer = owner;
        }

        private SplitButton SplitButtonOnwer { get; }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.SplitButton;
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Invoke:
                    return this;

                default:
                    return base.GetPattern(patternInterface);
            }
        }

        /// <inheritdoc />
        protected override string GetAutomationIdCore()
        {
            var id = base.GetAutomationIdCore();

            if (string.IsNullOrEmpty(id))
            {
                if (this.SplitButtonOnwer.Command is RoutedCommand routedCommand
                    && string.IsNullOrEmpty(routedCommand.Name) == false)
                {
                    id = routedCommand.Name;
                }
            }

            return id ?? string.Empty;
        }

        /// <inheritdoc />
        protected override string? GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                if (this.SplitButtonOnwer.Command is RoutedUICommand routedUiCommand
                    && string.IsNullOrEmpty(routedUiCommand.Text) == false)
                {
                    name = routedUiCommand.Text;
                }
                else if (this.SplitButtonOnwer.Button?.Content is string buttonContent)
                {
                    name = AccessTextHelper.RemoveAccessKeyMarker(buttonContent);
                }
            }

            return name;
        }

        /// <inheritdoc />
        public void Invoke()
        {
            if (this.IsEnabled() == false)
            {
                throw new ElementNotEnabledException();
            }

            this.RunInDispatcherAsync(() => this.SplitButtonOnwer.AutomationButtonClick(), DispatcherPriority.Input);
        }
    }
}