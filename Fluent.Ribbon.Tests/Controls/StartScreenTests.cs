namespace Fluent.Tests.Controls;

using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

[TestFixture]
public class StartScreenTests
{
    [Test(Description = "Closing a StartScreen that was not displayed (because it was already shown once) must not overwrite RibbonTitleBar.IsCollapsed")]
    public void TitleBar_IsCollapsed_is_only_restored_once_after_the_StartScreen_was_displayed()
    {
        var titleBar = new RibbonTitleBar();
        var startScreen = new StartScreen
        {
            Content = new StartScreenTabControl()
        };
        var ribbon = new Ribbon
        {
            TitleBar = titleBar,
            StartScreen = startScreen
        };

        using (new TestRibbonWindow(ribbon))
        {
            Assert.That(titleBar.IsCollapsed, Is.False, "Precondition: the title bar starts expanded.");

            // First open/close: the start screen is really displayed, collapses the title bar and restores it on close.
            startScreen.IsOpen = true;

            Assert.That(startScreen.Shown, Is.True, "Precondition: the first open must display the start screen.");
            Assert.That(titleBar.IsCollapsed, Is.True, "Precondition: displaying the start screen collapses the title bar.");

            startScreen.IsOpen = false;

            Assert.That(titleBar.IsCollapsed, Is.False, "Precondition: closing the start screen restores the title bar.");

            // Later the application collapses the title bar on its own.
            titleBar.IsCollapsed = true;

            // Second open/close: StartScreen.Show returns early because Shown is already true, so nothing is displayed
            // and the title bar was never touched. Closing must therefore not write an old saved value back.
            startScreen.IsOpen = true;
            startScreen.IsOpen = false;

            Assert.That(titleBar.IsCollapsed, Is.True, "Closing a start screen that was not displayed must not restore an outdated RibbonTitleBar.IsCollapsed value.");
        }
    }

    /// <summary>
    /// A displayed start screen that is unloaded (for example its ribbon is taken out of the window and put back) stays open.
    /// Unloading hides it and gives the ribbon its normal state back; loading it again must display it again,
    /// like a backstage, even though it was already shown once. Before, nothing was displayed while IsOpen stayed true,
    /// and the title bar stayed collapsed.
    /// </summary>
    [Test]
    public void Displayed_StartScreen_is_displayed_again_when_it_is_loaded_again()
    {
        var titleBar = new RibbonTitleBar();
        var startScreen = new StartScreen
        {
            Content = new StartScreenTabControl(),
            AreAnimationsEnabled = false
        };
        var ribbon = new Ribbon
        {
            TitleBar = titleBar,
            StartScreen = startScreen,
            AutomaticStateManagement = false
        };

        using (var window = new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            startScreen.IsOpen = true;
            UIHelper.DoEvents();

            Assert.That(startScreen.Shown, Is.True, "Precondition: the start screen is displayed.");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "Precondition: the ribbon is in start screen mode.");
            Assert.That(titleBar.IsCollapsed, Is.True, "Precondition: displaying the start screen collapses the title bar.");

            window.Content = null;
            UIHelper.DoEvents();

            Assert.That(startScreen.IsLoaded, Is.False, "Precondition: the start screen was unloaded.");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "Precondition: unloading gives the ribbon its normal state back.");
            Assert.That(titleBar.IsCollapsed, Is.False, "Unloading the displayed start screen gives the title bar back.");

            window.Content = ribbon;
            UIHelper.DoEvents();

            // An open backstage that is loaded shows itself one dispatcher pass later (Backstage.OnDelayedShow).
            UIHelper.DoEvents();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(startScreen.IsOpen, Is.True, "The start screen is still open.");
                Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "The open start screen must be displayed again when it is loaded again.");
                Assert.That(titleBar.IsCollapsed, Is.True, "The displayed start screen collapses the title bar.");
            }

            startScreen.IsOpen = false;
            UIHelper.DoEvents();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "Closing it gives the ribbon its normal state back.");
                Assert.That(titleBar.IsCollapsed, Is.False, "Closing it restores the title bar as it was before the start screen was first displayed.");
            }

            // It was shown once: opening it again still displays nothing.
            startScreen.IsOpen = true;
            UIHelper.DoEvents();

            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "A start screen that was closed after it was shown is not displayed again.");
            startScreen.IsOpen = false;
        }
    }

    /// <summary>
    /// A displayed start screen closed while it's unloaded is not displayed again when it's loaded again, nor when it's opened again.
    /// </summary>
    [Test]
    public void StartScreen_closed_while_unloaded_is_not_displayed_again()
    {
        var titleBar = new RibbonTitleBar();
        var startScreen = new StartScreen
        {
            Content = new StartScreenTabControl(),
            AreAnimationsEnabled = false
        };
        var ribbon = new Ribbon
        {
            TitleBar = titleBar,
            StartScreen = startScreen,
            AutomaticStateManagement = false
        };

        using (var window = new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            startScreen.IsOpen = true;
            UIHelper.DoEvents();
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "Precondition: the start screen is displayed.");

            window.Content = null;
            UIHelper.DoEvents();

            startScreen.IsOpen = false;

            window.Content = ribbon;
            UIHelper.DoEvents();
            UIHelper.DoEvents();

            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "A start screen closed while unloaded is not displayed when it's loaded again.");

            startScreen.IsOpen = true;
            UIHelper.DoEvents();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "It was shown once: opening it again displays nothing.");
                Assert.That(titleBar.IsCollapsed, Is.False, "The title bar stays as it was.");
            }

            startScreen.IsOpen = false;
        }
    }

    /// <summary>
    /// A start screen hidden through its Visibility collapses or expands the title bar (#445), but only while it's displayed.
    /// One that was shown once and opened again displays nothing, so changing its Visibility must not collapse the title bar:
    /// nothing would give it back.
    /// </summary>
    [Test]
    public void Visibility_of_a_StartScreen_that_isnt_displayed_leaves_the_title_bar_alone()
    {
        var titleBar = new RibbonTitleBar();
        var startScreen = new StartScreen
        {
            Content = new StartScreenTabControl(),
            AreAnimationsEnabled = false
        };
        var ribbon = new Ribbon
        {
            TitleBar = titleBar,
            StartScreen = startScreen,
            AutomaticStateManagement = false
        };

        using (new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            startScreen.IsOpen = true;
            startScreen.IsOpen = false;
            Assert.That(titleBar.IsCollapsed, Is.False, "Precondition: closing the start screen restores the title bar.");

            // Shown once: opening it again displays nothing.
            startScreen.IsOpen = true;
            UIHelper.DoEvents();
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "Precondition: nothing is displayed.");

            startScreen.Visibility = System.Windows.Visibility.Collapsed;
            startScreen.Visibility = System.Windows.Visibility.Visible;

            Assert.That(titleBar.IsCollapsed, Is.False, "A start screen that isn't displayed must not collapse the title bar.");

            startScreen.IsOpen = false;
            Assert.That(titleBar.IsCollapsed, Is.False, "The title bar stays as it was.");
        }
    }

    /// <summary>
    /// Guard for #445: a displayed start screen hidden through its Visibility (not closed) expands the title bar, and collapses it again when it's visible again.
    /// </summary>
    [Test]
    public void Visibility_of_a_displayed_StartScreen_collapses_and_expands_the_title_bar()
    {
        var titleBar = new RibbonTitleBar();
        var startScreen = new StartScreen
        {
            Content = new StartScreenTabControl(),
            AreAnimationsEnabled = false
        };
        var ribbon = new Ribbon
        {
            TitleBar = titleBar,
            StartScreen = startScreen,
            AutomaticStateManagement = false
        };

        using (new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            startScreen.IsOpen = true;
            Assert.That(titleBar.IsCollapsed, Is.True, "Precondition: the displayed start screen collapses the title bar.");

            startScreen.Visibility = System.Windows.Visibility.Collapsed;
            Assert.That(titleBar.IsCollapsed, Is.False, "Hiding the displayed start screen through its Visibility expands the title bar.");

            startScreen.Visibility = System.Windows.Visibility.Visible;
            Assert.That(titleBar.IsCollapsed, Is.True, "Making it visible again collapses the title bar again.");

            startScreen.IsOpen = false;
            Assert.That(titleBar.IsCollapsed, Is.False, "Closing it restores the title bar.");
        }
    }
}