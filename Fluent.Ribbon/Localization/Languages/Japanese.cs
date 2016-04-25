#pragma warning disable 1591

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Japanese", "ja")]
    public class Japanese : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "自動";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = "ファイル";
        public override string CustomizeStatusBar { get; } = "ステータス バーのユーザー設定";
        public override string ExpandButtonScreenTipText { get; } = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";
        public override string ExpandButtonScreenTipTitle { get; } = "リボンの展開 (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";
        public override string MinimizeButtonScreenTipTitle { get; } = "リボンの最小化 (Ctrl + F1)";
        public override string MoreColors { get; } = "他の色.";
        public override string NoColor { get; } = "色なし";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "クイック アクセス ツール バーのユーザー設定";
        public override string QuickAccessToolBarMenuHeader { get; } = "クイック アクセス ツール バーのユーザー設定";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "リボンの上に表示";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "リボンの下に表示";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "その他のボタン";
        public override string RibbonContextMenuAddGallery { get; } = "ギャラリーをクイック アクセス ツール バーに追加";
        public override string RibbonContextMenuAddGroup { get; } = "グループをクイック アクセス ツール バーに追加";
        public override string RibbonContextMenuAddItem { get; } = "クイック アクセス ツール バーに追加";
        public override string RibbonContextMenuAddMenu { get; } = "メニューをクイック アクセス ツール バーに追加";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "クイック アクセス ツール バーのユーザー設定...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "リボンのユーザー設定...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "リボンの最小化";
        public override string RibbonContextMenuRemoveItem { get; } = "クイック アクセス ツール バーから削除";
        public override string RibbonContextMenuShowAbove { get; } = "クイック アクセス ツール バーをリボンの上に表示";
        public override string RibbonContextMenuShowBelow { get; } = "クイック アクセス ツール バーをリボンの下に表示";
        public override string ScreenTipDisableReasonHeader { get; } = "このコマンドは現在無効になっています";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}