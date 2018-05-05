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
        public override string ExpandButtonScreenTipText { get; } = "Visualizza la barra multifunzione in modo che rimanga sempre espansa, anche se l’utente ha fatto click su un comando.";
        public override string ExpandButtonScreenTipTitle { get; } = "Espandi la barra multifunzione (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Consente di visualizzare solo i nomi delle schede nella barra multifunzione.";
        public override string MinimizeButtonScreenTipTitle { get; } = "Riduci a icona barra multifunzione (Ctrl + F1)";
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
        public override string ScreenTipDisableReasonHeader { get; } = "Questo commando è disattivato.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}