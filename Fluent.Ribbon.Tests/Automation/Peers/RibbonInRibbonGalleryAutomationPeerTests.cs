namespace Fluent.Tests.Automation.Peers
{
    using System.Linq;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using Fluent.Automation.Peers;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonInRibbonGalleryAutomationPeerTests
    {
        [Test]
        public void CreatePeerShouldCreateCorrectPeer()
        {
            var control = new InRibbonGallery();

            {
                var peer = UIElementAutomationPeer.FromElement(control);

                Assert.That(peer, Is.Null);
            }

            {
                var peer = UIElementAutomationPeer.CreatePeerForElement(control);

                Assert.That(peer, Is.InstanceOf<RibbonInRibbonGalleryAutomationPeer>());
            }

            {
                var peer = UIElementAutomationPeer.FromElement(control);

                Assert.That(peer, Is.InstanceOf<RibbonInRibbonGalleryAutomationPeer>());
            }
        }

        [Test]
        public void CreateItemAutomationPeerShouldCreateCorrectPeer()
        {
            var control = new InRibbonGallery();
            var galleryItem = new GalleryItem();
            control.Items.Add(galleryItem);

            using (new TestRibbonWindow(control))
            {
                var peer = UIElementAutomationPeer.CreatePeerForElement(control);

                Assert.That(peer.GetChildren().OfType<GalleryItemAutomationPeer>().FirstOrDefault(x => x.Item == galleryItem), Is.Not.Null);
            }
        }

        [Test]
        public void ExpandCollapseShouldWorkCorrectly()
        {
            var control = new InRibbonGallery();

            var peer = UIElementAutomationPeer.CreatePeerForElement(control);

            var expandCollapsePattern = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

            Assert.That(expandCollapsePattern, Is.Not.Null);

            Assert.That(control.IsDropDownOpen, Is.False);
            Assert.That(expandCollapsePattern.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Collapsed));

            expandCollapsePattern.Expand();
            Assert.That(control.IsDropDownOpen, Is.True);
            Assert.That(expandCollapsePattern.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Expanded));

            expandCollapsePattern.Collapse();
            Assert.That(control.IsDropDownOpen, Is.False);
            Assert.That(expandCollapsePattern.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Collapsed));
        }

        [Test]
        public void ExpandCollapseShouldThrowOnDisabledControl()
        {
            var control = new InRibbonGallery { IsEnabled = false };

            var peer = UIElementAutomationPeer.CreatePeerForElement(control);

            var expandCollapsePattern = peer.GetPattern(PatternInterface.ExpandCollapse) as IExpandCollapseProvider;

            Assert.That(expandCollapsePattern, Is.Not.Null);

            Assert.That(control.IsDropDownOpen, Is.False);
            Assert.That(expandCollapsePattern.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Collapsed));

            Assert.That(() => expandCollapsePattern.Expand(), Throws.Exception.InstanceOf<ElementNotEnabledException>());
            Assert.That(control.IsDropDownOpen, Is.False);
            Assert.That(expandCollapsePattern.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Collapsed));

            Assert.That(() => expandCollapsePattern.Collapse(), Throws.Exception.InstanceOf<ElementNotEnabledException>());
            Assert.That(control.IsDropDownOpen, Is.False);
            Assert.That(expandCollapsePattern.ExpandCollapseState, Is.EqualTo(ExpandCollapseState.Collapsed));
        }

        //ISelectionItemProvider
        [Test]
        public void SelectionShouldWorkCorrectly()
        {
            var control = new InRibbonGallery();
            var galleryItem = new GalleryItem();
            control.Items.Add(galleryItem);

            using (new TestRibbonWindow(control))
            {
                var peer = UIElementAutomationPeer.CreatePeerForElement(control);

                Assert.That(control.SelectedItem, Is.Null);

                var galleryItemPeer = peer.GetChildren().OfType<GalleryItemAutomationPeer>().FirstOrDefault(x => x.Item == galleryItem);

                Assert.That(galleryItemPeer, Is.Not.Null);

                var selectionPeer = (ISelectionItemProvider)galleryItemPeer;

                Assert.That(selectionPeer.IsSelected, Is.False);

                selectionPeer.Select();

                Assert.That(selectionPeer.IsSelected, Is.True);
                Assert.That(control.SelectedItem, Is.EqualTo(galleryItem));

                selectionPeer.RemoveFromSelection();

                Assert.That(selectionPeer.IsSelected, Is.False);
                Assert.That(control.SelectedItem, Is.Null);

                selectionPeer.AddToSelection();

                Assert.That(selectionPeer.IsSelected, Is.True);
                Assert.That(control.SelectedItem, Is.EqualTo(galleryItem));

                selectionPeer.RemoveFromSelection();

                Assert.That(selectionPeer.IsSelected, Is.False);
                Assert.That(control.SelectedItem, Is.Null);

                control.SelectedItem = galleryItem;

                Assert.That(selectionPeer.IsSelected, Is.True);
                Assert.That(control.SelectedItem, Is.EqualTo(galleryItem));
            }
        }
    }
}