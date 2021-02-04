using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ReFilter.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetPropValue(this object entity, string propName)
        {
            string[] nameParts = propName.Split('.');
            if (nameParts.Length == 1)
            {
                return entity.GetType().GetProperty(propName).GetValue(entity, null);
            }

            foreach (string part in nameParts)
            {
                if (entity == null) { return null; }

                Type type = entity.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                entity = info.GetValue(entity, null);
            }
            return entity;
        }

        public static Dictionary<string, object> GetPropertiesWithValue<T>(this T entity)
        {
            var values = typeof(T).GetProperties()
                .Where(p => p.GetValue(entity) != null)
                .ToDictionary(pv => pv.Name, pv => pv.GetValue(entity));

            return values;
        }

        public static Dictionary<string, object> GetObjectPropertiesWithValue(this object entity)
        {
            var values = entity.GetType().GetProperties()
                .Where(p => p.GetValue(entity) != null)
                .ToDictionary(pv => pv.Name, pv => pv.GetValue(entity));

            return values;
        }

        public static Dictionary<string, object> GetObjectPropertiesWithValueUnsafe(this object entity)
        {
            var values = entity.GetType().GetProperties()
                .ToDictionary(pv => pv.Name, pv => pv.GetValue(entity));

            return values;
        }

        public static string GetDescription<T>(this T value)
        {
            return value.GetType()
                .GetMember(value.ToString())
                .First()
                .GetCustomAttribute<DescriptionAttribute>()?
                .Description ?? string.Empty;
        }
    }
}
