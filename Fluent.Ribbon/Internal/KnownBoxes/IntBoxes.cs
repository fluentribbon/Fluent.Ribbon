namespace Fluent.Internal.KnownBoxes
{
    /// <summary>
    /// Class containing boxed values for <see cref="int"/>.
    /// </summary>
    internal static class IntBoxes
    {
        /// <summary>
        /// Gets a boxed value for <c>0</c>.
        /// </summary>
        internal static readonly object Zero = 0;

        /// <summary>
        /// Gets a boxed value for <see cref="int.MaxValue"/>.
        /// </summary>
        internal static readonly object MaxValue = int.MaxValue;
    }
}