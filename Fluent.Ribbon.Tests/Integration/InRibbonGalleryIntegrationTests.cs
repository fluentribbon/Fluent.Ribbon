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
        public void Opening_And_Closing_DropDown_Should_Not_Change_Size_For_Fixed_Item_Width()
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
                ItemsSource = this.sampleDataItemsForFixedWidth
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
                ItemsSource = this.sampleDataItemsForFixedWidth
            };

            groupBox.Items.Add(secondInRibbonGallery);

            using (new TestRibbonWindow(ribbonGroupsContainer))
            {
                UIHelper.DoEvents();

                ribbonGroupsContainer.Width = 520;

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));

                Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                for (var i = 0; i < 5; i++)
                {
                    UIHelper.DoEvents();

                    Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                    Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                    Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));

                    Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                    // open and close first
                    {
                        firstInRibbonGallery.IsDropDownOpen = true;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(0));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                        firstInRibbonGallery.IsDropDownOpen = false;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(456));
                    }

                    // open and close second
                    {
                        secondInRibbonGallery.IsDropDownOpen = true;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(0));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                        secondInRibbonGallery.IsDropDownOpen = false;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(456));
                    }

                    ++ribbonGroupsContainer.Width;
                }

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(219));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));

                Assert.That(groupBox.ActualWidth, Is.EqualTo(456));

                ribbonGroupsContainer.Width = 560;

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(269));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(269));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(5));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(5));
                Assert.That(groupBox.ActualWidth, Is.EqualTo(556));
            }
        }

        [Test]
        public void Opening_And_Closing_DropDown_Should_Not_Change_Size_For_Dynamic_Item_Width()
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
                ItemHeight = 18,
                GroupBy = "Group",
                ResizeMode = ContextMenuResizeMode.Both,
                ItemsSource = this.sampleDataItemsForDynamicWidth
            };

            groupBox.Items.Add(firstInRibbonGallery);

            var secondInRibbonGallery = new InRibbonGallery
            {
                MinItemsInRow = 1,
                MaxItemsInRow = 5,
                ItemHeight = 18,
                GroupBy = "Group",
                ResizeMode = ContextMenuResizeMode.Both,
                ItemsSource = this.sampleDataItemsForDynamicWidth
            };

            groupBox.Items.Add(secondInRibbonGallery);

            using (new TestRibbonWindow(ribbonGroupsContainer))
            {
                UIHelper.DoEvents();

                ribbonGroupsContainer.Width = 620;

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));

                Assert.That(groupBox.ActualWidth, Is.EqualTo(512));

                for (var i = 0; i < 5; i++)
                {
                    UIHelper.DoEvents();

                    Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                    Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                    Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));
                    Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));

                    Assert.That(groupBox.ActualWidth, Is.EqualTo(512));

                    // open and close first
                    {
                        firstInRibbonGallery.IsDropDownOpen = true;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(0));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(512));

                        firstInRibbonGallery.IsDropDownOpen = false;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(512));
                    }

                    // open and close second
                    {
                        secondInRibbonGallery.IsDropDownOpen = true;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(0));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(512));

                        secondInRibbonGallery.IsDropDownOpen = false;
                        UIHelper.DoEvents();

                        Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                        Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));
                        Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));

                        Assert.That(groupBox.ActualWidth, Is.EqualTo(512));
                    }

                    ++ribbonGroupsContainer.Width;
                }

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(247));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(247));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(3));

                Assert.That(groupBox.ActualWidth, Is.EqualTo(512));

                ribbonGroupsContainer.Width = 670;

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(323));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(323));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(4));
                Assert.That(groupBox.ActualWidth, Is.EqualTo(664));

                ribbonGroupsContainer.Width = 900;

                UIHelper.DoEvents();

                Assert.That(firstInRibbonGallery.ActualWidth, Is.EqualTo(399));
                Assert.That(secondInRibbonGallery.ActualWidth, Is.EqualTo(399));
                Assert.That(firstInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(5));
                Assert.That(secondInRibbonGallery.CurrentGalleryPanelState.GalleryPanel.MaxItemsInRow, Is.EqualTo(5));
                Assert.That(groupBox.ActualWidth, Is.EqualTo(816));
            }
        }

        private readonly SampleDataItem[] sampleDataItemsForFixedWidth =
        {
            new SampleDataItem("A", "Blue"),
            new SampleDataItem("A", "Brown"),
            new SampleDataItem("A", "Gray"),
            new SampleDataItem("A", "Green"),
            new SampleDataItem("A", "Orange"),

            new SampleDataItem("B", "Pink"),
            new SampleDataItem("B", "Red"),
            new SampleDataItem("B", "Yellow")
        };

        private readonly SampleDataItem[] sampleDataItemsForDynamicWidth =
        {
            new SampleDataItem("A", "Blue"),
            new SampleDataItem("A", "Brown"),
            new SampleDataItem("A", "Hallo text text"),
            new SampleDataItem("A", "Green"),
            new SampleDataItem("A", "Orange"),

            new SampleDataItem("B", "Pink"),
            new SampleDataItem("B", "Red"),
            new SampleDataItem("B", "Yellow")
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