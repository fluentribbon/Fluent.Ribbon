// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Markup;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;

/// <summary>
/// Represents button
/// </summary>
[ContentProperty(nameof(Header))]
[DebuggerDisplay("{GetType().FullName}: Header = {Header}, Size = {Size}, IsSimplified = {IsSimplified}")]
public class Button : System.Windows.Controls.Button, IRibbonControl, IQuickAccessItemProvider, ILargeIconProvider, IMediumIconProvider, ISimplifiedRibbonControl, ICornerRadiusControl
{
    #region Properties

    #region Size

    /// <inheritdoc />
    public RibbonControlSize Size
    {
        get { return (RibbonControlSize)this.GetValue(SizeProperty); }
        set { this.SetValue(SizeProperty, value); }
    }

    /// <summary>Identifies the <see cref="Size"/> dependency property.</summary>
    public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(Button));

    #endregion

    #region SizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SizeDefinition
    {
        get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
        set { this.SetValue(SizeDefinitionProperty, value); }
    }

    /// <summary>Identifies the <see cref="SizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(Button));

    #endregion

    #region SimplifiedSizeDefinition

    /// <inheritdoc />
    public RibbonControlSizeDefinition SimplifiedSizeDefinition
    {
        get { return (RibbonControlSizeDefinition)this.GetValue(SimplifiedSizeDefinitionProperty); }
        set { this.SetValue(SimplifiedSizeDefinitionProperty, value); }
    }

    /// <summary>Identifies the <see cref="SimplifiedSizeDefinition"/> dependency property.</summary>
    public static readonly DependencyProperty SimplifiedSizeDefinitionProperty = RibbonProperties.SimplifiedSizeDefinitionProperty.AddOwner(typeof(Button));

    #endregion

    #region KeyTip

    /// <inheritdoc />
    public string? KeyTip
    {
        get { return (string?)this.GetValue(KeyTipProperty); }
        set { this.SetValue(KeyTipProperty, value); }
    }

    /// <summary>
    /// <see cref="DependencyProperty"/> for <see cref="KeyTip"/>.
    /// </summary>
    public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(Button));

    #endregion

    #region Header

    /// <inheritdoc />
    public object? Header
    {
        get { return this.GetValue(HeaderProperty); }
        set { this.SetValue(HeaderProperty, value); }
    }

    /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderProperty = RibbonControl.HeaderProperty.AddOwner(typeof(Button), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    /// <inheritdoc />
    public DataTemplate? HeaderTemplate
    {
        get { return (DataTemplate?)this.GetValue(HeaderTemplateProperty); }
        set { this.SetValue(HeaderTemplateProperty, value); }
    }

    /// <summary>Identifies the <see cref="HeaderTemplate"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateProperty = RibbonControl.HeaderTemplateProperty.AddOwner(typeof(Button), new PropertyMetadata());

    /// <inheritdoc />
    public DataTemplateSelector? HeaderTemplateSelector
    {
        get { return (DataTemplateSelector?)this.GetValue(HeaderTemplateSelectorProperty); }
        set { this.SetValue(HeaderTemplateSelectorProperty, value); }
    }

    /// <summary>Identifies the <see cref="HeaderTemplateSelector"/> dependency property.</summary>
    public static readonly DependencyProperty HeaderTemplateSelectorProperty = RibbonControl.HeaderTemplateSelectorProperty.AddOwner(typeof(Button), new PropertyMetadata());

    #endregion

    #region Icon

    /// <inheritdoc />
    public object? Icon
    {
        get { return this.GetValue(IconProperty); }
        set { this.SetValue(IconProperty, value); }
    }

    /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
    public static readonly DependencyProperty IconProperty = RibbonControl.IconProperty.AddOwner(typeof(Button), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region LargeIcon

    /// <inheritdoc />
    public object? LargeIcon
    {
        get { return this.GetValue(LargeIconProperty); }
        set { this.SetValue(LargeIconProperty, value); }
    }

    /// <summary>Identifies the <see cref="LargeIcon"/> dependency property.</summary>
    public static readonly DependencyProperty LargeIconProperty = LargeIconProviderProperties.LargeIconProperty.AddOwner(typeof(Button), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region MediumIcon

    /// <inheritdoc />
    public object? MediumIcon
    {
        get { return this.GetValue(MediumIconProperty); }
        set { this.SetValue(MediumIconProperty, value); }
    }

    /// <summary>Identifies the <see cref="MediumIcon"/> dependency property.</summary>
    public static readonly DependencyProperty MediumIconProperty = MediumIconProviderProperties.MediumIconProperty.AddOwner(typeof(Button), new PropertyMetadata(LogicalChildSupportHelper.OnLogicalChildPropertyChanged));

    #endregion

    #region IsDefinitive

    /// <summary>
    /// Gets or sets whether ribbon control click must close backstage
    /// </summary>
    public bool IsDefinitive
    {
        get { return (bool)this.GetValue(IsDefinitiveProperty); }
        set { this.SetValue(IsDefinitiveProperty, BooleanBoxes.Box(value)); }
    }

    /// <summary>Identifies the <see cref="IsDefinitive"/> dependency property.</summary>
    public static readonly DependencyProperty IsDefinitiveProperty =
        DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(Button), new PropertyMetadata(BooleanBoxes.TrueBox));

    #endregion

    #region IsSimplified

    /// <summary>
    /// Gets or sets whether or not the ribbon is in Simplified mode
    /// </summary>
    public bool IsSimplified
    {
        get { return (bool)this.GetValue(IsSimplifiedProperty); }
        private set { this.SetValue(IsSimplifiedPropertyKey, BooleanBoxes.Box(value)); }
    }

    private static readonly DependencyPropertyKey IsSimplifiedPropertyKey =
        DependencyProperty.RegisterReadOnly(nameof(IsSimplified), typeof(bool), typeof(Button), new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Identifies the <see cref="IsSimplified"/> dependency property.</summary>
    public static readonly DependencyProperty IsSimplifiedProperty = IsSimplifiedPropertyKey.DependencyProperty;

    #endregion

    #region CornerRadius

    /// <inheritdoc />
    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)this.GetValue(CornerRadiusProperty); }
        set { this.SetValue(CornerRadiusProperty, value); }
    }

    /// <summary>Identifies the <see cref="CornerRadius"/> dependency property.</summary>
    public static readonly DependencyProperty CornerRadiusProperty = RibbonControl.CornerRadiusProperty.AddOwner(typeof(Button));

    #endregion

    #endregion Properties

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
        set { this.SetValue(CanAddToQuickAccessToolBarProperty, BooleanBoxes.Box(value)); }
    }

    /// <summary>Identifies the <see cref="CanAddToQuickAccessToolBar"/> dependency property.</summary>
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

            if (this.LargeIcon is not null)
            {
                yield return this.LargeIcon;
            }

            if (this.Header is not null)
            {
                yield return this.Header;
            }
        }
    }

    /// <inheritdoc />
    protected override AutomationPeer OnCreateAutomationPeer() => new Fluent.Automation.Peers.RibbonButtonAutomationPeer(this);
}