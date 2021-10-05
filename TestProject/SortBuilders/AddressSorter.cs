using System.Linq;
using ReFilter.Models.Filtering.Contracts;
using TestProject.Models;

namespace TestProject.SortBuilders
{
    internal class AddressSorter : IReSort<School>
    {
        public IOrderedQueryable<School> SortQuery(IQueryable<School> query, bool isFirst = true)
        {
            return query.OrderBy(e => e.Country.Alpha2Code).ThenBy(e => e.Address);
        }
    }
}
