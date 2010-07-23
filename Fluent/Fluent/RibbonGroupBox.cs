#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
    public class RibbonGroupBox : ItemsControl, IQuickAccessItemProvider, IDropDownControl
    {
        #region Fields

        // Dropdown poup
        private Popup popup;

        // Down part
        private Grid downGrid;
        // up part
        private Panel upPanel;

        private Panel parentPanel;

        // Freezed image (created during snapping)
        Image snappedImage;
        // Is visual currently snapped
        bool isSnapped;

        // Saved group state for QAT
        RibbonGroupBoxState savedState;

        private Popup quickAccessPopup;

        // Saved scale for Collapsing
        private int savedScale;

        #endregion

        #region Properties

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup
        {
            get { return popup; }
        }

        /// <summary>
        /// Gets a value indicating whether context menu is opened
        /// </summary>
        public bool IsContextMenuOpened { get; set; }

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
            if (root is IRibbonControl)
            {
                RibbonControl.SetAppropriateSize(root, ribbonGroupBoxState);
                return;
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < childrenCount; i++)
            {
                SetAppropriateSizeRecursive(VisualTreeHelper.GetChild(root, i) as UIElement, ribbonGroupBoxState);
            }
        }

        #endregion

        #region Scale

        // Current scale index
        int scale;

        /// <summary>
        /// Gets or sets scale index (for internal IRibbonScalableControl)
        /// </summary>
        internal int Scale
        {
            get { return scale; }
            set
            {
                int difference = value - scale;
                scale = value;

                for (int i = 0; i < Math.Abs(difference); i++)
                {
                    if (difference > 0) IncreaseScalableElement();
                    else DecreaseScalableElement();
                }
            }
        }

        // Finds and increase size of all scalable elements in the given group box
        void IncreaseScalableElement()
        {
            foreach (object item in Items)
            {
                IScalableRibbonControl scalableRibbonControl = item as IScalableRibbonControl;
                if (scalableRibbonControl == null) continue;
                scalableRibbonControl.Enlarge();
            }
        }

        void OnScalableControlScaled(object sender, EventArgs e)
        {
            if (!SuppressCacheReseting) cachedMeasures.Clear();
        }

        /// <summary>
        /// Gets or sets whether to reset cache when scalable control is scaled
        /// </summary>
        internal bool SuppressCacheReseting
        {
            get;
            set;
        }

        // Finds and decrease size of all scalable elements in the given group box
        void DecreaseScalableElement()
        {
            foreach (object item in Items)
            {
                IScalableRibbonControl scalableRibbonControl = item as IScalableRibbonControl;
                if (scalableRibbonControl == null) continue;
                scalableRibbonControl.Reduce();
            }
        }

        void UpdateScalableControlSubscribing()
        {
            foreach (object item in Items)
            {
                IScalableRibbonControl scalableRibbonControl = item as IScalableRibbonControl;
                if (scalableRibbonControl == null) continue;
                scalableRibbonControl.Scaled -= OnScalableControlScaled;
                scalableRibbonControl.Scaled += OnScalableControlScaled;
            }
        }

        #endregion

        #region Header

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

        #endregion

        #region IsLauncherVisible

        /// <summary>
        /// Gets or sets dialog launcher button visibility
        /// </summary>
        public bool IsLauncherVisible
        {
            get { return (bool)GetValue(IsLauncherVisibleProperty); }
            set { SetValue(IsLauncherVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLauncherVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLauncherVisibleProperty =
            DependencyProperty.Register("IsLauncherVisible", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(false));

        #endregion

        #region LauncherKeys

        /// <summary>
        /// Gets or sets key tip for dialog launcher button
        /// </summary>
        [System.ComponentModel.DisplayName("DialogLauncher Keys"),
        System.ComponentModel.Category("KeyTips"),
        System.ComponentModel.Description("Key tip keys for dialog launcher button")]
        public string LauncherKeys
        {
            get { return (string)GetValue(DialogLauncherButtonKeyTipKeysProperty); }
            set { SetValue(DialogLauncherButtonKeyTipKeysProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for 
        /// LauncherKeys.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DialogLauncherButtonKeyTipKeysProperty =
            DependencyProperty.Register("LauncherKeys",
            typeof(string), typeof(RibbonGroupBox), new UIPropertyMetadata(null, OnDialogLauncherButtonKeyTipKeysChanged));

        static void OnDialogLauncherButtonKeyTipKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox ribbonGroupBox = (RibbonGroupBox)d;
            if (ribbonGroupBox.LauncherButton != null)
            {
                KeyTip.SetKeys(ribbonGroupBox.LauncherButton, (string)e.NewValue);
            }
        }

        #endregion

        #region LauncherIcon

        /// <summary>
        /// Gets or sets launcher button icon
        /// </summary>
        public ImageSource LauncherIcon
        {
            get { return (ImageSource)GetValue(LauncherIconProperty); }
            set { SetValue(LauncherIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherIconProperty =
            DependencyProperty.Register("LauncherIcon", typeof(ImageSource), typeof(RibbonGroupBox), new UIPropertyMetadata(null));

        #endregion

        #region LauncherIcon

        /// <summary>
        /// Gets or sets launcher button text
        /// </summary>
        public string LauncherText
        {
            get { return (string)GetValue(LauncherTextProperty); }
            set { SetValue(LauncherTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherTextProperty =
            DependencyProperty.Register("LauncherText", typeof(string), typeof(RibbonGroupBox), new UIPropertyMetadata(null));

        #endregion

        #region LauncherCommand

        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed. This is a dependency property.
        /// </summary>
        [Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand LauncherCommand
        {
            get
            {
                return (ICommand)GetValue(LauncherCommandProperty);
            }
            set
            {
                SetValue(LauncherCommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the System.Windows.Controls.Primitives.ButtonBase.Command property. This is a dependency property.
        /// </summary>
        [Bindable(true), Localizability(LocalizationCategory.NeverLocalize), Category("Action")]
        public object LauncherCommandParameter
        {
            get
            {
                return GetValue(LauncherCommandParameterProperty);
            }
            set
            {
                SetValue(LauncherCommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the element on which to raise the specified command. This is a dependency property.
        /// </summary>
        [Bindable(true), Category("Action")]
        public IInputElement LauncherCommandTarget
        {
            get
            {
                return (IInputElement)GetValue(LauncherCommandTargetProperty);
            }
            set
            {
                SetValue(LauncherCommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Identifies the System.Windows.Controls.Primitives.ButtonBase.CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherCommandParameterProperty = DependencyProperty.Register("LauncherCommandParameter", typeof(object), typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the routed System.Windows.Controls.Primitives.ButtonBase.Command dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherCommandProperty = DependencyProperty.Register("LauncherCommand", typeof(ICommand), typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the System.Windows.Controls.Primitives.ButtonBase.CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty LauncherCommandTargetProperty = DependencyProperty.Register("LauncherCommandTarget", typeof(IInputElement), typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null));

        #endregion

        #region LauncherToolTip

        /// <summary>
        /// Gets or sets launcher button tooltip
        /// </summary>
        public object LauncherToolTip
        {
            get { return (object)GetValue(LauncherToolTipProperty); }
            set { SetValue(LauncherToolTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherToolTipProperty =
            DependencyProperty.Register("LauncherToolTip", typeof(object), typeof(RibbonGroupBox), new UIPropertyMetadata(null));



        #endregion

        #region IsLauncherEnabled

        /// <summary>
        /// Gets or sets whether launcher button is enabled
        /// </summary>
        public bool IsLauncherEnabled
        {
            get { return (bool)GetValue(IsLauncherEnabledProperty); }
            set { SetValue(IsLauncherEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsLauncherEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLauncherEnabledProperty =
            DependencyProperty.Register("IsLauncherEnabled", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(true));

        #endregion

        #region LauncherButton

        /// <summary>
        /// Gets launcher button
        /// </summary>
        public Button LauncherButton
        {
            get { return (Button)GetValue(LauncherButtonProperty); }
            private set { SetValue(LauncherButtonPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey LauncherButtonPropertyKey =
            DependencyProperty.RegisterReadOnly("LauncherButton", typeof(Button), typeof(RibbonGroupBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherButton.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherButtonProperty = LauncherButtonPropertyKey.DependencyProperty;

        #endregion

        #region IsOpen

        /// <summary>
        /// Gets or sets drop down popup visibility
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsOpen.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(false, OnIsOpenChanged, CoerceIsDropDownOpen));

        private static object CoerceIsDropDownOpen(DependencyObject d, object basevalue)
        {
            RibbonGroupBox box = d as RibbonGroupBox;
            if (box.State != RibbonGroupBoxState.Collapsed) return false;
            return basevalue;
        }

        #endregion

        #region LogicalChildren

        /// <summary>
        /// Gets an enumerator for the logical child objects of 
        /// the System.Windows.Controls.ItemsControl object.
        /// </summary>
        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                ArrayList array = new ArrayList();
                //if (parentPanel != null) array.Add(parentPanel);

                array.AddRange(Items);
                if (LauncherButton != null) array.Add(LauncherButton);
                return array.GetEnumerator();
            }
        }

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets icon
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(RibbonGroupBox), new UIPropertyMetadata(null));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Dialog launcher btton click event
        /// </summary>
        public event RoutedEventHandler LauncherClick;

        #endregion

        #region Initialize

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(typeof(RibbonGroupBox)));
            VisibilityProperty.AddOwner(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(OnVisibilityChanged));
            ContextMenuProperty.AddOwner(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null, OnContextMenuChanged, CoerceContextMenu));

            PopupService.Attach(typeof(RibbonGroupBox));
        }

        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ContextMenuProperty);
        }

        private static object CoerceContextMenu(DependencyObject d, object basevalue)
        {
            if (basevalue == null) return Ribbon.RibbonContextMenu;
            return basevalue;
        }

        // Handles visibility changed
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox box = (d as RibbonGroupBox);
            box.ClearCache();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupBox()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
            ToolTip = new ToolTip();
            (ToolTip as ToolTip).Template = null;
            CoerceValue(ContextMenuProperty);
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
                IsDropDownOpen = true;
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

        /// <summary>
        /// Gets cmmon layout root for popup and groupbox
        /// </summary>
        /// <returns></returns>
        internal Panel GetLayoutRoot() { return parentPanel; }

        #endregion

        #region Snapping

        /// <summary>
        /// Snaps / Unsnaps the Visual 
        /// (remove visuals and substitute with freezed image)
        /// </summary>
        public bool IsSnapped
        {
            get
            {
                return isSnapped;
            }
            set
            {
                if (value == isSnapped) return;
                if (value)
                {
                    if (IsVisible)
                    {
                        // Render the freezed image
                        snappedImage = new Image();
                        RenderOptions.SetBitmapScalingMode(snappedImage, BitmapScalingMode.NearestNeighbor);
                        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ActualWidth,
                                                                                       (int)ActualHeight, 96, 96,
                                                                                       PixelFormats.Pbgra32);
                        renderTargetBitmap.Render((Visual)VisualTreeHelper.GetChild(this, 0));
                        snappedImage.FlowDirection = FlowDirection;
                        snappedImage.Source = renderTargetBitmap;

                        // Attach freezed image
                        AddVisualChild(snappedImage);
                        isSnapped = value;
                    }
                }
                else if (snappedImage != null)
                {
                    // Clean up
                    RemoveVisualChild(snappedImage);
                    snappedImage = null;
                    isSnapped = value;
                }


                InvalidateVisual();

            }
        }

        /// <summary>
        /// Gets visual children count
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                if (isSnapped && IsVisible) return 1;
                return base.VisualChildrenCount;
            }
        }

        /// <summary>
        /// Returns a child at the specified index from a collection of child elements
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection</param>
        /// <returns>The requested child element</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (isSnapped && IsVisible) return snappedImage;
            return base.GetVisualChild(index);
        }

        #endregion

        #region Caching

        // Pair of chached states
        struct StateScale
        {
            public RibbonGroupBoxState State;
            public int Scale;
        }

        // Cache
        readonly Dictionary<StateScale, Size> cachedMeasures = new Dictionary<StateScale, Size>();

        /// <summary>
        /// Gets or sets intermediate state of the group box
        /// </summary>
        internal RibbonGroupBoxState StateIntermediate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets intermediate scale of the group box
        /// </summary>
        internal int ScaleIntermediate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets intermediate desired size
        /// </summary>
        internal Size DesiredSizeIntermediate
        {
            get
            {
                Size result;
                StateScale stateScale = new StateScale { Scale = ScaleIntermediate, State = StateIntermediate };
                if (!cachedMeasures.TryGetValue(stateScale, out result))
                {
                    SuppressCacheReseting = true;
                    UpdateScalableControlSubscribing();

                    // Get desired size for these values
                    RibbonGroupBoxState backupState = State;
                    int backupScale = Scale;
                    State = StateIntermediate;
                    Scale = ScaleIntermediate;
                    InvalidateLayout();
                    Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    cachedMeasures.Add(stateScale, DesiredSize);
                    result = DesiredSize;

                    // Rollback changes
                    State = backupState;
                    Scale = backupScale;
                    InvalidateLayout();
                    Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                    SuppressCacheReseting = false;
                }
                return result;
            }
        }

        /// <summary>
        /// Clears cache
        /// </summary>
        public void ClearCache()
        {
            cachedMeasures.Clear();
        }

        /// <summary>
        /// Invalidates layout (with children)
        /// </summary>
        internal void InvalidateLayout()
        {
            InvalidateMeasureRecursive(this);
        }

        static void InvalidateMeasureRecursive(UIElement element)
        {
            if (element == null) return;
            element.InvalidateMeasure();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                InvalidateMeasureRecursive(VisualTreeHelper.GetChild(element, i) as UIElement);
            }
        }

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
                    RibbonControl.SetAppropriateSize((UIElement)visual, State);
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
            // Clear cache
            cachedMeasures.Clear();

            if (LauncherButton != null) LauncherButton.Click -= OnDialogLauncherButtonClick;
            LauncherButton = GetTemplateChild("PART_DialogLauncherButton") as Button;
            if (LauncherButton != null)
            {
                LauncherButton.Click += OnDialogLauncherButtonClick;
                if (LauncherKeys != null)
                    KeyTip.SetKeys(LauncherButton, LauncherKeys);
            }

            popup = GetTemplateChild("PART_Popup") as Popup;
            if (popup != null)
            {
                /*Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                popup.SetBinding(Popup.IsOpenProperty, binding);*/
            }

            downGrid = GetTemplateChild("PART_DownGrid") as Grid;
            upPanel = GetTemplateChild("PART_UpPanel") as Panel;
            parentPanel = GetTemplateChild("PART_ParentPanel") as Panel;
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown�routed 
        /// event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if ((State == RibbonGroupBoxState.Collapsed) && (popup != null))
            {
                e.Handled = true;
                //Mouse.Capture(popup, CaptureMode.Element);
                //RaiseEvent(new RoutedEventArgs(RibbonControl.ClickEvent, this));
                if (!IsDropDownOpen)
                {
                    IsDropDownOpen = true;
                }
                else PopupService.RaiseDismissPopupEvent(this,DismissPopupMode.MouseNotOver);
            }
        }

        /// <summary>
        /// Called to remeasure a control.
        /// </summary>
        /// <param name="constraint">The maximum size that the method can return.</param>
        /// <returns>The size of the control, up to the maximum specified by constraint.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            // System.Diagnostics.Debug.WriteLine("Measure " + Header + " (" + State + ") (" + scale + ")");
            if (State == RibbonGroupBoxState.Collapsed) return base.MeasureOverride(constraint);

            Size size = base.MeasureOverride(constraint);
            if ((upPanel != null) && (upPanel.DesiredSize.Width < downGrid.DesiredSize.Width))
            {
                return base.MeasureOverride(new Size(upPanel.DesiredSize.Width + upPanel.Margin.Left + upPanel.Margin.Right, constraint.Height));
            }
            return size;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Dialog launcher button click handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">the event data</param>
        void OnDialogLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            if (LauncherClick != null) LauncherClick(this, e);
        }

        // Handles popup closing
        void OnRibbonGroupBoxPopupClosing()
        {
            //IsHitTestVisible = true;
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }

        // handles popup opening
        void OnRibbonGroupBoxPopupOpening()
        {
            //IsHitTestVisible = false;            
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Handles IsOpen propertyu changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox ribbon = (RibbonGroupBox)d;

            if (ribbon.IsDropDownOpen)
            {
                ribbon.OnRibbonGroupBoxPopupOpening();
            }
            else
            {
                ribbon.OnRibbonGroupBoxPopupClosing();
            }
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public FrameworkElement CreateQuickAccessItem()
        {
            ToggleButton button = new ToggleButton();

            button.Size = RibbonControlSize.Small;

            button.PreviewMouseLeftButtonDown += OnQuickAccessClick;

            Binding binding = new Binding("Icon");
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            button.SetBinding(RibbonControl.IconProperty, binding);

            return button;
        }

        UIElement popupPlacementTarget;

        void OnQuickAccessClick(object sender, MouseButtonEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            if ((!IsDropDownOpen) && (!IsSnapped))
            {
                if (popup == null)
                {
                    // Trying to load control
                    RibbonTabItem item = Parent as RibbonTabItem;
                    if (item != null)
                    {
                        RibbonTabControl tabControl = item.Parent as RibbonTabControl;
                        if (tabControl != null)
                        {
                            RibbonTabItem selectedItem = tabControl.SelectedItem as RibbonTabItem;
                            tabControl.SelectedItem = item;
                            tabControl.UpdateLayout();
                            tabControl.SelectedItem = selectedItem;
                        }
                    }
                }
                IsSnapped = true;
                savedState = this.State;
                this.State = RibbonGroupBoxState.Collapsed;
                if (!IsVisible)
                {
                    UIElement element = popup.Child;
                    popup.Child = null;
                    if (element != null)
                    {
                        Decorator parent = VisualTreeHelper.GetParent(element) as Decorator;
                        if (parent != null) parent.Child = null;
                    }
                    quickAccessPopup = new Popup();
                    quickAccessPopup.AllowsTransparency = popup.AllowsTransparency;
                    quickAccessPopup.Child = element;
                }
                else quickAccessPopup = popup as Popup;
                quickAccessPopup.Closed += OnMenuClosed;
                popupPlacementTarget = popup.PlacementTarget;
                quickAccessPopup.PlacementTarget = button;
                quickAccessPopup.Tag = button;
                if (IsVisible)
                {
                    Width = ActualWidth;
                    Height = ActualHeight;
                }
                savedScale = Scale;
                Scale = -100;
                quickAccessPopup.IsOpen = true;
                //RaiseEvent(new RoutedEventArgs(RibbonControl.ClickEvent, this));
                /*
                if (quickAccessPopup.Child != null)
                {
                    Decorator parent = VisualTreeHelper.GetParent(quickAccessPopup.Child) as Decorator;
                    if (parent != null) parent.UpdateLayout();
                }
                */
                if (quickAccessPopup.Child != null) quickAccessPopup.Child.InvalidateMeasure();
                button.IsChecked = true;
                e.Handled = true;
            }
        }

        private void OnMenuClosed(object sender, EventArgs e)
        {
            Scale = savedScale;
            if (quickAccessPopup != popup)
            {
                UIElement element = quickAccessPopup.Child;
                quickAccessPopup.Child = null;
                if (element != null)
                {
                    Decorator parent = VisualTreeHelper.GetParent(element) as Decorator;
                    if (parent != null) parent.Child = null;
                }
                popup.Child = element;
            }
            Width = double.NaN;
            Height = double.NaN;
            this.State = savedState;
            quickAccessPopup.PlacementTarget = popupPlacementTarget;
            UpdateLayout();
            ((ToggleButton)((Popup)sender).Tag).IsChecked = false;
            quickAccessPopup.Closed -= OnMenuClosed;
            quickAccessPopup = null;
            IsSnapped = false;
            IsDropDownOpen = false;
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddToQuickAccessToolBarProperty); }
            set { SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty =
            DependencyProperty.Register("CanAddToQuickAccessToolBar", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(true));

        #endregion
    }
}
