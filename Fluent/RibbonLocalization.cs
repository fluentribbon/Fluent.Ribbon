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
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Culture

        private CultureInfo culture;

        /// <summary>
        /// Gets or sets current culture used for localization
        /// </summary>
        public CultureInfo Culture
        {
            get { return this.culture; }
            set
            {
                if (!Equals(this.culture, value))
                {
                    this.culture = value;
                    this.LoadCulture(this.culture);
                    this.RaisePropertyChanged("Culture");
                }
            }
        }

        #endregion

        #region Text of backstage button

        // Text of backstage button
        private string backstageButtonText;

        /// <summary>
        /// Gets or sets text of backstage button
        /// </summary>
        public string BackstageButtonText
        {
            get { return this.backstageButtonText; }
            set
            {
                if (this.backstageButtonText != value)
                {
                    this.backstageButtonText = value;
                    this.RaisePropertyChanged("BackstageButtonText");
                }
            }
        }

        #endregion

        #region KeyTip of backstage button

        // KeyTip of backstage button
        private string backstageButtonKeyTip;

        /// <summary>
        /// Gets or sets KeyTip of backstage button
        /// </summary>
        public string BackstageButtonKeyTip
        {
            get { return this.backstageButtonKeyTip; }
            set
            {
                if (this.backstageButtonKeyTip != value)
                {
                    this.backstageButtonKeyTip = value;
                    this.RaisePropertyChanged("BackstageButtonKeyTip");
                }
            }
        }

        #endregion

        #region Minimize Button ScreenTip Title

        // Minimize Button ScreenTip Title
        private string minimizeButtonScreenTipTitle;

        /// <summary>
        /// Minimize Button ScreenTip Title
        /// </summary>
        public string MinimizeButtonScreenTipTitle
        {
            get { return this.minimizeButtonScreenTipTitle; }
            set
            {
                if (this.minimizeButtonScreenTipTitle != value)
                {
                    this.minimizeButtonScreenTipTitle = value;
                    this.RaisePropertyChanged("MinimizeButtonScreenTipTitle");
                }
            }
        }

        #endregion

        #region Minimize Button ScreenTip Text

        // Minimize Button ScreenTip Text
        private string minimizeButtonScreenTipText;

        /// <summary>
        /// Minimize Button ScreenTip Text
        /// </summary>
        public string MinimizeButtonScreenTipText
        {
            get { return this.minimizeButtonScreenTipText; }
            set
            {
                if (this.minimizeButtonScreenTipText != value)
                {
                    this.minimizeButtonScreenTipText = value;
                    this.RaisePropertyChanged("MinimizeButtonScreenTipText");
                }
            }
        }

        #endregion

        #region Expand Button ScreenTip Title

        // Expand Button ScreenTip Title
        private string expandButtonScreenTipTitle;

        /// <summary>
        /// Expand Button ScreenTip Title
        /// </summary>
        public string ExpandButtonScreenTipTitle
        {
            get { return this.expandButtonScreenTipTitle; }
            set
            {
                if (this.expandButtonScreenTipTitle != value)
                {
                    this.expandButtonScreenTipTitle = value;
                    this.RaisePropertyChanged("ExpandButtonScreenTipTitle");
                }
            }
        }

        #endregion

        #region Expand Button ScreenTip Text

        // Expand Button ScreenTip Text
        private string expandButtonScreenTipText;

        /// <summary>
        /// Expand Button ScreenTip Text
        /// </summary>
        public string ExpandButtonScreenTipText
        {
            get { return this.expandButtonScreenTipText; }
            set
            {
                if (this.expandButtonScreenTipText != value)
                {
                    this.expandButtonScreenTipText = value;
                    this.RaisePropertyChanged("ExpandButtonScreenTipText");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar DropDown Button ToolTip

        // Quick Access ToolBar DropDown Button ToolTip
        private string quickAccessToolBarDropDownButtonTooltip;

        /// <summary>
        /// Quick Access ToolBar DropDown Button ToolTip
        /// </summary>
        public string QuickAccessToolBarDropDownButtonTooltip
        {
            get { return this.quickAccessToolBarDropDownButtonTooltip; }
            set
            {
                if (this.quickAccessToolBarDropDownButtonTooltip != value)
                {
                    this.quickAccessToolBarDropDownButtonTooltip = value;
                    this.RaisePropertyChanged("QuickAccessToolBarDropDownButtonTooltip");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar MoreControls Button ToolTip

        // Quick Access ToolBar MoreControls Button ToolTip
        private string quickAccessToolBarMoreControlsButtonTooltip;

        /// <summary>
        /// Quick Access ToolBar MoreControls Button ToolTip
        /// </summary>
        public string QuickAccessToolBarMoreControlsButtonTooltip
        {
            get { return this.quickAccessToolBarMoreControlsButtonTooltip; }
            set
            {
                if (this.quickAccessToolBarMoreControlsButtonTooltip != value)
                {
                    this.quickAccessToolBarMoreControlsButtonTooltip = value;
                    this.RaisePropertyChanged("QuickAccessToolBarMoreControlsButtonTooltip");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Header

        // Quick Access ToolBar  Menu Header
        private string quickAccessToolBarMenuHeader;

        /// <summary>
        /// Quick Access ToolBar  Menu Header
        /// </summary>
        public string QuickAccessToolBarMenuHeader
        {
            get { return this.quickAccessToolBarMenuHeader; }
            set
            {
                if (this.quickAccessToolBarMenuHeader != value)
                {
                    this.quickAccessToolBarMenuHeader = value;
                    this.RaisePropertyChanged("QuickAccessToolBarMenuHeader");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Context Menu Show Below

        // Quick Access ToolBar Minimize Quick Access Toolbar
        private string quickAccessToolBarMenuShowBelow;

        /// <summary>
        /// Quick Access ToolBar Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowBelow
        {
            get { return this.quickAccessToolBarMenuShowBelow; }
            set
            {
                if (this.quickAccessToolBarMenuShowBelow != value)
                {
                    this.quickAccessToolBarMenuShowBelow = value;
                    this.RaisePropertyChanged("QuickAccessToolBarMenuShowBelow");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Show Above

        // Quick Access ToolBar Menu Minimize Quick Access Toolbar
        private string quickAccessToolBarMenuShowAbove;

        /// <summary>
        /// Quick Access ToolBar Menu Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowAbove
        {
            get { return this.quickAccessToolBarMenuShowAbove; }
            set
            {
                if (this.quickAccessToolBarMenuShowAbove != value)
                {
                    this.quickAccessToolBarMenuShowAbove = value;
                    this.RaisePropertyChanged("QuickAccessToolBarMenuShowAbove");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Item

        // Quick Access ToolBar Menu Add Item
        private string ribbonContextMenuAddItem;

        /// <summary>
        /// Quick Access ToolBar Menu Add Item
        /// </summary>
        public string RibbonContextMenuAddItem
        {
            get { return this.ribbonContextMenuAddItem; }
            set
            {
                if (this.ribbonContextMenuAddItem != value)
                {
                    this.ribbonContextMenuAddItem = value;
                    this.RaisePropertyChanged("RibbonContextMenuAddItem");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Group

        // Quick Access ToolBar Menu Add Group
        private string ribbonContextMenuAddGroup;

        /// <summary>
        /// Quick Access ToolBar Menu Add Group
        /// </summary>
        public string RibbonContextMenuAddGroup
        {
            get { return this.ribbonContextMenuAddGroup; }
            set
            {
                if (this.ribbonContextMenuAddGroup != value)
                {
                    this.ribbonContextMenuAddGroup = value;
                    this.RaisePropertyChanged("RibbonContextMenuAddGroup");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Gallery

        // Quick Access ToolBar Menu Add Gallery
        private string ribbonContextMenuAddGallery;

        /// <summary>
        /// Quick Access ToolBar Menu Add Gallery
        /// </summary>
        public string RibbonContextMenuAddGallery
        {
            get { return this.ribbonContextMenuAddGallery; }
            set
            {
                if (this.ribbonContextMenuAddGallery != value)
                {
                    this.ribbonContextMenuAddGallery = value;
                    this.RaisePropertyChanged("RibbonContextMenuAddGallery");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Add Menu

        // Quick Access ToolBar Menu Add Menu
        private string ribbonContextMenuAddMenu;

        /// <summary>
        /// Quick Access ToolBar Menu Add Menu
        /// </summary>
        public string RibbonContextMenuAddMenu
        {
            get { return this.ribbonContextMenuAddMenu; }
            set
            {
                if (this.ribbonContextMenuAddMenu != value)
                {
                    this.ribbonContextMenuAddMenu = value;
                    this.RaisePropertyChanged("RibbonContextMenuAddMenu");
                }
            }
        }

        #endregion

        #region Quick Access ToolBar Menu Remove Item

        // Quick Access ToolBar Menu Remove Item
        private string ribbonContextMenuRemoveItem;

        /// <summary>
        /// Quick Access ToolBar Menu Remove Item
        /// </summary>
        public string RibbonContextMenuRemoveItem
        {
            get { return this.ribbonContextMenuRemoveItem; }
            set
            {
                if (this.ribbonContextMenuRemoveItem != value)
                {
                    this.ribbonContextMenuRemoveItem = value;
                    this.RaisePropertyChanged("RibbonContextMenuRemoveItem");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Customize Quick Access Toolbar

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string ribbonContextMenuCustomizeQuickAccessToolbar;

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeQuickAccessToolBar
        {
            get { return this.ribbonContextMenuCustomizeQuickAccessToolbar; }
            set
            {
                if (this.ribbonContextMenuCustomizeQuickAccessToolbar != value)
                {
                    this.ribbonContextMenuCustomizeQuickAccessToolbar = value;
                    this.RaisePropertyChanged("RibbonContextMenuCustomizeQuickAccessToolBar");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Customize Ribbon

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string ribbonContextMenuCustomizeRibbon;

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeRibbon
        {
            get { return this.ribbonContextMenuCustomizeRibbon; }
            set
            {
                if (this.ribbonContextMenuCustomizeRibbon != value)
                {
                    this.ribbonContextMenuCustomizeRibbon = value;
                    this.RaisePropertyChanged("RibbonContextMenuCustomizeRibbon");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Minimize Ribbon

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuMinimizeRibbon;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuMinimizeRibbon
        {
            get { return this.ribbonContextMenuMinimizeRibbon; }
            set
            {
                if (this.ribbonContextMenuMinimizeRibbon != value)
                {
                    this.ribbonContextMenuMinimizeRibbon = value;
                    this.RaisePropertyChanged("RibbonContextMenuMinimizeRibbon");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Show Below

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuShowBelow;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowBelow
        {
            get { return this.ribbonContextMenuShowBelow; }
            set
            {
                if (this.ribbonContextMenuShowBelow != value)
                {
                    this.ribbonContextMenuShowBelow = value;
                    this.RaisePropertyChanged("RibbonContextMenuShowBelow");
                }
            }
        }

        #endregion

        #region Ribbon Context Menu Show Above

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuShowAbove;

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowAbove
        {
            get { return this.ribbonContextMenuShowAbove; }
            set
            {
                if (this.ribbonContextMenuShowAbove != value)
                {
                    this.ribbonContextMenuShowAbove = value;
                    this.RaisePropertyChanged("RibbonContextMenuShowAbove");
                }
            }
        }

        #endregion

        #region ScreenTipDisableReasonHeader

        // ScreenTip's Disable Reason Header
        private string screenTipDisableReasonHeader;

        /// <summary>
        /// Gets or sets ScreenTip's disable reason header
        /// </summary>
        public string ScreenTipDisableReasonHeader
        {
            get { return this.screenTipDisableReasonHeader; }
            set
            {
                if (this.screenTipDisableReasonHeader != value)
                {
                    this.screenTipDisableReasonHeader = value;
                    this.RaisePropertyChanged("ScreenTipDisableReasonHeader");
                }
            }
        }

        #endregion

        #region ScreenTipF1Label

        // ScreenTip's Disable Reason Header
        private string screenTipF1LabelHeader;

        /// <summary>
        /// Gets or sets ScreenTip's disable reason header
        /// </summary>
        public string ScreenTipF1LabelHeader
        {
            get { return this.screenTipF1LabelHeader; }
            set
            {
                if (this.screenTipF1LabelHeader != value)
                {
                    this.screenTipF1LabelHeader = value;
                    this.RaisePropertyChanged("ScreenTipF1LabelHeader");
                }
            }
        }

        #endregion

        #region Customize Status Bar

        // Text of backstage button
        private string customizeStatusBar;

        /// <summary>
        /// Gets or sets customize Status Bar
        /// </summary>
        public string CustomizeStatusBar
        {
            get { return this.customizeStatusBar; }
            set
            {
                if (this.customizeStatusBar != value)
                {
                    this.customizeStatusBar = value;
                    this.RaisePropertyChanged("CustomizeStatusBar");
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
                    this.LoadEnglish();
                    break;

                case "ru":
                    this.LoadRussian();
                    break;

                case "uk":
                    this.LoadUkrainian();
                    break;

                case "fa":
                    this.LoadPersian();
                    break;

                case "de":
                    this.LoadGerman();
                    break;

                case "hu":
                    this.LoadHungarian();
                    break;

                case "cs":
                    this.LoadCzech();
                    break;

                case "fr":
                    this.LoadFrench();
                    break;

                case "pl":
                    this.LoadPolish();
                    break;

                case "ja":
                    this.LoadJapanese();
                    break;

                case "nl":
                    this.LoadDutch();
                    break;
                case "pt":
                    {
                        if (culture.Name == "pt-BR")
                        {
                            this.LoadPortugueseBrazilian();
                        }
                        else
                        {
                            this.LoadPortuguese();
                        }
                        break;
                    }

                case "es":
                    this.LoadSpanish();
                    break;

                case "zh":
                    this.LoadChinese();
                    break;

                case "sv":
                    this.LoadSwedish();
                    break;

                case "sk":
                    this.LoadSlovak();
                    break;

                case "ro":
                    this.LoadRomanian();
                    break;

                case "it":
                    this.LoadItalian();
                    break;

                case "ar":
                    this.LoadArabic();
                    break;

                case "da":
                    this.LoadDanish();
                    break;

                case "az":
                    this.LoadAzerbaijani();
                    break;

                case "fi":
                    this.LoadFinnish();
                    break;

                case "nb":
                case "nn":
                case "no":
                    this.LoadNorwegian();
                    break;

                case "tr":
                    this.LoadTurkish();
                    break;

                case "he":
                    this.LoadHebrew();
                    break;

                case "ge":
                    this.LoadGreek();
                    break;

                case "ko":
                    this.LoadKorean();
                    break;

                case "vi":
                    this.LoadVietnamese();
                    break;

                case "si":
                    this.LoadSinhala();
                    break;

                case "sl":
                    this.LoadSlovenian();
                    break;

                case "ca":
                    this.LoadCatalan();
                    break;

                case "et":
                    this.LoadEstonian();
                    break;
            }

            // Coerce all values

            this.RaisePropertyChanged("BackstageButtonText");
            this.RaisePropertyChanged("BackstageButtonKeyTip");

            this.RaisePropertyChanged("MinimizeButtonScreenTipTitle");
            this.RaisePropertyChanged("MinimizeButtonScreenTipText");
            this.RaisePropertyChanged("ExpandButtonScreenTipTitle");
            this.RaisePropertyChanged("ExpandButtonScreenTipText");
            this.RaisePropertyChanged("QuickAccessToolBarDropDownButtonTooltip");
            this.RaisePropertyChanged("QuickAccessToolBarMoreControlsButtonTooltip");
            this.RaisePropertyChanged("QuickAccessToolBarMenuHeader");
            this.RaisePropertyChanged("QuickAccessToolBarMenuShowAbove");
            this.RaisePropertyChanged("QuickAccessToolBarMenuShowBelow");

            this.RaisePropertyChanged("RibbonContextMenuAddItem");
            this.RaisePropertyChanged("RibbonContextMenuAddGroup");
            this.RaisePropertyChanged("RibbonContextMenuAddGallery");
            this.RaisePropertyChanged("RibbonContextMenuAddMenu");
            this.RaisePropertyChanged("RibbonContextMenuRemoveItem");
            this.RaisePropertyChanged("RibbonContextMenuCustomizeRibbon");
            this.RaisePropertyChanged("RibbonContextMenuCustomizeQuickAccessToolBar");
            this.RaisePropertyChanged("RibbonContextMenuShowAbove");
            this.RaisePropertyChanged("RibbonContextMenuShowBelow");
            this.RaisePropertyChanged("RibbonContextMenuMinimizeRibbon");

            this.RaisePropertyChanged("ScreenTipDisableReasonHeader");
            this.RaisePropertyChanged("ScreenTipF1LabelHeader");
            this.RaisePropertyChanged("CustomizeStatusBar");
        }

        #endregion

        #region English

        private void LoadEnglish()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "File";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Collapse the Ribbon (Ctrl+F1)";
            this.minimizeButtonScreenTipText = "Need a bit more space? Collapse the ribbon so only the tab names show.";
            this.expandButtonScreenTipTitle = "Pin the Ribbon (Ctrl+F1)";
            this.expandButtonScreenTipText = "Like seeing the ribbon? Keep it open while you work.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Customize Quick Access Toolbar";
            this.quickAccessToolBarMoreControlsButtonTooltip = "More controls"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Customize Quick Access Toolbar";
            this.quickAccessToolBarMenuShowAbove = "Show Above the Ribbon";
            this.quickAccessToolBarMenuShowBelow = "Show Below the Ribbon";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Add to Quick Access Toolbar"; // Button
            this.ribbonContextMenuAddGroup = "Add Group to Quick Access Toolbar"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Add Gallery to Quick Access Toolbar"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Add Menu to Quick Access Toolbar"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Remove from Quick Access Toolbar"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
            this.ribbonContextMenuShowBelow = "Show Quick Access Toolbar Below the Ribbon";
            this.ribbonContextMenuShowAbove = "Show Quick Access Toolbar Above the Ribbon";
            this.ribbonContextMenuCustomizeRibbon = "Customize the Ribbon...";
            this.ribbonContextMenuMinimizeRibbon = "Collapse the Ribbon";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            //Screentips
            this.screenTipDisableReasonHeader = "This command is currently disabled.";
            this.screenTipF1LabelHeader = "Press F1 for help";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Customize Status Bar";
        }

        #endregion

        #region Russian

        private void LoadRussian()
        {
            this.backstageButtonText = "Файл";
            this.backstageButtonKeyTip = "Ф";

            this.minimizeButtonScreenTipTitle = "Свернуть ленту (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только имена вкладок.";
            this.expandButtonScreenTipTitle = "Развернуть ленту (Ctrl + F1)";
            this.expandButtonScreenTipText = "Отображение или скрытие ленты\n\nКогда лента скрыта, отображаются только имена вкладок.";

            this.quickAccessToolBarDropDownButtonTooltip = "Настройка панели быстрого доступа";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Другие элементы";
            this.quickAccessToolBarMenuHeader = "Настройка панели быстрого доступа";
            this.quickAccessToolBarMenuShowAbove = "Разместить над лентой";
            this.quickAccessToolBarMenuShowBelow = "Разместить под лентой";

            this.ribbonContextMenuAddItem = "Добавить на панель быстрого доступа";
            this.ribbonContextMenuAddGroup = "Добавить группу на панель быстрого доступа";
            this.ribbonContextMenuAddGallery = "Добавить коллекцию на панель быстрого доступа";
            this.ribbonContextMenuAddMenu = "Добавить меню на панель быстрого доступа";
            this.ribbonContextMenuRemoveItem = "Удалить с панели быстрого доступа";
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Настройка панели быстрого доступа...";
            this.ribbonContextMenuShowBelow = "Разместить панель быстрого доступа под лентой";
            this.ribbonContextMenuShowAbove = "Разместить панель быстрого доступа над лентой";
            this.ribbonContextMenuCustomizeRibbon = "Настройка ленты...";
            this.ribbonContextMenuMinimizeRibbon = "Свернуть ленту";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            this.screenTipDisableReasonHeader = "В настоящее время эта команда отключена.";

            this.customizeStatusBar = "Настройка строки состояния";
        }

        #endregion

        #region Ukrainian

        private void LoadUkrainian()
        {
            // Backstage button text & key tip 
            this.backstageButtonText = "Файл";
            this.backstageButtonKeyTip = "Ф";
            // See right-top corner... (two different tooltips must be if you press it) 
            this.minimizeButtonScreenTipTitle = "Сховати Стрічку (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Показати або сховати Стрічку\n\nКоли стрічка схована, видно тільки назви вкладок";
            this.expandButtonScreenTipTitle = "Показати Стрічку (Ctrl + F1)";
            this.expandButtonScreenTipText = "Показати або сховати Стрічку\n\nКоли стрічка схована, видно тільки назви вкладок";
            // QAT tooltips and menu items 
            this.quickAccessToolBarDropDownButtonTooltip = "Налаштувати Панель Інструментів Швидкого Доступу";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Більше елементів"; // When two arrows appear ">>" 
            this.quickAccessToolBarMenuHeader = "Налаштувати Панель Інструментів Швидкого Доступу";
            this.quickAccessToolBarMenuShowAbove = "Показати Поверх Стрічки";
            this.quickAccessToolBarMenuShowBelow = "Показати Знизу Стрічки";
            // Click on Ribbon to show context menu 
            this.ribbonContextMenuAddItem = "Додати до Панелі Інструментів Швидкого Доступу"; // Button 
            this.ribbonContextMenuAddGroup = "Додати Групу до Панелі Інструментів Швидкого Доступу"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Додати Галерею до Панелі Інструментів Швидкого Доступу"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Додати Меню до Панелі Інструментів Швидкого Доступу"; // By dashed splitter in context menu 
            this.ribbonContextMenuRemoveItem = "Видалити з Панелі Інструментів Швидкого Доступу"; // By item in QAT 
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Налаштувати Панель Інструментів Швидкого Доступу...";
            this.ribbonContextMenuShowBelow = "Показати Панель Інструментів Швидкого Доступу Знизу Стрічки";
            this.ribbonContextMenuShowAbove = "Показати Панель Інструментів Швидкого Доступу Поверх Стрічки";
            this.ribbonContextMenuCustomizeRibbon = "Налаштувати Стрічку...";
            this.ribbonContextMenuMinimizeRibbon = "Зменшити Стрічку";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot 
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Ця команда на даний момент недоступна.";
        }

        #endregion

        #region Persian

        private void LoadPersian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "فایل";
            this.backstageButtonKeyTip = "ف";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "کوچک کردن نوار (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";
            this.expandButtonScreenTipTitle = "بزرگ کردن نوار (Ctrl + F1)";
            this.expandButtonScreenTipText = "نمایش یا مخفی کردن نوار\n\nهنگامی که نوار مخفی است، تنها\nنام زبانه ها نمایش داده می شود.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "دلخواه سازی میله ابزار دسترسی سریع";
            this.quickAccessToolBarMoreControlsButtonTooltip = "ابزارهای دیگر"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "دلخواه سازی میله ابزار دسترسی سریع";
            this.quickAccessToolBarMenuShowAbove = "نمایش در بالای نوار";
            this.quickAccessToolBarMenuShowBelow = "نمایش در پایین نوار";
            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "اضافه کردن به میله ابزار دسترسی سریع"; // Button
            this.ribbonContextMenuAddGroup = "اضافه کردن گروه به میله ابزار دسترسی سریع"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "اضافه کردن گالری به میله ابزار دسترسی سریع"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "اضاقه کردن منو به میله ابزار دسترسی سریع"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "حذف از میله ابزار دسترسی سریع"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "دلخواه سازی میله ابزار دسترسی سریع...";
            this.ribbonContextMenuShowBelow = "نمایش میله ابزار دسترسی سریع در پایین نوار";
            this.ribbonContextMenuShowAbove = "نمایش میله ابزار دسترسی سریع در بالای نوار";
            this.ribbonContextMenuCustomizeRibbon = "دلخواه سازی نوار...";
            this.ribbonContextMenuMinimizeRibbon = "کوچک کردن نوار";
        }

        #endregion

        #region German

        private void LoadGerman()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Datei";
            this.backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Menüband minimieren (Strg+F1)";
            this.minimizeButtonScreenTipText = "Sie benötigen etwas mehr Platz? Reduzieren Sie das Menüband, sodass nur die Registerkartennamen angezeigt werden.";
            this.expandButtonScreenTipTitle = "Menüband erweitern (Strg+F1)";
            this.expandButtonScreenTipText = "Ist es Ihnen lieber, wenn Sie das Menüband sehen? Lassen Sie es während der Arbeit geöffnet.";

            // QAT tooltips and menu items
            this.quickAccessToolBarDropDownButtonTooltip = "Symbolleiste für den Schnellzugriff anpassen";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Weitere Befehle…"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Symbolleiste für den Schnellzugriff anpassen";
            this.quickAccessToolBarMenuShowAbove = "Über dem Menüband anzeigen";
            this.quickAccessToolBarMenuShowBelow = "Unter dem Menüband anzeigen";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Zur Symbolleiste für den Schnellzugriff hinzufügen"; // Button
            this.ribbonContextMenuAddGroup = "Gruppe zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Katalog zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Zur Symbolleiste für den Schnellzugriff hinzufügen"; // By dashed splitter in context menu

            this.ribbonContextMenuRemoveItem = "Aus Symbolleiste für den Schnellzugriff entfernen"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Symbolleiste für den Schnellzugriff anpassen...";
            this.ribbonContextMenuShowBelow = "Symbolleiste für den Schnellzugriff unter dem Menüband anzeigen";
            this.ribbonContextMenuShowAbove = "Symbolleiste für den Schnellzugriff über dem Menüband anzeigen";
            this.ribbonContextMenuCustomizeRibbon = "Menüband anpassen...";
            this.ribbonContextMenuMinimizeRibbon = "Menüband minimieren";

            //Screentips
            this.screenTipDisableReasonHeader = "Diese Funktion ist momentan deaktiviert.";
            this.screenTipF1LabelHeader = "Drücken Sie F1 für die Hilfe";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Statusleiste anpassen";
        }

        #endregion

        #region Hungarian

        private void LoadHungarian()
        {
            // Backstage button text & key tip 
            this.backstageButtonText = "Fájl";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "A menüszalag összecsukása (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Csak a lapnevek megjelenítése a menüszalagon";
            this.expandButtonScreenTipTitle = "Menüszalag kibontása (Ctrl + F1)";
            this.expandButtonScreenTipText = "A menüszalag megjelenítése úgy, hogy egy parancsra kattintás után is látható maradjon";

            // QAT tooltips and menu items
            this.quickAccessToolBarDropDownButtonTooltip = "Gyorselérési eszköztár testreszabása";
            this.quickAccessToolBarMoreControlsButtonTooltip = "További vezérlők"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Gyorselérési eszköztár testreszabása";
            this.quickAccessToolBarMenuShowAbove = "Megjelenítés a menüszalag alatt";
            this.quickAccessToolBarMenuShowBelow = "Megjelenítés a menüszalag felett";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Felvétel a gyorselérési eszköztárra"; // Button
            this.ribbonContextMenuAddGroup = "Felvétel a gyorselérési eszköztárra"; // For ex., by collapsed group 
            this.ribbonContextMenuAddGallery = "Gyűjtemény felvétele a gyorselérési eszköztárra"; // For ex., by opened font context menu 
            this.ribbonContextMenuAddMenu = "Felvétel a gyorselérési eszköztárra"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Eltávolítás a gyorselérési eszköztárról"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Gyorselérési eszköztár testreszabása...";
            this.ribbonContextMenuShowBelow = "A gyorselérési eszköztár megjelenítése a menüszalag alatt";
            this.ribbonContextMenuShowAbove = "A gyorselérési eszköztár megjelenítése a menüszalag felett";
            this.ribbonContextMenuCustomizeRibbon = "Menüszalag testreszabása...";
            this.ribbonContextMenuMinimizeRibbon = " A menüszalag összecsukása";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3)
            this.screenTipDisableReasonHeader = "Ez a parancs jelenleg nem használható.";
        }

        #endregion

        #region Czech

        private void LoadCzech()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Soubor";
            this.backstageButtonKeyTip = "S";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Skrýt pás karet (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Zobrazit nebo skrýt pás karet\n\nJe-li pás karet skrytý, jsou zobrazeny pouze názvy karet";
            this.expandButtonScreenTipTitle = "Zobrazit pás karet (Ctrl + F1)";
            this.expandButtonScreenTipText = "Zobrazit nebo skrýt pás karet\n\nJe-li pás karet skrytý, jsou zobrazeny pouze názvy karet";

            // QAT tooltips and menu items 
            this.quickAccessToolBarDropDownButtonTooltip = "Přizpůsobit panel nástrojů Rychlý přístup";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Další příkazy"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Přizpůsobit panel nástrojů Rychlý přístup";
            this.quickAccessToolBarMenuShowAbove = "Zobrazit nad pásem karet";
            this.quickAccessToolBarMenuShowBelow = "Zobrazit pod pásem karet";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Přidat na panel nástrojů Rychlý přístup"; // Button
            this.ribbonContextMenuAddGroup = "Přidat na panel nástrojů Rychlý přístup"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Přidat galerii na panel nástrojů Rychlý přístup"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Přidat na panel nástrojů Rychlý přístup"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Odebrat z panelu nástrojů Rychlý přístup"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Přizpůsobit panel nástrojů Rychlý přístup...";
            this.ribbonContextMenuShowBelow = "Zobrazit panel nástrojů Rychlý přístup pod pásem karet";
            this.ribbonContextMenuShowAbove = "Zobrazit panel nástrojů Rychlý přístup nad pásem karet";
            this.ribbonContextMenuCustomizeRibbon = "Přizpůsobit pás karet...";
            this.ribbonContextMenuMinimizeRibbon = "Skrýt pás karet";

            //Screentips
            this.screenTipDisableReasonHeader = "Tento příkaz je aktuálně zakázán.";
            this.screenTipF1LabelHeader = "Stiskni F1 pro nápovědu";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Přizpůsobit Status Bar";
        }

        #endregion

        #region French

        private void LoadFrench()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Fichier";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimiser le Ruban (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul les noms sont affichés";
            this.expandButtonScreenTipTitle = "Agrandir le Ruban (Ctrl + F1)";
            this.expandButtonScreenTipText = "Afficher ou masquer le Ruban \n\nQuand le Ruban est masqué, seul les noms sont affichés";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Personnaliser la barre d'outils Accès Rapide";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Plus de contrôles"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Personnaliser la barre d'outil Accès Rapide";
            this.quickAccessToolBarMenuShowAbove = "Afficher au dessus du Ruban";
            this.quickAccessToolBarMenuShowBelow = "Afficher en dessous du Ruban";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Ajouter un élément à la barre d'outils Accès Rapide"; // Button
            this.ribbonContextMenuAddGroup = "Ajouter un groupe à la barre d'outils Accès Rapide"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Ajouter une galerie à la barre d'outils Accès Rapide"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Ajouter un menu à la barre d'outils Accès Rapide"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Supprimer de la barre d'outils Accès Rapide"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Personnaliser la barre d'outils Accès Rapide...";
            this.ribbonContextMenuShowBelow = "Afficher la barre d'outils Accès Rapide en dessous du Ruban";
            this.ribbonContextMenuShowAbove = "Afficher la barre d'outils Accès Rapide au dessus du Ruban";
            this.ribbonContextMenuCustomizeRibbon = "Personnaliser le Ruban...";
            this.ribbonContextMenuMinimizeRibbon = "Minimiser le Ruban";
            this.customizeStatusBar = "Personnaliser la barre de statut";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            this.screenTipDisableReasonHeader = "Cette commande est actuellement désactivée.";
        }

        #endregion

        #region Polish

        private void LoadPolish()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Plik";
            this.backstageButtonKeyTip = "P";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimalizuj Wstążkę (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko nazwy zakładek są widoczne";
            this.expandButtonScreenTipTitle = "Rozwiń Wstążkę (Ctrl + F1)";
            this.expandButtonScreenTipText = "Pokazuje lub ukrywa Wstążkę\n\nGdy Wstążka jest ukryta, tylko nazwy zakładek są widoczne";

            // QAT tooltips and menu items
            this.quickAccessToolBarDropDownButtonTooltip = "Dostosuj pasek narzędzi Szybki dostęp";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Więcej poleceń..."; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Dostosuj pasek narzędzi Szybki dostęp";
            this.quickAccessToolBarMenuShowAbove = "Pokaż powyżej Wstążki";
            this.quickAccessToolBarMenuShowBelow = "Pokaż poniżej Wstążki";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Dodaj do paska narzędzi Szybki dostęp"; // Button
            this.ribbonContextMenuAddGroup = "Dodaj Grupę do paska narzędzi Szybki dostęp"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Dodaj Galerię do paska narzędzi Szybki dostęp"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Dodaj do paska narzędzi Szybki dostęp"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Usuń z paska narzędzi Szybki dostęp"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Dostosuj pasek narzędzi Szybki dostęp...";
            this.ribbonContextMenuShowBelow = "Pokaż pasek Szybki dostęp poniżej Wstążki";
            this.ribbonContextMenuShowAbove = "Pokaż pasek Szybki dostęp powyżej Wstążki";
            this.ribbonContextMenuCustomizeRibbon = "Dostosuj Wstążkę...";
            this.ribbonContextMenuMinimizeRibbon = "Minimalizuj Wstążkę";
        }

        #endregion

        #region Japanese

        private void LoadJapanese()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "ファイル";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "リボンの最小化 (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";
            this.expandButtonScreenTipTitle = "リボンの展開 (Ctrl + F1)";
            this.expandButtonScreenTipText = "リボンの表示/非表示を切り替えます。\n\nリボンを非表示にすると、タブ名のみが表示されます。";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "クイック アクセス ツール バーのユーザー設定";
            this.quickAccessToolBarMoreControlsButtonTooltip = "その他のボタン"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "クイック アクセス ツール バーのユーザー設定";
            this.quickAccessToolBarMenuShowAbove = "リボンの上に表示";
            this.quickAccessToolBarMenuShowBelow = "リボンの下に表示";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "クイック アクセス ツール バーに追加"; // Button
            this.ribbonContextMenuAddGroup = "グループをクイック アクセス ツール バーに追加"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "ギャラリーをクイック アクセス ツール バーに追加"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "メニューをクイック アクセス ツール バーに追加"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "クイック アクセス ツール バーから削除"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "クイック アクセス ツール バーのユーザー設定...";
            this.ribbonContextMenuShowBelow = "クイック アクセス ツール バーをリボンの下に表示";
            this.ribbonContextMenuShowAbove = "クイック アクセス ツール バーをリボンの上に表示";
            this.ribbonContextMenuCustomizeRibbon = "リボンのユーザー設定...";
            this.ribbonContextMenuMinimizeRibbon = "リボンの最小化";
            this.customizeStatusBar = "ステータス バーのユーザー設定";

            this.screenTipDisableReasonHeader = "このコマンドは現在無効になっています";
        }

        #endregion

        #region Dutch

        private void LoadDutch()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Bestand";
            this.backstageButtonKeyTip = "B";
            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Het lint minimaliseren (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, zijn alleen de tabulatie namen zichtbaar";
            this.expandButtonScreenTipTitle = "Het lint Maximaliseren (Ctrl + F1)";
            this.expandButtonScreenTipText = "Verberg of laat het lint zien\n\nWanneer het lint verborgen is, zijn alleen de tabulatie namen zichtbaar";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Werkbalk snelle toegang aanpassen";
            this.quickAccessToolBarMoreControlsButtonTooltip = "meer opdrachten"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = " Werkbalk snelle toegang aanpassen ";
            this.quickAccessToolBarMenuShowAbove = "Boven het lint weergeven";
            this.quickAccessToolBarMenuShowBelow = "beneden het lint weergeven";
            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Menu toevoegen aan werkbalk snelle toegang"; // Button
            this.ribbonContextMenuAddGroup = "Groep toevoegen aan werkbalk snelle toegang"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Galerij toevoegen aan werkbalk snelle toegang"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = " Menu toevoegen aan werkbalk snelle toegang "; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = " Verwijder uit werkbalk snelle toegang "; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";
            this.ribbonContextMenuShowBelow = " Werkbalk snelle toegang onder het lint weergeven";
            this.ribbonContextMenuShowAbove = " Werkbalk snelle toegang boven het lint weergeven ";
            this.ribbonContextMenuCustomizeRibbon = "Lint aanpassen...";
            this.ribbonContextMenuMinimizeRibbon = " Het lint minimaliseren";
        }

        #endregion

        #region Brazilian

        private void LoadPortugueseBrazilian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Arquivo";
            this.backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimizar o Ribbon (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Mostrar ou esconder o  Ribbon\n\nQuando o Ribbon estiver escondido, somente o nome das abas serão mostrados";
            this.expandButtonScreenTipTitle = "Expandir o Ribbon (Ctrl + F1)";
            this.expandButtonScreenTipText = "Mostrar ou esconder o  Ribbon\n\nQuando o Ribbon estiver escondido, somente o nome das abas serão mostrados";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Customizar Barra de acesso rápido";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Mais controles"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = " Customizar Barra de acesso rápido";
            this.quickAccessToolBarMenuShowAbove = "Mostrar acima do Ribbon";
            this.quickAccessToolBarMenuShowBelow = "Mostrar abaixo do Ribbon";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Adicionar para Barra de acesso rápido"; // Button
            this.ribbonContextMenuAddGroup = " Adicionar o grupo para Barra de acesso rápido"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Adicionar a galeria para Barra de acesso rápido";
            // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = " Adicionar o menu para Barra de acesso rápido";
            // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Remover da Barra de acesso rápido"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Customizar Barra de acesso rápido...";
            this.ribbonContextMenuShowBelow = "Mostrar Barra de acesso rápido abaixo do Ribbon";
            this.ribbonContextMenuShowAbove = "Mostrar Barra de acesso rápido acima do Ribbon";
            this.ribbonContextMenuCustomizeRibbon = "Customizar o Ribbon...";
            this.ribbonContextMenuMinimizeRibbon = "Minimizar o Ribbon";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Este comando está desativado.";
        }

        #endregion

        #region Spanish

        private void LoadSpanish()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Archivo";
            this.backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            // TRANSLATOR'S NOTE: This block is not shown at Windows 7's Apps (WordPad or Paint)
            this.minimizeButtonScreenTipTitle = "Minimizar la cinta (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Muestra u oculta la cinta\n\nCuando la cinta está oculta, sólo se muestran los nombres de las pestañas";
            this.expandButtonScreenTipTitle = "Expandir la cinta (Ctrl + F1)";
            this.expandButtonScreenTipText = "Muestra u oculta la cinta\n\nCuando la cinta está oculta, sólo se muestran los nombres de las pestañas";

            // QAT tooltips and menu items
            this.quickAccessToolBarDropDownButtonTooltip = "Personalizar barra de herramientas de acceso rápido";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Más controles"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Personalizar barra de herramientas de acceso rápido";
            this.quickAccessToolBarMenuShowAbove = "Mostrar sobre la cinta";
            this.quickAccessToolBarMenuShowBelow = "Mostrar bajo la cinta";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Agregar a la barra de herramientas de acceso rápido"; // Button
            this.ribbonContextMenuAddGroup = "Agregar grupo a la barra de herramientas de acceso rápido"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Agregar galería a la barra de herramientas de acceso rápido"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Agregar menú a la barra de herramientas de acceso rápido"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Quitar de la barra de herramientas de acceso rápido"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar la barra de herramientas de acceso rápido...";
            this.ribbonContextMenuShowBelow = "Mostrar barra de herramientas de acceso rápido bajo la cinta";
            this.ribbonContextMenuShowAbove = "Mostrar barra de herramientas de acceso rápido sobre la cinta";
            this.ribbonContextMenuCustomizeRibbon = "Personalizar la cinta...";
            this.ribbonContextMenuMinimizeRibbon = "Minimizar la cinta";

            //Screentips
            this.screenTipDisableReasonHeader = "Este comando está desactivado actualmente";
            this.screenTipF1LabelHeader = "Pulse F1 para obtener más ayuda";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Personalizar barra de estado";
        }

        #endregion

        #region Chinese

        private void LoadChinese()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "文件";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "功能区最小化 (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "仅显示功能区上的选项卡名称。单击选项卡可显示命令。";
            this.expandButtonScreenTipTitle = "展开功能区 (Ctrl + F1)";
            this.expandButtonScreenTipText = "始终显示功能区选项卡和命令。";

            // QAT tooltips and menu items         
            this.quickAccessToolBarDropDownButtonTooltip = "自定义快速访问具栏";
            this.quickAccessToolBarMoreControlsButtonTooltip = "其他命令"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "自定义快速访问工具栏";
            this.quickAccessToolBarMenuShowAbove = "在功能区上方显示";
            this.quickAccessToolBarMenuShowBelow = "在功能区下方显示";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "添加到快速访问工具栏"; // Button
            this.ribbonContextMenuAddGroup = "在快速访问工具栏中添加组"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "在快速访问工具栏中添加样式"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "在快速访问工具栏中添加菜单"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "在快速访问工具栏中移除"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "自定义快速访问工具栏...";
            this.ribbonContextMenuShowBelow = "在功能区下方显示快速访问工具栏";
            this.ribbonContextMenuShowAbove = "在功能区上方显示快速访问工具栏";
            this.ribbonContextMenuCustomizeRibbon = "自定义功能区...";
            this.ribbonContextMenuMinimizeRibbon = "功能区最小化";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "此命令当前已被禁用。";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "自定义状态栏";
        }

        #endregion

        #region Swedish

        private void LoadSwedish()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Arkiv";
            this.backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimera menyfliksområdet (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas endast flikarna";
            this.expandButtonScreenTipTitle = "Expandera menyfliksområdet (Ctrl + F1)";
            this.expandButtonScreenTipText = "Visa eller göm menyfliksområdet \n\nNär menyfliksområdet är dolt, visas endast flikarna";

            // QAT tooltips and menu items          
            this.quickAccessToolBarDropDownButtonTooltip = "Anpassa verktygsfältet Snabbåtkomst ";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Fler kommandon"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = " Anpassa verktygsfältet Snabbåtkomst";
            this.quickAccessToolBarMenuShowAbove = "Visa ovanför menyfliksområdet";
            this.quickAccessToolBarMenuShowBelow = "Visa under menyfliksområdet";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Lägg till i verktygsfältet Snabbåtkomst"; // Button
            this.ribbonContextMenuAddGroup = "Lägg till i verktygsfältet Snabbåtkomst"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Lägg till galleriet i verktygsfältet Snabbåtkomst"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = " Lägg till menyn i verktygsfältet Snabbåtkomst "; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Ta bort från verktygsfältet Snabbåtkomst"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Anpassa verktygsfältet Snabbåtkomst...";
            this.ribbonContextMenuShowBelow = " Visa verktygsfältet Snabbåtkomst under menyfliksområdet";
            this.ribbonContextMenuShowAbove = " Visa verktygsfältet Snabbåtkomst ovanför menyfliksområdet ";
            this.ribbonContextMenuCustomizeRibbon = "Anpassa menyfliksområdet...";
            this.ribbonContextMenuMinimizeRibbon = "Minimera menyfliksområdet";
        }

        #endregion

        #region Slovak

        private void LoadSlovak()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Súbor";
            this.backstageButtonKeyTip = "S";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Skryť pás s nástrojmi (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet";
            this.expandButtonScreenTipTitle = "Zobraziť pás s nástrojmi (Ctrl + F1)";
            this.expandButtonScreenTipText = " Zobraziť alebo skryť pás s nástrojmi\n\nKeď je pás s nástrojmi skrytý, sú zobrazené iba názvy kariet ";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Prispôsobenie panela s nástrojmi Rýchly prístup";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Ďalšie príkazy"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Prispôsobenie panela s nástrojmi Rýchly prístup";
            this.quickAccessToolBarMenuShowAbove = " Zobraziť nad pásom s nástrojmi ";
            this.quickAccessToolBarMenuShowBelow = "Zobraziť pod pásom s nástrojmi";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Pridať na panel s nástrojmi Rýchly prístup"; // Button
            this.ribbonContextMenuAddGroup = " Pridať na panel s nástrojmi Rýchly prístup "; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = " Pridať galériu do panela s nástrojmi Rýchly prístup "; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Pridať na panel s nástrojmi Rýchly prístup"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Odstrániť z panela s nástrojmi Rýchly prístup "; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = " Prispôsobenie panela s nástrojmi Rýchly prístup...";
            this.ribbonContextMenuShowBelow = "Panel s nástrojmi Rýchly prístup zobraziť pod panelom s nástrojmi";
            this.ribbonContextMenuShowAbove = "Panel s nástrojmi Rýchly prístup zobraziť nad panelom s nástrojmi ";
            this.ribbonContextMenuCustomizeRibbon = "Prispôsobenie panela s nástrojmi Rýchly prístup...";
            this.ribbonContextMenuMinimizeRibbon = "Minimalizovať pás s nástrojmi";
        }

        #endregion

        #region Romanian

        private void LoadRomanian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Fișier";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimizează Ribbon-ul (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt afișate doar numele taburilor";
            this.expandButtonScreenTipTitle = "Expandează Ribbon-ul (Ctrl + F1)";
            this.expandButtonScreenTipText = "Afișează sau ascunde Ribbon-ul\nCând Ribbon-ul este ascuns, sunt afișate doar numele taburilor";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Personalizează Bara de Acces Rapid";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Mai multe controale"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Personalizează Bara de Acces Rapid";
            this.quickAccessToolBarMenuShowAbove = "Afișează peste Ribbon";
            this.quickAccessToolBarMenuShowBelow = "Afișează sub Ribbon";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Adaugă la Bara de Acess Rapid"; // Button
            this.ribbonContextMenuAddGroup = "Adaugă Grupul la Bara de Acess Rapid"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Adaugă Galeria la Bara de Acess Rapid";
            // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Adaugă Meniul la Bara de Acess Rapid"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Eimină din Bara de Acess Rapid"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizează Bara de Acces Rapid...";
            this.ribbonContextMenuShowBelow = "Afișează Bara de Acces Rapid sub Ribbon";
            this.ribbonContextMenuShowAbove = "Afișează Bara de Acces Rapid peste Ribbon";
            this.ribbonContextMenuCustomizeRibbon = "Personalizează Ribbon-ul...";
            this.ribbonContextMenuMinimizeRibbon = "Minimizează Ribbon-ul...";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Această comandă nu este disponibilă momentan.";
        }

        #endregion

        #region Italian

        private void LoadItalian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "File";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Riduci a icona barra multifunzione (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Consente di visualizzare solo i nomi delle schede nella barra multifunzione.";
            this.expandButtonScreenTipTitle = "Espandi la barra multifunzione (Ctrl + F1)";
            this.expandButtonScreenTipText = "Visualizza la barra multifunzione in modo che rimanga sempre espansa, anche se l’utente ha fatto click su un comando.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Personalizza barra di accesso rapido";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Altri comandi…"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Personalizza barra di accesso rapido";
            this.quickAccessToolBarMenuShowAbove = "Mostra sopra la barra multifunzione";
            this.quickAccessToolBarMenuShowBelow = "Mostra sotto la barra multifunzione";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Aggiungi alla barra di accesso rapido"; // Button
            this.ribbonContextMenuAddGroup = "Aggiungi gruppo alla barra di accesso rapido"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Aggiungi raccolta alla barra di accesso rapido"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Aggiungi menu alla barra di accesso rapido"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Rimuovi dalla barra di accesso rapido"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizza barra di accesso rapido...";
            this.ribbonContextMenuShowBelow = "Mostra la barra di accesso rapido sotto la barra multifunzione";
            this.ribbonContextMenuShowAbove = "Mostra la barra di accesso rapido sopra la barra multifunzione";
            this.ribbonContextMenuCustomizeRibbon = "Personalizza barra multifunzione...";
            this.ribbonContextMenuMinimizeRibbon = "Riduci a icona barra multifunzione";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Questo commando è disattivato.";
        }

        #endregion

        #region Arabic

        private void LoadArabic()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "ملف    ";
            this.backstageButtonKeyTip = "م    ";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "(Ctrl + F1)تصغير الشريط ";
            this.minimizeButtonScreenTipText = "إظهار أسماء علامات التبويب فقط على الشريط.";
            this.expandButtonScreenTipTitle = "(Ctrl + F1)توسيع الشريط ";
            this.expandButtonScreenTipText = "إظهار الشريط بحيث يكون موسعاً دائماً حتى بعد النقر فوق أمر.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "تخصيص شريط أدوات الوصول السريع";
            this.quickAccessToolBarMoreControlsButtonTooltip = "أوامر إضافية"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "تخصيص شريط أدوات الوصول السريع";
            this.quickAccessToolBarMenuShowAbove = "إظهار أعلى الشريط";
            this.quickAccessToolBarMenuShowBelow = "إظهار أسفل الشريط";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "إضافة إلى شريط أدوات الوصول السريع"; // Button
            this.ribbonContextMenuAddGroup = "إضافة إلى شريط أدوات الوصول السريع"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "إضافة إلى شريط أدوات الوصول السريع"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "إضافة إلى شريط أدوات الوصول السريع"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "إزالة إلى شريط أدوات الوصول السريع"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "تخصيص شريط أدوات الوصول السريع...";
            this.ribbonContextMenuShowBelow = "إظهار شريط أدوات الوصول السريع أسفل الشريط";
            this.ribbonContextMenuShowAbove = "إظهار شريط أدوات الوصول السريع أعلى الشريط";
            this.ribbonContextMenuCustomizeRibbon = "تخصيص الشريط...";
            this.ribbonContextMenuMinimizeRibbon = "تصغير الشريط";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "تم حالياً تعطيل هذا الأمر.";
        }

        #endregion

        #region Danish

        private void LoadDanish()
        {
            // Backstage button text & key 
            this.backstageButtonText = "Filer";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimer båndet (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Vis kun fanenavnene på båndet.";
            this.expandButtonScreenTipTitle = "Udvid båndet (Ctrl + F1)";
            this.expandButtonScreenTipText = "Vis båndet, så det altid er udvidet, selv når du klikker på en kommando.";

            // QAT tooltips and menu items          
            this.quickAccessToolBarDropDownButtonTooltip = "Tilpas værktøjslinjen Hurtig adgang";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Flere kontrolelementer"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = " Tilpas værktøjslinjen Hurtig adgang";
            this.quickAccessToolBarMenuShowAbove = "Vis ovenover båndet";
            this.quickAccessToolBarMenuShowBelow = "Vis under båndet";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Føj til værktøjslinjen Hurtig adgang"; // Button
            this.ribbonContextMenuAddGroup = "Føj til værktøjslinjen Hurtig adgang"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Tilføj Galleri til værktøjslinjen Hurtig adgang"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Føj til værktøjslinjen Hurtig adgang"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Fjern fra værktøjslinjen Hurtig adgang"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Tilpas værktøjslinjen Hurtig adgang...";
            this.ribbonContextMenuShowBelow = "Vis værktøjslinjen Hurtig adgang under båndet";
            this.ribbonContextMenuShowAbove = "Vis værktøjslinjen Hurtig adgang ovenover båndet";
            this.ribbonContextMenuCustomizeRibbon = "Tilpas båndet...";
            this.ribbonContextMenuMinimizeRibbon = "Minimer båndet";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Denne kommando er aktuelt deaktiveret.";
        }

        #endregion

        #region Portuguese

        private void LoadPortuguese()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Ficheiro";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimizar o Friso (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Mostrar apenas os nomes dos separadores no Frisos.";
            this.expandButtonScreenTipTitle = "Expandir o Friso (Ctrl + F1)";
            this.expandButtonScreenTipText = "Mostrar o Friso de modo a aparecer sempre expandido mesmo depois de clicar num comando.";

            // QAT tooltips and menu items           
            this.quickAccessToolBarDropDownButtonTooltip = "Personalizar Barra de Ferramentas de Acesso Rápido";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Mais Comandos..."; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Personalizar Barra de Ferramentas de Acesso Rápido";
            this.quickAccessToolBarMenuShowAbove = "Mostrar Acima do Friso";
            this.quickAccessToolBarMenuShowBelow = "Mostrar Abaixo do Friso";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Adicionar à Barra de Ferramentas de Acesso Rápido";
            this.ribbonContextMenuAddGroup = "Adicionar Grupo à Barra de Ferramentas de Acesso Rápido";
            this.ribbonContextMenuAddGallery = "Adicionar Galeria à Barra de Ferramentas de Acesso Rápido";
            this.ribbonContextMenuAddMenu = "Adicionar Menu à Barra de Ferramentas de Acesso Rápido";
            this.ribbonContextMenuRemoveItem = "Remover da Barra de Ferramentas de Acesso Rápido";
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar Barra de Ferramentas de Acesso Rápido...";
            this.ribbonContextMenuShowBelow = "Mostrar Barra de Ferramentas de Acesso Rápido Abaixo do Friso";
            this.ribbonContextMenuShowAbove = "Mostrar Barra de Ferramentas de Acesso Rápido Acima do Friso";
            this.ribbonContextMenuCustomizeRibbon = "Personalizar o Friso...";
            this.ribbonContextMenuMinimizeRibbon = "Minimizar o Friso";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3)
            this.screenTipDisableReasonHeader = "Este comando está desactivado actualmente.";
        }

        #endregion

        #region Azerbaijani

        private void LoadAzerbaijani()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Fayl";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Menyu lentini kiçilt (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Menyu lentini göstər və ya gizlət\n\nMenyu lentini kiçiləndə, yalnız tabların adları göstərilir";
            this.expandButtonScreenTipTitle = "Menyu lentini böyüt(Ctrl + F1)";
            this.expandButtonScreenTipText = " Menyu lentini göstər və ya gizlət\n\nMenyu lentini gizldəndə, yalnız, tabların adları göstərilir";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Sürətli Keçidin Alətlərini fərdiləşdir";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Digər nəzarət vasitələri"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = " Sürətli Keçidin Alətlərini fərdiləşdir ";
            this.quickAccessToolBarMenuShowAbove = "Menyu lentinin üstündə göstər";
            this.quickAccessToolBarMenuShowBelow = " Menyu lentinin altında göstər ";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Sürətli Keçidin Alətlərinə əlavə et"; // Button
            this.ribbonContextMenuAddGroup = " Sürətli Keçidin Alətlərinə Qrup əlavə et "; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = " Sürətli Keçidin Alətlərinə Qalereya əlavə et"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = " Sürətli Keçidin Alətlərinə Menyu əlavə et"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = " Sürətli Keçidin Alətlərindən sil"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = " Sürətli Keçidin Alətlərini fərdiləşdir...";
            this.ribbonContextMenuShowBelow = " Sürətli Keçidin Alətlərini Menyu lentinin altında göstər ";
            this.ribbonContextMenuShowAbove = " Sürətli Keçidin Alətlərini Menyu lentinin üstündə göstər ";
            this.ribbonContextMenuCustomizeRibbon = "Menyu lentini fərdiləşdir...";
            this.ribbonContextMenuMinimizeRibbon = " Menyu lentini kiçilt";
        }

        #endregion

        #region Finnish

        private void LoadFinnish()
        {
            this.backstageButtonText = "Tiedosto";
            this.backstageButtonKeyTip = "T";
            this.minimizeButtonScreenTipTitle = "Pienennä valintanauha (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Näytä valintanauhassa vain välilehtien nimet";
            this.expandButtonScreenTipTitle = "Laajenna valintanauha (Ctrl + F1)";
            this.expandButtonScreenTipText = "Näytä valintanauha aina laajennettuna silloinkin, kun valitset komennon";
            this.quickAccessToolBarDropDownButtonTooltip = "Mukauta pikatyökaluriviä";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Lisää valintoja";
            this.quickAccessToolBarMenuHeader = "Mukauta pikatyökaluriviä";
            this.quickAccessToolBarMenuShowAbove = "Näytä valintanauhan yläpuolella";
            this.quickAccessToolBarMenuShowBelow = "Näytä valintanauhan alapuolella";
            this.ribbonContextMenuAddItem = "Lisää pikatyökaluriville";
            this.ribbonContextMenuAddGroup = "Lisää ryhmä pikatyökaluriviin";
            this.ribbonContextMenuAddGallery = "Lisää valikoima pikatyökaluriviin";
            this.ribbonContextMenuAddMenu = "Lisää valikko pikatyökaluriviin";
            this.ribbonContextMenuRemoveItem = "Poista pikatyökaluriviltä";
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Mukauta pikatyökaluriviä...";
            this.ribbonContextMenuShowBelow = "Näytä pikatyökalurivi valintanauhan alapuolella";
            this.ribbonContextMenuShowAbove = "Näytä pikatyökalurivi valintanauhan yläpuolella";
            this.ribbonContextMenuCustomizeRibbon = "Mukauta valintanauhaa...";
            this.ribbonContextMenuMinimizeRibbon = "Pienennä valintanauha";
            this.screenTipDisableReasonHeader = "Tämä komento on tällä hetkellä poissa käytöstä";
        }

        #endregion

        #region Norwegian

        private void LoadNorwegian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Fil";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimer båndet (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Viser bare kategorinavnene på båndet";
            this.expandButtonScreenTipTitle = "Utvider båndet (Ctrl + F1)";
            this.expandButtonScreenTipText = "Vis båndet slik at det alltid er utvidet selv etter at du har valgt en kommando";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Tilpass verktøylinje for hurtigtilgang";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Flere kontroller"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Tilpass verktøylinje for hurtigtilgang";
            this.quickAccessToolBarMenuShowAbove = "Vis over båndet";
            this.quickAccessToolBarMenuShowBelow = "Vis under båndet";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Legg til på verktøylinje for hurtigtilgang"; // Button
            this.ribbonContextMenuAddGroup = "Legg til gruppe på verktøylinje for hurtigtilgang"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Legg til galleri på verktøylinje for hurtigtilgang"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Legg til meny på verktøylinje for hurtigtilgang"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Fjern verktøylinjen for hurtigtilgang"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Tilpass verktøylinje for hurtigtilgang...";
            this.ribbonContextMenuShowBelow = "Vis verktøylinjen for hurtigtilgang under båndet";
            this.ribbonContextMenuShowAbove = "Vis verktøylinjen for hurtigtilgang over båndet";
            this.ribbonContextMenuCustomizeRibbon = "Tilpass båndet...";
            this.ribbonContextMenuMinimizeRibbon = "Minimer båndet";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Denne kommandoen er for øyeblikket deaktivert.";
        }

        #endregion

        #region Turkish

        private void LoadTurkish()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Dosya";
            this.backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Şeridi Daralt (Ctrl+F1)";
            this.minimizeButtonScreenTipText = "Daha fazla alana mı\nihtiyacınız var? Şeridi daraltın, yalnızca sekme isimleri görünsün.";
            this.expandButtonScreenTipTitle = "Şeridi Sabitle (Ctrl+F1)";
            this.expandButtonScreenTipText = "Şeridi görmek mi istiyorsunuz? Çalışırken açık tutun.";

            // QAT tooltips and menu items           
            this.quickAccessToolBarDropDownButtonTooltip = "Hızlı Erişim Araç Çubuğu'nu Özelleştir";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Diğer denetimler"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Hızlı Erişim Araç Çubuğu'nu Özelleştir";
            this.quickAccessToolBarMenuShowAbove = "Şeridin Üstünde Göster";
            this.quickAccessToolBarMenuShowBelow = "Şeridin Altında Göster";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Hızlı Erişim Araç Çubuğu'na Ekle"; // Button
            this.ribbonContextMenuAddGroup = "Grubu Hızlı Erişim Araç Çubuğu'na Ekle"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Galeriyi Hızlı Erişim Araç Çubuğu'na Ekle"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Menüyü Hızlı Erişim Araç Çubuğu'na Ekle"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Hızlı Erişim Araç Çubuğu'ndan Kaldır"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Hızlı Erişim Araç Çubuğu'nu Özelleştir";
            this.ribbonContextMenuShowBelow = "Hızlı Erişim Araç Çubuğu'nu Şeridin Altında Göster";
            this.ribbonContextMenuShowAbove = "Hızlı Erişim Araç Çubuğu'nu Şeridin Üstünde Göster";
            this.ribbonContextMenuCustomizeRibbon = "Şeridi Özelleştir...";
            this.ribbonContextMenuMinimizeRibbon = "Şeridi Daralt";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Bu komut şu anda devre dışı";
            this.screenTipF1LabelHeader = "Yardım için F1'e basın.";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Durum Çubuğunu Özelleştir";
        }

        #endregion

        #region Hebrew

        private void LoadHebrew()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "קובץ";
            this.backstageButtonKeyTip = "ק";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "מזער את רצועת הכלים (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "הצג רק את שמות הכרטיסיות\nברצועת הכלים.";
            this.expandButtonScreenTipTitle = "הרחב את רצועת הכלים (Ctrl + F1)";
            this.expandButtonScreenTipText = "הצג את רצועת הכלים כשהיא\nמורחבת תמיד, גם לאחר\nשתלחץ על הפקודה.";

            // QAT tooltips and menu items           
            this.quickAccessToolBarDropDownButtonTooltip = "התאמה אישית של סרגל הכלים לגישה מהירה";
            this.quickAccessToolBarMoreControlsButtonTooltip = "פקודות נוספות"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "התאמה אישית של סרגל הכלים לגישה מהירה";
            this.quickAccessToolBarMenuShowAbove = "הצג מעל לרצועת הכלים";
            this.quickAccessToolBarMenuShowBelow = "הצג מעל לרצועת הכלים";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "הוסף לסרגל הכלים לגישה מהירה"; // Button
            this.ribbonContextMenuAddGroup = "הוסף קבוצה לסרגל הכלים לגישה מהירה"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "הוסף גלריה לסרגל הכלים לגישה מהירה"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "הוסף תפריט לסרגל הכלים לגישה מהירה"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "הסר מסרגל הכלים לגישה מהירה"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "של סרגל הכלים רצועת הכלים...";
            this.ribbonContextMenuShowBelow = "הצג את סרגל הכלים לגישה מהירה מתחת לרצועת הכלים";
            this.ribbonContextMenuShowAbove = "הצג את סרגל הכלים לגישה מהירה מעל לרצועת הכלים";
            this.ribbonContextMenuCustomizeRibbon = "התאמה אישית של רצועת הכלים...";
            this.ribbonContextMenuMinimizeRibbon = "מזער את רצועת הכלים";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "פקודה זו אינה זמינה כעת.";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "התאמה אישית של שורת המצב";
        }

        #endregion

        #region Greek

        private void LoadGreek()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Αρχείο";
            this.backstageButtonKeyTip = "Α";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Ελαχιστοποίηση της Κορδέλας (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Εμφάνιση μόνο των ονομάτων καρτελών στην Κορδέλα.";
            this.expandButtonScreenTipTitle = "Ανάπτυξη της Κορδέλας (Ctrl + F1)";
            this.expandButtonScreenTipText = "Εμφάνιση της Κορδέλας προκειμένου να αναπτύσσεται πάντα, ακόμα και αφού κάνετε κλικ σε μια εντολή.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Περισσότερες εντολές"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης";
            this.quickAccessToolBarMenuShowAbove = "Εμφάνιση πάνω από την Κορδέλα";
            this.quickAccessToolBarMenuShowBelow = "Εμφάνιση κάτω από την Κορδέλα";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Προσθήκη στη γραμμή εργαλείων γρήγορης πρόσβασης"; // Button
            this.ribbonContextMenuAddGroup = "Προσθήκη ομάδας στη γραμμή εργαλείων γρήγορης πρόσβασης"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Προσθήκη συλλογής στη γραμμή εργαλείων γρήγορης πρόσβασης"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Προσθήκη μενού στη γραμμή εργαλείων γρήγορης πρόσβασης"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Κατάργηση από τη γραμμή εργαλείων γρήγορης πρόσβασης"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Προσαρμογή γραμμής εργαλείων γρήγορης πρόσβασης...";
            this.ribbonContextMenuShowBelow = "Εμφάνιση της γραμμής εργαλείων γρήγορης πρόσβασης κάτω από την Κορδέλα";
            this.ribbonContextMenuShowAbove = "Εμφάνιση της γραμμής εργαλείων γρήγορης πρόσβασης πάνω από την Κορδέλα";
            this.ribbonContextMenuCustomizeRibbon = "Προσαρμογή της Κορδέλας...";
            this.ribbonContextMenuMinimizeRibbon = "Ελαχιστοποίηση της Κορδέλας";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Αυτή η εντολή είναι απενεργοποιημένη προς το παρόν.";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Προσαρμογή γραμμής κατάστασης";
        }

        #endregion

        #region Korean

        private void LoadKorean()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "파일";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "리본 메뉴를 최소화 합니다 (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";
            this.expandButtonScreenTipTitle = "리본 메뉴를 표시합니다 (Ctrl + F1)";
            this.expandButtonScreenTipText = "리본 메뉴를 표시하거나 숨깁니다\n\n리본 메뉴가 숨김 상태일때만,\n탭이름이 보여집니다";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "빠른 실행 도구 모음 사용자 지정";
            this.quickAccessToolBarMoreControlsButtonTooltip = "기타 컨트롤들"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "빠른 실행 도구 모음 사용자 지정";
            this.quickAccessToolBarMenuShowAbove = "리본 메뉴 위에 표시";
            this.quickAccessToolBarMenuShowBelow = "리본 메뉴 아래에 표시";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "빠른 실행 도구 모음에 추가"; // Button
            this.ribbonContextMenuAddGroup = "그룹을 빠른 실행 도구 모음에 추가"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "갤러리를 빠른 실행 도구 모음에 추가"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "메뉴를 빠른 실행 도구 모음에 추가"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "빠른 실행 도구 모음에서 단추 제거"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "빠른 실행 도구 모음 사용자 지정...";
            this.ribbonContextMenuShowBelow = "리본 메뉴 아래에 빠른 실행 도구 모음 표시";
            this.ribbonContextMenuShowAbove = "리본 메뉴 위에 빠른 실행 도구 모음 표시";
            this.ribbonContextMenuCustomizeRibbon = "리본 메뉴 사용자 지정...";
            this.ribbonContextMenuMinimizeRibbon = "리본 메뉴 최소화";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "이 명령은 현재 사용할 수 없습니다.";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "상태 표시줄 사용자 지정";
        }

        #endregion

        #region Vietnamese

        private void LoadVietnamese()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Tệp";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Thu gọn Ruy băng (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ có tên thẻ được hiện";
            this.expandButtonScreenTipTitle = "Mở rộng Ruy băng (Ctrl + F1)";
            this.expandButtonScreenTipText = "Hiện hoặc ẩn Ruy băng\n\nKhi Ruy băng ẩn, chỉ có tên thẻ được hiện";

            // QAT tooltips and menu items
            this.quickAccessToolBarDropDownButtonTooltip = "Tùy chỉnh thanh công cụ Truy cập nhanh";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Thêm điều khiển"; // khi có hai mũi tên ">>"
            this.quickAccessToolBarMenuHeader = "Tùy chỉnh thanh công cụ Truy cập nhanh";
            this.quickAccessToolBarMenuShowAbove = "Hiện trên thanh Ruy băng";
            this.quickAccessToolBarMenuShowBelow = "Hiện dưới thanh Ruy băng";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Thêm vào thanh công cụ Truy cập nhanh"; // Button
            this.ribbonContextMenuAddGroup = "Thêm nhóm vào thanh công cụ Truy cập nhanh"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Thêm bộ sưu tập vào thanh công cụ Truy cập nhanh"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Thêm menu vào thanh công cụ Truy cập nhanh"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Loại"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Tùy chỉnh thanh công cụ Truy cập nhanh...";
            this.ribbonContextMenuShowBelow = "Hiện thanh công cụ truy cập nhanh dưới thanh Ruy băng";
            this.ribbonContextMenuShowAbove = "Hiện thanh công cụ truy cập nhanh trên thanh Ruy băng";
            this.ribbonContextMenuCustomizeRibbon = "Tùy biến thanh Ruy băng...";
            this.ribbonContextMenuMinimizeRibbon = "Thu gọn Ruy băng";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Lệnh này hiện bị tắt.";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Tùy biến thanh Trạng thái";
        }

        #endregion

        #region Sinhala (Sri Lanka)

        private void LoadSinhala()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "ගොනුව";
            this.backstageButtonKeyTip = "න1";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "රිබනය හකුළන්න (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "රිබනය මත පටිති නාම පමණක් පෙන්වන්න.";
            this.expandButtonScreenTipTitle = "රිබනය විහිදන්න (Ctrl + F1)";
            this.expandButtonScreenTipText = "රිබනය පෙන්වන්න, එවිට ඔබ\n\n විධානයක් ක්ලික් කළද එය\n\n සැමවිටම විහිදී පවතී.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය";
            this.quickAccessToolBarMoreControlsButtonTooltip = "තවත් විධාන"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය";
            this.quickAccessToolBarMenuShowAbove = "රිබනයට ඉහලින් පෙන්වන්න";
            this.quickAccessToolBarMenuShowBelow = "රිබනයට පහලින් පෙන්වන්න";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට එක් කරන්න"; // Button
            this.ribbonContextMenuAddGroup = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට එක් කරන්න"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට ගැලරිය එක් කරන්න"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයට මෙනුව එක් කරන්න"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරයෙන් ඉවත් කරන්න"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය අභිමත කරණය කරන්න...";
            this.ribbonContextMenuShowBelow = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය රිබනයට පහලින් පෙන්වන්න";
            this.ribbonContextMenuShowAbove = "ඉක්මන් ප්‍රෙව්ශ මෙවලම් තීරය රිබනයට ඉහලින් පෙන්වන්න";
            this.ribbonContextMenuCustomizeRibbon = "රිබනය අභිමත කරණය කරන්න...";
            this.ribbonContextMenuMinimizeRibbon = "රිබනය හකුළන්න";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "මෙම විධානය දැනට භාවිතා කළ නොහැක";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "තත්ව තීරුව අභිමත කරණය";
        }

        #endregion

        #region Slovenian

        private void LoadSlovenian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Datoteka";
            this.backstageButtonKeyTip = "D";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Minimiraj trak (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Pokaži ali skrij trak\n\nKo je trak skrit, so prikazani samo zavihki";
            this.expandButtonScreenTipTitle = "Razširi trak (Ctrl + F1)";
            this.expandButtonScreenTipText = "Pokaži ali skrij trak\n\nKo je trak skrit, so prikazani samo zavihki";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Prilagodi orodno vrstico za hitri dostop";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Več ukazov"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Prilagodi orodno vrstico za hitri dostop";
            this.quickAccessToolBarMenuShowAbove = "Pokaži nad trakom";
            this.quickAccessToolBarMenuShowBelow = "Pokaži pod trakom";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Dodaj v orodno vrstico za hitri dostop"; // Button
            this.ribbonContextMenuAddGroup = "Dodaj skupino orodni vrstici za hitri dostop"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Dodaj galerijo orodni vrstici za hitri dostop"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Dodaj meni orodni vrstici za hitri dostop"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Odstrani iz orodne vrstice za hitri dostop"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Prilagodi orodno vrstico za hitri dostop...";
            this.ribbonContextMenuShowBelow = "Pokaži orodno vrstico za hitri dostop pod trakom";
            this.ribbonContextMenuShowAbove = "Pokaži orodno vrstico za hitri dostop nad trakom";
            this.ribbonContextMenuCustomizeRibbon = "Prilagodi trak...";
            this.ribbonContextMenuMinimizeRibbon = "Minimiraj trak";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            this.screenTipDisableReasonHeader = "Ta ukaz je trenutno onemogočen.";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Prilagodi vrstico stanja";
        }

        #endregion

        #region Catalan

        private void LoadCatalan()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Fitxer";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            // TRANSLATOR'S NOTE: This block is not shown at Windows 7's Apps (WordPad or Paint)
            this.minimizeButtonScreenTipTitle = "Minimitza la cinta (Ctrl + F1)";
            this.minimizeButtonScreenTipText = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només s'ensenyen els noms de les pestanyes";
            this.expandButtonScreenTipTitle = "Expandeix la cinta (Ctrl + F1)";
            this.expandButtonScreenTipText = "Ensenya o amaga la cinta\n\nQuan la cinta no es mostri, només s'ensenyen els noms de les pestanyes";

            // QAT tooltips and menu items
            this.quickAccessToolBarDropDownButtonTooltip = "Personalitza la barra d'eines d'accés ràpid";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Més controls"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Personalitza la barra d'eines d'accés ràpid";
            this.quickAccessToolBarMenuShowAbove = "Mostra sobre la cinta";
            this.quickAccessToolBarMenuShowBelow = "Mostra sota la cinta";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Afegeix a la barra d'eines d'accés ràpid"; // Button
            this.ribbonContextMenuAddGroup = "Afegeix grup a la barra d'eines d'accés ràpid"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Afegeix galeria a la barra d'eines d'accés ràpid"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Afegeix menú a la barra d'eines d'accés ràpid"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Elimina la barra d'eines d'accés ràpid"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Personalitza la barra d'eines d'accés ràpid...";
            this.ribbonContextMenuShowBelow = "Mostra la barra d'eines d'accés ràpid sota la cinta";
            this.ribbonContextMenuShowAbove = "Mostra la barra d'eines d'accés ràpid sobre la cinta";
            this.ribbonContextMenuCustomizeRibbon = "Personalitza la cinta...";
            this.ribbonContextMenuMinimizeRibbon = "Minimitza la cinta";
        }

        #endregion

        #region Estonian

        private void LoadEstonian()
        {
            // Backstage button text & key tip
            this.backstageButtonText = "Fail";
            this.backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            this.minimizeButtonScreenTipTitle = "Ahenda menüülint (Ctrl+F1)";
            this.minimizeButtonScreenTipText = "Kas vajate rohkem ruumi? Ahendage lint, siis kuvatakse \nainult menüünimed.";
            this.expandButtonScreenTipTitle = "Kinnita lint (Ctrl+F1)";
            this.expandButtonScreenTipText = "Kas soovite, et lint oleks kuvatud? Saate selle töötamise \najal avatuna hoida.";

            // QAT tooltips and menu items            
            this.quickAccessToolBarDropDownButtonTooltip = "Kohanda kiirpääsuriba";
            this.quickAccessToolBarMoreControlsButtonTooltip = "Rohkem juhtelemente"; // When two arrows appear ">>"
            this.quickAccessToolBarMenuHeader = "Kohanda kiirpääsuriba";
            this.quickAccessToolBarMenuShowAbove = "Kuva lindi kohal";
            this.quickAccessToolBarMenuShowBelow = "Kuva lindi all";

            // Click on Ribbon to show context menu
            this.ribbonContextMenuAddItem = "Lisa kiirpääsuribale"; // Button
            this.ribbonContextMenuAddGroup = "Lisa rühm kiirpääsuribale"; // For ex., by collapsed group
            this.ribbonContextMenuAddGallery = "Lisa galerii kiirpääsuribale"; // For ex., by opened font context menu
            this.ribbonContextMenuAddMenu = "Lisa menüü kiirpääsuribale"; // By dashed splitter in context menu
            this.ribbonContextMenuRemoveItem = "Eemalda kiirpääsuribalt"; // By item in QAT
            this.ribbonContextMenuCustomizeQuickAccessToolbar = "Kohanda kiirpääsuriba...";
            this.ribbonContextMenuShowBelow = "Kuva kiirpääsuriba lindi all";
            this.ribbonContextMenuShowAbove = "Kuva kiirpääsuriba lindi kohal";
            this.ribbonContextMenuCustomizeRibbon = "Kohanda linti...";
            this.ribbonContextMenuMinimizeRibbon = "Ahenda menüülint";

            // To see it in Word: open *.doc (not *.docx) and see Insert->Screenshot
            // (This prop was introduced after v1.3) 
            //Screentips
            this.screenTipDisableReasonHeader = "See käsk on praegu keelatud.";
            this.screenTipF1LabelHeader = "Spikri kuvamiseks vajutage klahvi F1";

            // Right-click on status bar to see it. NEW! from v2.0
            this.customizeStatusBar = "Kohanda olekuriba";
        }

        #endregion
    }
}