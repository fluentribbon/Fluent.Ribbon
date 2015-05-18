# Keyboard Access

## Key Tips

Key tips provide users accessibility to the Ribbon using the keyboard.
To start the process the user must press `Alt` or `F10`.
`KeyTips` are shown over controls.

To make key tips work, it is enough to set the attached property `Fluent:KeyTip.Keys` to the target control,
and the ribbon will arrange and show the key tips automatically.
It is possible to set key tips to menu and / or submenu items.
You also need to set a key tip for groups to open them while they are collapsed.

```xaml
<Fluent:RibbonGroupBox Fluent:KeyTip.Keys="ZC" ... >
    <Fluent:SplitButton Fluent:KeyTip.Keys="R" ... >
        <Fluent:MenuItem Fluent:KeyTip.Keys="P" ... />
        <Fluent:MenuItem Fluent:KeyTip.Keys="R" ... >
            <Fluent:MenuItem Fluent:KeyTip.Keys="O" ... />
        </Fluent:MenuItem>
    </Fluent:SplitButton>
    ...
```

As you can see, items in the Quick Access Toolbar are key tipped automatically.
Also key tips are placed well automatically.
However, there are cases when custom placement is required.
In this case you have to set `Fluent:KeyTip.AutoPlacement` to `false` and use additional attached properties:

Property                    | Description
--------------------------- | -----------------------------------------------------------------
`KeyTip.AutoPlacement`      | `true` by default. Set to `false` to switch on custom placement.
`KeyTip.Horizontal`         | Horizontal alignment relative to key tipped control
`KeyTip.VerticalAlignment`  | Vertical alignment relative to key tipped control
`KeyTip.Margin`             | Margin to offset key tip

```xaml
<Fluent:RibbonGroupBox Header="Group">
    <Fluent:Button Text="Center" LargeIcon="Images\GreenLarge.png"
                   Fluent:KeyTip.AutoPlacement="False"
                   Fluent:KeyTip.HorizontalAlignment="Center"
                   Fluent:KeyTip.VerticalAlignment="Center"
                   Fluent:KeyTip.Keys="C" />
    <Fluent:Button Text="Left" LargeIcon="Images\GrayLarge.png"
                   Fluent:KeyTip.AutoPlacement="False"
                   Fluent:KeyTip.HorizontalAlignment="Left"
                   Fluent:KeyTip.VerticalAlignment="Center"
                   Fluent:KeyTip.Keys="L" />
    <Fluent:Button Text="Top" LargeIcon="Images\YellowLarge.png"
                   Fluent:KeyTip.AutoPlacement="False"
                   Fluent:KeyTip.HorizontalAlignment="Center"
                   Fluent:KeyTip.VerticalAlignment="Top"
                   Fluent:KeyTip.Keys="T"/>
</Fluent:RibbonGroupBox>
```
