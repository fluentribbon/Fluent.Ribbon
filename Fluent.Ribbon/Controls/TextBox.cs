// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Collections;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents custom Fluent UI TextBox
/// </summary>
[TemplatePart(Name = "PART_ContentHost", Type = typeof(UIElement))]
public class TextBox : System.Windows.Controls.TextBox, IQuickAccessItemProvider, IRibbonControl, IMediumIconProvider, ISimplifiedRibbonControl
{
    private UIElement? contentHost;

    #region Properties (Dependency)

    #region IsSimplified

    /// <summary>
    /// Gets or sets whether or not the ribbon is in Simplified mode
    /// </summary>
    public bool IsSimplified
    {
        get => (bool)this.GetValue(IsSimplifiedProperty);
        private set => this.SetValue(IsSimplifiedPropertyKey, BooleanBoxes.Box(value));
    }

    private static readonly DependencyPropertyKey IsSimplifiedPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsSimplified), typeof(bool), typeof(TextBox), new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Identifies the <see cref="IsSimplified"/> dependency property.</summary>
    public static readonly DependencyProperty IsSimplifiedProperty = IsSimplifiedPropertyKey.DependencyProperty;

    #endregion

    #endregion Properties

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
            if (coerced is not null)
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
        get => (bool)this.GetValue(CanAddToQuickAccessToolBarProperty);
        set => this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
    public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty = RibbonControl.CanAddToQuickAccessToolBarProperty.AddOwner(typeof(TextBox), new PropertyMetadata(BooleanBoxes.TrueBox, RibbonControl.OnCanAddToQuickAccessToolBarChanged));

    /// <summary>
    /// This method must be overridden to bind properties to use in quick access creating
    /// </summary>
    /// <param name="element">Toolbar item</param>
    protected virtual void BindQuickAccessItem(FrameworkElement element)
    {
        RibbonControl.BindQuickAccessItem(this, element);

        RibbonControl.BindQuickAccessItem(this, element);

        RibbonControl.Bind(this, element, nameof(this.Text), TextProperty, BindingMode.TwoWay, UpdateSourceTrigger.PropertyChanged);
        RibbonControl.Bind(this, element, nameof(this.IsReadOnly), IsReadOnlyProperty, BindingMode.OneWay);
        RibbonControl.Bind(this, element, nameof(this.CharacterCasing), CharacterCasingProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.MaxLength), MaxLengthProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.TextAlignment), TextAlignmentProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.TextDecorations), TextDecorationsProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.IsUndoEnabled), IsUndoEnabledProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.UndoLimit), UndoLimitProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.AutoWordSelection), AutoWordSelectionProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.SelectionBrush), SelectionBrushProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.SelectionOpacity), SelectionOpacityProperty, BindingMode.TwoWay);
        RibbonControl.Bind(this, element, nameof(this.CaretBrush), CaretBrushProperty, BindingMode.TwoWay);
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
        get => (RibbonControlSize)this.GetValue(SizeProperty);
        set => this.SetValue(SizeProperty, value);
    }

    /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
    public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(TextBox));

    #endregion

    #region SizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty);
        set => this.SetValue(SizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(TextBox));

    #endregion

    #region SimplifiedSizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SimplifiedSizeDefinition
    {
        get => (RibbonControlSizeDefinition)this.GetValue(SimplifiedSizeDefinitionProperty);
        set => this.SetValue(SimplifiedSizeDefinitionProperty, value);
    }

    /// <summary>Identifies the <see cref="SimplifiedSizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SimplifiedSizeDefinitionProperty = RibbonProperties.SimplifiedSizeDefinitionProperty.AddOwner(typeof(TextBox));

    #endregion

    #region KeyTip

    /// <inheritdoc />
    public string? KeyTip
    {
        get => (string?)this.GetValue(KeyTipProperty);
        set => this.SetValue(KeyTipProperty, value);
    }

    /// <inheritdoc cref="Fluent.KeyTip.KeysProperty"/>
    public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(TextBox));

    #endregion

    #region Header

    /// <inheritdoc />
    public object? Header
    {
        get => this.GetValue(HeaderProperty);
        set => this.SetValue(HeaderProperty, value);
    }

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(TextBox), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    /// <inheritdoc />
    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)this.GetValue(HeaderTemplateProperty);
        set => this.SetValue(HeaderTemplateProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateProperty = RibbonControl.HeaderTemplateProperty.AddOwner(typeof(TextBox), new PropertyMetadata());

    /// <inheritdoc />
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get => (DataTemplateSelector?)this.GetValue(HeaderTemplateSelectorProperty);
        set => this.SetValue(HeaderTemplateSelectorProperty, value);
    }

    /// <summary>Identifies the <see cref="HeaderTemplateSelector"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty = RibbonControl.HeaderTemplateSelectorProperty.AddOwner(typeof(TextBox), new PropertyMetadata());

    #endregion

    #region Icon

    /// <inheritdoc />
    public object? Icon
    {
        get => this.GetValue(IconProperty);
        set => this.SetValue(IconProperty, value);
    }

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(TextBox), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region MediumIcon

    /// <inheritdoc />
    public object? MediumIcon
    {
        get => this.GetValue(MediumIconProperty);
        set => this.SetValue(MediumIconProperty, value);
    }

    /// <summary>Identifies the <see cref="MediumIcon"/> dependency property.</summary>
    public static readonly DependencyProperty MediumIconProperty = MediumIconProviderProperties.MediumIconProperty.AddOwner(typeof(TextBox), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #endregion

    /// <inheritdoc />
    void ISimplifiedStateControl.UpdateSimplifiedState(bool isSimplified)
    {
        this.IsSimplified = isSimplified;
    }

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

            if (this.Icon is not null)
            {
                yield return this.Icon;
            }

            if (this.MediumIcon is not null)
            {
                yield return this.MediumIcon;
            }

            if (this.Header is not null)
            {
                yield return this.Header;
            }
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonTextBoxAutomationPeer(this);
}