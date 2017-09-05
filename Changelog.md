# Changelog for Fluent.Ribbon

## 6.0.0 (preview)

- ### Breaking changes
  - **[#466](../../issues/466) - StrongName signed assembly? Hey, it's 2017...**  
  Fluent.Ribbon is no longer a strong-named assembly.
  - [#433](../../issues/433) - ToggleButton not working correctly when placed in collapsed GroupBox
    `ToggleButton` grouping was done like it's done for a regular `RadioButton`, except that it was bound to visual root scope.  
    The dependency on a visual root scope has been remove, so please adjust the `GroupName` for groups if you have groups with the same name in different visual root scopes.
  - `DropDownButton` (and thus also `SplitButton`) now inherit from `ItemsControl` instead of `MenuBase`.  
    This was changed because `MenuBase` causes a lot of issues regarding keyboard navigation, focus etc..
    This also means that you should use `Fluent:MenuItem` instead of the system provided `MenuItem` as immediate children of `DropDownButton` and `SplitButton`.
  - `SplitButton` now generates two `KeyTip` elements for itself. If you previously used "S" as `KeyTip` there will now be "S1" for the button action and "S2" will open the dropdown.
  - Changes made to `Ribbon`
    - Internal properties for `QuickAccessToolBar` and `TabControl` got converted to `DependencyProperty`
    - Changed order of Children returned from `LogicalChildren`
    - Type of metadata for `Menu` and `StartScreen` changed from `PropertyMetadata` to `FrameworkPropertyMetadata`
  - There are a lot new resources to control the colorization. Please have a look at Colors.xaml for a list of all available resources.  
    This also means that, for example, simply changing the foreground/background of one outer control won't change the foreground/background of all inner controls.
  - [#457](../../issues/457) - Return type of `IKeyTipedControl.OnKeyTipPressed` was changed from `void` to `KeyTipPressedResult`.
  - The following `Color` and `Brush` resources got replaced/renamed/removed:

<details><summary>Click here to show the list of replaced/renamed/removed things</summary><p>

|Old|New|
|---|---|
| Fluent:MetroColors.ThemeColorKey | Fluent.Ribbon.Colors.AccentBaseColor |
| ButtonDisabledBackgroundBrush | --- |
| ButtonDisabledBorderBrush | --- |
| SliderShadowBrush | WhiteBrush |
| SliderLightenBrush | WhiteBrush |
| BackstageBackgroundBrush | WhiteBrush |
| BackstageControlHoverBorderBrush | Fluent.Ribbon.Brushes.Button.MouseOver.BorderBrush |
| BackstageControlActiveBorderBrush | Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush |
| ButtonBorderBrush | Fluent.Ribbon.Brushes.Control.BorderBrush |
| ButtonHoverOuterBackgroundBrush | Fluent.Ribbon.Brushes.Button.MouseOver.Background |
| ButtonHoverOuterBorderBrush | Fluent.Ribbon.Brushes.Button.MouseOver.BorderBrush |
| ButtonPressedOuterBackgroundBrush | Fluent.Ribbon.Brushes.Button.Pressed.Background |
| ButtonPressedOuterBorderBrush | Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush |
| ButtonPressedInnerBorderBrush | Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush |
| ButtonPressedInnerBackgroundBrush | Fluent.Ribbon.Brushes.Button.Pressed.Background |
| ButtonHoverInnerBackgroundBrush | --- |
| ButtonHoverInnerBorderBrush | --- |
| ButtonCheckedBrush | Fluent.Ribbon.Brushes.HighlightBrush & Fluent.Ribbon.Brushes.Button.MouseOver.Background |
| CheckBoxHoverBackgroundBrush | Fluent.Ribbon.Brushes.Button.MouseOver.Background |
| CheckBoxHoverBorderBrush | Fluent.Ribbon.Brushes.Button.MouseOver.BorderBrush |
| CheckBoxPressedBackgroundBrush | Fluent.Ribbon.Brushes.Button.Pressed.Background |
| CheckBoxPressedBorderBrush | Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush |
| CheckBoxOutterBorderBrush | --- |
| CheckBoxOutterBackgroundBrush | --- |
| CheckBoxInnerBorderBrush | --- |
| CheckBoxInnerBackgroundBrush | --- |
| CheckBoxHoverOutterBorderBrush | --- |
| CheckBoxHoverOutterBackgroundBrush | --- |
| CheckBoxHoverInnerBorderBrush | --- |
| CheckBoxHoverInnerBackgroundBrush | --- |
| CheckBoxPressedOutterBorderBrush | --- |
| CheckBoxPressedOutterBackgroundBrush | --- |
| CheckBoxPressedInnerBorderBrush | --- |
| CheckBoxPressedInnerBackgroundBrush | --- |
| ContextMenuLineBrush | --- |
| ContextMenuBarBackgroundBrush | Fluent.Ribbon.Brushes.DropDown.BackgroundBrush |
| ContextMenuBarBorderBrush | Fluent.Ribbon.Brushes.DropDown.BorderBrush |
| ContextMenuBarResizeBorderBrush | Fluent.Ribbon.Brushes.DropDown.Resize.BorderBrush |
| ContextMenuBarResizeBackgoundBrush | Fluent.Ribbon.Brushes.DropDown.Resize.BackgoundBrush |
| GalleryBorderBrush | Fluent.Ribbon.Brushes.Control.BorderBrush |
| InRibbonGalleryBorderBrush | Fluent.Ribbon.Brushes.Control.BorderBrush |
| BackstageGalleryItemHoverBackgroundBrush | Fluent.Ribbon.Brushes.Button.MouseOver.Background |
| BackstageGalleryItemSelectedBackgroundBrush | Fluent.Ribbon.Brushes.Button.Pressed.Background |
| BackstageToggleButtonCheckedBorderBrush | Fluent.Ribbon.Brushes.HighlightBrush |
| BackstageToggleButtonCheckedBackgroundBrush | Fluent.Ribbon.Brushes.Button.MouseOver.Background |
| QuickAccessToolbarPopupBackgrondBrush | {Binding Background, RelativeSource={RelativeSource AncestorType=Window}} |
| WindowContentBorderBrush | Fluent.Ribbon.Brushes.Separator.BorderBrush |
| GroupBoxSeparatorBorderBrush | Fluent.Ribbon.Brushes.Separator.BorderBrush |
| GroupBoxSeparatorBackgroundBrush | Fluent.Ribbon.Brushes.Separator.Background |
| ContextMenuSeparatorBrush | Fluent.Ribbon.Brushes.Separator.BorderBrush |
| KeyTipBackgroundBrush | Fluent.Ribbon.Brushes.KeyTip.Background |
| KeyTipBorderBrush | Fluent.Ribbon.Brushes.KeyTip.BorderBrush |
| GalleryHeaderBackgroundBrush | Fluent.Ribbon.Brushes.Gallery.Header.Background |
| TextBoxBorderBrush | Fluent.Ribbon.Brushes.TextBox.BorderBrush |
| TextBoxBackgroundBrush | Fluent.Ribbon.Brushes.TextBox.Background |
| TextBoxHoverBackgroundBrush | Fluent.Ribbon.Brushes.TextBox.MouseOver.Background |
| TextBoxHoverBorderBrush | Fluent.Ribbon.Brushes.TextBox.MouseOver.BorderBrush |
| TextBoxDisabledBackgroundBrush | Fluent.Ribbon.Brushes.TextBox.Disabled.Background |
| TextBoxDisabledBorderBrush | Fluent.Ribbon.Brushes.TextBox.Disabled.BorderBrush |
| RibbonSeparatorBrush | Fluent.Ribbon.Brushes.GroupSeparator.Background |
| GroupSeparatorBrush | Fluent.Ribbon.Brushes.GroupSeparator.Background |
| CloseButtonHoverBackgroundBrush | Fluent.Ribbon.Brushes.WindowCommands.CloseButton.MouseOver.Background |
| CloseButtonPressedBackgroundBrush| Fluent.Ribbon.Brushes.WindowCommands.CloseButton.Pressed.Background |
| MenuItemBackground | Fluent.Ribbon.Brushes.MenuItem.Background |
| MenuItemCheckBoxBackgroundBrush | Fluent.Ribbon.Brushes.ApplicationMenuItem.CheckBox.Background |
| MenuItemCheckBoxBorderBrush | Fluent.Ribbon.Brushes.ApplicationMenuItem.CheckBox.BorderBrush |
| RibbonThemeColorBrush | Fluent.Ribbon.Brushes.AccentBaseColorBrush |
| TransparentBrush | --- |
| BackstageFontBrush | Fluent.Ribbon.Brushes.IdealForegroundColorBrush |
| TabItemFontBrush | Fluent.Ribbon.Brushes.LabelTextBrush |
| Fluent.Ribbon.Brushes.LabelTextBrush | Fluent.Ribbon.Brushes.LabelTextBrush |
| GroupHoverBrush | Fluent.Ribbon.Brushes.RibbonGroupBox.Collapsed.MouseOver.Background GroupHoverBrush |
| GroupHighlightBrush | Fluent.Ribbon.Brushes.RibbonGroupBox.DropDownOpen.Background |
| GroupBoxFontBrush | Fluent.Ribbon.Brushes.RibbonGroupBox.Header.Foreground |
| ActiveTabBackgroundBrush | Fluent.Ribbon.Brushes.RibbonTabItem.Active.Background |
| TabItemSelectedFontBrush | Fluent.Ribbon.Brushes.RibbonTabItem.Selected.Foreground |
| RibbonBackgoundBrush | Fluent.Ribbon.Brushes.Ribbon.Background |
| RibbonTopBorderBrush | Fluent.Ribbon.Brushes.RibbonTabItem.BorderBrush & Fluent.Ribbon.Brushes.ColorGallery.Item.BorderBrush |
| ScrollButtonDefaultBorderBrush | Fluent.Ribbon.Brushes.ScrollButton.Default.BorderBrush |
| ScrollButtonDefaultBackgroundBrush | Fluent.Ribbon.Brushes.ScrollButton.Default.Background |
| ScrollButtonHoverBorderBrush | Fluent.Ribbon.Brushes.ScrollButton.Hover.BorderBrush |
| ScrollButtonHoverBackgroundBrush | Fluent.Ribbon.Brushes.ScrollButton.Hover.Background |
| ScrollButtonPressedBorderBrush | Fluent.Ribbon.Brushes.ScrollButton.Pressed.BorderBrush |
| ScrollButtonPressedBackgroundBrush | Fluent.Ribbon.Brushes.ScrollButton.Pressed.Background |
| ScrollBackgroundBrush | Fluent.Ribbon.Brushes.ScrollBar.Background |
| ScrollVerticalBackgroundBrush | Fluent.Ribbon.Brushes.ScrollBar.Background |
| ScrollThumbDefaultBorderBrush | Fluent.Ribbon.Brushes.ScrollThumb.Default.BorderBrush |
| ScrollThumbDefaultBackgroundBrush | Fluent.Ribbon.Brushes.ScrollThumb.Default.Background |
| ScrollThumbHoverBorderBrush | Fluent.Ribbon.Brushes.Button.Hover.BorderBrush |
| ScrollThumbHoverBackgroundBrush | Fluent.Ribbon.Brushes.Button.Hover.Background |
| ScrollThumbPressedBorderBrush | Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush |
| ScrollThumbPressedBackgroundBrush | Fluent.Ribbon.Brushes.Button.Pressed.Background |
| ScrollViewerButtonBorderBrush | Fluent.Ribbon.Brushes.ScrollViewer.Button.BorderBrush |
| ScrollViewerButtonBackgroundBrush | Fluent.Ribbon.Brushes.ScrollViewer.Button.BackgroundBrush |
|  |  |

  - The following default values changed:

|Name/Location|Old|New|
|---|---|---|
| Button.BorderThickness | 0 | 1 |
| ContentGapHeight | 5 | 1 |

  - The following styles got removed/renamed:

|Old|New|
|---|---|
| NonEditableComboBoxItemStyle | is now the default style |
| EditableComboBoxItemStyle | --- |
</p></details>

- ### Bug fixes
  - [#344](../../issues/344) - Invisible header of combobox on QuickAccessToolBar.
  - [#395](../../issues/395) - issues with contextual tab group
  - [#405](../../issues/405) - InRibbonGallery when loaded, mouse wheel up or down cause crash.  (thanks @Yumeryu)
  - [#419](../../issues/419) - BackstageTabItem Is Never Deselected if BackstageTabControl is not Logical Parent
  - [#428](../../issues/428) - Values from DependencyProperties with FrameworkPropertyMetadataOptions.Inherits are not properly inherited from Ribbon to Menu, StartScreen and some other children  
    This also overrules the fix made for [#415](../../issues/415).
  - [#430](../../issues/430) - No Rezising at Fluent:RibbonContextualTabGroup at Version 5.0
  - [#433](../../issues/433) - ToggleButton not working correctly when placed in collapsed GroupBox
  - [#438](../../issues/438) - Titlebar icon missing left margin when maximized
  - [#439](../../issues/439) - Context menu and submenu disappears after a right click
  - [#446](../../issues/446) - Wrong submenu Position in application menu
  - [#450](../../issues/450) - Fix Ribbon.OnTitleBarChanged clearing the new title bar instead of the old one (thanks @Cubey2019)
  - [#451](../../issues/451) - combobox and dateformat
  - [#454](../../issues/454) - RibbonWindow title not shown in correct position when using SizeToContent
  - [#456](../../issues/456) - Maximising a RibbonWindow with MaxWidth or MaxHeight causes an extra border above the title bar
  - [#457](../../issues/457) - Using the keytip shortcut to open a backstage briefly opens it, then closes it immediately
  - [#459](../../issues/459) - Label of the Spinner disappears, if there isn't enough place
  - [#463](../../issues/463) - ShowQuickAccessToolBarAboveRibbon="False" causes crash
  - [#464](../../issues/464) - Clicking on hyperlink inside of ApplicationMenu causes crash
  - [#465](../../issues/465) - Clicking on ContextualGroup background or scrolling through ribbon will open disabled tab  
    During this fix `RibbonTabControl.GetFirstVisibleItem` was renamed to `RibbonTabControl.GetFirstVisibleAndEnabledItem`.
  - [#467](../../issues/467) - Fluent:SplitButton IsEnabled

- ### Enhancements
  - `LayoutTransform` and `RenderTransform` can now be used directly on `RibbonWindow` as this now gets forwarded to the first template child of the window. Have a look at `TestWindow` in the showcase application for an example on how to use it. This was added as the fix for [#430](../../issues/430).
  - You can change accent and base colors by using `ThemeManager` just like in MahApps.Metro. Please read the [documentation](http://fluentribbon.github.io/documentation/styles) for details.
  - [#275](../../issues/275) - Option for customizing the quick access toolbar access keys  
  `QuickAccessToolBar` now has a property called `UpdateKeyTipsAction` which accepts an instance of `QuickAccessToolBar` through which you can create your own keytips for items in the toolbar.  
  Look at the UnitTest `QuickAccessToolBarTests.TestCustomKeyTips` for a sample.
  - [#313](../../issues/313) - Window state buttons not updating to Windows 10 look / feel
  - [#417](../../issues/417) - Keytips for Splitbuttons  
  You now get 2 KeyTips for `SplitButton`. One for the main action button and one for the dropdown. Those KeyTips get generated by append "A" and "B" to the original `KeyTip`.
  - [#431](../../issues/431) - Changing Ribbon Tab Control Background color and possibly adding Graphic Styling.
  - [#435](../../issues/435) - Add ability to change the Window Title Background the Ribbon Bar is on in order to match the Office 365 Style.
  - [#440](../../issues/440) - Enable changing the height of content area of RibbonTabControl  
  `ContentHeight` is available on `Ribbon` and tranferred to `RibbonTabControl` from there
  - [#443](../../issues/443) - Release .NET 4.6.2 version
  - [#444](../../issues/444) - Add `IgnoreTaskbarOnMaximize` to `RibbonWindow`
  - [#445](../../issues/445) - Startsceen "shines" through backstage
  - [#455](../../issues/455) - Add brushes for checked toggle buttons (thanks @Cubey2019)

## 5.0.2
  - [#437](../../issues/437) - "Could not load ControlzEx"

## 5.0.1
- ### Bug fixes
  - [#412](../../issues/412) - Specific version for nuget references causes issues
  - [#413](../../issues/413) - ColorGallery SelectedColorChanged issue
  - [#415](../../issues/415) - DataContext not forwarded from Ribbon to Backstage (in adorner) and TitleBar

## 5.0.0

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
  - [#400](../../issues/400) - Default menu / menuitem appearance overridden by FluentRibbon
    - Styles for the following types are no longer overwritten globally `ContextMenu`, `MenuItem`, `MenuItem.SeparatorStyleKey`, `StatusBar`, `StatusBarItem`, `StatusBar.SeparatorStyleKey`
    - `MenuItemStyle` is renamed to `FluentDefaultSystemMenuItemStyle`

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
  - [#388](../../issues/388) - QuickAccess item not visible when added from backstage
  - [#391](../../issues/391) - QAT items not displayed properly if tab is made visible after initialization
  - [#392](../../issues/392) - Change Window Title doesn't update Title length
  - [#397](../../issues/397) - Backstage Opening Animation
  - [#398](../../issues/398) - ColorGallery: SelectedColorChanged event raised before SelectedColor changed
  - [#403](../../issues/403) - Tabs can "detach" from ribbon area

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
  - [#387](../../issues/387) - Disable/Hide the Menu-DropDown in QuickAccessToolbar
    - `Ribbon` now has `IsQuickAccessToolBarMenuDropDownVisible` and `QuickAccessToolbar` now has `IsMenuDropDownVisible`.
  - Added `CanChangeIsOpen` to `Backstage`
  - Added `ActiveTabBackground` and `ActiveTabBorderBrush` to `RibbonTabItem`
  - Improved interop with windows which are not of type `RibbonWindow`.  
    The showcase contains a sample showcasing working context-tabs in a MahApps.Metro `MetroWindow`.

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
## Major changes
* Office 2013 theme is now included
* MVVM support got better (yet not complete, please file a bug for things you need to work)
* Samples are not divided anymore (all features shown there are now present in the showcase application)
* All resource reference are now of type "DynamicResource" see comments for changeset https://fluent.codeplex.com/SourceControl/changeset/3572af781b96
* We now use the WindowChrome class provided by Microsoft instead of custom code to render in the non client area
* Large amount of [fixed bugs](https://fluent.codeplex.com/workitem/list/advanced?keyword=&status=Resolved%7cClosed&type=All&priority=All&release=All&assignedTo=All&component=All&reasonClosed=Fixed&sortField=LastUpdatedDate&sortDirection=Descending&page=0)
* Style resources have been improved. That means you now have to include "Themes/Generic.xaml" to get Office 2010 silver and you can just import "Themes/Office2010/Black.xaml" etc. afterwards to get the different colors. To use the Office 2013 theme you can omit "Themes/Generic.xaml" and include "Themes/Office2013/Generic.xaml" instead. You can use RibbonWindow for Office 2010 or Office 2013 themes and you can switch between those at runtime. The showcase application shows how you can do that.

## Breaking changes
* No control in this library sets IsFocusScope=True anymore. This means that ApplicationCommands (such as paste, cut or copy), when bound to a button, don't get enabled when you would expect them to be activated. The showcase application uses those buttons with IsFocusScope=True.