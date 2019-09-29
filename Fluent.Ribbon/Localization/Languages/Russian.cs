#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Russian", "ru")]
    public class Russian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Автоматически";
        public override string BackstageButtonKeyTip { get; } = "Ф";
        public override string BackstageButtonText { get; } = "Файл";
        public override string CustomizeStatusBar { get; } = "Настройка строки состояния";
        public override string ExpandButtonScreenTipText { get; } = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только имена вкладок.";
        public override string ExpandButtonScreenTipTitle { get; } = "Развернуть ленту (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только имена вкладок.";
        public override string MinimizeButtonScreenTipTitle { get; } = "Свернуть ленту (Ctrl + F1)";
        public override string MoreColors { get; } = "Больше цветов...";
        public override string NoColor { get; } = "Без цвета";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Настройка панели быстрого доступа";
        public override string QuickAccessToolBarMenuHeader { get; } = "Настройка панели быстрого доступа";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Разместить над лентой";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Разместить под лентой";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Другие элементы";
        public override string RibbonContextMenuAddGallery { get; } = "Добавить коллекцию на панель быстрого доступа";
        public override string RibbonContextMenuAddGroup { get; } = "Добавить группу на панель быстрого доступа";
        public override string RibbonContextMenuAddItem { get; } = "Добавить на панель быстрого доступа";
        public override string RibbonContextMenuAddMenu { get; } = "Добавить меню на панель быстрого доступа";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Настройка панели быстрого доступа...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Настройка ленты...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Свернуть ленту";
        public override string RibbonContextMenuRemoveItem { get; } = "Удалить с панели быстрого доступа";
        public override string RibbonContextMenuShowAbove { get; } = "Разместить панель быстрого доступа над лентой";
        public override string RibbonContextMenuShowBelow { get; } = "Разместить панель быстрого доступа под лентой";
        public override string ScreenTipDisableReasonHeader { get; } = "В настоящее время эта команда отключена.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}