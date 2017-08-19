namespace Fluent.Tests.Helper
{
    using System;
    using System.Reflection;

    public static class ReflectionHelper
    {
        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            return (T)GetPrivateFieldInfo(obj.GetType(), fieldName).GetValue(obj);
        }

        private static FieldInfo GetPrivateFieldInfo(Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}