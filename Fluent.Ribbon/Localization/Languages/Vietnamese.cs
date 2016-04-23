#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Vietnamese", "vi")]
    public class Vietnamese : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Tệp";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string MinimizeButtonScreenTipTitle { get; } = "Thu gọn Ruy băng (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ có tên thẻ được hiện";
        public override string ExpandButtonScreenTipTitle { get; } = "Mở rộng Ruy băng (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ có tên thẻ được hiện";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Tùy chỉnh thanh công cụ Truy cập nhanh";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Thêm điều khiển";
        public override string QuickAccessToolBarMenuHeader { get; } = "Tùy chỉnh thanh công cụ Truy cập nhanh";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Hiện trên thanh Ruy băng";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Hiện dưới thanh Ruy băng";
        public override string RibbonContextMenuAddItem { get; } = "Thêm vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuAddGroup { get; } = "Thêm nhóm vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuAddGallery { get; } = "Thêm bộ sưu tập vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuAddMenu { get; } = "Thêm menu vào thanh công cụ Truy cập nhanh";
        public override string RibbonContextMenuRemoveItem { get; } = "Loại";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Tùy chỉnh thanh công cụ Truy cập nhanh...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Tùy biến thanh Ruy băng...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Thu gọn Ruy băng";
        public override string RibbonContextMenuShowAbove { get; } = "Hiện thanh công cụ truy cập nhanh trên thanh Ruy băng";
        public override string RibbonContextMenuShowBelow { get; } = "Hiện thanh công cụ truy cập nhanh dưới thanh Ruy băng";
        public override string ScreenTipDisableReasonHeader { get; } = "Lệnh này hiện bị tắt.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Tùy biến thanh Trạng thái";
    }
}