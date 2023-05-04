using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryApi.Configurations;

public class AppSettings : IAppSettings
{
    public string? Secret { get; set; }
}