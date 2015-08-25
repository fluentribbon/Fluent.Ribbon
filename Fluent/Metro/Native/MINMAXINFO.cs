using System.Runtime.InteropServices;

namespace Fluent.Metro.Native
{
#pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential)]
    internal struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;

        public override string ToString()
        {
            return string.Format("MINMAXINFO {{ ptMaxPosition : {0} / ptMaxSize : {1} }}", this.ptMaxPosition, this.ptMaxSize);
        }
    };
}