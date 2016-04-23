#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Swedish", "sv")]
    public class Swedish : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Arkiv";
        public override string BackstageButtonKeyTip { get; } = "A";
        public override string MinimizeButtonScreenTipTitle { get; } = "Minimera menyfliksområdet (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas endast flikarna";
        public override string ExpandButtonScreenTipTitle { get; } = "Expandera menyfliksområdet (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas endast flikarna";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Anpassa verktygsfältet Snabbåtkomst ";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Fler kommandon";
        public override string QuickAccessToolBarMenuHeader { get; } = " Anpassa verktygsfältet Snabbåtkomst";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Visa ovanför menyfliksområdet";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Visa under menyfliksområdet";
        public override string RibbonContextMenuAddItem { get; } = "Lägg till i verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuAddGroup { get; } = "Lägg till i verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuAddGallery { get; } = "Lägg till galleriet i verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuAddMenu { get; } = " Lägg till menyn i verktygsfältet Snabbåtkomst ";
        public override string RibbonContextMenuRemoveItem { get; } = "Ta bort från verktygsfältet Snabbåtkomst";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Anpassa verktygsfältet Snabbåtkomst...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Anpassa menyfliksområdet...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Minimera menyfliksområdet";
        public override string RibbonContextMenuShowAbove { get; } = " Visa verktygsfältet Snabbåtkomst ovanför menyfliksområdet ";
        public override string RibbonContextMenuShowBelow { get; } = " Visa verktygsfältet Snabbåtkomst under menyfliksområdet";
        public override string ScreenTipDisableReasonHeader { get; } = "This command is currently disabled.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
    }
}