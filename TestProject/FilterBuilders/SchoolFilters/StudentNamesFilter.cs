using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using ReFilter.Models.Filtering.Contracts;
using TestProject.Models;

namespace TestProject.FilterBuilders.SchoolFilters
{
    internal class StudentNamesFilter : IReFilter<School>
    {
        private readonly List<string> studentNames;

        public StudentNamesFilter(List<string> studentNames)
        {
            this.studentNames = studentNames;
        }

        public IQueryable<School> FilterQuery(IQueryable<School> query)
        {
            return query.Where(s => studentNames.Any(sn => s.Name.Contains(sn)));
        }

        public Expression<Func<School, bool>> GeneratePredicate()
        {
            var basePredicate = PredicateBuilder.New<School>();
            studentNames.ForEach(sn =>
            {
                basePredicate.Or(s => s.Name.Contains(sn));
            });

            return basePredicate;
        }
    }
}
