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
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
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
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "Ta ukaz je trenutno onemogočen.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Uporabite klasičen trak";
        public override string UseSimplifiedRibbon { get; } = "_Uporabite poenostavljen trak";
    }
}