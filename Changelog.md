# Changelog for Fluent.Ribbon

## 3.5.1

- ### Bug fixes

  - [#161](../../issues/161) - Window not maximized correctly when MaxHeight and/or MaxWidth are set
  - [#160](../../issues/160) - Invisible first tab is selected when selected tab becomes invisible
  - [#159](../../issues/159) - Maximized RibbonWindow state error after long IO operation.
  - [#158](../../issues/158) - Treat all themes equally by generating their Generic.xaml in the specific theme folder
  - [#154](../../issues/154) - Removing item (that has DropDown ) from QAT removes items in Ribbon Toolbar
  - [#153](../../issues/153) - Incorrect rendering with SizeToContent="WidthAndHeight"
  - [#151](../../issues/151) - RibbonToolBarLayoutDefinition is not compatible with some bindings?
  - [#150](../../issues/150) - Menu Item Tool tip not appearing when in a datatemplate
  - [#147](../../issues/147) - Foreground colour backstage items
  - [#80](../../issues/80) - Window "Not Responding" cut Title

Commits: [9f8c919f1b...175ba6c882](../../compare/9f8c919f1b...175ba6c882)

## 3.5.0

- ### Bug fixes
  - [#143](../../issues/143) "Resizing InRibbonGallery causes content to be narrower than container"

- ### Enhancements
  - [#141](../../issues/141) "Add Lithuanian (lt) language support"
  - [#142](../../issues/142) "GalleryPanel should inherit from VirtualizingStackPanel instead of Panel"

## 3.4.2

- ### Bug fixes
  - [#113](../../issues/113) "Ribbon starts with no active tab selected"
  - [#122](../../issues/122) "The CanAddToQuickAccessToolBar="False" property not work correctly for "MenuItem""
  - [#135](../../issues/135) "Null pointer exception when trying to bring back QuickAccessItem"
  - [#136](../../issues/136) "Add translation for TradChinese(Taiwan)"
  - When backstage was open while items were added to quick access the items were not immediately visible after closing backstage
  - Show above/below can't be added to quick access from now on
  - Fixing warning that was caused by an not applied storyboard being removed
  - Removed opacity animation from backstage in Office 2013 theme to align with Office 2013

## 3.4.1

- ### Bug fixes
  - [#78](../../issues/78) "Minimized but visible ribbon lacks top border in theme 2013"
  - [#81](../../issues/81) "Invalid typeof in style resolution of RadioButton"
  - [#83](../../issues/83) "Aligned vertical alignment of dropdownbutton and button."
  - [#85](../../issues/85) "ComboBox Items don t get a overlay color when mouse over items"
  - [#95](../../issues/95) "repair nullref combobox combined with focus on winforms control"
  - [#97](../../issues/97) "Fixed possible InvalidCastException when creating a QAT item of a CheckBox"
  - [#102](../../issues/102) "Redundant separator in context menu if quick access is hidden"
  - [#104](../../issues/104) "Non-DWM fullscreen window is not fullscreen"
  - [#106](../../issues/106) "BackStage should be 1 px higher"
  - [#115](../../issues/115) "MenuItem: IsSplited="True" gray line issue"
  - [#117](../../issues/117) "Backstage not shown when Window Content is not a FrameworkElement"
  - [#124](../../issues/124) "The text in fluent:Spinner cannot be centered vertically."

## 3.4.0
- ### Features
  - [#74](../../issues/74) "Added DropDownButton.ClosePopupOnMouseDownDelay to allow the end-developer to tweak the delay (sometimes 100 ms is too fast)"

- ### Enhancements
  - [#67](../../issues/67) "Make pin image vector based"
  - [#76](../../issues/76) "Tabbing between spinners goes via spinner buttons"

- ### Bug fixes
  - [#34](../../issues/34) "A bug in the Menu of DropdownButton"
  - [#45](../../issues/45) "Little style Bug in Office2013 Style"
  - [#61](../../issues/61) "Fix for backstage controls in Windows 8 theme"
  - [#62](../../issues/62) "Review TODO:s in Windows 8 theme"
  - [#63](../../issues/63) "Office2013 Icon fixing"
  - [#65](../../issues/65) "Problems when layout is scaled"
  - [#66](../../issues/66) "RibbonWindow not closed when system menu icon is double clicked"
  - [#68](../../issues/68) "NullReferenceException is thrown on closing by Alt+Space and C"
  - [#69](../../issues/69) "Ribbon Title not update its position"
  - [#71](../../issues/71) "Starting application with backstage menu open causes problems"

## 3.3.0
- ### Reverted changes
  - [#28](../../issues/28) "Height and FontSize of MenuItem"
  - [#31](../../issues/31) "System wide font settings."

- ### Features
  - [#14](../../issues/14) Ribbon Button: when passing the same icon file to the properties Icon and LargeIcon, the control doesn't seem to take the good icon frame
  - [#52](../../issues/52) Windows 8 theme
  - [#60](../../issues/60) First vector images for Windows 8 theme

- ### Bug fixes
  - [#45](../../issues/45) Little style Bug in Office2013 Style
  - [#46](../../issues/46) Little Bug in Office2010 style
  - [#49](../../issues/49) Fluent:RibbonProperties.TitleBarHeight="0" no longer works!
  - [#50](../../issues/50) Blurred Window Icon in Office 2013 Style
  - [#55](../../issues/55) Changing menu font size in Windows mess ups ribbon 
  - [#57](../../issues/57) Slimmer arrow on combo box
  - [#58](../../issues/58) ColorGallery SelectedColor
  - [#59](../../issues/59) Little style bug in Windows 8 theme

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
  - [#3](../../issues/3)   "Consider using GitLink to allow users to step through Fluent source code"

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