using DeliveryInfrastructure.InfrastructureModels.Users;
using Microsoft.EntityFrameworkCore;

namespace DeliveryInfrastructure;

public class UserDataContext : DbContext, IUserDataContext
{
    public DbContext DbContext => this;
    public DbSet<UserInfra>? Users { get; set; }

    public UserDataContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseInMemoryDatabase("DeliveryApi_Db");
    }
}