namespace Fluent.Automation.Peers
{
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using Fluent.Internal.KnownBoxes;
    using JetBrains.Annotations;

    /// <summary>
    ///     Automation peer for <see cref="InRibbonGallery" />
    /// </summary>
    // todo: add full automation for listing items (?) etc.
    public class RibbonInRibbonGalleryAutomationPeer : SelectorAutomationPeer, IExpandCollapseProvider
    {
        private readonly InRibbonGallery owner;

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public RibbonInRibbonGalleryAutomationPeer([NotNull] InRibbonGallery owner)
            : base(owner)
        {
            this.owner = owner;
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return this.Owner.GetType().Name;
        }

        /// <inheritdoc />
        protected override string? GetNameCore()
        {
            var name = base.GetNameCore();

            if (string.IsNullOrEmpty(name))
            {
                name = (this.Owner as IHeaderedControl)?.Header as string;
            }

            return name;
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.List;
        }

        /// <inheritdoc />
        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new GalleryItemAutomationPeer(item, this);
        }

        /// <inheritdoc />
        public override object? GetPattern(PatternInterface patternInterface)
        {
            return patternInterface switch
            {
                PatternInterface.ExpandCollapse => this,
                PatternInterface.Scroll when this.owner.IsDropDownOpen == false => this,
                _ => base.GetPattern(patternInterface)
            };
        }

        /// <inheritdoc />
        public void Collapse()
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            this.owner.SetCurrentValue(InRibbonGallery.IsDropDownOpenProperty, BooleanBoxes.FalseBox);
        }

        /// <inheritdoc />
        public void Expand()
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            this.owner.SetCurrentValue(InRibbonGallery.IsDropDownOpenProperty, BooleanBoxes.TrueBox);
        }

        /// <inheritdoc />
        public ExpandCollapseState ExpandCollapseState => this.owner.IsDropDownOpen
            ? ExpandCollapseState.Expanded
            : ExpandCollapseState.Collapsed;

        // BUG 1555137: Never inline, as we don't want to unnecessarily link the automation DLL
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
        {
            this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
                oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
                newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
        }
    }
}