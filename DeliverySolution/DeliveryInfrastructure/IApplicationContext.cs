using DeliveryInfrastructure.InfrastructureModels;
using Microsoft.EntityFrameworkCore;

namespace DeliveryInfrastructure;

public interface IApplicationContext : IDisposable
{
    DbContext DbContext { get; }
    DbSet<UserInfra>? Users { get; set; }
}