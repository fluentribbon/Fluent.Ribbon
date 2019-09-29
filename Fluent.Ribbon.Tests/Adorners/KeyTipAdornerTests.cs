namespace Fluent.Tests.Adorners
{
    using System.Windows.Controls;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class KeyTipAdornerTests
    {
        [Test]
        public void Adorner_Should_Properly_Grab_Keys_From_KeyTipInformationProvider()
        {
            {
                var splitButton = new SplitButton();
                var panel = new Grid();
                panel.Children.Add(splitButton);
                using (var window = new TestRibbonWindow(panel))
                {
                    var adorner = new KeyTipAdorner(splitButton, panel, null);

                    Assert.That(adorner.KeyTipInformations, Has.Count.EqualTo(0));
                }
            }

            {
                var splitButton = new SplitButton
                {
                    KeyTip = "A"
                };
                var panel = new Grid();
                panel.Children.Add(splitButton);

                using (var window = new TestRibbonWindow(panel))
                {
                    var adorner = new KeyTipAdorner(splitButton, panel, null);

                    Assert.That(adorner.KeyTipInformations, Has.Count.EqualTo(2));
                    Assert.That(adorner.KeyTipInformations[0].Keys, Is.EqualTo("AA"));
                    Assert.That(adorner.KeyTipInformations[1].Keys, Is.EqualTo("AB"));
                }
            }

            {
                var splitButton = new SplitButton
                {
                    SecondaryKeyTip = "B"
                };
                var panel = new Grid();
                panel.Children.Add(splitButton);

                using (var window = new TestRibbonWindow(panel))
                {
                    var adorner = new KeyTipAdorner(splitButton, panel, null);

                    Assert.That(adorner.KeyTipInformations, Has.Count.EqualTo(1));
                    Assert.That(adorner.KeyTipInformations[0].Keys, Is.EqualTo("B"));
                }
            }
        }
    }
}
