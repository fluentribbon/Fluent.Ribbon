#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Hungarian", "hu")]
    public class Hungarian : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Fájl";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string MinimizeButtonScreenTipTitle { get; } = "A menüszalag összecsukása (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Csak a lapnevek megjelenítése a menüszalagon";
        public override string ExpandButtonScreenTipTitle { get; } = "Menüszalag kibontása (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "A menüszalag megjelenítése úgy, hogy egy parancsra kattintás után is látható maradjon";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Gyorselérési eszköztár testreszabása";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "További vezérlők";
        public override string QuickAccessToolBarMenuHeader { get; } = "Gyorselérési eszköztár testreszabása";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Megjelenítés a menüszalag alatt";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Megjelenítés a menüszalag felett";
        public override string RibbonContextMenuAddItem { get; } = "Felvétel a gyorselérési eszköztárra";
        public override string RibbonContextMenuAddGroup { get; } = "Felvétel a gyorselérési eszköztárra";
        public override string RibbonContextMenuAddGallery { get; } = "Gyűjtemény felvétele a gyorselérési eszköztárra";
        public override string RibbonContextMenuAddMenu { get; } = "Felvétel a gyorselérési eszköztárra";
        public override string RibbonContextMenuRemoveItem { get; } = "Eltávolítás a gyorselérési eszköztárról";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Gyorselérési eszköztár testreszabása...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Menüszalag testreszabása...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = " A menüszalag összecsukása";
        public override string RibbonContextMenuShowAbove { get; } = "A gyorselérési eszköztár megjelenítése a menüszalag felett";
        public override string RibbonContextMenuShowBelow { get; } = "A gyorselérési eszköztár megjelenítése a menüszalag alatt";
        public override string ScreenTipDisableReasonHeader { get; } = "Ez a parancs jelenleg nem használható.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
    }
}