using BugzNet.Core.Interfaces;
using BugzNet.Core.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BugzNet.Core.Extensions
{
    public static class TypeExtensions
    {
        private static readonly Assembly[] _customAssemblies = Assembly.GetEntryAssembly()
                                                .GetReferencedAssemblies()
                                                .Where(an => an.FullName.StartsWith("BugzNet"))
                                                .Select(an => Assembly.Load(an)).ToArray();
        public static Type GetEntityType(this string entityName)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().ToList().First(an => an.FullName.StartsWith("BugzNet.Infrastructure"))
                    .GetTypes()
                    .Where(t => t.IsClass && t.Namespace == "BugzNet.Infrastructure.DataEF.Identity")
                    .ToList();

            types.AddRange(Assembly.GetAssembly(typeof(BaseEntity<>)).GetTypes());


            var entityType = types
                                    .Where(t => !t.IsInterface && !t.IsAbstract && t.Name == entityName)
                                    .FirstOrDefault();

            return entityType;
        }

        public static Type GetDomainType(this string entityName)
        {
            var type = Assembly.GetAssembly(typeof(BugzNetFeatureFlags))
                                        .GetTypes()
                                        .Where(t => !t.IsInterface && t.Name == entityName)
                                        .FirstOrDefault();

            return type;
        }

        public static Type GetTypeByString(this string entityName)
        {
            var type = _customAssemblies.SelectMany(a => a.GetTypes())
                                        .Where(t => !t.IsInterface && t.Name == entityName)
                                        .FirstOrDefault();

            return type;
        }

        public static PropertyInfo GetCollectionOfProperty(this Type type, Type collectionType)
        {
            return type.GetProperties()
                .Where(p => typeof(IEnumerable<>).MakeGenericType(collectionType).IsAssignableFrom(p.PropertyType)
                         && p.PropertyType.GetGenericArguments()[0] == collectionType)
                .FirstOrDefault();
        }

        public static Type GetModificationEntity(this string entityName)
        {
            return _customAssemblies.SelectMany(a => a.GetTypes())
                                    .Where(t => typeof(BaseEntity<int>).IsAssignableFrom(t) && t.Name == entityName)
                                    .FirstOrDefault();
        }

        public static Type GetProjectionType(this string entityName)
        {
            return GetProjectionType(entityName.GetEntityType());
        }

        public static Type GetProjectionType(this Type entityType)
        {
            return _customAssemblies
                            .SelectMany(a => a.GetTypes())
                            .Where(t => typeof(IEntityProjection<>).MakeGenericType(entityType).IsAssignableFrom(t)
                                    && !t.IsInterface)
                            .FirstOrDefault();
        }

        public static bool IsInteger(this Type type)
        {
            return (new Type[] { typeof(SByte), typeof(Int16), typeof(Int32), typeof(Int64),
                                typeof(Byte), typeof(UInt16), typeof(UInt32), typeof(UInt64)}).Contains(type);
        }

        public static bool IsNullable(this Type type)
        {
            //if (!type.IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false; // value-type
        }

        public static PropertyInfo GetPropertyWithAttribute(this Type type, Type attributeType)
        {
            return type.GetProperties().Where(p => p.GetCustomAttribute(attributeType) != null).FirstOrDefault();
        }

        public static bool HasInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(i => i.Name.Contains(interfaceType.Name));
        }
    }
}
