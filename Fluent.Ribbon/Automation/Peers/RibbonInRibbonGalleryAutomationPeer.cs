namespace Fluent.Automation.Peers;

using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using Fluent.Internal.KnownBoxes;

/// <summary>
///     Automation peer for <see cref="InRibbonGallery" />
/// </summary>
// todo: add full automation for listing items (?) etc.
public class RibbonInRibbonGalleryAutomationPeer : SelectorAutomationPeer, IExpandCollapseProvider
{
    private readonly InRibbonGallery owner;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    public RibbonInRibbonGalleryAutomationPeer(InRibbonGallery owner)
        : base(owner)
    {
        this.owner = owner;
    }

    /// <inheritdoc />
    protected override string GetClassNameCore()
    {
        return this.Owner.GetType().Name;
    }

    /// <inheritdoc />
    protected override string? GetNameCore()
    {
        // Explicit name, then the text header, then the ScreenTip title (for icon-only controls).
        return AutomationPeerHelper.GetName(this.Owner, base.GetNameCore());
    }

    /// <inheritdoc />
    protected override string? GetAccessKeyCore()
    {
        // Expose the KeyTip so screen readers can announce the keyboard shortcut.
        return AutomationPeerHelper.GetAccessKey(this.Owner, base.GetAccessKeyCore());
    }

    /// <inheritdoc />
    protected override string GetHelpTextCore()
    {
        // Expose the ScreenTip text, which the default implementation can't read.
        return AutomationPeerHelper.GetHelpText(this.Owner, base.GetHelpTextCore());
    }

    /// <inheritdoc />
    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.List;
    }

    /// <inheritdoc />
    protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
    {
        return new GalleryItemAutomationPeer(item, this);
    }

    /// <inheritdoc />
    public override object? GetPattern(PatternInterface patternInterface)
    {
        return patternInterface switch
        {
            PatternInterface.ExpandCollapse => this,

            // Scroll is intentionally not handled here: this peer does not implement IScrollProvider,
            // so returning "this" made UIA clients fail when they cast it to the provider interface.
            // The base (ItemsControlAutomationPeer) returns the ScrollViewer's IScrollProvider when there is one, otherwise null.
            _ => base.GetPattern(patternInterface)
        };
    }

    /// <inheritdoc />
    public void Collapse()
    {
        if (!this.IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        this.owner.SetCurrentValue(InRibbonGallery.IsDropDownOpenProperty, BooleanBoxes.FalseBox);
    }

    /// <inheritdoc />
    public void Expand()
    {
        if (!this.IsEnabled())
        {
            throw new ElementNotEnabledException();
        }

        this.owner.SetCurrentValue(InRibbonGallery.IsDropDownOpenProperty, BooleanBoxes.TrueBox);
    }

    /// <inheritdoc />
    public ExpandCollapseState ExpandCollapseState => this.owner.IsDropDownOpen
        ? ExpandCollapseState.Expanded
        : ExpandCollapseState.Collapsed;

    // BUG 1555137: Never inline, as we don't want to unnecessarily link the automation DLL
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    internal void RaiseExpandCollapseAutomationEvent(bool oldValue, bool newValue)
    {
        this.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty,
            oldValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed,
            newValue ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed);
    }
}