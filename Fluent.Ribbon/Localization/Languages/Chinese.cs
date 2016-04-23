#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Chinese", "zh")]
    public class Chinese : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "文件";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string MinimizeButtonScreenTipTitle { get; } = "功能区最小化 (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "仅显示功能区上的选项卡名称。单击选项卡可显示命令。";
        public override string ExpandButtonScreenTipTitle { get; } = "展开功能区 (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "始终显示功能区选项卡和命令。";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "自定义快速访问具栏";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "其他命令";
        public override string QuickAccessToolBarMenuHeader { get; } = "自定义快速访问工具栏";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "在功能区上方显示";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "在功能区下方显示";
        public override string RibbonContextMenuAddItem { get; } = "添加到快速访问工具栏";
        public override string RibbonContextMenuAddGroup { get; } = "在快速访问工具栏中添加组";
        public override string RibbonContextMenuAddGallery { get; } = "在快速访问工具栏中添加样式";
        public override string RibbonContextMenuAddMenu { get; } = "在快速访问工具栏中添加菜单";
        public override string RibbonContextMenuRemoveItem { get; } = "在快速访问工具栏中移除";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "自定义快速访问工具栏...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "自定义功能区...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "功能区最小化";
        public override string RibbonContextMenuShowAbove { get; } = "在功能区上方显示快速访问工具栏";
        public override string RibbonContextMenuShowBelow { get; } = "在功能区下方显示快速访问工具栏";
        public override string ScreenTipDisableReasonHeader { get; } = "此命令当前已被禁用。";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "自定义状态栏";
    }
}