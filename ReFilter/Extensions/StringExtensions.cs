namespace ReFilter.Extensions
{
    public static class StringExtensions
    {
        public static string NullSafeToLower(this string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            return value.ToLower();
        }
    }
}
