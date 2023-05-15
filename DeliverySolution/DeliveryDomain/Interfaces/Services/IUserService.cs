using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Services;

public interface IUserService : IBaseMongoDbService<UserDomain, CreateUserDomain, UpdateUserDomain>
{
    Task<UserDomain?> GetByUsername(string? username, CancellationToken cancellationToken);
}