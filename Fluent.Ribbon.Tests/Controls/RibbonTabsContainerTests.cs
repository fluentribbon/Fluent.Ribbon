namespace Fluent.Tests.Controls;

using System.Linq;
using System.Windows.Controls;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;
using Size = System.Windows.Size;

[TestFixture]
public class RibbonTabsContainerTests
{
    //private static readonly Size zeroSize = default;
    private const double ReferenceWidth = 320;

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
        var tabsContainer = new RibbonTabsContainer();
        var tabItem = new RibbonTabItem();
        tabsContainer.Children.Add(tabItem);

        var container = new ContentControl { Content = tabsContainer, Width = ReferenceWidth };

        using (new TestRibbonWindow(container))
        {
            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(18, ReferenceHeight)));

            tabItem.Header = "ABC";

            tabsContainer.UpdateLayout();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(42, ReferenceHeight)));
        }
    }

    [Test]
    public void With_Many_Tab()
    {
        var tabsContainer = new RibbonTabsContainer();

        var longestTab = new RibbonTabItem { Header = "Longest header text" };
        var secondLongestTab = new RibbonTabItem { Header = "Header text" };
        var middleTab = new RibbonTabItem { Header = "Header" };
        var smallTab = new RibbonTabItem { Header = "Text" };

        tabsContainer.Children.Add(longestTab);
        tabsContainer.Children.Add(secondLongestTab);
        tabsContainer.Children.Add(middleTab);
        tabsContainer.Children.Add(smallTab);

        var childrenWidths = new[]
        {
            longestTab,
            secondLongestTab,
            middleTab,
            smallTab
        }.Select(x => x.DesiredSize.Width);

        var container = new ContentControl { Content = tabsContainer, Width = ReferenceWidth };

        using (var testWindow = new TestRibbonWindow(container))
        {
            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(317, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(tabsContainer.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                131,
                85,
                59,
                42
            }));

            container.Width = 300;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(300, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(tabsContainer.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                126,
                81,
                55,
                38
            }));

            container.Width = 299;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(299, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(tabsContainer.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                125,
                81,
                55,
                38
            }));

            container.Width = 290;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(290, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(tabsContainer.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                78,
                53,
                36
            }));

            container.Width = 289;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(289, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(tabsContainer.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                53,
                36
            }));

            container.Width = 230;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(230, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(285));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                51,
                34
            }));

            container.Width = 150;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(150, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(285));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                51,
                34
            }));

            container.Width = 130;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(130, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(285));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                123,
                77,
                51,
                34
            }));

            container.Width = 120;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(120, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(282));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                120,
                77,
                51,
                34
            }));

            container.Width = 300;
            UIHelper.DoEvents();

            Assert.That(tabsContainer.DesiredSize, Is.EqualTo(new Size(300, ReferenceHeight)));
            Assert.That(tabsContainer.ExtentWidth, Is.EqualTo(tabsContainer.ViewportWidth));

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                127,
                81,
                55,
                37
            }));

            container.Width = ReferenceWidth;
            UIHelper.DoEvents();

            Assert.That(childrenWidths, Is.EquivalentTo(new[]
            {
                131,
                85,
                59,
                42
            }));
        }
    }
}