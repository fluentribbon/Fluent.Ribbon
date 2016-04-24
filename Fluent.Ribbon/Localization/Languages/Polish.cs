#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Polish", "pl")]
    public class Polish : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Plik";
        public override string BackstageButtonKeyTip { get; } = "P";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimalizuj Wstążkę (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko nazwy zakładek są widoczne";
        public override string ExpandButtonScreenTipTitle { get; } = "Rozwiń Wstążkę (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko nazwy zakładek są widoczne";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Dostosuj pasek narzędzi Szybki dostęp";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Więcej poleceń...";
        public override string QuickAccessToolBarMenuHeader { get; } = "Dostosuj pasek narzędzi Szybki dostęp";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Pokaż powyżej Wstążki";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Pokaż poniżej Wstążki";
        public override string RibbonContextMenuAddItem { get; } = "Dodaj do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuAddGroup { get; } = "Dodaj Grupę do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuAddGallery { get; } = "Dodaj Galerię do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuAddMenu { get; } = "Dodaj do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuRemoveItem { get; } = "Usuń z paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Dostosuj pasek narzędzi Szybki dostęp...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Dostosuj Wstążkę...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimalizuj Wstążkę";
        public override string RibbonContextMenuShowAbove { get; } = "Pokaż pasek Szybki dostęp powyżej Wstążki";
        public override string RibbonContextMenuShowBelow { get; } = "Pokaż pasek Szybki dostęp poniżej Wstążki";
        public override string ScreenTipDisableReasonHeader { get; } = "This command is currently disabled.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
        public override string NoColor { get; } = "Brak koloru";
    }
}