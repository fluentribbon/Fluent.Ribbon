using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace Fluent
{
    [TemplatePart(Name = "PART_ContentContainer", Type = typeof(Border))]
    [ContentProperty("Groups")]
    public class RibbonTabItem:Control
    {
        #region Dependency properties

        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsParentMeasure, new PropertyChangedCallback(RibbonTabItem.OnIsSelectedChanged)));

        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register("IsMinimized", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        #endregion

        #region Fields

        private Border contentContainer = null;
        
        private double desiredWidth = 0;

        private ObservableCollection<RibbonGroupBox> groups = null;

        private RibbonGroupsContainer groupsContainer = new RibbonGroupsContainer();

        #endregion

        #region Properties

        public RibbonGroupsContainer GroupsContainer
        {
            get { return groupsContainer; }
        }

        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public string ReduceOrder
        {
            get { return groupsContainer.ReduceOrder; }
            set { groupsContainer.ReduceOrder = value; }
        }

        #region IsContextual

        /// <summary>
        /// Gets or sets whether tab item is contextual
        /// </summary>
        public bool IsContextual
        {
            get { return (bool)GetValue(IsContextualProperty); }
            set { SetValue(IsContextualProperty, value); }
        }
                       
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsContextual.  
        /// This enables animation, styling, binding, etc...
        /// </summary>  
        public static readonly DependencyProperty IsContextualProperty =
            DependencyProperty.Register("IsContextual", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));

        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                ArrayList array = new ArrayList();
                array.Add(groupsContainer);
                return array.GetEnumerator();
            }
        }

        #endregion


        [Bindable(true), Category("Appearance")]
        public bool IsSelected
        {
            get
            {                
                return (bool)base.GetValue(IsSelectedProperty);
            }
            set
            {
                base.SetValue(IsSelectedProperty, value);
            }
        }

        internal RibbonTabControl TabControlParent
        {
            get
            {
                return (ItemsControl.ItemsControlFromItemContainer(this) as RibbonTabControl);
            }
        }



        public double Whitespace
        {
            get { return (double)GetValue(WhitespaceProperty); }
            set { SetValue(WhitespaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WhitespaceProperty =
            DependencyProperty.Register("Whitespace", typeof(double), typeof(RibbonTabItem), new UIPropertyMetadata((double)12.0));



        public bool IsSeparatorVisible
        {
            get { return (bool)GetValue(IsSeparatorVisibleProperty); }
            set { SetValue(IsSeparatorVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSeparatorVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSeparatorVisibleProperty =
            DependencyProperty.Register("IsSeparatorVisible", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));


        public RibbonContextualTabGroup Group
        {
            get { return (RibbonContextualTabGroup)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Group.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("Group", typeof(RibbonContextualTabGroup), typeof(RibbonTabItem), new UIPropertyMetadata(null, OnGroupChanged));

        private static void OnGroupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabItem tab = d as RibbonTabItem;
            if (e.OldValue != null) (e.OldValue as RibbonContextualTabGroup).RemoveTabItem(tab);
            if (e.NewValue != null) (e.NewValue as RibbonContextualTabGroup).AppendTabItem(tab);
        }

        /// <summary>
        /// Gets or sets desired width of the tab item
        /// </summary>
        internal double DesiredWidth
        {
            get { return desiredWidth; }
            set { desiredWidth = value; InvalidateMeasure(); }
        }



        public bool HasLeftGroupBorder
        {
            get { return (bool)GetValue(HasLeftGroupBorderProperty); }
            set { SetValue(HasLeftGroupBorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HaseLeftGroupBorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasLeftGroupBorderProperty =
            DependencyProperty.Register("HasLeftGroupBorder", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));


        public bool HasRightGroupBorder
        {
            get { return (bool)GetValue(HasRightGroupBorderProperty); }
            set { SetValue(HasRightGroupBorderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HaseLeftGroupBorder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasRightGroupBorderProperty =
            DependencyProperty.Register("HasRightGroupBorder", typeof(bool), typeof(RibbonTabItem), new UIPropertyMetadata(false));


        public ObservableCollection<RibbonGroupBox> Groups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new ObservableCollection<RibbonGroupBox>();
                    this.groups.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnGroupsCollectionChanged);
                }
                return this.groups;
            }
        }

        private void OnGroupsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object obj2 in e.NewItems)
                    {
                        if (groupsContainer != null) groupsContainer.Children.Add(obj2 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object obj3 in e.OldItems)
                    {
                        if (groupsContainer != null) groupsContainer.Children.Remove(obj3 as UIElement);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object obj4 in e.OldItems)
                    {
                        if (groupsContainer != null) groupsContainer.Children.Remove(obj4 as UIElement);
                    }
                    foreach (object obj5 in e.NewItems)
                    {
                        if (groupsContainer != null) groupsContainer.Children.Add(obj5 as UIElement);
                    }
                    break;
            }

        }



        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(RibbonTabItem), new UIPropertyMetadata(null));

        #region Focusable

        /// <summary>
        /// Handles IsEnabled changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">The event data.</param>
        private static void OnFocusableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces IsEnabled 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="basevalue"></param>
        /// <returns></returns>
        private static object CoerceFocusable(DependencyObject d, object basevalue)
        {
            if (d is RibbonTabItem)
            {
                RibbonTabItem control = (d as RibbonTabItem);
                Ribbon ribbon = control.FindParentRibbon();
                if (ribbon != null)
                {
                    return ((bool)basevalue) && ribbon.Focusable;
                }
            }
            return basevalue;
        }

        private Ribbon FindParentRibbon()
        {
            DependencyObject element = this.Parent;
            while (element != null)
            {
                if (element is Ribbon) return element as Ribbon;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        #endregion 

        #endregion

        #region Events

        public event RoutedEventHandler TabChanged;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTabItem), new FrameworkPropertyMetadata(typeof(RibbonTabItem)));
            FocusableProperty.AddOwner(typeof(RibbonTabItem), new FrameworkPropertyMetadata(OnFocusableChanged, CoerceFocusable));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonTabItem()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (TabControlParent != null) if (TabControlParent.SelectedItem is RibbonTabItem)
                    (TabControlParent.SelectedItem as RibbonTabItem).IsSelected = false;
            IsSelected = true;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            if(Group!=null) (Group.Parent as RibbonTitleBar).InvalidateMeasure();
        }

        #endregion

        #region Overrides

        protected override Size MeasureOverride(Size constraint)
        {
            if (contentContainer == null) return base.MeasureOverride(constraint);
            contentContainer.Padding = new Thickness(Whitespace, contentContainer.Padding.Top, Whitespace, contentContainer.Padding.Bottom);
            Size baseConstraint = base.MeasureOverride(constraint);            
            double totalWidth = contentContainer.DesiredSize.Width - contentContainer.Margin.Left - contentContainer.Margin.Right;
            (contentContainer.Child).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            double headerWidth = contentContainer.Child.DesiredSize.Width;
            if (totalWidth < headerWidth + Whitespace * 2)
            {
                double newPaddings = Math.Max(0, (totalWidth - headerWidth) / 2);
                contentContainer.Padding = new Thickness(newPaddings, contentContainer.Padding.Top, newPaddings, contentContainer.Padding.Bottom);
            }
            else
            {
                if (desiredWidth != 0)
                {
                    if (constraint.Width > desiredWidth) baseConstraint.Width = desiredWidth;
                    else
                        baseConstraint.Width = headerWidth + Whitespace*2 + contentContainer.Margin.Left +
                                               contentContainer.Margin.Right;
                }
            }
            return baseConstraint;
        }

        /// <summary>
        /// On new style applying
        /// </summary>
        public override void OnApplyTemplate()
        {
            contentContainer = GetTemplateChild("PART_ContentContainer") as Border;
        }

        /*protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.IsSelected)
            {
                RibbonTabControl tabControlParent = this.TabControlParent;
                if (tabControlParent != null)
                {
                    tabControlParent.SelectedContent = newContent;
                }
            }
        }*/

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (((e.Source == this) &&(e.ClickCount==2)))
            {
                e.Handled = true;
                if (TabControlParent != null) TabControlParent.IsMinimized = !TabControlParent.IsMinimized;
            }
            else if (((e.Source == this) || !this.IsSelected))
            {
                if (TabControlParent!=null) if (TabControlParent.SelectedItem is RibbonTabItem)
                    (TabControlParent.SelectedItem as RibbonTabItem).IsSelected = false;
                e.Handled = true;
                this.IsSelected = true;
            }            
            //base.OnMouseLeftButtonDown(e);
        }


        #endregion

        #region Private methods

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonTabItem container = d as RibbonTabItem;
            bool newValue = (bool)e.NewValue;
            RibbonTabControl tabControlParent = container.TabControlParent;

            if (newValue)
            {
                container.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, container));
            }
            else
            {
                container.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, container));
            }
            
        }

        protected virtual void OnSelected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(true, e);
        }

        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            this.HandleIsSelectedChanged(false, e);
        }

        #endregion

        #region Event handling

        private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
        {
            base.RaiseEvent(e);
        }

        #endregion
    }
}
