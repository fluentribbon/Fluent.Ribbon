namespace Fluent.Tests.Integration
{
    using System.Windows.Media;
    using Fluent.Tests.Helper;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class InRibbonGalleryIntegrationTests
    {
        [Test]
        public void Opening_And_Closing_DropDown_Should_Not_Change_Size()
        {
            var ribbonGroupsContainer = new RibbonGroupsContainer
            {
                Height = RibbonTabControl.DefaultContentHeight,
                ReduceOrder = "(MyGroup),(MyGroup),(MyGroup),(MyGroup),(MyGroup),(MyGroup),(MyGroup),(MyGroup),(MyGroup),(MyGroup)"
            };

            var groupBox = new RibbonGroupBox
            {
                Name = "MyGroup",
                BorderBrush = Brushes.Red
            };

            ribbonGroupsContainer.Children.Add(groupBox);

            var firstInRibbonGallery = new InRibbonGallery
            {
                MinItemsInRow = 1,
                MaxItemsInRow = 5,
                ItemWidth = 50,
                ItemHeight = 18,
                GroupBy = "Group",
                ResizeMode = ContextMenuResizeMode.Both,
                ItemsSource = this.sampleDataItems
            };

            groupBox.Items.Add(firstInRibbonGallery);

            var secondInRibbonGallery = new InRibbonGallery
            {
                MinItemsInRow = 1,
                MaxItemsInRow = 5,
                ItemWidth = 50,
                ItemHeight = 18,
                GroupBy = "Group",
                ResizeMode = ContextMenuResizeMode.Both,
                ItemsSource = this.sampleDataItems
            };

            groupBox.Items.Add(secondInRibbonGallery);

            using (new TestRibbonWindow(ribbonGroupsContainer))
            {
                UIHelper.DoEvents();

                ribbonGroupsContainer.Width = 520;

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));

                Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                for (var i = 0; i < 5; i++)
                {
                    UIHelper.DoEvents();

                    Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                    firstInRibbonGallery.IsDropDownOpen = true;
                    UIHelper.DoEvents();

                    Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                    firstInRibbonGallery.IsDropDownOpen = false;
                    UIHelper.DoEvents();

                    Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                    ++ribbonGroupsContainer.Width;
                }

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                //ribbonGroupsContainer.Width = 700;

                //UIHelper.DoEvents();

                //Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                //Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(269));
                //Assert.That(groupBox.ActualWidth, Is.EqualTo(506));

                //ribbonGroupsContainer.Width = 700;

                //UIHelper.DoEvents();

                //Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(269));
                //Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(269));
                //Assert.That(groupBox.ActualWidth, Is.EqualTo(556));
            }
        }

        private readonly SampleDataItem[] sampleDataItems =
        {
            new SampleDataItem("A", "1"),
            new SampleDataItem("A", "2"),
            new SampleDataItem("A", "3"),
            new SampleDataItem("A", "4"),
            new SampleDataItem("A", "5"),

            new SampleDataItem("B", "1"),
            new SampleDataItem("B", "2"),
            new SampleDataItem("B", "3")
        };

        private class SampleDataItem
        {
            public SampleDataItem(string group, string text)
            {
                this.Group = group;
                this.Text = text;
            }

            public string Group { get; }

            public string Text { get; }

            /// <inheritdoc />
            public override string ToString()
            {
                return this.Text;
            }
        }
    }
}