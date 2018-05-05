#pragma warning disable

namespace Fluent.Localization.Languages
{
    [RibbonLocalization("Korean", "ko")]
    public class Korean : RibbonLocalizationBase
    {
        public override string Automatic { get; } = "자동";
        public override string BackstageButtonKeyTip { get; } = FallbackLocalization.BackstageButtonKeyTip;
        public override string BackstageButtonText { get; } = "파일";
        public override string CustomizeStatusBar { get; } = "상태 표시줄 사용자 지정";
        public override string ExpandButtonScreenTipText { get; } = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";
        public override string ExpandButtonScreenTipTitle { get; } = "리본 메뉴를 표시합니다 (Ctrl + F1)";
        public override string MinimizeButtonScreenTipText { get; } = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";
        public override string MinimizeButtonScreenTipTitle { get; } = "리본 메뉴를 최소화 합니다 (Ctrl + F1)";
        public override string MoreColors { get; } = "더 많은 색상...";
        public override string NoColor { get; } = "색 없음";
        public override string QuickAccessToolBarDropDownButtonTooltip { get; } = "빠른 실행 도구 모음 사용자 지정";
        public override string QuickAccessToolBarMenuHeader { get; } = "빠른 실행 도구 모음 사용자 지정";
        public override string QuickAccessToolBarMenuShowAbove { get; } = "리본 메뉴 위에 표시";
        public override string QuickAccessToolBarMenuShowBelow { get; } = "리본 메뉴 아래에 표시";
        public override string QuickAccessToolBarMoreControlsButtonTooltip { get; } = "기타 컨트롤들";
        public override string RibbonContextMenuAddGallery { get; } = "갤러리를 빠른 실행 도구 모음에 추가";
        public override string RibbonContextMenuAddGroup { get; } = "그룹을 빠른 실행 도구 모음에 추가";
        public override string RibbonContextMenuAddItem { get; } = "빠른 실행 도구 모음에 추가";
        public override string RibbonContextMenuAddMenu { get; } = "메뉴를 빠른 실행 도구 모음에 추가";
        public override string RibbonContextMenuCustomizeQuickAccessToolBar { get; } = "빠른 실행 도구 모음 사용자 지정...";
        public override string RibbonContextMenuCustomizeRibbon { get; } = "리본 메뉴 사용자 지정...";
        public override string RibbonContextMenuMinimizeRibbon { get; } = "리본 메뉴 최소화";
        public override string RibbonContextMenuRemoveItem { get; } = "빠른 실행 도구 모음에서 단추 제거";
        public override string RibbonContextMenuShowAbove { get; } = "리본 메뉴 위에 빠른 실행 도구 모음 표시";
        public override string RibbonContextMenuShowBelow { get; } = "리본 메뉴 아래에 빠른 실행 도구 모음 표시";
        public override string ScreenTipDisableReasonHeader { get; } = "이 명령은 현재 사용할 수 없습니다.";
        public override string ScreenTipF1LabelHeader { get; } = FallbackLocalization.ScreenTipF1LabelHeader;
    }
}