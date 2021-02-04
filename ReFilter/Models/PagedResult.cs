using System.Collections.Generic;
using System.Linq;

namespace ReFilter.Models
{
    public class PagedResult<T> : PagedResultBase where T : new()
    {
        public List<T> Results { get; set; }
        public IQueryable<T> ResultQuery { get; set; }
    }
}
