namespace Fluent.Internal
{
    using System.Reflection;
    using System.Windows.Controls;

    internal static class AccessTextHelper
    {
        private static readonly MethodInfo? removeAccessKeyMarkerMethodInfo = typeof(AccessText).GetMethod("RemoveAccessKeyMarker", BindingFlags.Static | BindingFlags.NonPublic);

        public static string? RemoveAccessKeyMarker(string? input)
        {
            if (input is null)
            {
                return null;
            }

            return (string?)removeAccessKeyMarkerMethodInfo?.Invoke(null, new object[] { input });
        }
    }
}