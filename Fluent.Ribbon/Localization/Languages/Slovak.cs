#pragma warning disable

namespace Fluent.Localization.Languages;

[RibbonLocalization("Slovak", "sk")]
public class Slovak : RibbonLocalizationBase
{
    public override string Automatic { get; } = "Automatické";
    public override string BackstageButtonKeyTip { get; } = "S";
    public override string BackstageButtonText { get; } = "Súbor";
    public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
    public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
    public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
    public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
    public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
    public override string MoreColors { get; } = "Ďalšie farby...";
    public override string NoColor { get; } = "Žiadna farba";
    public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Prispôsobenie panela s nástrojmi Rýchly prístup";
    public override string QuickAccessToolBarMenuHeader { get; } = "Prispôsobenie panela s nástrojmi Rýchly prístup";
    public override string QuickAccessToolBarMenuShowAbove { get; } = " Zobraziť nad pásom s nástrojmi ";
    public override string QuickAccessToolBarMenuShowBelow { get; } = "Zobraziť pod pásom s nástrojmi";
    public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Ďalšie príkazy";
    public override string RibbonContextMenuAddGallery { get; } = " Pridať galériu do panela s nástrojmi Rýchly prístup ";
    public override string RibbonContextMenuAddGroup { get; } = " Pridať na panel s nástrojmi Rýchly prístup ";
    public override string RibbonContextMenuAddItem { get; } = "Pridať na panel s nástrojmi Rýchly prístup";
    public override string RibbonContextMenuAddMenu { get; } = "Pridať na panel s nástrojmi Rýchly prístup";
    public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = " Prispôsobenie panela s nástrojmi Rýchly prístup...";
    public override string RibbonContextMenuCustomizeRibbon { get; } = "Prispôsobenie panela s nástrojmi Rýchly prístup...";
    public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimalizovať pás s nástrojmi";
    public override string RibbonContextMenuRemoveItem { get; } = "Odstrániť z panela s nástrojmi Rýchly prístup ";
    public override string RibbonContextMenuShowAbove { get; } = "Panel s nástrojmi Rýchly prístup zobraziť nad panelom s nástrojmi ";
    public override string RibbonContextMenuShowBelow { get; } = "Panel s nástrojmi Rýchly prístup zobraziť pod panelom s nástrojmi";
    public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
    public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader /* This command is currently disabled. */;
    public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
    public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
    public override string UseClassicRibbon { get; } = "_Použiť klasický pás s nástrojmi";
    public override string UseSimplifiedRibbon { get; } = "_Použiť zjednodušený pás s nástrojmi";
}