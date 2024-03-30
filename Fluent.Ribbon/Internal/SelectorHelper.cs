namespace Fluent.Internal;

using System;
using System.Reflection;
using System.Windows.Controls.Primitives;

internal static class SelectorHelper
{
    private static readonly PropertyInfo? canSelectMultiplePropertyInfo = typeof(Selector).GetProperty("CanSelectMultiple", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void SetCanSelectMultiple(Selector selector, bool value)
    {
        if (canSelectMultiplePropertyInfo is null)
        {
            throw new MissingMemberException(nameof(Selector), "CanSelectMultiple");
        }

        canSelectMultiplePropertyInfo.SetValue(selector, value);
    }
}