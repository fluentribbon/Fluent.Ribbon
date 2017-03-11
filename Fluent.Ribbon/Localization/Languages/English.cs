#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("English", "en")]
    public class English : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatic";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string BackstageButtonText { get; } = "File";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
        public override string ExpandButtonScreenTipText { get; } = "Like seeing the ribbon? Keep it open while you work.";
        public override string ExpandButtonScreenTipTitle { get; } = "Pin the Ribbon (Ctrl+F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Need a bit more space? Collapse the ribbon so only the tab names show.";
        public override string MinimizeButtonScreenTipTitle { get; } = "Collapse the Ribbon (Ctrl+F1)";
        public override string MoreColors { get; } = "More colors...";
        public override string NoColor { get; } = "No color";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Customize Quick Access Toolbar";
        public override string QuickAccessToolBarMenuHeader { get; } = "Customize Quick Access Toolbar";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Show Above the Ribbon";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Show Below the Ribbon";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "More controls";
        public override string RibbonContextMenuAddGallery { get; } = "Add Gallery to Quick Access Toolbar";
        public override string RibbonContextMenuAddGroup { get; } = "Add Group to Quick Access Toolbar";
        public override string RibbonContextMenuAddItem { get; } = "Add to Quick Access Toolbar";
        public override string RibbonContextMenuAddMenu { get; } = "Add Menu to Quick Access Toolbar";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Customize Quick Access Toolbar...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Customize the Ribbon...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Collapse the Ribbon";
        public override string RibbonContextMenuRemoveItem { get; } = "Remove from Quick Access Toolbar";
        public override string RibbonContextMenuShowAbove { get; } = "Show Quick Access Toolbar Above the Ribbon";
        public override string RibbonContextMenuShowBelow { get; } = "Show Quick Access Toolbar Below the Ribbon";
        public override string ScreenTipDisableReasonHeader { get; } = "This command is currently disabled.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
    }
}