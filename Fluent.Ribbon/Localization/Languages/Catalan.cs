#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Catalan", "ca")]
    public class Catalan : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Fitxer";
        public override string BackstageButtonKeyTip { get; } = "F";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimitza la cinta (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només s'ensenyen els noms de les pestanyes";
        public override string ExpandButtonScreenTipTitle { get; } = "Expandeix la cinta (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només s'ensenyen els noms de les pestanyes";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Personalitza la barra d'eines d'accés ràpid";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Més controls";
        public override string QuickAccessToolBarMenuHeader { get; } = "Personalitza la barra d'eines d'accés ràpid";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Mostra sobre la cinta";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Mostra sota la cinta";
        public override string RibbonContextMenuAddItem { get; } = "Afegeix a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuAddGroup { get; } = "Afegeix grup a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuAddGallery { get; } = "Afegeix galeria a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuAddMenu { get; } = "Afegeix menú a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuRemoveItem { get; } = "Elimina la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Personalitza la barra d'eines d'accés ràpid...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Personalitza la cinta...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimitza la cinta";
        public override string RibbonContextMenuShowAbove { get; } = "Mostra la barra d'eines d'accés ràpid sobre la cinta";
        public override string RibbonContextMenuShowBelow { get; } = "Mostra la barra d'eines d'accés ràpid sota la cinta";
        public override string ScreenTipDisableReasonHeader { get; } = "This command is currently disabled.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
    }
}