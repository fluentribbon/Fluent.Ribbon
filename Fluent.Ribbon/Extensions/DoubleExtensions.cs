namespace Fluent.Extensions
{
    internal static class DoubleExtensions
    {
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
}