namespace ReFilter.Models.Filtering.Contracts
{
    public interface IReFilterable<T> where T : struct
    {
        T Id { get; set; }
    }
}
