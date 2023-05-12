namespace DeliveryDomain.DomainModels
{
    public abstract class BaseModelDomain
    {
        public string? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}