#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Estonian", "et")]
    public class Estonian : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "Automaatne";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "Fail";
        public override string CustomizeStatusBar { get; } = "Kohanda olekuriba";
        public override string DisplayOptionsButtonScreenTipText { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipText /* Configure Ribbon display options. */;
        public override string DisplayOptionsButtonScreenTipTitle { get; } = FallbackLocalization.DisplayOptionsButtonScreenTipTitle /* Ribbon Display Options */;
        public override string ExpandRibbon { get; } = FallbackLocalization.ExpandRibbon /* Expand the Ribbon */;
        public override string MinimizeRibbon { get; } = FallbackLocalization.MinimizeRibbon /* Minimize the Ribbon */;
        public override string MoreColors { get; } = "Rohkem värve...";
        public override string NoColor { get; } = "Ei värvi";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "Kohanda kiirpääsuriba";
        public override string QuickAccessToolBarMenuHeader { get; } = "Kohanda kiirpääsuriba";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "Kuva lindi kohal";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "Kuva lindi all";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "Rohkem juhtelemente";
        public override string RibbonContextMenuAddGallery { get; } = "Lisa galerii kiirpääsuribale";
        public override string RibbonContextMenuAddGroup { get; } = "Lisa rühm kiirpääsuribale";
        public override string RibbonContextMenuAddItem { get; } = "Lisa kiirpääsuribale";
        public override string RibbonContextMenuAddMenu { get; } = "Lisa menüü kiirpääsuribale";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "Kohanda kiirpääsuriba...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "Kohanda linti...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "Ahenda menüülint";
        public override string RibbonContextMenuRemoveItem { get; } = "Eemalda kiirpääsuribalt";
        public override string RibbonContextMenuShowAbove { get; } = "Kuva kiirpääsuriba lindi kohal";
        public override string RibbonContextMenuShowBelow { get; } = "Kuva kiirpääsuriba lindi all";
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "See käsk on praegu keelatud.";
        public override string ScreenTipF1LabelHeader { get; } = "Spikri kuvamiseks vajutage klahvi F1";
        public override string ShowRibbon { get; } = FallbackLocalization.ShowRibbon /* Show Ribbon */;
        public override string UseClassicRibbon { get; } = "_Kasuta klassikalist linti";
        public override string UseSimplifiedRibbon { get; } = "_Kasuta üherealist linti";
    }
}