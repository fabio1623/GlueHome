using DeliveryDomain.DomainModels;
using DeliveryDomain.DomainModels.Users;

namespace DeliveryDomain.Interfaces.Businesses;

public interface IUserBusiness
{
    Task<AuthenticateUserResponseDomain?> Authenticate(AuthenticateUserRequestDomain? authenticateRequestDomain, CancellationToken cancellationToken);
    Task<PagedListDomain<UserDomain?>> GetPaged(int? requestedPage, int? pageSize, CancellationToken cancellationToken);
    Task<UserDomain?> Get(string? id, CancellationToken cancellationToken);
}