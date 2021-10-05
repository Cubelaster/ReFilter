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

        public IOrderedQueryable<School> BuildSortedQuery(IQueryable<School> query, PropertyFilterConfig propertyFilterConfig, bool isFirst = true)
        {
            var sorter = GetSort(propertyFilterConfig);
            if (sorter == null)
            {
                return (IOrderedQueryable<School>)query;
            }

            var sortedQuery = sorter.SortQuery(query, isFirst);
            return sortedQuery;
        }

        public IReSort<School> GetSort(PropertyFilterConfig propertyFilterConfig)
        {
            List<IReSort<School>> sorters = new List<IReSort<School>>();

            if (propertyFilterConfig != null)
            {
                if (propertyFilterConfig.PropertyName == nameof(SchoolFilterRequest.Address))
                {
                    sorters.Add(new AddressSorter());
                }
            }

            return null;
        }
    }
}
