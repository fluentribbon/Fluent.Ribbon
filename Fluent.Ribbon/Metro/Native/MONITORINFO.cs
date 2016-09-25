using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Fluent.Metro.Native
{
    #pragma warning disable 1591
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class MONITORINFO
    {     
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor = new RECT(); 
        public RECT rcWork = new RECT();           
        public int dwFlags = 0;

        public enum MonitorOptions
        {
            MONITOR_DEFAULTTONULL = 0x00000000,
            MONITOR_DEFAULTTOPRIMARY = 0x00000001,
            MONITOR_DEFAULTTONEAREST = 0x00000002
        }
    }
}