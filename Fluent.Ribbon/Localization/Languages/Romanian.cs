#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Romanian", "ro")]
    public class Romanian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automată";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "Fișier";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Mai multe culori...";
        public override string NoColor { get; } = "Nici o culoare";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Personalizează Bara de Acces Rapid";
        public override string QuickAccessToolBarMenuHeader { get; } = "Personalizează Bara de Acces Rapid";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Afișează peste Ribbon";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Afișează sub Ribbon";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Mai multe controale";
        public override string RibbonContextMenuAddGallery { get; } = "Adaugă Galeria la Bara de Acess Rapid";
        public override string RibbonContextMenuAddGroup { get; } = "Adaugă Grupul la Bara de Acess Rapid";
        public override string RibbonContextMenuAddItem { get; } = "Adaugă la Bara de Acess Rapid";
        public override string RibbonContextMenuAddMenu { get; } = "Adaugă Meniul la Bara de Acess Rapid";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Personalizează Bara de Acces Rapid...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Personalizează Ribbon-ul...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimizează Ribbon-ul...";
        public override string RibbonContextMenuRemoveItem { get; } = "Eimină din Bara de Acess Rapid";
        public override string RibbonContextMenuShowAbove { get; } = "Afișează Bara de Acces Rapid peste Ribbon";
        public override string RibbonContextMenuShowBelow { get; } = "Afișează Bara de Acces Rapid sub Ribbon";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "Această comandă nu este disponibilă momentan.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Utilizați panglica clasică";
        public override string UseSimplifiedRibbon { get; } = "_Utilizați panglica simplificată";
    }
}