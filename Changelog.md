# Changelog for Fluent.Ribbon

## 3.2.0
- ### Features
  - [#31](../../issues/31) "System wide font settings."
  - [#38](../../issues/38) "A simple code to enable RibbonTabControl react to touch manipulation"

- ### Misc
  - Ribbon, RibbonTitleBar and RibbonStatusBar now use SystemFonts.MenuFont* as font settings

- ### Bug fixes
  - [#23](../../issues/23) "Drop support for ancient .NET 3.5"
  - [#24](../../issues/24) "RibbonTitleBar.MeasureOverride should not return the constraint parameter value"
  - [#25](../../issues/25) "Press Alt key without Backstage raises ArgumentNullException"
  - [#28](../../issues/28) "Height and FontSize of MenuItem"
  - [#30](../../issues/30) "Line break in _minimizeButtonScreenTipText"
  - [#33](../../issues/33) "Fix Issue #24 : measureoverride"
  - [#35](../../issues/35) "Quick Access Toolbar IsChecked never correctly set"
  - [#36](../../issues/36) "Fix size of group box controls with large font."
  - [#39](../../issues/39) "Application menu button has wrong height"
  - [#40](../../issues/40) "Simplified Chinese translation may be wrong"
  - [#41](../../issues/41) "Fix issue #30 - Line break in _minimizeButtonScreenTipText"
  - [#42](../../issues/42) "Set correct height on backstage and application menu button"
  - [#43](../../issues/43) "Added delay in ClosePopupOnMouseDown feature"
  - [#44](../../issues/44) "Little Bug in Office2013 style"

## 3.1.0
- ### Features
 - Added DropDownButton.ClosePopupOnMouseDown property which defaults to false. If true, it will close the drop down popup automatically when a mouse down event occurs.
 - [#3](../../issues/3)     "Consider using GitLink to allow users to step through Fluent source code"

- ### Misc
 - Renaming InnerBackstageTabCotrolItem to InnerBackstageTabControlItem

- ### Bug fixes
 - Fixed: [#11](../../issues/11)   "ContextMenu and Popup has 10px space at bottom and right side"
 - Fixed: [#13](../../issues/13)   "Backstage does not open at application start anymore"
 - Fixed: [#17](../../issues/17)   "Problem with Mouse Capture on SplitButton"
 - Fixed: [#18](../../issues/18)   "Tabs are disabled when using datatemplate for viewmodel in window content"
 - Fixed: [#19](../../issues/19)   "Vertical Scrollbar will not hide in Fluent:Combobox"
 - Fixed: 22521 "RibbonWindow is activated when focus is lost after Alt+Tab"
 - Fixed: 22523 "Typo in Generic.xaml"

## 3.0.3
- Fixed: 22519 "Normalize button closes application) Thanks to GeertvanHorrik for finding out that the version of Microsoft.Windows.Shell for .NET 4.0 that was used is buggy."

## 3.0.2
- Fixed: 22519 "Normalize button closes application"

## 3.0.1
- Fixed an issue with closed direct member menus (keytips were always redirected for direct childs)
- Fixed: 22518 "Faulty white margin on the right and left of the window body"
- Fixed: 22516 "Issues with backstage content localization using WPF Localization Extension"

## 3.0.0
- Too much work to write down changes made in 4 years. Please have a look at [changes in version 3.0](https://fluent.codeplex.com/wikipage?title=Changes%20in%20version%203.0&referringTitle=Documentation).