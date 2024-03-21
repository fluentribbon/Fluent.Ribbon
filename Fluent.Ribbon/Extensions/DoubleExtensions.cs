namespace Fluent.Extensions;

using Fluent.Internal;

internal static class DoubleExtensions
{
    public static bool AlmostEquals(this double x, double y)
    {
        return DoubleUtil.AreClose(x, y);
    }

    public static double GetZeroIfInfinityOrNaN(this double doubleValue)
    {
        if (double.IsInfinity(doubleValue)
            || double.IsNaN(doubleValue))
        {
            return 0;
        }

        return doubleValue;
    }
}