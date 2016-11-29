# Changelog for Fluent.Ribbon

## 5.0.0 (preview)

- ### Breaking changes
  - Office 2010 and Windows 8 themes got removed. 
    Office 2013 theme was moved from "Themes/Office2013/Generic.xaml" to "Themes/Generic.xaml".
    This was a [community voted decision](../../issues/282).
  - `ComboBox` now uses the default `ItemsPanelTemplate` instead of `VirtualizingStackPanel` again.  
    If you prefer to use `VirtualizingStackPanel` you can change this on a per instance level or add it to your default `Style`.
  - [#301](../../issues/301) - Remove Office 2010 and Windows 8 themes
  - [#302](../../issues/302) - Rename Office 2013 theme to Generic
  - [#309](../../issues/309) - Remove grouping from ComboBox and make GalleryPanel inherit from StackPanel
  - [#310](../../issues/310) - Refactoring of localization
  - [#349](../../issues/349) - Move `RibbonTitleBar` from `Ribbon` to `RibbonWindow`
    - `RibbonWindow` exposes `TitleBar` which is of type `RibbonTitleBar`.  
      `Ribbon` uses the `TitleBar` provided by the `RibbonWindow` by default, through it's `Style`.  
      This allows changing the `TitleBar` being used by the `Ribbon`.
  - Removed `RibbonProperties.TitleBarHeight` to simplify usage of the `Ribbon` in non `RibbonWindow`.  
    To support this the following things were added: 
    - `SelectedContentMargin` was added to `BackstageTabControl`
    - `QuickAccessToolBarHeight` on `Ribbon`
    - `TitleBarHeight` on `RibbonWindow`

- ### Bug fixes
  - [#288](../../issues/288) - Fluent:DropDownButton Background can't be changed
  - [#300](../../issues/300) - Startscreen not working in 4.0.3
  - [#305](../../issues/305) - Combobox items will be drawn above each other on scroll down
  - [#311](../../issues/311) - State loading too verbose
  - [#315](../../issues/315) - Menu button gets blurry
  - [#325](../../issues/325) - Running RibbonWindow not on the main thread.
  - [#329](../../issues/329) - ArgumentOutOfRangeException when adding StatusBarItem
  - [#333](../../issues/333) - NullReference Exception app.Run
  - [#339](../../issues/339) - SelectedTabChanged fired when selection of contained Selector changes
  - [#341](../../issues/341) - Fix text going under custom window commands (thanks @chrfin)
  - [#342](../../issues/342) - Fixed dependency property name of property Groups in GalleryGroupFilter. (thanks @polepage)
  - [#346](../../issues/346) - RibbonWindow icon is blurry (thanks @chdft)
  - [#365](../../issues/365) - BackstageAdorner is not removed
  - [#368](../../issues/368) - Render Glitch when Maximizing RibbonWindow with SizeToContent Enabled 
  - [#369](../../issues/369) - "Restore" button in title bar not displayed correctly when application starts maximized
  - [#377](../../issues/377) - ToggleButton highlight doesn't match the button size

- ### Enhancements
  - [#250](../../issues/250) - Enable change/hide of window title foreground
    - `Foreground` of window titel can now be changed by setting `TitleForeground` on `RibbonWindow`.  
    This is bound to `Foreground` of `RibbonWindow`.
  - [#276](../../issues/276) - Custom method for saving and restoring QAT items
  - [#279](../../issues/279) - Localization of ColorGallery
  - [#299](../../issues/299) - Quick access items should show item text as tool tip if no tooltip is set
  - [#324](../../issues/324) - Add "IsSeparatorVisible" to RibbonGroupBox (thanks to @maurosampietro)
  - [#326](../../issues/326) - Add interface for controls which provide LargeIcon
  - [#334](../../issues/334) - Select all text in Spinner on focus
  - [#340](../../issues/340) - Expose GroupByAdvanced from GalleryPanel on Gallery and InRibbonGallery
  - [#360](../../issues/360) - Toggling of KeyTips should not happen when Shift is pressed (thanks to @stylefish)
  - Added `CanChangeIsOpen` to `Backstage`
  - Added `ActiveTabBackground` and `ActiveTabBorderBrush` to `RibbonTabItem`

## 4.0.3

- ### Bug fixes
  - Fixes issues with window border on Windows 7 and Windows 8

## 4.0.2

- ### Bug fixes
  - [#294](../../issues/294) - Issues with default style and OverridesDefaultStyle

## 4.0.1

- ### Bug fixes
  - [#293](../../issues/293) - ArgumentNullException in BackstageTabControl while trying to find the selected tab

## 4.0.0

- ### Notices
  - **As of now the Office 2010 and Windows 8 themes will be removed in version 5.0. You can vote for this at [Future direction of this library](../../issues/282).**

- ### Breaking changes
  - New nuget dependency: [ControlzEx](https://www.nuget.org/packages/ControlzEx). So don't forget to add ControlzEx.dll to your distribution/setup.
  - BorderlessWindowBehavior was replaced by WindowChromeBehavior from [ControlzEx](https://github.com/ControlzEx/ControlzEx).
    This also removes the dependency Microsoft.Windows.Shell as ControlzEx contains everything we need to use WindowChrome.
    This behavior is initialized in code behind (InitializeWindowChromeBehavior) and shows which properties of RibbonWindow can be used to control the behavior.
  - SaveWindowPosition and WindowSettingBehavior were removed [#196](../../issues/196)
  - GlassBorderThickness was renamed to GlassFrameThickness to be consitent with WindowChrome and WindowChromeBehavior [#209](../../issues/209)
  - FluentTest project was renamed to Fluent.Ribbon.Showcase [#212](../../issues/212)

- ### Development/Contributing changes
  - We switched to Visual Studio 2015 so we can use nameof etc. [#219](../../issues/219)

- ### Bug fixes
  - [#10](../../issues/10) - Maximising a ribbon window with DWM enabled results in it not maximising properly
  - [#90](../../issues/90) - Window chrome turning black during window resize.
  - [#116](../../issues/116) - MenuItem: When the description is long the MenuItem does not expands in Width or Height, does not wrap text
  - [#129](../../issues/129) - DontUseDwm="True" case the window corner unpainted
  - [#146](../../issues/146) - Rendering of Office 2010 theme incorrect on Windows 10
  - [#191](../../issues/191) - Windows 8 Theme - Blue/black margin around the window in windows 10
  - [#210](../../issues/210) - Fix wrong dimensions assumed for WindowCaptionButtons on Windows 10 in Office2010 and Windows 8 themes
  - [#218](../../issues/218) - Not displaying properly when maximized
  - [#221](../../issues/221) - Office2013 theme undesired space between tabItem and its content when showing unpinned
  - [#223](../../issues/223) - RibbonContextualTabGroup ignores Window.ResizeMode
  - [#226](../../issues/226) - Maximising a ribbon window on Win7 "Non-Aero" Mode causes rendering issues
  - [#228](../../issues/228) - Backstage disappears when changing DontUseDwm
  - [#234](../../issues/234) - Disable KeyTips when Ribbon isn't Enabled
  - [#235](../../issues/235) - Items of invisble tab are shown
  - [#236](../../issues/236) - Window size is wrong when window is maximized, backstage is closed and window should be restored to normal size
  - [#237](../../issues/237) - Ribbon Buttons with large text cut off a few pixels
  - [#238](../../issues/238) - New WindowChrome does not work correctly on Windows 7
  - [#240](../../issues/240) - Backstage closes when popup is dismissed inside backstage
  - [#241](../../issues/241) - Keytips should be cancelled if Alt+Num0 is pressed
  - [#244](../../issues/244) - KeyTip not working for childs of ContentPresenter
  - [#246](../../issues/246) - Bind RibbonGroupBox.DataContext on QuickAccessToolBar (thanks to @nishy2000)
  - [#251](../../issues/251) - Changing RibbonStatusBar height to 23 and RibbonStatusBarItem foreground to BackstageFontBrush (thanks to @maurosampietro)
  - [#253](../../issues/253) - Icons of buttons not aligned correctly in Office 2013 theme (thanks to @floele-sp)
  - [#254](../../issues/254) - Basic fix for KeyTips not working when focus is inside a WinForms control
  - [#256](../../issues/256) - ComboBox items don't update properly on ItemsSource binding source collection changes
  - [#255](../../issues/255) - Submenus don't show scroll viewer if items exceed the available space on screen (thanks to @floele-sp)
  - [#257](../../issues/257) - Windows8 RibbonWindowTitleTextGlowBackground was missing (thanks to @maurosampietro)
  - [#263](../../issues/263) - Changing theme from backstage is broken
  - [#267](../../issues/267) - maximizing in Win 10
  - [#269](../../issues/269) - Show underscore of header text on RibbonTabItem
  - [#272](../../issues/272) - Changing RibbonThemeColorBrush does not change background of ItemsPanel in Backstage
  - [#274](../../issues/274) - RadioButton Icon and LargeIcon
  - [#280](../../issues/280) - Keytips of the Ribbon overlay StartScreen
  - [#284](../../issues/284) - Overriding width of button does not work as it should
  - [#285](../../issues/285) - MaterialDesign DialogHost issue with FluentRibbon
  - [#291](../../issues/291) - BackStageTabItem leftmousedown event not firing
  - OpenBackstage command was not acting on the correct backstage in a multiple backstage scenario (thanks to @maurosampietro)

- ### Enhancements
  - [#120](../../issues/120) - Adding short-cuts or additional information to Application Menu Item
  - [#185](../../issues/185) - Major refactoring of how WindowChrome is used
  - [#194](../../issues/194) - There should be an option to disable animations in the whole control
  - [#205](../../issues/205) - Fluent Spinner handles Format="P0" incorrectly.
  - [#207](../../issues/207) - Enable DragMove on unused RibbonTabControl space like in Office 2013
  - [#230](../../issues/230) - Option to disable the "Minimize"-Ribbon Button & Behavior (thanks to @robertmuehsig)
  - [#242](../../issues/242) - Add start screen like in office 2013 and upwards
  - [#258](../../issues/258) - Refactoring of KeyTipService and KeyTipAdorner (merged with [#264](../../issues/264))

## 3.6.1

- ### Bug fixes

  - [#29](../../issues/29) - Backstage flicker
  - [#112](../../issues/112) - Backstage flicker
  - [#192](../../issues/192) - Background of RibbonGroupBox can't be set/changed
  - [#197](../../issues/197) - Some adaptions to better align with Office 2013
  - [#200](../../issues/200) - KeyTips in SplitButton no longer work
  - [#201](../../issues/201) - Cannot change Fluent.Button Icon Dynamically
  - [#202](../../issues/202) - Center the button labels in Office 2013 theme
  - [#206](../../issues/206) - Center alignment button inside RIbbonGroupBox
  - [#214](../../issues/214) - Assign a StringFormat binding to a RibbonGroupBox Header

Commits: [v3.6.0...v3.6.1](../../compare/v3.6.0...v3.6.1)

## 3.6.0

- ### Bug fixes
  - [#163](../../issues/163) - ColorGallery produces Binding errors because of Binding of Color on SolidColorBrush in Fill/Background
  - [#166](../../issues/166) - Type is mismatch for IsCheckedProperty of ToggleButton
  - [#170](../../issues/170) - Black RibbonWindow title bar
  - [#173](../../issues/173) - Binding Observable Collection to InRibbonGallery causes Layout Error
  - [#178](../../issues/178) - Ribbonwindow crashes

- ### Enhancements
  - [#172](../../issues/172) - ToggleButtonHelper.OnIsCheckedChanged() accepts type of "bool?". contributed by ([nishy2000](https://github.com/nishy2000))
  - [#175](../../issues/175) - Add "CloseOnEsc" property to Backstage to disable closing the backstage when ESC is pressed. contributed by Christoph Fink ([chrfin](https://github.com/chrfin))

Commits: [c7151027f5...97001b17e9](../../compare/c7151027f5...97001b17e9)

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