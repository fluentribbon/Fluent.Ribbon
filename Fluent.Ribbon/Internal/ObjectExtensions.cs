namespace Fluent.Internal
{
    using System.Runtime.CompilerServices;
    using JetBrains.Annotations;

    internal static class ObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("obj:null => true")]
        public static bool IsNull(this object obj)
        {
            return obj is null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("obj:null => false")]
        public static bool IsNotNull(this object obj)
        {
            return !(obj is null);
        }
    }
}
