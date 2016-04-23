#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Dutch", "nl")]
    public class Dutch : RibbonLocalizationBase
    {
        public override string BackstageButtonText { get; } = "Bestand";
        public override string BackstageButtonKeyTip { get; } = "B";
        public override string MinimizeButtonScreenTipTitle { get; } = "Het lint minimaliseren (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, zijn alleen de tabulatie namen zichtbaar";
        public override string ExpandButtonScreenTipTitle { get; } = "Het lint Maximaliseren (Ctrl + F1)";
        public override string ExpandButtonScreenTipText { get; } = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, zijn alleen de tabulatie namen zichtbaar";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Werkbalk snelle toegang aanpassen";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "meer opdrachten";
        public override string QuickAccessToolBarMenuHeader { get; } = " Werkbalk snelle toegang aanpassen ";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Boven het lint weergeven";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "beneden het lint weergeven";
        public override string RibbonContextMenuAddItem { get; } = "Menu toevoegen aan werkbalk snelle toegang";
        public override string RibbonContextMenuAddGroup { get; } = "Groep toevoegen aan werkbalk snelle toegang";
        public override string RibbonContextMenuAddGallery { get; } = "Galerij toevoegen aan werkbalk snelle toegang";
        public override string RibbonContextMenuAddMenu { get; } = " Menu toevoegen aan werkbalk snelle toegang ";
        public override string RibbonContextMenuRemoveItem { get; } = " Verwijder uit werkbalk snelle toegang ";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Customize Quick Access Toolbar...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Lint aanpassen...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = " Het lint minimaliseren";
        public override string RibbonContextMenuShowAbove { get; } = " Werkbalk snelle toegang boven het lint weergeven ";
        public override string RibbonContextMenuShowBelow { get; } = " Werkbalk snelle toegang onder het lint weergeven";
        public override string ScreenTipDisableReasonHeader { get; } = "This command is currently disabled.";
        public override string ScreenTipF1LabelHeader { get; } = "Press F1 for help";
        public override string CustomizeStatusBar { get; } = "Customize Status Bar";
    }
}