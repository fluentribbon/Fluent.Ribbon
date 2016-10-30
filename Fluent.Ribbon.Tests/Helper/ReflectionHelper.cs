using System;

namespace Fluent.Tests.Helper
{
    using System.Reflection;

    public static class ReflectionHelper
    {
        public static object GetPrivateFieldValue(this object obj, string fieldName)
        {
            return GetPrivateFieldInfo(obj.GetType(), fieldName).GetValue(obj);
        }

        private static FieldInfo GetPrivateFieldInfo(Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}