// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Markup;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents button
    /// </summary>
    [ContentProperty(nameof(Header))]
    public class Button : System.Windows.Controls.Button, IRibbonControl, IQuickAccessItemProvider, ILargeIconProvider
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
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(Button));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(Button));

        #endregion

        #region KeyTip

        /// <inheritdoc />
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="KeyTip"/>.
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(Button));

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
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(Button), new PropertyMetadata());

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
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(Button), new PropertyMetadata(RibbonControl.OnIconChanged));

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
        public static readonly DependencyProperty LargeIconProperty = DependencyProperty.Register(nameof(LargeIcon), typeof(object), typeof(Button), new PropertyMetadata());

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
            DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(Button), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static Button()
        {
            var type = typeof(Button);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            ContextMenuService.Attach(type);
            ToolTipService.Attach(type);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Button()
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

            base.OnClick();
        }

        #endregion

        #region Quick Access Item Creating

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var button = new Button();
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(Button), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

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
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.ButtonAutomationPeer(this);
    }
}