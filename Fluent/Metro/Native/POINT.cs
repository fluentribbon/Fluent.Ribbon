using System;
using System.Runtime.InteropServices;

namespace Fluent.Metro.Native
{
#pragma warning disable 1591
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        private int x;
        private int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is POINT)
            {
                var point = (POINT)obj;

                return point.x == this.x && point.y == this.y;
            }

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return this.x.GetHashCode() ^ this.y.GetHashCode();
        }

        public static bool operator ==(POINT a, POINT b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(POINT a, POINT b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return string.Format("POINT {{ x: {0} / y: {1} }}", this.X, this.Y);
        }
    }
}