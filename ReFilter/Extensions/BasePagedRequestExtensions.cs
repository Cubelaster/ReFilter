using System.Collections.Generic;
using System.Linq;
using LinqKit;
using ReFilter.Models;

namespace ReFilter.Extensions
{
    public static class BasePagedRequestExtensions
    {
        public static List<PropertyFilterConfig> GetPropertyFilterConfigs(this BasePagedRequest request, string filterKey, Dictionary<string, object> filterValues)
        {
            var existingPfcs = request.PropertyFilterConfigs?
                .Where(pfc => pfc.PropertyName == filterKey);

            if (existingPfcs is not null && existingPfcs.Count() > 0)
            {
                return existingPfcs
                    .Select(pfc =>
                    {
                        // Key may come only from a value-less IsEmpty/IsNotEmpty PFC, so it's not guaranteed to be in filterValues
                        if (filterValues.TryGetValue(filterKey, out var filterValue))
                        {
                            pfc.Value ??= filterValue;
                        }
                        return pfc;
                    })
                    // Skip value-less PFCs (e.g. sort-only) sharing the key with an emptiness filter
                    .Where(pfc => pfc.Value is not null || pfc.IsEmptinessFilter())
                    .ToList();
            }
            else
            {
                return new List<PropertyFilterConfig>
                    {
                        new()
                        {
                            PropertyName = filterKey,
                            Value = filterValues[filterKey],
                            PredicateOperator = PredicateOperator.And
                        }
                    };
            }
        }
    }
}
