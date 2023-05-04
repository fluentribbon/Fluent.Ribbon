#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Ukrainian", "uk")]
    public class Ukrainian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Автоматичні";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = "Ф";
        public override string BackstageButtonText { get; } = "Файл";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Інші кольори...";
        public override string NoColor { get; } = "Без кольору";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Налаштувати Панель Інструментів Швидкого Доступу";
        public override string QuickAccessToolBarMenuHeader { get; } = "Налаштувати Панель Інструментів Швидкого Доступу";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Показати Поверх Стрічки";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Показати Знизу Стрічки";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Більше елементів";
        public override string RibbonContextMenuAddGallery { get; } = "Додати Галерею до Панелі Інструментів Швидкого Доступу";
        public override string RibbonContextMenuAddGroup { get; } = "Додати Групу до Панелі Інструментів Швидкого Доступу";
        public override string RibbonContextMenuAddItem { get; } = "Додати до Панелі Інструментів Швидкого Доступу";
        public override string RibbonContextMenuAddMenu { get; } = "Додати Меню до Панелі Інструментів Швидкого Доступу";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Налаштувати Панель Інструментів Швидкого Доступу...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Налаштувати Стрічку...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Зменшити Стрічку";
        public override string RibbonContextMenuRemoveItem { get; } = "Видалити з Панелі Інструментів Швидкого Доступу";
        public override string RibbonContextMenuShowAbove { get; } = "Показати Панель Інструментів Швидкого Доступу Поверх Стрічки";
        public override string RibbonContextMenuShowBelow { get; } = "Показати Панель Інструментів Швидкого Доступу Знизу Стрічки";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "Ця команда на даний момент недоступна.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Використовувати класичну стрічку";
        public override string UseSimplifiedRibbon { get; } = "_Використовувати спрощену стрічку";
    }
}