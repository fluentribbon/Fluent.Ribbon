namespace Fluent.Tests.Internal
{
    using System.Windows.Controls;
    using Fluent.Internal;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class WhenLoadedTests
    {
        [Test]
        public void Action_Should_Be_Called_When_Already_Loaded()
        {
            var control = new Control();

            var loadedActionCallCount = 0;

            using (new TestRibbonWindow(control))
            {
                WhenLoadedExtension.WhenLoaded(control, x => ++loadedActionCallCount);

                Assert.That(loadedActionCallCount, Is.EqualTo(1));
            }

            // Check that only one loaded event triggers the action
            using (new TestRibbonWindow(control))
            {
                Assert.That(loadedActionCallCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void Action_Should_Be_Called_When_Loaded_Later()
        {
            var control = new Control();

            var loadedActionCallCount = 0;

            control.WhenLoaded(x => ++loadedActionCallCount);

            Assert.That(loadedActionCallCount, Is.EqualTo(0));

            using (new TestRibbonWindow(control))
            {
                Assert.That(loadedActionCallCount, Is.EqualTo(1));
            }

            // Check that only one loaded event triggers the action
            using (new TestRibbonWindow(control))
            {
                Assert.That(loadedActionCallCount, Is.EqualTo(1));
            }
        }
    }
}