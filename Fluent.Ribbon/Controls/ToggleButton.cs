// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Data;
    using System.Windows.Markup;
    using Fluent.Helpers;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents toggle button
    /// </summary>
    [ContentProperty(nameof(Header))]
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton, IToggleButton, IRibbonControl, IQuickAccessItemProvider, ILargeIconProvider
    {
        #region Properties

        #region Size

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public object Header
        {
            get { return this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region Icon

        /// <inheritdoc />
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

        #endregion

        #region LargeIcon

        /// <inheritdoc />
        public object LargeIcon
        {
            get { return this.GetValue(LargeIconProperty); }
            set { this.SetValue(LargeIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SmallIcon.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LargeIconProperty = LargeIconProviderProperties.LargeIconProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

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

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var button = new ToggleButton();

            RibbonControl.Bind(this, button, nameof(this.IsChecked), IsCheckedProperty, BindingMode.TwoWay);
            button.Click += (sender, e) => this.RaiseEvent(e);
            RibbonControl.BindQuickAccessItem(this, button);

            return button;
        }

        /// <inheritdoc />
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(ToggleButton), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

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

        /// <inheritdoc />
        void ILogicalChildSupport.AddLogicalChild(object child)
        {
            this.AddLogicalChild(child);
        }

        /// <inheritdoc />
        void ILogicalChildSupport.RemoveLogicalChild(object child)
        {
            this.RemoveLogicalChild(child);
        }

        /// <inheritdoc />
        protected override IEnumerator LogicalChildren
        {
            get
            {
                var baseEnumerator = base.LogicalChildren;
                while (baseEnumerator?.MoveNext() == true)
                {
                    yield return baseEnumerator.Current;
                }

                if (this.Icon != null)
                {
                    yield return this.Icon;
                }

                if (this.LargeIcon != null)
                {
                    yield return this.LargeIcon;
                }

                if (this.Header != null)
                {
                    yield return this.Header;
                }
            }
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonToggleButtonAutomationPeer(this);
    }
}