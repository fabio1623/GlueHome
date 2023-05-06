using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Services;

public interface IUserService
{
    Task Create(UserDomain userDomain);
    Task<UserDomain?> GetByUsername(string? username);
    Task<IEnumerable<UserDomain>?> GetAll();
    Task<UserDomain?> GetById(int? id);
}