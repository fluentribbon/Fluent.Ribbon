#pragma warning disable

namespace Fluent.Localization.Languages;

[RibbonLocalization("Hungarian", "hu")]
public class Hungarian : RibbonLocalizationBase
{
    public override string Automatic { get; } = "Automatikus";
    public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
    public override string BackstageButtonText { get; } = "Fájl";
    public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
    public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
    public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
    public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
    public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
    public override string MoreColors { get; } = "További színek...";
    public override string NoColor { get; } = "Nincs szín";
    public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Gyorselérési eszköztár testreszabása";
    public override string QuickAccessToolBarMenuHeader { get; } = "Gyorselérési eszköztár testreszabása";
    public override string QuickAccessToolBarMenuShowAbove { get; } = "Megjelenítés a menüszalag alatt";
    public override string QuickAccessToolBarMenuShowBelow { get; } = "Megjelenítés a menüszalag felett";
    public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "További vezérlők";
    public override string RibbonContextMenuAddGallery { get; } = "Gyűjtemény felvétele a gyorselérési eszköztárra";
    public override string RibbonContextMenuAddGroup { get; } = "Felvétel a gyorselérési eszköztárra";
    public override string RibbonContextMenuAddItem { get; } = "Felvétel a gyorselérési eszköztárra";
    public override string RibbonContextMenuAddMenu { get; } = "Felvétel a gyorselérési eszköztárra";
    public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Gyorselérési eszköztár testreszabása...";
    public override string RibbonContextMenuCustomizeRibbon { get; } = "Menüszalag testreszabása...";
    public override string RibbonContextMenuMinimizeRibbon { get; } = " A menüszalag összecsukása";
    public override string RibbonContextMenuRemoveItem { get; } = "Eltávolítás a gyorselérési eszköztárról";
    public override string RibbonContextMenuShowAbove { get; } = "A gyorselérési eszköztár megjelenítése a menüszalag felett";
    public override string RibbonContextMenuShowBelow { get; } = "A gyorselérési eszköztár megjelenítése a menüszalag alatt";
    public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
    public override string ScreenTipDisableReasonHeader { get; } = "Ez a parancs jelenleg nem használható.";
    public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
    public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
    public override string UseClassicRibbon { get; } = "_Klasszikus menüszalag használata";
    public override string UseSimplifiedRibbon { get; } = "Egys_zerűsített menüszalag használata";
}