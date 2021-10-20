#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Swedish", "sv")]
    public class Swedish : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatisk";
        public override string BackstageButtonKeyTip { get; } = "A";
        public override string BackstageButtonText { get; } = "Arkiv";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
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
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader /* This command is currently disabled. */;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Använd klassiska menyfliksområdet";
        public override string UseSimplifiedRibbon { get; } = "_Använd förenklade menyfliksområdet";
    }
}