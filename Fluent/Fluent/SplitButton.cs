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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents button control that allows 
    /// you to add menu and handle clicks
    /// </summary>
    [TemplatePart(Name = "PART_Button", Type = typeof(ButtonBase))]
    public class SplitButton : DropDownButton, IToggleButton, ICommandSource
    {
        #region Fields

        // Inner button
        ToggleButton button;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                ArrayList list = new ArrayList();
                if (Items != null) list.AddRange(Items);
                if (button != null) list.Add(button);
                return list.GetEnumerator();
            }
        }

        #region Command

        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed. This is a dependency property.
        /// </summary>
        [Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the parameter to pass to the System.Windows.Controls.Primitives.ButtonBase.Command property. This is a dependency property.
        /// </summary>
        [Bindable(true), Localizability(LocalizationCategory.NeverLocalize), Category("Action")]
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the element on which to raise the specified command. This is a dependency property.
        /// </summary>
        [Bindable(true), Category("Action")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }
            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata(null));

        #endregion

        #region GroupName

        /// <summary>
        /// Gets or sets the name of the group that the toggle button belongs to. 
        /// Use the GroupName property to specify a grouping of toggle buttons to 
        /// create a mutually exclusive set of controls. You can use the GroupName 
        /// property when only one selection is possible from a list of available 
        /// options. When this property is set, only one ToggleButton in the specified
        /// group can be selected at a time.
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(SplitButton),
            new UIPropertyMetadata(null, ToggleButtonHelper.OnGroupNameChanged));

        #endregion

        #region IsChecked

        /// <summary>
        /// Gets or sets a value indicating whether SplitButton is checked
        /// </summary>
        public bool? IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(SplitButton), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged, CoerceIsChecked));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SplitButton button = d as SplitButton;
            if (button.IsCheckable)
            {
                if ((bool)e.NewValue) button.RaiseEvent(new RoutedEventArgs(CheckedEvent, button));
                else button.RaiseEvent(new RoutedEventArgs(UncheckedEvent, button));

                ToggleButtonHelper.OnIsCheckedChanged(d, e);
            }
        }

        private static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            SplitButton button = d as SplitButton;

            if (!button.IsCheckable) return false;

            return ToggleButtonHelper.CoerceIsChecked(d, basevalue);
        }

        #endregion

        #region IsCheckable

        /// <summary>
        /// Gets or sets a value indicating whether SplitButton can be checked
        /// </summary>
        public bool IsCheckable
        {
            get { return (bool)GetValue(IsCheckableProperty); }
            set { SetValue(IsCheckableProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register("IsCheckable", typeof(bool), typeof(SplitButton), new UIPropertyMetadata(false));

        #endregion

        #region DropDownToolTip

        /// <summary>
        /// Gets or sets tooltip of dropdown part of split button
        /// </summary>
        public object DropDownToolTip
        {
            get { return GetValue(DropDownToolTipProperty); }
            set { SetValue(DropDownToolTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownToolTipProperty =
            DependencyProperty.Register("DropDownToolTip", typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));

        #endregion

        #region IsButtonEnabled

        /// <summary>
        /// Gets or sets a value indicating whether dropdown part of split button is enabled
        /// </summary>
        public bool IsButtonEnabled
        {
            get { return (bool)GetValue(IsButtonEnabledProperty); }
            set { SetValue(IsButtonEnabledProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsButtonEnabledProperty =
            DependencyProperty.Register("IsButtonEnabled", typeof(bool), typeof(SplitButton), new UIPropertyMetadata(true));

        #endregion

        #region IsDefinitive

        /// <summary>
        /// Gets or sets whether ribbon control click must close backstage
        /// </summary>
        public bool IsDefinitive
        {
            get { return (bool)GetValue(IsDefinitiveProperty); }
            set { SetValue(IsDefinitiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register("IsDefinitive", typeof(bool), typeof(SplitButton), new UIPropertyMetadata(true));

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs when user clicks
        /// </summary>
        public static readonly RoutedEvent ClickEvent = ButtonBase.ClickEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when user clicks
        /// </summary>
        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }

            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is checked
        /// </summary>
        public static readonly RoutedEvent CheckedEvent = ToggleButton.CheckedEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is checked
        /// </summary>
        public event RoutedEventHandler Checked
        {
            add
            {
                AddHandler(CheckedEvent, value);
            }

            remove
            {
                RemoveHandler(CheckedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public static readonly RoutedEvent UncheckedEvent = ToggleButton.UncheckedEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public event RoutedEventHandler Unchecked
        {
            add
            {
                AddHandler(UncheckedEvent, value);
            }

            remove
            {
                RemoveHandler(UncheckedEvent, value);
            }
        }

        #endregion

        #region Constructors

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            FocusVisualStyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null));
            StyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(SplitButton));
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitButton()
        {
            ContextMenuService.Coerce(this);
            //FocusManager.SetIsFocusScope(this, true);
            Click += OnClick;
            //            AddHandler(ClickEvent, OnClick);

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;
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

            if (this.button != null)
            {
                this.button.Click += this.OnButtonClick;
            }
        }

        private void UnSubscribeEvents()
        {
            if (this.button != null)
            {
                this.button.Click -= this.OnButtonClick;
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource != this) e.Handled = true;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked 
        /// whenever application code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            this.UnSubscribeEvents();

            button = GetTemplateChild("PART_Button") as ToggleButton;

            base.OnApplyTemplate();

            this.SubscribeEvents();
        }
        /// <summary>
        /// Invoked when an unhandled System.Windows.UIElement.PreviewMouseLeftButtonDown routed event 
        /// reaches an element in its route that is derived from this class. Implement this method to add 
        /// class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.MouseButtonEventArgs that contains the event data. 
        /// The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!PopupService.IsMousePhysicallyOver(button)) base.OnPreviewMouseLeftButtonDown(e);
        }

        void OnButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }

        #endregion

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be synchronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public override FrameworkElement CreateQuickAccessItem()
        {
            SplitButton button = new SplitButton();
            button.Click += ((sender, e) => RaiseEvent(e));
            button.Size = RibbonControlSize.Small;
            button.CanAddButtonToQuickAccessToolBar = false;
            BindQuickAccessItem(button);
            BindQuickAccessItemDropDownEvents(button);
            button.DropDownOpened += OnQuickAccessOpened;
            return button;
        }

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.Bind(this, element, "DisplayMemberPath", DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "GroupStyleSelector", GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "ItemContainerStyle", ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "ItemsPanel", ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "ItemStringFormat", ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "ItemTemplate", ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, "IsChecked", IsCheckedProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, "DropDownToolTip", DropDownToolTipProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, "IsCheckable", IsCheckableProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "IsButtonEnabled", IsButtonEnabledProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "ContextMenu", ContextMenuProperty, BindingMode.Default);
            RibbonControl.BindQuickAccessItem(this, element);
            RibbonControl.Bind(this, element, "ResizeMode", ResizeModeProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "MaxDropDownHeight", MaxDropDownHeightProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, "HasTriangle", HasTriangleProperty, BindingMode.Default);
        }

        /// <summary>
        /// Gets or sets whether button can be added to quick access toolbar
        /// </summary>
        public bool CanAddButtonToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddButtonToQuickAccessToolBarProperty); }
            set { SetValue(CanAddButtonToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddButtonToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddButtonToQuickAccessToolBarProperty = DependencyProperty.Register("CanAddButtonToQuickAccessToolBar", typeof(bool), typeof(SplitButton), new UIPropertyMetadata(true, RibbonControl.OnCanAddToQuickAccessToolbarChanged));


        #endregion
    }
}
