namespace Fluent.Automation.Peers
{
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="DropDownButton"/>.
    /// </summary>
    public class DropDownButtonAutomationPeer : HeaderedControlAutomationPeer, IToggleProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DropDownButtonAutomationPeer([NotNull] DropDownButton owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "DropDownButton";
        }

        /// <inheritdoc />
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Toggle)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <inheritdoc />
        public void Toggle()
        {
            ((DropDownButton)this.Owner).IsDropDownOpen = !((DropDownButton)this.Owner).IsDropDownOpen;
        }

        /// <inheritdoc />
        public ToggleState ToggleState => ConvertToToggleState(((DropDownButton)this.Owner).IsDropDownOpen);

        private static ToggleState ConvertToToggleState(bool value)
        {
            return value
                       ? ToggleState.On
                       : ToggleState.Off;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal virtual void RaiseToggleStatePropertyChangedEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(TogglePatternIdentifiers.ToggleStateProperty, ConvertToToggleState(oldValue), ConvertToToggleState(newValue));
        }
    }
}