using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Businesses;

public interface IUserBusiness
{
    AuthenticateResponseDomain? Authenticate(AuthenticateRequestDomain? authenticateRequestDomain);
    IEnumerable<UserDomain>? GetAll();
    UserDomain? GetById(int? userId);
}