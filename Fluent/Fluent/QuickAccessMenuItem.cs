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
using System.Windows.Controls;

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
        FrameworkElement CreateQuickAccessItem();

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
        #region Fields

        internal Ribbon Ribbon;

        #endregion

        #region Initialization

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static QuickAccessMenuItem()
        {
            IsCheckableProperty.AddOwner(typeof(QuickAccessMenuItem), new FrameworkPropertyMetadata(true));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public QuickAccessMenuItem()
        {
            Checked += OnChecked;
            Unchecked += OnUnchecked;
            Loaded += OnFirstLoaded;
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
            IRibbonControl ribbonControl = e.NewValue as IRibbonControl;
            if ((quickAccessMenuItem.Header==null) && (ribbonControl != null))
            {
                // Set Default Text Value
                RibbonControl.Bind(ribbonControl, quickAccessMenuItem, "Header", HeaderProperty, BindingMode.OneWay);
            }
            if(ribbonControl!=null)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent((DependencyObject)ribbonControl);
                if (parent == null) quickAccessMenuItem.AddLogicalChild(ribbonControl);
            }
            IRibbonControl oldRibbonControl = e.OldValue as IRibbonControl;
            if(oldRibbonControl!=null)
            {
                DependencyObject parent = LogicalTreeHelper.GetParent((DependencyObject)oldRibbonControl);
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
                if (Target != null)
                {
                    DependencyObject parent = LogicalTreeHelper.GetParent(Target);
                    if (parent == this)
                    {
                        ArrayList list = new ArrayList();
                        list.Add(Target);
                        return list.GetEnumerator();
                    }
                }
                return base.LogicalChildren;
            }
        }

        #endregion

        #region Event Handlers

        void OnChecked(object sender, RoutedEventArgs e)
        {
            if (Ribbon != null)
            {
                Ribbon.AddToQuickAccessToolBar(Target);
            }
        }

        void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if(!IsLoaded) return;
            if (Ribbon != null)
            {
                Ribbon.RemoveFromQuickAccessToolBar(Target);
            }
        }

        void OnItemLoaded(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded) return;
            if (Ribbon != null)
            {
                IsChecked = Ribbon.IsInQuickAccessToolBar(Target);
            }
        }


        private void OnFirstLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnFirstLoaded;
            if ((IsChecked)&&(Ribbon != null))
            {
                Ribbon.AddToQuickAccessToolBar(Target);
            }
        }

        #endregion

        #region Private Methods

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
        public static FrameworkElement GetQuickAccessItem(UIElement element)
        {
            FrameworkElement result = null;

            // If control supports the interface just return what it provides 
            IQuickAccessItemProvider provider = (element as IQuickAccessItemProvider);
            if ((provider != null) && (provider.CanAddToQuickAccessToolBar)) 
                result = ((IQuickAccessItemProvider)element).CreateQuickAccessItem();
            
            // The control isn't supported
            if (result == null) throw new ArgumentException("The contol " + element.GetType().Name + " is not able to provide a quick access toolbar item");

            return result;
        }

        /// <summary>
        /// Finds the top supported control
        /// </summary>
        /// <param name="visual">Visual</param>
        /// <param name="point">Point</param>
        /// <returns>Point</returns>
        public static FrameworkElement FindSupportedControl(Visual visual, Point point)
        {
            HitTestResult result = VisualTreeHelper.HitTest(visual, point);
            if (result == null) return null;
            
            // Try to find in visual (or logical) tree
            FrameworkElement element = result.VisualHit as FrameworkElement;
            while (element != null)
            {
                if(IsSupported(element)) return element;
                FrameworkElement visualParent = VisualTreeHelper.GetParent(element) as FrameworkElement;
                FrameworkElement logicalParent = LogicalTreeHelper.GetParent(element) as FrameworkElement;
                element = visualParent ?? logicalParent;
            }

            return null;
        }

        #endregion
    }
}
