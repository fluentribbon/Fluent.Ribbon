namespace Fluent.Tests.Controls;

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using NUnit.Framework;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using Selector = System.Windows.Controls.Primitives.Selector;

/// <summary>
/// Tests for <see cref="RibbonControl.BindQuickAccessItem"/>.
/// </summary>
[TestFixture]
public class RibbonControlBindQuickAccessItemTests
{
    [Test]
    public void Binds_FrameworkElement_Properties()
    {
        var source = new Button();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        var dataContext = new object();
        var fontFamily = new FontFamily("Consolas");

        source.DataContext = dataContext;
        source.FontFamily = fontFamily;
        source.FontSize = 42;
        source.FontStretch = FontStretches.Condensed;
        source.FontStyle = FontStyles.Italic;
        source.FontWeight = FontWeights.Bold;
        source.Foreground = Brushes.Red;
        source.IsEnabled = false;
        source.Opacity = 0.5;
        source.SnapsToDevicePixels = true;
        FocusManager.SetIsFocusScope(source, true);
        InputControlProperties.SetInputMinWidth(source, 11);
        InputControlProperties.SetInputWidth(source, 22);
        InputControlProperties.SetInputHeight(source, 33);

        Assert.That(target.DataContext, Is.SameAs(dataContext));
        Assert.That(target.FontFamily, Is.SameAs(fontFamily));
        Assert.That(target.FontSize, Is.EqualTo(42));
        Assert.That(target.FontStretch, Is.EqualTo(FontStretches.Condensed));
        Assert.That(target.FontStyle, Is.EqualTo(FontStyles.Italic));
        Assert.That(target.FontWeight, Is.EqualTo(FontWeights.Bold));
        Assert.That(target.Foreground, Is.SameAs(Brushes.Red));
        Assert.That(target.IsEnabled, Is.False);
        Assert.That(target.Opacity, Is.EqualTo(0.5));
        Assert.That(target.SnapsToDevicePixels, Is.True);
        Assert.That(FocusManager.GetIsFocusScope(target), Is.True);
        Assert.That(InputControlProperties.GetInputMinWidth(target), Is.EqualTo(11));
        Assert.That(InputControlProperties.GetInputWidth(target), Is.EqualTo(22));
        Assert.That(InputControlProperties.GetInputHeight(target), Is.EqualTo(33));
    }

    [Test]
    public void Binds_Command_Properties_For_CommandSource()
    {
        var source = new Button();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        var command = new RoutedCommand();
        var commandParameter = new object();
        var commandTarget = new Button();

        source.Command = command;
        source.CommandParameter = commandParameter;
        source.CommandTarget = commandTarget;

        Assert.That(target.Command, Is.SameAs(command));
        Assert.That(target.CommandParameter, Is.SameAs(commandParameter));
        Assert.That(target.CommandTarget, Is.SameAs(commandTarget));
    }

    [Test]
    public void Binds_Command_Properties_For_MenuItem()
    {
        var source = new MenuItem();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        var command = new RoutedCommand();
        var commandParameter = new object();
        var commandTarget = new Button();

        source.Command = command;
        source.CommandParameter = commandParameter;
        source.CommandTarget = commandTarget;

        Assert.That(target.Command, Is.SameAs(command));
        Assert.That(target.CommandParameter, Is.SameAs(commandParameter));
        Assert.That(target.CommandTarget, Is.SameAs(commandTarget));
    }

    [Test]
    public void Does_Not_Bind_Command_Properties_For_Non_CommandSource()
    {
        var source = new TextBox();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(BindingOperations.IsDataBound(target, ButtonBase.CommandProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, ButtonBase.CommandParameterProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, ButtonBase.CommandTargetProperty), Is.False);
    }

    [TestCase(typeof(ToggleButton))]
    [TestCase(typeof(CheckBox))]
    [TestCase(typeof(RadioButton))]
    public void Binds_IsChecked_TwoWay_For_ToggleButtons(Type toggleButtonType)
    {
        var source = (System.Windows.Controls.Primitives.ToggleButton)Activator.CreateInstance(toggleButtonType);
        var target = (System.Windows.Controls.Primitives.ToggleButton)Activator.CreateInstance(toggleButtonType);

        RibbonControl.BindQuickAccessItem(source, target);

        source.IsChecked = true;
        Assert.That(target.IsChecked, Is.True);

        target.IsChecked = false;
        Assert.That(source.IsChecked, Is.False);
    }

    [Test]
    public void Does_Not_Bind_IsChecked_When_Target_Is_Not_A_ToggleButton()
    {
        var source = new ToggleButton();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(BindingOperations.IsDataBound(target, System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty), Is.False);
    }

    [Test]
    public void Binds_ItemsControl_Properties()
    {
        var source = new DropDownButton();
        var target = new DropDownButton();

        RibbonControl.BindQuickAccessItem(source, target);

        var itemBindingGroup = new BindingGroup();
        var itemTemplate = new DataTemplate();
        var itemTemplateSelector = new System.Windows.Controls.DataTemplateSelector();
        var itemsPanel = new System.Windows.Controls.ItemsPanelTemplate();
        var itemContainerStyle = new Style();
        var itemContainerStyleSelector = new System.Windows.Controls.StyleSelector();
        System.Windows.Controls.GroupStyleSelector groupStyleSelector = (_, _) => null;

        source.AlternationCount = 3;
        source.ItemBindingGroup = itemBindingGroup;
        source.ItemTemplate = itemTemplate;
        source.ItemTemplateSelector = itemTemplateSelector;
        source.ItemsPanel = itemsPanel;
        source.ItemContainerStyle = itemContainerStyle;
        source.ItemContainerStyleSelector = itemContainerStyleSelector;
        source.GroupStyleSelector = groupStyleSelector;

        Assert.That(target.AlternationCount, Is.EqualTo(3));
        Assert.That(target.ItemBindingGroup, Is.SameAs(itemBindingGroup));
        Assert.That(target.ItemTemplate, Is.SameAs(itemTemplate));
        Assert.That(target.ItemTemplateSelector, Is.SameAs(itemTemplateSelector));
        Assert.That(target.ItemsPanel, Is.SameAs(itemsPanel));
        Assert.That(target.ItemContainerStyle, Is.SameAs(itemContainerStyle));
        Assert.That(target.ItemContainerStyleSelector, Is.SameAs(itemContainerStyleSelector));
        Assert.That(target.GroupStyleSelector, Is.SameAs(groupStyleSelector));
    }

    [Test]
    public void Binds_DisplayMemberPath_And_ItemStringFormat_For_ItemsControl()
    {
        // DisplayMemberPath and ItemStringFormat make WPF replace ItemTemplateSelector, so they are tested separately
        var source = new DropDownButton();
        var target = new DropDownButton();

        RibbonControl.BindQuickAccessItem(source, target);

        source.DisplayMemberPath = "DisplayMember";
        source.ItemStringFormat = "Format {0}";

        Assert.That(target.DisplayMemberPath, Is.EqualTo("DisplayMember"));
        Assert.That(target.ItemStringFormat, Is.EqualTo("Format {0}"));
    }

    [Test]
    public void Syncs_GroupStyle_For_ItemsControl()
    {
        var existingGroupStyle = new System.Windows.Controls.GroupStyle();
        var source = new DropDownButton();
        source.GroupStyle.Add(existingGroupStyle);

        var target = new DropDownButton();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(target.GroupStyle, Is.EqualTo(new[] { existingGroupStyle }));

        var addedGroupStyle = new System.Windows.Controls.GroupStyle();
        source.GroupStyle.Add(addedGroupStyle);

        Assert.That(target.GroupStyle, Is.EqualTo(new[] { existingGroupStyle, addedGroupStyle }));

        source.GroupStyle.Remove(existingGroupStyle);

        Assert.That(target.GroupStyle, Is.EqualTo(new[] { addedGroupStyle }));
    }

    [Test]
    public void Binds_Selector_Properties()
    {
        var source = new ComboBox();
        var target = new ComboBox();

        RibbonControl.BindQuickAccessItem(source, target);

        source.SelectedValuePath = "SelectedValue";
        source.IsSynchronizedWithCurrentItem = true;

        Assert.That(target.SelectedValuePath, Is.EqualTo("SelectedValue"));
        Assert.That(target.IsSynchronizedWithCurrentItem, Is.True);
    }

    [Test]
    public void Does_Not_Bind_Selector_Properties_When_Target_Is_Not_A_Selector()
    {
        var source = new ComboBox();
        var target = new DropDownButton();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(BindingOperations.IsDataBound(target, System.Windows.Controls.ItemsControl.DisplayMemberPathProperty), Is.True);
        Assert.That(BindingOperations.IsDataBound(target, Selector.SelectedValuePathProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, Selector.IsSynchronizedWithCurrentItemProperty), Is.False);
    }

    [Test]
    public void Does_Not_Bind_ItemsControl_Properties_When_Target_Is_Not_An_ItemsControl()
    {
        var source = new DropDownButton();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(BindingOperations.IsDataBound(target, System.Windows.Controls.ItemsControl.DisplayMemberPathProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, System.Windows.Controls.ItemsControl.ItemTemplateProperty), Is.False);
    }

    [Test]
    public void Binds_Header_For_HeaderedControl()
    {
        var source = new Button();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        source.Header = "Header";

        Assert.That(target.Header, Is.EqualTo("Header"));
    }

    [Test]
    public void Binds_Header_Properties_For_HeaderedItemsControl()
    {
        var source = new RibbonGroupBox();
        var target = new RibbonGroupBox();

        RibbonControl.BindQuickAccessItem(source, target);

        var headerTemplate = new DataTemplate();
        var headerTemplateSelector = new System.Windows.Controls.DataTemplateSelector();

        source.Header = "Header";
        source.HeaderStringFormat = "Format {0}";
        source.HeaderTemplate = headerTemplate;
        source.HeaderTemplateSelector = headerTemplateSelector;

        Assert.That(target.Header, Is.EqualTo("Header"));
        Assert.That(target.HeaderStringFormat, Is.EqualTo("Format {0}"));
        Assert.That(target.HeaderTemplate, Is.SameAs(headerTemplate));
        Assert.That(target.HeaderTemplateSelector, Is.SameAs(headerTemplateSelector));
    }

    [Test]
    public void ToolTip_Falls_Back_To_Header_When_Source_Has_No_ToolTip()
    {
        var source = new Button
        {
            Header = "Header"
        };
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(target.ToolTip, Is.EqualTo("Header"));

        source.Header = "Changed";

        Assert.That(target.ToolTip, Is.EqualTo("Changed"));
    }

    [Test]
    public void ToolTip_Is_Bound_When_Source_Has_ToolTip()
    {
        var source = new Button
        {
            Header = "Header",
            ToolTip = "ToolTip"
        };
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(target.ToolTip, Is.EqualTo("ToolTip"));

        source.ToolTip = "Changed";

        Assert.That(target.ToolTip, Is.EqualTo("Changed"));
    }

    [Test]
    public void ToolTip_Is_Bound_When_Source_ToolTip_Is_DataBound()
    {
        var toolTipSource = new FrameworkElement();

        var source = new Button
        {
            Header = "Header"
        };
        source.SetBinding(FrameworkElement.ToolTipProperty, new Binding(nameof(FrameworkElement.Tag)) { Source = toolTipSource });

        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(target.ToolTip, Is.Null);

        toolTipSource.Tag = "ToolTip";

        Assert.That(target.ToolTip, Is.EqualTo("ToolTip"));
    }

    [Test]
    public void Does_Not_Bind_Header_And_ToolTip_For_Non_HeaderedControl()
    {
        var source = new System.Windows.Controls.Border
        {
            ToolTip = "ToolTip"
        };
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(BindingOperations.IsDataBound(target, RibbonControl.HeaderProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, FrameworkElement.ToolTipProperty), Is.False);
    }

    [Test]
    public void Binds_CustomIconSize()
    {
        var source = new Button();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        RibbonProperties.SetCustomIconSize(source, new Size(20, 21));

        Assert.That(RibbonProperties.GetCustomIconSize(target), Is.EqualTo(new Size(20, 21)));
    }

    [Test]
    public void Binds_Icons_When_Icons_Are_Not_Visuals()
    {
        var source = new Button
        {
            Icon = "Icon",
            MediumIcon = "MediumIcon",
            LargeIcon = "LargeIcon"
        };
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(target.Icon, Is.EqualTo("Icon"));
        Assert.That(target.MediumIcon, Is.EqualTo("MediumIcon"));
        Assert.That(target.LargeIcon, Is.EqualTo("LargeIcon"));

        source.Icon = "ChangedIcon";
        source.MediumIcon = "ChangedMediumIcon";
        source.LargeIcon = "ChangedLargeIcon";

        Assert.That(target.Icon, Is.EqualTo("ChangedIcon"));
        Assert.That(target.MediumIcon, Is.EqualTo("ChangedMediumIcon"));
        Assert.That(target.LargeIcon, Is.EqualTo("ChangedLargeIcon"));
    }

    [Test]
    public void Creates_Rectangles_When_Icons_Are_Visuals()
    {
        var icon = new System.Windows.Controls.Border();
        var mediumIcon = new System.Windows.Controls.Border();
        var largeIcon = new System.Windows.Controls.Border();

        var source = new Button
        {
            Icon = icon,
            MediumIcon = mediumIcon,
            LargeIcon = largeIcon
        };
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        AssertIsRectangleWithVisualBrush(target.Icon, icon, 16);
        AssertIsRectangleWithVisualBrush(target.MediumIcon, mediumIcon, 24);
        AssertIsRectangleWithVisualBrush(target.LargeIcon, largeIcon, 32);

        Assert.That(BindingOperations.IsDataBound(target, RibbonControl.IconProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, MediumIconProviderProperties.MediumIconProperty), Is.False);
        Assert.That(BindingOperations.IsDataBound(target, LargeIconProviderProperties.LargeIconProperty), Is.False);
    }

    [Test]
    public void Binds_QATIconSize_To_IconSize()
    {
        var source = new Button();
        var target = new Button();

        RibbonControl.BindQuickAccessItem(source, target);

        RibbonProperties.SetQATIconSize(source, IconSize.Medium);

        Assert.That(RibbonProperties.GetIconSize(target), Is.EqualTo(IconSize.Medium));
    }

    [Test]
    public void Sets_Size_To_Small()
    {
        var source = new Button
        {
            Size = RibbonControlSize.Large
        };
        var target = new Button
        {
            Size = RibbonControlSize.Large
        };

        RibbonControl.BindQuickAccessItem(source, target);

        Assert.That(target.Size, Is.EqualTo(RibbonControlSize.Small));
    }

    [Test]
    public void Works_For_Plain_FrameworkElements()
    {
        var dataContext = new object();

        var source = new System.Windows.Controls.Border
        {
            DataContext = dataContext
        };
        var target = new System.Windows.Controls.Border();

        Assert.That(() => RibbonControl.BindQuickAccessItem(source, target), Throws.Nothing);

        Assert.That(target.DataContext, Is.SameAs(dataContext));
        Assert.That(RibbonProperties.GetSize(target), Is.EqualTo(RibbonControlSize.Small));
    }

    internal static void AssertIsRectangleWithVisualBrush(object value, Visual expectedVisual, double expectedSize)
    {
        Assert.That(value, Is.TypeOf<Rectangle>());

        var rectangle = (Rectangle)value;

        Assert.That(rectangle.Width, Is.EqualTo(expectedSize));
        Assert.That(rectangle.Height, Is.EqualTo(expectedSize));
        Assert.That(rectangle.Fill, Is.TypeOf<VisualBrush>());
        Assert.That(((VisualBrush)rectangle.Fill).Visual, Is.SameAs(expectedVisual));
    }
}
