using System.Linq;
using ReFilter.Enums;
using ReFilter.Models.Filtering.Contracts;
using TestProject.Models;

namespace TestProject.SortBuilders
{
    internal class CountrySorter : IReSort<School>
    {
        public IOrderedQueryable<School> SortQuery(IQueryable<School> query, SortDirection sortDirection, bool isFirst = true)
        {
            if (sortDirection == SortDirection.ASC)
            {
                return query.OrderBy(e => (e.Country == null ? null : e.Country.Alpha2Code)).ThenBy(e => e.Address);
            }
            else
            {
                return query.OrderByDescending(e => (e.Country == null ? null : e.Country.Alpha2Code)).ThenByDescending(e => e.Address);
            }
        }
    }
}
