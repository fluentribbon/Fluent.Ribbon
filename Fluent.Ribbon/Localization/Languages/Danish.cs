#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Danish", "da")]
    public class Danish : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatisk";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "Filer";
        public override string CustomizeStatusBar { get; } = "Tilpas statuslinje";
        public override string DisplayOptionsButtonScreenTipText { get; } = "Tilpas indstillinger for båndets visning.";
        public override string DisplayOptionsButtonScreenTipTitle { get; } = "Indstillinger for båndets visning";
        public override string ExpandRibbon { get; } = "Udvid båndet";
        public override string MinimizeRibbon { get; } = "Skjul båndet";
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
        public override string RibbonLayout { get; } = "Båndets udseende";
        public override string ScreenTipDisableReasonHeader { get; } = "Denne kommando er aktuelt deaktiveret.";
        public override string ScreenTipF1LabelHeader { get; } = "Tryk på F1 for hjælp";
        public override string ShowRibbon { get; } = "Vis båndet";
        public override string UseClassicRibbon { get; } = "_Brug Klassisk bånd";
        public override string UseSimplifiedRibbon { get; } = "_Brug Forenklet bånd";
    }
}