namespace DeliveryApi.Models;

public class User : BaseModel
{
    public string? Username { get; init; }
    public string? Role { get; set; }
}