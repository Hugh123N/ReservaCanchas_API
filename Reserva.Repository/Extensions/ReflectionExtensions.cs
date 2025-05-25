using System.Reflection;

namespace Reserva.Repository.Extensions
{
    public static class ReflectionExtensions
    {
        public static Dictionary<Type, PropertyInfo[]> PropertyInfoCache;
        static ReflectionExtensions() => PropertyInfoCache = new Dictionary<Type, PropertyInfo[]>();

        public static bool IsSimple(this Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? type.GetGenericArguments()[0].GetTypeInfo().IsSimple()
                : type.IsEnum
                  || type.IsPrimitive
                  || type.IsValueType
                  || type == typeof(string)
                  || type == typeof(double)
                  || type == typeof(decimal);

        public static bool IsCollection(this Type type)
            => !type.IsSimple() && type.GetInterface("IEnumerable`1") != null;

        public static PropertyInfo[] GetTypeProperties(this Type type)
        {
            if (!PropertyInfoCache.ContainsKey(type))
                PropertyInfoCache[type] = type.GetProperties();

            return PropertyInfoCache[type];
        }
    }
}
