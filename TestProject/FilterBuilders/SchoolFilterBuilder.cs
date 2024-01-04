using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReFilterProvider;
using TestProject.FilterBuilders.SchoolFilters;
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

        public List<Expression<Func<School, bool>>> BuildPredicates(IReFilterRequest filterRequest)
        {
            var filters = GetFilters(filterRequest).ToList();

            List<Expression<Func<School, bool>>> expressions = new();

            filters.ForEach(filter =>
            {
                expressions.Add(filter.GeneratePredicate());
            });

            return expressions;
        }

        public IEnumerable<IReFilter<School>> GetFilters(IReFilterRequest filterRequest)
        {
            List<IReFilter<School>> filters = new();

            if (filterRequest == null)
            {
                return filters;
            }

            var realFilter = (SchoolFilterRequest)filterRequest;

            if (realFilter.StudentNames is not null && realFilter.StudentNames.Any())
            {
                filters.Add(new StudentNamesFilter(realFilter.StudentNames));
            }

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
