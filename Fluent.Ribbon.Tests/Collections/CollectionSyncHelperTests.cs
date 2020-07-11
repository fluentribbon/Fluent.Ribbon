namespace Fluent.Tests.Collections
{
    using System.Collections.ObjectModel;
    using Fluent.Collections;
    using NUnit.Framework;

    [TestFixture]
    public class CollectionSyncHelperTests
    {
        [Test]
        public void NewInstanceShouldCopyItems()
        {
            var source = new ObservableCollection<string> { "One", "Two" };
            var target = new ObservableCollection<string>();

            Assert.That(target, Is.Not.EquivalentTo(source));

            var sync = new CollectionSyncHelper<string>(source, target);

            Assert.That(target, Is.EquivalentTo(source));
        }

        [Test]
        public void CollectionActionShouldSync()
        {
            var source = new ObservableCollection<string>();
            var target = new ObservableCollection<string>();

            Assert.That(target, Is.EquivalentTo(source));

            var sync = new CollectionSyncHelper<string>(source, target);

            Assert.That(target, Is.EquivalentTo(source));

            {
                source.Add("One");

                Assert.That(target, Is.EquivalentTo(source));
            }

            {
                source.RemoveAt(0);

                Assert.That(target, Is.EquivalentTo(source));
            }

            {
                source.Add("One");

                Assert.That(target, Is.EquivalentTo(source));
            }

            {
                source[0] = "Two";

                Assert.That(target, Is.EquivalentTo(source));
            }

            {
                source.Clear();

                Assert.That(target, Is.EquivalentTo(source));
            }
        }
    }
}