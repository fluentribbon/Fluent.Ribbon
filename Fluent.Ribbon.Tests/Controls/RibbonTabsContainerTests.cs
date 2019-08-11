namespace Fluent.Tests.Controls
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonTabsContainerTests
    {
        //private static readonly Size zeroSize = default;
        private const double ReferenceWidth = 300;

        private static readonly double ReferenceHeight = 26;
        private static readonly Size referenceSize = new Size(ReferenceWidth, SystemParameters.WindowCaptionHeight);
        private static readonly double DefaultTabsContainerHeight = SystemParameters.WindowCaptionHeight;

        [Test]
        public void Empty()
        {
            var container = new RibbonTabsContainer();

            using (new TestRibbonWindow(container))
            {
                container.Measure(referenceSize);

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(0, 0)));
            }
        }

        [Test]
        public async Task With_One_Tab()
        {
            var container = new RibbonTabsContainer();
            var tabItem = new RibbonTabItem();
            container.Children.Add(tabItem);

            using (new TestRibbonWindow(container) { Width = ReferenceWidth })
            {
                Assert.That(container.DesiredSize, Is.EqualTo(new Size(16, ReferenceHeight)));

                tabItem.Header = "ABC";

                container.UpdateLayout();

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(38, ReferenceHeight)));
            }

            await Task.Yield();
        }

        [Test]
        public async Task With_Many_Tab()
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
                Assert.That(container.DesiredSize, Is.EqualTo(new Size(290, ReferenceHeight)));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                121,
                                                                78,
                                                                54,
                                                                37
                                                            }));

                testWindow.Width = 290;

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(290, ReferenceHeight)));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                121,
                                                                78,
                                                                54,
                                                                37
                                                            }));

                testWindow.Width = 289;

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(283, ReferenceHeight)));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                114,
                                                                78,
                                                                54,
                                                                37
                                                            }));

                testWindow.Width = 230;

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(229, ReferenceHeight)));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                67,
                                                                71,
                                                                54,
                                                                37
                                                            }));

                testWindow.Width = 150;

                //await Task.Delay(TimeSpan.FromSeconds(30));

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(147, ReferenceHeight)));
                Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                37,
                                                                33,
                                                                40,
                                                                37
                                                            }));

                testWindow.Width = 130;

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(133, ReferenceHeight)));
                Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                30,
                                                                33,
                                                                33,
                                                                37
                                                            }));

                testWindow.Width = 120;

                Assert.That(container.DesiredSize, Is.EqualTo(new Size(133, ReferenceHeight)));
                Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));

                Assert.That(childrenWidths, Is.EquivalentTo(new[]
                                                            {
                                                                30,
                                                                33,
                                                                33,
                                                                37
                                                            }));

                //Assert.That(container.DesiredSize, Is.EqualTo(new Size(246, ReferenceHeight)));
                //Assert.That(container.ExtentWidth, Is.EqualTo(container.ViewportWidth));
            }

            await Task.Yield();
        }
    }
}