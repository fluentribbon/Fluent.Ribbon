#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Hebrew", "he")]
    public class Hebrew : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "אוטומטי";
        public override string BackstageButtonKeyTip { get; } = "ק";
        public override string BackstageButtonText { get; } = "קובץ";
        public override string CustomizeStatusBar { get; } = "התאמה אישית של שורת המצב";
        public override string ExpandButtonScreenTipText { get; } = "הצג את רצועת הכלים כשהיא\nמורחבת תמיד, גם לאחר\nשתלחץ על הפקודה.";
        public override string ExpandButtonScreenTipTitle { get; } = "הרחב את רצועת הכלים (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "הצג רק את שמות הכרטיסיות\nברצועת הכלים.";
        public override string MinimizeButtonScreenTipTitle { get; } = "מזער את רצועת הכלים (Ctrl + F1)";
        public override string MoreColors { get; } = "צבעים יותר...";
        public override string NoColor { get; } = "אין צבע";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "התאמה אישית של סרגל הכלים לגישה מהירה";
        public override string QuickAccessToolBarMenuHeader { get; } = "התאמה אישית של סרגל הכלים לגישה מהירה";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "הצג מעל לרצועת הכלים";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "הצג מעל לרצועת הכלים";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "פקודות נוספות";
        public override string RibbonContextMenuAddGallery { get; } = "הוסף גלריה לסרגל הכלים לגישה מהירה";
        public override string RibbonContextMenuAddGroup { get; } = "הוסף קבוצה לסרגל הכלים לגישה מהירה";
        public override string RibbonContextMenuAddItem { get; } = "הוסף לסרגל הכלים לגישה מהירה";
        public override string RibbonContextMenuAddMenu { get; } = "הוסף תפריט לסרגל הכלים לגישה מהירה";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "של סרגל הכלים רצועת הכלים...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "התאמה אישית של רצועת הכלים...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "מזער את רצועת הכלים";
        public override string RibbonContextMenuRemoveItem { get; } = "הסר מסרגל הכלים לגישה מהירה";
        public override string RibbonContextMenuShowAbove { get; } = "הצג את סרגל הכלים לגישה מהירה מעל לרצועת הכלים";
        public override string RibbonContextMenuShowBelow { get; } = "הצג את סרגל הכלים לגישה מהירה מתחת לרצועת הכלים";
        public override string ScreenTipDisableReasonHeader { get; } = "פקודה זו אינה זמינה כעת.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}