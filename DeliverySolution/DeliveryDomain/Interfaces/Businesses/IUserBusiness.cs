using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Businesses;

public interface IUserBusiness
{
    Task<AuthenticateResponseDomain?> Authenticate(AuthenticateRequestDomain? authenticateRequestDomain);
    Task<IEnumerable<UserDomain>?> GetAll();
    Task<UserDomain?> GetById(int? userId);
}