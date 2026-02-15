namespace Fluent.Internal;

using System;
using System.Windows;

/// <summary>
/// Loading of "Styles.xaml" can be suppressed by setting AppContext switch "Switch.Fluent.Ribbon.DisableDefaultStyleLoading" to "true".
/// </summary>
/// <example>
/// &lt;ItemGroup&gt;
///  &lt;RuntimeHostConfigurationOption Include="Switch.Fluent.Ribbon.DisableDefaultStyleLoading" Value="true" /&gt;
/// &lt;/ItemGroup&gt;
/// </example>
internal class StylesResourceDictionary : ResourceDictionary
{
    public StylesResourceDictionary()
    {
        if (AppContext.TryGetSwitch("Switch.Fluent.Ribbon.DisableDefaultStyleLoading", out var enabled) is false
            || enabled is false)
        {
            this.Source = new("/Fluent;component/Themes/Styles.xaml", UriKind.Relative);
        }
    }
}