#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Fluent
{
    public enum Themes
    {
        Default=0,
        Office2010Silver,
        Office2010Black,
        Office2010Blue,
        Office2007Silver,
        Office2007Black,
        Office2007Blue,
        Scenic
    }
    /// <summary>
    /// Represets functionality for theme  managment
    /// </summary>
    public static class ThemesManager
    {
        #region Default style

        #region Fields
        
        // Default style dictionary
        private static ResourceDictionary defaultResourceDictionary;

        #endregion

        #region Properties

        /// <summary>
        /// Gets 
        /// </summary>
        internal static ResourceDictionary DefaultResourceDictionary
        {
            get
            {
                if(defaultResourceDictionary==null)
                {
                    defaultResourceDictionary = new ResourceDictionary();
                    defaultResourceDictionary.BeginInit();
                    defaultResourceDictionary.Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml");
                    defaultResourceDictionary.EndInit();
                }
                return defaultResourceDictionary;
            }
        }

        internal static Style DefaultBackstageStyle
        {
            get{ return DefaultResourceDictionary["BackstageStyle"] as Style;}
        }

        internal static Style DefaultBackstageButtonStyle
        {
            get{ return DefaultResourceDictionary["BackstageButtonStyle"] as Style;}
        }

        internal static Style DefaultBackstageTabItemStyle
        {
            get{ return DefaultResourceDictionary["BackstageTabItemStyle"] as Style;}
        }

        internal static Style DefaultButtonStyle
        {
            get{ return DefaultResourceDictionary["RibbonButtonStyle"] as Style;}
        }

        internal static Style DefaultContextMenuStyle
        {
            get{ return DefaultResourceDictionary["EmptyContextMenuStyle"] as Style;}
        }

        internal static Style DefaultContextMenuBarStyle
        {
            get{ return DefaultResourceDictionary["ContextMenuBarStyle"] as Style;}
        }

        internal static Style DefaultDropDownButtonStyle
        {
            get{ return DefaultResourceDictionary["RibbonDropDownButtonStyle"] as Style;}
        }

        internal static Style DefaultSplitButtonStyle
        {
            get { return DefaultResourceDictionary["RibbonSplitButtonStyle"] as Style; }
        }

        internal static Style DefaultGroupSeparatorMenuItemStyle
        {
            get { return DefaultResourceDictionary["MenuGroupSeparatorStyle"] as Style; }
        }

        internal static Style DefaulKeyTipStyle
        {
            get{ return DefaultResourceDictionary["KeyTipStyle"] as Style;}
        }

        internal static Style DefaulQuickAccessToolBarStyle
        {
            get { return DefaultResourceDictionary["QuickAccessToolbarStyle"] as Style; }
        }

        internal static Style DefaultRibbonStyle
        {
            get { return DefaultResourceDictionary["RibbonStyle"] as Style; }
        }

        internal static Style DefaultRibbonContextualTabGroupStyle
        {
            get { return DefaultResourceDictionary["RibbonContextualTabGroupStyle"] as Style; }
        }

        internal static Style DefaultRibbonGroupBoxStyle
        {
            get { return DefaultResourceDictionary["RibbonGroupBoxStyle"] as Style; }
        }

        internal static Style DefaultRibbonTabControlStyle
        {
            get { return DefaultResourceDictionary["RibbonTabControlStyle"] as Style; }
        }

        internal static Style DefaultRibbonTabItemStyle
        {
            get { return DefaultResourceDictionary["RibbonTabItemStyle"] as Style; }
        }

        internal static Style DefaultRibbonTitleBarStyle
        {
            get { return DefaultResourceDictionary["RibbonTitleBarStyle"] as Style; }
        }

        internal static Style DefaultRibbonWindowStyle
        {
            get { return DefaultResourceDictionary["RibbonWindowStyle"] as Style; }
        }

        internal static Style DefaultScreenTipStyle
        {
            get { return DefaultResourceDictionary["ScreenTipStyle"] as Style; }
        }

        internal static Style DefaultToggleButtonStyle
        {
            get { return DefaultResourceDictionary["RibbonToggleButtonStyle"] as Style; }
        }

        internal static Style DefaultTwoLineLabelStyle
        {
            get { return DefaultResourceDictionary["ControlLabelStyle"] as Style; }
        }

        internal static Style DefaultRibbonToolBarControlGroupStyle
        {
            get { return DefaultResourceDictionary["RibbonToolBarControlGroupStyle"] as Style; }
        }

        #endregion

        #endregion

        #region Office 2010 Silver style

        #region Fields

        private static ResourceDictionary office2010SilverResourceDictionary;

        #endregion

        #region Properties

        internal static ResourceDictionary Office2010SilverResourceDictionary
        {
            get
            {
                if (office2010SilverResourceDictionary == null)
                {
                    office2010SilverResourceDictionary = new ResourceDictionary();
                    office2010SilverResourceDictionary.BeginInit();
                    office2010SilverResourceDictionary.Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml");
                    office2010SilverResourceDictionary.EndInit();
                }
                return office2010SilverResourceDictionary;
            }
        }

        #endregion

        #endregion

        #region Office 2010 Black style

        #region Fields

        private static ResourceDictionary office2010BlackResourceDictionary;

        #endregion
        
        #region Properties

        internal static ResourceDictionary Office2010BlackResourceDictionary
        {
            get
            {
                if (office2010BlackResourceDictionary == null)
                {
                    office2010BlackResourceDictionary = new ResourceDictionary();
                    office2010BlackResourceDictionary.BeginInit();
                    office2010BlackResourceDictionary.Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml");
                    office2010BlackResourceDictionary.EndInit();
                }
                return office2010BlackResourceDictionary;
            }
        }


        #endregion

        #endregion

        #region Properties

        public static Themes GetTheme(DependencyObject obj)
        {
            return (Themes)obj.GetValue(ThemeProperty);
        }

        public static void SetTheme(DependencyObject obj, Themes value)
        {
            obj.SetValue(ThemeProperty, value);
        }

        // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.RegisterAttached("Theme", typeof(Themes), typeof(ThemesManager), new UIPropertyMetadata(Themes.Default, OnThemeChanged));

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.OldValue!=e.NewValue)
            {
                SetTheme(d as Window, (Themes)e.NewValue, (Themes)e.OldValue);
            }
        }

        #endregion

        #region Methods

        private static void SetTheme(Window window, ResourceDictionary newDictionary, ResourceDictionary oldDictionary)
        {
            if ((newDictionary==oldDictionary)&&(window.Resources.MergedDictionaries.Contains(oldDictionary))) return;
            window.BeginInit();
            window.Resources.MergedDictionaries.Add(newDictionary);
            if ((window.Resources.MergedDictionaries.Contains(oldDictionary)) && ((newDictionary != oldDictionary))) window.Resources.MergedDictionaries.Remove(oldDictionary);
            window.EndInit();
        }

        internal static void SetTheme(Window window, Themes newTheme, Themes oldTheme)
        {
            SetTheme(window, GetDictionary(newTheme), GetDictionary(oldTheme));
        }           

        private static ResourceDictionary GetDictionary(Themes theme)
        {
            switch (theme)
            {
                case Themes.Default: return DefaultResourceDictionary;
                case Themes.Office2010Silver: return Office2010SilverResourceDictionary;
                case Themes.Office2010Black: return Office2010BlackResourceDictionary; 
                default: throw new NotImplementedException(); break;
            }
        }

	    #endregion
    }
}
