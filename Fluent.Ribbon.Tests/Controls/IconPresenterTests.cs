namespace Fluent.Tests.Controls
{
    using System.Windows.Controls;
    using NUnit.Framework;

    [TestFixture]
    public class IconPresenterTests
    {
        [Test]
        [TestCase(IconSize.Small, "Small", 16)]
        [TestCase(IconSize.Medium, "Medium", 24)]
        [TestCase(IconSize.Large, "Large", 32)]
        public void IconSizeShouldWorkCorrectly(IconSize iconSize, string expectedIcon, double expectedSize)
        {
            var control = new IconPresenter
            {
                SmallIcon = new TextBlock { Text = "Small" },
                MediumIcon = new TextBlock { Text = "Medium" },
                LargeIcon = new TextBlock { Text = "Large" },
                IconSize = iconSize
            };

            Assert.That(control.OptimalIcon, Is.Not.Null);
            Assert.That(control.OptimalIcon, Is.InstanceOf<TextBlock>());
            Assert.That(((TextBlock)control.OptimalIcon).Text, Is.EqualTo(expectedIcon));

            Assert.That(control.Width, Is.EqualTo(expectedSize));
            Assert.That(control.Height, Is.EqualTo(expectedSize));
        }

        [Test]
        [TestCase(IconSize.Small, "Small", "Medium", "Large", "Small")]
        [TestCase(IconSize.Small, null, "Medium", "Large", "Medium")]
        [TestCase(IconSize.Small, null, null, "Large", "Large")]
        [TestCase(IconSize.Small, null, null, null, null)]
        [TestCase(IconSize.Small, null, "Medium", null, "Medium")]
        [TestCase(IconSize.Small, "Small", "Medium", null, "Small")]
        [TestCase(IconSize.Small, "Small", null, null, "Small")]
        [TestCase(IconSize.Small, "Small", null, "Large", "Small")]

        [TestCase(IconSize.Medium, "Small", "Medium", "Large", "Medium")]
        [TestCase(IconSize.Medium, null, "Medium", "Large", "Medium")]
        [TestCase(IconSize.Medium, null, null, "Large", "Large")]
        [TestCase(IconSize.Medium, null, null, null, null)]
        [TestCase(IconSize.Medium, null, "Medium", null, "Medium")]
        [TestCase(IconSize.Medium, "Small", "Medium", null, "Medium")]
        [TestCase(IconSize.Medium, "Small", null, null, "Small")]
        [TestCase(IconSize.Medium, "Small", null, "Large", "Large")]

        [TestCase(IconSize.Large, "Small", "Medium", "Large", "Large")]
        [TestCase(IconSize.Large, null, "Medium", "Large", "Large")]
        [TestCase(IconSize.Large, null, null, "Large", "Large")]
        [TestCase(IconSize.Large, null, null, null, null)]
        [TestCase(IconSize.Large, null, "Medium", null, "Medium")]
        [TestCase(IconSize.Large, "Small", "Medium", null, "Medium")]
        [TestCase(IconSize.Large, "Small", null, null, "Small")]
        [TestCase(IconSize.Large, "Small", null, "Large", "Large")]

        [TestCase(IconSize.Custom, "Small", "Medium", "Large", "Large")]
        [TestCase(IconSize.Custom, null, "Medium", "Large", "Large")]
        [TestCase(IconSize.Custom, null, null, "Large", "Large")]
        [TestCase(IconSize.Custom, null, null, null, null)]
        [TestCase(IconSize.Custom, null, "Medium", null, "Medium")]
        [TestCase(IconSize.Custom, "Small", "Medium", null, "Medium")]
        [TestCase(IconSize.Custom, "Small", null, null, "Small")]
        [TestCase(IconSize.Custom, "Small", null, "Large", "Large")]
        public void IconSizeFallbackShouldReturnCorrectIcon(IconSize iconSize, string smallIcon, string mediumIcon, string largeIcon, string expectedIcon)
        {
            var control = new IconPresenter
            {
                SmallIcon = smallIcon is null ? null : new TextBlock { Text = "Small" },
                MediumIcon = mediumIcon is null ? null : new TextBlock { Text = "Medium" },
                LargeIcon = largeIcon is null ? null : new TextBlock { Text = "Large" },
                IconSize = iconSize
            };

            if (expectedIcon is null)
            {
                Assert.That(control.OptimalIcon, Is.Null);
            }
            else
            {
                Assert.That(control.OptimalIcon, Is.Not.Null);
                Assert.That(control.OptimalIcon, Is.InstanceOf<TextBlock>());
                Assert.That(((TextBlock)control.OptimalIcon).Text, Is.EqualTo(expectedIcon));
            }
        }

        [Test]
        public void IsEnabledFalseShouldSetEffect()
        {
            var control = new IconPresenter();

            Assert.That(control.Effect, Is.Null);

            control.IsEnabled = false;
            Assert.That(control.Effect, Is.Not.Null);
            Assert.That(control.Effect, Is.InstanceOf<GrayscaleEffect>());

            control.IsEnabled = true;
            Assert.That(control.Effect, Is.Null);
        }
    }
}