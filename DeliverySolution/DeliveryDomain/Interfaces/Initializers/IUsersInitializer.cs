namespace DeliveryDomain.Interfaces.Initializers;

public interface IUsersInitializer
{
    Task Initialize(CancellationToken cancellationToken);
}