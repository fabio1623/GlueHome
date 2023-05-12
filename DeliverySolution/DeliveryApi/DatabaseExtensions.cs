using DeliveryDomain.Interfaces.Initializers;

namespace DeliveryApi;

public static class DatabaseExtensions
{
    public static async Task<WebApplication> ConfigureDatabase(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        
        var usersInitializer = scope.ServiceProvider.GetRequiredService<IUsersInitializer>();
        await usersInitializer.Initialize(CancellationToken.None);
        
        var deliveriesInitializer = scope.ServiceProvider.GetRequiredService<IDeliveriesInitializer>();
        await deliveriesInitializer.Initialize(CancellationToken.None);

        return webApplication;
    }
}