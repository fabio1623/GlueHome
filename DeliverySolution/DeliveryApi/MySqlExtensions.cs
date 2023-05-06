using DeliveryDomain.Interfaces.Services;

namespace DeliveryApi;

public static class MySqlExtensions
{
    public static async Task<WebApplication> ConfigureMySqlDatabase(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var dataContext = scope.ServiceProvider.GetRequiredService<IMySqlInitializer>();
        await dataContext.InitializeMySql();

        return webApplication;
    }
}