using DeliveryDomain.Interfaces.Configurations;

namespace DeliveryApi.Configurations;

public class MySqlConfiguration : IMySqlConfiguration
{
    public string? Server { get; set; }
    public string? DbName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
}