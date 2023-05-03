using Delivery.API.Entities;
using Delivery.API.Models.Users;

namespace Delivery.API.Services;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(int id);
}