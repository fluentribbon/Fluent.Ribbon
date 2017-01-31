using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents button control that allows 
    /// you to add menu and handle clicks
    /// </summary>
    [TemplatePart(Name = "PART_Button", Type = typeof(ButtonBase))]
    public class SplitButton : DropDownButton, IToggleButton, ICommandSource
    {
        #region Fields

        // Inner button
        private ToggleButton button;

        private SplitButton quickAccessButton;
        TwoLineLabel label;

        #endregion

        #region Properties

        /// <summary>
        /// Gets an enumerator for logical child elements of this element.
        /// </summary>
        protected override IEnumerator LogicalChildren
            {
                get
                {
                    var baseEnumerator = base.LogicalChildren;
                    while (baseEnumerator.MoveNext())
                    {
                        yield return baseEnumerator.Current;
                    }

                    if (this.button != null)
                    {
                        yield return this.button;
                    }
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
                    return (ICommand)this.GetValue(CommandProperty);
                }
                set
                {
                    this.SetValue(CommandProperty, value);
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
                    return this.GetValue(CommandParameterProperty);
                }
                set
                {
                    this.SetValue(CommandParameterProperty, value);
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
                    return (IInputElement)this.GetValue(CommandTargetProperty);
                }
                set
                {
                    this.SetValue(CommandTargetProperty, value);
                }
            }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata());
        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata());

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(SplitButton), new FrameworkPropertyMetadata());

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
                get { return (string)this.GetValue(GroupNameProperty); }
                set { this.SetValue(GroupNameProperty, value); }
            }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(SplitButton),
            new PropertyMetadata(ToggleButtonHelper.OnGroupNameChanged));

            #endregion

            #region IsChecked

            /// <summary>
            /// Gets or sets a value indicating whether SplitButton is checked
            /// </summary>
            public bool? IsChecked
            {
                get { return (bool)this.GetValue(IsCheckedProperty); }
                set { this.SetValue(IsCheckedProperty, value); }
            }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool?), typeof(SplitButton), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsCheckedChanged, CoerceIsChecked));

           private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (SplitButton)d;
            if (button.IsCheckable)
            {
                if ((bool)e.NewValue)
                {
                    button.RaiseEvent(new RoutedEventArgs(CheckedEvent, button));
                }
                else
                {
                    button.RaiseEvent(new RoutedEventArgs(UncheckedEvent, button));
                }

                ToggleButtonHelper.OnIsCheckedChanged(d, e);
            }
        }

     private static object CoerceIsChecked(DependencyObject d, object basevalue)
        {
            var button = (SplitButton)d;

            if (button.IsCheckable == false)
            {
                return false;
            }

            return ToggleButtonHelper.CoerceIsChecked(d, basevalue);
        }

            #endregion

            #region IsCheckable

            /// <summary>
            /// Gets or sets a value indicating whether SplitButton can be checked
            /// </summary>
            public bool IsCheckable
            {
                get { return (bool)this.GetValue(IsCheckableProperty); }
                set { this.SetValue(IsCheckableProperty, value); }
            }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsChecked.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsCheckableProperty =
            DependencyProperty.Register(nameof(IsCheckable), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.FalseBox));

            #endregion

            #region DropDownToolTip

            /// <summary>
            /// Gets or sets tooltip of dropdown part of split button
            /// </summary>
            public object DropDownToolTip
            {
                get { return this.GetValue(DropDownToolTipProperty); }
                set { this.SetValue(DropDownToolTipProperty, value); }
            }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DropDownToolTip.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DropDownToolTipProperty =
            DependencyProperty.Register(nameof(DropDownToolTip), typeof(object), typeof(SplitButton), new PropertyMetadata());

            #endregion

            #region IsButtonEnabled

            /// <summary>
            /// Gets or sets a value indicating whether dropdown part of split button is enabled
            /// </summary>
            public bool IsButtonEnabled
            {
                get { return (bool)this.GetValue(IsButtonEnabledProperty); }
                set { this.SetValue(IsButtonEnabledProperty, value); }
            }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDropDownEnabled.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsButtonEnabledProperty =
            DependencyProperty.Register(nameof(IsButtonEnabled), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.TrueBox));

            #endregion

            #region IsDefinitive

            /// <summary>
            /// Gets or sets whether ribbon control click must close backstage
            /// </summary>
            public bool IsDefinitive
            {
                get { return (bool)this.GetValue(IsDefinitiveProperty); }
                set { this.SetValue(IsDefinitiveProperty, value); }
            }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.TrueBox));

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
                this.AddHandler(ClickEvent, value);
            }

            remove
            {
                this.RemoveHandler(ClickEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is checked
        /// </summary>
        public static readonly RoutedEvent CheckedEvent = System.Windows.Controls.Primitives.ToggleButton.CheckedEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is checked
        /// </summary>
        public event RoutedEventHandler Checked
        {
            add
            {
                this.AddHandler(CheckedEvent, value);
            }

            remove
            {
                this.RemoveHandler(CheckedEvent, value);
            }
        }

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public static readonly RoutedEvent UncheckedEvent = System.Windows.Controls.Primitives.ToggleButton.UncheckedEvent.AddOwner(typeof(SplitButton));

        /// <summary>
        /// Occurs when button is unchecked
        /// </summary>
        public event RoutedEventHandler Unchecked
        {
            add
            {
                this.AddHandler(UncheckedEvent, value);
            }

            remove
            {
                this.RemoveHandler(UncheckedEvent, value);
            }
        }

        #endregion

        #region Constructors

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
            FocusVisualStyleProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata());
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SplitButton()
        {
            ContextMenuService.Coerce(this);
            this.Click += this.OnClick;
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
            if (ReferenceEquals(e.OriginalSource, this) == false
                && ReferenceEquals(e.OriginalSource, this.quickAccessButton) == false)
            {
                e.Handled = true;
            }
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

            this.button = this.GetTemplateChild("PART_Button") as ToggleButton;

            this.label = this.GetTemplateChild("controlLabel") as TwoLineLabel;

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
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!PopupService.IsMousePhysicallyOver(this.button))
            {
                base.OnPreviewMouseLeftButtonDown(e);
            }
            else
            {
                this.IsDropDownOpen = false;
            }
        }

        #region Overrides of DropDownButton

        /// <summary>
        /// Provides class handling for the <see cref="E:System.Windows.UIElement.KeyDown"/> routed event that occurs when the user presses a key.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.UIElement.KeyDown"/> event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Enter)
            {
                this.button.InvokeClick();
            }
        }

        #endregion

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }

    protected override Size MeasureOverride(Size constraint)
    {
      if (label != null && label.Text != null)
        label.Measure(constraint);

      Size size = base.MeasureOverride(constraint);
      if (label == null)
        return size;
      else
        return new Size(Math.Max(label.DesiredSize.Width, size.Width), size.Height);
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
            var buttonForQAT = new SplitButton();
            buttonForQAT.Click += (sender, e) => this.RaiseEvent(e);
            RibbonProperties.SetSize(buttonForQAT, RibbonControlSize.Small);
            buttonForQAT.CanAddButtonToQuickAccessToolBar = false;
            this.BindQuickAccessItem(buttonForQAT);
            this.BindQuickAccessItemDropDownEvents(buttonForQAT);
            buttonForQAT.DropDownOpened += this.OnQuickAccessOpened;
            this.quickAccessButton = buttonForQAT;
            return buttonForQAT;
        }

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected override void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.Bind(this, element, nameof(this.DisplayMemberPath), DisplayMemberPathProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.GroupStyleSelector), GroupStyleSelectorProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemContainerStyle), ItemContainerStyleProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemsPanel), ItemsPanelProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemStringFormat), ItemStringFormatProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.ItemTemplate), ItemTemplateProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.OneWay);
            RibbonControl.Bind(this, element, nameof(this.IsChecked), IsCheckedProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, nameof(this.DropDownToolTip), DropDownToolTipProperty, BindingMode.TwoWay);
            RibbonControl.Bind(this, element, nameof(this.IsCheckable), IsCheckableProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.IsButtonEnabled), IsButtonEnabledProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.ContextMenu), ContextMenuProperty, BindingMode.Default);

            RibbonControl.Bind(this, element, nameof(this.ResizeMode), ResizeModeProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.MaxDropDownHeight), MaxDropDownHeightProperty, BindingMode.Default);
            RibbonControl.Bind(this, element, nameof(this.HasTriangle), HasTriangleProperty, BindingMode.Default);

            RibbonControl.BindQuickAccessItem(this, element);
        }

        /// <summary>
        /// Gets or sets whether button can be added to quick access toolbar
        /// </summary>
        public bool CanAddButtonToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddButtonToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddButtonToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddButtonToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddButtonToQuickAccessToolBarProperty = DependencyProperty.Register(nameof(CanAddButtonToQuickAccessToolBar), typeof(bool), typeof(SplitButton), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        #endregion
    }
}
