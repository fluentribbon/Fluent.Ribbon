# Changelog for Fluent.Ribbon

## 7.0.0 (preview)
- ### Breaking changes
  - [#471](../../issues/471) - **Drop support for .Net 4.0**
  - Reverted [#466](../../issues/466) - **StrongName signed assembly?**  
  **Assemblies are strong named again**, but `AssemblyVersion` is now fixed for every major release.  
  This means that, for example, for version `7.1` the `AssemblyVersion` will still be `7.0`.  
  Other versions like `AssemblyFileVersion` etc. won't be fixed.
  - [#515](../../issues/515) - Switch to new csproj format and require VS2017 for development
  - [#545](../../issues/545) - SplitButton.IsChecked is not bound 2 way by default
  - Due to [#549](../../issues/549) `RibbonGroupBox.Header` is now of type object and `RibbonGroupBox` now inherits from `HeaderedItemsControl` instead of `ItemsControl`.  
  Please have a look at the [documentation](http://fluentribbon.github.io/documentation/controls/ribbon-group-box#customizingHeader) for details.
  - Removed `CornerRadius` everywhere except in all controls related to `ApplicationMenu`.
  - Default `Foreground` for `ComboBox` changed from `Inherited` to `Fluent.Ribbon.Brushes.LabelTextBrush`.
  - Default `VerticalContentAlignment` for `ComboBox`, `Spinner` and `TextBox` changed from `Top` to `Center`.
  - Key tips can not be opened by pressing `Space` anymore.
  - [#574](../../issues/574) - Move backstage animations from BackstageTabControl to Backstage  .
  - `Backstage.IsOpenAnimationEnabled` got renamed to `Backstage.AreAnimationsEnabled`. This also applies to `StartScreen`.
  - `Backstage.HideAnimationDuration` got removed. This also applies to `StartScreen`.
  - Default value of dependency property `Backstage.HideContextTabsOnOpen` is now `true`. The default value from style was already set to `true`. So this should only be a breaking change for you if you did not use the default style for `Backstage`.
  - Default value for `Foreground` of `RibbonTabItem` is now `Fluent.Ribbon.Brushes.RibbonTabItem.Foreground` instead of `Fluent.Ribbon.Brushes.RibbonTabItem.Selected.Foreground` because trigger order was fixed in [#578](../../issues/578).
  - `GlassFrameThickness` was removed in favor of the new `GlowWindowBehavior`. Thus you can now use `GlowBrush` and `NonActiveGlowBrush` on `RibbonWindow`. The resize border now also works "outside" of the window.  
    This fixes [#307](../../issues/307), [#319](../../issues/319) and [#556](../../issues/556).
  - The default value for `TabItemSelectedForeground` on `RibbonContextualTabGroup` is now bound to `Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemSelectedForeground`. Previously this was bound to `Foreground` of `RibbonContextualTabGroup`.
  - The default value for `TabItemMouseOverForeground` on `RibbonContextualTabGroup` is now bound to `Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemMouseOverForeground`. Previously this was bound to `Fluent.Ribbon.Brushes.HighlightBrush` or `BlackBrush`.
  - The default value for `Fluent:RibbonProperties.MouseOverForeground` on `RibbonTabItem` is now bound to `Fluent.Ribbon.Brushes.RibbonTabItem.MouseOver.Foreground`. Previously this was bound to `Fluent.Ribbon.Brushes.HighlightBrush` or `BlackBrush`.
  - [#596](../../issues/596) - Breaking changes in theming  
    The theming got rewritten this means that there is no longer a separation between `AppTheme` and `Accent`. These got replaced by `Theme`.  
    Instead of something like `pack://application:,,,/Fluent;component/Themes/Colors/BaseLight.xaml` and `pack://application:,,,/Fluent;component/Themes/Accents/Cobalt.xaml` you now have to use `pack://application:,,,/Fluent;component/Themes/Themes/Light.Cobalt.xaml`.
    The words `AppTheme` and `Accent` are replaced by `Theme` everywhere.
  - The target type for `Fluent.Ribbon.Styles.MenuItem` (previously named `FluentDefaultSystemMenuItemStyle`) now is `Fluent:MenuItem` instead of `MenuItem`
  - Instead of depending on `System.Windows.Interactivity` we now depend on the open source version `Microsoft.Xaml.Behaviors.Wpf`
  - [#650](../../issues/650) - Create XAML icons and replace the shipped pngs with these (thanks @DenZuck for creating the xaml icons)  
    Due to this change all shipped images are now of type `DrawingImage` instead of `BitmapImage`.
  - The filename inside `IsolatedStorageFile` for the default `RibbonStateStorage` is now "Fluent.Ribbon.State." + Hex value of MD5 instead of "Fluent.Ribbon.State.2.0." + hex value of `GetHashCode`. This was done because `GetHashCode` does not return a stable value, especially on .NET core 3.0.

**The following resources were added, renamed or removed ("---" indicates added when in column "Old" and removed when in column "New"):**

<details><summary>Click here to show the list of renamed and removed things</summary><p>

|Old|New|
|---|---|
| BackstageButtonStyle | Fluent.Ribbon.Styles.BackstageTabControl.Button |
| FluentDefaultSystemMenuItemStyle | Fluent.Ribbon.Styles.MenuItem |
| BackstageButtonControlTemplate | Fluent.Ribbon.Templates.BackstageTabControl.Button |
| BackstageSeparatorTabItemStyle | Fluent.Ribbon.Styles.BackstageTabControl.SeparatorTabItem |
| ComboBoxBackstageStyle | Fluent.Ribbon.Styles.Backstage.ComboBox |
| ComboBoxItemBackstageControlTemplate | --- |
| MetroComboBoxItemBackstageStyle | --- |
| ComboBoxBackstageControlTemplate | --- |
| ButtonBackstageStyle | Fluent.Ribbon.Styles.Backstage.Button |
| ButtonBackstageControlTemplate | --- |
| ToggleButtonBackstageStyle | Fluent.Ribbon.Styles.Backstage.ToggleButton |
| ToggleButtonBackstageControlTemplate | --- |
| DropDownButtonBackstageStyle | Fluent.Ribbon.Styles.Backstage.DropDownButton |
| DropDownButtonBackstageControlTemplate | --- |
| --- | Fluent.Ribbon.Brushes.TextBox.CaretBrush |
| --- | Fluent.Ribbon.Brushes.TextBox.SelectionBrush |
| DialogLauncherButtonKeyTipKeysProperty | LauncherKeysProperty |
| OnCanAddToQuickAccessToolbarChanged | OnCanAddToQuickAccessToolBarChanged |
| OnIsOpenTrueStoryboard | Fluent.Ribbon.Storyboards.Backstage.IsOpenTrueStoryboard |
| OnIsOpenFalseStoryboard | Fluent.Ribbon.Storyboards.Backstage.IsOpenFalseStoryboard |
| --- | Fluent.Ribbon.Brushes.RibbonWindow.TitleBackground |
| --- | Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemSelectedForeground |
| --- | Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemMouseOverForeground |
| --- | Fluent.Ribbon.Brushes.RibbonContextualTabGroup.TabItemSelectedMouseOverForeground |
| --- | Fluent.Ribbon.Brushes.RibbonTabItem.MouseOver.Foreground |
| --- | Fluent.Ribbon.Brushes.RibbonTabItem.Selected.MouseOver.Foreground |
| --- | Fluent.Ribbon.Brushes.Backstage.Background |
| --- | Fluent.Ribbon.Brushes.Backstage.Foreground |
| --- | Fluent.Ribbon.Brushes.BackstageTabControl.Button.MouseOver.Background |
| --- | Fluent.Ribbon.Brushes.BackstageTabItem.Header.Foreground |
| --- | Fluent.Ribbon.Brushes.BackstageTabItem.MouseOver.Background |
| --- | Fluent.Ribbon.Brushes.BackstageTabItem.Selected.Background |
| --- | Fluent.Ribbon.Brushes.Backstage.BackButton.Background |
| --- | Fluent.Ribbon.Brushes.Backstage.BackButton.Foreground |
| --- | Fluent.Ribbon.Brushes.BackstageTabControl.ItemsPanelBackground |
| --- | Fluent.Ribbon.Brushes.RibbonWindow.TitleForeground |
</p></details>

- ### Bug fixes
  - [#165](../../issues/165) - Save As menu is added to QAT but does not have child items
  - [#307](../../issues/307) - Black flicker on complete window-area when resizing
  - [#319](../../issues/319) - How to make window resizable with Win32 content?
  - [#535](../../issues/535) - BorderBush on bottom of RibbonTabItem (and Ribbon)
  - [#536](../../issues/536) - RibbonContextualTabGroup header text trimmed until hovered
  - [#542](../../issues/542) - InRibbonGallery not reducing properly
  - [#543](../../issues/543) - Using images that can't be found during design time crashes designer  
  A generic "error" image is rendered during design time and an exception is thrown during runtime.
  - [#551](../../issues/551) - "Auto" size for ribbon group box header to support custom font sizes (thanks @chrfin)
  - [#552](../../issues/552) - RibbonGroupBox should resize when font family or size are changed
  - [#556](../../issues/556) - Wrong Window Resize-Border Sensitivity
  - [#562](../../issues/562) - Pressing "right" arrow key to open submenu on menuitem causes NullRef exception when there is no submenu
  - [#564](../../issues/564) - Gallery overflow panel (with menu items) doesn't close when clicking once in the application
  - [#572](../../issues/572) - KeyTip.Keys Position
  - [#573](../../issues/573) - Empty context menu on controls and ribbon
  - [#576](../../issues/576) - Does RibbonGroupBox set Foreground invalid?
  - [#581](../../issues/581) - StackOverflow Exception when trying to access ApplicationMenu while RibbonMenu is minimized
  - [#586](../../issues/586) - BackstageTabItem IsEnabled=False still displays content
  - [#587](../../issues/587) - DisplayMemberPath no longer working on DropDownButton/MenuItem as of version 6.0
  - [#593](../../issues/593) - Disable state selected in InRibbonGallery after click
  - [#594](../../issues/594) - Keep title in the same location when opening the backstage
  - [#602](../../issues/602) - Pin button not clickable when Ribbon in collapsed state
  - [#607](../../issues/607) - Submenu contained in DropDownButton closes too slow
  - [#616](../../issues/616) - ContextMenu auto hidden after right click (related to [#439](../../issues/439))
  - [#632](../../issues/632) - Ribbon sometimes clips over other applications
  - [#637](../../issues/637) - Escape key doesn't close menu on data-bound DropDownButton
  - [#638](../../issues/638) - Setting AreTabHeadersVisible="False" on startup makes entire ribbon disappear
  - [#639](../../issues/639) - Group headers take focus when tabbing through with keyboard
  - [#653](../../issues/653) - Incorrect context menu of Fluent:TextBox
  - [#656](../../issues/656) - Backstage icons not showing
  - [#659](../../issues/659) - Fix Dutch localization errors (thanks @carloslubbers)
  - [#660](../../issues/660) - ContextualTabs visibility problem
  - [#662](../../issues/662) - Backstage/StartScreen closing if clicking outside of Application
  - [#663](../../issues/663) - Fluent:ApplicationMenu not closing on outside click after opening context menu.
  - [#666](../../issues/666) - InRibbonGallery DropDown not layouting correctly
  - [#673](../../issues/673) - RibbonContextualTabGroup not shown
  - [#677](../../issues/677) - Alt Codes no longer working in alpha version v7.0.0
  - [#688](../../issues/688) - Backstage and StartScreen closing when pressing Alt
  - [#698](../../issues/698) - Submenus in the application menu are not opened each time
  - [#704](../../issues/704) - CheckBox.Header - InvalidCastException
  - [#714](../../issues/714) - ResizeMode="NoResize" and ShowInTaskbar="False" causes crash on startup
  - [#722](../../issues/722) - NullReferenceException in KeyTipService.OnAdornerChainTerminated

- ### Enhancements/Features
  - [#516](../../issues/516) - Add options to hide the row containing RibbonTabItems  
    You can achieve this by:
    - Setting `Ribbon.Menu` to `null` (or never assigning anything)
    - Setting `Ribbon.CanMinimize` to `false`
    - Setting `Ribbon.AreTabHeadersVisible` to `false`
    - Setting `Ribbon.IsToolBarVisible` to `false`
  - [#533](../../issues/533) - Issue when using templated ribbon items
  - [#544](../../issues/544) - Add proper DPI support for icons/images aquired through ObjectToImageConverter on .NET 4.6.2
  - [#549](../../issues/549) - Implement RibbonGroupBox header template
  - [#553](../../issues/553) - Introduce resources for CaretBrush and SelectionBrush for TextBox
  - [#554](../../issues/554) - No Keytips on templated ribbon items.
  - [#563](../../issues/563) - Add customizable keys for activating the key tips. (thanks @pschimmel)  
  You can now set your own keys for showing key tips. Have a look at `Ribbon.KeyTipKeys`.
  - [#568](../../issues/568) - Allow setting the height of GalleryPanel inside InRibbonGallery  
  You can now control the height of the `GalleryPanel` inside `InRibbonGallery` by setting `GalleryPanelContainerHeight`.
  - [#578](../../issues/578) - Theming of selected context ribbon tab  
  You can now use `Fluent.Ribbon.Brushes.RibbonWindow.TitleBackground`, `RibbonContextualTabGroup.TabItemSelectedForeground` and `RibbonContextualTabGroup.TabItemMouseOverForeground` to further control colors.
  - [#590](../../issues/590) - SplitButton custom KeyTip  
  You can now use `PrimaryActionKeyTipPostfix` and `SecondaryActionKeyTipPostfix` on `SplitButton` to control the postfix for key tips.
  - [#592](../../issues/592) - Disable context menu on Ribbon  
  You can now use `IsDefaultContextMenuEnabled` on `Ribbon` to disable the default context menu.
  - [#599](../../issues/599) - MahApps.Metro dialog on backstage  
  You can now use `UseHighestAvailableAdornerLayer` on `Backstage` to improve interop with MahApps.Metro regarding dialogs above the backstage.
  - [#606](../../issues/606) - added non-generated Colorful.Blue and Colorful.Gray themes (thanks @stylefish)
  - [#635](../../issues/635) - Quick access menu arrow customization
  - [#640](../../issues/640) - Narrator doesn't read out button headers
  - [#642](../../issues/642) - Ignore Alt Gr key, by blacklisting modifier keys, in KeyTips detection (thanks @stylefish)
  - [#692](../../issues/692) - Add dedicated secondary KeyTip on SplitButton  
  You can now use `SecondaryKeyTip` on `SplitButton` for the secondary key tip. If `KeyTip` is empty or null only the `SecondaryKeyTip` will be used.
  - [#696](../../issues/696) - Adding Greek language translations (thanks @b-karamichael)

## 6.1.0

- ### Bug fixes
  - [#510](../../issues/510) - Submenus in DropDownButton are not opened each time
  - [#511](../../issues/511) - Binding on RibbonWindow.Icon not working
  - [#512](../../issues/512) - Ideal text color should match the colors in the ribbon
  - [#513](../../issues/513) - Hovering causes flickering of ribbon backstage menu items
  - [#517](../../issues/517) - Fluent 6.0: buttons inside drop downs don't seem to work when using ClosePopupOnMouseDown
  - [#524](../../issues/524) - Fixes Korean translation error. (thanks @softinus)

- ### Enhancements
  - [#509](../../issues/509) - Checked mark + Icon image at Menuitem
  - [#514](../../issues/514) - Detect Windows 10 app mode setting and adjust current AppTheme automatically  
  You can use `ThemeManager.SyncAppThemeWithWindowsAppModeSetting` to align the `AppTheme` once.
  You can use `ThemeManager.IsAutomaticWindowsAppModeSettingSyncEnabled` to align the `AppTheme` automatically when the Windows setting is changed during runtime.
  - [#518](../../issues/518) - Replace Thread.Sleep with Task.Delay for non-.NET 4.0 platforms (thanks @GeertvanHorrik)

## 6.0.0

- ### Breaking changes
  - **[#466](../../issues/466) - StrongName signed assembly? Hey, it's 2017...**  
  **Fluent.Ribbon is no longer a strong-named assembly.**
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
  - The following `Color` and `Brush` resources were replaced/renamed/removed ("---" indicates removed):

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
| ContextMenuBarResizeBackgoundBrush | Fluent.Ribbon.Brushes.DropDown.Resize.BackgroundBrush |
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
| ScrollButtonHoverBorderBrush | Fluent.Ribbon.Brushes.Button.MouseOver.BorderBrush |
| ScrollButtonHoverBackgroundBrush | Fluent.Ribbon.Brushes.Button.MouseOver.Background |
| ScrollButtonPressedBorderBrush | Fluent.Ribbon.Brushes.Button.Pressed.BorderBrush |
| ScrollButtonPressedBackgroundBrush | Fluent.Ribbon.Brushes.Button.Pressed.Background |
| ScrollBackgroundBrush | Fluent.Ribbon.Brushes.ScrollBar.Background |
| ScrollVerticalBackgroundBrush | Fluent.Ribbon.Brushes.ScrollBar.Background |
| ScrollThumbDefaultBorderBrush | Fluent.Ribbon.Brushes.ScrollThumb.Default.BorderBrush |
| ScrollThumbDefaultBackgroundBrush | Fluent.Ribbon.Brushes.ScrollThumb.Default.Background |
| ScrollThumbHoverBorderBrush | Fluent.Ribbon.Brushes.Button.MouseOver.BorderBrush |
| ScrollThumbHoverBackgroundBrush | Fluent.Ribbon.Brushes.Button.MouseOver.Background |
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
  - [#473](../../issues/473) - RibbonWindow does not resize when Children resize and SizeToContent is used
  - [#481](../../issues/481) - ToggleButton behaviour is wrong when GroupName is set
  - [#485](../../issues/485) - InRibbonGallery broken when ItemsSource is empty
  - [#486](../../issues/486) - Stretching of DropDownButton differs from Button
  - [#489](../../issues/489) - Fluent Ribbon crashes program on shutdown
  - [#493](../../issues/493) - Sometimes icons are not drawn when using ObjectToImageConverter
  - [#500](../../issues/500) - Binding error for RibbonProperties.MouseOverBackground
  - [#501](../../issues/501) - Ribbon controls disappear when ribbon is initially disabled

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
  - [#478](../../issues/478) - Custom MinWidth of Start Screen (LeftContent)
  - [#479](../../issues/479) - Bulgarian language (thanks @kalatchev)
  - [#480](../../issues/480) - Disable Scroll Wheel for tab selection  
  You can control this behavior by using `IsMouseWheelScrollingEnabled` on `Ribbon`
  - [#484](../../issues/484) - Add special style/template for MenuItem with set description
  - [#488](../../issues/488) - Display border around content area of RibbonTabControl  
  You can now use `Fluent.Ribbon.Values.RibbonTabControl.Content.BorderThickness` to control the thickness of the border around the content area of `RibbonTabControl`
  - [#494](../../issues/494) - How to align controls such as ToggleButton, Spinner with text  
  You can now opt in to align the headers of controls like `ComboBox` or `Spinner` which are placed in the same column of a `RibbonGroupBox` by adding `Grid.IsSharedSizeScope="True"` to a `RibbonGroupBox`.
  You can opt out of this behavior for single controls in that column by adding `Fluent:RibbonGroupBoxWrapPanel.ExcludeFromSharedSize="True"` to that control.  
  Documentation can be found at http://fluentribbon.github.io/documentation/concepts/sizing#aligningControls
  - [#495](../../issues/495) - Add option disable handling of KeyTips  
  You can now disable handling of all KeyTips by setting `IsKeyTipHandlingEnabled` on `Ribbon` to `False`.
  - [#503](../../issues/503) - Add IsDefinitive property to GalleryItem (thanks @noctis0430)  

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