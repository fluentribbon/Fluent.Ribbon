#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Romanian", "ro")]
    public class Romanian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automată";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = "Fișier";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt afișate doar numele taburilor";
        public override string ExpandButtonScreenTipTitle { get; } = "Expandează Ribbon-ul (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt afișate doar numele taburilor";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimizează Ribbon-ul (Ctrl + F1)";
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
        public override string ScreenTipDisableReasonHeader { get; } = "Această comandă nu este disponibilă momentan.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}