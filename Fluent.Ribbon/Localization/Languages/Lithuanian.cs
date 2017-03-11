#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Lithuanian", "lt")]
    public class Lithuanian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatinis";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = "Failas";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = "Rodyti juostelę taip, kad visada butų išskleista net ir spustelėjus komandą.";
        public override string ExpandButtonScreenTipTitle { get; } = "Išplėsti juostelę (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Juostelėje rodyti tik skirtukų pavadinimus.";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimizuoti juostelę (Ctrl + F1)";
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
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}