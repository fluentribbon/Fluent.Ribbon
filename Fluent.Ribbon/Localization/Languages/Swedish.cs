#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Swedish", "sv")]
    public class Swedish : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatisk";
        public override string BackstageButtonKeyTip { get; } = "A";
        public override string BackstageButtonText { get; } = "Arkiv";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas endast flikarna";
        public override string ExpandButtonScreenTipTitle { get; } = "Expandera menyfliksområdet (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas endast flikarna";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimera menyfliksområdet (Ctrl + F1)";
        public override string MoreColors { get; } = "Fler färger...";
        public override string NoColor { get; } = "Ingen färg";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Anpassa verktygsfältet Snabbåtkomst ";
        public override string QuickAccessToolBarMenuHeader { get; } = " Anpassa verktygsfältet Snabbåtkomst";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Visa ovanför menyfliksområdet";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Visa under menyfliksområdet";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Fler kommandon";
        public override string RibbonContextMenuAddGallery { get; } = "Lägg till galleriet i verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuAddGroup { get; } = "Lägg till i verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuAddItem { get; } = "Lägg till i verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuAddMenu { get; } = " Lägg till menyn i verktygsfältet Snabbåtkomst ";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Anpassa verktygsfältet Snabbåtkomst...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Anpassa menyfliksområdet...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimera menyfliksområdet";
        public override string RibbonContextMenuRemoveItem { get; } = "Ta bort från verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuShowAbove { get; } = " Visa verktygsfältet Snabbåtkomst ovanför menyfliksområdet ";
        public override string RibbonContextMenuShowBelow { get; } = " Visa verktygsfältet Snabbåtkomst under menyfliksområdet";
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}