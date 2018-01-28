namespace Fluent.Tests.Controls
{
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using NUnit.Framework;

    [TestFixture]
    public class QuickAccessToolBarTests
    {
        [Test]
        public void TestDefaultKeyTips()
        {
            var toolbar = new QuickAccessToolBar();

            for (var i = 0; i < 30; i++)
            {
                toolbar.Items.Add(new UIElement());
            }

            TestDefaultKeyTips(toolbar);
        }

        private static void TestDefaultKeyTips(QuickAccessToolBar toolbar)
        {
            var keyTips = toolbar.Items.Select(KeyTip.GetKeys);
            var expectedKeyTips = new[]
                                  {
                                      "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                      "09", "08", "07", "06", "05", "04", "03", "02", "01",
                                      "0A", "0B", "0C", "0D", "0E", "0F", "0G", "0H", "0I", "0J", "0K", "0L"
                                  };

            Assert.That(keyTips, Is.EquivalentTo(expectedKeyTips));
        }

        [Test]
        public void TestCustomKeyTips()
        {
            var toolbar = new QuickAccessToolBar
                          {
                              UpdateKeyTipsAction = quickAccessToolBar =>
                                              {
                                                  for (var i = 0; i < quickAccessToolBar.Items.Count; i++)
                                                  {
                                                      KeyTip.SetKeys(quickAccessToolBar.Items[i], (i + 1).ToString("00", CultureInfo.InvariantCulture));
                                                  }
                                              }
                          };

            for (var i = 0; i < 30; i++)
            {
                toolbar.Items.Add(new UIElement());
            }

            var keyTips = toolbar.Items.Select(KeyTip.GetKeys);
            var expectedKeyTips = new[]
                                  {
                                      "01", "02", "03", "04", "05", "06", "07", "08", "09",
                                      "10", "11", "12", "13", "14", "15", "16", "17", "18",
                                      "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30"
                                  };

            Assert.That(keyTips, Is.EquivalentTo(expectedKeyTips));

            toolbar.UpdateKeyTipsAction = null;

            TestDefaultKeyTips(toolbar);
        }
    }
}