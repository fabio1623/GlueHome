using AutoMapper;
using DeliveryDomain.DomainEnums;
using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.Enums;
using DeliveryInfrastructure.Exceptions;
using DeliveryInfrastructure.InfrastructureModels;
using MongoDB.Driver;

namespace DeliveryInfrastructure.Services;

public class UserService : BaseMongoDbService<UserDomain, CreateUserDomain, UpdateUserDomain, UserInfra>, IUserService
{
    protected override string? CollectionName => "Users";
    
    public UserService(IMongoDbConfiguration mongoDbConfiguration, IMapper mapper) : base(mongoDbConfiguration, mapper)
    {
    }
    protected override CreateIndexModel<UserInfra> GetCreateIndexModel()
    {
        var indexKeysDefinition = Builders<UserInfra>
            .IndexKeys
            .Ascending(x => x.Username);

        var createIndexOptions = new CreateIndexOptions { Unique = true };
        
        return new CreateIndexModel<UserInfra>(indexKeysDefinition, createIndexOptions);
    }

    protected override UpdateDefinition<UserInfra> GetUpdateDefinition(UpdateUserDomain? domainModelUpdate)
    {
        var mappedRoleInfra = Mapper.Map<RoleInfra?>(domainModelUpdate?.RoleDomain);
        if (mappedRoleInfra == null)
            throw new InfrastructureException($"Could not parse {nameof(RoleDomain)} '{domainModelUpdate?.RoleDomain?.ToString()}'.");
        
        var currentDateTime = DateTime.UtcNow;

        return Builders<UserInfra>
            .Update
            .Set(x => x.Role, mappedRoleInfra)
            .Set(x => x.UpdatedAt, currentDateTime);
    }

    public async Task<UserDomain?> GetByUsername(string? username, CancellationToken cancellationToken)
    {
        var filterDefinition = GetUsernameFilterDefinition(username);
        return await FindOne(filterDefinition, cancellationToken);
    }
    
    private static FilterDefinition<UserInfra> GetUsernameFilterDefinition(string? username)
    {
        return Builders<UserInfra>
            .Filter
            .Eq(x => x.Username, username);
    }
}