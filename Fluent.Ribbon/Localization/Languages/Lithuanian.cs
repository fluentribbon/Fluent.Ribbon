#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Lithuanian", "lt")]
    public class Lithuanian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatinis";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "Failas";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Daugiau spalvų...";
        public override string NoColor { get; } = "Spalvų";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Tinkinti sparčiosios prieigos įrankių juostą";
        public override string QuickAccessToolBarMenuHeader { get; } = "Tinkinti sparčiosios prieigos įrankių juostą";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Rodyti virš juostelės";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Rodyti po juostele";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Daugiau valdiklių";
        public override string RibbonContextMenuAddGallery { get; } = "Įtraukti galeriją į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuAddGroup { get; } = "Pridėti į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuAddItem { get; } = "Pridėti į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuAddMenu { get; } = "Pridėti į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Tinkinti sparčiosios prieigos įrankių juostą...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Tinkinti juostelę:";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimizuoti juostelę";
        public override string RibbonContextMenuRemoveItem { get; } = "Šalinti iš sparčiosios prieigos įrankių juostos";
        public override string RibbonContextMenuShowAbove { get; } = "Rodyti virš juostelės";
        public override string RibbonContextMenuShowBelow { get; } = "Rodyti po juostele";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader /* This command is currently disabled. */;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Naudoti klasikinę juostelę";
        public override string UseSimplifiedRibbon { get; } = "_Naudoti supaprastintą juostelę";
    }
}