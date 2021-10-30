namespace Fluent.Tests.Converters
{
    using Fluent.Converters;
    using Fluent.Internal.KnownBoxes;
    using NUnit.Framework;

    [TestFixture]
    public class InverseBoolConverterTests
    {
        [Test]
        public void TestConvert()
        {
            var converter = new InverseBoolConverter();

            Assert.That(converter.Convert(BooleanBoxes.TrueBox), Is.False);
            Assert.That(converter.Convert(BooleanBoxes.FalseBox), Is.True);
            Assert.That(() => converter.Convert(null), Throws.Exception);
        }

        [Test]
        public void TestConvertBack()
        {
            var converter = new InverseBoolConverter();

            Assert.That(converter.ConvertBack(BooleanBoxes.TrueBox), Is.False);
            Assert.That(converter.ConvertBack(BooleanBoxes.FalseBox), Is.True);
            Assert.That(() => converter.ConvertBack(null), Throws.Exception);
        }
    }
}