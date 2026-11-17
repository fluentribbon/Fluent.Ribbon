namespace Fluent.Tests.Automation.Peers;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// A ribbon control without a text Header (an icon-only button, common in small groups and on the Quick Access Toolbar)
/// had no name for screen readers, even when its <see cref="ScreenTip"/> carries a title like "Bold".
/// The title is the best name the control has, so the automation peers fall back to it.
/// </summary>
[TestFixture]
public class NameFromScreenTipTitleTests
{
    // Types (not instances) because WPF controls must be created on the STA test thread, not during test discovery.
    private static IEnumerable<Type> ControlTypes()
    {
        yield return typeof(Button);
        yield return typeof(ToggleButton);
        yield return typeof(CheckBox);
        yield return typeof(RadioButton);
        yield return typeof(DropDownButton);
        yield return typeof(SplitButton);
        yield return typeof(Spinner);
        yield return typeof(ComboBox);
        yield return typeof(TextBox);
        yield return typeof(InRibbonGallery);
    }

    [Test]
    [TestCaseSource(nameof(ControlTypes))]
    public void Control_without_Header_is_named_by_its_ScreenTip_title(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        control.ToolTip = new ScreenTip { Title = "Bold", Text = "Makes the text bold" };

        var peer = UIElementAutomationPeer.CreatePeerForElement(control);
        Assert.That(peer, Is.Not.Null, "Precondition: the control creates an automation peer");

        Assert.That(peer.GetName(), Is.EqualTo("Bold"), "Without a header the ScreenTip title must be the name");
        Assert.That(peer.GetHelpText(), Is.EqualTo("Makes the text bold"), "The ScreenTip text stays the help text");
    }

    [Test]
    [TestCaseSource(nameof(ControlTypes))]
    public void Control_with_a_Header_that_is_not_text_is_named_by_its_ScreenTip_title(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        ((IHeaderedControl)control).Header = new System.Windows.Shapes.Rectangle();
        control.ToolTip = new ScreenTip { Title = "Bold" };

        var peer = UIElementAutomationPeer.CreatePeerForElement(control);

        Assert.That(peer.GetName(), Is.EqualTo("Bold"), "A header that is not text can't be announced, so the ScreenTip title must be the name");
    }

    [Test]
    [TestCaseSource(nameof(ControlTypes))]
    public void Control_with_an_empty_Header_is_named_by_its_ScreenTip_title(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        ((IHeaderedControl)control).Header = string.Empty;
        control.ToolTip = new ScreenTip { Title = "Bold" };

        var peer = UIElementAutomationPeer.CreatePeerForElement(control);

        Assert.That(peer.GetName(), Is.EqualTo("Bold"), "An empty header is no name, so the ScreenTip title must be the name");
    }

    // Guard: a text header stays the name.
    [Test]
    [TestCaseSource(nameof(ControlTypes))]
    public void Header_text_wins_over_the_ScreenTip_title(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        ((IHeaderedControl)control).Header = "Header";
        control.ToolTip = new ScreenTip { Title = "Title" };

        var peer = UIElementAutomationPeer.CreatePeerForElement(control);

        Assert.That(peer.GetName(), Is.EqualTo("Header"));
    }

    // Guard: an explicit name from the app stays the name.
    [Test]
    [TestCaseSource(nameof(ControlTypes))]
    public void Explicit_AutomationProperties_Name_wins_over_the_ScreenTip_title(Type controlType)
    {
        var control = (FrameworkElement)Activator.CreateInstance(controlType)!;
        AutomationProperties.SetName(control, "Explicit");
        control.ToolTip = new ScreenTip { Title = "Title" };

        var peer = UIElementAutomationPeer.CreatePeerForElement(control);

        Assert.That(peer.GetName(), Is.EqualTo("Explicit"));
    }

    // Guard: a ScreenTip without a title, or a plain text ToolTip (which already is the help text), adds no name.
    [Test]
    [TestCaseSource(nameof(ControlTypes))]
    public void ScreenTip_without_title_and_plain_ToolTip_add_no_name(Type controlType)
    {
        var withEmptyTitle = (FrameworkElement)Activator.CreateInstance(controlType)!;
        withEmptyTitle.ToolTip = new ScreenTip { Text = "Only text" };

        var withPlainToolTip = (FrameworkElement)Activator.CreateInstance(controlType)!;
        withPlainToolTip.ToolTip = "Plain";

        Assert.That(UIElementAutomationPeer.CreatePeerForElement(withEmptyTitle).GetName(), Is.Null.Or.Empty);
        Assert.That(UIElementAutomationPeer.CreatePeerForElement(withPlainToolTip).GetName(), Is.Null.Or.Empty);
    }

    [Test]
    public void Name_follows_a_changed_ScreenTip_title()
    {
        var screenTip = new ScreenTip { Title = "Bold" };
        var button = new Button { ToolTip = screenTip };
        var peer = UIElementAutomationPeer.CreatePeerForElement(button);

        Assert.That(peer.GetName(), Is.EqualTo("Bold"));

        screenTip.Title = "Fett";

        Assert.That(peer.GetName(), Is.EqualTo("Fett"), "The name must follow the title (for example after a language change)");
    }

    // A split button without a header was already named by its command's text. That name must stay:
    // the ScreenTip title (often longer, like "Paste (Ctrl+V)") only comes after it.
    [Test]
    public void SplitButton_command_text_wins_over_the_ScreenTip_title()
    {
        var splitButton = new SplitButton
        {
            Command = System.Windows.Input.ApplicationCommands.Paste,
            ToolTip = new ScreenTip { Title = "Paste (Ctrl+V)" }
        };

        var peer = UIElementAutomationPeer.CreatePeerForElement(splitButton);

        Assert.That(peer.GetName(), Is.EqualTo(System.Windows.Input.ApplicationCommands.Paste.Text), "The command text was the name before and must stay the name");
    }

    // Guard: with its template applied, the split button's inner button is named by the command text too (WPF finds it itself),
    // so both parts agree.
    [Test]
    public void SplitButton_inner_button_is_named_by_the_command_text_too()
    {
        var splitButton = new SplitButton
        {
            Command = System.Windows.Input.ApplicationCommands.Paste,
            ToolTip = new ScreenTip { Title = "Paste (Ctrl+V)" }
        };

        using (new TestRibbonWindow(splitButton))
        {
            splitButton.ApplyTemplate();
            UIHelper.DoEvents();

            var innerButton = splitButton.Template.FindName("PART_Button", splitButton) as System.Windows.Controls.Primitives.ButtonBase;
            Assert.That(innerButton, Is.Not.Null, "Precondition: the template has PART_Button");

            Assert.That(UIElementAutomationPeer.CreatePeerForElement(splitButton).GetName(), Is.EqualTo(System.Windows.Input.ApplicationCommands.Paste.Text));
            Assert.That(UIElementAutomationPeer.CreatePeerForElement(innerButton).GetName(), Is.EqualTo(System.Windows.Input.ApplicationCommands.Paste.Text));
        }
    }

    // A Spinner takes no focus itself; its text box does, so the text box needs the same name.
    [Test]
    public void Spinner_text_box_is_named_by_the_ScreenTip_title_of_the_spinner()
    {
        var spinner = new Spinner { ToolTip = new ScreenTip { Title = "Font size" } };

        using (new TestRibbonWindow(spinner))
        {
            spinner.ApplyTemplate();
            UIHelper.DoEvents();

            var textBox = spinner.Template.FindName("PART_TextBox", spinner) as System.Windows.Controls.TextBox;
            Assert.That(textBox, Is.Not.Null, "Precondition: the template has PART_TextBox");

            var peer = UIElementAutomationPeer.CreatePeerForElement(textBox);
            Assert.That(peer.GetName(), Is.EqualTo("Font size"), "The focused text box must be announced with the spinner's ScreenTip title");

            spinner.Header = "Size";
            UIHelper.DoEvents();
            Assert.That(peer.GetName(), Is.EqualTo("Size"), "A text header wins over the ScreenTip title");

            System.Windows.Automation.AutomationProperties.SetName(spinner, "Explicit");
            UIHelper.DoEvents();
            Assert.That(peer.GetName(), Is.EqualTo("Explicit"), "An explicit name wins over both");
        }
    }

    // The spinner's text box must not keep an old title: it follows a changed title (for example after a language change)
    // and a replaced or removed ScreenTip, like the spinner itself does.
    [Test]
    public void Spinner_text_box_name_follows_the_ScreenTip()
    {
        var screenTip = new ScreenTip { Title = "Font size" };
        var spinner = new Spinner { ToolTip = screenTip };

        using (new TestRibbonWindow(spinner))
        {
            spinner.ApplyTemplate();
            UIHelper.DoEvents();

            var textBox = spinner.Template.FindName("PART_TextBox", spinner) as System.Windows.Controls.TextBox;
            Assert.That(textBox, Is.Not.Null, "Precondition: the template has PART_TextBox");

            var peer = UIElementAutomationPeer.CreatePeerForElement(textBox);
            Assert.That(peer.GetName(), Is.EqualTo("Font size"), "Precondition: the text box is named by the title");

            screenTip.Title = "Schriftgrad";
            UIHelper.DoEvents();
            Assert.That(peer.GetName(), Is.EqualTo("Schriftgrad"), "The text box name must follow a changed title");

            spinner.ToolTip = new ScreenTip { Title = "Size" };
            UIHelper.DoEvents();
            Assert.That(peer.GetName(), Is.EqualTo("Size"), "The text box name must follow a replaced ScreenTip");

            screenTip.Title = "Old";
            UIHelper.DoEvents();
            Assert.That(peer.GetName(), Is.EqualTo("Size"), "A ScreenTip that is no longer the spinner's must not name the text box");

            spinner.ToolTip = "Plain text";
            UIHelper.DoEvents();
            Assert.That(peer.GetName(), Is.Null.Or.Empty, "A plain text ToolTip is the help text, not a name");
        }
    }

    [Test]
    public void Group_launcher_button_is_named_by_the_title_of_its_ScreenTip()
    {
        var groupBox = new RibbonGroupBox
        {
            Header = "Font",
            IsLauncherVisible = true,
            LauncherToolTip = new ScreenTip { Title = "Font settings", Text = "Shows all font options" }
        };

        using (new TestRibbonWindow(groupBox))
        {
            UIHelper.DoEvents();

            Assert.That(groupBox.LauncherButton, Is.Not.Null, "Precondition: the group has its launcher button");
            Assert.That(groupBox.LauncherButton.ToolTip, Is.InstanceOf<ScreenTip>(), "Precondition: the launcher button shows the ScreenTip");

            var peer = UIElementAutomationPeer.CreatePeerForElement(groupBox.LauncherButton);

            Assert.That(peer.GetName(), Is.EqualTo("Font settings"), "Without LauncherText the ScreenTip title must be the launcher's name");
        }
    }

    // Guard: LauncherText stays the launcher's name.
    [Test]
    public void Group_launcher_button_keeps_LauncherText_as_its_name()
    {
        var groupBox = new RibbonGroupBox
        {
            Header = "Font",
            IsLauncherVisible = true,
            LauncherText = "Font dialog",
            LauncherToolTip = new ScreenTip { Title = "Font settings" }
        };

        using (new TestRibbonWindow(groupBox))
        {
            UIHelper.DoEvents();

            Assert.That(groupBox.LauncherButton, Is.Not.Null, "Precondition: the group has its launcher button");

            var peer = UIElementAutomationPeer.CreatePeerForElement(groupBox.LauncherButton);

            Assert.That(peer.GetName(), Is.EqualTo("Font dialog"));
        }
    }
}
