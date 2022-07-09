#pragma warning disable

namespace Fluent.Localization.Languages;

[RibbonLocalization("Bulgarian", "bg")]
public class Bulgarian : RibbonLocalizationBase
{
    public override string Automatic { get; } = "Автоматично";
    public override string BackstageButtonKeyTip { get; } = "Ф";
    public override string BackstageButtonText { get; } = "Файл";
    public override string CustomizeStatusBar { get; } = "Персонализиране на статус линията";
    public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
    public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
    public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
    public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
    public override string MoreColors { get; } = "Повече цветове...";
    public override string NoColor { get; } = "Без цвят";
    public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Персонализиране Бързия достъп";
    public override string QuickAccessToolBarMenuHeader { get; } = "Персонализиране на Бързия достъп";
    public override string QuickAccessToolBarMenuShowAbove { get; } = "Показване над лентата";
    public override string QuickAccessToolBarMenuShowBelow { get; } = "Показване под лентата";
    public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Повече контроли";
    public override string RibbonContextMenuAddGallery { get; } = "Добвяне на галерия в Бърз достъп";
    public override string RibbonContextMenuAddGroup { get; } = "Добавяне на група до Бързия достъп";
    public override string RibbonContextMenuAddItem { get; } = "Добавяне в Бързия достъп";
    public override string RibbonContextMenuAddMenu { get; } = "Добавяне на меню до Бързия достъп";
    public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Персонализиране на панела за Бърз достъп...";
    public override string RibbonContextMenuCustomizeRibbon { get; } = "Персонализиране на лентата...";
    public override string RibbonContextMenuMinimizeRibbon { get; } = "Свиване на лентата";
    public override string RibbonContextMenuRemoveItem { get; } = "Премахване от панела за Бърз достъп";
    public override string RibbonContextMenuShowAbove { get; } = "Показване на Бързия достъп над лентата";
    public override string RibbonContextMenuShowBelow { get; } = "Показване на Бързия достъп под лентата";
    public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
    public override string ScreenTipDisableReasonHeader { get; } = "Тази команда е забранена.";
    public override string ScreenTipF1LabelHeader { get; } = "Натиснете F1 за помощ";
    public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
    public override string UseClassicRibbon { get; } = "_Използване на класическата лента";
    public override string UseSimplifiedRibbon { get; } = "_Използване на опростената лента";
}