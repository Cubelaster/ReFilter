using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ReFilter.Models;
using ReFilter.Models.Filtering.Contracts;
using ReFilter.ReFilterBuilder;
using TestProject.FilterBuilders.SchoolFilters;
using TestProject.Models;
using TestProject.Models.FilterRequests;
using TestProject.TestData;

namespace TestProject.FilterBuilders
{
    class SchoolFilterBuilder : IReFilterBuilder<School>
    {
        public IQueryable<School> BuildEntityQuery(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
        {
            var query = SchoolServiceTestData.Schools.AsQueryable();

            query = BuildFilteredQuery(query, filterRequest, propertyFilterConfigs);

            return query;
        }

        public IQueryable<School> BuildFilteredQuery(IQueryable<School> query, IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
        {
            var filters = GetFilters(filterRequest, propertyFilterConfigs).ToList();

            filters.ForEach(filter =>
            {
                query = filter.FilterQuery(query);
            });

            return query;
        }

        public List<Expression<Func<School, bool>>> BuildPredicates(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs, IQueryable<School> query = null)
        {
            var realFilter = filterRequest as SchoolFilterRequest;
            var filters = GetFilters(filterRequest, propertyFilterConfigs).ToList();
            List<Expression<Func<School, bool>>> expressions = new();

            if (realFilter?.Country != null)
            {
                filters.Add(new CountryFilter(realFilter.Country, propertyFilterConfigs?
                        .Where(p => p.PropertyName.StartsWith("Country."))
                        .Select(p => new PropertyFilterConfig
                        {
                            PropertyName = p.PropertyName["Country.".Length..],
                            OperatorComparer = p.OperatorComparer,
                            PredicateOperator = p.PredicateOperator
                        })
                        .ToList()));
            }

            filters.ForEach(filter => expressions.Add(filter.GeneratePredicate(query)));
            return expressions;
        }

        public IEnumerable<IReFilter<School>> GetFilters(IReFilterRequest filterRequest, List<PropertyFilterConfig> propertyFilterConfigs)
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
    }
}
