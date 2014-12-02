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

        private CultureInfo _culture;

        /// <summary>
        /// Gets or sets current culture used for localization
        /// </summary>
        public CultureInfo Culture
        {
            get { return _culture; }
            set
            {
                if (!Equals(_culture, value))
                {
                    _culture = value;
                    LoadCulture(_culture);
                    RaisePropertyChanged("Culture");
                }
            }
        }

        #endregion

        #region Text of backstage button

        // Text of backstage button
        private string _backstageButtonText;

        /// <summary>
        /// Gets or sets text of backstage button
        /// </summary>
        public string BackstageButtonText
        {
            get { return _backstageButtonText; }
            set
            {
                if (_backstageButtonText != value)
                {
                    _backstageButtonText = value;
                    RaisePropertyChanged("BackstageButtonText");
                }
            }
        }

        #endregion

        #region KeyTip of backstage button

        // KeyTip of backstage button
        private string _backstageButtonKeyTip;

        /// <summary>
        /// Gets or sets KeyTip of backstage button
        /// </summary>
        public string BackstageButtonKeyTip
        {
            get { return _backstageButtonKeyTip; }
            set
            {
                if (_backstageButtonKeyTip != value)
                {
                    _backstageButtonKeyTip = value;
                    RaisePropertyChanged("BackstageButtonKeyTip");
                }
            }
        }

        #endregion

        #region Minimize Button ScreenTip Title

        // Minimize Button ScreenTip Title
        private string _minimizeButtonScreenTipTitle;

        /// <summary>
        /// Minimize Button ScreenTip Title
        /// </summary>
        public string MinimizeButtonScreenTipTitle
        {
            get { return _minimizeButtonScreenTipTitle; }
            set
            {
                if (_minimizeButtonScreenTipTitle != value)
                {
                    _minimizeButtonScreenTipTitle = value;
                    RaisePropertyChanged("MinimizeButtonScreenTipTitle");
                }
            }
        }

        #endregion

        #region Minimize Button ScreenTip Text

        // Minimize Button ScreenTip Text
        private string _minimizeButtonScreenTipText;

        /// <summary>
        /// Minimize Button ScreenTip Text
        /// </summary>
        public string MinimizeButtonScreenTipText
        {
            get { return _minimizeButtonScreenTipText; }
            set
            {
                if (_minimizeButtonScreenTipText != value)
                {
                    _minimizeButtonScreenTipText = value;
                    RaisePropertyChanged("MinimizeButtonScreenTipText");
                }
            }
        }

        #endregion

        #region Expand Button ScreenTip Title

        // Expand Button ScreenTip Title
        private string _expandButtonScreenTipTitle;

        /// <summary>
        /// Expand Button ScreenTip Title
        /// </summary>
        public string ExpandButtonScreenTipTitle
        {
            get { return _expandButtonScreenTipTitle; }
            set
            {
                if (_expandButtonScreenTipTitle != value)
                {
                    _expandButtonScreenTipTitle = value;
                    RaisePropertyChanged("ExpandButtonScreenTipTitle");
                }
            }
        }

        #endregion

        #region Expand Button ScreenTip Text

        // Expand Button ScreenTip Text
        private string _expandButtonScreenTipText;

        /// <summary>
        /// Expand Button ScreenTip Text
        /// </summary>
        public string ExpandButtonScreenTipText
        {
            get { return _expandButtonScreenTipText; }
            set
            {
                if (_expandButtonScreenTipText != value)
                {
                    _expandButtonScreenTipText = value;
                    RaisePropertyChanged("ExpandButtonScreenTipText");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar DropDown Button ToolTip

        // Quick Access ToolBar DropDown Button ToolTip
        private string _quickAccessToolBarDropDownButtonTooltip;

        /// <summary>
        /// Quick Access ToolBar DropDown Button ToolTip
        /// </summary>
        public string QuickAccessToolBarDropDownButtonTooltip
        {
            get { return _quickAccessToolBarDropDownButtonTooltip; }
            set
            {
                if (_quickAccessToolBarDropDownButtonTooltip != value)
                {
                    _quickAccessToolBarDropDownButtonTooltip = value;
                    RaisePropertyChanged("QuickAccessToolBarDropDownButtonTooltip");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar MoreControls Button ToolTip

        // Quick Access ToolBar MoreControls Button ToolTip
        private string _quickAccessToolBarMoreControlsButtonTooltip;

        /// <summary>
        /// Quick Access ToolBar MoreControls Button ToolTip
        /// </summary>
        public string QuickAccessToolBarMoreControlsButtonTooltip
        {
            get { return _quickAccessToolBarMoreControlsButtonTooltip; }
            set
            {
                if (_quickAccessToolBarMoreControlsButtonTooltip != value)
                {
                    _quickAccessToolBarMoreControlsButtonTooltip = value;
                    RaisePropertyChanged("QuickAccessToolBarMoreControlsButtonTooltip");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Header

        // Quick Access ToolBar  Menu Header
        private string _quickAccessToolBarMenuHeader;

        /// <summary>
        /// Quick Access ToolBar  Menu Header
        /// </summary>
        public string QuickAccessToolBarMenuHeader
        {
            get { return _quickAccessToolBarMenuHeader; }
            set
            {
                if (_quickAccessToolBarMenuHeader != value)
                {
                    _quickAccessToolBarMenuHeader = value;
                    RaisePropertyChanged("QuickAccessToolBarMenuHeader");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Context Menu Show Below

        // Quick Access ToolBar Minimize Quick Access Toolbar
        private string _quickAccessToolBarMenuShowBelow;

        /// <summary>
        /// Quick Access ToolBar Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowBelow
        {
            get { return _quickAccessToolBarMenuShowBelow; }
            set
            {
                if (_quickAccessToolBarMenuShowBelow != value)
                {
                    _quickAccessToolBarMenuShowBelow = value;
                    RaisePropertyChanged("QuickAccessToolBarMenuShowBelow");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Show Above

        // Quick Access ToolBar Menu Minimize Quick Access Toolbar
        private string _quickAccessToolBarMenuShowAbove;

        /// <summary>
        /// Quick Access ToolBar Menu Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowAbove
        {
            get { return _quickAccessToolBarMenuShowAbove; }
            set
            {
                if (_quickAccessToolBarMenuShowAbove != value)
                {
                    _quickAccessToolBarMenuShowAbove = value;
                    RaisePropertyChanged("QuickAccessToolBarMenuShowAbove");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Item

        // Quick Access ToolBar Menu Add Item
        private string _ribbonContextMenuAddItem;

        /// <summary>
        /// Quick Access ToolBar Menu Add Item
        /// </summary>
        public string RibbonContextMenuAddItem
        {
            get { return _ribbonContextMenuAddItem; }
            set
            {
                if (_ribbonContextMenuAddItem != value)
                {
                    _ribbonContextMenuAddItem = value;
                    RaisePropertyChanged("RibbonContextMenuAddItem");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Group

        // Quick Access ToolBar Menu Add Group
        private string _ribbonContextMenuAddGroup;

        /// <summary>
        /// Quick Access ToolBar Menu Add Group
        /// </summary>
        public string RibbonContextMenuAddGroup
        {
            get { return _ribbonContextMenuAddGroup; }
            set
            {
                if (_ribbonContextMenuAddGroup != value)
                {
                    _ribbonContextMenuAddGroup = value;
                    RaisePropertyChanged("RibbonContextMenuAddGroup");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Gallery

        // Quick Access ToolBar Menu Add Gallery
        private string _ribbonContextMenuAddGallery;

        /// <summary>
        /// Quick Access ToolBar Menu Add Gallery
        /// </summary>
        public string RibbonContextMenuAddGallery
        {
            get { return _ribbonContextMenuAddGallery; }
            set
            {
                if (_ribbonContextMenuAddGallery != value)
                {
                    _ribbonContextMenuAddGallery = value;
                    RaisePropertyChanged("RibbonContextMenuAddGallery");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Menu

        // Quick Access ToolBar Menu Add Menu
        private string _ribbonContextMenuAddMenu;

        /// <summary>
        /// Quick Access ToolBar Menu Add Menu
        /// </summary>
        public string RibbonContextMenuAddMenu
        {
            get { return _ribbonContextMenuAddMenu; }
            set
            {
                if (_ribbonContextMenuAddMenu != value)
                {
                    _ribbonContextMenuAddMenu = value;
                    RaisePropertyChanged("RibbonContextMenuAddMenu");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Remove Item

        // Quick Access ToolBar Menu Remove Item
        private string _ribbonContextMenuRemoveItem;

        /// <summary>
        /// Quick Access ToolBar Menu Remove Item
        /// </summary>
        public string RibbonContextMenuRemoveItem
        {
            get { return _ribbonContextMenuRemoveItem; }
            set
            {
                if (_ribbonContextMenuRemoveItem != value)
                {
                    _ribbonContextMenuRemoveItem = value;
                    RaisePropertyChanged("RibbonContextMenuRemoveItem");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Customize Quick Access Toolbar

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string _ribbonContextMenuCustomizeQuickAccessToolbar;

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeQuickAccessToolBar
        {
            get { return _ribbonContextMenuCustomizeQuickAccessToolbar; }
            set
            {
                if (_ribbonContextMenuCustomizeQuickAccessToolbar != value)
                {
                    _ribbonContextMenuCustomizeQuickAccessToolbar = value;
                    RaisePropertyChanged("RibbonContextMenuCustomizeQuickAccessToolBar");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Customize Ribbon

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string _ribbonContextMenuCustomizeRibbon;

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeRibbon
        {
            get { return _ribbonContextMenuCustomizeRibbon; }
            set
            {
                if (_ribbonContextMenuCustomizeRibbon != value)
                {
                    _ribbonContextMenuCustomizeRibbon = value;
                    RaisePropertyChanged("RibbonContextMenuCustomizeRibbon");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Minimize Ribbon

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string _ribbonContextMenuMinimizeRibbon;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuMinimizeRibbon
        {
            get { return _ribbonContextMenuMinimizeRibbon; }
            set
            {
                if (_ribbonContextMenuMinimizeRibbon != value)
                {
                    _ribbonContextMenuMinimizeRibbon = value;
                    RaisePropertyChanged("RibbonContextMenuMinimizeRibbon");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Show Below

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string _ribbonContextMenuShowBelow;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowBelow
        {
            get { return _ribbonContextMenuShowBelow; }
            set
            {
                if (_ribbonContextMenuShowBelow != value)
                {
                    _ribbonContextMenuShowBelow = value;
                    RaisePropertyChanged("RibbonContextMenuShowBelow");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Show Above

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string _ribbonContextMenuShowAbove;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowAbove
        {
            get { return _ribbonContextMenuShowAbove; }
            set
            {
                if (_ribbonContextMenuShowAbove != value)
                {
                    _ribbonContextMenuShowAbove = value;
                    RaisePropertyChanged("RibbonContextMenuShowAbove");
                }
            }
        }

        #endregion

        #region ScreenTipDisableReasonHeader

        // ScreenTip's Disable Reason Header
        private string _screenTipDisableReasonHeader;

        /// <summary>
        /// Gets or sets ScreenTip's disable reason header
        /// </summary>
        public string ScreenTipDisableReasonHeader
        {
            get { return _screenTipDisableReasonHeader; }
            set
            {
                if (_screenTipDisableReasonHeader != value)
                {
                    _screenTipDisableReasonHeader = value;
                    RaisePropertyChanged("ScreenTipDisableReasonHeader");
                }
            }
        }

        #endregion

        #region ScreenTipF1Label

        // ScreenTip's Disable Reason Header
        private string _screenTipF1LabelHeader;

        /// <summary>
        /// Gets or sets ScreenTip's disable reason header
        /// </summary>
        public string ScreenTipF1LabelHeader
        {
            get { return _screenTipF1LabelHeader; }
            set
            {
                if (_screenTipF1LabelHeader != value)
                {
                    _screenTipF1LabelHeader = value;
                    RaisePropertyChanged("ScreenTipF1LabelHeader");
                }
            }
        }

        #endregion

        #region Customize Status Bar

        // Text of backstage button
        private string _customizeStatusBar;

        /// <summary>
        /// Gets or sets customize Status Bar
        /// </summary>
        public string CustomizeStatusBar
        {
            get { return _customizeStatusBar; }
            set
            {
                if (_customizeStatusBar != value)
                {
                    _customizeStatusBar = value;
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
            // Fallback values
            this.LoadEnglish();

            this.Culture = CultureInfo.CurrentUICulture;
        }

        #endregion

        #region Methods

        // Coerce all localized values
        private void LoadCulture(CultureInfo culture)
        {
            var language = culture.TwoLetterISOLanguageName;

            switch (language)
            {
                case "en":
                    LoadEnglish();
                    break;

                case "ru":
                    LoadRussian();
                    break;

                case "uk":
                    LoadUkrainian();
                    break;

                case "fa":
                    LoadPersian();
                    break;

                case "de":
                    LoadGerman();
                    break;

                case "hu":
                    LoadHungarian();
                    break;

                case "cs":
                    LoadCzech();
                    break;

                case "fr":
                    LoadFrench();
                    break;

                case "pl":
                    LoadPolish();
                    break;

                case "ja":
                    LoadJapanese();
                    break;

                case "nl":
                    LoadDutch();
                    break;
                case "pt":
                    {
                        if (culture.Name == "pt-BR")
                        {
                            LoadPortugueseBrazilian();
                        }
                        else
                        {
                            LoadPortuguese();
                        }
                        break;
                    }

                case "es":
                    LoadSpanish();
                    break;

                case "zh":
                    LoadChinese();
                    break;

                case "sv":
                    LoadSwedish();
                    break;

                case "sk":
                    LoadSlovak();
                    break;

                case "ro":
                    LoadRomanian();
                    break;

                case "it":
                    LoadItalian();
                    break;

                case "ar":
                    LoadArabic();
                    break;

                case "da":
                    LoadDanish();
                    break;

                case "az":
                    LoadAzerbaijani();
                    break;

                case "fi":
                    LoadFinnish();
                    break;

                case "nb":
                case "nn":
                case "no":
                    LoadNorwegian();
                    break;

                case "tr":
                    LoadTurkish();
                    break;

                case "he":
                    LoadHebrew();
                    break;

                case "ge":
                    LoadGreek();
                    break;

                case "ko":
                    LoadKorean();
                    break;

                case "vi":
                    LoadVietnamese();
                    break;

                case "si":
                    LoadSinhala();
                    break;

                case "sl":
                    LoadSlovenian();
                    break;

                case "ca":
                    LoadCatalan();
                    break;

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
            RaisePropertyChanged("ScreenTipF1LabelHeader");
            RaisePropertyChanged("CustomizeStatusBar");
        }

        #endregion

        #region English

        private void LoadEnglish()
        {
            // Backstage button text & key tip
            _backstageButtonText = "File";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Collapse the Ribbon (Ctrl+F1)";
            _minimizeButtonScreenTipText = "Need a bit more space? Collapse the ribbon so only the tab names show.";
            _expandButtonScreenTipTitle = "Pin the Ribbon (Ctrl+F1)";
            _expandButtonScreenTipText = "Like seeing the ribbon? Keep it open while you work.";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Customize Quick Access Toolbar";
            _quickAccessToolBarMoreControlsButtonTooltip = "More controls"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Customize Quick Access Toolbar";
            _quickAccessToolBarMenuShowAbove = "Show Above the Ribbon";
            _quickAccessToolBarMenuShowBelow = "Show Below the Ribbon";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Add to Quick Access Toolbar"; // Button
            _ribbonContextMenuAddGroup = "Add Group to Quick Access Toolbar"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Add Gallery to Quick Access Toolbar"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Add Menu to Quick Access Toolbar"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Remove from Quick Access Toolbar"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
            _ribbonContextMenuShowBelow = "Show Quick Access Toolbar Below the Ribbon";
            _ribbonContextMenuShowAbove = "Show Quick Access Toolbar Above the Ribbon";
            _ribbonContextMenuCustomizeRibbon = "Customize the Ribbon...";
            _ribbonContextMenuMinimizeRibbon = "Collapse the Ribbon";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            //Screentips
            _screenTipDisableReasonHeader = "This command is currently disabled.";
            _screenTipF1LabelHeader = "Press F1 for help";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Customize Status Bar";
        }

        #endregion

        #region Russian

        private void LoadRussian()
        {
            _backstageButtonText = "Файл";
            _backstageButtonKeyTip = "Ф";

            _minimizeButtonScreenTipTitle = "Свернуть ленту (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только\nимена вкладок.";
            _expandButtonScreenTipTitle = "Развернуть ленту (Ctrl + F1)";
            _expandButtonScreenTipText = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только\nимена вкладок.";

            _quickAccessToolBarDropDownButtonTooltip = "Настройка панели быстрого доступа";
            _quickAccessToolBarMoreControlsButtonTooltip = "Другие элементы";
            _quickAccessToolBarMenuHeader = "Настройка панели быстрого доступа";
            _quickAccessToolBarMenuShowAbove = "Разместить над лентой";
            _quickAccessToolBarMenuShowBelow = "Разместить под лентой";

            _ribbonContextMenuAddItem = "Добавить на панель быстрого доступа";
            _ribbonContextMenuAddGroup = "Добавить группу на панель быстрого доступа";
            _ribbonContextMenuAddGallery = "Добавить коллекцию на панель быстрого доступа";
            _ribbonContextMenuAddMenu = "Добавить меню на панель быстрого доступа";
            _ribbonContextMenuRemoveItem = "Удалить с панели быстрого доступа";
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Настройка панели быстрого доступа...";
            _ribbonContextMenuShowBelow = "Разместить панель быстрого доступа под лентой";
            _ribbonContextMenuShowAbove = "Разместить панель быстрого доступа над лентой";
            _ribbonContextMenuCustomizeRibbon = "Настройка ленты...";
            _ribbonContextMenuMinimizeRibbon = "Свернуть ленту";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            _screenTipDisableReasonHeader = "В настоящее время эта команда отключена.";

            _customizeStatusBar = "Настройка строки состояния";
        }

        #endregion

        #region Ukrainian

        private void LoadUkrainian()
        {
            // Backstage button text & key tip 
            _backstageButtonText = "Файл";
            _backstageButtonKeyTip = "Ф";
            // See right-top corner... (two different tooltips must be if you press it) 
            _minimizeButtonScreenTipTitle = "Сховати Стрічку (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Показати або сховати Стрічку\n\nКоли стрічка схована, видно\nтільки назви вкладок";
            _expandButtonScreenTipTitle = "Показати Стрічку (Ctrl + F1)";
            _expandButtonScreenTipText = "Показати або сховати Стрічку\n\nКоли стрічка схована, видно\nтільки назви вкладок";
            // QAT tooltips and menu items 
            _quickAccessToolBarDropDownButtonTooltip = "Налаштувати Панель Інструментів Швидкого Доступу";
            _quickAccessToolBarMoreControlsButtonTooltip = "Більше елементів"; // When two arrows appear ">>" 
            _quickAccessToolBarMenuHeader = "Налаштувати Панель Інструментів Швидкого Доступу";
            _quickAccessToolBarMenuShowAbove = "Показати Поверх Стрічки";
            _quickAccessToolBarMenuShowBelow = "Показати Знизу Стрічки";
            // Click on Ribbon to show context menu 
            _ribbonContextMenuAddItem = "Додати до Панелі Інструментів Швидкого Доступу"; // Button 
            _ribbonContextMenuAddGroup = "Додати Групу до Панелі Інструментів Швидкого Доступу"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Додати Галерею до Панелі Інструментів Швидкого Доступу"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Додати Меню до Панелі Інструментів Швидкого Доступу"; // By dashed splitter in context menu 
            _ribbonContextMenuRemoveItem = "Видалити з Панелі Інструментів Швидкого Доступу"; // By item in QAT 
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Налаштувати Панель Інструментів Швидкого Доступу...";
            _ribbonContextMenuShowBelow = "Показати Панель Інструментів Швидкого Доступу Знизу Стрічки";
            _ribbonContextMenuShowAbove = "Показати Панель Інструментів Швидкого Доступу Поверх Стрічки";
            _ribbonContextMenuCustomizeRibbon = "Налаштувати Стрічку...";
            _ribbonContextMenuMinimizeRibbon = "Зменшити Стрічку";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot 
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Ця команда на даний момент недоступна.";
        }

        #endregion

        #region Persian

        private void LoadPersian()
        {
            // Backstage button text & key tip
            _backstageButtonText = "فایل";
            _backstageButtonKeyTip = "ف";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "کوچک کردن نوار (Ctrl + F1)";
            _minimizeButtonScreenTipText = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";
            _expandButtonScreenTipTitle = "بزرگ کردن نوار (Ctrl + F1)";
            _expandButtonScreenTipText = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "دلخواه سازی میله ابزار دسترسی سریع";
            _quickAccessToolBarMoreControlsButtonTooltip = "ابزارهای دیگر"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "دلخواه سازی میله ابزار دسترسی سریع";
            _quickAccessToolBarMenuShowAbove = "نمایش در بالای نوار";
            _quickAccessToolBarMenuShowBelow = "نمایش در پایین نوار";
            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "اضافه کردن به میله ابزار دسترسی سریع"; // Button
            _ribbonContextMenuAddGroup = "اضافه کردن گروه به میله ابزار دسترسی سریع"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "اضافه کردن گالری به میله ابزار دسترسی سریع"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "اضاقه کردن منو به میله ابزار دسترسی سریع"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "حذف از میله ابزار دسترسی سریع"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "دلخواه سازی میله ابزار دسترسی سریع...";
            _ribbonContextMenuShowBelow = "نمایش میله ابزار دسترسی سریع در پایین نوار";
            _ribbonContextMenuShowAbove = "نمایش میله ابزار دسترسی سریع در بالای نوار";
            _ribbonContextMenuCustomizeRibbon = "دلخواه سازی نوار...";
            _ribbonContextMenuMinimizeRibbon = "کوچک کردن نوار";
        }

        #endregion

        #region German

        private void LoadGerman()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Datei";
            _backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Menüband minimieren (Strg+F1)";
            _minimizeButtonScreenTipText = "Sie benötigen etwas mehr Platz? Reduzieren Sie das Menüband, sodass nur die Registerkartennamen angezeigt werden.";
            _expandButtonScreenTipTitle = "Menüband erweitern (Strg+F1)";
            _expandButtonScreenTipText = "Ist es Ihnen lieber, wenn Sie das Menüband sehen? Lassen Sie es während der Arbeit geöffnet.";

            // QAT tooltips and menu items
            _quickAccessToolBarDropDownButtonTooltip = "Symbolleiste für den Schnellzugriff anpassen";
            _quickAccessToolBarMoreControlsButtonTooltip = "Weitere Befehle…"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Symbolleiste für den Schnellzugriff anpassen";
            _quickAccessToolBarMenuShowAbove = "Über dem Menüband anzeigen";
            _quickAccessToolBarMenuShowBelow = "Unter dem Menüband anzeigen";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Zur Symbolleiste für den Schnellzugriff hinzufügen"; // Button
            _ribbonContextMenuAddGroup = "Gruppe zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Katalog zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Zur Symbolleiste für den Schnellzugriff hinzufügen"; // By dashed splitter in context menu

            _ribbonContextMenuRemoveItem = "Aus Symbolleiste für den Schnellzugriff entfernen"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Symbolleiste für den Schnellzugriff anpassen...";
            _ribbonContextMenuShowBelow = "Symbolleiste für den Schnellzugriff unter dem Menüband anzeigen";
            _ribbonContextMenuShowAbove = "Symbolleiste für den Schnellzugriff über dem Menüband anzeigen";
            _ribbonContextMenuCustomizeRibbon = "Menüband anpassen...";
            _ribbonContextMenuMinimizeRibbon = "Menüband minimieren";

            //Screentips
            _screenTipDisableReasonHeader = "Diese Funktion ist momentan deaktiviert.";
            _screenTipF1LabelHeader = "Drücken Sie F1 für die Hilfe";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Statusleiste anpassen";
        }

        #endregion

        #region Hungarian

        private void LoadHungarian()
        {
            // Backstage button text & key tip 
            _backstageButtonText = "Fájl";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "A menüszalag összecsukása (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Csak a lapnevek megjelenítése a menüszalagon";
            _expandButtonScreenTipTitle = "Menüszalag kibontása (Ctrl + F1)";
            _expandButtonScreenTipText = "A menüszalag megjelenítése úgy, hogy egy parancsra kattintás után is látható maradjon";

            // QAT tooltips and menu items
            _quickAccessToolBarDropDownButtonTooltip = "Gyorselérési eszköztár testreszabása";
            _quickAccessToolBarMoreControlsButtonTooltip = "További vezérlők"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Gyorselérési eszköztár testreszabása";
            _quickAccessToolBarMenuShowAbove = "Megjelenítés a menüszalag alatt";
            _quickAccessToolBarMenuShowBelow = "Megjelenítés a menüszalag felett";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Felvétel a gyorselérési eszköztárra"; // Button
            _ribbonContextMenuAddGroup = "Felvétel a gyorselérési eszköztárra"; // For ex., by collapsed group 
            _ribbonContextMenuAddGallery = "Gyűjtemény felvétele a gyorselérési eszköztárra"; // For ex., by opened font context menu 
            _ribbonContextMenuAddMenu = "Felvétel a gyorselérési eszköztárra"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Eltávolítás a gyorselérési eszköztárról"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Gyorselérési eszköztár testreszabása...";
            _ribbonContextMenuShowBelow = "A gyorselérési eszköztár megjelenítése a menüszalag alatt";
            _ribbonContextMenuShowAbove = "A gyorselérési eszköztár megjelenítése a menüszalag felett";
            _ribbonContextMenuCustomizeRibbon = "Menüszalag testreszabása...";
            _ribbonContextMenuMinimizeRibbon = " A menüszalag összecsukása";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3)
            _screenTipDisableReasonHeader = "Ez a parancs jelenleg nem használható.";
        }

        #endregion

        #region Czech

        private void LoadCzech()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Soubor";
            _backstageButtonKeyTip = "S";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Skrýt pás karet (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Zobrazit nebo skrýt pás karet\n\nJe-li pás karet skrytý, jsou\nzobrazeny pouze názvy karet";
            _expandButtonScreenTipTitle = "Zobrazit pás karet (Ctrl + F1)";
            _expandButtonScreenTipText = "Zobrazit nebo skrýt pás karet\n\nJe-li pás karet skrytý, jsou\nzobrazeny pouze názvy karet";

            // QAT tooltips and menu items 
            _quickAccessToolBarDropDownButtonTooltip = "Přizpůsobit panel nástrojů Rychlý přístup";
            _quickAccessToolBarMoreControlsButtonTooltip = "Další příkazy"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Přizpůsobit panel nástrojů Rychlý přístup";
            _quickAccessToolBarMenuShowAbove = "Zobrazit nad pásem karet";
            _quickAccessToolBarMenuShowBelow = "Zobrazit pod pásem karet";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Přidat na panel nástrojů Rychlý přístup"; // Button
            _ribbonContextMenuAddGroup = "Přidat na panel nástrojů Rychlý přístup"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Přidat galerii na panel nástrojů Rychlý přístup"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Přidat na panel nástrojů Rychlý přístup"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Odebrat z panelu nástrojů Rychlý přístup"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Přizpůsobit panel nástrojů Rychlý přístup...";
            _ribbonContextMenuShowBelow = "Zobrazit panel nástrojů Rychlý přístup pod pásem karet";
            _ribbonContextMenuShowAbove = "Zobrazit panel nástrojů Rychlý přístup nad pásem karet";
            _ribbonContextMenuCustomizeRibbon = "Přizpůsobit pás karet...";
            _ribbonContextMenuMinimizeRibbon = "Skrýt pás karet";

            _screenTipDisableReasonHeader = "Tento příkaz je aktuálně zakázán.";
        }

        #endregion

        #region French

        private void LoadFrench()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Fichier";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimiser le Ruban (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul\nles noms sont affichés";
            _expandButtonScreenTipTitle = "Agrandir le Ruban (Ctrl + F1)";
            _expandButtonScreenTipText = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul\nles noms sont affichés";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Personnaliser la barre d'outils Accès Rapide";
            _quickAccessToolBarMoreControlsButtonTooltip = "Plus de contrôles"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Personnaliser la barre d'outil Accès Rapide";
            _quickAccessToolBarMenuShowAbove = "Afficher au dessus du Ruban";
            _quickAccessToolBarMenuShowBelow = "Afficher en dessous du Ruban";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Ajouter un élément à la barre d'outils Accès Rapide"; // Button
            _ribbonContextMenuAddGroup = "Ajouter un groupe à la barre d'outils Accès Rapide"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Ajouter une galerie à la barre d'outils Accès Rapide"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Ajouter un menu à la barre d'outils Accès Rapide"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Supprimer de la barre d'outils Accès Rapide"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Personnaliser la barre d'outils Accès Rapide...";
            _ribbonContextMenuShowBelow = "Afficher la barre d'outils Accès Rapide en dessous du Ruban";
            _ribbonContextMenuShowAbove = "Afficher la barre d'outils Accès Rapide au dessus du Ruban";
            _ribbonContextMenuCustomizeRibbon = "Personnaliser le Ruban...";
            _ribbonContextMenuMinimizeRibbon = "Minimiser le Ruban";
            _customizeStatusBar = "Personnaliser la barre de statut";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            _screenTipDisableReasonHeader = "Cette commande est actuellement désactivée.";
        }

        #endregion

        #region Polish

        private void LoadPolish()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Plik";
            _backstageButtonKeyTip = "P";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimalizuj Wstążkę (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko\nnazwy zakładek są widoczne";
            _expandButtonScreenTipTitle = "Rozwiń Wstążkę (Ctrl + F1)";
            _expandButtonScreenTipText = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko\nnazwy zakładek są widoczne";

            // QAT tooltips and menu items
            _quickAccessToolBarDropDownButtonTooltip = "Dostosuj pasek narzędzi Szybki dostęp";
            _quickAccessToolBarMoreControlsButtonTooltip = "Więcej poleceń..."; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Dostosuj pasek narzędzi Szybki dostęp";
            _quickAccessToolBarMenuShowAbove = "Pokaż powyżej Wstążki";
            _quickAccessToolBarMenuShowBelow = "Pokaż poniżej Wstążki";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Dodaj do paska narzędzi Szybki dostęp"; // Button
            _ribbonContextMenuAddGroup = "Dodaj Grupę do paska narzędzi Szybki dostęp"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Dodaj Galerię do paska narzędzi Szybki dostęp"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Dodaj do paska narzędzi Szybki dostęp"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Usuń z paska narzędzi Szybki dostęp"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Dostosuj pasek narzędzi Szybki dostęp...";
            _ribbonContextMenuShowBelow = "Pokaż pasek Szybki dostęp poniżej Wstążki";
            _ribbonContextMenuShowAbove = "Pokaż pasek Szybki dostęp powyżej Wstążki";
            _ribbonContextMenuCustomizeRibbon = "Dostosuj Wstążkę...";
            _ribbonContextMenuMinimizeRibbon = "Minimalizuj Wstążkę";
        }

        #endregion

        #region Japanese

        private void LoadJapanese()
        {
            // Backstage button text & key tip
            _backstageButtonText = "ファイル";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "リボンの最小化 (Ctrl + F1)";
            _minimizeButtonScreenTipText = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";
            _expandButtonScreenTipTitle = "リボンの展開 (Ctrl + F1)";
            _expandButtonScreenTipText = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "クイック アクセス ツール バーのユーザー設定";
            _quickAccessToolBarMoreControlsButtonTooltip = "その他のボタン"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "クイック アクセス ツール バーのユーザー設定";
            _quickAccessToolBarMenuShowAbove = "リボンの上に表示";
            _quickAccessToolBarMenuShowBelow = "リボンの下に表示";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "クイック アクセス ツール バーに追加"; // Button
            _ribbonContextMenuAddGroup = "グループをクイック アクセス ツール バーに追加"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "ギャラリーをクイック アクセス ツール バーに追加"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "メニューをクイック アクセス ツール バーに追加"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "クイック アクセス ツール バーから削除"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "クイック アクセス ツール バーのユーザー設定...";
            _ribbonContextMenuShowBelow = "クイック アクセス ツール バーをリボンの下に表示";
            _ribbonContextMenuShowAbove = "クイック アクセス ツール バーをリボンの上に表示";
            _ribbonContextMenuCustomizeRibbon = "リボンのユーザー設定...";
            _ribbonContextMenuMinimizeRibbon = "リボンの最小化";
            _customizeStatusBar = "ステータス バーのユーザー設定";

            _screenTipDisableReasonHeader = "このコマンドは現在無効になっています";
        }

        #endregion

        #region Dutch

        private void LoadDutch()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Bestand";
            _backstageButtonKeyTip = "B";
            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Het lint minimaliseren (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, \nzijn alleen de tabulatie namen zichtbaar";
            _expandButtonScreenTipTitle = "Het lint Maximaliseren (Ctrl + F1)";
            _expandButtonScreenTipText = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is,\nzijn alleen de tabulatie namen zichtbaar";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Werkbalk snelle toegang aanpassen";
            _quickAccessToolBarMoreControlsButtonTooltip = "meer opdrachten"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = " Werkbalk snelle toegang aanpassen ";
            _quickAccessToolBarMenuShowAbove = "Boven het lint weergeven";
            _quickAccessToolBarMenuShowBelow = "beneden het lint weergeven";
            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Menu toevoegen aan werkbalk snelle toegang"; // Button
            _ribbonContextMenuAddGroup = "Groep toevoegen aan werkbalk snelle toegang"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Galerij toevoegen aan werkbalk snelle toegang"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = " Menu toevoegen aan werkbalk snelle toegang "; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = " Verwijder uit werkbalk snelle toegang "; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
            _ribbonContextMenuShowBelow = " Werkbalk snelle toegang onder het lint weergeven";
            _ribbonContextMenuShowAbove = " Werkbalk snelle toegang boven het lint weergeven ";
            _ribbonContextMenuCustomizeRibbon = "Lint aanpassen...";
            _ribbonContextMenuMinimizeRibbon = " Het lint minimaliseren";
        }

        #endregion

        #region Brazilian

        private void LoadPortugueseBrazilian()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Arquivo";
            _backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimizar o Ribbon (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Mostrar ou esconder o  Ribbon\n\nQuando o Ribbon estiver escondido, somente\no nome das abas serão mostrados";
            _expandButtonScreenTipTitle = "Expandir o Ribbon (Ctrl + F1)";
            _expandButtonScreenTipText = "Mostrar ou esconder o  Ribbon\n\nQuando o Ribbon estiver escondido, somente\no nome das abas serão mostrados";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Customizar Barra de acesso rápido";
            _quickAccessToolBarMoreControlsButtonTooltip = "Mais controles"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = " Customizar Barra de acesso rápido";
            _quickAccessToolBarMenuShowAbove = "Mostrar acima do Ribbon";
            _quickAccessToolBarMenuShowBelow = "Mostrar abaixo do Ribbon";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Adicionar para Barra de acesso rápido"; // Button
            _ribbonContextMenuAddGroup = " Adicionar o grupo para Barra de acesso rápido"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Adicionar a galeria para Barra de acesso rápido";
            // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = " Adicionar o menu para Barra de acesso rápido";
            // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Remover da Barra de acesso rápido"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Customizar Barra de acesso rápido...";
            _ribbonContextMenuShowBelow = "Mostrar Barra de acesso rápido abaixo do Ribbon";
            _ribbonContextMenuShowAbove = "Mostrar Barra de acesso rápido acima do Ribbon";
            _ribbonContextMenuCustomizeRibbon = "Customizar o Ribbon...";
            _ribbonContextMenuMinimizeRibbon = "Minimizar o Ribbon";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Este comando está desativado.";
        }

        #endregion

        #region Spanish

        private void LoadSpanish()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Archivo";
            _backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            // TRANSLATOR'S NOTE: This block is not shown at Windows 7's Apps (WordPad or Paint)
            _minimizeButtonScreenTipTitle = "Minimizar la cinta (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Muestra u oculta la cinta\n\nCuando la cinta está oculta, sólo\nse muestran los nombres de las pestañas";
            _expandButtonScreenTipTitle = "Expandir la cinta (Ctrl + F1)";
            _expandButtonScreenTipText = "Muestra u oculta la cinta\n\nCuando la cinta está oculta, sólo\nse muestran los nombres de las pestañas";

            // QAT tooltips and menu items
            _quickAccessToolBarDropDownButtonTooltip = "Personalizar barra de herramientas de acceso rápido";
            _quickAccessToolBarMoreControlsButtonTooltip = "Más controles"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Personalizar barra de herramientas de acceso rápido";
            _quickAccessToolBarMenuShowAbove = "Mostrar sobre la cinta";
            _quickAccessToolBarMenuShowBelow = "Mostrar bajo la cinta";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Agregar a la barra de herramientas de acceso rápido"; // Button
            _ribbonContextMenuAddGroup = "Agregar grupo a la barra de herramientas de acceso rápido"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Agregar galería a la barra de herramientas de acceso rápido"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Agregar menú a la barra de herramientas de acceso rápido"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Quitar de la barra de herramientas de acceso rápido"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar la barra de herramientas de acceso rápido...";
            _ribbonContextMenuShowBelow = "Mostrar barra de herramientas de acceso rápido bajo la cinta";
            _ribbonContextMenuShowAbove = "Mostrar barra de herramientas de acceso rápido sobre la cinta";
            _ribbonContextMenuCustomizeRibbon = "Personalizar la cinta...";
            _ribbonContextMenuMinimizeRibbon = "Minimizar la cinta";

            //Screentips
            _screenTipDisableReasonHeader = "Este comando está desactivado actualmente";
            _screenTipF1LabelHeader = "Pulse F1 para obtener más ayuda";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Personalizar barra de estado";
        }

        #endregion

        #region Chinese

        private void LoadChinese()
        {
            // Backstage button text & key tip
            _backstageButtonText = "文件";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "功能区最小化 (Ctrl + F1)";
            _minimizeButtonScreenTipText = "隐藏功能区时，仅显示选项卡名称";
            _expandButtonScreenTipTitle = "展开功能区 (Ctrl + F1)";
            _expandButtonScreenTipText = "隐藏功能区时，仅显示选项卡名称";

            // QAT tooltips and menu items         
            _quickAccessToolBarDropDownButtonTooltip = "自定义快速访问具栏";
            _quickAccessToolBarMoreControlsButtonTooltip = "其他命令"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "自定义快速访问工具栏";
            _quickAccessToolBarMenuShowAbove = "在功能区上方显示";
            _quickAccessToolBarMenuShowBelow = "在功能区下方显示";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "添加到快速访问工具栏"; // Button
            _ribbonContextMenuAddGroup = "在快速访问工具栏中添加组"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "在快速访问工具栏中添加样式"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "在快速访问工具栏中添加菜单"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "在快速访问工具栏中移除"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "自定义快速访问工具栏...";
            _ribbonContextMenuShowBelow = "在功能区下方显示快速访问工具栏";
            _ribbonContextMenuShowAbove = "在功能区上方显示快速访问工具栏";
            _ribbonContextMenuCustomizeRibbon = "自定义功能区...";
            _ribbonContextMenuMinimizeRibbon = "功能区最小化";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "此命令当前已被禁用。";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "自定义状态栏";
        }

        #endregion

        #region Swedish

        private void LoadSwedish()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Arkiv";
            _backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimera menyfliksområdet (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas\nendast flikarna";
            _expandButtonScreenTipTitle = "Expandera menyfliksområdet (Ctrl + F1)";
            _expandButtonScreenTipText = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas\nendast flikarna";

            // QAT tooltips and menu items          
            _quickAccessToolBarDropDownButtonTooltip = "Anpassa verktygsfältet Snabbåtkomst ";
            _quickAccessToolBarMoreControlsButtonTooltip = "Fler kommandon"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = " Anpassa verktygsfältet Snabbåtkomst";
            _quickAccessToolBarMenuShowAbove = "Visa ovanför menyfliksområdet";
            _quickAccessToolBarMenuShowBelow = "Visa under menyfliksområdet";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Lägg till i verktygsfältet Snabbåtkomst"; // Button
            _ribbonContextMenuAddGroup = "Lägg till i verktygsfältet Snabbåtkomst"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Lägg till galleriet i verktygsfältet Snabbåtkomst"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = " Lägg till menyn i verktygsfältet Snabbåtkomst "; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Ta bort från verktygsfältet Snabbåtkomst"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Anpassa verktygsfältet Snabbåtkomst...";
            _ribbonContextMenuShowBelow = " Visa verktygsfältet Snabbåtkomst under menyfliksområdet";
            _ribbonContextMenuShowAbove = " Visa verktygsfältet Snabbåtkomst ovanför menyfliksområdet ";
            _ribbonContextMenuCustomizeRibbon = "Anpassa menyfliksområdet...";
            _ribbonContextMenuMinimizeRibbon = "Minimera menyfliksområdet";
        }

        #endregion

        #region Slovak

        private void LoadSlovak()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Súbor";
            _backstageButtonKeyTip = "S";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Skryť pás s nástrojmi (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet";
            _expandButtonScreenTipTitle = "Zobraziť pás s nástrojmi (Ctrl + F1)";
            _expandButtonScreenTipText = " Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet ";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Prispôsobenie panela s nástrojmi Rýchly prístup";
            _quickAccessToolBarMoreControlsButtonTooltip = "Ďalšie príkazy"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Prispôsobenie panela s nástrojmi Rýchly prístup";
            _quickAccessToolBarMenuShowAbove = " Zobraziť nad pásom s nástrojmi ";
            _quickAccessToolBarMenuShowBelow = "Zobraziť pod pásom s nástrojmi";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Pridať na panel s nástrojmi Rýchly prístup"; // Button
            _ribbonContextMenuAddGroup = " Pridať na panel s nástrojmi Rýchly prístup "; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = " Pridať galériu do panela s nástrojmi Rýchly prístup "; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Pridať na panel s nástrojmi Rýchly prístup"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Odstrániť z panela s nástrojmi Rýchly prístup "; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = " Prispôsobenie panela s nástrojmi Rýchly prístup...";
            _ribbonContextMenuShowBelow = "Panel s nástrojmi Rýchly prístup zobraziť pod panelom s nástrojmi";
            _ribbonContextMenuShowAbove = "Panel s nástrojmi Rýchly prístup zobraziť nad panelom s nástrojmi ";
            _ribbonContextMenuCustomizeRibbon = "Prispôsobenie panela s nástrojmi Rýchly prístup...";
            _ribbonContextMenuMinimizeRibbon = "Minimalizovať pás s nástrojmi";
        }

        #endregion

        #region Romanian

        private void LoadRomanian()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Fișier";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimizează Ribbon-ul (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt\nafișate doar numele taburilor";
            _expandButtonScreenTipTitle = "Expandează Ribbon-ul (Ctrl + F1)";
            _expandButtonScreenTipText = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt\nafișate doar numele taburilor";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Personalizează Bara de Acces Rapid";
            _quickAccessToolBarMoreControlsButtonTooltip = "Mai multe controale"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Personalizează Bara de Acces Rapid";
            _quickAccessToolBarMenuShowAbove = "Afișează peste Ribbon";
            _quickAccessToolBarMenuShowBelow = "Afișează sub Ribbon";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Adaugă la Bara de Acess Rapid"; // Button
            _ribbonContextMenuAddGroup = "Adaugă Grupul la Bara de Acess Rapid"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Adaugă Galeria la Bara de Acess Rapid";
            // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Adaugă Meniul la Bara de Acess Rapid"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Eimină din Bara de Acess Rapid"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizează Bara de Acces Rapid...";
            _ribbonContextMenuShowBelow = "Afișează Bara de Acces Rapid sub Ribbon";
            _ribbonContextMenuShowAbove = "Afișează Bara de Acces Rapid peste Ribbon";
            _ribbonContextMenuCustomizeRibbon = "Personalizează Ribbon-ul...";
            _ribbonContextMenuMinimizeRibbon = "Minimizează Ribbon-ul...";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Această comandă nu este disponibilă momentan.";
        }

        #endregion

        #region Italian

        private void LoadItalian()
        {
            // Backstage button text & key tip
            _backstageButtonText = "File";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Riduci a icona barra multifunzione (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Consente di visualizzare solo i nomi \ndelle schede nella barra multifunzione.";
            _expandButtonScreenTipTitle = "Espandi la barra multifunzione (Ctrl + F1)";
            _expandButtonScreenTipText = "Visualizza la barra multifunzione in modo\n che rimanga sempre espansa, anche se l’utente \nha fatto click su un comando.";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Personalizza barra di accesso rapido";
            _quickAccessToolBarMoreControlsButtonTooltip = "Altri comandi…"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Personalizza barra di accesso rapido";
            _quickAccessToolBarMenuShowAbove = "Mostra sopra la barra multifunzione";
            _quickAccessToolBarMenuShowBelow = "Mostra sotto la barra multifunzione";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Aggiungi alla barra di accesso rapido"; // Button
            _ribbonContextMenuAddGroup = "Aggiungi gruppo alla barra di accesso rapido"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Aggiungi raccolta alla barra di accesso rapido"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Aggiungi menu alla barra di accesso rapido"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Rimuovi dalla barra di accesso rapido"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizza barra di accesso rapido...";
            _ribbonContextMenuShowBelow = "Mostra la barra di accesso rapido sotto la barra multifunzione";
            _ribbonContextMenuShowAbove = "Mostra la barra di accesso rapido sopra la barra multifunzione";
            _ribbonContextMenuCustomizeRibbon = "Personalizza barra multifunzione...";
            _ribbonContextMenuMinimizeRibbon = "Riduci a icona barra multifunzione";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Questo commando è disattivato.";
        }

        #endregion

        #region Arabic

        private void LoadArabic()
        {
            // Backstage button text & key tip
            _backstageButtonText = "ملف    ";
            _backstageButtonKeyTip = "م    ";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "(Ctrl + F1)تصغير الشريط ";
            _minimizeButtonScreenTipText = "إظهار أسماء علامات التبويب فقط على الشريط.";
            _expandButtonScreenTipTitle = "(Ctrl + F1)توسيع الشريط ";
            _expandButtonScreenTipText = "إظهار الشريط بحيث يكون موسعاً دائماً حتى بعد النقر فوق أمر.";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "تخصيص شريط أدوات الوصول السريع";
            _quickAccessToolBarMoreControlsButtonTooltip = "أوامر إضافية"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "تخصيص شريط أدوات الوصول السريع";
            _quickAccessToolBarMenuShowAbove = "إظهار أعلى الشريط";
            _quickAccessToolBarMenuShowBelow = "إظهار أسفل الشريط";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "إضافة إلى شريط أدوات الوصول السريع"; // Button
            _ribbonContextMenuAddGroup = "إضافة إلى شريط أدوات الوصول السريع"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "إضافة إلى شريط أدوات الوصول السريع"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "إضافة إلى شريط أدوات الوصول السريع"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "إزالة إلى شريط أدوات الوصول السريع"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "تخصيص شريط أدوات الوصول السريع...";
            _ribbonContextMenuShowBelow = "إظهار شريط أدوات الوصول السريع أسفل الشريط";
            _ribbonContextMenuShowAbove = "إظهار شريط أدوات الوصول السريع أعلى الشريط";
            _ribbonContextMenuCustomizeRibbon = "تخصيص الشريط...";
            _ribbonContextMenuMinimizeRibbon = "تصغير الشريط";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "تم حالياً تعطيل هذا الأمر.";
        }

        #endregion

        #region Danish

        private void LoadDanish()
        {
            // Backstage button text & key 
            _backstageButtonText = "Filer";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimer båndet (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Vis kun fanenavnene på båndet.";
            _expandButtonScreenTipTitle = "Udvid båndet (Ctrl + F1)";
            _expandButtonScreenTipText = "Vis båndet, så det altid er udvidet, selv\nnår du klikker på en kommando.";

            // QAT tooltips and menu items          
            _quickAccessToolBarDropDownButtonTooltip = "Tilpas værktøjslinjen Hurtig adgang";
            _quickAccessToolBarMoreControlsButtonTooltip = "Flere kontrolelementer"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = " Tilpas værktøjslinjen Hurtig adgang";
            _quickAccessToolBarMenuShowAbove = "Vis ovenover båndet";
            _quickAccessToolBarMenuShowBelow = "Vis under båndet";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Føj til værktøjslinjen Hurtig adgang"; // Button
            _ribbonContextMenuAddGroup = "Føj til værktøjslinjen Hurtig adgang"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Tilføj Galleri til værktøjslinjen Hurtig adgang"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Føj til værktøjslinjen Hurtig adgang"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Fjern fra værktøjslinjen Hurtig adgang"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Tilpas værktøjslinjen Hurtig adgang...";
            _ribbonContextMenuShowBelow = "Vis værktøjslinjen Hurtig adgang under båndet";
            _ribbonContextMenuShowAbove = "Vis værktøjslinjen Hurtig adgang ovenover båndet";
            _ribbonContextMenuCustomizeRibbon = "Tilpas båndet...";
            _ribbonContextMenuMinimizeRibbon = "Minimer båndet";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Denne kommando er aktuelt deaktiveret.";
        }

        #endregion

        #region Portuguese

        private void LoadPortuguese()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Ficheiro";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimizar o Friso (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Mostrar apenas os nomes dos separadores\n no Frisos.";
            _expandButtonScreenTipTitle = "Expandir o Friso (Ctrl + F1)";
            _expandButtonScreenTipText = "Mostrar o Friso de modo a aparecer sempre\nexpandido mesmo depois de clicar num\ncomando.";

            // QAT tooltips and menu items           
            _quickAccessToolBarDropDownButtonTooltip = "Personalizar Barra de Ferramentas de Acesso Rápido";
            _quickAccessToolBarMoreControlsButtonTooltip = "Mais Comandos..."; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Personalizar Barra de Ferramentas de Acesso Rápido";
            _quickAccessToolBarMenuShowAbove = "Mostrar Acima do Friso";
            _quickAccessToolBarMenuShowBelow = "Mostrar Abaixo do Friso";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Adicionar à Barra de Ferramentas de Acesso Rápido";
            _ribbonContextMenuAddGroup = "Adicionar Grupo à Barra de Ferramentas de Acesso Rápido";
            _ribbonContextMenuAddGallery = "Adicionar Galeria à Barra de Ferramentas de Acesso Rápido";
            _ribbonContextMenuAddMenu = "Adicionar Menu à Barra de Ferramentas de Acesso Rápido";
            _ribbonContextMenuRemoveItem = "Remover da Barra de Ferramentas de Acesso Rápido";
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar Barra de Ferramentas de Acesso Rápido...";
            _ribbonContextMenuShowBelow = "Mostrar Barra de Ferramentas de Acesso Rápido Abaixo do Friso";
            _ribbonContextMenuShowAbove = "Mostrar Barra de Ferramentas de Acesso Rápido Acima do Friso";
            _ribbonContextMenuCustomizeRibbon = "Personalizar o Friso...";
            _ribbonContextMenuMinimizeRibbon = "Minimizar o Friso";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3)
            _screenTipDisableReasonHeader = "Este comando está desactivado actualmente.";
        }

        #endregion

        #region Azerbaijani

        private void LoadAzerbaijani()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Fayl";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Menyu lentini kiçilt (Ctrl + F1)";
            _minimizeButtonScreenTipText = " Menyu lentini göstər və ya gizlət\n\n Menyu lentini kiçiləndə, yalnız\n tabların adları göstərilir";
            _expandButtonScreenTipTitle = " Menyu lentini böyüt(Ctrl + F1)";
            _expandButtonScreenTipText = " Menyu lentini göstər və ya gizlət\n\n Menyu lentini gizldəndə, yalnız, \n tabların adları göstərilir";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Sürətli Keçidin Alətlərini fərdiləşdir";
            _quickAccessToolBarMoreControlsButtonTooltip = "Digər nəzarət vasitələri"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = " Sürətli Keçidin Alətlərini fərdiləşdir ";
            _quickAccessToolBarMenuShowAbove = "Menyu lentinin üstündə göstər";
            _quickAccessToolBarMenuShowBelow = " Menyu lentinin altında göstər ";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Sürətli Keçidin Alətlərinə əlavə et"; // Button
            _ribbonContextMenuAddGroup = " Sürətli Keçidin Alətlərinə Qrup əlavə et "; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = " Sürətli Keçidin Alətlərinə Qalereya əlavə et"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = " Sürətli Keçidin Alətlərinə Menyu əlavə et"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = " Sürətli Keçidin Alətlərindən sil"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = " Sürətli Keçidin Alətlərini fərdiləşdir...";
            _ribbonContextMenuShowBelow = " Sürətli Keçidin Alətlərini Menyu lentinin altında göstər ";
            _ribbonContextMenuShowAbove = " Sürətli Keçidin Alətlərini Menyu lentinin üstündə göstər ";
            _ribbonContextMenuCustomizeRibbon = "Menyu lentini fərdiləşdir...";
            _ribbonContextMenuMinimizeRibbon = " Menyu lentini kiçilt";
        }

        #endregion

        #region Finnish

        private void LoadFinnish()
        {
            _backstageButtonText = "Tiedosto";
            _backstageButtonKeyTip = "T";
            _minimizeButtonScreenTipTitle = "Pienennä valintanauha (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Näytä valintanauhassa vain\nvälilehtien nimet";
            _expandButtonScreenTipTitle = "Laajenna valintanauha (Ctrl + F1)";
            _expandButtonScreenTipText = "Näytä valintanauha aina\nlaajennettuna silloinkin, kun\nvalitset komennon";
            _quickAccessToolBarDropDownButtonTooltip = "Mukauta pikatyökaluriviä";
            _quickAccessToolBarMoreControlsButtonTooltip = "Lisää valintoja";
            _quickAccessToolBarMenuHeader = "Mukauta pikatyökaluriviä";
            _quickAccessToolBarMenuShowAbove = "Näytä valintanauhan yläpuolella";
            _quickAccessToolBarMenuShowBelow = "Näytä valintanauhan alapuolella";
            _ribbonContextMenuAddItem = "Lisää pikatyökaluriville";
            _ribbonContextMenuAddGroup = "Lisää ryhmä pikatyökaluriviin";
            _ribbonContextMenuAddGallery = "Lisää valikoima pikatyökaluriviin";
            _ribbonContextMenuAddMenu = "Lisää valikko pikatyökaluriviin";
            _ribbonContextMenuRemoveItem = "Poista pikatyökaluriviltä";
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Mukauta pikatyökaluriviä...";
            _ribbonContextMenuShowBelow = "Näytä pikatyökalurivi valintanauhan alapuolella";
            _ribbonContextMenuShowAbove = "Näytä pikatyökalurivi valintanauhan yläpuolella";
            _ribbonContextMenuCustomizeRibbon = "Mukauta valintanauhaa...";
            _ribbonContextMenuMinimizeRibbon = "Pienennä valintanauha";
            _screenTipDisableReasonHeader = "Tämä komento on tällä hetkellä poissa käytöstä";
        }

        #endregion

        #region Norwegian

        private void LoadNorwegian()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Fil";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimer båndet (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Viser bare kategorinavnene på båndet";
            _expandButtonScreenTipTitle = "Utvider båndet (Ctrl + F1)";
            _expandButtonScreenTipText = "Vis båndet slik at det alltid er utvidet selv etter at du har valgt en kommando";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Tilpass verktøylinje for hurtigtilgang";
            _quickAccessToolBarMoreControlsButtonTooltip = "Flere kontroller"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Tilpass verktøylinje for hurtigtilgang";
            _quickAccessToolBarMenuShowAbove = "Vis over båndet";
            _quickAccessToolBarMenuShowBelow = "Vis under båndet";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Legg til på verktøylinje for hurtigtilgang"; // Button
            _ribbonContextMenuAddGroup = "Legg til gruppe på verktøylinje for hurtigtilgang"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Legg til galleri på verktøylinje for hurtigtilgang"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Legg til meny på verktøylinje for hurtigtilgang"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Fjern verktøylinjen for hurtigtilgang"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Tilpass verktøylinje for hurtigtilgang...";
            _ribbonContextMenuShowBelow = "Vis verktøylinjen for hurtigtilgang under båndet";
            _ribbonContextMenuShowAbove = "Vis verktøylinjen for hurtigtilgang over båndet";
            _ribbonContextMenuCustomizeRibbon = "Tilpass båndet...";
            _ribbonContextMenuMinimizeRibbon = "Minimer båndet";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Denne kommandoen er for øyeblikket deaktivert.";
        }

        #endregion

        #region Turkish

        private void LoadTurkish()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Dosya";
            _backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Şeridi Daralt (Ctrl+F1)";
            _minimizeButtonScreenTipText = "Daha fazla alana mı\nihtiyacınız var? Şeridi\ndaraltın, yalnızca sekme\nisimleri görünsün.";
            _expandButtonScreenTipTitle = "Şeridi Sabitle (Ctrl+F1)";
            _expandButtonScreenTipText = "Şeridi görmek mi\nistiyorsunuz? Çalışırken\naçık tutun.";

            // QAT tooltips and menu items           
            _quickAccessToolBarDropDownButtonTooltip = "Hızlı Erişim Araç Çubuğu'nu Özelleştir";
            _quickAccessToolBarMoreControlsButtonTooltip = "Diğer denetimler"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Hızlı Erişim Araç Çubuğu'nu Özelleştir";
            _quickAccessToolBarMenuShowAbove = "Şeridin Üstünde Göster";
            _quickAccessToolBarMenuShowBelow = "Şeridin Altında Göster";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Hızlı Erişim Araç Çubuğu'na Ekle"; // Button
            _ribbonContextMenuAddGroup = "Grubu Hızlı Erişim Araç Çubuğu'na Ekle"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Galeriyi Hızlı Erişim Araç Çubuğu'na Ekle"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Menüyü Hızlı Erişim Araç Çubuğu'na Ekle"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Hızlı Erişim Araç Çubuğu'ndan Kaldır"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Hızlı Erişim Araç Çubuğu'nu Özelleştir";
            _ribbonContextMenuShowBelow = "Hızlı Erişim Araç Çubuğu'nu Şeridin Altında Göster";
            _ribbonContextMenuShowAbove = "Hızlı Erişim Araç Çubuğu'nu Şeridin Üstünde Göster";
            _ribbonContextMenuCustomizeRibbon = "Şeridi Özelleştir...";
            _ribbonContextMenuMinimizeRibbon = "Şeridi Daralt";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Bu komut şu anda devre dışı";
            _screenTipF1LabelHeader = "Yardım için F1'e basın.";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Durum Çubuğunu Özelleştir";
        }

        #endregion

        #region Hebrew

        private void LoadHebrew()
        {
            // Backstage button text & key tip
            _backstageButtonText = "קובץ";
            _backstageButtonKeyTip = "ק";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "מזער את רצועת הכלים (Ctrl + F1)";
            _minimizeButtonScreenTipText = "הצג רק את שמות הכרטיסיות\nברצועת הכלים.";
            _expandButtonScreenTipTitle = "הרחב את רצועת הכלים (Ctrl + F1)";
            _expandButtonScreenTipText = "הצג את רצועת הכלים כשהיא\nמורחבת תמיד, גם לאחר\nשתלחץ על הפקודה.";

            // QAT tooltips and menu items           
            _quickAccessToolBarDropDownButtonTooltip = "התאמה אישית של סרגל הכלים לגישה מהירה";
            _quickAccessToolBarMoreControlsButtonTooltip = "פקודות נוספות"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "התאמה אישית של סרגל הכלים לגישה מהירה";
            _quickAccessToolBarMenuShowAbove = "הצג מעל לרצועת הכלים";
            _quickAccessToolBarMenuShowBelow = "הצג מעל לרצועת הכלים";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "הוסף לסרגל הכלים לגישה מהירה"; // Button
            _ribbonContextMenuAddGroup = "הוסף קבוצה לסרגל הכלים לגישה מהירה"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "הוסף גלריה לסרגל הכלים לגישה מהירה"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "הוסף תפריט לסרגל הכלים לגישה מהירה"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "הסר מסרגל הכלים לגישה מהירה"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "של סרגל הכלים רצועת הכלים...";
            _ribbonContextMenuShowBelow = "הצג את סרגל הכלים לגישה מהירה מתחת לרצועת הכלים";
            _ribbonContextMenuShowAbove = "הצג את סרגל הכלים לגישה מהירה מעל לרצועת הכלים";
            _ribbonContextMenuCustomizeRibbon = "התאמה אישית של רצועת הכלים...";
            _ribbonContextMenuMinimizeRibbon = "מזער את רצועת הכלים";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "פקודה זו אינה זמינה כעת.";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "התאמה אישית של שורת המצב";
        }

        #endregion

        #region Greek

        private void LoadGreek()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Αρχείο";
            _backstageButtonKeyTip = "Α";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Ελαχιστοποίηση της Κορδέλας (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Εμφάνιση μόνο των ονομάτων καρτελών στην\nΚορδέλα.";
            _expandButtonScreenTipTitle = "Ανάπτυξη της Κορδέλας (Ctrl + F1)";
            _expandButtonScreenTipText = "Εμφάνιση της Κορδέλας προκειμένου να αναπτύσσεται\nπάντα, ακόμα και αφού κάνετε κλικ σε μια εντολή.";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης";
            _quickAccessToolBarMoreControlsButtonTooltip = "Περισσότερες εντολές"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης";
            _quickAccessToolBarMenuShowAbove = "Εμφάνιση πάνω από την Κορδέλα";
            _quickAccessToolBarMenuShowBelow = "Εμφάνιση κάτω από την Κορδέλα";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Προσθήκη στη γραμμή εργαλείων γρήγορης πρόσβασης"; // Button
            _ribbonContextMenuAddGroup = "Προσθήκη ομάδας στη γραμμή εργαλείων γρήγορης πρόσβασης"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Προσθήκη συλλογής στη γραμμή εργαλείων γρήγορης πρόσβασης"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Προσθήκη μενού στη γραμμή εργαλείων γρήγορης πρόσβασης"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Κατάργηση από τη γραμμή εργαλείων γρήγορης πρόσβασης"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης...";
            _ribbonContextMenuShowBelow = "Εμφάνιση της γραμμής εργαλείων γρήγορης πρόσβασης κάτω από την Κορδέλα";
            _ribbonContextMenuShowAbove = "Εμφάνιση της γραμμής εργαλείων γρήγορης πρόσβασης πάνω από την Κορδέλα";
            _ribbonContextMenuCustomizeRibbon = "Προσαρμογή της Κορδέλας...";
            _ribbonContextMenuMinimizeRibbon = "Ελαχιστοποίηση της Κορδέλας";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Αυτή η εντολή είναι απενεργοποιημένη προς το παρόν.";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Προσαρμογή γραμμής κατάστασης";
        }

        #endregion

        #region Korean

        private void LoadKorean()
        {
            // Backstage button text & key tip
            _backstageButtonText = "파일";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "리본 메뉴를 최소화 합니다 (Ctrl + F1)";
            _minimizeButtonScreenTipText = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";
            _expandButtonScreenTipTitle = "리본 메뉴를 표시합니다 (Ctrl + F1)";
            _expandButtonScreenTipText = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "빠른 실행 도구 모음 사용자 지정";
            _quickAccessToolBarMoreControlsButtonTooltip = "기타 컨트롤들"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "빠른 실행 도구 모음 사용자 지정";
            _quickAccessToolBarMenuShowAbove = "리본 메뉴 위에 표시";
            _quickAccessToolBarMenuShowBelow = "리본 메뉴 아래에 표시";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "빠른 실행 도구 모음에 추가"; // Button
            _ribbonContextMenuAddGroup = "그룹을 빠른 실행 도구 모음에 추가"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "갤러리를 빠른 실행 도구 모음에 추가"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "메뉴를 빠른 실행 도구 모음에 추가"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "빠른 실행 도구 모음에서 단추 제거"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "빠른 실행 도구 모음 사용자 지정...";
            _ribbonContextMenuShowBelow = "리본 메뉴 아래에 빠른 실행 도구 모음 표시";
            _ribbonContextMenuShowAbove = "리본 메뉴 위에 빠른 실행 도구 모음 표시";
            _ribbonContextMenuCustomizeRibbon = "리본 메뉴 사용자 지정...";
            _ribbonContextMenuMinimizeRibbon = "리본 메뉴 최소화";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "이 명령은 현재 사용할 수 없습니다.";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "상태 표시줄 사용자 지정";
        }

        #endregion

        #region Vietnamese

        private void LoadVietnamese()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Tệp";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Thu gọn Ruy băng (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ\ncó tên thẻ được hiện";
            _expandButtonScreenTipTitle = "Mở rộng Ruy băng (Ctrl + F1)";
            _expandButtonScreenTipText = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ\ncó tên thẻ được hiện";

            // QAT tooltips and menu items
            _quickAccessToolBarDropDownButtonTooltip = "Tùy chỉnh thanh công cụ Truy cập nhanh";
            _quickAccessToolBarMoreControlsButtonTooltip = "Thêm điều khiển"; // khi có hai mũi tên ">>"
            _quickAccessToolBarMenuHeader = "Tùy chỉnh thanh công cụ Truy cập nhanh";
            _quickAccessToolBarMenuShowAbove = "Hiện trên thanh Ruy băng";
            _quickAccessToolBarMenuShowBelow = "Hiện dưới thanh Ruy băng";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Thêm vào thanh công cụ Truy cập nhanh"; // Button
            _ribbonContextMenuAddGroup = "Thêm nhóm vào thanh công cụ Truy cập nhanh"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Thêm bộ sưu tập vào thanh công cụ Truy cập nhanh"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Thêm menu vào thanh công cụ Truy cập nhanh"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Loại"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Tùy chỉnh thanh công cụ Truy cập nhanh...";
            _ribbonContextMenuShowBelow = "Hiện thanh công cụ truy cập nhanh dưới thanh Ruy băng";
            _ribbonContextMenuShowAbove = "Hiện thanh công cụ truy cập nhanh trên thanh Ruy băng";
            _ribbonContextMenuCustomizeRibbon = "Tùy biến thanh Ruy băng...";
            _ribbonContextMenuMinimizeRibbon = "Thu gọn Ruy băng";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Lệnh này hiện bị tắt.";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Tùy biến thanh Trạng thái";
        }

        #endregion

        #region Sinhala (Sri Lanka)

        private void LoadSinhala()
        {
            // Backstage button text & key tip
            _backstageButtonText = "ගොනුව";
            _backstageButtonKeyTip = "න1";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "රිබනය හකුළන්න (Ctrl + F1)";
            _minimizeButtonScreenTipText = "රිබනය මත පටිති නාම පමණක් පෙන්වන්න.";
            _expandButtonScreenTipTitle = "රිබනය විහිදන්න (Ctrl + F1)";
            _expandButtonScreenTipText = "රිබනය පෙන්වන්න, එවිට ඔබ\n\n විධානයක් ක්ලික් කළද එය\n\n සැමවිටම විහිදී පවතී.";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය";
            _quickAccessToolBarMoreControlsButtonTooltip = "තවත් විධාන"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය";
            _quickAccessToolBarMenuShowAbove = "රිබනයට ඉහලින් පෙන්වන්න";
            _quickAccessToolBarMenuShowBelow = "රිබනයට පහලින් පෙන්වන්න";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට එක් කරන්න"; // Button
            _ribbonContextMenuAddGroup = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට එක් කරන්න"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට ගැලරිය එක් කරන්න"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට මෙනුව එක් කරන්න"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයෙන් ඉවත් කරන්න"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය කරන්න...";
            _ribbonContextMenuShowBelow = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය රිබනයට පහලින් පෙන්වන්න";
            _ribbonContextMenuShowAbove = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය රිබනයට ඉහලින් පෙන්වන්න";
            _ribbonContextMenuCustomizeRibbon = "රිබනය අභිමත කරණය කරන්න...";
            _ribbonContextMenuMinimizeRibbon = "රිබනය හකුළන්න";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "මෙම විධානය දැනට භාවිතා කළ නොහැක";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "තත්ව තීරුව අභිමත කරණය";
        }

        #endregion

        #region Slovenian

        private void LoadSlovenian()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Datoteka";
            _backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            _minimizeButtonScreenTipTitle = "Minimiraj trak (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Pokaži ali skrij trak\n\nKo je trak skrit, so\nprikazani samo zavihki";
            _expandButtonScreenTipTitle = "Razširi trak (Ctrl + F1)";
            _expandButtonScreenTipText = "Pokaži ali skrij trak\n\nKo je trak skrit, so\nprikazani samo zavihki";

            // QAT tooltips and menu items            
            _quickAccessToolBarDropDownButtonTooltip = "Prilagodi orodno vrstico za hitri dostop";
            _quickAccessToolBarMoreControlsButtonTooltip = "Več ukazov"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Prilagodi orodno vrstico za hitri dostop";
            _quickAccessToolBarMenuShowAbove = "Pokaži nad trakom";
            _quickAccessToolBarMenuShowBelow = "Pokaži pod trakom";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Dodaj v orodno vrstico za hitri dostop"; // Button
            _ribbonContextMenuAddGroup = "Dodaj skupino orodni vrstici za hitri dostop"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Dodaj galerijo orodni vrstici za hitri dostop"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Dodaj meni orodni vrstici za hitri dostop"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Odstrani iz orodne vrstice za hitri dostop"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Prilagodi orodno vrstico za hitri dostop...";
            _ribbonContextMenuShowBelow = "Pokaži orodno vrstico za hitri dostop pod trakom";
            _ribbonContextMenuShowAbove = "Pokaži orodno vrstico za hitri dostop nad trakom";
            _ribbonContextMenuCustomizeRibbon = "Prilagodi trak...";
            _ribbonContextMenuMinimizeRibbon = "Minimiraj trak";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            _screenTipDisableReasonHeader = "Ta ukaz je trenutno onemogočen.";

            // Right-click on status bar to see it. NEW! from v2.0
            _customizeStatusBar = "Prilagodi vrstico stanja";
        }

        #endregion

        #region Catalan

        private void LoadCatalan()
        {
            // Backstage button text & key tip
            _backstageButtonText = "Fitxer";
            _backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            // TRANSLATOR'S NOTE: This block is not shown at Windows 7's Apps (WordPad or Paint)
            _minimizeButtonScreenTipTitle = "Minimitza la cinta (Ctrl + F1)";
            _minimizeButtonScreenTipText = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només\ns'ensenyen els noms de les pestanyes";
            _expandButtonScreenTipTitle = "Expandeix la cinta (Ctrl + F1)";
            _expandButtonScreenTipText = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només\ns'ensenyen els noms de les pestanyes";

            // QAT tooltips and menu items
            _quickAccessToolBarDropDownButtonTooltip = "Personalitza la barra d'eines d'accés ràpid";
            _quickAccessToolBarMoreControlsButtonTooltip = "Més controls"; // When two arrows appear ">>"
            _quickAccessToolBarMenuHeader = "Personalitza la barra d'eines d'accés ràpid";
            _quickAccessToolBarMenuShowAbove = "Mostra sobre la cinta";
            _quickAccessToolBarMenuShowBelow = "Mostra sota la cinta";

            // Click on Ribbon to show context menu
            _ribbonContextMenuAddItem = "Afegeix a la barra d'eines d'accés ràpid"; // Button
            _ribbonContextMenuAddGroup = "Afegeix grup a la barra d'eines d'accés ràpid"; // For ex., by collapsed group
            _ribbonContextMenuAddGallery = "Afegeix galeria a la barra d'eines d'accés ràpid"; // For ex., by opened font context menu
            _ribbonContextMenuAddMenu = "Afegeix menú a la barra d'eines d'accés ràpid"; // By dashed splitter in context menu
            _ribbonContextMenuRemoveItem = "Elimina la barra d'eines d'accés ràpid"; // By item in QAT
            _ribbonContextMenuCustomizeQuickAccessToolbar = "Personalitza la barra d'eines d'accés ràpid...";
            _ribbonContextMenuShowBelow = "Mostra la barra d'eines d'accés ràpid sota la cinta";
            _ribbonContextMenuShowAbove = "Mostra la barra d'eines d'accés ràpid sobre la cinta";
            _ribbonContextMenuCustomizeRibbon = "Personalitza la cinta...";
            _ribbonContextMenuMinimizeRibbon = "Minimitza la cinta";
        }

        #endregion
    }
}