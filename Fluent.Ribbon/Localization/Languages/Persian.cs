#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Persian", "fa")]
    public class Persian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "خودکار";
        public override string BackstageButtonKeyTip { get; } = "ف";
        public override string BackstageButtonText { get; } = "فایل";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";
        public override string ExpandButtonScreenTipTitle { get; } = "بزرگ کردن نوار (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";
        public override string MinimizeButtonScreenTipTitle { get; } = "کوچک کردن نوار (Ctrl + F1)";
        public override string MoreColors { get; } = "رنگهای بیشتر...";
        public override string NoColor { get; } = "بدون رنگ";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "دلخواه سازی میله ابزار دسترسی سریع";
        public override string QuickAccessToolBarMenuHeader { get; } = "دلخواه سازی میله ابزار دسترسی سریع";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "نمایش در بالای نوار";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "نمایش در پایین نوار";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "ابزارهای دیگر";
        public override string RibbonContextMenuAddGallery { get; } = "اضافه کردن گالری به میله ابزار دسترسی سریع";
        public override string RibbonContextMenuAddGroup { get; } = "اضافه کردن گروه به میله ابزار دسترسی سریع";
        public override string RibbonContextMenuAddItem { get; } = "اضافه کردن به میله ابزار دسترسی سریع";
        public override string RibbonContextMenuAddMenu { get; } = "اضاقه کردن منو به میله ابزار دسترسی سریع";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "دلخواه سازی میله ابزار دسترسی سریع...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "دلخواه سازی نوار...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "کوچک کردن نوار";
        public override string RibbonContextMenuRemoveItem { get; } = "حذف از میله ابزار دسترسی سریع";
        public override string RibbonContextMenuShowAbove { get; } = "نمایش میله ابزار دسترسی سریع در بالای نوار";
        public override string RibbonContextMenuShowBelow { get; } = "نمایش میله ابزار دسترسی سریع در پایین نوار";
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}