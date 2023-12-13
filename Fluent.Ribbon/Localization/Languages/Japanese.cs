#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Japanese", "ja")]
    public class Japanese : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "自動";
        public override string BackstageBackButtonUid { get; } = FallbackLocalization.BackstageBackButtonUid /* Close Backstage */;
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip /* F */;
        public override string BackstageButtonText { get; } = "ファイル";
        public override string CustomizeStatusBar { get; } = "ステータス バーのユーザー設定";
        public override string DisplayOptionsButtonScreenTipText { get; } = "リボンの状態を切り替えます。\r\nリボンを折りたたむ、または展開するには、Ctrl + F1 を押します。";
        public override string DisplayOptionsButtonScreenTipTitle { get; } = "リボンの表示オプション";
        public override string ExpandRibbon { get; } = "常にリボンを表示する";
        public override string MinimizeRibbon { get; } = "タブのみを表示する";
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
        public override string RibbonLayout { get; } = FallbackLocalization.RibbonLayout /* Ribbon Layout */;
        public override string ScreenTipDisableReasonHeader { get; } = "このコマンドは現在無効になっています";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader /* Press F1 for help */;
        public override string ShowRibbon { get; } = "リボンを表示";
        public override string UseClassicRibbon { get; } = "クラシック リボンを使用(_U)";
        public override string UseSimplifiedRibbon { get; } = "シンプル リボンを使用(_U)";
    }
}