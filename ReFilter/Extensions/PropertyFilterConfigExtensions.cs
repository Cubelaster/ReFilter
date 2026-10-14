using System.Collections.Generic;
using System.Linq;
using ReFilter.Enums;
using ReFilter.Models;

namespace ReFilter.Extensions
{
    public static class PropertyFilterConfigExtensions
    {
        // IsEmpty/IsNotEmpty don't carry a value, so they must trigger filtering even when Value is null
        public static bool IsEmptinessFilter(this PropertyFilterConfig propertyFilterConfig)
        {
            return propertyFilterConfig?.OperatorComparer is OperatorComparer.IsEmpty or OperatorComparer.IsNotEmpty;
        }

        public static List<PropertyFilterConfig> FilterForSubEntity(this List<PropertyFilterConfig> originalList, string subPropertyName)
        {
            if (originalList == null)
            {
                return [];
            }

            var nameMarker = subPropertyName + ".";

            return originalList?
                .Where(p => p.PropertyName.StartsWith(nameMarker))
                .Select(p => new PropertyFilterConfig
                {
                    PropertyName = p.PropertyName[nameMarker.Length..],
                    OperatorComparer = p.OperatorComparer,
                    PredicateOperator = p.PredicateOperator,
                    Value = p.Value
                })
                .ToList();
        }
    }
}
