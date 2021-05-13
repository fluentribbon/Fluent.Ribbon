namespace Fluent.Automation.Peers
{
    using System.Windows;
    using System.Windows.Automation.Peers;

    /// <inheritdoc />
    public class GalleryItemWrapperAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <inheritdoc cref="FrameworkElementAutomationPeer" />
        public GalleryItemWrapperAutomationPeer(GalleryItem owner)
            : base(owner)
        {
        }

        /// <inheritdoc />
        protected override string GetClassNameCore() => "ListBoxItem";

        /// <inheritdoc />
        protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.ListItem;
    }
}