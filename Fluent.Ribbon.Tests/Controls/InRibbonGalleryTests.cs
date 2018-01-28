namespace Fluent.Tests.Controls
{
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class InRibbonGalleryTests
    {
        [Test]
        public void Opening_DropDown_Should_Not_Throw_When_GalleryPanel_Has_No_Width()
        {
            var control = new InRibbonGallery
                          {
                              Width = 10,
                              Height = 30
                          };

            using (new TestRibbonWindow(control))
            {
                Assert.That(() => control.IsDropDownOpen = true, Throws.Nothing);
            }
        }
    }
}