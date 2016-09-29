using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Markup;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents button
    /// </summary>
    [ContentProperty(nameof(Header))]
    public class Button : System.Windows.Controls.Button, IRibbonControl, IQuickAccessItemProvider, ILargeIconProvider
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
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(Button));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(Button));

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
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(Button));

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
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
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(Button), new PropertyMetadata(OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (Button)d;

            var oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null)
            {
                element.RemoveLogicalChild(oldElement);
            }

            var newElement = e.NewValue as FrameworkElement;
            if (newElement != null)
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

        #region CornerRadius

        /// <summary>
        /// Gets or sets the CornerRadius for the element
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
            set { this.SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(Button), new PropertyMetadata(default(CornerRadius)));

        #endregion CornerRadius

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
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

        /// <summary>
        /// Called when a <see cref="T:System.Windows.Controls.Button"/> is clicked. 
        /// </summary>
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

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be synchronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var button = new Button();
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(Button), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        #endregion

        #region Implementation of IKeyTipedControl

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            this.OnClick();
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
        }

        #endregion
    }
}