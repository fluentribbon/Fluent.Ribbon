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
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents states of ribbon group 
    /// </summary>
    public enum RibbonGroupBoxState
    {
        /// <summary>
        /// Large. All controls in the group will try to be large size
        /// </summary>
        Large = 0,
        /// <summary>
        /// Middle. All controls in the group will try to be middle size
        /// </summary>
        Middle,
        /// <summary>
        /// Small. All controls in the group will try to be small size
        /// </summary>
        Small,
        /// <summary>
        /// Collapsed. Group will collapse its content in a single button
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// RibbonGroup represents a logical group of controls as they appear on
    /// a RibbonTab.  These groups can resize its content
    /// </summary>
    [TemplatePart(Name = "PART_DialogLauncherButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_DownGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_UpPanel", Type = typeof(Panel))]
    public class RibbonGroupBox :ItemsControl
    {
        #region Fields

        // Dialog launcher button
        private Button dialogLauncherButton;

        // Dropdown poup
        private Popup popup;

        // Down part
        private Grid downGrid;
        // up part
        private Panel upPanel;

        #endregion

        #region Properties

        #region State

        /// <summary>
        /// Gets or sets the current state of the group
        /// </summary>
        public RibbonGroupBoxState State
        {
            get { return (RibbonGroupBoxState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for State.  
        /// This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(RibbonGroupBoxState), typeof(RibbonGroupBox), new UIPropertyMetadata(RibbonGroupBoxState.Large, StatePropertyChanged));

        /// <summary>
        /// On state property changed
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox ribbonGroupBox = (RibbonGroupBox)d;
            RibbonGroupBoxState ribbonGroupBoxState = (RibbonGroupBoxState)e.NewValue;

            SetChildSizes(ribbonGroupBoxState, ribbonGroupBox);

        }
        
        // Set child sizes
        private static void SetChildSizes(RibbonGroupBoxState ribbonGroupBoxState, RibbonGroupBox ribbonGroupBox)
        {                            
            for (int i = 0; i < ribbonGroupBox.Items.Count; i++)
            {
                SetAppropriateSizeRecursive((UIElement)ribbonGroupBox.Items[i], ribbonGroupBoxState);
                //RibbonControl.SetAppropriateSize((UIElement)ribbonGroupBox.Items[i], ribbonGroupBoxState);
            }
        }

        static void SetAppropriateSizeRecursive(UIElement root, RibbonGroupBoxState ribbonGroupBoxState)
        {
            if (root == null) return;
            if (root is RibbonControl) RibbonControl.SetAppropriateSize(root, ribbonGroupBoxState);
            if (root is RibbonToolBarTray) ((RibbonToolBarTray)root).IsCondensed = !(ribbonGroupBoxState == RibbonGroupBoxState.Collapsed || ribbonGroupBoxState == RibbonGroupBoxState.Large);

            int childrenCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < childrenCount; i++)
            {
                SetAppropriateSizeRecursive(VisualTreeHelper.GetChild(root,i) as UIElement, ribbonGroupBoxState);
            }
        }

        #endregion

        /// <summary>
        /// Gets or sets group box header
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RibbonGroupBox), new UIPropertyMetadata("RibbonGroupBox"));

        /// <summary>
        /// Gets or sets dialog launcher button visibility
        /// </summary>
        public bool IsDialogLauncherButtonVisible
        {
            get { return (bool)GetValue(IsDialogLauncherButtonVisibleProperty); }
            set { SetValue(IsDialogLauncherButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDialogLauncherButtonVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDialogLauncherButtonVisibleProperty =
            DependencyProperty.Register("IsDialogLauncherButtonVisible", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets drop down popup visibility
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(false, OnIsOpenChanged));

        /// <summary>
        /// Gets or sets image for group box
        /// </summary>
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonGroupBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets an enumerator for the logical child objects of the System.Windows.Controls.ItemsControl object.
        /// </summary>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                ArrayList array = new ArrayList();
                array.AddRange(Items);
                return array.GetEnumerator();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Dialog launcher btton click event
        /// </summary>
        public event RoutedEventHandler DialogLauncherButtonClick;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonGroupBox()
        {
            StyleProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(typeof(RibbonGroupBox)));
        }

        // Coerce control style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null) basevalue = ThemesManager.DefaultRibbonGroupBoxStyle;
            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupBox()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
        }

        /// <summary>
        /// Click event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (State == RibbonGroupBoxState.Collapsed)
            {
                IsOpen = true;
                e.Handled = true;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a panel with items
        /// </summary>
        /// <returns></returns>
        internal Panel GetPanel() { return upPanel; }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when the System.Windows.Controls.ItemsControl.Items property changes.
        /// </summary>
        /// <param name="e">Information about the change.</param>
        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Visual visual in e.NewItems)
                {
                    RibbonControl.SetAppropriateSize((UIElement) visual, State);
                }
            }
            base.OnItemsChanged(e);
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (dialogLauncherButton != null) dialogLauncherButton.Click -= OnDialogLauncherButtonClick;
            dialogLauncherButton = GetTemplateChild("PART_DialogLauncherButton") as Button;
            if (dialogLauncherButton != null) dialogLauncherButton.Click += OnDialogLauncherButtonClick;
            
            popup = GetTemplateChild("PART_Popup") as Popup;
            if(popup!=null)
            {
                Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                popup.SetBinding(Popup.IsOpenProperty, binding);
            }

            downGrid = GetTemplateChild("PART_DownGrid") as Grid;
            upPanel = GetTemplateChild("PART_UpPanel") as Panel;
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed 
        /// event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if ((State == RibbonGroupBoxState.Collapsed)&&(popup!=null)&&(!IsOpen))
            {
                e.Handled = true;
                Mouse.Capture(popup, CaptureMode.Element);
                RaiseEvent(new RoutedEventArgs(RibbonControl.ClickEvent,this));                
            }
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if ((upPanel == null) || (downGrid == null)) return base.MeasureOverride(constraint);
            upPanel.Measure(new Size(double.PositiveInfinity, constraint.Height));
            double width = upPanel.DesiredSize.Width +upPanel.Margin.Left + upPanel.Margin.Right;
            Size size = new Size(width,constraint.Height);
            (upPanel.Parent as Grid).Measure(size);
            return new Size(width, (upPanel.Parent as Grid).DesiredSize.Height);
        }

        #endregion

        #region Event handling

        /// <summary>
        /// Dialog launcher button click handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">the event data</param>
        private void OnDialogLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            if (DialogLauncherButtonClick != null) DialogLauncherButtonClick(this, e);
        }
        
        // Handles popup closing
        private void OnRibbonGroupBoxPopupClosing()
        {
            IsHitTestVisible = true;
        }

        // handles popup opening
        private void OnRibbonGroupBoxPopupOpening()
        {
            IsHitTestVisible = false;
        }
        
        /// <summary>
        /// handles IsOpen propertyu changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox ribbon = (RibbonGroupBox)d;

            if (ribbon.IsOpen)
            {
                ribbon.OnRibbonGroupBoxPopupOpening();
            }
            else
            {
                ribbon.OnRibbonGroupBoxPopupClosing();
            }
        }

        #endregion
    }
}
