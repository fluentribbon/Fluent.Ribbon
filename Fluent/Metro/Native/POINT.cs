using System;
using System.Runtime.InteropServices;

namespace Fluent.Metro.Native
{
#pragma warning disable 1591
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        private int _x;
        private int _y;

        public POINT(int x, int y)
        {
            this._x = x;
            this._y = y;
        }

        public int X
        {
            get { return this._x; }
            set { this._x = value; }
        }

        public int Y
        {
            get { return this._y; }
            set { this._y = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj is POINT)
            {
                var point = (POINT)obj;

                return point._x == this._x && point._y == this._y;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return this._x.GetHashCode() ^ this._y.GetHashCode();
        }

        public static bool operator ==(POINT a, POINT b)
        {
            return a._x == b._x && a._y == b._y;
        }

        public static bool operator !=(POINT a, POINT b)
        {
            return !(a == b);
        }

        public override string ToString()
        {
            return "POINT { x: " + this._x + " / y : " + this._y + " }";
        }
    }
}