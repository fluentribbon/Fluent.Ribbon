namespace Fluent.Extensions
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Automation.Peers;

    /// <summary>
    /// Extension methods for <see cref="AutomationPeer"/>.
    /// </summary>
    internal static class AutomationPeerExtensions
    {
        private static readonly MethodInfo forceEnsureChildrenMethodInfo = typeof(AutomationPeer).GetMethod("ForceEnsureChildren", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo getWrapperPeerMethodInfo = typeof(ItemAutomationPeer).GetMethod("GetWrapperPeer", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo getWrapperMethodInfo = typeof(ItemAutomationPeer).GetMethod("GetWrapper", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static void ForceEnsureChildren(this AutomationPeer automationPeer)
        {
            forceEnsureChildrenMethodInfo.Invoke(automationPeer, null);
        }

        internal static AutomationPeer GetWrapperPeer(this ItemAutomationPeer automationPeer)
        {
            return (AutomationPeer)getWrapperPeerMethodInfo.Invoke(automationPeer, null);
        }

        internal static UIElement GetWrapper(this ItemAutomationPeer automationPeer)
        {
            return (UIElement)getWrapperMethodInfo.Invoke(automationPeer, null);
        }
    }
}
