﻿// ReSharper disable once CheckNamespace
namespace Fluent;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Fluent.Extensibility;
using Fluent.Helpers;
using Fluent.Internal.KnownBoxes;
using JetBrains.Annotations;

/// <summary>
/// Attached Properties for the Fluent Ribbon library
/// </summary>
[PublicAPI]
public class RibbonProperties : DependencyObject
{
    #region Size Property

    /// <summary>
    /// Using a DependencyProperty as the backing store for Size.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.RegisterAttached("Size", typeof(RibbonControlSize), typeof(RibbonProperties),
            new FrameworkPropertyMetadata(RibbonControlSize.Large,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                OnSizeChanged));

    /// <summary>
    /// Sets <see cref="SizeProperty"/> for <paramref name="element"/>.
    /// </summary>
    public static void SetSize(DependencyObject element, RibbonControlSize value)
    {
        element.SetValue(SizeProperty, value);
    }

    /// <summary>
    /// Gets <see cref="SizeProperty"/> for <paramref name="element"/>.
    /// </summary>
    //[AttachedPropertyBrowsableForType(typeof(IRibbonControl))]
    public static RibbonControlSize GetSize(DependencyObject element)
    {
        return (RibbonControlSize)element.GetValue(SizeProperty);
    }

    private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var sink = d as IRibbonSizeChangedSink;

        sink?.OnSizePropertyChanged((RibbonControlSize)e.OldValue, (RibbonControlSize)e.NewValue);
    }

    #endregion

    #region SizeDefinition Property

    /// <summary>
    /// Using a DependencyProperty as the backing store for SizeDefinition.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty SizeDefinitionProperty =
        DependencyProperty.RegisterAttached("SizeDefinition", typeof(RibbonControlSizeDefinition), typeof(RibbonProperties),
            new FrameworkPropertyMetadata(new RibbonControlSizeDefinition(RibbonControlSize.Large, RibbonControlSize.Middle, RibbonControlSize.Small),
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                OnSizeDefinitionChanged));

    /// <summary>
    /// Sets <see cref="SizeDefinitionProperty"/> for <paramref name="element"/>.
    /// </summary>
    public static void SetSizeDefinition(DependencyObject element, RibbonControlSizeDefinition value)
    {
        element.SetValue(SizeDefinitionProperty, value);
    }

    /// <summary>
    /// Gets <see cref="SizeDefinitionProperty"/> for <paramref name="element"/>.
    /// </summary>
    //[AttachedPropertyBrowsableForType(typeof(IRibbonControl))]
    public static RibbonControlSizeDefinition GetSizeDefinition(DependencyObject element)
    {
        return (RibbonControlSizeDefinition)element.GetValue(SizeDefinitionProperty);
    }

    // Handles RibbonSizeDefinitionProperty changes
    internal static void OnSizeDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Find parent group box
        var groupBox = FindParentRibbonGroupBox(d);
        var element = (UIElement)d;
        var isSimplified = groupBox?.IsSimplified ?? false;

        if (!isSimplified)
        {
            SetAppropriateSize(element, groupBox?.State ?? RibbonGroupBoxState.Large, isSimplified);
        }
    }

    // Finds parent group box
    internal static RibbonGroupBox? FindParentRibbonGroupBox(DependencyObject element)
    {
        var currentElement = element;
        RibbonGroupBox? groupBox;

        while ((groupBox = currentElement as RibbonGroupBox) is null)
        {
            currentElement = VisualTreeHelper.GetParent(currentElement)
                             ?? LogicalTreeHelper.GetParent(currentElement);

            if (currentElement is null)
            {
                break;
            }
        }

        return groupBox;
    }

    /// <summary>
    /// Sets appropriate size of the control according to the
    /// given group box state and control's size definition
    /// </summary>
    /// <param name="element">UI Element</param>
    /// <param name="state">Group box state</param>
    /// <param name="isSimplified">Group box isSimplified state</param>
    public static void SetAppropriateSize(DependencyObject element, RibbonGroupBoxState state, bool isSimplified)
    {
        var sizeDefinition = isSimplified ? GetSimplifiedSizeDefinition(element) : GetSizeDefinition(element);
        SetSize(element, sizeDefinition.GetSize(state));
    }

    #endregion

    #region SimplifiedSizeDefinition Property

    /// <summary>
    /// Using a DependencyProperty as the backing store for SimplifiedSizeDefinition.
    /// This enables animation, styling, binding, etc...
    /// </summary>
    public static readonly DependencyProperty SimplifiedSizeDefinitionProperty =
        DependencyProperty.RegisterAttached(nameof(ISimplifiedRibbonControl.SimplifiedSizeDefinition), typeof(RibbonControlSizeDefinition), typeof(RibbonProperties),
            new FrameworkPropertyMetadata(new RibbonControlSizeDefinition(RibbonControlSize.Large, RibbonControlSize.Middle, RibbonControlSize.Small),
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.AffectsMeasure |
                FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsParentArrange |
                FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                OnSimplifiedSizeDefinitionChanged));

    /// <summary>
    /// Sets <see cref="SimplifiedSizeDefinitionProperty"/> for <paramref name="element"/>.
    /// </summary>
    public static void SetSimplifiedSizeDefinition(DependencyObject element, RibbonControlSizeDefinition value)
    {
        element.SetValue(SimplifiedSizeDefinitionProperty, value);
    }

    /// <summary>
    /// Gets <see cref="SimplifiedSizeDefinitionProperty"/> for <paramref name="element"/>.
    /// </summary>
    //[AttachedPropertyBrowsableForType(typeof(ISimplifiedRibbonControl))]
    public static RibbonControlSizeDefinition GetSimplifiedSizeDefinition(DependencyObject element)
    {
        return (RibbonControlSizeDefinition)element.GetValue(SimplifiedSizeDefinitionProperty);
    }

    // Handles RibbonSizeDefinitionProperty changes
    internal static void OnSimplifiedSizeDefinitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Find parent group box
        var groupBox = FindParentRibbonGroupBox(d);
        var element = (UIElement)d;
        var isSimplified = groupBox?.IsSimplified ?? false;

        if (isSimplified)
        {
            SetAppropriateSize(element, groupBox?.State ?? RibbonGroupBoxState.Large, isSimplified);
        }
    }

    /// <summary>
    /// Sets appropriate size of the control according to the
    /// given ribbon control size and control's size definition
    /// </summary>
    /// <param name="element">UI Element</param>
    /// <param name="size">Ribbon control size before applying SizeDefinition</param>
    public static void SetAppropriateSize(DependencyObject element, RibbonControlSize size)
    {
        SetSize(element, GetSizeDefinition(element).GetSize(size));
    }

    #endregion

    #region MouseOverBackgroundProperty

    /// <summary>
    /// <see cref="DependencyProperty"/> for specifying MouseOverBackground.
    /// </summary>
    public static readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.RegisterAttached("MouseOverBackground", typeof(Brush), typeof(RibbonProperties), new PropertyMetadata(default(Brush)));

    /// <summary>
    /// Sets <see cref="MouseOverBackgroundProperty"/> for <paramref name="element"/>.
    /// </summary>
    public static void SetMouseOverBackground(DependencyObject element, Brush? value)
    {
        element.SetValue(MouseOverBackgroundProperty, value);
    }

    /// <summary>
    /// Gets <see cref="MouseOverBackgroundProperty"/> for <paramref name="element"/>.
    /// </summary>
    //[AttachedPropertyBrowsableForType(typeof(IRibbonControl))]
    public static Brush? GetMouseOverBackground(DependencyObject element)
    {
        return (Brush)element.GetValue(MouseOverBackgroundProperty);
    }

    #endregion

    #region MouseOverForegroundProperty

    /// <summary>
    /// <see cref="DependencyProperty"/> for specifying MouseOverForeground.
    /// </summary>
    public static readonly DependencyProperty MouseOverForegroundProperty = DependencyProperty.RegisterAttached("MouseOverForeground", typeof(Brush), typeof(RibbonProperties), new PropertyMetadata(default(Brush)));

    /// <summary>
    /// Sets <see cref="MouseOverForegroundProperty"/> for <paramref name="element"/>.
    /// </summary>
    public static void SetMouseOverForeground(DependencyObject element, Brush? value)
    {
        element.SetValue(MouseOverForegroundProperty, value);
    }

    /// <summary>
    /// Gets <see cref="MouseOverForegroundProperty"/> for <paramref name="element"/>.
    /// </summary>
    //[AttachedPropertyBrowsableForType(typeof(IRibbonControl))]
    public static Brush? GetMouseOverForeground(DependencyObject element)
    {
        return (Brush?)element.GetValue(MouseOverForegroundProperty);
    }

    #endregion

    #region IsSelectedBackgroundProperty

    /// <summary>
    /// <see cref="DependencyProperty"/> for specifying IsSelectedBackground.
    /// </summary>
    public static readonly DependencyProperty IsSelectedBackgroundProperty = DependencyProperty.RegisterAttached("IsSelectedBackground", typeof(Brush), typeof(RibbonProperties), new PropertyMetadata(default(Brush)));

    /// <summary>
    /// Sets <see cref="IsSelectedBackgroundProperty"/> for <paramref name="element"/>.
    /// </summary>
    public static void SetIsSelectedBackground(DependencyObject element, Brush? value)
    {
        element.SetValue(IsSelectedBackgroundProperty, value);
    }

    /// <summary>
    /// Gets <see cref="IsSelectedBackgroundProperty"/> for <paramref name="element"/>.
    /// </summary>
    //[AttachedPropertyBrowsableForType(typeof(IRibbonControl))]
    public static Brush? GetIsSelectedBackground(DependencyObject element)
    {
        return (Brush?)element.GetValue(IsSelectedBackgroundProperty);
    }

    #endregion

    #region LastVisibleWidthProperty

    /// <summary>
    /// Stores the last visible width of an element.
    /// </summary>
    public static readonly DependencyProperty LastVisibleWidthProperty = DependencyProperty.RegisterAttached(
        "LastVisibleWidth", typeof(double), typeof(RibbonProperties), new PropertyMetadata(DoubleBoxes.Zero));

    /// <summary>Helper for setting <see cref="LastVisibleWidthProperty"/> on <paramref name="element"/>.</summary>
    public static void SetLastVisibleWidth(DependencyObject element, double value)
    {
        element.SetValue(LastVisibleWidthProperty, value);
    }

    /// <summary>Helper for getting <see cref="LastVisibleWidthProperty"/> on <paramref name="element"/>.</summary>
    public static double GetLastVisibleWidth(DependencyObject? element)
    {
#pragma warning disable WPF0042 // Avoid side effects in CLR accessors.
        if (element is null)
        {
            return 0;
        }
#pragma warning restore WPF0042 // Avoid side effects in CLR accessors.

        return (double)element.GetValue(LastVisibleWidthProperty);
    }

    #endregion LastVisibleWidthProperty

    #region IsElementInQuickAccessToolBarProperty

    /// <summary>
    /// Defines if the element is part of the <see cref="QuickAccessToolBar"/>.
    /// </summary>
    public static readonly DependencyProperty IsElementInQuickAccessToolBarProperty = DependencyProperty.RegisterAttached(
        "IsElementInQuickAccessToolBar", typeof(bool), typeof(RibbonProperties), new PropertyMetadata(BooleanBoxes.FalseBox));

    /// <summary>Helper for setting <see cref="IsElementInQuickAccessToolBarProperty"/> on <paramref name="element"/>.</summary>
    public static void SetIsElementInQuickAccessToolBar(DependencyObject element, bool value)
    {
        element.SetValue(IsElementInQuickAccessToolBarProperty, BooleanBoxes.Box(value));
    }

    /// <summary>Helper for getting <see cref="IsElementInQuickAccessToolBarProperty"/> on <paramref name="element"/>.</summary>
    public static bool GetIsElementInQuickAccessToolBar(DependencyObject element)
    {
        return (bool)element.GetValue(IsElementInQuickAccessToolBarProperty);
    }

    #endregion IsElementInQuickAccessToolBarProperty

    #region DesiredIconSize

#pragma warning disable WPF0010
    /// <summary>
    /// Defines the desired icon size for the element.
    /// </summary>
    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.RegisterAttached(
        "IconSize", typeof(IconSize), typeof(RibbonProperties), new PropertyMetadata(IconSizeBoxes.Small));
#pragma warning restore WPF0010

    /// <summary>Helper for setting <see cref="IconSizeProperty"/> on <paramref name="element"/>.</summary>
    public static void SetIconSize(DependencyObject element, IconSize value)
    {
        element.SetValue(IconSizeProperty, IconSizeBoxes.Box(value));
    }

    /// <summary>Helper for getting <see cref="IconSizeProperty"/> from <paramref name="element"/>.</summary>
    [AttachedPropertyBrowsableForType(typeof(IRibbonControl))]
    [AttachedPropertyBrowsableForType(typeof(IMediumIconProvider))]
    [AttachedPropertyBrowsableForType(typeof(ILargeIconProvider))]
    public static IconSize GetIconSize(DependencyObject element)
    {
        return (IconSize)element.GetValue(IconSizeProperty);
    }

    #endregion

    #region IsReadOnly

    /// <summary>
    /// Defines the read only property for the element.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.RegisterAttached("IsReadOnly", typeof(bool), typeof(RibbonProperties), new FrameworkPropertyMetadata(false, OnIsReadOnlyChanged));

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement frameworkElement)
        {
            if ((bool)e.NewValue == true)
            {
                VisualStateManager.GoToState(frameworkElement, "Disabled", true);
            }
            else
            {
                VisualStateManager.GoToState(frameworkElement, "Normal", true);
            }
        }
    }

    /// <summary>Helper for setting <see cref="IsReadOnlyProperty"/> on <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to set <see cref="IsReadOnlyProperty"/> on.</param>
    /// <param name="value">IsReadOnly property value.</param>
    public static void SetIsReadOnly(DependencyObject element, bool value)
    {
        element.SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>Helper for getting <see cref="IsReadOnlyProperty"/> from <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to read <see cref="IsReadOnlyProperty"/> from.</param>
    /// <returns>IsReadOnly property value.</returns>
    [AttachedPropertyBrowsableForType(typeof(IReadOnlyControl))]
    public static bool GetIsReadOnly(DependencyObject element)
    {
        return (bool)element.GetValue(IsReadOnlyProperty);
    }

    #endregion

    #region CommandCanExectue

    internal static readonly DependencyProperty CommandCanExectueProperty =
        DependencyProperty.RegisterAttached("CommandCanExectue", typeof(CommandCanExecuteChangedHelper), typeof(RibbonProperties));

    /// <summary>Helper for setting <see cref="CommandCanExectueProperty"/> on <paramref name="element"/>.</summary>
    /// <param name="element"><see cref="DependencyObject"/> to set <see cref="CommandCanExectueProperty"/> on.</param>
    /// <param name="value">CommandCanExectue property value.</param>
    internal static void SetCommandCanExectue(DependencyObject element, CommandCanExecuteChangedHelper? value)
    {
        element.SetValue(CommandCanExectueProperty, value);
    }

    internal static CommandCanExecuteChangedHelper? GetCommandCanExectue(DependencyObject element)
    {
        return element?.GetValue(CommandCanExectueProperty) as CommandCanExecuteChangedHelper;
    }

    internal static void OnCommandChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
    {
        if (GetCommandCanExectue(element) is CommandCanExecuteChangedHelper oldHelper)
        {
            oldHelper.UnRegisterCommand();
        }

        if (e.NewValue is ICommand newCommand
          && element is IReadOnlyControl ribbonControl)
        {
            var newHelper = new CommandCanExecuteChangedHelper(ribbonControl, newCommand);
            SetCommandCanExectue(element, newHelper);
            newHelper.RegisterCommand();
        }
    }

    #endregion
}