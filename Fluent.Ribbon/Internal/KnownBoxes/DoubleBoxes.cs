namespace Fluent.Internal.KnownBoxes
{
    /// <summary>
    /// Class containing boxed values for <see cref="double"/>.
    /// </summary>
    internal static class DoubleBoxes
    {
        /// <summary>
        /// Gets a boxed value for <c>0.0D</c>.
        /// </summary>
        internal static readonly object Zero = 0.0D;

        /// <summary>
        /// Gets a boxed value for <see cref="double.NaN"/>.
        /// </summary>
        internal static readonly object NaN = double.NaN;

        /// <summary>
        /// Gets a boxed value for <see cref="double.MaxValue"/>.
        /// </summary>
        internal static readonly object MaxValue = double.MaxValue;
    }
}