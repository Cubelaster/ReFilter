using System;

namespace ReFilter.Utilities
{
    public static class FilterHelper
    {
        public static Type GetMatchingType<T>() where T : class, new()
        {
            switch (typeof(T))
            {
                default:
                    return typeof(T);
            }
        }
    }
}
