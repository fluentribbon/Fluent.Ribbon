namespace Fluent.Tests.Controls;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

/// <summary>
/// Tests for <see cref="DropDownButton.DismissOnClickOutside"/>.
/// </summary>
/// <remarks>
/// An open drop down holds the mouse capture (<see cref="CaptureMode.SubTree"/>). While it does, WPF delivers a click
/// that lands outside the drop down button and its popup to the drop down button, and first raises
/// <see cref="Mouse.PreviewMouseDownOutsideCapturedElementEvent"/> on it. These tests raise that event the way WPF does.
/// Real mouse input can't be produced in the test run (the test window is off screen and not active),
/// so they check the capture state instead of a real click on the other button.
/// </remarks>
[TestFixture]
public class DropDownButtonDismissOnClickOutsideTests
{
    [Test]
    public void Outside_click_closes_the_drop_down_and_releases_capture_by_default()
    {
        var (dropDownButton, otherButton, panel) = CreateContent(dismissOnClickOutside: true);

        using (new TestRibbonWindow(panel))
        {
            Open(dropDownButton);

            Assert.That(Mouse.Captured, Is.SameAs(dropDownButton), "Precondition: the open drop down should hold the mouse capture");
            Assert.That(dropDownButton.IsAncestorOf(otherButton), Is.False, "Precondition: the other button must be outside the drop down button");

            // By default an outside click only closes the drop down when the real cursor is not over its popup
            // (PopupService checks the physical mouse position). The test can't move the cursor, so if it happens
            // to be over the popup, not closing would be correct and the test can't say anything.
            if (PopupService.IsMousePhysicallyOver(dropDownButton.DropDownPopup))
            {
                Assert.Inconclusive("The real mouse cursor is over the drop down's popup, so an outside click would rightly not close it.");
            }

            RaiseClickOutside(dropDownButton);

            Assert.That(dropDownButton.IsDropDownOpen, Is.False, "A click outside should close the drop down");
            Assert.That(Mouse.Captured, Is.Null, "Closing should release the mouse capture");
        }
    }

    [Test]
    public void Outside_click_keeps_the_drop_down_open_and_keeps_capture_when_DismissOnClickOutside_is_false()
    {
        var (dropDownButton, otherButton, panel) = CreateContent(dismissOnClickOutside: false);

        using (new TestRibbonWindow(panel))
        {
            Open(dropDownButton);

            Assert.That(Mouse.Captured, Is.SameAs(dropDownButton), "Precondition: the open drop down should hold the mouse capture");
            Assert.That(dropDownButton.IsAncestorOf(otherButton), Is.False, "Precondition: the other button must be outside the drop down button");

            RaiseClickOutside(dropDownButton);

            Assert.That(dropDownButton.IsDropDownOpen, Is.True, "A click outside must not close the drop down when DismissOnClickOutside is false");

            // This only documents the early return in PopupService for DismissOnClickOutside = false: nothing on this
            // path releases the capture, so the check can only fail if that return is removed. It does not show that
            // the capture survives in real use; losing it to another window or element is tested separately.
            // While the capture is held, further clicks elsewhere in the window keep going to the drop down (like a
            // dialog) until it is closed by its button or Escape. The application losing focus closes it only when the
            // window has a Ribbon (its KeyTipService closes popups when the window is deactivated); this test has none.
            Assert.That(Mouse.Captured, Is.SameAs(dropDownButton), "Documents the early return: the fake outside click must not release the capture");

            // Escape still closes it and gives the mouse back to the rest of the window.
            var args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(dropDownButton), 0, Key.Escape)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };

            dropDownButton.RaiseEvent(args);
            UIHelper.DoEvents();

            Assert.That(dropDownButton.IsDropDownOpen, Is.False, "Escape should close the drop down");
            Assert.That(Mouse.Captured, Is.Null, "Closing should release the mouse capture");
        }
    }

    /// <summary>
    /// The open drop down can lose the mouse capture without being closed by a click: Windows cancels it when another
    /// window takes it, and any element in the application can take it. WPF then raises LostMouseCapture on the
    /// drop down button, exactly as <see cref="Mouse.Capture(IInputElement)"/> does here.
    /// Without the capture WPF no longer sends outside clicks to it, so an open drop down would no longer be closed by them.
    /// So it closes, like WPF's ComboBox does when it loses the capture.
    /// With DismissOnClickOutside = false it stays open instead: see
    /// <see cref="Losing_mouse_capture_keeps_the_drop_down_open_when_DismissOnClickOutside_is_false"/>.
    /// A Ribbon in the window is tested too: its KeyTipService closes popups only when the window is deactivated,
    /// which losing the capture alone does not do.
    /// </summary>
    [Test]
    public void Losing_mouse_capture_closes_the_drop_down(
        [Values(false, true)] bool withRibbon,
        [Values(false, true)] bool takenByOtherElement)
    {
        var (dropDownButton, otherButton, panel) = CreateContent(dismissOnClickOutside: true);

        object content = panel;

        if (withRibbon)
        {
            var ribbon = new Ribbon
            {
                Tabs = { new RibbonTabItem { Header = "Home" } }
            };

            DockPanel.SetDock(ribbon, Dock.Top);

            content = new DockPanel
            {
                Children =
                {
                    ribbon,
                    panel
                }
            };
        }

        using (new TestRibbonWindow(content))
        {
            Open(dropDownButton);

            if (takenByOtherElement)
            {
                Assert.That(otherButton.CaptureMouse(), Is.True, "Precondition: the other button should be able to take the mouse capture");
            }
            else
            {
                // What WPF does when Windows cancels the capture because another window took it.
                Mouse.Capture(null);
            }

            UIHelper.DoEvents();

            Assert.That(dropDownButton.IsDropDownOpen, Is.False, "A drop down that lost the mouse capture must close, outside clicks no longer reach it");
            Assert.That(Mouse.Captured, Is.Not.SameAs(dropDownButton), "A closed drop down must not hold the mouse capture");

            if (takenByOtherElement)
            {
                Assert.That(Mouse.Captured, Is.SameAs(otherButton), "The element that took the capture must keep it");
            }
        }
    }

    /// <summary>
    /// DismissOnClickOutside = false was added (upstream #1048) for drop downs used like a dialog: they must not close
    /// "unless the user clicks over the DropDownButton". Losing the mouse capture is not such a click, so it stays open
    /// (it still closes by its button or Escape).
    /// </summary>
    [Test]
    public void Losing_mouse_capture_keeps_the_drop_down_open_when_DismissOnClickOutside_is_false(
        [Values(false, true)] bool takenByOtherElement)
    {
        var (dropDownButton, otherButton, panel) = CreateContent(dismissOnClickOutside: false);

        using (new TestRibbonWindow(panel))
        {
            Open(dropDownButton);

            if (takenByOtherElement)
            {
                Assert.That(otherButton.CaptureMouse(), Is.True, "Precondition: the other button should be able to take the mouse capture");
            }
            else
            {
                Mouse.Capture(null);
            }

            UIHelper.DoEvents();

            Assert.That(dropDownButton.IsDropDownOpen, Is.True, "With DismissOnClickOutside = false only the drop down's button or Escape closes it");

            if (takenByOtherElement)
            {
                Assert.That(Mouse.Captured, Is.SameAs(otherButton), "The element that took the capture must keep it");
            }
        }
    }

    /// <summary>
    /// The use case of DismissOnClickOutside = false (upstream #1048): the drop down holds a form, and the form opens a dialog
    /// (for example a message box). Windows cancels the capture when the dialog disables the window, so the drop down
    /// loses it, but it must still be open when the dialog shows and after it closes.
    /// </summary>
    [Test]
    public void Opening_a_dialog_from_the_drop_down_keeps_it_open_when_DismissOnClickOutside_is_false()
    {
        var (dropDownButton, _, panel) = CreateContent(dismissOnClickOutside: false);

        using (var window = new TestRibbonWindow(panel))
        {
            Open(dropDownButton);

            bool? openWhileTheDialogIsShown = null;
            var dialog = new Window
            {
                Width = 100,
                Height = 100,
                ShowInTaskbar = false,
                ShowActivated = false,
                Owner = window,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = window.Left,
                Top = window.Top
            };

            // Loaded always comes (ContentRendered doesn't for a window that's off screen and not activated).
            // The check waits until the dispatcher is idle, so the lost capture has been handled by then.
            dialog.Loaded += (_, _) => dialog.Dispatcher.BeginInvoke(
                System.Windows.Threading.DispatcherPriority.ApplicationIdle,
                new System.Action(() =>
                {
                    openWhileTheDialogIsShown = dropDownButton.IsDropDownOpen;
                    dialog.Close();
                }));

            var lostTheCapture = false;
            dropDownButton.LostMouseCapture += (_, _) => lostTheCapture = true;

            // Safety net only: if the dialog never loads (for example without a usable desktop), close it so the run can't hang.
            var dialogNeverLoaded = false;
            var failsafe = new System.Windows.Threading.DispatcherTimer(
                System.TimeSpan.FromSeconds(30),
                System.Windows.Threading.DispatcherPriority.Normal,
                (_, _) =>
                {
                    dialogNeverLoaded = openWhileTheDialogIsShown is null;
                    dialog.Close();
                },
                dialog.Dispatcher);

            try
            {
                dialog.ShowDialog();
            }
            finally
            {
                failsafe.Stop();
            }

            UIHelper.DoEvents();

            if (dialogNeverLoaded)
            {
                Assert.Inconclusive("The dialog never loaded in this test environment.");
            }

            // Without a lost capture the test would pass without checking anything.
            if (lostTheCapture == false)
            {
                Assert.Inconclusive("Showing the dialog didn't take the mouse capture away from the drop down in this test environment.");
            }

            Assert.That(openWhileTheDialogIsShown, Is.True, "The drop down must stay open while the dialog is shown");
            Assert.That(dropDownButton.IsDropDownOpen, Is.True, "The drop down must still be open after the dialog closed");
        }
    }

    /// <summary>
    /// Guard: an element inside the open drop down that takes the mouse capture (like a button does while it's pressed) doesn't close it.
    /// </summary>
    [TestCase(true)]
    [TestCase(false)]
    public void Capture_taken_inside_the_drop_down_keeps_it_open(bool dismissOnClickOutside)
    {
        var (dropDownButton, _, panel) = CreateContent(dismissOnClickOutside);
        var innerButton = new System.Windows.Controls.Button { Content = "Inner" };
        dropDownButton.Items.Add(innerButton);

        using (new TestRibbonWindow(panel))
        {
            Open(dropDownButton);

            Assert.That(innerButton.CaptureMouse(), Is.True, "Precondition: the inner button should be able to take the mouse capture");
            UIHelper.DoEvents();

            Assert.That(dropDownButton.IsDropDownOpen, Is.True, "Capture taken inside the drop down must not close it");

            innerButton.ReleaseMouseCapture();
            UIHelper.DoEvents();

            Assert.That(dropDownButton.IsDropDownOpen, Is.True, "Capture released inside the drop down must not close it");
            Assert.That(Mouse.Captured, Is.SameAs(dropDownButton), "The drop down takes the capture back when the inner element releases it");
        }
    }

    /// <summary>
    /// A drop down inside the drop down of another one loses the capture to the outer one
    /// (the outer one takes it back for example when the user clicks elsewhere in its drop down).
    /// The outer one holds the capture, so it stays open. The inner one closes, unless it has DismissOnClickOutside = false
    /// (for it, that click in the outer drop down is an outside click).
    /// </summary>
    [Test]
    public void Inner_drop_down_losing_the_capture_to_the_outer_one_leaves_the_outer_one_open(
        [Values(true, false)] bool dismissOnClickOutside)
    {
        var (outer, _, panel) = CreateContent(dismissOnClickOutside: true);
        var inner = new DropDownButton
        {
            Header = "Inner",
            DismissOnClickOutside = dismissOnClickOutside,
            Items = { new Fluent.MenuItem { Header = "Inner item" } }
        };
        outer.Items.Add(inner);

        using (new TestRibbonWindow(panel))
        {
            Open(outer);
            Open(inner);

            Mouse.Capture(outer, CaptureMode.SubTree);
            UIHelper.DoEvents();

            Assert.That(outer.IsDropDownOpen, Is.True, "The outer drop down holds the capture and must stay open");
            Assert.That(Mouse.Captured, Is.SameAs(outer), "The outer drop down must keep the capture");
            Assert.That(inner.IsDropDownOpen, Is.EqualTo(!dismissOnClickOutside), dismissOnClickOutside ? "The inner drop down lost the capture, so it closes" : "With DismissOnClickOutside = false the inner drop down stays open");
        }
    }

    /// <summary>
    /// The inner drop down loses the capture to another element in the outer drop down (that popup also shows the inner one):
    /// the inner one closes, the outer one stays open.
    /// </summary>
    [Test]
    public void Inner_drop_down_losing_the_capture_to_an_element_in_the_outer_drop_down_closes()
    {
        var (outer, _, panel) = CreateContent(dismissOnClickOutside: true);
        var inner = new DropDownButton
        {
            Header = "Inner",
            Items = { new Fluent.MenuItem { Header = "Inner item" } }
        };
        var otherInOuter = new System.Windows.Controls.Button { Content = "Other in outer" };
        outer.Items.Add(inner);
        outer.Items.Add(otherInOuter);

        using (new TestRibbonWindow(panel))
        {
            Open(outer);
            Open(inner);

            Assert.That(otherInOuter.CaptureMouse(), Is.True, "Precondition: the other element in the outer drop down should be able to take the mouse capture");
            UIHelper.DoEvents();

            Assert.That(inner.IsDropDownOpen, Is.False, "The inner drop down lost the capture, so it closes");
            Assert.That(outer.IsDropDownOpen, Is.True, "The outer drop down must stay open");
            Assert.That(Mouse.Captured, Is.SameAs(otherInOuter), "The element that took the capture must keep it");
        }
    }

    /// <summary>
    /// Guard: a popup that isn't part of the drop down (for example the calendar of a date picker in it, created in code)
    /// can take the capture without closing the drop down, the same exception outside clicks make for such popups.
    /// </summary>
    [Test]
    public void Capture_taken_by_another_popup_keeps_the_drop_down_open()
    {
        var (dropDownButton, _, panel) = CreateContent(dismissOnClickOutside: true);
        var popupButton = new System.Windows.Controls.Button { Content = "In popup" };
        var popup = new System.Windows.Controls.Primitives.Popup { Child = popupButton, Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse };

        using (new TestRibbonWindow(panel))
        {
            Open(dropDownButton);

            popup.IsOpen = true;
            UIHelper.DoEvents();

            try
            {
                Assert.That(popupButton.CaptureMouse(), Is.True, "Precondition: the button in the other popup should be able to take the mouse capture");
                UIHelper.DoEvents();

                Assert.That(dropDownButton.IsDropDownOpen, Is.True, "Another popup taking the capture must not close the drop down");
            }
            finally
            {
                popupButton.ReleaseMouseCapture();
                popup.IsOpen = false;
            }
        }
    }

    private static (DropDownButton DropDownButton, System.Windows.Controls.Button OtherButton, StackPanel Panel) CreateContent(bool dismissOnClickOutside)
    {
        var dropDownButton = new DropDownButton
        {
            Header = "DropDown",
            DismissOnClickOutside = dismissOnClickOutside,
            Items = { new Fluent.MenuItem { Header = "Item" } }
        };

        var otherButton = new System.Windows.Controls.Button
        {
            Content = "Other"
        };

        var panel = new StackPanel
        {
            Children =
            {
                dropDownButton,
                otherButton
            }
        };

        return (dropDownButton, otherButton, panel);
    }

    private static void Open(DropDownButton dropDownButton)
    {
        dropDownButton.ApplyTemplate();
        UIHelper.DoEvents();

        dropDownButton.IsDropDownOpen = true;
        UIHelper.DoEvents();

        Assert.That(dropDownButton.IsDropDownOpen, Is.True, "Precondition: the drop down should be open");

        // Windows only gives the mouse capture to a window that can take it (for example not while the screen is
        // locked or another desktop has the input). Without it these tests can't check anything.
        if (Mouse.Captured != dropDownButton)
        {
            Assert.Inconclusive("Windows did not give the test window the mouse capture in this test environment.");
        }
    }

    // What WPF raises on the captured element when the user presses a mouse button outside of it.
    private static void RaiseClickOutside(DropDownButton dropDownButton)
    {
        var args = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
        {
            RoutedEvent = Mouse.PreviewMouseDownOutsideCapturedElementEvent
        };

        dropDownButton.RaiseEvent(args);
        UIHelper.DoEvents();
    }
}
