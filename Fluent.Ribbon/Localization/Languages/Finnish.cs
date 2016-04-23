#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Finnish", "fi")]
    public class Finnish : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Tiedosto";
        public override string BackstageButtonKeyTip { get; } = "T";
        public override string MinimizeButtonScreenTipTitle { get; } = "Pienennä valintanauha (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Näytä valintanauhassa vain välilehtien nimet";
        public override string ExpandButtonScreenTipTitle { get; } = "Laajenna valintanauha (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Näytä valintanauha aina laajennettuna silloinkin, kun valitset komennon";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Mukauta pikatyökaluriviä";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Lisää valintoja";
        public override string QuickAccessToolBarMenuHeader { get; } = "Mukauta pikatyökaluriviä";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Näytä valintanauhan yläpuolella";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Näytä valintanauhan alapuolella";
        public override string RibbonContextMenuAddItem { get; } = "Lisää pikatyökaluriville";
        public override string RibbonContextMenuAddGroup { get; } = "Lisää ryhmä pikatyökaluriviin";
        public override string RibbonContextMenuAddGallery { get; } = "Lisää valikoima pikatyökaluriviin";
        public override string RibbonContextMenuAddMenu { get; } = "Lisää valikko pikatyökaluriviin";
        public override string RibbonContextMenuRemoveItem { get; } = "Poista pikatyökaluriviltä";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Mukauta pikatyökaluriviä...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Mukauta valintanauhaa...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Pienennä valintanauha";
        public override string RibbonContextMenuShowAbove { get; } = "Näytä pikatyökalurivi valintanauhan yläpuolella";
        public override string RibbonContextMenuShowBelow { get; } = "Näytä pikatyökalurivi valintanauhan alapuolella";
        public override string ScreenTipDisableReasonHeader { get; } = "Tämä komento on tällä hetkellä poissa käytöstä";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
    }
}