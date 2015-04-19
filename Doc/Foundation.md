# Foundation

## Windows

Let's learn how to create a simple window with basic elements, such as `RibbonTabItem`, `RibbonGroupBox`, _Quick Access Toolbar_ and so on.

![Ribbon Window with Aero style](https://github.com/fluentribbon/Fluent.Ribbon/blob/master/Images/Screenshots/Office 2010 - Silver.png)

As you can see, the window above is not a usual WPF window: it has _aero_ style, and some elements are located in the title bar area. To achieve this, you need to use `Fluent.RibbonWindow`. `RibbonWindow` is designed to provide proper _office_-like _glass_ style. `RibbonWindow` will automatically use a special non-DWM style on Windows XP or the _basic_ Windows 7/Vista theme (see below).

![Ribbon Window with non-DWM style](https://github.com/fluentribbon/Fluent.Ribbon/blob/master/Images/Screenshots/Office 2010 - Silver - Non DWM.png)

One of the ways to create `RibbonWindow` is to create a regular WPF window and change `System.Windows.Window` to `Fluent.RibbonWindow` in the code and XAML. Your code and XAML will something like this:

```csharp
/// <summary>
/// Represents the main window of the application
/// </summary>
public partial class Window : RibbonWindow
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Window()
    {
        InitializeComponent();
    }
}
```

```xaml
<Fluent:RibbonWindow x:Class="Fluent.Sample.Foundation.Window"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:Fluent="clr-namespace:Fluent;assembly=Fluent"
    Title="Fluent.Sample.Foundation" Width="500" Height="250" >
    <Grid>
    </Grid>
</Fluent:RibbonWindow>
```

BEWARE: You also need to add one of the themes in `App.xaml`, for example:

```xaml
<Application.Resources>
    <!--Attach Default Fluent Control's Theme-->
    <ResourceDictionary Source="pack://application:,,,/Fluent;
        Component/Themes/Office2010/Silver.xaml" />
</Application.Resources>
```

## Ribbon

Let's add a `Ribbon` to the `Window`:

```xaml
<Fluent:Ribbon>
    <!--Backstage-->
    <Fluent:Ribbon.Menu>
        <Fluent:Backstage>
        </Fluent:Backstage>
    </Fluent:Ribbon.Menu>
    <!--Tabs-->
    <Fluent:RibbonTabItem Header="Tab">
        <Fluent:RibbonGroupBox Header="Group">
            <Fluent:Button Name="buttonGreen" Header="Green"
                           Icon="Images\Green.png"
                           LargeIcon="Images\GreenLarge.png" />
            <Fluent:Button Name="buttonGray" Header="Grey" Icon="Images\Gray.png"
                           LargeIcon="Images\GrayLarge.png" />
        </Fluent:RibbonGroupBox>
    </Fluent:RibbonTabItem>
</Fluent:Ribbon>
```

The code above produces the ribbon with a single tab, a group and two buttons.

## Quick Access Toolbar

_Quick Access Toolbar_ is a toolbar with shortcut controls. You can add any Fluent-based control using the context menu (a control has to implement the `IQuickAccessToolbarItem` interface). You can also pin controls to the _Quick Access Toolbar_ menu.

![Quick Access Toolbar](https://github.com/fluentribbon/Fluent.Ribbon/blob/master/Images/Screenshots/Office 2010 - Silver - ComboBox in QuickAccess.png)

You can add pinned _Quick Access Items_ named `QuickAccessMenuItem` to the collection `Ribbon.QuickAccessItems`. To associate a target element you may bind it to the `QuickAccessMenuItem.Target` property or just set the content.

```xaml
<!--Quick Access Toolbar Items-->
<Fluent:Ribbon.QuickAccessItems>
    <!--Use Content or Target Property to set QAT item-->
    <Fluent:QuickAccessMenuItem IsChecked="true">
        <Fluent:Button Header="Pink" Icon="Images\Pink.png" />
    </Fluent:QuickAccessMenuItem>
    <!--You Can Just Bind with Any Control-->
    <Fluent:QuickAccessMenuItem Target="{Binding ElementName=buttonGreen}"/>
</Fluent:Ribbon.QuickAccessItems>
```

## Backstage

The _File_ button in the top-left corner opens an empty backstage. Let's fill the backstage with items:

```xaml
<!--Backstage-->
<Fluent:Ribbon.Menu>
    <Fluent:Backstage>
        <Fluent:BackstageTabControl>
            <Fluent:BackstageTabItem Header="New"/>
            <Fluent:BackstageTabItem Header="Print"/>
            <Fluent:Button Header="Blue" Icon="Images\Blue.png"/>
        </Fluent:BackstageTabControl>
    </Fluent:Backstage>
</Fluent:Ribbon.Menu>
```

![Backstage and Contextual Tabs](https://github.com/fluentribbon/Fluent.Ribbon/blob/master/Images/Screenshots/Office 2010 - Silver - Backstage.png)

## Contextual Tabs

The last step is to add a contextual tab. A contextual tab is visible when a particular object in an app is selected. Contextual tabs cannot exist outside a contextual tab group, so we need to create a contextual tab group (`RibbonContextualTabGroup`) and bind a tab to this group. `RibbonContextualTabGroup` needs to be added to the `Ribbon.ContextualGroups` collection:

```xaml
<!--Contextual Tab Groups-->
<Fluent:Ribbon.ContextualGroups>
    <Fluent:RibbonContextualTabGroup Header="Tools" Visibility="Visible"
        x:Name="toolsGroup" Background="Green" BorderBrush="Green" />
</Fluent:Ribbon.ContextualGroups>
```

And associate a tab to this group:

```xaml
<!--Contextual Tabs-->
<Fluent:RibbonTabItem Header="CT" Group="{Binding ElementName=toolsGroup}"/>
```

`RibbonContextualTabGroup` is not visible by default. To show or hide a contextual tab you must set the `RibbonContextualTabGroup.Visibility` property to `Visible` or `Collapsed`.
