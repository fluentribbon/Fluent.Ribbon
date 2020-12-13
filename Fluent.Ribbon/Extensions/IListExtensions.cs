namespace Fluent.Extensions
{
    using System.Collections;

    internal static class IListExtensions
    {
        private static readonly IList emptyReadOnlyList = ArrayList.ReadOnly(new ArrayList());

        public static IList NullSafe(this IList? list)
        {
            return list ?? emptyReadOnlyList;
        }
    }
}
