# Screen Tips

To use screen tips you have to create a `Fluent.ScreenTip` instance and set the `ToolTip` property:

```xaml
<Fluent:Button ... >
    <Fluent:Button.ToolTip>
        <Fluent:ScreenTip Title="Gray"
                          HelpTopic="Help for Gray ScreenTip"
                          Image="Images\GrayLarge.png"
                          Text="This ScreenTip is ribbon aligned. &#x0a;
                                It has the image and handles F1."/>
    </Fluent:Button.ToolTip>
</Fluent:Button>
```

`ScreenTip` has a unique feature to invoke the contextual help when it is in the _open_ state.
To handle contextual help you must set `ScreenTip.HelpTopic` in XAML
and subscribe to the `ScreenTip.HelpPressed` event.
Be aware that the event is static,
so you must manually unsubscribe from it when it is appropriate to avoid memory leaks
or subscribe the application's lifetime object as shown below:

```csharp
public partial class Application : System.Windows.Application
{
    void OnStartup(object sender, StartupEventArgs e)
    {
        ScreenTip.HelpPressed += OnScreenTipHelpPressed;
    }

    /// <summary>
    /// Handles F1 pressed on ScreenTip with help capability
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    static void OnScreenTipHelpPressed(object sender, ScreenTipHelpEventArgs e)
    {
        // Show help according the given help topic
        // (here just show help topic as string)
        MessageBox.Show(e.HelpTopic.ToString());
    }
}
```

`ScreenTip` shows on disabled Fluent-based controls automatically.
Moreover you can set text which should be shown when the target control is disabled.

```xaml
<Fluent:Button IsEnabled="False" ... >
    <Fluent:Button.ToolTip>
        <Fluent:ScreenTip Title="Orange" Width ="250"
                          Image="Images\OrangeLarge.png"
                          Text="This control is disabled and has fixed width 250px"
                          HelpTopic="Help for Orange ScreenTip"
                          DisableReason="This control is disabled
                                         to show 'disable reason' section"/>
    </Fluent:Button.ToolTip>
</Fluent:Button>
```

You can find the main properties of `ScreenTip` in the table below.

Property                  | Description
------------------------- | ------------------------------------------------------------------------------------------
`ScreenTip.Title`         | The title of the screen tip
`ScreenTip.Text`          | The text of the screen tip
`ScreenTip.Image`         | Image
`ScreenTip.Width`         | Set this property if you want to make fixed sized screen tip.
`ScreenTip.DisableReason` | If the target control is disabled, this text will be shown to user.
`ScreenTip.HelpTopic`     | Set this property and subscribe to `ScreenTip.HelpPressed` to execute your contextual help.
