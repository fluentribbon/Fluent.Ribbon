# NOTE: This page is not finished

As i got little time to write a full documentation (feel free to help me with that) i would strongly advice to have a look at the showcase/sample application which shows the usage of most of the features available in the library.

# Version 3.0
## Major changes
* Office 2013 theme is now included
* MVVM support got better (yet not complete, please file a bug for things you need to work)
* Samples are not divided anymore (all features shown there are now present in the showcase application)
* All resource reference are now of type "DynamicResource" see comments for changeset https://fluent.codeplex.com/SourceControl/changeset/3572af781b96
* We now use the WindowChrome class provided by Microsoft instead of custom code to render in the non client area
* Large amount of [fixed bugs](https://fluent.codeplex.com/workitem/list/advanced?keyword=&status=Resolved%7cClosed&type=All&priority=All&release=All&assignedTo=All&component=All&reasonClosed=Fixed&sortField=LastUpdatedDate&sortDirection=Descending&page=0)
* Style resources have been improved. That means you now have to include "Themes/Generic.xaml" to get Office 2010 silver and you can just import "Themes/Office2010/Black.xaml" etc. afterwards to get the different colors. To use the Office 2013 theme you can omit "Themes/Generic.xaml" and include "Themes/Office2013/Generic.xaml" instead. You can use RibbonWindow for Office 2010 or Office 2013 themes and you can switch between those at runtime. The showcase application shows how you can do that.

## Breaking changes
* ~~DropDownButton now inherits from ItemsControl instead of MenuBase, this makes it necessary to wrap MenuItems in a Menu when used there (as it is done anywhere else in WPF). Inheriting from MenuBase caused a lot of problems. There are samples in the showcase application which show the correct/new usage. DropDownMenuStyle is the keyword you should look for.~~
* No control in this library sets IsFocusScope=True anymore. This means that ApplicationCommands (such as paste, cut or copy), when bound to a button, don't get enabled when you would expect them to be activated. The showcase application uses those buttons with IsFocusScope=True.

## Known issues
* ~~The window width is one px less than it should be to avoid a nasty bug regarding maximized windows. So in case your resolution is 1920x1080 it's 1919x1050 (-30 height to take taskbar into account). Please see https://fluent.codeplex.com/workitem/22490~~
* DropDownButton menu breaks Windows Forms keys https://fluent.codeplex.com/workitem/22472. This is because we inherit from MenuBase which does some nasty state management to which we couldn't find a way to work around in our library. Please use the workaround described in the issue if you need it to work.
