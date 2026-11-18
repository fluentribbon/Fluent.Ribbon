namespace Fluent.Tests.Controls;

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Fluent.Tests.Helper;
using Fluent.Tests.TestClasses;
using NUnit.Framework;

[TestFixture]
public class BackstageReopenDuringCloseTests
{
    /// <summary>
    /// Closing the backstage with animations enabled starts a close animation and hides the backstage
    /// (and gives the ribbon back its normal state) when that animation has completed.
    /// When the user opens the backstage again while the close animation is still running
    /// (for example a quick double click on the File button), the backstage must stay open and visible:
    /// the "completed" step of the old, replaced close animation must not hide it afterwards.
    /// </summary>
    [Test]
    public void Reopening_during_the_close_animation_keeps_the_backstage_displayed()
    {
        var backstage = new Backstage { Content = new System.Windows.Controls.Button { Content = "Inside" }, AreAnimationsEnabled = true };
        var ribbon = new Ribbon { Menu = backstage };

        using (new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            // A normal animated open and close, to measure how long a close takes in this environment.
            backstage.IsOpen = true;
            PumpDispatcherFor(TimeSpan.FromSeconds(1));
            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Visible), "Precondition: the backstage is displayed after opening");

            backstage.IsOpen = false;
            var closeDuration = PumpDispatcherUntil(() => GetAdorner(backstage).Visibility == Visibility.Collapsed, TimeSpan.FromSeconds(10));

            if (closeDuration is null)
            {
                Assert.Inconclusive("The close animation did not complete within 10 seconds: animations don't run in this test environment.");
            }

            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "Precondition: a normal close restores the ribbon");

            backstage.IsOpen = true;
            PumpDispatcherFor(TimeSpan.FromSeconds(1));
            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Visible), "Precondition: the backstage is displayed after opening again");

            // Close and open again right away, while the close animation is still running.
            backstage.IsOpen = false;
            UIHelper.DoEvents();

            if (GetAdorner(backstage).Visibility != Visibility.Visible)
            {
                Assert.Inconclusive("The close animation already completed before the backstage could be reopened.");
            }

            backstage.IsOpen = true;

            // Let much more time pass than the old close animation needs (and than the open animation needs).
            var wait = TimeSpan.FromTicks(Math.Max(TimeSpan.FromSeconds(2).Ticks, closeDuration.Value.Ticks * 4));
            PumpDispatcherFor(wait);

            Assert.That(backstage.IsOpen, Is.True);
            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Visible), "The reopened backstage must stay displayed after the old close animation has ended");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "The ribbon must stay in backstage mode while the backstage is open");

            // And a normal close still works afterwards.
            backstage.IsOpen = false;
            var finalCloseDuration = PumpDispatcherUntil(() => GetAdorner(backstage).Visibility == Visibility.Collapsed, TimeSpan.FromSeconds(10));

            Assert.That(finalCloseDuration, Is.Not.Null, "A normal close after the reopen must hide the backstage");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "A normal close after the reopen must restore the ribbon");
        }
    }

    /// <summary>
    /// The backstage is removed (for example from <see cref="Ribbon.Menu"/>) while its close animation is running.
    /// The ribbon must leave backstage mode right away: the animation of a removed backstage may never raise Completed
    /// (WPF only holds its clock weakly once nothing animates with it), so the end of the animation can't be relied on.
    /// </summary>
    [Test]
    public void Removing_the_backstage_during_the_close_animation_restores_the_ribbon()
    {
        var backstage = new Backstage { Content = new System.Windows.Controls.Button { Content = "Inside" }, AreAnimationsEnabled = true };
        var ribbon = new Ribbon { Menu = backstage };

        using (new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            var firstOpen = UseOpenStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = true;
            WaitUntilCompleted(firstOpen, "first open");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "Precondition: the ribbon is in backstage mode while the backstage is open");

            // Long enough to still be running when the backstage has been removed (that takes a fraction of a second).
            var close = UseCloseStoryboard(backstage, TimeSpan.FromSeconds(5));
            backstage.IsOpen = false;

            ribbon.Menu = null;
            UIHelper.DoEvents();

            Assert.That(backstage.IsLoaded, Is.False, "Precondition: the backstage was unloaded");
            Assert.That(close.IsCompleted, Is.False, "Precondition: the close animation is still running");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "The ribbon must leave backstage mode when the closing backstage is removed");
            Assert.That(ribbon.TabControl.HighlightSelectedItem, Is.True, "The selected tab must be highlighted again when the closing backstage is removed");

            // Let the abandoned close animation end too, so it isn't still running during other tests.
            WaitUntilCompleted(close, "abandoned close");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "The end of the abandoned close animation changes nothing");

            // Put back, the backstage still opens and closes normally.
            ribbon.Menu = backstage;
            UIHelper.DoEvents();

            var reopen = UseOpenStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = true;
            WaitUntilCompleted(reopen, "open after putting the backstage back");

            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Visible), "The backstage put back is displayed when opened");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "The ribbon is in backstage mode again");

            var finalClose = UseCloseStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = false;
            WaitUntilCompleted(finalClose, "final close");

            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Collapsed), "The final close hides the backstage");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "The final close restores the ribbon");
        }
    }

    /// <summary>
    /// The backstage is moved to another window while its close animation is running and opened there.
    /// The end of the first close is skipped (it was shown again), so the first window must have been let go
    /// when the backstage left it: Esc there must not close the backstage that is open in the second window.
    /// </summary>
    [Test]
    public void Reopening_in_another_window_during_the_close_animation_stops_listening_to_the_first_window()
    {
        var backstage = new Backstage { Content = new System.Windows.Controls.Button { Content = "Inside" }, AreAnimationsEnabled = true };

        using (var firstWindow = new TestRibbonWindow(backstage))
        using (var secondWindow = new TestRibbonWindow())
        {
            UIHelper.DoEvents();

            var firstOpen = UseOpenStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = true;
            WaitUntilCompleted(firstOpen, "first open");

            // Long enough to still be running when Esc is pressed below (the steps until then take a fraction of a second).
            var close = UseCloseStoryboard(backstage, TimeSpan.FromSeconds(5));
            backstage.IsOpen = false;

            firstWindow.Content = null;
            secondWindow.Content = backstage;
            UIHelper.DoEvents();

            var reopen = UseOpenStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = true;
            WaitUntilCompleted(reopen, "reopen in the second window");

            Assert.That(close.IsCompleted, Is.False, "Precondition: the close animation is still running");
            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Visible), "Precondition: the backstage is displayed in the second window");

            PressEscape(firstWindow);

            Assert.That(backstage.IsOpen, Is.True, "Esc in the first window must not close the backstage that is open in the second window");

            var finalClose = UseCloseStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            PressEscape(secondWindow);

            Assert.That(backstage.IsOpen, Is.False, "Esc in the window that shows the backstage closes it");

            WaitUntilCompleted(finalClose, "final close");
            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Collapsed), "The final close hides the backstage");

            // Let the abandoned close animation end too, so it isn't still running during other tests.
            WaitUntilCompleted(close, "abandoned close");
            Assert.That(GetAdorner(backstage).Visibility, Is.EqualTo(Visibility.Collapsed), "The end of the abandoned close animation changes nothing");
        }
    }

    /// <summary>
    /// The end of a close animation gives the keyboard focus back to the backstage button.
    /// When the backstage was moved to another window during that animation, the move already restored everything,
    /// and the user may be working in the other window by the time the old animation ends:
    /// it must not take the keyboard focus there.
    /// </summary>
    [Test]
    public void Moving_to_another_window_during_the_close_animation_leaves_the_focus_alone_there()
    {
        var backstage = new Backstage { Content = new System.Windows.Controls.Button { Content = "Inside" }, AreAnimationsEnabled = true };
        var otherButton = new System.Windows.Controls.Button { Content = "Other" };
        var secondWindowContent = new System.Windows.Controls.StackPanel();
        secondWindowContent.Children.Add(otherButton);

        using (var firstWindow = new TestRibbonWindow(backstage))
        using (var secondWindow = new TestRibbonWindow(secondWindowContent))
        {
            UIHelper.DoEvents();

            var open = UseOpenStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = true;
            WaitUntilCompleted(open, "open");

            // Long enough to still be running after the move and the focus change below (they take a fraction of a second).
            var close = UseCloseStoryboard(backstage, TimeSpan.FromSeconds(3));
            backstage.IsOpen = false;

            firstWindow.Content = null;
            secondWindowContent.Children.Insert(0, backstage);
            UIHelper.DoEvents();

            secondWindow.Activate();
            otherButton.Focus();
            UIHelper.DoEvents();
            if (otherButton.IsKeyboardFocused == false)
            {
                Assert.Inconclusive("Keyboard focus is not available in this test environment.");
            }

            Assert.That(close.IsCompleted, Is.False, "Precondition: the close animation is still running");

            WaitUntilCompleted(close, "abandoned close");

            UIHelper.InconclusiveIfKeyboardFocusLost("after the close animation ended");
            Assert.That(Keyboard.FocusedElement, Is.SameAs(otherButton), "The end of the old close animation must not move the focus to the moved backstage");
        }
    }

    /// <summary>
    /// A new template (for example from an app style) also replaces the adorner, without an unload.
    /// When that happens during the close animation, the ribbon must get its normal state back right away, like when the
    /// backstage is removed: the animation of the destroyed adorner may never end (WPF holds its clock only weakly).
    /// The end of the animation must not change anything after that.
    /// </summary>
    [Test]
    public void New_template_during_the_close_animation_restores_the_ribbon_right_away()
    {
        var backstage = new Backstage { Content = new System.Windows.Controls.Button { Content = "Inside" }, AreAnimationsEnabled = true };
        var ribbon = new Ribbon { Menu = backstage };

        using (new TestRibbonWindow(ribbon))
        {
            UIHelper.DoEvents();

            var open = UseOpenStoryboard(backstage, TimeSpan.FromMilliseconds(10));
            backstage.IsOpen = true;
            WaitUntilCompleted(open, "open");
            Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.True, "Precondition: the ribbon is in backstage mode while the backstage is open");

            // Long enough to still be running after the new template (that takes a fraction of a second).
            var close = UseCloseStoryboard(backstage, TimeSpan.FromSeconds(3));
            backstage.IsOpen = false;

            backstage.Template = new System.Windows.Controls.ControlTemplate(typeof(Backstage))
            {
                VisualTree = new FrameworkElementFactory(typeof(System.Windows.Controls.Border))
            };
            backstage.ApplyTemplate();
            UIHelper.DoEvents();

            Assert.That(close.IsCompleted, Is.False, "Precondition: the close animation is still running");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "The ribbon must leave backstage mode when the closing backstage gets a new template");
                Assert.That(ribbon.TabControl.HighlightSelectedItem, Is.True, "The selected tab must be highlighted again when the closing backstage gets a new template");
            }

            WaitUntilCompleted(close, "close");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(ribbon.IsBackstageOrStartScreenOpen, Is.False, "The end of the close animation changes nothing");
                Assert.That(ribbon.TabControl.HighlightSelectedItem, Is.True, "The end of the close animation changes nothing");
            }
        }
    }

    private static UIElement GetAdorner(Backstage backstage)
    {
        return backstage.GetFieldValue<UIElement>("adorner");
    }

    private static void PressEscape(Window window)
    {
        var args = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(window), 0, Key.Escape)
        {
            RoutedEvent = Keyboard.KeyDownEvent
        };

        window.RaiseEvent(args);
    }

    private static StoryboardRun UseOpenStoryboard(Backstage backstage, TimeSpan duration)
    {
        return UseStoryboard(backstage, "Fluent.Ribbon.Storyboards.Backstage.IsOpenTrueStoryboard", new Thickness(-125, 0, 0, 0), new Thickness(0), duration);
    }

    private static StoryboardRun UseCloseStoryboard(Backstage backstage, TimeSpan duration)
    {
        return UseStoryboard(backstage, "Fluent.Ribbon.Storyboards.Backstage.IsOpenFalseStoryboard", new Thickness(0), new Thickness(-125, 0, 0, 0), duration);
    }

    // Same kind of animation as the default storyboards, with the given duration, put into the backstage's resources
    // where it looks them up. The backstage clones the storyboard it finds and begins the clone. A clone doesn't copy
    // event handlers, but it is created by the storyboard's CreateInstanceCore, so the clone of a RecordingStoryboard
    // reports to the same run, and the returned run learns when the backstage's copy has raised Completed.
    // A 10 ms animation completes before a longer one that was started at the same time or later, however slow the
    // machine is, so the order of the steps is fixed without waiting for a fixed time.
    private static StoryboardRun UseStoryboard(Backstage backstage, string key, Thickness from, Thickness to, TimeSpan duration)
    {
        var animation = new ThicknessAnimation(from, to, new Duration(duration));
        Storyboard.SetTargetProperty(animation, new PropertyPath(FrameworkElement.MarginProperty));

        var run = new StoryboardRun();

        var storyboard = new RecordingStoryboard(run);
        storyboard.Children.Add(animation);

        backstage.Resources[key] = storyboard;

        return run;
    }

    // Pumps the dispatcher (animation frames are rendered by it) until the storyboard has completed.
    private static void WaitUntilCompleted(StoryboardRun run, string step)
    {
        var waitTimeout = TimeSpan.FromSeconds(30);

        if (run.IsCompleted == false)
        {
            var frame = new DispatcherFrame();
            run.Frame = frame;

            var timeoutTimer = new DispatcherTimer(waitTimeout, DispatcherPriority.Normal, (_, _) => frame.Continue = false, Dispatcher.CurrentDispatcher);

            // Keeps animation frames coming, even when nothing else is invalidated.
            EventHandler keepRendering = (_, _) => { };
            CompositionTarget.Rendering += keepRendering;

            try
            {
                Dispatcher.PushFrame(frame);
            }
            finally
            {
                CompositionTarget.Rendering -= keepRendering;
                timeoutTimer.Stop();
                run.Frame = null;
            }
        }

        Assert.That(run.IsCompleted, Is.True, $"The {step} animation did not complete within {waitTimeout.TotalSeconds} seconds.");

        UIHelper.DoEvents();
    }

    private sealed class RecordingStoryboard : Storyboard
    {
        private readonly StoryboardRun run;

        public RecordingStoryboard(StoryboardRun run)
        {
            this.run = run;
            this.Completed += run.HandleCompleted;
        }

        protected override Freezable CreateInstanceCore()
        {
            return new RecordingStoryboard(this.run);
        }

        // WPF holds the clocks of a running storyboard only weakly once nothing animates with them any more
        // (for example after another storyboard replaced the animation, or after the animated adorner was destroyed),
        // so a garbage collection can end them without Completed ever being raised.
        // The run keeps the clock alive, so the test always gets to the moment the abandoned animation ends.
        protected override Clock AllocateClock()
        {
            var clock = base.AllocateClock();
            this.run.Clock = clock;
            return clock;
        }
    }

    private sealed class StoryboardRun
    {
        public bool IsCompleted { get; private set; }

        public DispatcherFrame Frame { get; set; }

        public Clock Clock { get; set; }

        public void HandleCompleted(object sender, EventArgs e)
        {
            this.IsCompleted = true;

            if (this.Frame is not null)
            {
                this.Frame.Continue = false;
            }
        }
    }

    // WPF animation clocks run in real time, so the dispatcher has to keep running while time passes.
    private static void PumpDispatcherFor(TimeSpan duration)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < duration)
        {
            UIHelper.DoEvents();
            Thread.Sleep(10);
        }

        UIHelper.DoEvents();
    }

    // Returns how long it took until the condition was met, or null when the timeout was reached first.
    private static TimeSpan? PumpDispatcherUntil(Func<bool> condition, TimeSpan timeout)
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            UIHelper.DoEvents();

            if (condition())
            {
                return stopwatch.Elapsed;
            }

            Thread.Sleep(10);
        }

        return null;
    }
}
