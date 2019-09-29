#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Slovenian", "sl")]
    public class Slovenian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Samodejno";
        public override string BackstageButtonKeyTip { get; } = "D";
        public override string BackstageButtonText { get; } = "Datoteka";
        public override string CustomizeStatusBar { get; } = "Prilagodi vrstico stanja";
        public override string ExpandButtonScreenTipText { get; } = "Pokaži ali skrij trak\n\nKo je trak skrit, so prikazani samo zavihki";
        public override string ExpandButtonScreenTipTitle { get; } = "Razširi trak (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Pokaži ali skrij trak\n\nKo je trak skrit, so prikazani samo zavihki";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimiraj trak (Ctrl + F1)";
        public override string MoreColors { get; } = "Več barv...";
        public override string NoColor { get; } = "Brez barve";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Prilagodi orodno vrstico za hitri dostop";
        public override string QuickAccessToolBarMenuHeader { get; } = "Prilagodi orodno vrstico za hitri dostop";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Pokaži nad trakom";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Pokaži pod trakom";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Več ukazov";
        public override string RibbonContextMenuAddGallery { get; } = "Dodaj galerijo orodni vrstici za hitri dostop";
        public override string RibbonContextMenuAddGroup { get; } = "Dodaj skupino orodni vrstici za hitri dostop";
        public override string RibbonContextMenuAddItem { get; } = "Dodaj v orodno vrstico za hitri dostop";
        public override string RibbonContextMenuAddMenu { get; } = "Dodaj meni orodni vrstici za hitri dostop";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Prilagodi orodno vrstico za hitri dostop...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Prilagodi trak...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimiraj trak";
        public override string RibbonContextMenuRemoveItem { get; } = "Odstrani iz orodne vrstice za hitri dostop";
        public override string RibbonContextMenuShowAbove { get; } = "Pokaži orodno vrstico za hitri dostop nad trakom";
        public override string RibbonContextMenuShowBelow { get; } = "Pokaži orodno vrstico za hitri dostop pod trakom";
        public override string ScreenTipDisableReasonHeader { get; } = "Ta ukaz je trenutno onemogočen.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}