namespace Fluent.Automation.Peers
{
    using JetBrains.Annotations;

    /// <summary>
    ///     Automation peer for <see cref="InRibbonGallery" />
    /// </summary>
    // todo: add full automation for expansion, listing items (?) etc.
    public class InRibbonGalleryAutomationPeer : HeaderedControlAutomationPeer
    {
        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        public InRibbonGalleryAutomationPeer([NotNull] InRibbonGallery owner)
            : base(owner)
        {
        }
    }
}