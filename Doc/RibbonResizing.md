# Ribbon Resizing

A `RibbonGroupBox` can be in one of four states: `Large` ▻ `Middle` ▻ `Small` ▻ `Collapsed`.
By default, a `RibbonGroupBox` is in the ''Large'' state.
When a `RibbonGroupBox` changes its state, it changes the size of all its controls.

`RibbonTabItem` has a `ReduceOrder` property.
This property defines the order how the groups reduce.
You can enumerate group names from last to first to reduce.

All ribbon contols (`Button`, `DropDownButton`, `Spinner` and so on) have a `SizeDefinition` property.
You can define which size will be used when the group will be in a particular state.
For example, if you set `SizeDefinition = "Middle, Middle, Small"`, that means:

* Large state of the group -> Middle size of the control
* Middle state of the group -> Middle size of the control
* Small state of the group -> Small size of the control

```xaml
<Fluent:RibbonTabItem ReduceOrder="Default,Default,Default,
                                   Large,Large,Large,
                                   Other,Other,Other" ...>
    <!--By default ReduceOrder="Large, Middle, Small"-->
    <Fluent:RibbonGroupBox Name="Default" Header="Default Behaviour">
        <Fluent:Button ... />
        <Fluent:Button ... />
        <Fluent:Button ... />
        <Fluent:Button ... />
    </Fluent:RibbonGroupBox>
    <!--You can use short form
    (for ex, "Middle" is equal "Middle,Middle,Middle")-->
    <Fluent:RibbonGroupBox Name="Large" Header="Large Only">
        <Fluent:Button SizeDefinition="Large" .../>
        <Fluent:Button SizeDefinition="Large" .../>
        <Fluent:Button SizeDefinition="Large" .../>
        <Fluent:Button SizeDefinition="Large" .../>
    </Fluent:RibbonGroupBox>
    <Fluent:RibbonGroupBox Name="Other" Header="Other">
        <Fluent:Button SizeDefinition="Large, Large, Large" Icon="Images\Green.png" ... />
        <Fluent:Button SizeDefinition="Large, Large, Small" Icon="Images\Gray.png" ... />
        <Fluent:Button SizeDefinition="Middle, Small, Small" Icon="Images\Yellow.png" ... />
        <Fluent:Button SizeDefinition="Middle, Small, Small" Icon="Images\Brown.png" ... />
    </Fluent:RibbonGroupBox>
</Fluent:RibbonTabItem>
```

The pseudo code above produces the following reducing behavior:

![](https://github.com/fluentribbon/Fluent.Ribbon/blob/master/Images/Screenshots/Resizing.png)