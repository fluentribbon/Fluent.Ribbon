// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents custom Fluent UI TextBox
    /// </summary>
    [TemplatePart(Name = "PART_ContentHost", Type = typeof(UIElement))]
    public class TextBox : System.Windows.Controls.TextBox, IQuickAccessItemProvider, IRibbonControl
    {
        private UIElement contentHost;

        #region Properties (Dependency)

        #region InputWidth

        /// <summary>
        /// Gets or sets width of the value input part of textbox
        /// </summary>
        public double InputWidth
        {
            get { return (double)this.GetValue(InputWidthProperty); }
            set { this.SetValue(InputWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for InputWidth.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty InputWidthProperty =
            DependencyProperty.Register(nameof(InputWidth), typeof(double), typeof(TextBox), new PropertyMetadata(DoubleBoxes.NaN));

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentHost = this.Template.FindName("PART_ContentHost", this) as UIElement;
        }

        /// <inheritdoc />
        // Handling context menu manually to fix #653
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            this.InvalidateProperty(ContextMenuProperty);

            if (this.contentHost?.IsMouseOver == true
                || this.contentHost?.IsKeyboardFocusWithin == true)
            {
                base.OnContextMenuOpening(e);
            }
            else
            {
                var coerced = ContextMenuService.CoerceContextMenu(this, this.ContextMenu);
                if (coerced != null)
                {
                    this.SetCurrentValue(ContextMenuProperty, coerced);
                }

                base.OnContextMenuOpening(e);
            }
        }

        /// <inheritdoc />
        protected override void OnContextMenuClosing(ContextMenuEventArgs e)
        {
            this.InvalidateProperty(ContextMenuProperty);

            base.OnContextMenuClosing(e);
        }

        /// <inheritdoc />
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // Avoid Click invocation (from RibbonControl)
            if (e.Key == Key.Enter
                || e.Key == Key.Space)
            {
                return;
            }

            base.OnKeyUp(e);
        }

        #endregion

        #region Quick Access Item Creating

        /// <inheritdoc />
        public virtual FrameworkElement CreateQuickAccessItem()
        {
            var textBoxForQAT = new TextBox();

            this.BindQuickAccessItem(textBoxForQAT);

            return textBoxForQAT;
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(TextBox), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected virtual void BindQuickAccessItem(FrameworkElement element)
        {
            RibbonControl.BindQuickAccessItem(this, element);

            var textBoxQAT = (TextBox)element;

            textBoxQAT.Width = this.Width;

            this.ForwardBindingsForQAT(this, textBoxQAT);

            RibbonControl.BindQuickAccessItem(this, element);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private void ForwardBindingsForQAT(TextBox source, TextBox target)
        {
            RibbonControl.Bind(source, target, nameof(this.Text), TextProperty, BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged);
            RibbonControl.Bind(source, target, nameof(this.IsReadOnly), IsReadOnlyProperty, BindingMode.OneWay);
            RibbonControl.Bind(source, target, nameof(this.CharacterCasing), CharacterCasingProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.MaxLength), MaxLengthProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.TextAlignment), TextAlignmentProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.TextDecorations), TextDecorationsProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.IsUndoEnabled), IsUndoEnabledProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.UndoLimit), UndoLimitProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.AutoWordSelection), AutoWordSelectionProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.SelectionBrush), SelectionBrushProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.SelectionOpacity), SelectionOpacityProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.CaretBrush), CaretBrushProperty, BindingMode.TwoWay);
            RibbonControl.Bind(source, target, nameof(this.InputWidth), InputWidthProperty, BindingMode.TwoWay);
        }

        #endregion

        #region Implementation of Ribbon interfaces

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            this.SelectAll();
            this.Focus();

            return new KeyTipPressedResult(true, false);
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }

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
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(TextBox));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(TextBox));

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
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(TextBox));

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
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(TextBox), new PropertyMetadata());

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
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(TextBox), new PropertyMetadata(RibbonControl.OnIconChanged));

        #endregion

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
        protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.TextBoxAutomationPeer(this);
    }
}