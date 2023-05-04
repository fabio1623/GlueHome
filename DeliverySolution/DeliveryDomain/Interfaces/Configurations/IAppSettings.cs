namespace DeliveryDomain.Interfaces.Configurations;

public interface IAppSettings
{
    string? Secret { get; set; }
}