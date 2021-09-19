using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReFilter.Core.Attributes;

namespace ReFilter.Extensions
{
    public static class TypeExtensions
    {
        public static List<PropertyInfo> GetSearchableProperties(this Type type)
        {
            return type.GetProperties()
                .Where(p => p.GetCustomAttributes().OfType<ReFilterProperty>().Any(e => e.UsedForSearchQuery))
                .ToList();
        }

        public static List<PropertyInfo> GetSpecialFilterProperties(this Type type)
        {
            return type.GetProperties()
                .Where(p => p.GetCustomAttributes().OfType<ReFilterProperty>().Any(e => e.HasSpecialFilter))
                .ToList();
        }
    }
}
