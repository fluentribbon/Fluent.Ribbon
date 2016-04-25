#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("French", "fr")]
    public class French : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatique";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = "Fichier";
        public override string CustomizeStatusBar { get; } = "Personnaliser la barre de statut";
        public override string ExpandButtonScreenTipText { get; } = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul les noms sont affichés";
        public override string ExpandButtonScreenTipTitle { get; } = "Agrandir le Ruban (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul les noms sont affichés";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimiser le Ruban (Ctrl + F1)";
        public override string MoreColors { get; } = "Plus de couleurs...";
        public override string NoColor { get; } = "Pas de couleur";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Personnaliser la barre d'outils Accès Rapide";
        public override string QuickAccessToolBarMenuHeader { get; } = "Personnaliser la barre d'outil Accès Rapide";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Afficher au dessus du Ruban";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Afficher en dessous du Ruban";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Plus de contrôles";
        public override string RibbonContextMenuAddGallery { get; } = "Ajouter une galerie à la barre d'outils Accès Rapide";
        public override string RibbonContextMenuAddGroup { get; } = "Ajouter un groupe à la barre d'outils Accès Rapide";
        public override string RibbonContextMenuAddItem { get; } = "Ajouter un élément à la barre d'outils Accès Rapide";
        public override string RibbonContextMenuAddMenu { get; } = "Ajouter un menu à la barre d'outils Accès Rapide";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Personnaliser la barre d'outils Accès Rapide...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Personnaliser le Ruban...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimiser le Ruban";
        public override string RibbonContextMenuRemoveItem { get; } = "Supprimer de la barre d'outils Accès Rapide";
        public override string RibbonContextMenuShowAbove { get; } = "Afficher la barre d'outils Accès Rapide au dessus du Ruban";
        public override string RibbonContextMenuShowBelow { get; } = "Afficher la barre d'outils Accès Rapide en dessous du Ruban";
        public override string ScreenTipDisableReasonHeader { get; } = "Cette commande est actuellement désactivée.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}