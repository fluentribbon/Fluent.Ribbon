namespace Fluent.Extensions;

using System.Windows;
using System.Windows.Automation.Peers;

internal static class UIElementExtensions
{
    public static AutomationPeer? GetOrCreateAutomationPeer(this UIElement element)
    {
        return UIElementAutomationPeer.FromElement(element)
               ?? UIElementAutomationPeer.CreatePeerForElement(element);
    }
}