using DeliveryInfrastructure.InfrastructureModels.Users;
using Microsoft.EntityFrameworkCore;

namespace DeliveryInfrastructure;

public interface IUserDataContext : IDisposable
{
    DbContext DbContext { get; }
    DbSet<UserInfra>? Users { get; set; }
}