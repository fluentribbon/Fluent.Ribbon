namespace Fluent.Tests.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// A control removed from <see cref="RibbonToolBar.Children"/> must be released by the toolbar right away,
/// so the app can put it somewhere else.
/// </summary>
/// <remarks>
/// The toolbar only detaches its children (visual and logical parent) in its next measure pass.
/// Until then a removed control still has the toolbar as its parent, and adding it to another panel throws
/// "Specified element is already the logical child of another element".
/// </remarks>
[TestFixture]
public class RibbonToolBarChildrenTests
{
    [Test]
    public void Removed_child_can_be_added_to_another_panel_right_away()
    {
        var button = new Fluent.Button { Header = "Moved" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(button);
        var otherPanel = new StackPanel();
        var root = new StackPanel { Children = { toolBar, otherPanel } };

        using (new TestRibbonWindow(root))
        {
            UIHelper.DoEvents();

            Assert.That(LogicalTreeHelper.GetParent(button), Is.SameAs(toolBar), "Precondition: the toolbar is the button's logical parent");

            toolBar.Children.Remove(button);

            Assert.That(LogicalTreeHelper.GetParent(button), Is.Null, "A removed child must no longer have the toolbar as its logical parent");
            Assert.That(VisualTreeHelper.GetParent(button), Is.Null, "A removed child must no longer have the toolbar as its visual parent");
            Assert.That(() => otherPanel.Children.Add(button), Throws.Nothing, "The app must be able to add the removed child to another panel");
        }
    }

    [Test]
    public void Child_removed_from_a_collapsed_toolbar_is_released()
    {
        var button = new Fluent.Button { Header = "Moved" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(button);
        var otherPanel = new StackPanel();
        var root = new StackPanel { Children = { toolBar, otherPanel } };

        using (new TestRibbonWindow(root))
        {
            UIHelper.DoEvents();

            // A collapsed toolbar (for example on a tab that isn't selected) isn't measured again.
            toolBar.Visibility = Visibility.Collapsed;
            UIHelper.DoEvents();

            toolBar.Children.Remove(button);
            UIHelper.DoEvents();

            Assert.That(() => otherPanel.Children.Add(button), Throws.Nothing, "The app must be able to add a child removed from a collapsed toolbar to another panel");
            Assert.That(LogicalTreeHelper.GetParent(button), Is.SameAs(otherPanel), "The other panel is now the child's logical parent");
        }
    }

    [Test]
    public void Cleared_children_are_released()
    {
        var first = new Fluent.Button { Header = "First" };
        var second = new Fluent.Button { Header = "Second" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(first);
        toolBar.Children.Add(second);

        using (new TestRibbonWindow(toolBar))
        {
            UIHelper.DoEvents();

            toolBar.Children.Clear();

            Assert.That(LogicalTreeHelper.GetParent(first), Is.Null, "Clear must release the first child");
            Assert.That(LogicalTreeHelper.GetParent(second), Is.Null, "Clear must release the second child");
        }
    }

    [Test]
    public void Child_removed_from_a_toolbar_with_a_layout_definition_is_released()
    {
        var inGroup = new Fluent.Button { Name = "InGroup", Header = "In group" };
        var direct = new Fluent.Button { Name = "Direct", Header = "Direct" };
        var kept = new Fluent.Button { Name = "Kept", Header = "Kept" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(inGroup);
        toolBar.Children.Add(direct);
        toolBar.Children.Add(kept);

        var group = new RibbonToolBarControlGroupDefinition();
        group.Children.Add(new RibbonToolBarControlDefinition { Target = "InGroup" });
        group.Children.Add(new RibbonToolBarControlDefinition { Target = "Kept" });
        var row = new RibbonToolBarRow();
        row.Children.Add(group);
        row.Children.Add(new RibbonToolBarControlDefinition { Target = "Direct" });
        var layout = new RibbonToolBarLayoutDefinition();
        layout.Rows.Add(row);
        toolBar.LayoutDefinitions.Add(layout);

        var otherPanel = new StackPanel();
        var root = new StackPanel { Children = { toolBar, otherPanel } };

        using (new TestRibbonWindow(root))
        {
            UIHelper.DoEvents();

            Assert.That(LogicalTreeHelper.GetParent(inGroup), Is.InstanceOf<RibbonToolBarControlGroup>(), "Precondition: the button is shown inside a control group");
            Assert.That(LogicalTreeHelper.GetParent(direct), Is.SameAs(toolBar), "Precondition: the toolbar is the direct button's logical parent");

            toolBar.Visibility = Visibility.Collapsed;
            UIHelper.DoEvents();

            toolBar.Children.Remove(inGroup);
            toolBar.Children.Remove(direct);

            Assert.That(() => otherPanel.Children.Add(inGroup), Throws.Nothing, "A child removed from a control group must be released");
            Assert.That(() => otherPanel.Children.Add(direct), Throws.Nothing, "A child removed from the toolbar must be released");

            toolBar.Visibility = Visibility.Visible;
            UIHelper.DoEvents();

            Assert.That(LogicalTreeHelper.GetParent(kept), Is.InstanceOf<RibbonToolBarControlGroup>(), "The remaining child is still shown inside its control group");
            Assert.That(kept.IsVisible, Is.True, "The remaining child is still shown");
        }
    }

    [Test]
    public void Child_replaced_by_itself_keeps_the_toolbar_as_parent()
    {
        var button = new Fluent.Button { Header = "Same" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(button);

        using (new TestRibbonWindow(toolBar))
        {
            UIHelper.DoEvents();

            toolBar.Visibility = Visibility.Collapsed;
            UIHelper.DoEvents();

            toolBar.Children[0] = button;

            Assert.That(LogicalTreeHelper.GetParent(button), Is.SameAs(toolBar), "A child that is still in the toolbar keeps it as its logical parent");
            Assert.That(VisualTreeHelper.GetParent(button), Is.SameAs(toolBar), "A child that is still in the toolbar keeps it as its visual parent");
        }
    }

    [Test]
    public void Cleared_children_of_a_toolbar_with_a_layout_definition_are_released()
    {
        var inGroup = new Fluent.Button { Name = "InGroup", Header = "In group" };
        var direct = new Fluent.Button { Name = "Direct", Header = "Direct" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(inGroup);
        toolBar.Children.Add(direct);

        var group = new RibbonToolBarControlGroupDefinition();
        group.Children.Add(new RibbonToolBarControlDefinition { Target = "InGroup" });
        var row = new RibbonToolBarRow();
        row.Children.Add(group);
        row.Children.Add(new RibbonToolBarControlDefinition { Target = "Direct" });
        var layout = new RibbonToolBarLayoutDefinition();
        layout.Rows.Add(row);
        toolBar.LayoutDefinitions.Add(layout);

        using (new TestRibbonWindow(toolBar))
        {
            UIHelper.DoEvents();

            toolBar.Children.Clear();

            Assert.That(LogicalTreeHelper.GetParent(inGroup), Is.Null, "Clear must release a child shown inside a control group");
            Assert.That(VisualTreeHelper.GetParent(inGroup), Is.Null, "Clear must release the visual parent of a child shown inside a control group");
            Assert.That(LogicalTreeHelper.GetParent(direct), Is.Null, "Clear must release a child shown directly");
        }
    }

    /// <summary>
    /// Guard: children that stay in the toolbar keep it as their parent after another child is removed.
    /// </summary>
    [Test]
    public void Remaining_children_keep_the_toolbar_as_parent()
    {
        var kept = new Fluent.Button { Header = "Kept" };
        var removed = new Fluent.Button { Header = "Removed" };
        var toolBar = new RibbonToolBar();
        toolBar.Children.Add(kept);
        toolBar.Children.Add(removed);

        using (new TestRibbonWindow(toolBar))
        {
            UIHelper.DoEvents();

            toolBar.Children.Remove(removed);
            UIHelper.DoEvents();

            Assert.That(LogicalTreeHelper.GetParent(kept), Is.SameAs(toolBar), "The remaining child keeps the toolbar as its logical parent");
            Assert.That(kept.IsVisible, Is.True, "The remaining child is still shown");
        }
    }
}
