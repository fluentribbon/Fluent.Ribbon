#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Arabic", "ar")]
    public class Arabic : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "تلقائي";
        public override string BackstageButtonKeyTip { get; } = "م    ";
        public override string BackstageButtonText { get; } = "ملف    ";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon;
        public override string MoreColors { get; } = "مزيد من الألوان...";
        public override string NoColor { get; } = "أي لون";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "تخصيص شريط أدوات الوصول السريع";
        public override string QuickAccessToolBarMenuHeader { get; } = "تخصيص شريط أدوات الوصول السريع";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "إظهار أعلى الشريط";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "إظهار أسفل الشريط";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "أوامر إضافية";
        public override string RibbonContextMenuAddGallery { get; } = "إضافة إلى شريط أدوات الوصول السريع";
        public override string RibbonContextMenuAddGroup { get; } = "إضافة إلى شريط أدوات الوصول السريع";
        public override string RibbonContextMenuAddItem { get; } = "إضافة إلى شريط أدوات الوصول السريع";
        public override string RibbonContextMenuAddMenu { get; } = "إضافة إلى شريط أدوات الوصول السريع";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "تخصيص شريط أدوات الوصول السريع...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "تخصيص الشريط...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "تصغير الشريط";
        public override string RibbonContextMenuRemoveItem { get; } = "إزالة إلى شريط أدوات الوصول السريع";
        public override string RibbonContextMenuShowAbove { get; } = "إظهار شريط أدوات الوصول السريع أعلى الشريط";
        public override string RibbonContextMenuShowBelow { get; } = "إظهار شريط أدوات الوصول السريع أسفل الشريط";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout;
        public override string ScreenTipDisableReasonHeader { get; } = "تم حالياً تعطيل هذا الأمر.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon;
        public override string UseClassicRibbon { get; } = "_استخدام الشريط التقليدي";
        public override string UseSimplifiedRibbon { get; } = "_استخدام الشريط المبسط";
    }
}