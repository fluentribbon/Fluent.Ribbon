namespace Fluent.Tests.Services
{
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    public class KeyTipServiceTests
    {
        [Test]
        public void TestDefaultKeyTipKeys()
        {
            var ribbon = new Ribbon();
            var keytipService = new KeyTipService(ribbon);

            Assert.That(ribbon.KeyTipKeys, Is.Empty);
            Assert.That(keytipService.KeyTipKeys, Is.EquivalentTo(KeyTipService.DefaultKeyTipKeys));

            var defaultKeys = KeyTipService.DefaultKeyTipKeys.ToList();

            keytipService.KeyTipKeys.RemoveAt(0);

            Assert.That(KeyTipService.DefaultKeyTipKeys, Is.EquivalentTo(defaultKeys));
        }
    }
}