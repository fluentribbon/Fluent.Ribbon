namespace Fluent.Helpers
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls.Primitives;

    internal static class SelectorHelper
    {
        private static readonly MethodInfo raiseIsSelectedChangedAutomationEventMethodInfo = typeof(Selector).GetMethod("RaiseIsSelectedChangedAutomationEvent", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(DependencyObject), typeof(bool) }, null);

        internal static void RaiseIsSelectedChangedAutomationEvent(Selector selector, DependencyObject container, bool isSelected)
        {
            if (selector == null)
            {
                return;
            }

            raiseIsSelectedChangedAutomationEventMethodInfo.Invoke(selector, new object[] { container, isSelected });
        }
    }
}
