using System;

namespace ReFilter.Core.Models
{
    public abstract class PagedResultBase : PagedBase
    {
        public int PageCount { get; set; }
        public int RowCount { get; set; }

        public int CurrentPage => PageIndex + 1;

        public int FirstRowOnPage => PageIndex * PageSize + 1;

        public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
    }
}
