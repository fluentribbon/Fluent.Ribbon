namespace Fluent.Extensions
{
    using System.Reflection;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;

    /// <summary>
    /// Extensions for <see cref="SelectorAutomationPeer"/>.
    /// </summary>
    public static class SelectorAutomationPeerExtensions
    {
        private static readonly MethodInfo? raiseSelectionEventsMethodInfo = typeof(SelectorAutomationPeer).GetMethod("RaiseSelectionEvents", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        /// <summary>
        /// Calls the internal method "RaiseSelectionEvents" on <paramref name="peer"/> and passes <paramref name="e"/> to it.
        /// </summary>
        public static void RaiseSelectionEvents(this SelectorAutomationPeer peer, SelectionChangedEventArgs e)
        {
            raiseSelectionEventsMethodInfo?.Invoke(peer, new object[] { e });
        }
    }
}