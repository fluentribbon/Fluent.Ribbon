#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Azerbaijani", "az")]
    public class Azerbaijani : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Fayl";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string MinimizeButtonScreenTipTitle { get; } = "Menyu lentini kiçilt (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Menyu lentini göstər və ya gizlət\n\nMenyu lentini kiçiləndə, yalnız tabların adları göstərilir";
        public override string ExpandButtonScreenTipTitle { get; } = "Menyu lentini böyüt(Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = " Menyu lentini göstər və ya gizlət\n\nMenyu lentini gizldəndə, yalnız, tabların adları göstərilir";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Sürətli Keçidin Alətlərini fərdiləşdir";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Digər nəzarət vasitələri";
        public override string QuickAccessToolBarMenuHeader { get; } = " Sürətli Keçidin Alətlərini fərdiləşdir ";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Menyu lentinin üstündə göstər";
        public override string QuickAccessToolBarMenuShowBelow { get; } = " Menyu lentinin altında göstər ";
        public override string RibbonContextMenuAddItem { get; } = "Sürətli Keçidin Alətlərinə əlavə et";
        public override string RibbonContextMenuAddGroup { get; } = " Sürətli Keçidin Alətlərinə Qrup əlavə et ";
        public override string RibbonContextMenuAddGallery { get; } = " Sürətli Keçidin Alətlərinə Qalereya əlavə et";
        public override string RibbonContextMenuAddMenu { get; } = " Sürətli Keçidin Alətlərinə Menyu əlavə et";
        public override string RibbonContextMenuRemoveItem { get; } = " Sürətli Keçidin Alətlərindən sil";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = " Sürətli Keçidin Alətlərini fərdiləşdir...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Menyu lentini fərdiləşdir...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = " Menyu lentini kiçilt";
        public override string RibbonContextMenuShowAbove { get; } = " Sürətli Keçidin Alətlərini Menyu lentinin üstündə göstər ";
        public override string RibbonContextMenuShowBelow { get; } = " Sürətli Keçidin Alətlərini Menyu lentinin altında göstər ";
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string NoColor { get; } = "No rəng";
    }
}