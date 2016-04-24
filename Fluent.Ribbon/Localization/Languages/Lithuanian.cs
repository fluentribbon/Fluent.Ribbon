#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Lithuanian", "lt")]
    public class Lithuanian : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Failas";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimizuoti juostelę (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Juostelėje rodyti tik skirtukų pavadinimus.";
        public override string ExpandButtonScreenTipTitle { get; } = "Išplėsti juostelę (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Rodyti juostelę taip, kad visada butų išskleista net ir spustelėjus komandą.";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Tinkinti sparčiosios prieigos įrankių juostą";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Daugiau valdiklių";
        public override string QuickAccessToolBarMenuHeader { get; } = "Tinkinti sparčiosios prieigos įrankių juostą";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Rodyti virš juostelės";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Rodyti po juostele";
        public override string RibbonContextMenuAddItem { get; } = "Pridėti į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuAddGroup { get; } = "Pridėti į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuAddGallery { get; } = "Įtraukti galeriją į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuAddMenu { get; } = "Pridėti į sparčiosios prieigos įrankių juostą";
        public override string RibbonContextMenuRemoveItem { get; } = "Šalinti iš sparčiosios prieigos įrankių juostos";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Tinkinti sparčiosios prieigos įrankių juostą...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Tinkinti juostelę:";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimizuoti juostelę";
        public override string RibbonContextMenuShowAbove { get; } = "Rodyti virš juostelės";
        public override string RibbonContextMenuShowBelow { get; } = "Rodyti po juostele";
        public override string ScreenTipDisableReasonHeader { get; } = "This command is currently disabled.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
        public override string NoColor { get; } = "Spalvų";
    }
}