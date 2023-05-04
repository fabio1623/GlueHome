using DeliveryInfrastructure;
using DeliveryInfrastructure.InfrastructureModels;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DeliveryApi;

public static class SeedExtensions
{
    public static WebApplication SeedUsers(this WebApplication webApplication)
    {
        // create hardcoded test users in db on startup
        var users = new List<UserInfra>
        {
            new() { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", PasswordHash = BCryptNet.HashPassword("admin"), RoleInfra = RoleInfra.Admin },
            new() { Id = 2, FirstName = "Partner", LastName = "User", Username = "partner", PasswordHash = BCryptNet.HashPassword("partner"), RoleInfra = RoleInfra.Partner },
            new() { Id = 3, FirstName = "User", LastName = "User", Username = "user", PasswordHash = BCryptNet.HashPassword("user"), RoleInfra = RoleInfra.User }
        };

        using var scope = webApplication.Services.CreateScope();
        var applicationContext = scope.ServiceProvider.GetRequiredService<IApplicationContext>();
        applicationContext.Users?.AddRange(users);
        applicationContext.DbContext.SaveChanges();

        return webApplication;
    }
}