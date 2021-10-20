#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Catalan", "ca")]
    public class Catalan : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automàtica";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "Fitxer";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Més colors...";
        public override string NoColor { get; } = "Cap color";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Personalitza la barra d'eines d'accés ràpid";
        public override string QuickAccessToolBarMenuHeader { get; } = "Personalitza la barra d'eines d'accés ràpid";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Mostra sobre la cinta";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Mostra sota la cinta";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Més controls";
        public override string RibbonContextMenuAddGallery { get; } = "Afegeix galeria a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuAddGroup { get; } = "Afegeix grup a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuAddItem { get; } = "Afegeix a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuAddMenu { get; } = "Afegeix menú a la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Personalitza la barra d'eines d'accés ràpid...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Personalitza la cinta...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimitza la cinta";
        public override string RibbonContextMenuRemoveItem { get; } = "Elimina la barra d'eines d'accés ràpid";
        public override string RibbonContextMenuShowAbove { get; } = "Mostra la barra d'eines d'accés ràpid sobre la cinta";
        public override string RibbonContextMenuShowBelow { get; } = "Mostra la barra d'eines d'accés ràpid sota la cinta";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader /* This command is currently disabled. */;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Fes servir la franja clàssica";
        public override string UseSimplifiedRibbon { get; } = "_Fes servir la franja simplificada";
    }
}