namespace DeliveryBackground.Configurations;

public interface IMySqlConfiguration
{
    string? Server { get; set; }
    string? DbName { get; set; }
    string? UserName { get; set; }
    string? Password { get; set; }
}