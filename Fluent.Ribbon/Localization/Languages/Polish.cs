#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Polish", "pl")]
    public class Polish : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatyczne";
        public override string BackstageButtonKeyTip { get; } = "P";
        public override string BackstageButtonText { get; } = "Plik";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Więcej kolorów...";
        public override string NoColor { get; } = "Brak koloru";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Dostosuj pasek narzędzi Szybki dostęp";
        public override string QuickAccessToolBarMenuHeader { get; } = "Dostosuj pasek narzędzi Szybki dostęp";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Pokaż powyżej Wstążki";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Pokaż poniżej Wstążki";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Więcej poleceń...";
        public override string RibbonContextMenuAddGallery { get; } = "Dodaj Galerię do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuAddGroup { get; } = "Dodaj Grupę do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuAddItem { get; } = "Dodaj do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuAddMenu { get; } = "Dodaj do paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Dostosuj pasek narzędzi Szybki dostęp...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Dostosuj Wstążkę...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimalizuj Wstążkę";
        public override string RibbonContextMenuRemoveItem { get; } = "Usuń z paska narzędzi Szybki dostęp";
        public override string RibbonContextMenuShowAbove { get; } = "Pokaż pasek Szybki dostęp powyżej Wstążki";
        public override string RibbonContextMenuShowBelow { get; } = "Pokaż pasek Szybki dostęp poniżej Wstążki";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader /* This command is currently disabled. */;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Użyj wstążki klasycznej";
        public override string UseSimplifiedRibbon { get; } = "_Użyj wstążki uproszczonej";
    }
}