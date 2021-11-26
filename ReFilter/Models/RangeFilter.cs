namespace ReFilter.Models
{
    public class RangeFilter<T> where T : struct
    {
        public T? Start { get; set; }
        public T? End { get; set; }
    }
}
