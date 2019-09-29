namespace Fluent.Helpers
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class DoubleHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFinite(double value)
        {
#if NETCOREAPP3_0
            return double.IsFinite(value);
#else
            // Copied from https://source.dot.net/#System.Private.CoreLib/shared/System/Double.cs,02ee32eb42d32941
            var bits = BitConverter.DoubleToInt64Bits(value);
            return (bits & 0x7FFFFFFFFFFFFFFF) < 0x7FF0000000000000;
#endif
        }
    }
}