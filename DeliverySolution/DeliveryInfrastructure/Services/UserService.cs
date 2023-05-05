using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Services;

namespace DeliveryInfrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserDataContext _context;

    public UserService(IUserDataContext context)
    {
        _context = context;
    }

    public UserDomain? GetByUsername(string? username)
    {
        var user = _context.Users?.SingleOrDefault(x => x.Username == username);
        return user?.ToDomain();
    }

    public IEnumerable<UserDomain>? GetAll()
    {
        var users = _context.Users;
        return users?.Select(x => x.ToDomain());
    }

    public UserDomain? GetById(int? id) 
    {
        var user = _context.Users?.Find(id);
        return user?.ToDomain();
    }
}