#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion


using System.Globalization;
using System.Windows;

namespace Fluent
{
    /// <summary>
    /// Contains localizable Ribbon's properties. 
    /// Set Culture property to change current Ribbon localization or 
    /// set properties independently to use your localization
    /// </summary>
    public class RibbonLocalization : DependencyObject
    {
        #region Culture

        /// <summary>
        /// Gets or sets current culture used for localization
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Culture. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(RibbonLocalization), new UIPropertyMetadata(null, OnCultureChanged));

        static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            localization.LoadCulture((CultureInfo)e.NewValue);
        }

        #endregion

        #region Text of backstage button

        // Text of backstage button
        string backstageButtonText = "File";

        /// <summary>
        /// Gets or sets text of backstage button
        /// </summary>
        public string BackstageButtonText
        {
            get { return (string)GetValue(BackstageButtonTextProperty); }
            set { SetValue(BackstageButtonTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BackstageButtonText.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageButtonTextProperty =
            DependencyProperty.Register("BackstageButtonText", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceBackstageButtonText));

        // Coerce value
        static object OnCoerceBackstageButtonText(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.backstageButtonText;
            return basevalue;
        }

        #endregion

        #region KeyTip of backstage button

        // KeyTip of backstage button
        string backstageButtonKeyTip = "F";

        /// <summary>
        /// Gets or sets KeyTip of backstage button
        /// </summary>
        public string BackstageButtonKeyTip
        {
            get { return (string)GetValue(BackstageButtonKeyTipProperty); }
            set { SetValue(BackstageButtonKeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BackstageButtonKeyTip.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty BackstageButtonKeyTipProperty =
            DependencyProperty.Register("BackstageButtonKeyTip", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceBackstageButtonKeyTip));

        // Coerce value
        static object OnCoerceBackstageButtonKeyTip(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.backstageButtonKeyTip;
            return basevalue;
        }

        #endregion

        #region Minimize Button ScreenTip Title

        // Minimize Button ScreenTip Title
        string minimizeButtonScreenTipTitle = "Minimize the Ribbon (Ctrl + F1)";

        /// <summary>
        /// Minimize Button ScreenTip Title
        /// </summary>
        public string MinimizeButtonScreenTipTitle
        {
            get { return (string)GetValue(MinimizeButtonScreenTipTitleProperty); }
            set { SetValue(MinimizeButtonScreenTipTitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimizeButtonScreenTipTitle.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonScreenTipTitleProperty =
            DependencyProperty.Register("MinimizeButtonScreenTipTitle", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceMinimizeButtonScreenTipTitle));

        // Coerce value
        static object OnCoerceMinimizeButtonScreenTipTitle(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.minimizeButtonScreenTipTitle;
            return basevalue;
        }

        #endregion

        #region Minimize Button ScreenTip Text

        // Minimize Button ScreenTip Text
        string minimizeButtonScreenTipText = "Show or hide the Ribbon\n\nWhen the Ribbon is hidden, only\nthe tab names are shown";

        /// <summary>
        /// Minimize Button ScreenTip Text
        /// </summary>
        public string MinimizeButtonScreenTipText
        {
            get { return (string)GetValue(MinimizeButtonScreenTipTextProperty); }
            set { SetValue(MinimizeButtonScreenTipTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinimizeButtonScreenTipText.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinimizeButtonScreenTipTextProperty =
            DependencyProperty.Register("MinimizeButtonScreenTipText", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceMinimizeButtonScreenTipText));

        // Coerce value
        static object OnCoerceMinimizeButtonScreenTipText(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.minimizeButtonScreenTipText;
            return basevalue;
        }

        #endregion

        #region Expand Button ScreenTip Title

        // Expand Button ScreenTip Title
        string expandButtonScreenTipTitle = "Expand the Ribbon (Ctrl + F1)";

        /// <summary>
        /// Expand Button ScreenTip Title
        /// </summary>
        public string ExpandButtonScreenTipTitle
        {
            get { return (string)GetValue(ExpandButtonScreenTipTitleProperty); }
            set { SetValue(ExpandButtonScreenTipTitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ExpandButtonScreenTipTitle.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExpandButtonScreenTipTitleProperty =
            DependencyProperty.Register("ExpandButtonScreenTipTitle", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceExpandButtonScreenTipTitle));

        // Coerce value
        static object OnCoerceExpandButtonScreenTipTitle(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.expandButtonScreenTipTitle;
            return basevalue;
        }

        #endregion

        #region Expand Button ScreenTip Text

        // Expand Button ScreenTip Text
        string expandButtonScreenTipText = "Show or hide the Ribbon\n\nWhen the Ribbon is hidden, only\nthe tab names are shown";

        /// <summary>
        /// Expand Button ScreenTip Text
        /// </summary>
        public string ExpandButtonScreenTipText
        {
            get { return (string)GetValue(ExpandButtonScreenTipTextProperty); }
            set { SetValue(ExpandButtonScreenTipTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ExpandButtonScreenTipText.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ExpandButtonScreenTipTextProperty =
            DependencyProperty.Register("ExpandButtonScreenTipText", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceExpandButtonScreenTipText));

        // Coerce value
        static object OnCoerceExpandButtonScreenTipText(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.expandButtonScreenTipText;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar DropDown Button ToolTip

        // Quick Access ToolBar DropDown Button ToolTip
        private string quickAccessToolBarDropDownButtonTooltip = "Customize Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar DropDown Button ToolTip
        /// </summary>
        public string QuickAccessToolBarDropDownButtonTooltip
        {
            get { return (string)GetValue(QuickAccessToolBarDropDownButtonTooltipProperty); }
            set { SetValue(QuickAccessToolBarDropDownButtonTooltipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for QuickAccessToolBarDropDownButtonTooltip.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarDropDownButtonTooltipProperty =
            DependencyProperty.Register("QuickAccessToolBarDropDownButtonTooltip", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceQuickAccessToolBarDropDownButtonTooltip));

        // Coerce value
        static object OnCoerceQuickAccessToolBarDropDownButtonTooltip(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.quickAccessToolBarDropDownButtonTooltip;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar MoreControls Button ToolTip

        // Quick Access ToolBar MoreControls Button ToolTip
        private string quickAccessToolBarMoreControlsButtonTooltip = "More controls";

        /// <summary>
        /// Quick Access ToolBar MoreControls Button ToolTip
        /// </summary>
        public string QuickAccessToolBarMoreControlsButtonTooltip
        {
            get { return (string)GetValue(QuickAccessToolBarMoreControlsButtonTooltipProperty); }
            set { SetValue(QuickAccessToolBarMoreControlsButtonTooltipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for QuickAccessToolBarMoreControlsButtonTooltip.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarMoreControlsButtonTooltipProperty =
            DependencyProperty.Register("QuickAccessToolBarMoreControlsButtonTooltip", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceQuickAccessToolBarMoreControlsButtonTooltip));

        // Coerce value
        static object OnCoerceQuickAccessToolBarMoreControlsButtonTooltip(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.quickAccessToolBarMoreControlsButtonTooltip;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Header

        // Quick Access ToolBar  Menu Header
        private string quickAccessToolBarMenuHeader = "Customize Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar  Menu Header
        /// </summary>
        public string QuickAccessToolBarMenuHeader
        {
            get { return (string)GetValue(QuickAccessToolBarMenuHeaderProperty); }
            set { SetValue(QuickAccessToolBarMenuHeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for QuickAccessToolBarButtonTooltip.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarMenuHeaderProperty =
            DependencyProperty.Register("QuickAccessToolBarMenuHeader", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceQuickAccessToolBarMenuHeader));

        // Coerce value
        static object OnCoerceQuickAccessToolBarMenuHeader(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.quickAccessToolBarMenuHeader;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Context Menu Show Below

        // Quick Access ToolBar Minimize Quick Access Toolbar
        private string quickAccessToolBarMenuShowBelow = "Show Below the Ribbon";

        /// <summary>
        /// Quick Access ToolBar Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowBelow
        {
            get { return (string)GetValue(QuickAccessToolBarMenuShowBelowProperty); }
            set { SetValue(QuickAccessToolBarMenuShowBelowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for QuickAccessToolBarMenuShowBelow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarMenuShowBelowProperty =
            DependencyProperty.Register("QuickAccessToolBarMenuShowBelow", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceQuickAccessToolBarMenuShowBelow));

        // Coerce value
        static object OnCoerceQuickAccessToolBarMenuShowBelow(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.quickAccessToolBarMenuShowBelow;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Show Above

        // Quick Access ToolBar Menu Minimize Quick Access Toolbar
        private string quickAccessToolBarMenuShowAbove = "Show Above the Ribbon";

        /// <summary>
        /// Quick Access ToolBar Menu Minimize Quick Access Toolbar
        /// </summary>
        public string QuickAccessToolBarMenuShowAbove
        {
            get { return (string)GetValue(QuickAccessToolBarMenuShowAboveProperty); }
            set { SetValue(QuickAccessToolBarMenuShowAboveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for QuickAccessToolBarMenuShowAbove.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty QuickAccessToolBarMenuShowAboveProperty =
            DependencyProperty.Register("QuickAccessToolBarMenuShowAbove", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceQuickAccessToolBarMenuShowAbove));

        // Coerce value
        static object OnCoerceQuickAccessToolBarMenuShowAbove(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.quickAccessToolBarMenuShowAbove;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Add Item

        // Quick Access ToolBar Menu Add Item
        private string ribbonContextMenuAddItem = "Add to Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar Menu Add Item
        /// </summary>
        public string RibbonContextMenuAddItem
        {
            get { return (string)GetValue(RibbonContextMenuAddItemProperty); }
            set { SetValue(RibbonContextMenuAddItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuAddItem.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuAddItemProperty =
            DependencyProperty.Register("RibbonContextMenuAddItem", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceQuickAccessToolBarMenuAddItem));

        // Coerce value
        static object OnCoerceQuickAccessToolBarMenuAddItem(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuAddItem;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Add Group

        // Quick Access ToolBar Menu Add Group
        private string ribbonContextMenuAddGroup = "Add Group to Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar Menu Add Group
        /// </summary>
        public string RibbonContextMenuAddGroup
        {
            get { return (string)GetValue(RibbonContextMenuAddGroupProperty); }
            set { SetValue(RibbonContextMenuAddGroupProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuAddGroup.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuAddGroupProperty =
            DependencyProperty.Register("RibbonContextMenuAddGroup", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuAddGroup));

        // Coerce value
        static object OnCoerceRibbonContextMenuAddGroup(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuAddGroup;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Add Gallery

        // Quick Access ToolBar Menu Add Gallery
        private string ribbonContextMenuAddGallery = "Add Gallery to Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar Menu Add Gallery
        /// </summary>
        public string RibbonContextMenuAddGallery
        {
            get { return (string)GetValue(RibbonContextMenuAddGalleryProperty); }
            set { SetValue(RibbonContextMenuAddGalleryProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuAddGallery.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuAddGalleryProperty =
            DependencyProperty.Register("RibbonContextMenuAddGallery", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuAddGallery));

        // Coerce value
        static object OnCoerceRibbonContextMenuAddGallery(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuAddGallery;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Add Menu

        // Quick Access ToolBar Menu Add Menu
        private string ribbonContextMenuAddMenu = "Add Menu to Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar Menu Add Menu
        /// </summary>
        public string RibbonContextMenuAddMenu
        {
            get { return (string)GetValue(RibbonContextMenuAddMenuProperty); }
            set { SetValue(RibbonContextMenuAddMenuProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuAddMenu.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuAddMenuProperty =
            DependencyProperty.Register("RibbonContextMenuAddMenu", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuAddMenu));

        // Coerce value
        static object OnCoerceRibbonContextMenuAddMenu(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuAddMenu;
            return basevalue;
        }

        #endregion

        #region Quick Access ToolBar Menu Remove Item

        // Quick Access ToolBar Menu Remove Item
        private string ribbonContextMenuRemoveItem = "Remove from Quick Access Toolbar";

        /// <summary>
        /// Quick Access ToolBar Menu Remove Item
        /// </summary>
        public string RibbonContextMenuRemoveItem
        {
            get { return (string)GetValue(RibbonContextMenuRemoveItemProperty); }
            set { SetValue(RibbonContextMenuRemoveItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuRemoveItem.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuRemoveItemProperty =
            DependencyProperty.Register("RibbonContextMenuRemoveItem", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuRemoveItem));

        // Coerce value
        static object OnCoerceRibbonContextMenuRemoveItem(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuRemoveItem;
            return basevalue;
        }

        #endregion

        #region Ribbon Context Menu Customize Quick Access Toolbar

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string ribbonContextMenuCustomizeQuickAccessToolbar = "Customize Quick Access Toolbar...";

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeQuickAccessToolBar
        {
            get { return (string)GetValue(RibbonContextMenuCustomizeQuickAccessToolBarProperty); }
            set { SetValue(RibbonContextMenuCustomizeQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuCustomizeQuickAccessToolBar.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuCustomizeQuickAccessToolBarProperty =
            DependencyProperty.Register("RibbonContextMenuCustomizeQuickAccessToolBar", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuCustomizeQuickAccessToolbar));

        // Coerce value
        static object OnCoerceRibbonContextMenuCustomizeQuickAccessToolbar(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuCustomizeQuickAccessToolbar;
            return basevalue;
        }

        #endregion

        #region Ribbon Context Menu Customize Ribbon

        // Ribbon Context Menu Customize Quick Access Toolbar
        private string ribbonContextMenuCustomizeRibbon = "Customize the Ribbon...";

        /// <summary>
        /// Ribbon Context Menu Customize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuCustomizeRibbon
        {
            get { return (string)GetValue(RibbonContextMenuCustomizeRibbonProperty); }
            set { SetValue(RibbonContextMenuCustomizeRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuCustomizeRibbon.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuCustomizeRibbonProperty =
            DependencyProperty.Register("RibbonContextMenuCustomizeRibbon", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuCustomizeRibbon));

        // Coerce value
        static object OnCoerceRibbonContextMenuCustomizeRibbon(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuCustomizeRibbon;
            return basevalue;
        }

        #endregion

        #region Ribbon Context Menu Minimize Ribbon

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuMinimizeRibbon = "Minimize the Ribbon";

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuMinimizeRibbon
        {
            get { return (string)GetValue(RibbonContextMenuMinimizeRibbonProperty); }
            set { SetValue(RibbonContextMenuMinimizeRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuMinimizeRibbon.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuMinimizeRibbonProperty =
            DependencyProperty.Register("RibbonContextMenuMinimizeRibbon", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuMinimizeRibbon));

        // Coerce value
        static object OnCoerceRibbonContextMenuMinimizeRibbon(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuMinimizeRibbon;
            return basevalue;
        }

        #endregion

        #region Ribbon Context Menu Show Below

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuShowBelow = "Show Quick Access Toolbar Below the Ribbon";

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowBelow
        {
            get { return (string)GetValue(RibbonContextMenuShowBelowProperty); }
            set { SetValue(RibbonContextMenuShowBelowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuShowBelow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuShowBelowProperty =
            DependencyProperty.Register("RibbonContextMenuShowBelow", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuShowBelow));

        // Coerce value
        static object OnCoerceRibbonContextMenuShowBelow(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuShowBelow;
            return basevalue;
        }

        #endregion

        #region Ribbon Context Menu Show Above

        // Ribbon Context Menu Minimize Quick Access Toolbar
        private string ribbonContextMenuShowAbove = "Show Quick Access Toolbar Above the Ribbon";

        /// <summary>
        /// Ribbon Context Menu Minimize Quick Access Toolbar
        /// </summary>
        public string RibbonContextMenuShowAbove
        {
            get { return (string)GetValue(RibbonContextMenuShowAboveProperty); }
            set { SetValue(RibbonContextMenuShowAboveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for RibbonContextMenuShowAbove.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty RibbonContextMenuShowAboveProperty =
            DependencyProperty.Register("RibbonContextMenuShowAbove", typeof(string), typeof(RibbonLocalization),
                                        new UIPropertyMetadata(null, null, OnCoerceRibbonContextMenuShowAbove));

        // Coerce value
        static object OnCoerceRibbonContextMenuShowAbove(DependencyObject d, object basevalue)
        {
            RibbonLocalization localization = (RibbonLocalization)d;
            if (basevalue == null) return localization.ribbonContextMenuShowAbove;
            return basevalue;
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
                case "ru": LoadRussian(); break;
                case "fa": LoadPersian(); break;
                case "de": LoadGerman(); break;
                case "hu": LoadHungarian(); break;
                case "cs": LoadCzech(); break;
                case "fr": LoadFrench(); break;
                case "pl": LoadPolish(); break;
                case "ja": LoadJapanese(); break;
                case "nl": LoadDutch(); break;
                case "pt": LoadPortuguese(); break;
                case "es": LoadSpanish(); break;
                case "zh": LoadChinese(); break;
                case "sv": LoadSwedish(); break;
            }

            // Coerce all values
            CoerceValue(BackstageButtonTextProperty);
            CoerceValue(BackstageButtonKeyTipProperty);

            CoerceValue(MinimizeButtonScreenTipTitleProperty);
            CoerceValue(MinimizeButtonScreenTipTextProperty);
            CoerceValue(ExpandButtonScreenTipTitleProperty);
            CoerceValue(ExpandButtonScreenTipTextProperty);
            CoerceValue(QuickAccessToolBarDropDownButtonTooltipProperty);
            CoerceValue(QuickAccessToolBarMoreControlsButtonTooltipProperty);
            CoerceValue(QuickAccessToolBarMenuHeaderProperty);
            CoerceValue(QuickAccessToolBarMenuShowAboveProperty);
            CoerceValue(QuickAccessToolBarMenuShowBelowProperty);

            CoerceValue(RibbonContextMenuAddItemProperty);
            CoerceValue(RibbonContextMenuAddGroupProperty);
            CoerceValue(RibbonContextMenuAddGalleryProperty);
            CoerceValue(RibbonContextMenuAddMenuProperty);
            CoerceValue(RibbonContextMenuRemoveItemProperty);
            CoerceValue(RibbonContextMenuCustomizeRibbonProperty);
            CoerceValue(RibbonContextMenuCustomizeQuickAccessToolBarProperty);
            CoerceValue(RibbonContextMenuShowAboveProperty);
            CoerceValue(RibbonContextMenuShowBelowProperty);
            CoerceValue(RibbonContextMenuMinimizeRibbonProperty);
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
            minimizeButtonScreenTipText = "Das Menüband anzeigen oder ausblenden.\n\nWenn das Menüband\nausgeblendet ist, werden nur die\nRegisterkartennamen angezeigt.";
            expandButtonScreenTipTitle = "Menüband erweitern (Strg + F1)";
            expandButtonScreenTipText = "Das Menüband anzeigen oder ausblenden.\n\nWenn das Menüband\nausgeblendet ist, werden nur die\nRegisterkartennamen angezeigt.";

            // QAT tooltips and menu items
            quickAccessToolBarDropDownButtonTooltip = "Symbolleiste für den Schnellzugriff anpassen";
            quickAccessToolBarMoreControlsButtonTooltip = "Weitere Befehle…"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Symbolleiste für den Schnellzugriff anpassen";
            quickAccessToolBarMenuShowAbove = "Über dem Menüband anzeigen";
            quickAccessToolBarMenuShowBelow = "Unter dem Menüband anzeigen";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Zu Symbolleiste für den Schnellzugriff hinzufügen"; // Button
            ribbonContextMenuAddGroup = "Gruppe zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by collapsed group
            ribbonContextMenuAddGallery = "Katalog zur Symbolleiste für den Schnellzugriff hinzufügen"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Zu Symbolleiste für den Schnellzugriff hinzufügen"; // By dashed splitter in context menu
            ribbonContextMenuRemoveItem = "Aus Symbolleiste für den Schnellzugriff entfernen"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Symbolleiste für den Schnellzugriff anpassen...";
            ribbonContextMenuShowBelow = "Symbolleiste für den Schnellzugriff unter dem Menüband anzeigen";
            ribbonContextMenuShowAbove = "Symbolleiste für den Schnellzugriff über dem Menüband anzeigen";
            ribbonContextMenuCustomizeRibbon = "Menüband anpassen...";
            ribbonContextMenuMinimizeRibbon = "Menüband minimieren";
        }

        #endregion

        #region Hungarian

        // Maybe not correct (?)
        void LoadHungarian()
        {
            // Backstage button text & key tip
            backstageButtonText = "Fájl";
            backstageButtonKeyTip = "F";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "A szalag ikonállapotúra kicsinyítése (Ctrl + F1)";
            minimizeButtonScreenTipText = "A szalag megjelenítése vagy elrejtése\n\nAmikor a szalag rejtett csak\na fülek neveit mutatja"; // guess, cannot find
            expandButtonScreenTipTitle = "A szalag kibontása (Ctrl + F1)"; // guess
            expandButtonScreenTipText = "A szalag megjelenítése vagy elrejtése\n\nAmikor a szalag rejtett csak\na fülek neveit mutatja"; // guess, cannot find

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Gyorselérési eszköztár testreszabása";
            quickAccessToolBarMoreControlsButtonTooltip = "További vezérlők"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Gyorselérési eszköztár testreszabása";
            quickAccessToolBarMenuShowAbove = "Megjelenítés a menüszalag felett";
            quickAccessToolBarMenuShowBelow = "Megjelenítés a menüszalag alatt";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Felvétel a gyorselérési eszköztárra"; // Button
            ribbonContextMenuAddGroup = "Csoport felvétele a gyorselérési eszköztárra"; // guess
            ribbonContextMenuAddGallery = "Gyűjtemény felvétele a gyorselérési eszköztárra"; // For ex., by opened font context menu
            ribbonContextMenuAddMenu = "Menü felvétele a gyorselérési eszköztárra"; // guess
            ribbonContextMenuRemoveItem = "Eltávolítás a gyorselérési eszköztárról"; // By item in QAT
            ribbonContextMenuCustomizeQuickAccessToolbar = "Gyorselérési eszköztár testreszabása...";
            ribbonContextMenuShowBelow = "Gyorselérési eszköztár megjelenítése a menüszalag alatt"; // guess
            ribbonContextMenuShowAbove = "Gyorselérési eszköztár megjelenítése a menüszalag felett"; // guess
            ribbonContextMenuCustomizeRibbon = "A szalag testreszabása..."; // guess
            ribbonContextMenuMinimizeRibbon = "A szalag kicsinyítése"; // guess
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

        #region Portuguese

        void LoadPortuguese()
        {
            // Backstage button text & key tip
            backstageButtonText = "Arquivo";
            backstageButtonKeyTip = "A";

            // See right-top corner... (two different tooltips must be if you press it)
            minimizeButtonScreenTipTitle = "Ocultar a barra de ferramentas (Ctrl + F1)";
            minimizeButtonScreenTipText = "Exibir ou ocultar a barra de ferramentas\n\nQuando a barra de ferramentas estiver oculta, apenas\nos nomes das guias serão exibidas";
            expandButtonScreenTipTitle = "Exibir a barra de ferramentas (Ctrl + F1)";
            expandButtonScreenTipText = "Exibir ou ocultar a barra de ferramentas\n\nQuando a barra de ferramentas estiver oculta, apenas\nos nomes das guias serão exibidas";

            // QAT tooltips and menu items            
            quickAccessToolBarDropDownButtonTooltip = "Personalizar barra de ferramentas de acesso rápido";
            quickAccessToolBarMoreControlsButtonTooltip = "Mais controles"; // When two arrows appear ">>"
            quickAccessToolBarMenuHeader = "Personalizar barra de ferramentas de acesso rápido";
            quickAccessToolBarMenuShowAbove = "Exibir abaixo da barra de ferramentas";
            quickAccessToolBarMenuShowBelow = "Exibir abaixo da barra de ferramentas";

            // Click on Ribbon to show context menu
            ribbonContextMenuAddItem = "Adicionar à barra de ferramentas de acesso rápido";
            ribbonContextMenuAddGroup = "Adicionar à um grupo da barra de ferramentas de acesso rápido";
            ribbonContextMenuAddGallery = "Adicionar grupo à barra de ferramentas de acesso rápido";
            ribbonContextMenuAddMenu = "Adicionar um menu à barra de ferramentas de acesso rápido";
            ribbonContextMenuRemoveItem = "Remover da barra de ferramentas de acesso rápido";
            ribbonContextMenuCustomizeQuickAccessToolbar = "Personalizar barra de ferramentas de acesso rápido...";
            ribbonContextMenuShowBelow = "Exibir a barra de ferramentas de acesso rápido abaixo da barra de ferramentas";
            ribbonContextMenuShowAbove = "Exibir a barra de ferramentas de acesso rápido acima da barra de ferramentas";
            ribbonContextMenuCustomizeRibbon = "Personalizar barra de ferramentas...";
            ribbonContextMenuMinimizeRibbon = "Ocultar barra de ferramentas";

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
    }
}