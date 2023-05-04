#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Portuguese (Brazil)", "pt-BR")]
    public class Portuguese_Brazil : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automático";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = "A";
        public override string BackstageButtonText { get; } = "Arquivo";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Mais cores...";
        public override string NoColor { get; } = "Nenhuma cor";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Customizar Barra de acesso rápido";
        public override string QuickAccessToolBarMenuHeader { get; } = " Customizar Barra de acesso rápido";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Mostrar acima do Ribbon";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Mostrar abaixo do Ribbon";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Mais controles";
        public override string RibbonContextMenuAddGallery { get; } = "Adicionar a galeria para Barra de acesso rápido";
        public override string RibbonContextMenuAddGroup { get; } = " Adicionar o grupo para Barra de acesso rápido";
        public override string RibbonContextMenuAddItem { get; } = "Adicionar para Barra de acesso rápido";
        public override string RibbonContextMenuAddMenu { get; } = " Adicionar o menu para Barra de acesso rápido";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Customizar Barra de acesso rápido...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Customizar o Ribbon...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimizar o Ribbon";
        public override string RibbonContextMenuRemoveItem { get; } = "Remover da Barra de acesso rápido";
        public override string RibbonContextMenuShowAbove { get; } = "Mostrar Barra de acesso rápido acima do Ribbon";
        public override string RibbonContextMenuShowBelow { get; } = "Mostrar Barra de acesso rápido abaixo do Ribbon";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "Este comando está desativado.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Usar a Faixa de Opções Clássica";
        public override string UseSimplifiedRibbon { get; } = "_Usar a Faixa de Opções Simplificada";
    }
}