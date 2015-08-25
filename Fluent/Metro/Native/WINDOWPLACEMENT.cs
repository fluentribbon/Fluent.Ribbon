using System;
using System.Runtime.InteropServices;

namespace Fluent.Metro.Native
{
#pragma warning disable 1591
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPLACEMENT
    {
        public int length;

        public int flags;

        public int showCmd;

        public POINT minPosition;

        public POINT maxPosition;

        public RECT normalPosition;

        public override string ToString()
        {
            return string.Format(
                "WINDOWPLACEMENT\n{{\nlength: {0}\nflags: {1}\nshowCmd: {2}\nminPosition: {3}\nmaxPosition: {4}\nnormalPosition: {5}\n}}"
                , this.length, this.flags, this.showCmd, this.minPosition, this.maxPosition, this.normalPosition);
        }
    }
}