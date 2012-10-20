#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.ComponentModel;
using System.Globalization;

namespace Fluent
{
    /// <summary>
    /// Contains localizable Ribbon's properties. 
    /// Set Culture property to change current Ribbon localization or 
    /// set properties independently to use your localization
    /// </summary>
    public class RibbonLocalization : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        /// <summary>
        /// Occurs then property is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        // Raises PropertYChanegd event
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Culture

        private CultureInfo culture;

        /// <summary>
        /// Gets or sets current culture used for localization
        /// </summary>
        public CultureInfo Culture
        {
            get { return culture; }
            set
            {
                if (value != culture)
                {
                    culture = value;
                    LoadCulture(culture);
                    RaisePropertyChanged("Culture");
                }
            }
        }

        #endregion

        #region Text of backstage button

        // Text of backstage button
        string backstageButtonText = "File";

        private string backstageButtonTextProperty;

        /// <summary>
        /// Gets or sets text of backstage button
        /// </summary>
        public string BackstageButtonText
        {
            get { return backstageButtonTextProperty ?? backstageButtonText; }
            set
            {
                if (backstageButtonTextProperty != value)
                {
                    backstageButtonTextProperty = value;
                    RaisePropertyChanged("BackstageButtonText");
                }
            }
        }

        #endregion

        #region KeyTip of backstage button

        // KeyTip of backstage button
        string backstageButtonKeyTip = "F";

        private string backstageButtonKeyTipProperty;
        /// <summary>
        /// Gets or sets KeyTip of backstage button
        /// </summary>
        public string BackstageButtonKeyTip
        {
            get { return backstageButtonKeyTipProperty ?? backstageButtonKeyTip; }
            set
            {
                if (backstageButtonKeyTipProperty != value)
                {
                    backstageButtonKeyTipProperty = value;
                    RaisePropertyChanged("BackstageButtonKeyTip");
                }
            }
        }

        #endregion

        #region Minimize Button ScreenTip Title

        // Minimize Button ScreenTip Title
        string minimizeButtonScreenTipTitle = "Minimize the Ribbon (Ctrl + F1)";

        private string minimizeButtonScreenTipTitleProperty;

        /// <summary>
        /// Minimize Button ScreenTip Title
        /// </summary>
        public string MinimizeButtonScreenTipTitle
        {
            get { return minimizeButtonScreenTipTitleProperty ?? minimizeButtonScreenTipTitle; }
            set
            {
                if (minimizeButtonScreenTipTitleProperty != value)
                {
                    minimizeButtonScreenTipTitleProperty = value;
                    RaisePropertyChanged("MinimizeButtonScreenTipTitle");
                }
            }
        }

        #endregion

        #region Minimize Button ScreenTip Text

        // Minimize Button ScreenTip Text
        string minimizeButtonScreenTipText = "Show or hide the Ribbon\n\nWhen the Ribbon is hidden, only\nthe tab names are shown";
        private string minimizeButtonScreenTipTextProperty;

        /// <summary>
        /// Minimize Button ScreenTip Text
        /// </summary>
        public string MinimizeButtonScreenTipText
        {
            get { return minimizeButtonScreenTipTextProperty ?? minimizeButtonScreenTipText; }
            set
            {
                if (minimizeButtonScreenTipTextProperty != value)
                {
                    minimizeButtonScreenTipTextProperty = value;
                    RaisePropertyChanged("MinimizeButtonScreenTipText");
                }
            }
        }

        #endregion

        #region Expand Button ScreenTip Title

        // Expand Button ScreenTip Title
        string expandButtonScreenTipTitle = "Expand the Ribbon (Ctrl + F1)";
        private string expandButtonScreenTipTitleProperty;

        /// <summary>
        /// Expand Button ScreenTip Title
        /// </summary>
        public string ExpandButtonScreenTipTitle
        {
            get { return expandButtonScreenTipTitleProperty ?? expandButtonScreenTipTitle; }
            set
            {
                if (expandButtonScreenTipTitleProperty != value)
                {
                    expandButtonScreenTipTitleProperty = value;
                    RaisePropertyChanged("ExpandButtonScreenTipTitle");
                }
            }
        }

        #endregion

        #region Expand Button ScreenTip Text

        // Expand Button ScreenTip Text
        string expandButtonScreenTipText = "Show or hide the Ribbon\n\nWhen the Ribbon is hidden, only\nthe tab names are shown";

        string expandButtonScreenTipTextProperty;

        /// <summary>
        /// Expand Button ScreenTip Text
        /// </summary>
        public string ExpandButtonScreenTipText
        {
            get { return expandButtonScreenTipTextProperty ?? expandButtonScreenTipText; }
            set
            {
                if (expandButtonScreenTipTextProperty != value)
                {
                    expandButtonScreenTipTextProperty = value;
                    RaisePropertyChanged("ExpandButtonScreenTipText");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar DropDown Button ToolTip

        // Quick Access ToolBar DropDown Button ToolTip
        private string quickAccessToolBarDropDownButtonTooltip = "Customize Quick Access Toolbar";

        private string quickAccessToolBarDropDownButtonTooltipProperty;

        /// <summary>
        /// Quick Access ToolBar DropDown Button ToolTip
        /// </summary>
        public string QuickAccessToolBarDropDownButtonTooltip
        {
            get { return quickAccessToolBarDropDownButtonTooltipProperty ?? quickAccessToolBarDropDownButtonTooltip; }
            set
            {
                if (quickAccessToolBarDropDownButtonTooltipProperty != value)
                {
                    quickAccessToolBarDropDownButtonTooltipProperty = value;
                    RaisePropertyChanged("QuickAccessToolBarDropDownButtonTooltip");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar MoreControls Button ToolTip

        // Quick Access ToolBar MoreControls Button ToolTip
        private string quickAccessToolBarMoreControlsButtonTooltip = "More controls";
        private string quickAccessToolBarMoreControlsButtonTooltipProperty;

        /// <summary>
        /// Quick Access ToolBar MoreControls Button ToolTip
        /// </summary>
        public string QuickAccessToolBarMoreControlsButtonTooltip
        {
            get { return quickAccessToolBarMoreControlsButtonTooltipProperty ?? quickAccessToolBarMoreControlsButtonTooltip; }
            set
            {
                if (quickAccessToolBarMoreControlsButtonTooltipProperty != value)
                {
                    quickAccessToolBarMoreControlsButtonTooltipProperty = value;
                    RaisePropertyChanged("QuickAccessToolBarMoreControlsButtonTooltip");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Header

        // Quick Access ToolBar  Menu Header
        private string quickAccessToolBarMenuHeader = "Customize Quick Access Toolbar";
        private string quickAccessToolBarMenuHeaderProperty;

        /// <summary>
        /// Quick Access ToolBar  Menu Header
        /// </summary>
        public string QuickAccessToolBarMenuHeader
        {
            get { return quickAccessToolBarMenuHeaderProperty ?? quickAccessToolBarMenuHeader; }
            set
            {
                if (quickAccessToolBarMenuHeaderProperty != value)
                {
                    quickAccessToolBarMenuHeaderProperty = value;
                    RaisePropertyChanged("QuickAccessToolBarMenuHeader");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Context Menu Show Below

        // Quick Access ToolBar Minimize Quick Access Toolbar
        private string quickAccessToolBarMenuShowBelow = "Show Below the Ribbon";
        private string quickAccessToolBarMenuShowBelowProperty;

        /// <summary>
        /// Quick Access ToolBar Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowBelow
        {
            get { return quickAccessToolBarMenuShowBelowProperty ?? quickAccessToolBarMenuShowBelow; }
            set
            {
                if (quickAccessToolBarMenuShowBelowProperty != value)
                {
                    quickAccessToolBarMenuShowBelowProperty = value;
                    RaisePropertyChanged("QuickAccessToolBarMenuShowBelow");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Show Above

        // Quick Access ToolBar Menu Minimize Quick Access Toolbar
        private string quickAccessToolBarMenuShowAbove = "Show Above the Ribbon";
        private string quickAccessToolBarMenuShowAboveProperty;

        /// <summary>
        /// Quick Access ToolBar Menu Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowAbove
        {
            get { return quickAccessToolBarMenuShowAboveProperty ?? quickAccessToolBarMenuShowAbove; }
            set
            {
                if (quickAccessToolBarMenuShowAboveProperty != value)
                {
                    quickAccessToolBarMenuShowAboveProperty = value;
                    RaisePropertyChanged("QuickAccessToolBarMenuShowAbove");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Item

        // Quick Access ToolBar Menu Add Item
        private string ribbonContextMenuAddItem = "Add to Quick Access Toolbar";
        private string ribbonContextMenuAddItemProperty;

        /// <summary>
        /// Quick Access ToolBar Menu Add Item
        /// </summary>
        public string RibbonContextMenuAddItem
        {
            get { return ribbonContextMenuAddItemProperty ?? ribbonContextMenuAddItem; }
            set
            {
                if (ribbonContextMenuAddItemProperty != value)
                {
                    ribbonContextMenuAddItemProperty = value;
                    RaisePropertyChanged("RibbonContextMenuAddItem");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Group

        // Quick Access ToolBar Menu Add Group
        private string ribbonContextMenuAddGroup = "Add Group to Quick Access Toolbar";
        private string ribbonContextMenuAddGroupProperty;

        /// <summary>
        /// Quick Access ToolBar Menu Add Group
        /// </summary>
        public string RibbonContextMenuAddGroup
        {
            get { return ribbonContextMenuAddGroupProperty ?? ribbonContextMenuAddGroup; }
            set
            {
                if (ribbonContextMenuAddGroupProperty != value)
                {
                    ribbonContextMenuAddGroupProperty = value;
                    RaisePropertyChanged("RibbonContextMenuAddGroup");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Gallery

        // Quick Access ToolBar Menu Add Gallery
        private string ribbonContextMenuAddGallery = "Add Gallery to Quick Access Toolbar";
        private string ribbonContextMenuAddGalleryProperty;

        /// <summary>
        /// Quick Access ToolBar Menu Add Gallery
        /// </summary>
        public string RibbonContextMenuAddGallery
        {
            get { return ribbonContextMenuAddGalleryProperty ?? ribbonContextMenuAddGallery; }
            set
            {
                if (ribbonContextMenuAddGalleryProperty != value)
                {
                    ribbonContextMenuAddGalleryProperty = value;
                    RaisePropertyChanged("RibbonContextMenuAddGallery");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Menu

        // Quick Access ToolBar Menu Add Menu
        private string ribbonContextMenuAddMenu = "Add Menu to Quick Access Toolbar";
        private string ribbonContextMenuAddMenuProperty;

        /// <summary>
        /// Quick Access ToolBar Menu Add Menu
        /// </summary>
        public string RibbonContextMenuAddMenu
        {
            get { return ribbonContextMenuAddMenuProperty ?? ribbonContextMenuAddMenu; }
            set
            {
                if (ribbonContextMenuAddMenuProperty != value)
                {
                    ribbonContextMenuAddMenuProperty = value;
                    RaisePropertyChanged("RibbonContextMenuAddMenu");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Remove Item

        // Quick Access ToolBar Menu Remove Item
        private string ribbonContextMenuRemoveItem = "Remove from Quick Access Toolbar";
        private string ribbonContextMenuRemoveItemProperty;

        /// <summary>
        /// Quick Access ToolBar Menu Remove Item
        /// </summary>
        public string RibbonContextMenuRemoveItem
        {
            get { return ribbonContextMenuRemoveItemProperty ?? ribbonContextMenuRemoveItem; }
            set
            {
                if (ribbonContextMenuRemoveItemProperty != value)
                {
                    ribbonContextMenuRemoveItemProperty = value;
                    RaisePropertyChanged("RibbonContextMenuRemoveItem");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Customize Quick Access Toolbar

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
        private string ribbonContextMenuCustomizeQuickAccessToolbarProperty;

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeQuickAccessToolBar
        {
            get { return ribbonContextMenuCustomizeQuickAccessToolbarProperty ?? ribbonContextMenuCustomizeQuickAccessToolbar; }
            set
            {
                if (ribbonContextMenuCustomizeQuickAccessToolbarProperty != value)
                {
                    ribbonContextMenuCustomizeQuickAccessToolbarProperty = value;
                    RaisePropertyChanged("RibbonContextMenuCustomizeQuickAccessToolBar");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Customize Ribbon

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string ribbonContextMenuCustomizeRibbon = "Customize the Ribbon...";
        private string ribbonContextMenuCustomizeRibbonProperty;

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeRibbon
        {
            get { return ribbonContextMenuCustomizeRibbonProperty ?? ribbonContextMenuCustomizeRibbon; }
            set
            {
                if (ribbonContextMenuCustomizeRibbonProperty != value)
                {
                    ribbonContextMenuCustomizeRibbonProperty = value;
                    RaisePropertyChanged("RibbonContextMenuCustomizeRibbon");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Minimize Ribbon

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuMinimizeRibbon = "Minimize the Ribbon";
        private string ribbonContextMenuMinimizeRibbonProperty;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuMinimizeRibbon
        {
            get { return ribbonContextMenuMinimizeRibbonProperty ?? ribbonContextMenuMinimizeRibbon; }
            set
            {
                if (ribbonContextMenuMinimizeRibbonProperty != value)
                {
                    ribbonContextMenuMinimizeRibbonProperty = value;
                    RaisePropertyChanged("RibbonContextMenuMinimizeRibbon");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Show Below

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuShowBelow = "Show Quick Access Toolbar Below the Ribbon";
        private string ribbonContextMenuShowBelowProperty;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowBelow
        {
            get { return ribbonContextMenuShowBelowProperty ?? ribbonContextMenuShowBelow; }
            set
            {
                if (ribbonContextMenuShowBelowProperty != value)
                {
                    ribbonContextMenuShowBelowProperty = value;
                    RaisePropertyChanged("RibbonContextMenuShowBelow");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Show Above

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuShowAbove = "Show Quick Access Toolbar Above the Ribbon";
        private string ribbonContextMenuShowAboveProperty;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowAbove
        {
            get { return ribbonContextMenuShowAboveProperty ?? ribbonContextMenuShowAbove; }
            set
            {
                if (ribbonContextMenuShowAboveProperty != value)
                {
                    ribbonContextMenuShowAboveProperty = value;
                    RaisePropertyChanged("RibbonContextMenuShowAbove");
                }
            }
        }

        #endregion

        #region ScreenTipDisableReasonHeader

        // ScreenTip's Disable Reason Header
        string screenTipDisableReasonHeader = "This command is currently disabled.";
        private string screenTipDisableReasonHeaderProperty;

        /// <summary>
        /// Gets or sets ScreenTip's disable reason header
        /// </summary>
        public string ScreenTipDisableReasonHeader
        {
            get { return screenTipDisableReasonHeaderProperty ?? screenTipDisableReasonHeader; }
            set
            {
                if (screenTipDisableReasonHeaderProperty != value)
                {
                    screenTipDisableReasonHeaderProperty = value;
                    RaisePropertyChanged("ScreenTipDisableReasonHeader");
                }
            }
        }

        #endregion

        #region ScreenTipF1Label

        // ScreenTip's Disable Reason Header
        string screenTipF1LabelHeader = "Press F1 to more help.";
        private string screenTipF1LabelHeaderProperty;

        /// <summary>
        /// Gets or sets ScreenTip's disable reason header
        /// </summary>
        public string ScreenTipF1LabelHeader
        {
            get { return screenTipF1LabelHeaderProperty ?? screenTipF1LabelHeader; }
            set
            {
                if (screenTipF1LabelHeaderProperty != value)
                {
                    screenTipF1LabelHeaderProperty = value;
                    RaisePropertyChanged("screenTipF1LabelHeader");
                }
            }
        }

        #endregion

        #region Customize Status Bar

        // Text of backstage button
        string customizeStatusBar = "Customize Status Bar";
        private string customizeStatusBarProperty;

        /// <summary>
        /// Gets or sets customize Status Bar
        /// </summary>
        public string CustomizeStatusBar
        {
            get { return customizeStatusBarProperty ?? customizeStatusBar; }
            set
            {
                if (customizeStatusBarProperty != value)
                {
                    customizeStatusBarProperty = value;
                    RaisePropertyChanged("CustomizeStatusBar");
                }
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonLocalization()
        {
            Culture = CultureInfo.CurrentUICulture;
        }

        #endregion

        #region Methods

        // Coerce all localized values
        void LoadCulture(CultureInfo culture)
        {
            string language = culture.TwoLetterISOLanguageName;

            switch (language)
            {
                case "en": LoadEnglish(); break;
                case "ru": LoadRussian(); break;
                case "uk": LoadUkrainian(); break;
                case "fa": LoadPersian(); break;
                case "de": LoadGerman(); break;
                case "hu": LoadHungarian(); break;
                case "cs": LoadCzech(); break;
                case "fr": LoadFrench(); break;
                case "pl": LoadPolish(); break;
                case "ja": LoadJapanese(); break;
                case "nl": LoadDutch(); break;
                case "pt": if (culture.Name == "pt-BR") LoadPortugueseBrazilian(); else LoadPortuguese(); break;
                case "es": LoadSpanish(); break;
                case "zh": LoadChinese(); break;
                case "sv": LoadSwedish(); break;
                case "sk": LoadSlovak(); break;
                case "ro": LoadRomanian(); break;
                case "it": LoadItalian(); break;
                case "ar": LoadArabic(); break;
                case "da": LoadDanish(); break;
                case "az": LoadAzerbaijani(); break;
                case "fi": LoadFinnish(); break;
                case "no": LoadNorwegian(); break;
                case "tr": LoadTurkish(); break;
                case "he": LoadHebrew(); break;
                case "ge": LoadGreek(); break;
                case "ko": LoadKorean(); break;
                case "vi": LoadVietnamese(); break;
                case "si": LoadSinhala(); break;
                case "sl": LoadSlovenian(); break;
                case "ca": LoadCatalan(); break;
            }

            // Coerce all values

            RaisePropertyChanged("BackstageButtonText");
            RaisePropertyChanged("BackstageButtonKeyTip");

            RaisePropertyChanged("MinimizeButtonScreenTipTitle");
            RaisePropertyChanged("MinimizeButtonScreenTipText");
            RaisePropertyChanged("ExpandButtonScreenTipTitle");
            RaisePropertyChanged("ExpandButtonScreenTipText");
            RaisePropertyChanged("QuickAccessToolBarDropDownButtonTooltip");
            RaisePropertyChanged("QuickAccessToolBarMoreControlsButtonTooltip");
            RaisePropertyChanged("QuickAccessToolBarMenuHeader");
            RaisePropertyChanged("QuickAccessToolBarMenuShowAbove");
            RaisePropertyChanged("QuickAccessToolBarMenuShowBelow");

            RaisePropertyChanged("RibbonContextMenuAddItem");
            RaisePropertyChanged("RibbonContextMenuAddGroup");
            RaisePropertyChanged("RibbonContextMenuAddGallery");
            RaisePropertyChanged("RibbonContextMenuAddMenu");
            RaisePropertyChanged("RibbonContextMenuRemoveItem");
            RaisePropertyChanged("RibbonContextMenuCustomizeRibbon");
            RaisePropertyChanged("RibbonContextMenuCustomizeQuickAccessToolBar");
            RaisePropertyChanged("RibbonContextMenuShowAbove");
            RaisePropertyChanged("RibbonContextMenuShowBelow");
            RaisePropertyChanged("RibbonContextMenuMinimizeRibbon");

            RaisePropertyChanged("ScreenTipDisableReasonHeader");
            RaisePropertyChanged("CustomizeStatusBar");
        }

        #endregion

        #region English

        void LoadEnglish()
        {
            // Backstage button text & key tip
            backstageButtonText = "File";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimize the Ribbon (Ctrl + F1)";
            minimizeButtonScreenTipText = "Show or hide the Ribbon\n\nWhen the Ribbon is hidden, only\nthe tab names are shown";
            expandButtonScreenTipTitle = "Expand the Ribbon (Ctrl + F1)";
            expandButtonScreenTipText = "Show or hide the Ribbon\n\nWhen the Ribbon is hidden, only\nthe tab names are shown";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Customize Quick Access Toolbar";
            quickAccessToolBarMoreControlsButtonTooltip = "More controls"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Customize Quick Access Toolbar";
            quickAccessToolBarMenuShowAbove = "Show Above the Ribbon";
            quickAccessToolBarMenuShowBelow = "Show Below the Ribbon";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Add to Quick Access Toolbar"; // Button
            ribbonContextMenuAddGroup = "Add Group to Quick Access Toolbar"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Add Gallery to Quick Access Toolbar"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Add Menu to Quick Access Toolbar"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Remove from Quick Access Toolbar"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
            ribbonContextMenuShowBelow = "Show Quick Access Toolbar Below the Ribbon";
            ribbonContextMenuShowAbove = "Show Quick Access Toolbar Above the Ribbon";
            ribbonContextMenuCustomizeRibbon = "Customize the Ribbon...";
            ribbonContextMenuMinimizeRibbon = "Minimize the Ribbon";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            //Screentips
            screenTipDisableReasonHeader = "This command is currently disabled.";
            ScreenTipF1LabelHeader = "Press F1 for help";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "Customize status bar";
        }

        #endregion

        #region Russian

        void LoadRussian()
        {
            backstageButtonText = "Файл";
            backstageButtonKeyTip = "Ф";

            minimizeButtonScreenTipTitle = "Свернуть ленту (Ctrl + F1)";
            minimizeButtonScreenTipText = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только\nимена вкладок.";
            expandButtonScreenTipTitle = "Развернуть ленту (Ctrl + F1)";
            expandButtonScreenTipText = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только\nимена вкладок.";

            quickAccessToolBarDropDownButtonTooltip = "Настройка панели быстрого доступа";
            quickAccessToolBarMoreControlsButtonTooltip = "Другие элементы";
            quickAccessToolBarMenuHeader = "Настройка панели быстрого доступа";
            quickAccessToolBarMenuShowAbove = "Разместить над лентой";
            quickAccessToolBarMenuShowBelow = "Разместить под лентой";

            ribbonContextMenuAddItem = "Добавить на панель быстрого доступа";
            ribbonContextMenuAddGroup = "Добавить группу на панель быстрого доступа";
            ribbonContextMenuAddGallery = "Добавить коллекцию на панель быстрого доступа";
            ribbonContextMenuAddMenu = "Добавить меню на панель быстрого доступа";
            ribbonContextMenuRemoveItem = "Удалить с панели быстрого доступа";
            ribbonContextMenuCustomizeQuickAccessToolbar = "Настройка панели быстрого доступа...";
            ribbonContextMenuShowBelow = "Разместить панель быстрого доступа под лентой";
            ribbonContextMenuShowAbove = "Разместить панель быстрого доступа над лентой";
            ribbonContextMenuCustomizeRibbon = "Настройка ленты...";
            ribbonContextMenuMinimizeRibbon = "Свернуть ленту";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            screenTipDisableReasonHeader = "В настоящее время эта команда отключена.";

            customizeStatusBar = "Настройка строки состояния";
        }

        #endregion

        #region Ukrainian

        void LoadUkrainian()
        {
            // Backstage button text & key tip 
            backstageButtonText = "Файл";
            backstageButtonKeyTip = "Ф";
            // See right-top corner... (two different tooltips must be if you press it) 
            minimizeButtonScreenTipTitle = "Сховати Стрічку (Ctrl + F1)";
            minimizeButtonScreenTipText = "Показати або сховати Стрічку\n\nКоли стрічка схована, видно\nтільки назви вкладок";
            expandButtonScreenTipTitle = "Показати Стрічку (Ctrl + F1)";
            expandButtonScreenTipText = "Показати або сховати Стрічку\n\nКоли стрічка схована, видно\nтільки назви вкладок";
            // QAT tooltips and menu items 
            quickAccessToolBarDropDownButtonTooltip = "Налаштувати Панель Інструментів Швидкого Доступу";
            quickAccessToolBarMoreControlsButtonTooltip = "Більше елементів"; // When two arrows appear ">>" 
            quickAccessToolBarMenuHeader = "Налаштувати Панель Інструментів Швидкого Доступу";
            quickAccessToolBarMenuShowAbove = "Показати Поверх Стрічки";
            quickAccessToolBarMenuShowBelow = "Показати Знизу Стрічки";
            // Click on Ribbon to show context menu 
            ribbonContextMenuAddItem = "Додати до Панелі Інструментів Швидкого Доступу"; // Button 
            ribbonContextMenuAddGroup = "Додати Групу до Панелі Інструментів Швидкого Доступу"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Додати Галерею до Панелі Інструментів Швидкого Доступу"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Додати Меню до Панелі Інструментів Швидкого Доступу"; // By dashed splitter in context menu 
            ribbonContextMenuRemoveItem = "Видалити з Панелі Інструментів Швидкого Доступу"; // By item in QAT 
            ribbonContextMenuCustomizeQuickAccessToolbar = "Налаштувати Панель Інструментів Швидкого Доступу...";
            ribbonContextMenuShowBelow = "Показати Панель Інструментів Швидкого Доступу Знизу Стрічки";
            ribbonContextMenuShowAbove = "Показати Панель Інструментів Швидкого Доступу Поверх Стрічки";
            ribbonContextMenuCustomizeRibbon = "Налаштувати Стрічку...";
            ribbonContextMenuMinimizeRibbon = "Зменшити Стрічку";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot 
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Ця команда на даний момент недоступна.";
        }

        #endregion

        #region Persian

        void LoadPersian()
        {
            // Backstage button text & key tip
            backstageButtonText = "فایل";
            backstageButtonKeyTip = "ف";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "کوچک کردن نوار (Ctrl + F1)";
            minimizeButtonScreenTipText = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";
            expandButtonScreenTipTitle = "بزرگ کردن نوار (Ctrl + F1)";
            expandButtonScreenTipText = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "دلخواه سازی میله ابزار دسترسی سریع";
            quickAccessToolBarMoreControlsButtonTooltip = "ابزارهای دیگر"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "دلخواه سازی میله ابزار دسترسی سریع"; ;
            quickAccessToolBarMenuShowAbove = "نمایش در بالای نوار";
            quickAccessToolBarMenuShowBelow = "نمایش در پایین نوار";
            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "اضافه کردن به میله ابزار دسترسی سریع"; // Button
            ribbonContextMenuAddGroup = "اضافه کردن گروه به میله ابزار دسترسی سریع"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "اضافه کردن گالری به میله ابزار دسترسی سریع"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "اضاقه کردن منو به میله ابزار دسترسی سریع"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "حذف از میله ابزار دسترسی سریع"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "دلخواه سازی میله ابزار دسترسی سریع...";
            ribbonContextMenuShowBelow = "نمایش میله ابزار دسترسی سریع در پایین نوار";
            ribbonContextMenuShowAbove = "نمایش میله ابزار دسترسی سریع در بالای نوار";
            ribbonContextMenuCustomizeRibbon = "دلخواه سازی نوار...";
            ribbonContextMenuMinimizeRibbon = "کوچک کردن نوار";
        }

        #endregion

        #region German

        void LoadGerman()
        {
            // Backstage button text & key tip
            backstageButtonText = "Datei";
            backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Menüband minimieren (Strg + F1)";
            minimizeButtonScreenTipText = "Das Menüband ausblenden.\n\nWenn das Menüband\nausgeblendet ist, werden nur die\nRegisterkartennamen angezeigt.";
            expandButtonScreenTipTitle = "Menüband erweitern (Strg + F1)";
            expandButtonScreenTipText = minimizeButtonScreenTipText;

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Symbolleiste für den Schnellzugriff anpassen";
            quickAccessToolBarMoreControlsButtonTooltip = "Weitere Befehle…"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Symbolleiste für den Schnellzugriff anpassen";
            quickAccessToolBarMenuShowAbove = "Über dem Menüband anzeigen";
            quickAccessToolBarMenuShowBelow = "Unter dem Menüband anzeigen";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Zur Symbolleiste für den Schnellzugriff hinzufügen"; // Button
            ribbonContextMenuAddGroup = "Gruppe zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Katalog zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Zur Symbolleiste für den Schnellzugriff hinzufügen"; // By dashed splitter in context menu

            ribbonContextMenuRemoveItem = "Aus Symbolleiste für den Schnellzugriff entfernen"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Symbolleiste für den Schnellzugriff anpassen...";
            ribbonContextMenuShowBelow = "Symbolleiste für den Schnellzugriff unter dem Menüband anzeigen";
            ribbonContextMenuShowAbove = "Symbolleiste für den Schnellzugriff über dem Menüband anzeigen";
            ribbonContextMenuCustomizeRibbon = "Menüband anpassen...";
            ribbonContextMenuMinimizeRibbon = "Menüband minimieren";

            //Screentips
            ScreenTipDisableReasonHeader = "Diese Funktion ist momentan deaktiviert.";
            ScreenTipF1LabelHeader = "Drücken Sie F1 für die Hilfe";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "Statusleiste anpassen";
        }

        #endregion

        #region Hungarian

        void LoadHungarian()
        {
            // Backstage button text & key tip 
            backstageButtonText = "Fájl";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "A menüszalag összecsukása (Ctrl + F1)";
            minimizeButtonScreenTipText = "Csak a lapnevek megjelenítése a menüszalagon";
            expandButtonScreenTipTitle = "Menüszalag kibontása (Ctrl + F1)";
            expandButtonScreenTipText = "A menüszalag megjelenítése úgy, hogy egy parancsra kattintás után is látható maradjon";

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Gyorselérési eszköztár testreszabása";
            quickAccessToolBarMoreControlsButtonTooltip = "További vezérlők"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Gyorselérési eszköztár testreszabása";
            quickAccessToolBarMenuShowAbove = "Megjelenítés a menüszalag alatt";
            quickAccessToolBarMenuShowBelow = "Megjelenítés a menüszalag felett";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Felvétel a gyorselérési eszköztárra"; // Button
            ribbonContextMenuAddGroup = "Felvétel a gyorselérési eszköztárra"; // For ex., by collapsed group 
            ribbonContextMenuAddGallery = "Gyűjtemény felvétele a gyorselérési eszköztárra"; // For ex., by opened font context menu 
            ribbonContextMenuAddMenu = "Felvétel a gyorselérési eszköztárra"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Eltávolítás a gyorselérési eszköztárról"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Gyorselérési eszköztár testreszabása...";
            ribbonContextMenuShowBelow = "A gyorselérési eszköztár megjelenítése a menüszalag alatt";
            ribbonContextMenuShowAbove = "A gyorselérési eszköztár megjelenítése a menüszalag felett";
            ribbonContextMenuCustomizeRibbon = "Menüszalag testreszabása...";
            ribbonContextMenuMinimizeRibbon = " A menüszalag összecsukása";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3)
            screenTipDisableReasonHeader = "Ez a parancs jelenleg nem használható.";
        }

        #endregion

        #region Czech

        void LoadCzech()
        {
            // Backstage button text & key tip
            backstageButtonText = "Soubor";
            backstageButtonKeyTip = "S";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Skrýt pás karet (Ctrl + F1)";
            minimizeButtonScreenTipText = "Zobrazit nebo skrýt pás karet\n\nJe-li pás karet skrytý, jsou\nzobrazeny pouze názvy karet";
            expandButtonScreenTipTitle = "Zobrazit pás karet (Ctrl + F1)";
            expandButtonScreenTipText = "Zobrazit nebo skrýt pás karet\n\nJe-li pás karet skrytý, jsou\nzobrazeny pouze názvy karet";

            // QAT tooltips and menu items 
            quickAccessToolBarDropDownButtonTooltip = "Přizpůsobit panel nástrojů Rychlý přístup";
            quickAccessToolBarMoreControlsButtonTooltip = "Další příkazy"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Přizpůsobit panel nástrojů Rychlý přístup";
            quickAccessToolBarMenuShowAbove = "Zobrazit nad pásem karet";
            quickAccessToolBarMenuShowBelow = "Zobrazit pod pásem karet";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Přidat na panel nástrojů Rychlý přístup"; // Button
            ribbonContextMenuAddGroup = "Přidat na panel nástrojů Rychlý přístup"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Přidat galerii na panel nástrojů Rychlý přístup"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Přidat na panel nástrojů Rychlý přístup"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Odebrat z panelu nástrojů Rychlý přístup"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Přizpůsobit panel nástrojů Rychlý přístup...";
            ribbonContextMenuShowBelow = "Zobrazit panel nástrojů Rychlý přístup pod pásem karet";
            ribbonContextMenuShowAbove = "Zobrazit panel nástrojů Rychlý přístup nad pásem karet";
            ribbonContextMenuCustomizeRibbon = "Přizpůsobit pás karet...";
            ribbonContextMenuMinimizeRibbon = "Skrýt pás karet";

            screenTipDisableReasonHeader = "Tento příkaz je aktuálně zakázán.";
        }

        #endregion

        #region French

        void LoadFrench()
        {
            // Backstage button text & key tip
            backstageButtonText = "Fichier";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimiser le Ruban (Ctrl + F1)";
            minimizeButtonScreenTipText = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul\nles noms sont affichés";
            expandButtonScreenTipTitle = "Agrandir le Ruban (Ctrl + F1)";
            expandButtonScreenTipText = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul\nles noms sont affichés";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Personnaliser la barre d'outils Accès Rapide";
            quickAccessToolBarMoreControlsButtonTooltip = "Plus de contrôles"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personnaliser la barre d'outil Accès Rapide";
            quickAccessToolBarMenuShowAbove = "Afficher au dessus du Ruban";
            quickAccessToolBarMenuShowBelow = "Afficher en dessous du Ruban";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Ajouter un élément à la barre d'outils Accès Rapide"; // Button
            ribbonContextMenuAddGroup = "Ajouter un groupe à la barre d'outils Accès Rapide"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Ajouter une galerie à la barre d'outils Accès Rapide"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Ajouter un menu à la barre d'outils Accès Rapide"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Supprimer de la barre d'outils Accès Rapide"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personnaliser la barre d'outils Accès Rapide...";
            ribbonContextMenuShowBelow = "Afficher la barre d'outils Accès Rapide en dessous du Ruban";
            ribbonContextMenuShowAbove = "Afficher la barre d'outils Accès Rapide au dessus du Ruban";
            ribbonContextMenuCustomizeRibbon = "Personnaliser le Ruban...";
            ribbonContextMenuMinimizeRibbon = "Minimiser le Ruban";
            customizeStatusBar = "Personnaliser la barre de statut";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            screenTipDisableReasonHeader = "Cette commande est actuellement désactivée.";
        }

        #endregion

        #region Polish

        void LoadPolish()
        {
            // Backstage button text & key tip
            backstageButtonText = "Plik";
            backstageButtonKeyTip = "P";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimalizuj Wstążkę (Ctrl + F1)";
            minimizeButtonScreenTipText = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko\nnazwy zakładek są widoczne";
            expandButtonScreenTipTitle = "Rozwiń Wstążkę (Ctrl + F1)";
            expandButtonScreenTipText = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko\nnazwy zakładek są widoczne";

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Dostosuj pasek narzędzi Szybki dostęp";
            quickAccessToolBarMoreControlsButtonTooltip = "Więcej poleceń..."; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Dostosuj pasek narzędzi Szybki dostęp";
            quickAccessToolBarMenuShowAbove = "Pokaż powyżej Wstążki";
            quickAccessToolBarMenuShowBelow = "Pokaż poniżej Wstążki";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Dodaj do paska narzędzi Szybki dostęp"; // Button
            ribbonContextMenuAddGroup = "Dodaj Grupę do paska narzędzi Szybki dostęp"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Dodaj Galerię do paska narzędzi Szybki dostęp"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Dodaj do paska narzędzi Szybki dostęp"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Usuń z paska narzędzi Szybki dostęp"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Dostosuj pasek narzędzi Szybki dostęp...";
            ribbonContextMenuShowBelow = "Pokaż pasek Szybki dostęp poniżej Wstążki";
            ribbonContextMenuShowAbove = "Pokaż pasek Szybki dostęp powyżej Wstążki";
            ribbonContextMenuCustomizeRibbon = "Dostosuj Wstążkę...";
            ribbonContextMenuMinimizeRibbon = "Minimalizuj Wstążkę";
        }

        #endregion

        #region Japanese

        void LoadJapanese()
        {
            // Backstage button text & key tip
            backstageButtonText = "ファイル";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "リボンの最小化 (Ctrl + F1)";
            minimizeButtonScreenTipText = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";
            expandButtonScreenTipTitle = "リボンの展開 (Ctrl + F1)";
            expandButtonScreenTipText = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "クイック アクセス ツール バーのユーザー設定";
            quickAccessToolBarMoreControlsButtonTooltip = "その他のボタン"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "クイック アクセス ツール バーのユーザー設定";
            quickAccessToolBarMenuShowAbove = "リボンの上に表示";
            quickAccessToolBarMenuShowBelow = "リボンの下に表示";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "クイック アクセス ツール バーに追加"; // Button
            ribbonContextMenuAddGroup = "グループをクイック アクセス ツール バーに追加"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "ギャラリーをクイック アクセス ツール バーに追加"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "メニューをクイック アクセス ツール バーに追加"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "クイック アクセス ツール バーから削除"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "クイック アクセス ツール バーのユーザー設定...";
            ribbonContextMenuShowBelow = "クイック アクセス ツール バーをリボンの下に表示";
            ribbonContextMenuShowAbove = "クイック アクセス ツール バーをリボンの上に表示";
            ribbonContextMenuCustomizeRibbon = "リボンのユーザー設定...";
            ribbonContextMenuMinimizeRibbon = "リボンの最小化";
        }

        #endregion

        #region Dutch

        void LoadDutch()
        {
            // Backstage button text & key tip
            backstageButtonText = "Bestand";
            backstageButtonKeyTip = "B";
            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Het lint minimaliseren (Ctrl + F1)";
            minimizeButtonScreenTipText = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, \nzijn alleen de tabulatie namen zichtbaar";
            expandButtonScreenTipTitle = "Het lint Maximaliseren (Ctrl + F1)";
            expandButtonScreenTipText = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is,\nzijn alleen de tabulatie namen zichtbaar";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Werkbalk snelle toegang aanpassen";
            quickAccessToolBarMoreControlsButtonTooltip = "meer opdrachten"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = " Werkbalk snelle toegang aanpassen ";
            quickAccessToolBarMenuShowAbove = "Boven het lint weergeven";
            quickAccessToolBarMenuShowBelow = "beneden het lint weergeven";
            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Menu toevoegen aan werkbalk snelle toegang"; // Button
            ribbonContextMenuAddGroup = "Groep toevoegen aan werkbalk snelle toegang"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Galerij toevoegen aan werkbalk snelle toegang"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = " Menu toevoegen aan werkbalk snelle toegang "; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = " Verwijder uit werkbalk snelle toegang "; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
            ribbonContextMenuShowBelow = " Werkbalk snelle toegang onder het lint weergeven";
            ribbonContextMenuShowAbove = " Werkbalk snelle toegang boven het lint weergeven ";
            ribbonContextMenuCustomizeRibbon = "Lint aanpassen...";
            ribbonContextMenuMinimizeRibbon = " Het lint minimaliseren";
        }

        #endregion

        #region Brazilian

        void LoadPortugueseBrazilian()
        {
            // Backstage button text & key tip
            backstageButtonText = "Arquivo";
            backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimizar o Ribbon (Ctrl + F1)";
            minimizeButtonScreenTipText = "Mostrar ou esconder o  Ribbon\n\nQuando o Ribbon estiver escondido, somente\no nome das abas serão mostrados";
            expandButtonScreenTipTitle = "Expandir o Ribbon (Ctrl + F1)";
            expandButtonScreenTipText = "Mostrar ou esconder o  Ribbon\n\nQuando o Ribbon estiver escondido, somente\no nome das abas serão mostrados";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Customizar Barra de acesso rápido";
            quickAccessToolBarMoreControlsButtonTooltip = "Mais controles"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = " Customizar Barra de acesso rápido";
            quickAccessToolBarMenuShowAbove = "Mostrar acima do Ribbon";
            quickAccessToolBarMenuShowBelow = "Mostrar abaixo do Ribbon";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Adicionar para Barra de acesso rápido"; // Button
            ribbonContextMenuAddGroup = " Adicionar o grupo para Barra de acesso rápido"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Adicionar a galeria para Barra de acesso rápido";
            // For ex., by opened font context menu
            ribbonContextMenuAddMenu = " Adicionar o menu para Barra de acesso rápido";
            // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Remover da Barra de acesso rápido"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Customizar Barra de acesso rápido...";
            ribbonContextMenuShowBelow = "Mostrar Barra de acesso rápido abaixo do Ribbon";
            ribbonContextMenuShowAbove = "Mostrar Barra de acesso rápido acima do Ribbon";
            ribbonContextMenuCustomizeRibbon = "Customizar o Ribbon...";
            ribbonContextMenuMinimizeRibbon = "Minimizar o Ribbon";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Este comando está desativado.";
        }

        #endregion

        #region Spanish

        void LoadSpanish()
        {
            // Backstage button text & key tip
            backstageButtonText = "Archivo";
            backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            // TRANSLATOR'S NOTE: This block is not shown at Windows 7's Apps (WordPad or Paint)
            minimizeButtonScreenTipTitle = "Minimizar la cinta (Ctrl + F1)";
            minimizeButtonScreenTipText = "Muestra u oculta la cinta\n\nCuando la cinta está oculta, sólo\nse muestran los nombres de las pestañas";
            expandButtonScreenTipTitle = "Expandir la cinta (Ctrl + F1)";
            expandButtonScreenTipText = "Muestra u oculta la cinta\n\nCuando la cinta está oculta, sólo\nse muestran los nombres de las pestañas";

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Personalizar barra de herramientas de acceso rápido";
            quickAccessToolBarMoreControlsButtonTooltip = "Más controles"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personalizar barra de herramientas de acceso rápido";
            quickAccessToolBarMenuShowAbove = "Mostrar sobre la cinta";
            quickAccessToolBarMenuShowBelow = "Mostrar bajo la cinta";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Agregar a la barra de herramientas de acceso rápido"; // Button
            ribbonContextMenuAddGroup = "Agregar grupo a la barra de herramientas de acceso rápido"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Agregar galería a la barra de herramientas de acceso rápido"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Agregar menú a la barra de herramientas de acceso rápido"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Quitar de la barra de herramientas de acceso rápido"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar la barra de herramientas de acceso rápido...";
            ribbonContextMenuShowBelow = "Mostrar barra de herramientas de acceso rápido bajo la cinta";
            ribbonContextMenuShowAbove = "Mostrar barra de herramientas de acceso rápido sobre la cinta";
            ribbonContextMenuCustomizeRibbon = "Personalizar la cinta...";
            ribbonContextMenuMinimizeRibbon = "Minimizar la cinta";
        }

        #endregion

        #region Chinese

        void LoadChinese()
        {
            // Backstage button text & key tip
            backstageButtonText = "文件";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "功能区最小化 (Ctrl + F1)";
            minimizeButtonScreenTipText = "隐藏功能区时，仅显示选项卡名称";
            expandButtonScreenTipTitle = "展开功能区 (Ctrl + F1)";
            expandButtonScreenTipText = "隐藏功能区时，仅显示选项卡名称";

            // QAT tooltips and menu items         
            quickAccessToolBarDropDownButtonTooltip = "自定义快速访问具栏";
            quickAccessToolBarMoreControlsButtonTooltip = "其他命令"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "自定义快速访问工具栏";
            quickAccessToolBarMenuShowAbove = "在功能区上方显示";
            quickAccessToolBarMenuShowBelow = "在功能区下方显示";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "添加到快速访问工具栏"; // Button
            ribbonContextMenuAddGroup = "在快速访问工具栏中添加组"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "在快速访问工具栏中添加样式"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "在快速访问工具栏中添加菜单"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "在快速访问工具栏中移除"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "自定义快速访问工具栏...";
            ribbonContextMenuShowBelow = "在功能区下方显示快速访问工具栏";
            ribbonContextMenuShowAbove = "在功能区上方显示快速访问工具栏";
            ribbonContextMenuCustomizeRibbon = "自定义功能区...";
            ribbonContextMenuMinimizeRibbon = "功能区最小化";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "此命令当前已被禁用。";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "自定义状态栏";
        }

        #endregion

        #region Swedish

        void LoadSwedish()
        {
            // Backstage button text & key tip
            backstageButtonText = "Arkiv";
            backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimera menyfliksområdet (Ctrl + F1)";
            minimizeButtonScreenTipText = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas\nendast flikarna";
            expandButtonScreenTipTitle = "Expandera menyfliksområdet (Ctrl + F1)";
            expandButtonScreenTipText = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas\nendast flikarna";

            // QAT tooltips and menu items          
            quickAccessToolBarDropDownButtonTooltip = "Anpassa verktygsfältet Snabbåtkomst ";
            quickAccessToolBarMoreControlsButtonTooltip = "Fler kommandon"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = " Anpassa verktygsfältet Snabbåtkomst";
            quickAccessToolBarMenuShowAbove = "Visa ovanför menyfliksområdet";
            quickAccessToolBarMenuShowBelow = "Visa under menyfliksområdet";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Lägg till i verktygsfältet Snabbåtkomst"; // Button
            ribbonContextMenuAddGroup = "Lägg till i verktygsfältet Snabbåtkomst"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Lägg till galleriet i verktygsfältet Snabbåtkomst"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = " Lägg till menyn i verktygsfältet Snabbåtkomst "; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Ta bort från verktygsfältet Snabbåtkomst"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Anpassa verktygsfältet Snabbåtkomst...";
            ribbonContextMenuShowBelow = " Visa verktygsfältet Snabbåtkomst under menyfliksområdet";
            ribbonContextMenuShowAbove = " Visa verktygsfältet Snabbåtkomst ovanför menyfliksområdet ";
            ribbonContextMenuCustomizeRibbon = "Anpassa menyfliksområdet...";
            ribbonContextMenuMinimizeRibbon = "Minimera menyfliksområdet";
        }

        #endregion

        #region Slovak

        void LoadSlovak()
        {
            // Backstage button text & key tip
            backstageButtonText = "Súbor";
            backstageButtonKeyTip = "S";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Skryť pás s nástrojmi (Ctrl + F1)";
            minimizeButtonScreenTipText = "Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet";
            expandButtonScreenTipTitle = "Zobraziť pás s nástrojmi (Ctrl + F1)";
            expandButtonScreenTipText = " Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet ";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Prispôsobenie panela s nástrojmi Rýchly prístup";
            quickAccessToolBarMoreControlsButtonTooltip = "Ďalšie príkazy"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Prispôsobenie panela s nástrojmi Rýchly prístup";
            quickAccessToolBarMenuShowAbove = " Zobraziť nad pásom s nástrojmi ";
            quickAccessToolBarMenuShowBelow = "Zobraziť pod pásom s nástrojmi";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Pridať na panel s nástrojmi Rýchly prístup"; // Button
            ribbonContextMenuAddGroup = " Pridať na panel s nástrojmi Rýchly prístup "; // For ex., by collapsed group
            ribbonContextMenuAddGallery = " Pridať galériu do panela s nástrojmi Rýchly prístup "; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Pridať na panel s nástrojmi Rýchly prístup"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Odstrániť z panela s nástrojmi Rýchly prístup "; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = " Prispôsobenie panela s nástrojmi Rýchly prístup...";
            ribbonContextMenuShowBelow = "Panel s nástrojmi Rýchly prístup zobraziť pod panelom s nástrojmi";
            ribbonContextMenuShowAbove = "Panel s nástrojmi Rýchly prístup zobraziť nad panelom s nástrojmi ";
            ribbonContextMenuCustomizeRibbon = "Prispôsobenie panela s nástrojmi Rýchly prístup...";
            ribbonContextMenuMinimizeRibbon = "Minimalizovať pás s nástrojmi";
        }

        #endregion

        #region Romanian

        void LoadRomanian()
        {
            // Backstage button text & key tip
            backstageButtonText = "Fișier";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimizează Ribbon-ul (Ctrl + F1)";
            minimizeButtonScreenTipText = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt\nafișate doar numele taburilor";
            expandButtonScreenTipTitle = "Expandează Ribbon-ul (Ctrl + F1)";
            expandButtonScreenTipText = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt\nafișate doar numele taburilor";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Personalizează Bara de Acces Rapid";
            quickAccessToolBarMoreControlsButtonTooltip = "Mai multe controale"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personalizează Bara de Acces Rapid";
            quickAccessToolBarMenuShowAbove = "Afișează peste Ribbon";
            quickAccessToolBarMenuShowBelow = "Afișează sub Ribbon";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Adaugă la Bara de Acess Rapid"; // Button
            ribbonContextMenuAddGroup = "Adaugă Grupul la Bara de Acess Rapid"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Adaugă Galeria la Bara de Acess Rapid";
            // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Adaugă Meniul la Bara de Acess Rapid"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Eimină din Bara de Acess Rapid"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizează Bara de Acces Rapid...";
            ribbonContextMenuShowBelow = "Afișează Bara de Acces Rapid sub Ribbon";
            ribbonContextMenuShowAbove = "Afișează Bara de Acces Rapid peste Ribbon";
            ribbonContextMenuCustomizeRibbon = "Personalizează Ribbon-ul...";
            ribbonContextMenuMinimizeRibbon = "Minimizează Ribbon-ul...";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Această comandă nu este disponibilă momentan.";
        }

        #endregion

        #region Italian

        void LoadItalian()
        {
            // Backstage button text & key tip
            backstageButtonText = "File";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Riduci a icona barra multifunzione (Ctrl + F1)";
            minimizeButtonScreenTipText = "Consente di visualizzare solo i nomi \ndelle schede nella barra multifunzione.";
            expandButtonScreenTipTitle = "Espandi la barra multifunzione (Ctrl + F1)";
            expandButtonScreenTipText = "Visualizza la barra multifunzione in modo\n che rimanga sempre espansa, anche se l’utente \nha fatto click su un comando.";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Personalizza barra di accesso rapido";
            quickAccessToolBarMoreControlsButtonTooltip = "Altri comandi…"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personalizza barra di accesso rapido";
            quickAccessToolBarMenuShowAbove = "Mostra sopra la barra multifunzione";
            quickAccessToolBarMenuShowBelow = "Mostra sotto la barra multifunzione";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Aggiungi alla barra di accesso rapido"; // Button
            ribbonContextMenuAddGroup = "Aggiungi gruppo alla barra di accesso rapido"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Aggiungi raccolta alla barra di accesso rapido"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Aggiungi menu alla barra di accesso rapido"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Rimuovi dalla barra di accesso rapido"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizza barra di accesso rapido...";
            ribbonContextMenuShowBelow = "Mostra la barra di accesso rapido sotto la barra multifunzione";
            ribbonContextMenuShowAbove = "Mostra la barra di accesso rapido sopra la barra multifunzione";
            ribbonContextMenuCustomizeRibbon = "Personalizza barra multifunzione...";
            ribbonContextMenuMinimizeRibbon = "Riduci a icona barra multifunzione";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Questo commando è disattivato.";
        }

        #endregion

        #region Arabic

        void LoadArabic()
        {
            // Backstage button text & key tip
            backstageButtonText = "ملف    ";
            backstageButtonKeyTip = "م    ";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "(Ctrl + F1)تصغير الشريط ";
            minimizeButtonScreenTipText = "إظهار أسماء علامات التبويب فقط على الشريط.";
            expandButtonScreenTipTitle = "(Ctrl + F1)توسيع الشريط ";
            expandButtonScreenTipText = "إظهار الشريط بحيث يكون موسعاً دائماً حتى بعد النقر فوق أمر.";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "تخصيص شريط أدوات الوصول السريع";
            quickAccessToolBarMoreControlsButtonTooltip = "أوامر إضافية"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "تخصيص شريط أدوات الوصول السريع";
            quickAccessToolBarMenuShowAbove = "إظهار أعلى الشريط";
            quickAccessToolBarMenuShowBelow = "إظهار أسفل الشريط";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "إضافة إلى شريط أدوات الوصول السريع"; // Button
            ribbonContextMenuAddGroup = "إضافة إلى شريط أدوات الوصول السريع"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "إضافة إلى شريط أدوات الوصول السريع"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "إضافة إلى شريط أدوات الوصول السريع"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "إزالة إلى شريط أدوات الوصول السريع"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "تخصيص شريط أدوات الوصول السريع...";
            ribbonContextMenuShowBelow = "إظهار شريط أدوات الوصول السريع أسفل الشريط";
            ribbonContextMenuShowAbove = "إظهار شريط أدوات الوصول السريع أعلى الشريط";
            ribbonContextMenuCustomizeRibbon = "تخصيص الشريط...";
            ribbonContextMenuMinimizeRibbon = "تصغير الشريط";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "تم حالياً تعطيل هذا الأمر.";
        }

        #endregion

        #region Danish

        void LoadDanish()
        {
            // Backstage button text & key 
            backstageButtonText = "Filer";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimer båndet (Ctrl + F1)";
            minimizeButtonScreenTipText = "Vis kun fanenavnene på båndet.";
            expandButtonScreenTipTitle = "Udvid båndet (Ctrl + F1)";
            expandButtonScreenTipText = "Vis båndet, så det altid er udvidet, selv\nnår du klikker på en kommando.";

            // QAT tooltips and menu items          
            quickAccessToolBarDropDownButtonTooltip = "Tilpas værktøjslinjen Hurtig adgang";
            quickAccessToolBarMoreControlsButtonTooltip = "Flere kontrolelementer"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = " Tilpas værktøjslinjen Hurtig adgang";
            quickAccessToolBarMenuShowAbove = "Vis ovenover båndet";
            quickAccessToolBarMenuShowBelow = "Vis under båndet";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Føj til værktøjslinjen Hurtig adgang"; // Button
            ribbonContextMenuAddGroup = "Føj til værktøjslinjen Hurtig adgang"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Tilføj Galleri til værktøjslinjen Hurtig adgang"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Føj til værktøjslinjen Hurtig adgang"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Fjern fra værktøjslinjen Hurtig adgang"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Tilpas værktøjslinjen Hurtig adgang...";
            ribbonContextMenuShowBelow = "Vis værktøjslinjen Hurtig adgang under båndet";
            ribbonContextMenuShowAbove = "Vis værktøjslinjen Hurtig adgang ovenover båndet";
            ribbonContextMenuCustomizeRibbon = "Tilpas båndet...";
            ribbonContextMenuMinimizeRibbon = "Minimer båndet";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Denne kommando er aktuelt deaktiveret.";
        }

        #endregion

        #region Portuguese

        void LoadPortuguese()
        {
            // Backstage button text & key tip
            backstageButtonText = "Ficheiro";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimizar o Friso (Ctrl + F1)";
            minimizeButtonScreenTipText = "Mostrar apenas os nomes dos separadores\n no Frisos.";
            expandButtonScreenTipTitle = "Expandir o Friso (Ctrl + F1)";
            expandButtonScreenTipText = "Mostrar o Friso de modo a aparecer sempre\nexpandido mesmo depois de clicar num\ncomando.";

            // QAT tooltips and menu items           
            quickAccessToolBarDropDownButtonTooltip = "Personalizar Barra de Ferramentas de Acesso Rápido";
            quickAccessToolBarMoreControlsButtonTooltip = "Mais Comandos..."; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personalizar Barra de Ferramentas de Acesso Rápido";
            quickAccessToolBarMenuShowAbove = "Mostrar Acima do Friso";
            quickAccessToolBarMenuShowBelow = "Mostrar Abaixo do Friso";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Adicionar à Barra de Ferramentas de Acesso Rápido";
            ribbonContextMenuAddGroup = "Adicionar Grupo à Barra de Ferramentas de Acesso Rápido";
            ribbonContextMenuAddGallery = "Adicionar Galeria à Barra de Ferramentas de Acesso Rápido";
            ribbonContextMenuAddMenu = "Adicionar Menu à Barra de Ferramentas de Acesso Rápido";
            ribbonContextMenuRemoveItem = "Remover da Barra de Ferramentas de Acesso Rápido";
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar Barra de Ferramentas de Acesso Rápido...";
            ribbonContextMenuShowBelow = "Mostrar Barra de Ferramentas de Acesso Rápido Abaixo do Friso";
            ribbonContextMenuShowAbove = "Mostrar Barra de Ferramentas de Acesso Rápido Acima do Friso";
            ribbonContextMenuCustomizeRibbon = "Personalizar o Friso...";
            ribbonContextMenuMinimizeRibbon = "Minimizar o Friso";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3)
            screenTipDisableReasonHeader = "Este comando está desactivado actualmente.";
        }

        #endregion

        #region Azerbaijani

        void LoadAzerbaijani()
        {
            // Backstage button text & key tip
            backstageButtonText = "Fayl";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Menyu lentini kiçilt (Ctrl + F1)";
            minimizeButtonScreenTipText = " Menyu lentini göstər və ya gizlət\n\n Menyu lentini kiçiləndə, yalnız\n tabların adları göstərilir";
            expandButtonScreenTipTitle = " Menyu lentini böyüt(Ctrl + F1)";
            expandButtonScreenTipText = " Menyu lentini göstər və ya gizlət\n\n Menyu lentini gizldəndə, yalnız, \n tabların adları göstərilir";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Sürətli Keçidin Alətlərini fərdiləşdir";
            quickAccessToolBarMoreControlsButtonTooltip = "Digər nəzarət vasitələri"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = " Sürətli Keçidin Alətlərini fərdiləşdir ";
            quickAccessToolBarMenuShowAbove = "Menyu lentinin üstündə göstər";
            quickAccessToolBarMenuShowBelow = " Menyu lentinin altında göstər ";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Sürətli Keçidin Alətlərinə əlavə et"; // Button
            ribbonContextMenuAddGroup = " Sürətli Keçidin Alətlərinə Qrup əlavə et "; // For ex., by collapsed group
            ribbonContextMenuAddGallery = " Sürətli Keçidin Alətlərinə Qalereya əlavə et"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = " Sürətli Keçidin Alətlərinə Menyu əlavə et"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = " Sürətli Keçidin Alətlərindən sil"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = " Sürətli Keçidin Alətlərini fərdiləşdir...";
            ribbonContextMenuShowBelow = " Sürətli Keçidin Alətlərini Menyu lentinin altında göstər ";
            ribbonContextMenuShowAbove = " Sürətli Keçidin Alətlərini Menyu lentinin üstündə göstər ";
            ribbonContextMenuCustomizeRibbon = "Menyu lentini fərdiləşdir...";
            ribbonContextMenuMinimizeRibbon = " Menyu lentini kiçilt";
        }

        #endregion

        #region Finnish

        void LoadFinnish()
        {
            backstageButtonText = "Tiedosto";
            backstageButtonKeyTip = "T";
            minimizeButtonScreenTipTitle = "Pienennä valintanauha (Ctrl + F1)";
            minimizeButtonScreenTipText = "Näytä valintanauhassa vain\nvälilehtien nimet";
            expandButtonScreenTipTitle = "Laajenna valintanauha (Ctrl + F1)";
            expandButtonScreenTipText = "Näytä valintanauha aina\nlaajennettuna silloinkin, kun\nvalitset komennon";
            quickAccessToolBarDropDownButtonTooltip = "Mukauta pikatyökaluriviä";
            quickAccessToolBarMoreControlsButtonTooltip = "Lisää valintoja";
            quickAccessToolBarMenuHeader = "Mukauta pikatyökaluriviä";
            quickAccessToolBarMenuShowAbove = "Näytä valintanauhan yläpuolella";
            quickAccessToolBarMenuShowBelow = "Näytä valintanauhan alapuolella";
            ribbonContextMenuAddItem = "Lisää pikatyökaluriville";
            ribbonContextMenuAddGroup = "Lisää ryhmä pikatyökaluriviin";
            ribbonContextMenuAddGallery = "Lisää valikoima pikatyökaluriviin";
            ribbonContextMenuAddMenu = "Lisää valikko pikatyökaluriviin";
            ribbonContextMenuRemoveItem = "Poista pikatyökaluriviltä";
            ribbonContextMenuCustomizeQuickAccessToolbar = "Mukauta pikatyökaluriviä...";
            ribbonContextMenuShowBelow = "Näytä pikatyökalurivi valintanauhan alapuolella";
            ribbonContextMenuShowAbove = "Näytä pikatyökalurivi valintanauhan yläpuolella";
            ribbonContextMenuCustomizeRibbon = "Mukauta valintanauhaa...";
            ribbonContextMenuMinimizeRibbon = "Pienennä valintanauha";
            screenTipDisableReasonHeader = "Tämä komento on tällä hetkellä poissa käytöstä";
        }

        #endregion

        #region Norwegian

        void LoadNorwegian()
        {
            // Backstage button text & key tip
            backstageButtonText = "Fil";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimer båndet (Ctrl + F1)";
            minimizeButtonScreenTipText = "Viser bare kategorinavnene på båndet";
            expandButtonScreenTipTitle = "Utvider båndet (Ctrl + F1)";
            expandButtonScreenTipText = "Vis båndet slik at det alltid er utvidet selv etter at du har valgt en kommando";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Tilpass verktøylinje for hurtigtilgang";
            quickAccessToolBarMoreControlsButtonTooltip = "Flere kontroller"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Tilpass verktøylinje for hurtigtilgang";
            quickAccessToolBarMenuShowAbove = "Vis over båndet";
            quickAccessToolBarMenuShowBelow = "Vis under båndet";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Legg til på verktøylinje for hurtigtilgang"; // Button
            ribbonContextMenuAddGroup = "Legg til gruppe på verktøylinje for hurtigtilgang"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Legg til galleri på verktøylinje for hurtigtilgang"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Legg til meny på verktøylinje for hurtigtilgang"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Fjern verktøylinjen for hurtigtilgang"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Tilpass verktøylinje for hurtigtilgang...";
            ribbonContextMenuShowBelow = "Vis verktøylinjen for hurtigtilgang under båndet";
            ribbonContextMenuShowAbove = "Vis verktøylinjen for hurtigtilgang over båndet";
            ribbonContextMenuCustomizeRibbon = "Tilpass båndet...";
            ribbonContextMenuMinimizeRibbon = "Minimer båndet";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Denne kommandoen er for øyeblikket deaktivert.";
        }

        #endregion

        #region Turkish

        void LoadTurkish()
        {
            // Backstage button text & key tip
            backstageButtonText = "Dosya";
            backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Şeridi Simge Durumuna Küçült (Ctrl + F1)";
            minimizeButtonScreenTipText = "Şeritte yanlız sekme adlarını\n görüntüleyin.";
            expandButtonScreenTipTitle = "Şeridi Genişlet (Ctrl + F1)";
            expandButtonScreenTipText = "Şeridi göstererek\n bir komut tıklatıldıktan sonra bile\n her zaman genişletilmiş olmasını sağlayın.";

            // QAT tooltips and menu items           
            quickAccessToolBarDropDownButtonTooltip = "Hızlı Erişim Çubuğu'nu Özelleştir";
            quickAccessToolBarMoreControlsButtonTooltip = "Daha Fazla Özellik"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Hızlı Erişim Çubuğu'nu Özelleştir";
            quickAccessToolBarMenuShowAbove = "Şeridin Üstünde Göster";
            quickAccessToolBarMenuShowBelow = "Şeridin Altında Göster";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Hızlı Erişim Çubuğu'na Ekle"; // Button
            ribbonContextMenuAddGroup = "Hızlı Erişim Çubuğu'na Grup Ekle"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Hızlı Erişim Çubuğu'na Galeri Ekle"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Hızlı Erişim Çubuğu'na Menü Ekle"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Hızlı Erişim Çubuğu'ndan Kaldır"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Hızlı Erişim Çubuğu'nu Özelleştir";
            ribbonContextMenuShowBelow = "Hızlı Erişim Çubuğu'nu Şeridin Altında Göster";
            ribbonContextMenuShowAbove = "Hızlı Erişim Çubuğu'nu Şeridin Üstünde Göster";
            ribbonContextMenuCustomizeRibbon = "Şeridi Özelleştir";
            ribbonContextMenuMinimizeRibbon = "Şeridi Simge Durumuna Küçült";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Bu Özellik Şu Anda Devre Dışı";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "Durum Çubuğunu Özelleştir";
        }

        #endregion

        #region Hebrew

        void LoadHebrew()
        {
            // Backstage button text & key tip
            backstageButtonText = "קובץ";
            backstageButtonKeyTip = "ק";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "מזער את רצועת הכלים (Ctrl + F1)";
            minimizeButtonScreenTipText = "הצג רק את שמות הכרטיסיות\nברצועת הכלים.";
            expandButtonScreenTipTitle = "הרחב את רצועת הכלים (Ctrl + F1)";
            expandButtonScreenTipText = "הצג את רצועת הכלים כשהיא\nמורחבת תמיד, גם לאחר\nשתלחץ על הפקודה.";

            // QAT tooltips and menu items           
            quickAccessToolBarDropDownButtonTooltip = "התאמה אישית של סרגל הכלים לגישה מהירה";
            quickAccessToolBarMoreControlsButtonTooltip = "פקודות נוספות"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "התאמה אישית של סרגל הכלים לגישה מהירה";
            quickAccessToolBarMenuShowAbove = "הצג מעל לרצועת הכלים";
            quickAccessToolBarMenuShowBelow = "הצג מעל לרצועת הכלים";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "הוסף לסרגל הכלים לגישה מהירה"; // Button
            ribbonContextMenuAddGroup = "הוסף קבוצה לסרגל הכלים לגישה מהירה"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "הוסף גלריה לסרגל הכלים לגישה מהירה"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "הוסף תפריט לסרגל הכלים לגישה מהירה"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "הסר מסרגל הכלים לגישה מהירה"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "של סרגל הכלים רצועת הכלים...";
            ribbonContextMenuShowBelow = "הצג את סרגל הכלים לגישה מהירה מתחת לרצועת הכלים";
            ribbonContextMenuShowAbove = "הצג את סרגל הכלים לגישה מהירה מעל לרצועת הכלים";
            ribbonContextMenuCustomizeRibbon = "התאמה אישית של רצועת הכלים...";
            ribbonContextMenuMinimizeRibbon = "מזער את רצועת הכלים";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "פקודה זו אינה זמינה כעת.";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "התאמה אישית של שורת המצב";
        }

        #endregion

        #region Greek

        void LoadGreek()
        {
            // Backstage button text & key tip
            backstageButtonText = "Αρχείο";
            backstageButtonKeyTip = "Α";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Ελαχιστοποίηση της Κορδέλας (Ctrl + F1)";
            minimizeButtonScreenTipText = "Εμφάνιση μόνο των ονομάτων καρτελών στην\nΚορδέλα.";
            expandButtonScreenTipTitle = "Ανάπτυξη της Κορδέλας (Ctrl + F1)";
            expandButtonScreenTipText = "Εμφάνιση της Κορδέλας προκειμένου να αναπτύσσεται\nπάντα, ακόμα και αφού κάνετε κλικ σε μια εντολή.";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης";
            quickAccessToolBarMoreControlsButtonTooltip = "Περισσότερες εντολές"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης";
            quickAccessToolBarMenuShowAbove = "Εμφάνιση πάνω από την Κορδέλα";
            quickAccessToolBarMenuShowBelow = "Εμφάνιση κάτω από την Κορδέλα";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Προσθήκη στη γραμμή εργαλείων γρήγορης πρόσβασης"; // Button
            ribbonContextMenuAddGroup = "Προσθήκη ομάδας στη γραμμή εργαλείων γρήγορης πρόσβασης"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Προσθήκη συλλογής στη γραμμή εργαλείων γρήγορης πρόσβασης"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Προσθήκη μενού στη γραμμή εργαλείων γρήγορης πρόσβασης"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Κατάργηση από τη γραμμή εργαλείων γρήγορης πρόσβασης"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης...";
            ribbonContextMenuShowBelow = "Εμφάνιση της γραμμής εργαλείων γρήγορης πρόσβασης κάτω από την Κορδέλα";
            ribbonContextMenuShowAbove = "Εμφάνιση της γραμμής εργαλείων γρήγορης πρόσβασης πάνω από την Κορδέλα";
            ribbonContextMenuCustomizeRibbon = "Προσαρμογή της Κορδέλας...";
            ribbonContextMenuMinimizeRibbon = "Ελαχιστοποίηση της Κορδέλας";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Αυτή η εντολή είναι απενεργοποιημένη προς το παρόν.";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "Προσαρμογή γραμμής κατάστασης";
        }

        #endregion

        #region Korean

        void LoadKorean()
        {
            // Backstage button text & key tip
            backstageButtonText = "파일";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "리본 메뉴를 최소화 합니다 (Ctrl + F1)";
            minimizeButtonScreenTipText = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";
            expandButtonScreenTipTitle = "리본 메뉴를 표시합니다 (Ctrl + F1)";
            expandButtonScreenTipText = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "빠른 실행 도구 모음 사용자 지정";
            quickAccessToolBarMoreControlsButtonTooltip = "기타 컨트롤들"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "빠른 실행 도구 모음 사용자 지정";
            quickAccessToolBarMenuShowAbove = "리본 메뉴 위에 표시";
            quickAccessToolBarMenuShowBelow = "리본 메뉴 아래에 표시";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "빠른 실행 도구 모음에 추가"; // Button
            ribbonContextMenuAddGroup = "그룹을 빠른 실행 도구 모음에 추가"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "갤러리를 빠른 실행 도구 모음에 추가"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "메뉴를 빠른 실행 도구 모음에 추가"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "빠른 실행 도구 모음에서 단추 제거"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "빠른 실행 도구 모음 사용자 지정...";
            ribbonContextMenuShowBelow = "리본 메뉴 아래에 빠른 실행 도구 모음 표시";
            ribbonContextMenuShowAbove = "리본 메뉴 위에 빠른 실행 도구 모음 표시";
            ribbonContextMenuCustomizeRibbon = "리본 메뉴 사용자 지정...";
            ribbonContextMenuMinimizeRibbon = "리본 메뉴 최소화";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "이 명령은 현재 사용할 수 없습니다.";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "상태 표시줄 사용자 지정";
        }

        #endregion

        #region Vietnamese

        void LoadVietnamese()
        {
            // Backstage button text & key tip
            backstageButtonText = "Tệp";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Thu gọn Ruy băng (Ctrl + F1)";
            minimizeButtonScreenTipText = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ\ncó tên thẻ được hiện";
            expandButtonScreenTipTitle = "Mở rộng Ruy băng (Ctrl + F1)";
            expandButtonScreenTipText = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ\ncó tên thẻ được hiện";

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Tùy chỉnh thanh công cụ Truy cập nhanh";
            quickAccessToolBarMoreControlsButtonTooltip = "Thêm điều khiển"; // khi có hai mũi tên ">>"
            quickAccessToolBarMenuHeader = "Tùy chỉnh thanh công cụ Truy cập nhanh";
            quickAccessToolBarMenuShowAbove = "Hiện trên thanh Ruy băng";
            quickAccessToolBarMenuShowBelow = "Hiện dưới thanh Ruy băng";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Thêm vào thanh công cụ Truy cập nhanh"; // Button
            ribbonContextMenuAddGroup = "Thêm nhóm vào thanh công cụ Truy cập nhanh"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Thêm bộ sưu tập vào thanh công cụ Truy cập nhanh"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Thêm menu vào thanh công cụ Truy cập nhanh"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Loại"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Tùy chỉnh thanh công cụ Truy cập nhanh...";
            ribbonContextMenuShowBelow = "Hiện thanh công cụ truy cập nhanh dưới thanh Ruy băng";
            ribbonContextMenuShowAbove = "Hiện thanh công cụ truy cập nhanh trên thanh Ruy băng";
            ribbonContextMenuCustomizeRibbon = "Tùy biến thanh Ruy băng...";
            ribbonContextMenuMinimizeRibbon = "Thu gọn Ruy băng";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Lệnh này hiện bị tắt.";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "Tùy biến thanh Trạng thái";
        }

        #endregion

        #region Sinhala (Sri Lanka)

        void LoadSinhala()
        {
            // Backstage button text & key tip
            backstageButtonText = "ගොනුව";
            backstageButtonKeyTip = "න1";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "රිබනය හකුළන්න (Ctrl + F1)";
            minimizeButtonScreenTipText = "රිබනය මත පටිති නාම පමණක් පෙන්වන්න.";
            expandButtonScreenTipTitle = "රිබනය විහිදන්න (Ctrl + F1)";
            expandButtonScreenTipText = "රිබනය පෙන්වන්න, එවිට ඔබ\n\n විධානයක් ක්ලික් කළද එය\n\n සැමවිටම විහිදී පවතී.";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය";
            quickAccessToolBarMoreControlsButtonTooltip = "තවත් විධාන"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය";
            quickAccessToolBarMenuShowAbove = "රිබනයට ඉහලින් පෙන්වන්න";
            quickAccessToolBarMenuShowBelow = "රිබනයට පහලින් පෙන්වන්න";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට එක් කරන්න"; // Button
            ribbonContextMenuAddGroup = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට එක් කරන්න"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට ගැලරිය එක් කරන්න"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට මෙනුව එක් කරන්න"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයෙන් ඉවත් කරන්න"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය කරන්න...";
            ribbonContextMenuShowBelow = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය රිබනයට පහලින් පෙන්වන්න";
            ribbonContextMenuShowAbove = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය රිබනයට ඉහලින් පෙන්වන්න";
            ribbonContextMenuCustomizeRibbon = "රිබනය අභිමත කරණය කරන්න...";
            ribbonContextMenuMinimizeRibbon = "රිබනය හකුළන්න";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "මෙම විධානය දැනට භාවිතා කළ නොහැක";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "තත්ව තීරුව අභිමත කරණය";
        }

        #endregion

        #region Slovenian

        void LoadSlovenian()
        {
            // Backstage button text & key tip
            backstageButtonText = "Datoteka";
            backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Minimiraj trak (Ctrl + F1)";
            minimizeButtonScreenTipText = "Pokaži ali skrij trak\n\nKo je trak skrit, so\nprikazani samo zavihki";
            expandButtonScreenTipTitle = "Razširi trak (Ctrl + F1)";
            expandButtonScreenTipText = "Pokaži ali skrij trak\n\nKo je trak skrit, so\nprikazani samo zavihki";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Prilagodi orodno vrstico za hitri dostop";
            quickAccessToolBarMoreControlsButtonTooltip = "Več ukazov"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Prilagodi orodno vrstico za hitri dostop";
            quickAccessToolBarMenuShowAbove = "Pokaži nad trakom";
            quickAccessToolBarMenuShowBelow = "Pokaži pod trakom";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Dodaj v orodno vrstico za hitri dostop"; // Button
            ribbonContextMenuAddGroup = "Dodaj skupino orodni vrstici za hitri dostop"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Dodaj galerijo orodni vrstici za hitri dostop"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Dodaj meni orodni vrstici za hitri dostop"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Odstrani iz orodne vrstice za hitri dostop"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Prilagodi orodno vrstico za hitri dostop...";
            ribbonContextMenuShowBelow = "Pokaži orodno vrstico za hitri dostop pod trakom";
            ribbonContextMenuShowAbove = "Pokaži orodno vrstico za hitri dostop nad trakom";
            ribbonContextMenuCustomizeRibbon = "Prilagodi trak...";
            ribbonContextMenuMinimizeRibbon = "Minimiraj trak";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            screenTipDisableReasonHeader = "Ta ukaz je trenutno onemogočen.";

            // Right-click on status bar to see it. NEW! from v2.0
            customizeStatusBar = "Prilagodi vrstico stanja";
        }

        #endregion

        #region Catalan

        void LoadCatalan()
        {
            // Backstage button text & key tip
            backstageButtonText = "Fitxer";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            // TRANSLATOR'S NOTE: This block is not shown at Windows 7's Apps (WordPad or Paint)
            minimizeButtonScreenTipTitle = "Minimitza la cinta (Ctrl + F1)";
            minimizeButtonScreenTipText = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només\ns'ensenyen els noms de les pestanyes";
            expandButtonScreenTipTitle = "Expandeix la cinta (Ctrl + F1)";
            expandButtonScreenTipText = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només\ns'ensenyen els noms de les pestanyes";

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Personalitza la barra d'eines d'accés ràpid";
            quickAccessToolBarMoreControlsButtonTooltip = "Més controls"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personalitza la barra d'eines d'accés ràpid";
            quickAccessToolBarMenuShowAbove = "Mostra sobre la cinta";
            quickAccessToolBarMenuShowBelow = "Mostra sota la cinta";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Afegeix a la barra d'eines d'accés ràpid"; // Button
            ribbonContextMenuAddGroup = "Afegeix grup a la barra d'eines d'accés ràpid"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Afegeix galeria a la barra d'eines d'accés ràpid"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Afegeix menú a la barra d'eines d'accés ràpid"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Elimina la barra d'eines d'accés ràpid"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personalitza la barra d'eines d'accés ràpid...";
            ribbonContextMenuShowBelow = "Mostra la barra d'eines d'accés ràpid sota la cinta";
            ribbonContextMenuShowAbove = "Mostra la barra d'eines d'accés ràpid sobre la cinta";
            ribbonContextMenuCustomizeRibbon = "Personalitza la cinta...";
            ribbonContextMenuMinimizeRibbon = "Minimitza la cinta";
        }

        #endregion
    }
}