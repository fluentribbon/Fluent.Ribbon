namespace Fluent.Tests.Controls
{
    using System.Windows.Markup;
    using Fluent.Tests.TestClasses;
    using NUnit.Framework;

    [TestFixture]
    public class RibbonTests
    {
        [Test]
        public void DependencyProperties_and_DataContext_should_be_inherited_from_window()
        {
            var ribbon = new Ribbon
                         {
                             Menu = new Backstage(),
                             StartScreen = new StartScreen()
                         };

            var enUs = XmlLanguage.GetLanguage("en-US");
            var deDe = XmlLanguage.GetLanguage("de-DE");

            using (var window = new TestRibbonWindow(ribbon)
                                {
                                    Language = deDe,
                                    DataContext = deDe
                                })
            {
                ribbon.ApplyTemplate();

                {
                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(window.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(window.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(window.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(window.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(window.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(window.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(window.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(window.DataContext), "DataContext on TabControl should match.");
                }

                {
                    window.Language = enUs;
                    window.DataContext = window.Language;

                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(window.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(window.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(window.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(window.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(window.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(window.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(window.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(window.DataContext), "DataContext on TabControl should match.");
                }

                {
                    window.Language = deDe;
                    window.DataContext = window.Language;

                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(window.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(window.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(window.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(window.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(window.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(window.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(window.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(window.DataContext), "DataContext on TabControl should match.");
                }

                {
                    window.Language = enUs;
                    window.DataContext = window.Language;

                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(window.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(window.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(window.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(window.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(window.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(window.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(window.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(window.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(window.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(window.DataContext), "DataContext on TabControl should match.");
                }
            }
        }

        [Test]
        public void DependencyProperties_and_DataContext_should_be_inherited_from_ribbon()
        {
            var ribbon = new Ribbon
                         {
                             Menu = new Backstage(),
                             StartScreen = new StartScreen()
                         };

            var enUs = XmlLanguage.GetLanguage("en-US");
            var deDe = XmlLanguage.GetLanguage("de-DE");

            using (var window = new TestRibbonWindow(ribbon)
                                {
                                    Language = deDe,
                                    DataContext = deDe
                                })
            {
                ribbon.ApplyTemplate();

                {
                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(ribbon.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(ribbon.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(ribbon.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(ribbon.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(ribbon.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on TabControl should match.");
                }

                {
                    ribbon.Language = enUs;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(ribbon.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(ribbon.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(ribbon.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(ribbon.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(ribbon.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on TabControl should match.");
                }

                {
                    ribbon.Language = deDe;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(ribbon.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(ribbon.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(ribbon.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(ribbon.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(ribbon.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on TabControl should match.");
                }

                {
                    ribbon.Language = enUs;
                    ribbon.DataContext = ribbon.Language;

                    Assert.That(window.Language, Is.EqualTo(window.Language), "Language on Window should match.");
                    Assert.That(ribbon.Language, Is.EqualTo(ribbon.Language), "Language on Ribbon should match.");
                    Assert.That(ribbon.Menu.Language, Is.EqualTo(ribbon.Language), "Language on Menu should match.");
                    Assert.That(ribbon.StartScreen.Language, Is.EqualTo(ribbon.Language), "Language on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.Language, Is.EqualTo(ribbon.Language), "Language on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.Language, Is.EqualTo(ribbon.Language), "Language on TabControl should match.");

                    Assert.That(window.DataContext, Is.EqualTo(window.DataContext), "DataContext on Window should match.");
                    Assert.That(ribbon.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Ribbon should match.");
                    Assert.That(ribbon.Menu.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on Menu should match.");
                    Assert.That(ribbon.StartScreen.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on StartScreen should match.");
                    Assert.That(ribbon.QuickAccessToolBar.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on QuickAccessToolBar should match.");
                    Assert.That(ribbon.TabControl.DataContext, Is.EqualTo(ribbon.DataContext), "DataContext on TabControl should match.");
                }
            }
        }
    }
}