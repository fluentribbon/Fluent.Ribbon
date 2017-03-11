// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents custom Fluent UI TextBox
    /// </summary>
    public class TextBox : System.Windows.Controls.TextBox, IQuickAccessItemProvider, IRibbonControl
    {
        ////#region Fields

        ////// Content when the textbox got focus
        ////private string textBoxContentWhenGotFocus;

        ////#endregion

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
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static TextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));

            ContextMenuService.Attach(typeof(TextBox));
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public TextBox()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyUp�attached event reaches 
        /// an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
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

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public FrameworkElement CreateQuickAccessItem()
        {
            var textBoxForQAT = new TextBox();

            this.BindQuickAccessItem(textBoxForQAT);

            return textBoxForQAT;
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
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(TextBox), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolbarChanged));

        /// <summary>
        /// This method must be overridden to bind properties to use in quick access creating
        /// </summary>
        /// <param name="element">Toolbar item</param>
        protected void BindQuickAccessItem(FrameworkElement element)
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
        public void OnKeyTipPressed()
        {
            // Use dispatcher to avoid focus moving to backup'ed element 
            // (focused element before keytips processing)
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                (ThreadStart)(() =>
                {
                    this.SelectAll();
                    this.Focus();
                }));
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }

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
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(TextBox));

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
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(TextBox));

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
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(TextBox));

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
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(TextBox), new PropertyMetadata());

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
        public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(TextBox), new PropertyMetadata(OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (TextBox)d;

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

        #endregion
    }
}