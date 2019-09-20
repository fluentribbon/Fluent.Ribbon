namespace Fluent.Automation.Peers
{
    using System.Runtime.CompilerServices;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Fluent.Internal;
    using JetBrains.Annotations;

    /// <summary>
    /// Automation peer for <see cref="DropDownButton"/>.
    /// </summary>
    public class RibbonDropDownButtonAutomationPeer : RibbonHeaderedControlAutomationPeer, IToggleProvider
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RibbonDropDownButtonAutomationPeer([NotNull] DropDownButton owner)
            : base(owner)
        {
            this.OwnerDropDownButton = owner;
        }

        private DropDownButton OwnerDropDownButton { get; }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "DropDownButton";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Button;
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
        void IToggleProvider.Toggle()
        {
            this.OwnerDropDownButton.IsDropDownOpen = !this.OwnerDropDownButton.IsDropDownOpen;
        }

        /// <inheritdoc />
        ToggleState IToggleProvider.ToggleState => ConvertToToggleState(this.OwnerDropDownButton.IsDropDownOpen);

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