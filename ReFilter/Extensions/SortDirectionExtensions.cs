using ReFilter.Enums;

namespace ReFilter.Extensions
{
    public static class SortDirectionExtensions
    {
        public static string GetOrderByNames(this SortDirection value, bool isThen = false)
        {
            switch (value)
            {
                case SortDirection.ASC:
                    return isThen ? "ThenBy" : "OrderBy";
                default:
                    return isThen ? "ThenByDescending" : "OrderByDescending";
            }
        }
    }
}
