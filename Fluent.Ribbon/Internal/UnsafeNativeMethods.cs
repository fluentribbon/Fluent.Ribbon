namespace Fluent.Internal
{
    using System.Runtime.InteropServices;

    internal static class UnsafeNativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
    }
}