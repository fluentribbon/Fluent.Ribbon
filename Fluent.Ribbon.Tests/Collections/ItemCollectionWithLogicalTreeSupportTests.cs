namespace Fluent.Tests.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using Fluent.Collections;
    using NUnit.Framework;

    [TestFixture]
    public class ItemCollectionWithLogicalTreeSupportTests
    {
        [Test]
        public void EmptyCollection()
        {
            var logicalChildSupportStub = new LogicalChildSupportNoFEStub();
            var collection = new ItemCollectionWithLogicalTreeSupport<object>(logicalChildSupportStub);

            Assert.That(collection.IsOwningItems, Is.True);
            Assert.That(collection, Is.Empty);
        }

        [Test]
        public void AddRemoveNullDoesNotThrow()
        {
            var logicalChildSupportStub = new LogicalChildSupportNoFEStub();
            var collection = new ItemCollectionWithLogicalTreeSupport<object>(logicalChildSupportStub);

            Assert.That(() => collection.Remove(null), Throws.Nothing);
            Assert.That(() => collection.Add(null), Throws.Nothing);
        }

        [Test]
        public void AquireAndReleaseDoesNotThrowOnEmptyCollection()
        {
            var logicalChildSupportStub = new LogicalChildSupportNoFEStub();
            var collection = new ItemCollectionWithLogicalTreeSupport<object>(logicalChildSupportStub);

            Assert.That(() => collection.AquireLogicalOwnership(), Throws.Nothing);
            Assert.That(() => collection.ReleaseLogicalOwnership(), Throws.Nothing);
        }

        [Test]
        public void CollectionOperationsWorkAsExpected()
        {
            var logicalChildSupportStub = new LogicalChildSupportNoFEStub();
            var collection = new ItemCollectionWithLogicalTreeSupport<object>(logicalChildSupportStub);

            var item1 = "Item_1";
            var item2 = "Item_2";
            var item3 = "Item_3";

            // add items
            {
                collection.Add(item1);
                collection.Add(item2);
                collection.Add(item3);
            }

            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            collection.Clear();

            Assert.That(collection, Is.Empty);
            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            // add items
            {
                collection.Add(item1);
                collection.Add(item2);
                collection.Add(item3);
            }

            collection.RemoveAt(1);

            Assert.That(collection, Is.EquivalentTo(new[] { item1, item3 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.EquivalentTo(collection));
            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            collection[1] = item2;

            Assert.That(collection, Is.EquivalentTo(new[] { item1, item2 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.EquivalentTo(collection));
            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            collection.Insert(0, item3);

            Assert.That(collection, Is.EquivalentTo(new[] { item3, item1, item2 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.EquivalentTo(collection));
            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            collection.ReleaseLogicalOwnership();

            Assert.That(collection, Is.EquivalentTo(new[] { item3, item1, item2 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.Empty);
            Assert.That(collection.GetLogicalChildren(), Is.Empty);

            Assert.That(() => collection.ReleaseLogicalOwnership(), Throws.Nothing);

            Assert.That(logicalChildSupportStub.LogicalChildren, Is.Empty);
            Assert.That(collection.GetLogicalChildren(), Is.Empty);

            var item4 = "Item_4";

            collection.Add(item4);

            Assert.That(collection, Is.EquivalentTo(new[] { item3, item1, item2, item4 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.Empty);
            Assert.That(collection.GetLogicalChildren(), Is.Empty);

            collection.AquireLogicalOwnership();

            Assert.That(collection, Is.EquivalentTo(new[] { item3, item1, item2, item4 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.EquivalentTo(collection));
            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            Assert.That(() => collection.AquireLogicalOwnership(), Throws.Nothing);

            Assert.That(logicalChildSupportStub.LogicalChildren, Is.EquivalentTo(collection));
            Assert.That(collection.GetLogicalChildren(), Is.EquivalentTo(collection));

            collection.ReleaseLogicalOwnership();

            collection.Remove(item4);

            Assert.That(collection, Is.EquivalentTo(new[] { item3, item1, item2 }));
            Assert.That(logicalChildSupportStub.LogicalChildren, Is.Empty);
            Assert.That(collection.GetLogicalChildren(), Is.Empty);
        }

        [Test]
        public void AddRemoveWorksAsExpectedWithNoFEParent()
        {
            var logicalChildSupportStub = new LogicalChildSupportNoFEStub();
            var collection = new ItemCollectionWithLogicalTreeSupport<object>(logicalChildSupportStub);

            var objItem = new object();

            {
                collection.Add(objItem);

                Assert.That(collection, Does.Contain(objItem));
                Assert.That(logicalChildSupportStub.LogicalChildren, Does.Contain(objItem));

                Assert.That(collection.GetLogicalChildren(), Does.Contain(objItem));
            }

            var frameworkElement = new FrameworkElement();

            {
                collection.Add(frameworkElement);

                Assert.That(collection, Does.Contain(frameworkElement));
                Assert.That(logicalChildSupportStub.LogicalChildren, Does.Contain(frameworkElement));

                Assert.That(collection.GetLogicalChildren(), Does.Contain(frameworkElement));
            }

            var frameworkContentElement = new FrameworkContentElement();

            {
                collection.Add(frameworkContentElement);

                Assert.That(collection, Does.Contain(frameworkContentElement));
                Assert.That(logicalChildSupportStub.LogicalChildren, Does.Contain(frameworkContentElement));

                Assert.That(collection.GetLogicalChildren(), Does.Contain(frameworkContentElement));
            }

            // Remove
            {
                collection.Remove(frameworkElement);

                Assert.That(collection, Does.Not.Contain(frameworkElement));
                Assert.That(logicalChildSupportStub.LogicalChildren, Does.Not.Contain(frameworkElement));

                Assert.That(collection.GetLogicalChildren(), Does.Not.Contain(frameworkElement));
            }
        }

        [Test]
        public void AddRemoveWorksAsExpectedWithFEParent()
        {
            var logicalChildSupportStub = new LogicalChildSupportFEStub();
            var collection = new ItemCollectionWithLogicalTreeSupport<object>(logicalChildSupportStub);

            var objItem = new object();

            {
                collection.Add(objItem);

                Assert.That(collection, Does.Contain(objItem));
                Assert.That(LogicalTreeHelper.GetChildren(logicalChildSupportStub), Does.Contain(objItem));

                Assert.That(collection.GetLogicalChildren(), Does.Contain(objItem));
            }

            var frameworkElement = new FrameworkElement();

            {
                collection.Add(frameworkElement);

                Assert.That(collection, Does.Contain(frameworkElement));
                Assert.That(LogicalTreeHelper.GetChildren(logicalChildSupportStub), Does.Contain(frameworkElement));

                Assert.That(collection.GetLogicalChildren(), Does.Contain(frameworkElement));
            }

            var frameworkContentElement = new FrameworkContentElement();

            {
                collection.Add(frameworkContentElement);

                Assert.That(collection, Does.Contain(frameworkContentElement));
                Assert.That(LogicalTreeHelper.GetChildren(logicalChildSupportStub), Does.Contain(frameworkContentElement));

                Assert.That(collection.GetLogicalChildren(), Does.Contain(frameworkContentElement));
            }

            // Remove
            {
                collection.Remove(frameworkElement);

                Assert.That(collection, Does.Not.Contain(frameworkElement));
                Assert.That(LogicalTreeHelper.GetChildren(logicalChildSupportStub), Does.Not.Contain(frameworkElement));

                Assert.That(collection.GetLogicalChildren(), Does.Not.Contain(frameworkElement));
            }
        }

        private class LogicalChildSupportNoFEStub : ILogicalChildSupport
        {
            public List<object> LogicalChildren { get; } = new List<object>();

            /// <inheritdoc />
            void ILogicalChildSupport.AddLogicalChild(object child)
            {
                this.LogicalChildren.Add(child);
            }

            /// <inheritdoc />
            void ILogicalChildSupport.RemoveLogicalChild(object child)
            {
                this.LogicalChildren.Remove(child);
            }
        }

        private class LogicalChildSupportFEStub : FrameworkElement, ILogicalChildSupport
        {
            private List<object> MyLogicalChildren { get; } = new List<object>();

            /// <inheritdoc />
            void ILogicalChildSupport.AddLogicalChild(object child)
            {
                this.MyLogicalChildren.Add(child);
                this.AddLogicalChild(child);
            }

            /// <inheritdoc />
            void ILogicalChildSupport.RemoveLogicalChild(object child)
            {
                this.MyLogicalChildren.Remove(child);
                this.RemoveLogicalChild(child);
            }

            /// <inheritdoc />
            protected override IEnumerator LogicalChildren => this.MyLogicalChildren.GetEnumerator();
        }
    }
}