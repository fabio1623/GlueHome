using DeliveryDomain.DomainModels;

namespace DeliveryDomain.Interfaces.Services;

public interface IUserService
{
    UserDomain? GetByUsername(string? username);
    IEnumerable<UserDomain>? GetAll();
    UserDomain? GetById(int id);
}