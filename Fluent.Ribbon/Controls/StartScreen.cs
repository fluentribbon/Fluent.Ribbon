// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Windows;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents the container for the <see cref="StartScreenTabControl"/>.
/// </summary>
public class StartScreen : Backstage
{
    /// <summary>
    /// Indicates whether the <see cref="StartScreen"/> has aleaady been shown or not.
    /// </summary>
    public bool Shown
    {
        get => (bool)this.GetValue(ShownProperty);
        set => this.SetValue(ShownProperty, value ? BooleanBoxes.TrueBox : BooleanBoxes.FalseBox);
    }

    /// <summary>Identifies the <see cref="Shown"/> dependency property.</summary>
    public static readonly DependencyProperty ShownProperty =
        DependencyProperty.Register(nameof(Shown), typeof(bool), typeof(StartScreen), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null));

    private bool? originalTitleBarIsCollapsed;

    // The title bar collapsed by the last Show, until it's restored.
    private RibbonTitleBar? collapsedTitleBar;

    // Displayed by the last Show, until it's hidden or unloaded.
    private bool isDisplayed;

    // Unloaded while displayed: it's still open, so it's displayed again when it's loaded again.
    private bool showAgainWhenLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartScreen"/> class.
    /// </summary>
    public StartScreen()
    {
        this.Unloaded += this.OnStartScreenUnloaded;
    }

    static StartScreen()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreen), new FrameworkPropertyMetadata(typeof(StartScreen)));

        VisibilityProperty.OverrideMetadata(typeof(StartScreen), new PropertyMetadata(OnVisibilityChanged));
    }

    private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((StartScreen)d).UpdateIsTitleBarCollapsed();
    }

    // This is required for scenarios like in #445 where the start screen is shown, but never hidden through Hide() but made hidden by changing just it's visibility.
    private void UpdateIsTitleBarCollapsed()
    {
        // Only while it's displayed: when it's open but not displayed (for example it was already shown once), nothing gives the title bar back.
        if (this.isDisplayed == false)
        {
            return;
        }

        var parentRibbon = GetParentRibbon(this);

        parentRibbon?.TitleBar?.SetCurrentValue(RibbonTitleBar.IsCollapsedProperty, this.Visibility == Visibility.Visible
            ? BooleanBoxes.TrueBox
            : BooleanBoxes.FalseBox);
    }

    /// <summary>
    /// Shows the <see cref="StartScreen"/>.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the <see cref="StartScreen"/> was made visible.
    /// <c>false</c> if the <see cref="StartScreen"/> was previously shown and was not made visible during this call
    /// (it's made visible again when it's loaded again while it's still open).
    /// </returns>
    protected override bool Show()
    {
        if (this.Shown
            && this.showAgainWhenLoaded == false)
        {
            return false;
        }

        if (base.Show() == false)
        {
            return false;
        }

        // Not written when it's already true: when it's displayed again after a reload, an app's OneWay binding on Shown is kept.
        if (this.Shown == false)
        {
            this.Shown = true;
        }

        this.isDisplayed = true;
        this.showAgainWhenLoaded = false;

        this.collapsedTitleBar = GetParentRibbon(this)?.TitleBar;
        this.originalTitleBarIsCollapsed = this.collapsedTitleBar?.IsCollapsed;
        this.collapsedTitleBar?.SetCurrentValue(RibbonTitleBar.IsCollapsedProperty, BooleanBoxes.TrueBox);

        return true;
    }

    /// <summary>
    /// Hides the <see cref="StartScreen" />.
    /// </summary>
    protected override void Hide()
    {
        base.Hide();

        this.isDisplayed = false;

        // Hidden (closed, or a new template) while unloaded: not displayed again when it's loaded again.
        this.showAgainWhenLoaded = false;

        this.RestoreTitleBar();
    }

    private void OnStartScreenUnloaded(object sender, RoutedEventArgs e)
    {
        // The backstage part hides it and gives the ribbon its normal state back when it's unloaded while displayed.
        // Give the title bar back too, and display it again when it's loaded again (it's still open).
        if (this.isDisplayed == false)
        {
            return;
        }

        this.isDisplayed = false;
        this.RestoreTitleBar();
        this.showAgainWhenLoaded = this.IsOpen;
    }

    // Shown stays true forever, so the saved value is cleared once restored: later closes
    // (where Show returned early and never touched the title bar) must not overwrite IsCollapsed again.
    private void RestoreTitleBar()
    {
        if (this.collapsedTitleBar is not null
            && this.originalTitleBarIsCollapsed.HasValue)
        {
            this.collapsedTitleBar.SetCurrentValue(RibbonTitleBar.IsCollapsedProperty, this.originalTitleBarIsCollapsed.Value);
        }

        this.collapsedTitleBar = null;
        this.originalTitleBarIsCollapsed = null;
    }
}