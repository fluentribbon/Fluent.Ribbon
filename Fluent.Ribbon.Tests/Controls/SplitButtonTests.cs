namespace Fluent.Tests.Controls
{
    using Fluent.Tests.TestClasses;
    using FluentTest.Commanding;
    using NUnit.Framework;

    [TestFixture]
    public class SplitButtonTests
    {
        [Test]
        public void Command_Should_Not_Disable_Control()
        {
            var splitButton = new SplitButton
            {
                Command = new RelayCommand(null, () => false)
            };

            using (new TestRibbonWindow(splitButton))
            {
                splitButton.ApplyTemplate();

                Assert.That(splitButton.IsEnabled, Is.True);

                var partButton = splitButton.Template.FindName("PART_Button", splitButton) as ToggleButton;

                Assert.That(partButton, Is.Not.Null);
                Assert.That(partButton.IsEnabled, Is.False);

                splitButton.Command = new RelayCommand(null, () => true);

                Assert.That(splitButton.IsEnabled, Is.True);
                Assert.That(partButton.IsEnabled, Is.True);
            }
        }

        [Test]
        public void Disabling_Control_Should_Disable_Popup()
        {
            var splitButton = new SplitButton
            {
                Command = new RelayCommand(null, () => false)
            };

            using (new TestRibbonWindow(splitButton))
            {
                splitButton.ApplyTemplate();

                Assert.That(splitButton.IsEnabled, Is.True);

                var dummyButton = new Button();

                splitButton.Items.Add(dummyButton);

                Assert.That(dummyButton.IsEnabled, Is.True);

                splitButton.IsDropDownOpen = true;

                splitButton.IsEnabled = false;

                Assert.That(splitButton.IsEnabled, Is.False);
                Assert.That(dummyButton.IsEnabled, Is.False);

                splitButton.IsDropDownOpen = false;

                Assert.That(splitButton.IsEnabled, Is.False);
                Assert.That(dummyButton.IsEnabled, Is.False);
            }
        }
    }
}
