#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Dutch", "nl")]
    public class Dutch : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automatisch";
        public override string BackstageButtonKeyTip { get; } = "B";
        public override string BackstageButtonText { get; } = "Bestand";
        public override string CustomizeStatusBar { get; } = FallbackLocalization.CustomizeStatusBar;
        public override string ExpandButtonScreenTipText { get; } = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, zijn alleen de tabulatie namen zichtbaar";
        public override string ExpandButtonScreenTipTitle { get; } = "Het lint Maximaliseren (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, zijn alleen de tabulatie namen zichtbaar";
        public override string MinimizeButtonScreenTipTitle { get; } = "Het lint minimaliseren (Ctrl + F1)";
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
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = FallbackLocalization.RibbonContextMenuCustomizeQuickAccessToolBar;
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Lint aanpassen...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = " Het lint minimaliseren";
        public override string RibbonContextMenuRemoveItem { get; } = " Verwijder uit werkbalk snelle toegang ";
        public override string RibbonContextMenuShowAbove { get; } = " Werkbalk snelle toegang boven het lint weergeven ";
        public override string RibbonContextMenuShowBelow { get; } = " Werkbalk snelle toegang onder het lint weergeven";
        public override string ScreenTipDisableReasonHeader { get; } = FallbackLocalization.ScreenTipDisableReasonHeader;
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}