using DeliveryInfrastructure.InfrastructureModels;
using Microsoft.EntityFrameworkCore;

namespace DeliveryInfrastructure;

public class ApplicationContext : DbContext, IApplicationContext
{
    public DbContext DbContext => this;
    public DbSet<UserInfra>? Users { get; set; }

    public ApplicationContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // in memory database used for simplicity, change to a real db for production applications
        options.UseInMemoryDatabase("DeliveryApi_Db");
    }
}