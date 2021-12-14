namespace Fluent.Tests.Localization
{
    using Fluent.Localization.Languages;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonLocalizationBaseTests
    {
        [Test]
        public void Equals_Test()
        {
            var firstLocation = new German();
            var secLocation = new English();
            var thiredLocation = new German();

            Assert.That(firstLocation.Equals(null), Is.False);
            Assert.That(firstLocation.Equals(secLocation), Is.False);
            Assert.That(firstLocation.Equals(thiredLocation), Is.True);
        }

        [Test]
        public void OperatorEqual_Test()
        {
            var firstLocation = new German();
            var secLocation = new English();
            var thiredLocation = new German();
            
            Assert.That(firstLocation == null, Is.False);
            Assert.That(firstLocation == secLocation, Is.False);
            Assert.That(firstLocation == thiredLocation, Is.True);
        }
        
        [Test]
        public void OperatorUnEqual_Test()
        {
            var firstLocation = new German();
            var secLocation = new English();
            var thiredLocation = new German();
            
            Assert.That(firstLocation != null, Is.True);
            Assert.That(firstLocation != secLocation, Is.True);
            Assert.That(firstLocation != thiredLocation, Is.False);
        }
    }
}