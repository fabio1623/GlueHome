namespace DeliveryApi.Models
{
    public class PagedList<TModel>
    {
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public long? TotalResults { get; set; }
        public IEnumerable<TModel>? Results { get; set; }
    }
}
