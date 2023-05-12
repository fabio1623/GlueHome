using AutoMapper;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Initializers;
using DeliveryInfrastructure.Enums;
using DeliveryInfrastructure.InfrastructureModels;
using DeliveryInfrastructure.Services;
using Microsoft.Extensions.Logging;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DeliveryInfrastructure.Initializers;

public class UsersInitializer : UserService, IUsersInitializer
{
    private readonly ILogger<UsersInitializer> _logger;

    public UsersInitializer(IMongoDbConfiguration mongoDbConfiguration, IMapper mapper, ILogger<UsersInitializer> logger) : base(mongoDbConfiguration, mapper)
    {
        _logger = logger;
    }

    public async Task Initialize(CancellationToken cancellationToken)
    {
        await SeedUsers();
    }
    
    private async Task SeedUsers()
    {
        var users = new List<UserInfra>
        {
            CreateUser("admin", "admin", RoleInfra.Admin),
            CreateUser("partner", "partner", RoleInfra.Partner),
            CreateUser("user", "user", RoleInfra.User)
        };
    
        try
        {
            await MongoCollection.InsertManyAsync(users);
            _logger.LogInformation("Users seed done.");
        }
        catch (Exception e)
        {
            _logger.LogWarning("Users seed not applied. Exception: {exception}", e);
        }
    }
    
    private static UserInfra CreateUser(string? username, string? password, RoleInfra? role)
    {
        var currentDateTime = DateTime.UtcNow;
        return new UserInfra
        {
            Username   = username,
            PasswordHash = BCryptNet.HashPassword(password),
            Role = role,
            CreatedAt = currentDateTime,
            UpdatedAt = currentDateTime
        };
    }
}