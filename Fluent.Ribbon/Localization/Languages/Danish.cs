#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Danish", "da")]
    public class Danish : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatisk";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = "Filer";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = "Vis båndet, så det altid er udvidet, selv når du klikker på en kommando.";
        public override string ExpandButtonScreenTipTitle { get; } = "Udvid båndet (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Vis kun fanenavnene på båndet.";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimer båndet (Ctrl + F1)";
        public override string MoreColors { get; } = "Flere farver...";
        public override string NoColor { get; } = "Ingen farve";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Tilpas værktøjslinjen Hurtig adgang";
        public override string QuickAccessToolBarMenuHeader { get; } = " Tilpas værktøjslinjen Hurtig adgang";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Vis ovenover båndet";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Vis under båndet";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Flere kontrolelementer";
        public override string RibbonContextMenuAddGallery { get; } = "Tilføj Galleri til værktøjslinjen Hurtig adgang";
        public override string RibbonContextMenuAddGroup { get; } = "Føj til værktøjslinjen Hurtig adgang";
        public override string RibbonContextMenuAddItem { get; } = "Føj til værktøjslinjen Hurtig adgang";
        public override string RibbonContextMenuAddMenu { get; } = "Føj til værktøjslinjen Hurtig adgang";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Tilpas værktøjslinjen Hurtig adgang...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Tilpas båndet...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimer båndet";
        public override string RibbonContextMenuRemoveItem { get; } = "Fjern fra værktøjslinjen Hurtig adgang";
        public override string RibbonContextMenuShowAbove { get; } = "Vis værktøjslinjen Hurtig adgang ovenover båndet";
        public override string RibbonContextMenuShowBelow { get; } = "Vis værktøjslinjen Hurtig adgang under båndet";
        public override string ScreenTipDisableReasonHeader { get; } = "Denne kommando er aktuelt deaktiveret.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}