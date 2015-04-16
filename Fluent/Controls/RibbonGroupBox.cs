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
    using Fluent.Internal;

    /// <summary>
    /// RibbonGroup represents a logical group of controls as they appear on
    /// a RibbonTab.  These groups can resize its content
    /// </summary>
    [TemplatePart(Name = "PART_DialogLauncherButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_UpPanel", Type = typeof(Panel))]
    public class RibbonGroupBox : ItemsControl, IQuickAccessItemProvider, IDropDownControl, IKeyTipedControl, IHeaderedControl
	{
        #region Fields

        // up part
        private Panel upPanel;

        private Panel parentPanel;

        // Freezed image (created during snapping)
        private Image snappedImage;
        
        // Is visual currently snapped
        private bool isSnapped;

        private readonly ItemContainerGeneratorAction updateChildSizesItemContainerGeneratorAction;

        #endregion

        #region Properties

        #region KeyTip

        /// <summary>
        /// Gets or sets KeyTip for element.
        /// </summary>
        public string KeyTip
        {
            get { return (string)GetValue(KeyTipProperty); }
            set { SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(RibbonGroupBox));

        #endregion

        /// <summary>
        /// Gets drop down popup
        /// </summary>
        public Popup DropDownPopup { get; private set; }

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
            var ribbonGroupBox = (RibbonGroupBox)d;
            ribbonGroupBox.updateChildSizesItemContainerGeneratorAction.QueueAction();
        }

        private void UpdateChildSizes()
        {
            var groupBoxState = this.State == RibbonGroupBoxState.QuickAccess
                            ? RibbonGroupBoxState.Collapsed
                            : this.State;

            foreach (var item in this.Items)
            {
                var element = this.ItemContainerGenerator.ContainerFromItem(item);

                if (element == null)
                {
                    continue;
                }

                RibbonProperties.SetAppropriateSize(element, groupBoxState);
            }
        }

        #endregion

        #region Scale

        // Current scale index
        private int scale;

        /// <summary>
        /// Gets or sets scale index (for internal IRibbonScalableControl)
        /// </summary>
        internal int Scale
        {
            get { return this.scale; }
            set
            {
                var difference = value - this.scale;
                this.scale = value;

                for (var i = 0; i < Math.Abs(difference); i++)
                {
                    if (difference > 0)
                    {
                        this.IncreaseScalableElement();
                    }
                    else
                    {
                        this.DecreaseScalableElement();
                    }
                }
            }
        }

        // Finds and increase size of all scalable elements in the given group box
        private void IncreaseScalableElement()
        {
            foreach (var item in Items)
            {
                var scalableRibbonControl = item as IScalableRibbonControl;
                if (scalableRibbonControl == null)
                {
                    continue;
                }
                scalableRibbonControl.Enlarge();
            }
        }

        private void OnScalableControlScaled(object sender, EventArgs e)
        {
            this.TryClearCache();
        }

        private void TryClearCache()
        {
            if (!this.SuppressCacheReseting)
            {
                this.cachedMeasures.Clear();
            }
        }

        /// <summary>
        /// Gets or sets whether to reset cache when scalable control is scaled
        /// </summary>
        internal bool SuppressCacheReseting { get; set; }

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

        private void UpdateScalableControlSubscribing()
        {
            UpdateScalableControlSubscribing(true);
        }

        private void UpdateScalableControlSubscribing(bool registerEvents)
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
        [DisplayName("DialogLauncher Keys"),
        Category("KeyTips"),
        Description("Key tip keys for dialog launcher button")]
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
                ribbonGroupBox.LauncherButton.KeyTip = (string)e.NewValue;
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

            PopupService.Attach(typeof(RibbonGroupBox));
            StyleProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(null, OnCoerceStyle));

            ContextMenuService.Attach(typeof(RibbonGroupBox));
        }

        // Coerce object style
        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonGroupBox));
            }

            return basevalue;
        }

        // Handles visibility changed
        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var box = (d as RibbonGroupBox);
            if (box != null) box.ClearCache();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonGroupBox()
        {
            this.ToolTip = new ToolTip();
            ((ToolTip)this.ToolTip).Template = null;
            this.CoerceValue(ContextMenuProperty);
            this.Focusable = false;

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            this.updateChildSizesItemContainerGeneratorAction = new ItemContainerGeneratorAction(this.ItemContainerGenerator, this.UpdateChildSizes);
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

            if (this.LauncherButton != null)
            {
                this.LauncherButton.Click += this.OnDialogLauncherButtonClick;
            }

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened += this.OnPopupOpened;
                this.DropDownPopup.Closed += this.OnPopupClosed;
            }
        }

        private void UnSubscribeEvents()
        {
            this.UpdateScalableControlSubscribing(false);

            if (this.LauncherButton != null)
            {
                this.LauncherButton.Click -= this.OnDialogLauncherButtonClick;
            }

            if (this.DropDownPopup != null)
            {
                this.DropDownPopup.Opened -= this.OnPopupOpened;
                this.DropDownPopup.Closed -= this.OnPopupClosed;
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
                return this.isSnapped;
            }
            set
            {
                if (value == this.isSnapped)
                {
                    return;
                }

                if (value)
                {
                    if (this.IsVisible)
                    {
                        // Render the freezed image                        
                        var renderTargetBitmap = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 96, 96, PixelFormats.Pbgra32);
                        renderTargetBitmap.Render((Visual)VisualTreeHelper.GetChild(this, 0));
                        this.snappedImage.FlowDirection = this.FlowDirection;
                        this.snappedImage.Source = renderTargetBitmap;
                        this.snappedImage.Width = this.ActualWidth;
                        this.snappedImage.Height = this.ActualHeight;
                        this.snappedImage.Visibility = Visibility.Visible;
                        this.isSnapped = true;
                    }
                }
                else if (this.snappedImage != null)
                {
                    // Clean up
                    this.snappedImage.Visibility = Visibility.Collapsed;
                    this.isSnapped = false;
                }

                this.InvalidateVisual();
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
                var stateScale = this.GetCurrentIntermediateStateScale();

                if (cachedMeasures.TryGetValue(stateScale, out result) == false)
                {
                    this.SuppressCacheReseting = true;
                    this.UpdateScalableControlSubscribing();

                    // Get desired size for these values
                    var backupState = State;
                    var backupScale = Scale;
                    this.State = this.StateIntermediate;
                    this.Scale = this.ScaleIntermediate;
                    this.InvalidateLayout();
                    this.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    this.cachedMeasures.Add(stateScale, this.DesiredSize);
                    result = this.DesiredSize;

                    // Rollback changes
                    this.State = backupState;
                    this.Scale = backupScale;
                    this.InvalidateLayout();
                    this.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                    this.SuppressCacheReseting = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Clears cache
        /// </summary>
        public void ClearCache()
        {
            this.cachedMeasures.Clear();
        }

        /// <summary>
        /// Invalidates layout (with children)
        /// </summary>
        internal void InvalidateLayout()
        {
            InvalidateMeasureRecursive(this);
        }

        private static void InvalidateMeasureRecursive(UIElement element)
        {
            if (element == null)
            {
                return;
            }

            element.InvalidateMeasure();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as UIElement;

                if (child == null)
                {
                    continue;
                }

                InvalidateMeasureRecursive(child);
            }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code 
        /// or internal processes call System.Windows.FrameworkElement.ApplyTemplate().
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.UnSubscribeEvents();

            // Clear cache
            this.cachedMeasures.Clear();

            this.LauncherButton = this.GetTemplateChild("PART_DialogLauncherButton") as Button;

            if (this.LauncherButton != null)
            {
                if (this.LauncherKeys != null)
                {
                    this.LauncherButton.KeyTip = this.LauncherKeys;
                }
            }

            this.DropDownPopup = this.GetTemplateChild("PART_Popup") as Popup;

            this.upPanel = this.GetTemplateChild("PART_UpPanel") as Panel;
            this.parentPanel = this.GetTemplateChild("PART_ParentPanel") as Panel;

            this.snappedImage = this.GetTemplateChild("PART_SnappedImage") as Image;

            this.SubscribeEvents();
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            if (this.DropDownOpened != null) this.DropDownOpened(this, e);
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            if (this.DropDownClosed != null) this.DropDownClosed(this, e);
        }

        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown
        /// event reaches an element in its route that is derived from this class. 
        /// Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source != this
                || this.DropDownPopup == null)
            {
                return;
            }

            if (this.State == RibbonGroupBoxState.Collapsed
                || this.State == RibbonGroupBoxState.QuickAccess)
            {
                e.Handled = true;

                if (!this.IsDropDownOpen)
                {
                    this.IsDropDownOpen = true;
                }
                else
                {
                    PopupService.RaiseDismissPopupEventAsync(this, DismissPopupMode.MouseNotOver);
                }
            }
        }

        /// <summary>
        /// Supports layout behavior when a child element is resized. 
        /// </summary>
        /// <param name="child">The child element that is being resized.</param>
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);

            this.cachedMeasures.Remove(this.GetCurrentIntermediateStateScale());
        }

        private StateScale GetCurrentIntermediateStateScale()
        {
            var stateScale = new StateScale 
                {
                    Scale = this.ScaleIntermediate, 
                    State = this.StateIntermediate
                };
            return stateScale;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Dialog launcher button click handler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">the event data</param>
        private void OnDialogLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.LauncherClick != null)
            {
                this.LauncherClick(this, e);
            }
        }

        // Handles popup closing
        private void OnRibbonGroupBoxPopupClosing()
        {
            //IsHitTestVisible = true;
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }

        // handles popup opening
        private void OnRibbonGroupBoxPopupOpening()
        {
            //IsHitTestVisible = false;            
            Mouse.Capture(this, CaptureMode.SubTree);
        }

        /// <summary>
        /// Handles IsOpen propertyu changes
        /// </summary>
        /// <param name="d">Object</param>
        /// <param name="e">The event data</param>
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ribbon = (RibbonGroupBox)d;

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
            var groupBox = new RibbonGroupBox();

            groupBox.DropDownOpened += this.OnQuickAccessOpened;
            groupBox.DropDownClosed += this.OnQuickAccessClosed;
            
            groupBox.State = RibbonGroupBoxState.QuickAccess;

            RibbonControl.Bind(this, groupBox, "ItemTemplateSelector", ItemTemplateSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "ItemsSource", ItemsSourceProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "LauncherCommandParameter", LauncherCommandParameterProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "LauncherCommand", LauncherCommandProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "LauncherCommandTarget", LauncherCommandTargetProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "LauncherIcon", LauncherIconProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "LauncherText", LauncherTextProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "LauncherToolTip", LauncherToolTipProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "IsLauncherEnabled", IsLauncherEnabledProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "IsLauncherVisible", IsLauncherVisibleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, groupBox, "DialogLauncherButtonKeyTipKeys", DialogLauncherButtonKeyTipKeysProperty, BindingMode.OneWay);
            groupBox.LauncherClick += this.LauncherClick;

            if (this.Icon != null)
            {
                var iconVisual = this.Icon as Visual;
                if (iconVisual != null)
                {
                    var rect = new Rectangle
                    {
                        Width = 16,
                        Height = 16,
                        Fill = new VisualBrush(iconVisual)
                    };
                    groupBox.Icon = rect;
                }
                else
                {
                    RibbonControl.Bind(this, groupBox, "Icon", RibbonControl.IconProperty, BindingMode.OneWay);
                }
            }

            if (this.Header != null)
            {
                RibbonControl.Bind(this, groupBox, "Header", RibbonControl.HeaderProperty, BindingMode.OneWay);
            }

            return groupBox;
        }

        private void OnQuickAccessOpened(object sender, EventArgs e)
        {
            if (!this.IsDropDownOpen
                && !this.IsSnapped)
            {
                var groupBox = sender as RibbonGroupBox;
                // Save state
                this.IsSnapped = true;

                if (this.ItemsSource == null)
                {
                    for (var i = 0; i < this.Items.Count; i++)
                    {
                        var item = this.Items[0];
                        this.Items.Remove(item);
                        groupBox.Items.Add(item);
                        i--;
                    }
                }
            }
        }

        private void OnQuickAccessClosed(object sender, EventArgs e)
        {
            var groupBox = sender as RibbonGroupBox;

            if (this.ItemsSource == null)
            {
                for (var i = 0; i < groupBox.Items.Count; i++)
                {
                    var item = groupBox.Items[0];
                    groupBox.Items.Remove(item);
                    this.Items.Add(item);
                    i--;
                }
            }

            this.IsSnapped = false;
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
            DependencyProperty.Register("CanAddToQuickAccessToolBar", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));        

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