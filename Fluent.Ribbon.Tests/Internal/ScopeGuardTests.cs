namespace Fluent.Tests.Internal
{
    using Fluent.Internal;
    using NUnit.Framework;

    [TestFixture]
    public class ScopeGuardTests
    {
        [Test]
        public void IsActive_Marker_Should_Change_On_Dispose()
        {
            var guard = new ScopeGuard();

            Assert.That(guard.IsActive, Is.True);

            guard.Dispose();

            Assert.That(guard.IsActive, Is.False);

            Assert.That(() => guard.Dispose(), Throws.Nothing);

            Assert.That(guard.IsActive, Is.False);
        }

        [Test]
        public void Actions_Should_Be_Called()
        {
            var entryActionCallCount = 0;
            void EntryAction() => ++entryActionCallCount;

            var disposeActionCallCount = 0;
            void DisposeAction() => ++disposeActionCallCount;

            var guard = new ScopeGuard(EntryAction, DisposeAction);

            Assert.That(entryActionCallCount, Is.EqualTo(1));
            Assert.That(disposeActionCallCount, Is.EqualTo(0));

            guard.Dispose();

            Assert.That(entryActionCallCount, Is.EqualTo(1));
            Assert.That(disposeActionCallCount, Is.EqualTo(1));

            guard.Dispose();

            Assert.That(entryActionCallCount, Is.EqualTo(1));
            Assert.That(disposeActionCallCount, Is.EqualTo(1));
        }
    }
}