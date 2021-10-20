#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Italian", "it")]
    public class Italian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatico";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = FallbackLocalization.BackstageButtonText;
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon;
        public override string MoreColors { get; } = "Più colori...";
        public override string NoColor { get; } = "Nessun colore";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Personalizza barra di accesso rapido";
        public override string QuickAccessToolBarMenuHeader { get; } = "Personalizza barra di accesso rapido";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Mostra sopra la barra multifunzione";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Mostra sotto la barra multifunzione";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Altri comandi…";
        public override string RibbonContextMenuAddGallery { get; } = "Aggiungi raccolta alla barra di accesso rapido";
        public override string RibbonContextMenuAddGroup { get; } = "Aggiungi gruppo alla barra di accesso rapido";
        public override string RibbonContextMenuAddItem { get; } = "Aggiungi alla barra di accesso rapido";
        public override string RibbonContextMenuAddMenu { get; } = "Aggiungi menu alla barra di accesso rapido";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Personalizza barra di accesso rapido...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Personalizza barra multifunzione...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Riduci a icona barra multifunzione";
        public override string RibbonContextMenuRemoveItem { get; } = "Rimuovi dalla barra di accesso rapido";
        public override string RibbonContextMenuShowAbove { get; } = "Mostra la barra di accesso rapido sopra la barra multifunzione";
        public override string RibbonContextMenuShowBelow { get; } = "Mostra la barra di accesso rapido sotto la barra multifunzione";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout;
        public override string ScreenTipDisableReasonHeader { get; } = "Questo commando è disattivato.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon;
        public override string UseClassicRibbon { get; } = "_Usa barra multifunzione classica";
        public override string UseSimplifiedRibbon { get; } = "_Usa barra multifunzione semplificata";
    }
}