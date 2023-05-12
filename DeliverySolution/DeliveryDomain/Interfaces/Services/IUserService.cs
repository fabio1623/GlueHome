using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Services;

public interface IUserService : IBaseMongoDbService<UserDomain, CreateUserRequestDomain, UpdateUserRequestDomain>
{
    Task<UserDomain?> GetByUsername(string? username, CancellationToken cancellationToken);
}