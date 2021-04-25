namespace Fluent.Internal
{
    using System;

    internal static class TypeHelper
    {
        public static bool InheritsFrom(this Type type, string typeName)
        {
            var currentType = type;

            do
            {
                if (currentType.Name == typeName)
                {
                    return true;
                }

                currentType = currentType.BaseType;
            }
            while (currentType is not null
                     && currentType != typeof(object));

            return false;
        }
    }
}