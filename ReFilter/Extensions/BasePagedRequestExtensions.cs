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
                        pfc.Value ??= filterValues[filterKey];
                        return pfc;
                    })
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
