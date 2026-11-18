namespace Fluent.Tests.Automation.Peers;

using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using Fluent.Automation.Peers;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// The quick access toolbar has two icon-only buttons: "Customize Quick Access Toolbar" and, when not all items fit,
/// "More controls". Icons carry no text, so without an automation name a screen reader announces them as unnamed buttons.
/// </summary>
[TestFixture]
public class RibbonQuickAccessToolBarAutomationPeerTests
{
    [Test]
    public void Customize_and_more_controls_buttons_are_exposed_with_localized_names()
    {
        var quickAccessToolBar = new QuickAccessToolBar
        {
            Width = 50,
            HorizontalAlignment = HorizontalAlignment.Left
        };

        using (new TestRibbonWindow(quickAccessToolBar))
        {
            UIHelper.DoEvents();

            // Real overflow depends on layout, which doesn't run reliably in the hidden test window.
            // HasOverflowItems is what the template's trigger uses to show the "More controls" button,
            // so set it directly (it is read only, hence the private key) and don't run layout afterwards,
            // because a measure pass would recalculate it.
            var hasOverflowItemsKey = (DependencyPropertyKey)typeof(QuickAccessToolBar)
                .GetField("HasOverflowItemsPropertyKey", BindingFlags.NonPublic | BindingFlags.Static)!
                .GetValue(null)!;
            quickAccessToolBar.SetValue(hasOverflowItemsKey, true);

            Assert.That(quickAccessToolBar.HasOverflowItems, Is.True, "Precondition: the toolbar must report overflow so the \"More controls\" button is shown");

            var peer = UIElementAutomationPeer.CreatePeerForElement(quickAccessToolBar);
            Assert.That(peer, Is.InstanceOf<Fluent.Automation.Peers.RibbonQuickAccessToolBarAutomationPeer>(), "Precondition");

            // CreatePeerForElement returns the existing peer if one was created earlier, e.g. by a UI Automation client
            // (screen reader, accessibility tool) that inspected the window before HasOverflowItems was set above.
            // Such a peer keeps the child list it built back then, so rebuild it to see the buttons the toolbar shows now.
            peer.ResetChildrenCache();

            var childNames = peer.GetChildren().Select(x => x.GetName()).ToList();

            var localization = RibbonLocalization.Current.Localization;
            Assert.That(childNames, Does.Contain(localization.QuickAccessToolBarDropDownButtonTooltip), "The customize button must be named");
            Assert.That(childNames, Does.Contain(localization.QuickAccessToolBarMoreControlsButtonTooltip), "The \"More controls\" button must be exposed and named");
        }
    }
}
