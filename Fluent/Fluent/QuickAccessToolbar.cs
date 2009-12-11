using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    [TemplatePart(Name = "PART_ShowAbove", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_ShowBelow", Type = typeof(MenuItem))]
    [TemplatePart(Name = "PART_MenuPanel", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_RootPanel", Type = typeof(Panel))]
    public class QuickAccessToolbar:ToolBar
    {
        #region Fields

        private MenuItem showAbove = null;
        private MenuItem showBelow = null;

        private Panel menuPanel = null;

        private ObservableCollection<QuickAccessMenuItem> quickAccessItems = null;

        private Panel rootPanel = null;

        #endregion

        #region Properties

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

        private void OnQuickAccessItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Add(obj2 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Remove(obj3 as QuickAccessMenuItem);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Remove(obj4 as QuickAccessMenuItem);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (menuPanel != null) menuPanel.Children.Add(obj5 as QuickAccessMenuItem);
                    }
                    break;
            }

        }

        public bool ShowAboveRibbon
        {
            get { return (bool)GetValue(ShowAboveRibbonProperty); }
            set { SetValue(ShowAboveRibbonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAboveRibbon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowAboveRibbonProperty =
            DependencyProperty.Register("ShowAboveRibbon", typeof(bool), typeof(QuickAccessToolbar), new UIPropertyMetadata(false));

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                ArrayList array = new ArrayList();                                
                foreach (var item in Items)
                {
                    if((item is DependencyObject)&&(!GetIsOverflowItem(item as DependencyObject)))array.Add(item);
                }
                if(HasOverflowItems)array.Add(rootPanel);
                return array.GetEnumerator();
            }
        }

        #endregion

        #region Initialize

        static QuickAccessToolbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolbar), new FrameworkPropertyMetadata(typeof(QuickAccessToolbar)));            
        }        

        public QuickAccessToolbar()
        {

        }

        #endregion

        #region Override

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Parent is Ribbon) (this.Parent as Ribbon).TitleBar.InvalidateMeasure();
            base.OnItemsChanged(e);
            UpdateKeyTips();
        }

        public override void OnApplyTemplate()
        {
            /*if (menuDownButton != null) menuDownButton.PreviewMouseLeftButtonDown -= OnMenuDownButtonClick;
            if (menuDownPopupButton != null) menuDownPopupButton.PreviewMouseLeftButtonDown -= OnMenuDownButtonClick;
            if (toolbarDownButton != null) toolbarDownButton.PreviewMouseLeftButtonDown -= OnToolbatDownButtonClick;
            */
            /*menuDownButton = GetTemplateChild("PART_MenuDownButton") as ToggleButton;
            menuDownPopupButton = GetTemplateChild("PART_MenuDownButtonPopup") as ToggleButton;
            toolbarDownButton = GetTemplateChild("PART_ToolbarDownButton") as ToggleButton;*/
            /*
            if (menuDownButton != null) menuDownButton.PreviewMouseLeftButtonDown += OnMenuDownButtonClick;
            if (menuDownPopupButton != null) menuDownPopupButton.PreviewMouseLeftButtonDown += OnMenuDownButtonClick;
            if (toolbarDownButton != null) toolbarDownButton.PreviewMouseLeftButtonDown += OnToolbatDownButtonClick;
            */
            /*menu = GetTemplateChild("PART_Menu") as Menu;
            menuPopup = GetTemplateChild("PART_MenuPopup") as Popup;
            toolbarPopup = GetTemplateChild("PART_ToolbarPopup") as Popup;*/

            if (showAbove != null) showAbove.Click -= OnShowAboveClick;
            if (showBelow != null) showBelow.Click -= OnShowBelowClick;
            
            showAbove = GetTemplateChild("PART_ShowAbove") as MenuItem;            
            showBelow = GetTemplateChild("PART_ShowBelow") as MenuItem;

            if (showAbove != null) showAbove.Click += OnShowAboveClick;
            if (showBelow != null) showBelow.Click += OnShowBelowClick;

            if (menuPanel != null)
            {
                menuPanel.Children.Clear();
            }
            menuPanel = GetTemplateChild("PART_MenuPanel") as Panel;
            if ((menuPanel != null) && (quickAccessItems != null))
            {
                for (int i = 0; i < quickAccessItems.Count; i++)
                {
                    menuPanel.Children.Add(quickAccessItems[i]);
                }
            }

            if (rootPanel != null) RemoveLogicalChild(rootPanel);
            rootPanel = GetTemplateChild("PART_RootPanel") as Panel;
            if (rootPanel != null)
            {
                AddLogicalChild(rootPanel);
            }
        }

        private void OnShowBelowClick(object sender, RoutedEventArgs e)
        {
            ShowAboveRibbon = false;
        }

        private void OnShowAboveClick(object sender, RoutedEventArgs e)
        {
            ShowAboveRibbon = true;
        }

        #endregion

        #region Methods

        // Updates keys for keytip access
        void UpdateKeyTips()
        {
            for (int i = 0; i < Math.Min(9, Items.Count); i++)
            {
                // 1, 2, 3, ... , 9
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], (i + 1).ToString());
            }
            for (int i = 9; i < Math.Min(18, Items.Count); i++)
            {
                // 09, 08, 07, ... , 01
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], "0" + (18 - i).ToString());
            }
            char startChar = 'A';
            for (int i = 18; i < Math.Min(9 + 9 + 26, Items.Count); i++)
            {
                // 0A, 0B, 0C, ... , 0Z
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], "0" + startChar++);
            }
        }

        #endregion

        #region Event handling

        #endregion
    }
}
