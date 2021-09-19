using System.Collections.Generic;
using System.Linq;
using ReFilter.Core.Models.Filtering.Contracts;
using ReFilter.ReFilterProvider;
using TestProject.Models;
using TestProject.Models.FilterRequests;
using TestProject.TestData;

namespace TestProject.FilterBuilders
{
    class SchoolFilterBuilder : IReFilterBuilder<School>
    {
        public IQueryable<School> BuildEntityQuery(IReFilterRequest filterRequest)
        {
            var query = SchoolServiceTestData.Schools.AsQueryable();

            query = BuildFilteredQuery(query, filterRequest);

            return query;
        }

        public IQueryable<School> BuildFilteredQuery(IQueryable<School> query, IReFilterRequest filterRequest)
        {
            var filters = GetFilters(filterRequest).ToList();

            filters.ForEach(filter =>
            {
                query = filter.FilterQuery(query);
            });

            return query;
        }

        public IEnumerable<IReFilter<School>> GetFilters(IReFilterRequest filterRequest)
        {
            List<IReFilter<School>> filters = new List<IReFilter<School>>();

            if (filterRequest == null)
            {
                return filters;
            }

            var realFilter = (SchoolFilterRequest)filterRequest;

            //if (!string.IsNullOrWhiteSpace(realFilter.Name))
            //{
            //    filters.Add(new NameFilter(realFilter.Name));
            //}

            return filters;
        }

        public List<int> GetForeignKeys(IReFilterRequest filterRequest)
        {
            var query = SchoolServiceTestData.Schools.AsQueryable();

            query = BuildFilteredQuery(query, filterRequest);

            return query.Select(e => e.Id)
                .Distinct()
                .ToList();
        }
    }
}
