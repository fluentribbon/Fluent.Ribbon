using System.Runtime.InteropServices;

namespace Fluent.Metro.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int leftWidth;
        public int rightWidth;
        public int topHeight;
        public int bottomHeight;
    }
}