// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents toggle button
    /// </summary>
    [ContentProperty(nameof(Header))]
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton, IToggleButton, IRibbonControl, IQuickAccessItemProvider, ILargeIconProvider
    {
        #region Properties

        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(ToggleButton));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(ToggleButton));

        #endregion

        #region KeyTip

        /// <summary>
        /// Gets or sets KeyTip for element.
        /// </summary>
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(ToggleButton));

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
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(ToggleButton), new PropertyMetadata(ToggleButtonHelper.OnGroupNameChanged));

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public object Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ToggleButton));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (ToggleButton)d;

            var oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null)
            {
                element.RemoveLogicalChild(oldElement);
            }

            var newElement = e.NewValue as FrameworkElement;
            if (newElement != null
                && LogicalTreeHelper.GetParent(newElement) == null)
            {
                element.AddLogicalChild(newElement);
            }
        }

        #endregion

        #region LargeIcon

        /// <summary>
        /// Gets or sets button large icon
        /// </summary>
        public object LargeIcon
        {
            get { return this.GetValue(LargeIconProperty); }
            set { this.SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty =
            DependencyProperty.Register(nameof(LargeIcon), typeof(object),
            typeof(ToggleButton), new PropertyMetadata());

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
            DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(ToggleButton), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ToggleButton"/> class.
        /// </summary>
        static ToggleButton()
        {
            var type = typeof(ToggleButton);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            ContextMenuService.Attach(type);
            ToolTipService.Attach(type);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleButton"/> class.
        /// </summary>
        public ToggleButton()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnClick()
        {
            // Close popup on click
            if (this.IsDefinitive)
            {
                PopupService.RaiseDismissPopupEvent(this, DismissPopupMode.Always);
            }

            // fix for #481
            // We can't overwrite OnToggle because it's "internal protected"...
            if (string.IsNullOrEmpty(this.GroupName) == false)
            {
                // Only forward click if button is not checked to prevent wrong bound values
                if (this.IsChecked == false)
                {
                    base.OnClick();
                }
            }
            else
            {
                base.OnClick();
            }
        }

        /// <inheritdoc />
        protected override void OnChecked(RoutedEventArgs e)
        {
            ToggleButtonHelper.UpdateButtonGroup(this);
            base.OnChecked(e);
        }

        #endregion

        /// <summary>
        /// Used to call OnClick (which is protected)
        /// </summary>
        public void InvokeClick()
        {
            this.OnClick();
        }

        #region Quick Access Item Creating

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var button = new ToggleButton();

            RibbonControl.Bind(this, button, nameof(this.IsChecked), IsCheckedProperty, BindingMode.TwoWay);
            button.Click += (sender, e) => this.RaiseEvent(e);
            RibbonControl.BindQuickAccessItem(this, button);

            return button;
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        #endregion

        #region Implementation of IKeyTipedControl

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            this.OnClick();

            return KeyTipPressedResult.Empty;
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }

        #endregion
    }
}