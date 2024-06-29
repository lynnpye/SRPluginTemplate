using System;
using System.Reflection;

namespace SRPlugin
{
    public class PrivateFieldAccessor
    {
        public static void SetPrivateFieldValue(object obj, string fieldName, object value)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found in type '{type.FullName}'.");
            }

            field.SetValue(obj, value);
        }

        public static T GetPrivateFieldValue<T>(object obj, string fieldName, T defaultValue)
        {
            Type type = obj.GetType();
            FieldInfo field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found in type '{type.FullName}'.");
            }
            object fieldValue = field.GetValue(obj);
            if (fieldValue == null)
            {
                return defaultValue;
            }
            return (T)fieldValue;
        }
    }
}
