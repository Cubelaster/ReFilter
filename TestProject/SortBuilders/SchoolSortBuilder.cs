using System.Collections.Generic;
using System.Linq;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReSortBuilder;
using TestProject.Models;
using TestProject.Models.FilterRequests;
using TestProject.TestData;

namespace TestProject.SortBuilders
{
    internal class SchoolSortBuilder : IReSortBuilder<School>
    {
        public IQueryable<School> BuildEntityQuery()
        {
            return SchoolServiceTestData.Schools.AsQueryable();
        }

        public IOrderedQueryable<School> BuildSortedQuery(IQueryable<School> query, PropertyFilterConfig propertyFilterConfig,
            bool isFirst = false)
        {
            var sorters = GetSorters(propertyFilterConfig);

            if (sorters == null || sorters.Count == 0)
            {
                return (IOrderedQueryable<School>)query;
            }

            IOrderedQueryable<School> orderedQuery = (IOrderedQueryable<School>)query;

            for (var i = 0; i < sorters.Count; i++)
            {
                orderedQuery = sorters[i].SortQuery(orderedQuery,
                    propertyFilterConfig.SortDirection.Value,
                    isFirst: (i == 0 && isFirst));
            }

            return orderedQuery;
        }

        public List<IReSort<School>> GetSorters(PropertyFilterConfig propertyFilterConfig)
        {
            List<IReSort<School>> sorters = new List<IReSort<School>>();

            if (propertyFilterConfig != null)
            {
                if (propertyFilterConfig.PropertyName == nameof(SchoolFilterRequest.Country))
                {
                    sorters.Add(new CountrySorter());
                }
            }

            return sorters;
        }
    }
}
