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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents quick access toolbar
    /// </summary>
    [TemplatePart(Name = "PART_ShowAbove", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_ShowBelow", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_RootPanel", Type = typeof(Panel))]
    public class QuickAccessToolBar:ToolBar
    {
        #region Events

        /// <summary>
        /// Occured when items are added or removed from Quick Access toolbar
        /// </summary>
        public event NotifyCollectionChangedEventHandler ItemsChanged;

        private DropDownButton menuButton;

        #endregion

        #region Fields

        // Show above menu item
        private MenuItem showAbove;
        // Show below menu item
        private MenuItem showBelow;
        // Menu panel
        private Panel menuPanel;
        // Items of quick access menu
        private ObservableCollection<QuickAccessMenuItem> quickAccessItems;
        // Root panel of control
        private Panel rootPanel;
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets quick access menu items
        /// </summary>
        public ObservableCollection<QuickAccessMenuItem> QuickAccessItems
        {
            get
            {
                if (this.quickAccessItems == null)
                {
                    this.quickAccessItems = new ObservableCollection<QuickAccessMenuItem>();
                    this.quickAccessItems.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnQuickAccessItemsCollectionChanged);
                }
                return this.quickAccessItems;
            }
        }
        /// <summary>
        /// Handles quick access menu items chages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Add(obj2 as QuickAccessMenuItem);
                        else AddLogicalChild(obj2);     
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Remove(obj3 as QuickAccessMenuItem);
                        else RemoveLogicalChild(obj3);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Remove(obj4 as QuickAccessMenuItem);
                        else RemoveLogicalChild(obj4);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Add(obj5 as QuickAccessMenuItem);
                        else AddLogicalChild(obj5);
                    }
                    break;
            }

        }

        /// <summary>
        /// Gets or sets whether quick access toolbar showes above ribbon
        /// </summary>
        public bool ShowAboveRibbon
        {
            get { return (bool)GetValue(ShowAboveRibbonProperty); }
            set { SetValue(ShowAboveRibbonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAboveRibbon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAboveRibbonProperty =
            DependencyProperty.Register("ShowAboveRibbon", typeof(bool), typeof(QuickAccessToolBar), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets an enumerator to the logical child elements of the System.Windows.Controls.HeaderedItemsControl.
        /// </summary>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                ArrayList array = new ArrayList();                                
                
                //if (menuButton!=null) array.Add(menuButton);
                foreach(var item in QuickAccessItems)
                {
                    array.Add(item);
                }
                
                foreach (var item in Items)
                {
                    DependencyObject dependencyObject = item as DependencyObject;
                    if ((dependencyObject != null) && (!GetIsOverflowItem(dependencyObject))) array.Add(item);
                }
                if (HasOverflowItems) array.Add(rootPanel);
                return array.GetEnumerator();
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static QuickAccessToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolBar), new FrameworkPropertyMetadata(typeof(QuickAccessToolBar)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public QuickAccessToolBar()
        {
         
        }

        #endregion

        #region Override

        /// <summary>
        /// Called when the System.Windows.Controls.ItemsControl.Items property changes.
        /// </summary>
        /// <param name="e">The arguments for the System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged 
        /// event.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Parent is Ribbon) (this.Parent as Ribbon).TitleBar.InvalidateMeasure();
            base.OnItemsChanged(e);
            UpdateKeyTips();

            // Raise items changed event
            if (ItemsChanged != null) ItemsChanged(this, e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or 
        /// internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (showAbove != null) showAbove.Click -= OnShowAboveClick;
            if (showBelow != null) showBelow.Click -= OnShowBelowClick;
            
            showAbove = GetTemplateChild("PART_ShowAbove") as MenuItem;            
            showBelow = GetTemplateChild("PART_ShowBelow") as MenuItem;

            if (showAbove != null) showAbove.Click += OnShowAboveClick;
            if (showBelow != null) showBelow.Click += OnShowBelowClick;

            if (menuPanel != null)
            {
                for (int i = 0; i < QuickAccessItems.Count; i++)
                {
                    menuPanel.Children.Remove(QuickAccessItems[i]);
                    QuickAccessItems[i].InvalidateProperty(QuickAccessMenuItem.TargetProperty);
                }
            }
            else if(quickAccessItems!=null)
            {
                //Ribbon ribbon = FindRibbon();
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    RemoveLogicalChild(quickAccessItems[i]);
                    //if ((quickAccessItems[i].IsChecked) && (ribbon != null)) ribbon.AddToQuickAccessToolbar(quickAccessItems[i].Target);
                }
            }
            menuPanel = GetTemplateChild("PART_MenuPanel") as Panel;
            if ((menuPanel != null) && (quickAccessItems != null))
            {
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    menuPanel.Children.Add(quickAccessItems[i]);
                    quickAccessItems[i].InvalidateProperty(QuickAccessMenuItem.TargetProperty);
                }
            }
            

            if (rootPanel != null) RemoveLogicalChild(rootPanel);
            rootPanel = GetTemplateChild("PART_RootPanel") as Panel;
            if (rootPanel != null)
            {
                AddLogicalChild(rootPanel);
            }

            menuButton = GetTemplateChild("PART_MenuDownButton") as DropDownButton;
        }

        /// <summary>
        /// Handles show below menu item click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnShowBelowClick(object sender, RoutedEventArgs e)
        {
            ShowAboveRibbon = false;
        }

        /// <summary>
        /// Handles show above menu item click
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnShowAboveClick(object sender, RoutedEventArgs e)
        {
            ShowAboveRibbon = true;
        }

        /// <summary>
        /// Finds ribbon
        /// </summary>
        /// <returns>Ribbon or null if not finded</returns>
        //QuickAccessToolBar FindQuickAccessToolbar()
        Ribbon FindRibbon()
        {
            UIElement element = VisualTreeHelper.GetParent(this) as UIElement;
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

        #region Methods

        // Updates keys for keytip access
        void UpdateKeyTips()
        {
            for (int i = 0; i < Math.Min(9, Items.Count); i++)
            {
                // 1, 2, 3, ... , 9
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], (i + 1).ToString(CultureInfo.InvariantCulture));
            }
            for (int i = 9; i < Math.Min(18, Items.Count); i++)
            {
                // 09, 08, 07, ... , 01
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], "0" + (18 - i).ToString(CultureInfo.InvariantCulture));
            }
            char startChar = 'A';
            for (int i = 18; i < Math.Min(9 + 9 + 26, Items.Count); i++)
            {
                // 0A, 0B, 0C, ... , 0Z
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], "0" + startChar++);
            }
        }

        #endregion

    }
}
