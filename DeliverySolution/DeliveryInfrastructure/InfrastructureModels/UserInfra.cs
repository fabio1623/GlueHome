using DeliveryInfrastructure.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliveryInfrastructure.InfrastructureModels;

public class UserInfra : BaseModelInfrastructure
{
    public string? Username { get; init; }
    
    [BsonRepresentation(BsonType.String)]
    public RoleInfra? Role { get; set; }
    public string? PasswordHash { get; init; }
}