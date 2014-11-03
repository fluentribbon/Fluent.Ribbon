using System.Runtime.InteropServices;

namespace Fluent.Metro.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;

        public override string ToString()
        {
            return "MINMAXINFO { ptMaxPosition : " + ptMaxPosition + " / ptMaxSize : " + ptMaxSize + " }";
        }
    };
}