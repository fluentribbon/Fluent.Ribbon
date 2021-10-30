#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Dutch", "nl")]
    public class Dutch : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatisch";
        public override string BackstageButtonKeyTip { get; } = "B";
        public override string BackstageButtonText { get; } = "Bestand";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar /* Customize Status Bar */;
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Meer kleuren...";
        public override string NoColor { get; } = "Geen kleur";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Werkbalk snelle toegang aanpassen";
        public override string QuickAccessToolBarMenuHeader { get; } = " Werkbalk snelle toegang aanpassen ";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Boven het lint weergeven";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Onder het lint weergeven";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Meer opdrachten";
        public override string RibbonContextMenuAddGallery { get; } = "Galerij toevoegen aan werkbalk snelle toegang";
        public override string RibbonContextMenuAddGroup { get; } = "Groep toevoegen aan werkbalk snelle toegang";
        public override string RibbonContextMenuAddItem { get; } = "Menu toevoegen aan werkbalk snelle toegang";
        public override string RibbonContextMenuAddMenu { get; } = " Menu toevoegen aan werkbalk snelle toegang ";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = FallbackLocalization.RibbonContextMenuCustomizeQuickAccessToolBar /* Customize Quick Access Toolbar... */;
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Lint aanpassen...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = " Het lint minimaliseren";
        public override string RibbonContextMenuRemoveItem { get; } = " Verwijder uit werkbalk snelle toegang ";
        public override string RibbonContextMenuShowAbove { get; } = " Werkbalk snelle toegang boven het lint weergeven ";
        public override string RibbonContextMenuShowBelow { get; } = " Werkbalk snelle toegang onder het lint weergeven";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader /* This command is currently disabled. */;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Klassieke lint gebruiken";
        public override string UseSimplifiedRibbon { get; } = "_Vereenvoudigd lint gebruiken";
    }
}