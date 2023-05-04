#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Spanish", "es")]
    public class Spanish : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automático";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = "A";
        public override string BackstageButtonText { get; } = "Archivo";
        public override string CustomizeStatusBar { get; } = "Personalizar barra de estado";
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Más colores...";
        public override string NoColor { get; } = "No hay color";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Personalizar barra de herramientas de acceso rápido";
        public override string QuickAccessToolBarMenuHeader { get; } = "Personalizar barra de herramientas de acceso rápido";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Mostrar sobre la cinta";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Mostrar bajo la cinta";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Más controles";
        public override string RibbonContextMenuAddGallery { get; } = "Agregar galería a la barra de herramientas de acceso rápido";
        public override string RibbonContextMenuAddGroup { get; } = "Agregar grupo a la barra de herramientas de acceso rápido";
        public override string RibbonContextMenuAddItem { get; } = "Agregar a la barra de herramientas de acceso rápido";
        public override string RibbonContextMenuAddMenu { get; } = "Agregar menú a la barra de herramientas de acceso rápido";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Personalizar la barra de herramientas de acceso rápido...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Personalizar la cinta...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimizar la cinta";
        public override string RibbonContextMenuRemoveItem { get; } = "Quitar de la barra de herramientas de acceso rápido";
        public override string RibbonContextMenuShowAbove { get; } = "Mostrar barra de herramientas de acceso rápido sobre la cinta";
        public override string RibbonContextMenuShowBelow { get; } = "Mostrar barra de herramientas de acceso rápido bajo la cinta";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "Este comando está desactivado actualmente";
        public override string ScreenTipF1LabelHeader { get; } = "Pulse F1 para obtener más ayuda";
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Usar cinta clásica";
        public override string UseSimplifiedRibbon { get; } = "_Usar cinta simplificada";
    }
}