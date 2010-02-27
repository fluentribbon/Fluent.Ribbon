#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// This interface must be implemented for controls
    /// which are intended to insert to quick access toolbar
    /// </summary>
    public interface IQuickAccessItemProvider
    {
        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        UIElement CreateQuickAccessItem();

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        bool CanAddToQuickAccessToolBar { get; set; }
    }
    
    /// <summary>
    /// Peresents quick access shortcut to another control
    /// </summary>
    [ContentProperty("Target")]
    public class QuickAccessMenuItem : MenuItem
    {
        #region Initialization

        static QuickAccessMenuItem()
        {
            CanAutoCheckProperty.AddOwner(typeof(QuickAccessMenuItem), new UIPropertyMetadata(true));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public QuickAccessMenuItem()
        {
            Checked += OnChecked;
            Unchecked += OnUnchecked;
            Loaded += OnItemLoaded;
        }

        #endregion

        #region Target Property

        /// <summary>
        /// Gets or sets shortcut to the target control
        /// </summary>
        public Control Target
        {
            get { return (Control)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for shortcut. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(Control), typeof(QuickAccessMenuItem), new UIPropertyMetadata(null,OnTargetChanged));

        static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            QuickAccessMenuItem quickAccessMenuItem = (QuickAccessMenuItem) d;
            RibbonControl ribbonControl = e.NewValue as RibbonControl;
            if ((String.IsNullOrEmpty(quickAccessMenuItem.Text)) && (ribbonControl != null))
            {
                // Set Default Text Value
                Bind(ribbonControl, quickAccessMenuItem, "Text", TextProperty, BindingMode.OneWay);
            }
            if(ribbonControl!=null)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(ribbonControl);
                if (parent == null) quickAccessMenuItem.AddLogicalChild(ribbonControl);
            }
            RibbonControl oldRibbonControl = e.OldValue as RibbonControl;
            if(oldRibbonControl!=null)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(oldRibbonControl);
                if (parent == quickAccessMenuItem) quickAccessMenuItem.RemoveLogicalChild(oldRibbonControl);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                DependencyObject parent = LogicalTreeHelper.GetParent(Target);
                if (parent == this)
                {
                    ArrayList list = new ArrayList();
                    list.Add(Target);
                    return list.GetEnumerator();
                }
                return base.LogicalChildren;
            }
        }

        #endregion

        #region Event Handlers

        void OnChecked(object sender, RoutedEventArgs e)
        {
            Ribbon ribbon = FindRibbon();
            if (ribbon != null)
            {
                ribbon.AddToQuickAccessToolbar(Target);
            }
        }

        void OnUnchecked(object sender, RoutedEventArgs e)
        {
            Ribbon ribbon = FindRibbon();
            if (ribbon != null)
            {
                ribbon.RemoveFromQuickAccessToolbar(Target);
            }
        }

        void OnItemLoaded(object sender, RoutedEventArgs e)
        {
            Ribbon ribbon = FindRibbon();
            if (ribbon != null)
            {
                IsChecked = ribbon.IsInQuickAccessToolbar(Target);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds ribbon
        /// </summary>
        /// <returns>Ribbon or null if not finded</returns>
        //QuickAccessToolBar FindQuickAccessToolbar()
        Ribbon FindRibbon()
        {
            UIElement element = this.Parent as UIElement;
            while (element != null)
            {
                Ribbon ribbon = element as Ribbon;
                if (ribbon != null) return ribbon;
                UIElement parent = (UIElement)VisualTreeHelper.GetParent(element as DependencyObject);
                if (parent != null) element = parent;
                else element = (UIElement)LogicalTreeHelper.GetParent(element as DependencyObject);
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// The class responds to mine controls for QuickAccessToolBar
    /// </summary>
    internal static class QuickAccessItemsProvider
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the given control can provide a quick access toolbar item
        /// </summary>
        /// <param name="element">Control</param>
        /// <returns>True if this control is able to provide
        /// a quick access toolbar item, false otherwise</returns>
        public static bool IsSupported(UIElement element)
        {
            IQuickAccessItemProvider provider = (element as IQuickAccessItemProvider);
            if ((provider != null) && (provider.CanAddToQuickAccessToolBar)) return true;
            return false;
        }

        /// <summary>
        /// Gets control which represents quick access toolbar item
        /// </summary>
        /// <param name="element">Host control</param>
        /// <returns>Control which represents quick access toolbar item</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800")]
        public static UIElement GetQuickAccessItem(UIElement element)
        {
            UIElement result = null;

            // If control supports the interface just return what it provides 
            IQuickAccessItemProvider provider = (element as IQuickAccessItemProvider);
            if ((provider != null) && (provider.CanAddToQuickAccessToolBar)) 
                result = ((IQuickAccessItemProvider)element).CreateQuickAccessItem();
            
            // The control isn't supported
            if (result == null) throw new ArgumentException("The contol " + element.GetType().Name + " is not able to provide a quick access toolbar item");
            
            return result;
        }
        
        /// <summary>
        /// Finds the top supported control and gets quick access item from it
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static UIElement PickQuickAccessItem(Visual visual, Point point)
        {
            UIElement element = FindSupportedControl(visual, point);
            if (element != null) return GetQuickAccessItem(element);
            else return null;
        }

        /// <summary>
        /// Finds the top supported control
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static UIElement FindSupportedControl(Visual visual, Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(visual, point);
            if (result == null) return null;
            
            // Try to find in visual (or logical) tree
            UIElement element = result.VisualHit as UIElement;
            while (element != null)
            {
                if(IsSupported(element)) return element;
                UIElement visualParent = VisualTreeHelper.GetParent(element) as UIElement;
                UIElement logicalParent = LogicalTreeHelper.GetParent(element) as UIElement;
                element = visualParent ?? logicalParent;
            }

            return null;
        }

        #endregion
    }
}
