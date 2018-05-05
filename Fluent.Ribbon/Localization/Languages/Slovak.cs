#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Slovak", "sk")]
    public class Slovak : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatické";
        public override string BackstageButtonKeyTip { get; } = "S";
        public override string BackstageButtonText { get; } = "Súbor";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = " Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet ";
        public override string ExpandButtonScreenTipTitle { get; } = "Zobraziť pás s nástrojmi (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet";
        public override string MinimizeButtonScreenTipTitle { get; } = "Skryť pás s nástrojmi (Ctrl + F1)";
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
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}