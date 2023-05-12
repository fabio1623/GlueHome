namespace DeliveryDomain.DomainModels
{
    public class PagedListDomain<TDomainModel>
    {
        public int? CurrentPage { get; set; }
        public int? TotalPages { get; set; }
        public long? TotalResults { get; set; }
        public IEnumerable<TDomainModel>? Results { get; set; }
    }
}
