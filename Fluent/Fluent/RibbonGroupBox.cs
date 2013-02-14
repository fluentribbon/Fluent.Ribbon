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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        Collapsed,
        /// <summary>
        /// QuickAccess. Group will collapse its content in a single button in quick access toolbar
        /// </summary>
        QuickAccess
    }

    /// <summary>
    /// RibbonGroup represents a logical group of controls as they appear on
    /// a RibbonTab.  These groups can resize its content
    /// </summary>
    [TemplatePart(Name = "PART_DialogLauncherButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_DownGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_UpPanel", Type = typeof(Panel))]
    public class RibbonGroupBox : ItemsControl, IQuickAccessItemProvider, IDropDownControl, IKeyTipedControl
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

        private void OnScalableControlScaled(object sender, EventArgs e)
        {
            if (!SuppressCacheReseting)
            {
                cachedMeasures.Clear();
            }
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
        private void DecreaseScalableElement()
        {
            foreach (object item in Items)
            {
                IScalableRibbonControl scalableRibbonControl = item as IScalableRibbonControl;
                if (scalableRibbonControl == null) continue;
                scalableRibbonControl.Reduce();
            }
        }

        private void UpdateScalableControlSubscribing(bool registerEvents = true)
        {
            foreach (var scalableRibbonControl in Items.OfType<IScalableRibbonControl>())
            {
                // Always unregister first to ensure that we don't subscribe twice
                scalableRibbonControl.Scaled -= OnScalableControlScaled;

                if (registerEvents)
                {
                    scalableRibbonControl.Scaled += OnScalableControlScaled;
                }
            }
        }

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets group box header
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            RibbonControl.HeaderProperty.AddOwner(typeof(RibbonGroupBox));

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
        public object LauncherIcon
        {
            get { return (ImageSource)GetValue(LauncherIconProperty); }
            set { SetValue(LauncherIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for LauncherIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LauncherIconProperty =
            DependencyProperty.Register("LauncherIcon", typeof(object), typeof(RibbonGroupBox), new UIPropertyMetadata(null, OnIconChanged));

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
            if ((box.State != RibbonGroupBoxState.Collapsed) && (box.State != RibbonGroupBoxState.QuickAccess)) return false;
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
        public object Icon
        //public ImageSource Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            RibbonControl.IconProperty.AddOwner(typeof(RibbonGroupBox), new UIPropertyMetadata(null, OnIconChanged));


        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox element = d as RibbonGroupBox;
            FrameworkElement oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null) element.RemoveLogicalChild(oldElement);
            FrameworkElement newElement = e.NewValue as FrameworkElement;
            if (newElement != null) element.AddLogicalChild(newElement);
        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Dialog launcher btton click event
        /// </summary>
        public event RoutedEventHandler LauncherClick;

        /// <summary>
        /// Occurs when context menu is opened
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when context menu is closed
        /// </summary>
        public event EventHandler DropDownClosed;

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
            StyleProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonGroupBox));
            }

            return basevalue;
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
            //AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
            ToolTip = new ToolTip();
            (ToolTip as ToolTip).Template = null;
            CoerceValue(ContextMenuProperty);
            Focusable = false;
            FocusManager.SetIsFocusScope(this, false);

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.SubscribeEvents();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.UnSubscribeEvents();
        }

        private void SubscribeEvents()
        {
            // Always unsubscribe events to ensure we don't subscribe twice
            this.UnSubscribeEvents();

            this.UpdateScalableControlSubscribing();

            if (LauncherButton != null)
            {
                LauncherButton.Click += OnDialogLauncherButtonClick;
            }

            if (popup != null)
            {
                popup.Opened += OnPopupOpened;
                popup.Closed += OnPopupClosed;
            }
        }

        private void UnSubscribeEvents()
        {
            this.UpdateScalableControlSubscribing(registerEvents: false);

            if (LauncherButton != null)
            {
                LauncherButton.Click -= OnDialogLauncherButtonClick;
            }

            if (popup != null)
            {
                popup.Opened -= OnPopupOpened;
                popup.Closed -= OnPopupClosed;
            }
        }

        /// <summary>
        /// Click event handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        private void OnClick(object sender, RoutedEventArgs e)
        {
            if ((State == RibbonGroupBoxState.Collapsed) || (State == RibbonGroupBoxState.QuickAccess))
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
                        RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap((int)ActualWidth,
                                                                                       (int)ActualHeight, 96, 96,
                                                                                       PixelFormats.Pbgra32);
                        renderTargetBitmap.Render((Visual)VisualTreeHelper.GetChild(this, 0));
                        snappedImage.FlowDirection = FlowDirection;
                        snappedImage.Source = renderTargetBitmap;
                        snappedImage.Width = ActualWidth;
                        snappedImage.Height = ActualHeight;
                        snappedImage.Visibility = Visibility.Visible;
                        isSnapped = value;
                    }
                }
                else if (snappedImage != null)
                {
                    // Clean up
                    snappedImage.Visibility = Visibility.Collapsed;
                    isSnapped = value;
                }
                InvalidateVisual();
            }
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
                    RibbonControl.SetAppropriateSize((UIElement)visual, State == RibbonGroupBoxState.QuickAccess ? RibbonGroupBoxState.Collapsed : State);
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
            this.UnSubscribeEvents();

            // Clear cache
            cachedMeasures.Clear();

            LauncherButton = GetTemplateChild("PART_DialogLauncherButton") as Button;

            if (LauncherButton != null)
            {
                if (LauncherKeys != null)
                {
                    KeyTip.SetKeys(LauncherButton, LauncherKeys);
                }
            }

            popup = GetTemplateChild("PART_Popup") as Popup;

            downGrid = GetTemplateChild("PART_DownGrid") as Grid;
            upPanel = GetTemplateChild("PART_UpPanel") as Panel;
            parentPanel = GetTemplateChild("PART_ParentPanel") as Panel;

            snappedImage = GetTemplateChild("PART_SnappedImage") as Image;

            this.SubscribeEvents();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            if (DropDownOpened != null) DropDownOpened(this, e);
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (DropDownClosed != null) DropDownClosed(this, e);
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
            if (((State == RibbonGroupBoxState.Collapsed) || (State == RibbonGroupBoxState.QuickAccess)) && (popup != null))
            {
                e.Handled = true;
                if (!IsDropDownOpen)
                {
                    IsDropDownOpen = true;
                }
                else PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.MouseNotOver);
            }
        }

        /*/// <summary>
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
        }*/

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
            RibbonGroupBox groupBox = new RibbonGroupBox();

            groupBox.DropDownOpened += OnQuickAccessOpened;
            groupBox.DropDownClosed += OnQuickAccessClosed;
            groupBox.State = RibbonGroupBoxState.QuickAccess;


            //RibbonControl.BindQuickAccessItem(this, groupBox);
            //if (QuickAccessElementStyle != null) RibbonControl.Bind(this, groupBox, "QuickAccessElementStyle", StyleProperty, BindingMode.OneWay);
            //RibbonControl.Bind(this, groupBox, "Icon", RibbonControl.IconProperty, BindingMode.OneWay);

            if (Icon != null)
            {
                Visual iconVisual = Icon as Visual;
                if (iconVisual != null)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 16;
                    rect.Height = 16;
                    rect.Fill = new VisualBrush(iconVisual);
                    groupBox.Icon = rect;
                }
                else RibbonControl.Bind(this, groupBox, "Icon", RibbonControl.IconProperty, BindingMode.OneWay);
            }
            if (Header != null) RibbonControl.Bind(this, groupBox, "Header", RibbonControl.HeaderProperty, BindingMode.OneWay);

            return groupBox;
        }

        private void OnQuickAccessOpened(object sender, EventArgs e)
        {
            if ((!IsDropDownOpen) && (!IsSnapped))
            {
                RibbonGroupBox groupBox = sender as RibbonGroupBox;
                // Save state
                IsSnapped = true;
                for (int i = 0; i < Items.Count; i++)
                {
                    object item = Items[0];
                    Items.Remove(item);
                    groupBox.Items.Add(item);
                    i--;
                }
            }
        }

        private void OnQuickAccessClosed(object sender, EventArgs e)
        {
            RibbonGroupBox groupBox = sender as RibbonGroupBox;

            for (int i = 0; i < groupBox.Items.Count; i++)
            {
                object item = groupBox.Items[0];
                groupBox.Items.Remove(item);
                Items.Add(item);
                i--;
            }
            IsSnapped = false;
        }

        /*UIElement popupPlacementTarget;
        
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
                //IsDropDownOpen = true;
                Mouse.Capture(this, CaptureMode.SubTree);
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
            if (Mouse.Captured == this) Mouse.Capture(null);
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
        }*/

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

        #region Implementation of IKeyTipedControl

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            if (this.State == RibbonGroupBoxState.Collapsed
                || this.State == RibbonGroupBoxState.QuickAccess)
            {
                this.IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
            this.IsDropDownOpen = false;
        }

        #endregion
    }
}
