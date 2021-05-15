namespace Fluent.Automation.Peers
{
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    /// <summary>
    /// <see cref="AutomationPeer"/> for <see cref="GalleryItem"/>.
    /// </summary>
    public class GalleryItemAutomationPeer : SelectorItemAutomationPeer, IScrollItemProvider
    {
        /// <inheritdoc cref="SelectorItemAutomationPeer" />
        public GalleryItemAutomationPeer(object owner, SelectorAutomationPeer selectorAutomationPeer)
            : base(owner, selectorAutomationPeer)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore()
        {
            return "GalleryItem";
        }

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.ListItem;
        }

        /// <inheritdoc />
        public override object? GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.ScrollItem)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        void IScrollItemProvider.ScrollIntoView()
        {
            switch (this.ItemsControlAutomationPeer.Owner)
            {
                case InRibbonGallery parent:
                    parent.ScrollIntoView(this.Item);
                    break;
                case ListBox parent:
                    parent.ScrollIntoView(this.Item);
                    break;
            }
        }
    }
}