#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Vietnamese", "vi")]
    public class Vietnamese : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Tự động";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "Tệp";
        public override string CustomizeStatusBar { get; } = "Tùy biến thanh Trạng thái";
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Nhiều màu sắc...";
        public override string NoColor { get; } = "Không có màu";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Tùy chỉnh thanh công cụ Truy cập nhanh";
        public override string QuickAccessToolBarMenuHeader { get; } = "Tùy chỉnh thanh công cụ Truy cập nhanh";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Hiện trên thanh Ruy băng";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Hiện dưới thanh Ruy băng";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Thêm điều khiển";
        public override string RibbonContextMenuAddGallery { get; } = "Thêm bộ sưu tập vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuAddGroup { get; } = "Thêm nhóm vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuAddItem { get; } = "Thêm vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuAddMenu { get; } = "Thêm menu vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Tùy chỉnh thanh công cụ Truy cập nhanh...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Tùy biến thanh Ruy băng...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Thu gọn Ruy băng";
        public override string RibbonContextMenuRemoveItem { get; } = "Loại";
        public override string RibbonContextMenuShowAbove { get; } = "Hiện thanh công cụ truy cập nhanh trên thanh Ruy băng";
        public override string RibbonContextMenuShowBelow { get; } = "Hiện thanh công cụ truy cập nhanh dưới thanh Ruy băng";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "Lệnh này hiện bị tắt.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Sử dụng Dải băng cổ điển";
        public override string UseSimplifiedRibbon { get; } = "_Sử dụng Dải băng đơn giản";
    }
}