// ReSharper disable once CheckNamespace
namespace Fluent;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Fluent.Extensions;
using Fluent.Internal;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents backstage button
/// </summary>
[ContentProperty(nameof(Content))]
public class Backstage : RibbonControl
{
    private static readonly object syncIsOpen = new();

    /// <summary>
    /// Occurs when IsOpen has been changed
    /// </summary>
    public event DependencyPropertyChangedEventHandler? IsOpenChanged;

    /// <summary>
    /// Occurs when the user is about to close the backstage.
    /// Set <see cref="CancelEventArgs.Cancel"/> to <c>true</c> to keep it open,
    /// for example to finish saving or validating what the user entered.
    /// </summary>
    /// <remarks>
    /// Raised for every way the user can close the backstage: Escape, clicking outside, the back button,
    /// the backstage button, commands and KeyTips.
    /// It's not raised when <see cref="IsOpen"/> is set directly from code, because then the app itself
    /// decided to close it.
    /// </remarks>
    public event EventHandler<CancelEventArgs>? Closing;

    private BackstageAdorner? adorner;

    #region Properties

    /// <summary>
    /// Gets the <see cref="AdornerLayer"/> for the <see cref="Backstage"/>.
    /// </summary>
    /// <remarks>This is exposed to make it possible to show content on the same <see cref="AdornerLayer"/> as the backstage is shown on.</remarks>
    public AdornerLayer? AdornerLayer { get; private set; }

    #region IsOpen

    /// <summary>
    /// Gets or sets whether backstage is shown
    /// </summary>
    public bool IsOpen
    {
        get => (bool)this.GetValue(IsOpenProperty);
        set => this.SetValue(IsOpenProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="IsOpen"/> dependency property.</summary>
    public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.FalseBox, OnIsOpenChanged, CoerceIsOpen));

    private static object? CoerceIsOpen(DependencyObject d, object? baseValue)
    {
        var backstage = (Backstage)d;

        if (backstage.CanChangeIsOpen == false)
        {
            return BooleanBoxes.Box(backstage.IsOpen);
        }

        return baseValue;
    }

    private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (Backstage)d;

        var oldValue = (bool)e.OldValue;
        var newValue = (bool)e.NewValue;

        lock (syncIsOpen)
        {
            if ((bool)e.NewValue)
            {
                control.Show();
            }
            else
            {
                control.Hide();
            }

            // Invoke the event
            control.IsOpenChanged?.Invoke(control, e);
        }

        (UIElementAutomationPeer.FromElement(control) as Fluent.Automation.Peers.RibbonBackstageAutomationPeer)?.RaiseExpandCollapseAutomationEvent(oldValue, newValue);
    }

    internal void SetIsOpen(bool isOpen)
    {
        // #1247: Give the app a chance to cancel closing.
        // Skipped when CanChangeIsOpen is false, because CoerceIsOpen keeps the backstage open anyway.
        if (isOpen == false
            && this.IsOpen
            && this.CanChangeIsOpen)
        {
            var args = new CancelEventArgs();
            this.OnClosing(args);

            if (args.Cancel)
            {
                return;
            }
        }

        this.SetCurrentValue(IsOpenProperty, BooleanBoxes.Box(isOpen));
    }

    /// <summary>
    /// Raises the <see cref="Closing"/> event.
    /// </summary>
    /// <param name="e">Set <see cref="CancelEventArgs.Cancel"/> to <c>true</c> to keep the backstage open.</param>
    protected virtual void OnClosing(CancelEventArgs e)
    {
        this.Closing?.Invoke(this, e);
    }

    /// <summary>
    /// Gets whether the <see cref="Backstage"/> is open and its content is actually displayed.
    /// </summary>
    /// <remarks>
    /// <see cref="IsOpen"/> alone is not enough, because <see cref="Show"/> can return early (e.g. <see cref="StartScreen"/> when it was already shown,
    /// or while waiting for Loaded) and then nothing is displayed although <see cref="IsOpen"/> is <c>true</c>.
    /// The adorner is created on the first real show and collapsed on hide, so only an existing, visible adorner means something is on screen.
    /// </remarks>
    internal bool IsOpenAndDisplayed => this.IsOpen
                                        && this.adorner is not null
                                        && this.adorner.Visibility == Visibility.Visible;

    #endregion

    /// <summary>
    /// Gets or sets whether backstage can be openend or closed.
    /// </summary>
    public bool CanChangeIsOpen
    {
        get => (bool)this.GetValue(CanChangeIsOpenProperty);
        set => this.SetValue(CanChangeIsOpenProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="CanChangeIsOpen"/> dependency property.</summary>
    public static readonly DependencyProperty CanChangeIsOpenProperty =
        DependencyProperty.Register(nameof(CanChangeIsOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether context tabs on the titlebar should be hidden when backstage is open
    /// </summary>
    public bool HideContextTabsOnOpen
    {
        get => (bool)this.GetValue(HideContextTabsOnOpenProperty);
        set => this.SetValue(HideContextTabsOnOpenProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="HideContextTabsOnOpen"/> dependency property.</summary>
    public static readonly DependencyProperty HideContextTabsOnOpenProperty =
        DependencyProperty.Register(nameof(HideContextTabsOnOpen), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether opening or closing should be animated.
    /// </summary>
    public bool AreAnimationsEnabled
    {
        get => (bool)this.GetValue(AreAnimationsEnabledProperty);
        set => this.SetValue(AreAnimationsEnabledProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="AreAnimationsEnabled"/> dependency property.</summary>
    public static readonly DependencyProperty AreAnimationsEnabledProperty =
        DependencyProperty.Register(nameof(AreAnimationsEnabled), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether to close the backstage when Esc is pressed
    /// </summary>
    public bool CloseOnEsc
    {
        get => (bool)this.GetValue(CloseOnEscProperty);
        set => this.SetValue(CloseOnEscProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="CloseOnEsc"/> dependency property.</summary>
    public static readonly DependencyProperty CloseOnEscProperty =
        DependencyProperty.Register(nameof(CloseOnEsc), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>Identifies the <see cref="UseHighestAvailableAdornerLayer"/> dependency property.</summary>
    public static readonly DependencyProperty UseHighestAvailableAdornerLayerProperty = DependencyProperty.Register(nameof(UseHighestAvailableAdornerLayer), typeof(bool), typeof(Backstage), new PropertyMetadata(BooleanBoxes.TrueBox));

    /// <summary>
    /// Gets or sets whether the highest available adorner layer should be used for the <see cref="BackstageAdorner"/>.
    /// This means that we will try to look up the visual tree till we find the highest <see cref="AdornerDecorator"/>.
    /// </summary>
    public bool UseHighestAvailableAdornerLayer
    {
        get => (bool)this.GetValue(UseHighestAvailableAdornerLayerProperty);
        set => this.SetValue(UseHighestAvailableAdornerLayerProperty, BooleanBoxes.Box(value));
    }

    #region Content

    /// <summary>
    /// Gets or sets content of the backstage
    /// </summary>
    public UIElement? Content
    {
        get => (UIElement?)this.GetValue(ContentProperty);
        set => this.SetValue(ContentProperty, value);
    }

    /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(UIElement), typeof(Backstage), new PropertyMetadata(OnContentChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var backstage = (Backstage)d;

        if (e.OldValue is not null)
        {
            // Release the old content: it must stop following the backstage's visibility.
            if (e.OldValue is DependencyObject dependencyObject)
            {
                BindingOperations.ClearBinding(dependencyObject, VisibilityProperty);
            }

            backstage.RemoveLogicalChild(e.OldValue);
        }

        if (e.NewValue is not null)
        {
            backstage.AddLogicalChild(e.NewValue);

            if (e.NewValue is DependencyObject dependencyObject)
            {
                BindingOperations.SetBinding(dependencyObject, VisibilityProperty, new Binding { Path = new PropertyPath(VisibilityProperty), Source = backstage });
            }
        }
    }

    #endregion

    #region LogicalChildren

    /// <inheritdoc />
    protected override IEnumerator LogicalChildren
    {
        get
        {
            var baseEnumerator = base.LogicalChildren;
            while (baseEnumerator?.MoveNext() == true)
            {
                yield return baseEnumerator.Current;
            }

            if (this.Content is not null)
            {
                yield return this.Content;
            }

            if (this.Icon is not null)
            {
                yield return this.Icon;
            }
        }
    }

    #endregion

    #endregion

    #region Initialization

    /// <summary>
    /// Static constructor
    /// </summary>
    static Backstage()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(typeof(Backstage)));
        // Disable QAT for this control
        CanAddToQuickAccessToolBarProperty.OverrideMetadata(typeof(Backstage), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Backstage()
    {
        this.Loaded += this.OnBackstageLoaded;
        this.Unloaded += this.OnBackstageUnloaded;
        this.DataContextChanged += this.Handle_DataContextChanged;
    }

    private void Handle_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (this.adorner is not null)
        {
            this.adorner.DataContext = e.NewValue;
        }
    }

    /// <summary>
    /// Called when this control receives the <see cref="PopupService.DismissPopupEvent"/>.
    /// </summary>
    protected virtual void OnDismissPopup(object? sender, DismissPopupEventArgs e)
    {
        // Don't close on dismiss popup event if application lost focus
        // or keytips should be shown.
        if (e.DismissReason == DismissPopupReason.ApplicationLostFocus
            || e.DismissReason == DismissPopupReason.ShowingKeyTips)
        {
            return;
        }

        // Only close backstage when popups should always be closed.
        // "Always" applies to controls marked with IsDefinitive for example.
        if (e.DismissMode != DismissPopupMode.Always)
        {
            return;
        }

        this.SetIsOpen(false);
    }

    #endregion

    #region Methods

    // Handles click event
    private void Click()
    {
        this.SetIsOpen(!this.IsOpen);
    }

    #region Show / Hide

    // We have to collapse WindowsFormsHost while Backstate is open
    private readonly Dictionary<FrameworkElement, Visibility> collapsedElements = new();

    // Counts the times the backstage was really shown (see ShowAdorner), so the end of a close animation can tell
    // whether the backstage was shown again while that animation was running.
    private int showCount;

    private Window? ownerWindow;
    private Ribbon? parentRibbon;

    private bool? originalHideContextTabs;

    /// <summary>
    /// Shows the <see cref="Backstage"/>
    /// </summary>
    /// <returns>
    /// <c>true</c> if the <see cref="Backstage"/> was made visible.
    /// <c>false</c> if the <see cref="Backstage"/> was not made visible.
    /// </returns>
    protected virtual bool Show()
    {
        // don't open the backstage while in design mode
        if (DesignerProperties.GetIsInDesignMode(this))
        {
            return false;
        }

        if (this.IsLoaded == false)
        {
            this.Loaded += this.OnDelayedShow;
            return false;
        }

        if (this.Content is null)
        {
            return false;
        }

        this.CreateAndAttachBackstageAdorner();

        this.ShowAdorner();

        this.parentRibbon = GetParentRibbon(this);

        if (this.parentRibbon is not null)
        {
            if (this.parentRibbon.TabControl is not null)
            {
                this.parentRibbon.TabControl.SetCurrentValue(RibbonTabControl.IsDropDownOpenProperty, BooleanBoxes.FalseBox);
                this.parentRibbon.TabControl.HighlightSelectedItem = false;

                // Unsubscribe first: when the backstage is reopened during its close animation, the ribbon was never restored
                // (see HideAdornerAndRestoreParentProperties), so the handler is still attached.
                this.parentRibbon.TabControl.RequestBackstageClose -= this.HandleTabControlRequestBackstageClose;
                this.parentRibbon.TabControl.RequestBackstageClose += this.HandleTabControlRequestBackstageClose;
            }

            this.parentRibbon.SetCurrentValue(Ribbon.IsBackstageOrStartScreenOpenProperty, BooleanBoxes.TrueBox);

            if (this.HideContextTabsOnOpen
                && this.parentRibbon.TitleBar is not null
                && this.parentRibbon.TitleBar.HideContextTabs is false)
            {
                this.originalHideContextTabs = this.parentRibbon.TitleBar.HideContextTabs;
                this.parentRibbon.TitleBar.SetCurrentValue(RibbonTitleBar.HideContextTabsProperty, BooleanBoxes.TrueBox);
            }
        }

        this.ownerWindow = Window.GetWindow(this);

        if (this.ownerWindow is null
            && this.Parent is not null)
        {
            this.ownerWindow = Window.GetWindow(this.Parent);
        }

        if (this.ownerWindow is not null)
        {
            // Unsubscribe first, for the same reason as RequestBackstageClose above.
            this.ownerWindow.KeyDown -= this.HandleOwnerWindowKeyDown;
            this.ownerWindow.KeyDown += this.HandleOwnerWindowKeyDown;

            // We are using an Adorner, so we have to collapse WindowsFormsHost while Backstage is open to draw above them.
            this.CollapseWindowsFormsHosts(this.ownerWindow);
        }

        return true;
    }

    /// <summary>
    /// Hides the <see cref="Backstage"/>
    /// </summary>
    protected virtual void Hide()
    {
        // potentially fixes https://github.com/fluentribbon/Fluent.Ribbon/issues/489
        if (this.Dispatcher.HasShutdownStarted
            || this.Dispatcher.HasShutdownFinished)
        {
            return;
        }

        this.Loaded -= this.OnDelayedShow;

        if (this.Content is null)
        {
            return;
        }

        if (!this.IsLoaded
            || this.adorner is null)
        {
            return;
        }

        this.HideAdornerAndRestoreParentProperties();
    }

    private void ShowAdorner()
    {
        if (this.adorner is null)
        {
            return;
        }

        this.showCount++;

        if (this.AreAnimationsEnabled
            && this.TryFindResource("Fluent.Ribbon.Storyboards.Backstage.IsOpenTrueStoryboard") is Storyboard storyboard)
        {
            storyboard = storyboard.Clone();

            storyboard.CurrentStateInvalidated += HandleStoryboardCurrentStateInvalidated;
            storyboard.Completed += HandleStoryboardOnCompleted;

            storyboard.Begin(this.adorner);
        }
        else
        {
            this.adorner.Visibility = Visibility.Visible;
            MoveFocusToContentAsync();
        }

        void HandleStoryboardCurrentStateInvalidated(object? sender, EventArgs e)
        {
            this.adorner.Visibility = Visibility.Visible;
            MoveFocusToContentAsync();
            storyboard.CurrentStateInvalidated -= HandleStoryboardCurrentStateInvalidated;
        }

        void HandleStoryboardOnCompleted(object? sender, EventArgs args)
        {
            this.AdornerLayer?.Update();

            storyboard.Completed -= HandleStoryboardOnCompleted;
        }

        void MoveFocusToContentAsync()
        {
            this.RunInDispatcherAsync(() => MoveFocusToContent(), DispatcherPriority.Background);
        }

        void MoveFocusToContent()
        {
            if (this.Content?.IsVisible is not true
                || this.Content.IsKeyboardFocusWithin)
            {
                return;
            }

            switch (this.Content)
            {
                case StartScreenTabControl { LeftContent: UIElement leftContent } when leftContent.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)):
                    return;
                case StartScreenTabControl { RightContent: UIElement rightContent } when rightContent.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)):
                    return;
                case BackstageTabControl { SelectedIndex: not -1 } backstageTabControl when backstageTabControl.ItemContainerGenerator.ContainerFromIndex(backstageTabControl.SelectedIndex) is BackstageTabItem backstageTabItem:
                    backstageTabItem.Focus();
                    return;
                case BackstageTabControl { SelectedContentHost: { } selectedContentHost }:
                    selectedContentHost.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    return;
                default:
                    this.Content.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    break;
            }
        }
    }

    private void HideAdornerAndRestoreParentProperties()
    {
        if (this.adorner is null)
        {
            return;
        }

        var showCountAtClose = this.showCount;
        var adornerAtClose = this.adorner;

        if (this.AreAnimationsEnabled
            && this.TryFindResource("Fluent.Ribbon.Storyboards.Backstage.IsOpenFalseStoryboard") is Storyboard storyboard)
        {
            storyboard = storyboard.Clone();

            storyboard.Completed += HandleStoryboardOnCompleted;

            storyboard.Begin(this.adorner);
        }
        else
        {
            this.adorner.Visibility = Visibility.Collapsed;
            this.RestoreParentProperties();

            // Same as the animated path below: give focus back to the backstage button,
            // otherwise it stays on the content that was just hidden.
            this.Focus();
        }

        void HandleStoryboardOnCompleted(object? sender, EventArgs args)
        {
            storyboard.Completed -= HandleStoryboardOnCompleted;

            // The backstage was shown again while this close animation was still running.
            // The open animation replaced this one, but this handler still runs when the old animation ends.
            // Hiding the adorner and restoring the ribbon now would hide the open backstage.
            // Not "IsOpen": IsOpen can be true again without a new show (a StartScreen is only shown once, a show can be delayed until Loaded).
            if (this.showCount != showCountAtClose)
            {
                return;
            }

            if (this.adorner is not null)
            {
                this.adorner.Visibility = Visibility.Collapsed;
            }

            if (this.AdornerLayer is not null)
            {
                this.AdornerLayer.Visibility = Visibility.Visible;
            }

            this.RestoreParentProperties();

            // Not when the adorner this animation ran on was replaced meanwhile: an unload (the backstage was removed,
            // or moved to another window) or a new template destroyed it, and with it the content that had the focus.
            // The user may be working elsewhere by then, even in another window, so the focus is left alone.
            if (ReferenceEquals(this.adorner, adornerAtClose))
            {
                this.Focus();
            }
        }
    }

    private static void HandleOpenBackstageCommandCanExecute(object sender, CanExecuteRoutedEventArgs args)
    {
        var target = ((BackstageAdorner)args.Source).Backstage;
        args.CanExecute = target.CanChangeIsOpen;
    }

    private static void HandleOpenBackstageCommandExecuted(object sender, ExecutedRoutedEventArgs args)
    {
        var target = ((BackstageAdorner)args.Source).Backstage;
        target.SetIsOpen(!target.IsOpen);
    }

    // The command binding this backstage added to the AdornerLayer.
    // The AdornerLayer is shared with other backstages/start screens (and maybe the app), so only this binding may be removed again.
    private CommandBinding? openBackstageCommandBinding;

    private void CreateAndAttachBackstageAdorner()
    {
        // It's possible that we created an adorner but it's parent AdornerLayer got destroyed.
        // If that's the case we have to destroy our adorner.
        // This fixes #228 Backstage disappears when changing DontUseDwm
        if (this.adorner?.Parent is null)
        {
            this.DestroyAdorner();
        }

        if (this.adorner is not null)
        {
            return;
        }

        FrameworkElement? elementToAdorn = UIHelper.GetParent<AdornerDecorator>(this)
                                           ?? UIHelper.GetParent<AdornerDecorator>(this.Parent);

        if (elementToAdorn is null)
        {
            return;
        }

        if (this.UseHighestAvailableAdornerLayer)
        {
            while (UIHelper.GetParent<AdornerDecorator>(elementToAdorn) is { } currentAdornerDecorator)
            {
                elementToAdorn = currentAdornerDecorator;
            }
        }

        this.AdornerLayer = UIHelper.GetAdornerLayer(elementToAdorn);

        if (this.AdornerLayer is null)
        {
            throw new Exception($"AdornerLayer could not be found for {this}.");
        }

        this.adorner = new BackstageAdorner(elementToAdorn, this);

        BindingOperations.SetBinding(this.adorner, DataContextProperty, new Binding
        {
            Path = new PropertyPath(DataContextProperty),
            Source = this
        });

        this.AdornerLayer.Add(this.adorner);

        this.openBackstageCommandBinding = new CommandBinding(RibbonCommands.OpenBackstage, HandleOpenBackstageCommandExecuted, HandleOpenBackstageCommandCanExecute);
        this.AdornerLayer.CommandBindings.Add(this.openBackstageCommandBinding);
    }

    private void DestroyAdorner()
    {
        // Don't clear all CommandBindings: that would also remove the bindings of other backstages sharing this AdornerLayer.
        if (this.openBackstageCommandBinding is not null)
        {
            this.AdornerLayer?.CommandBindings.Remove(this.openBackstageCommandBinding);
            this.openBackstageCommandBinding = null;
        }

        if (this.adorner is not null)
        {
            this.AdornerLayer?.Remove(this.adorner);
        }

        if (this.adorner is not null)
        {
            BindingOperations.ClearAllBindings(this.adorner);
        }

        this.adorner?.Clear();
        this.adorner = null;

        this.AdornerLayer = null;
    }

    private void RestoreParentProperties()
    {
        if (this.parentRibbon is not null)
        {
            if (this.parentRibbon.TabControl is not null)
            {
                this.parentRibbon.TabControl.HighlightSelectedItem = true;
                this.parentRibbon.TabControl.RequestBackstageClose -= this.HandleTabControlRequestBackstageClose;
            }

            this.parentRibbon.SetCurrentValue(Ribbon.IsBackstageOrStartScreenOpenProperty, BooleanBoxes.FalseBox);

            if (this.originalHideContextTabs.HasValue
                && this.parentRibbon.TitleBar is not null)
            {
                this.parentRibbon.TitleBar.SetCurrentValue(RibbonTitleBar.HideContextTabsProperty, this.originalHideContextTabs.Value);
            }

            this.parentRibbon = null;
            this.originalHideContextTabs = null;
        }

        if (this.ownerWindow is not null)
        {
            this.ownerWindow.KeyDown -= this.HandleOwnerWindowKeyDown;

            this.ownerWindow = null;
        }

        // Uncollapse elements
        foreach (var element in this.collapsedElements)
        {
            element.Key.Visibility = element.Value;
        }

        this.collapsedElements.Clear();
    }

    private void OnDelayedShow(object sender, EventArgs args)
    {
        this.Loaded -= this.OnDelayedShow;

        // Delaying show so everthing can load properly.
        // If we don't run this in the background setting IsOpen=true on application start we don't have access to the Bastage from the BackstageTabControl.
        this.RunInDispatcherAsync(() => this.Show(), DispatcherPriority.Background);
    }

    private void HandleTabControlRequestBackstageClose(object? sender, EventArgs e)
    {
        this.SetIsOpen(false);
    }

    // We have to collapse WindowsFormsHost while Backstage is open
    private void CollapseWindowsFormsHosts(DependencyObject parent)
    {
        switch (parent)
        {
            case null:
            case BackstageAdorner _:
                return;

            case FrameworkElement frameworkElement when parent is HwndHost
                                                        && frameworkElement.Visibility != Visibility.Collapsed:
                this.collapsedElements.Add(frameworkElement, frameworkElement.Visibility);
                frameworkElement.Visibility = Visibility.Collapsed;
                return;
        }

        // Traverse visual tree
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            this.CollapseWindowsFormsHosts(VisualTreeHelper.GetChild(parent, i));
        }
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Handled)
        {
            return;
        }

        if (e.Key == Key.Enter
            || e.Key == Key.Space)
        {
            if (this.IsFocused)
            {
                this.SetIsOpen(!this.IsOpen);
                e.Handled = true;
            }
        }

        base.OnKeyDown(e);
    }

    // Handles backstage Esc key keydown
    private void HandleOwnerWindowKeyDown(object sender, KeyEventArgs e)
    {
        if (this.CloseOnEsc == false
            || e.Key != Key.Escape)
        {
            return;
        }

        // only handle ESC when the backstage is open
        e.Handled = this.IsOpen;

        this.SetIsOpen(false);
    }

    private void OnBackstageLoaded(object sender, RoutedEventArgs e)
    {
        this.AddHandler(PopupService.DismissPopupEvent, (EventHandler<DismissPopupEventArgs>)this.OnDismissPopup);
    }

    private void OnBackstageUnloaded(object sender, RoutedEventArgs e)
    {
        this.RemoveHandler(PopupService.DismissPopupEvent, (EventHandler<DismissPopupEventArgs>)this.OnDismissPopup);

        // Unloaded while open or while the close animation is running. The adorner is destroyed below, and
        // neither Hide() nor the end of the close animation can be relied on to restore the ribbon and the
        // window any more (the animation of a removed backstage may never end, and its end is skipped when
        // the backstage is shown again elsewhere), so restore them now. Nothing to do when it's closed.
        this.RestoreParentProperties();

        if (this.IsOpen)
        {
            // IsOpen stays true: like a backstage opened before it was loaded, it's shown again when it's loaded again.
            this.Loaded -= this.OnDelayedShow;
            this.Loaded += this.OnDelayedShow;
        }

        this.DestroyAdorner();
    }

    #endregion

    #endregion

    #region Overrides

    /// <inheritdoc />
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (ReferenceEquals(e.Source, this) == false)
        {
            return;
        }

        this.Click();
    }

    /// <inheritdoc />
    public override KeyTipPressedResult OnKeyTipPressed()
    {
        this.SetIsOpen(true);
        base.OnKeyTipPressed();

        return KeyTipPressedResult.Empty;
    }

    /// <inheritdoc />
    public override void OnKeyTipBack()
    {
        this.SetIsOpen(false);
        base.OnKeyTipBack();
    }

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (this.IsOpen)
        {
            this.Hide();
        }

        // Like an unload: the close animation of the adorner destroyed below may never end, so restore the ribbon
        // and the window now. Nothing to do when it's closed and its close animation has ended; Show below collects them again if it shows it.
        this.RestoreParentProperties();

        this.DestroyAdorner();

        if (this.IsOpen)
        {
            this.Show();
        }
    }

    #endregion

    #region Quick Access Toolbar

    /// <inheritdoc />
    public override FrameworkElement CreateQuickAccessItem()
    {
        throw new NotImplementedException();
    }

    #endregion

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonBackstageAutomationPeer(this);
}