namespace Fluent.Tests.Controls
{
    using Fluent.Tests.Helper;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class BackstageTests
    {
        /// <summary>
        /// This test ensures that the <see cref="BackstageAdorner"/> is destroyed as soon as the <see cref="Backstage"/> is unloaded.
        /// </summary>
        [Test]
        public void Adorner_should_be_destroyed_on_unload()
        {
            var backstage = new Backstage
                            {
                                Content = new Button()
                            };

            using (var window = new TestRibbonWindow(backstage))
            {
                Assert.That(backstage.IsLoaded, Is.True);

                Assert.That(backstage.GetPrivateFieldValue("adorner"), Is.Null);

                backstage.IsOpen = true;

                Assert.That(backstage.GetPrivateFieldValue("adorner"), Is.Not.Null);

                backstage.IsOpen = false;

                Assert.That(backstage.GetPrivateFieldValue("adorner"), Is.Not.Null);

                window.Content = null;

                UIHelper.DoEvents();

                Assert.That(backstage.GetPrivateFieldValue("adorner"), Is.Null);
            }
        }
    }
}