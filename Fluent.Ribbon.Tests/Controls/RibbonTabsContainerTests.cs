namespace Fluent.Tests.Controls;

using System.Linq;
using Fluent.Tests.TestClasses;
using NUnit.Framework;
using Size = System.Windows.Size;

[TestFixture]
public class RibbonTabsContainerTests
{
    //private static readonly Size zeroSize = default;
    private const double ReferenceWidth = 326;

    private const double ReferenceHeight = 25;

    [Test]
    public void Empty()
    {
        var container = new RibbonTabsContainer();

        using (new TestRibbonWindow(container))
        {
            Assert.That(container.DesiredSize, Is.EqualTo(new Size(0, 0)));
        }
    }

    [Test]
    public void With_One_Tab()
    {
        var container = new RibbonTabsContainer();
        var tabItem = new RibbonTabItem();
        container.Children.Add(tabItem);

        using (new TestRibbonWindow(container) { Width = ReferenceWidth })
        {
            Assert.That(container.DesiredSize, Is.EqualTo(new Size(18, ReferenceHeight)));

            tabItem.Header = "ABC";

            container.UpdateLayout();

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(42, ReferenceHeight)));
        }
    }

    [Test]
    public void With_Many_Tab()
    {
        var container = new RibbonTabsContainer();

        var longestTab = new RibbonTabItem { Header = "Longest header text" };
        var secondLongestTab = new RibbonTabItem { Header = "Header text" };
        var middleTab = new RibbonTabItem { Header = "Header" };
        var smallTab = new RibbonTabItem { Header = "Text" };

        container.Children.Add(longestTab);
        container.Children.Add(secondLongestTab);
        container.Children.Add(middleTab);
        container.Children.Add(smallTab);

        var childrenWidths = new[]
        {
            longestTab,
            secondLongestTab,
            middleTab,
            smallTab
        }.Select(x => x.DesiredSize.Width);

        using (var testWindow = new TestRibbonWindow(container) { Width = ReferenceWidth })
        {
            Assert.That(container.DesiredSize, Is.EqualTo(new Size(310, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                129,
                83,
                57,
                41
            }));

            container.Measure(new Size(300, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(300, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                129,
                83,
                57,
                41
            }));

            container.Measure(new Size(299, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(299, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                126,
                81,
                55,
                38
            }));

            container.Measure(new Size(290, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(290, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                125,
                81,
                55,
                38
            }));

            container.Measure(new Size(289, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(289, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                78,
                53,
                36
            }));

            container.Measure(new Size(230, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(230, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(285));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                51,
                34
            }));

            container.Measure(new Size(150, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(150, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(285));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                51,
                34
            }));

            container.Measure(new Size(130, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(130, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(285));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                51,
                34
            }));

            container.Measure(new Size(120, ReferenceHeight));

            Assert.That(container.DesiredSize, Is.EqualTo(new Size(120, ReferenceHeight)));
            Assert.That(container.ExtentWidth, Is.EqualTo(282));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                120,
                77,
                51,
                34
            }));
        }
    }
}