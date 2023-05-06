using System.Data;
using Dapper;
using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.InfrastructureModels.Users;
using MySql.Data.MySqlClient;

namespace DeliveryInfrastructure.Services;

public class UserService : IUserService
{
    private readonly IMySqlConfiguration _mySqlConfiguration;

    public UserService(IMySqlConfiguration mySqlConfiguration)
    {
        _mySqlConfiguration = mySqlConfiguration;
    }

    public async Task Create(UserDomain userDomain)
    {
        using var connection = CreateConnection();
        const string sql = """
            INSERT INTO users (FirstName, LastName, Username, Role, PasswordHash)
            VALUES (@FirstName, @LastName, @Username, @Role, @PasswordHash)
        """;

        var userInfra = new UserInfra(userDomain);
        
        await connection.ExecuteAsync(sql, userInfra);
    }

    public async Task<UserDomain?> GetByUsername(string? username)
    {
        using var connection = CreateConnection();
        const string sql = """
            SELECT * FROM users 
            WHERE Username = @Username
        """;
        var userInfra = await connection.QuerySingleOrDefaultAsync<UserInfra>(sql, new { username });

        return userInfra?.ToDomain();
    }

    public async Task<IEnumerable<UserDomain>?> GetAll()
    {
        using var connection = CreateConnection();
        const string sql = """
            SELECT * FROM users
        """;
        var users= await connection.QueryAsync<UserInfra>(sql);
        return users?.Select(x => x.ToDomain());
    }

    public async Task<UserDomain?> GetById(int? id) 
    {
        using var connection = CreateConnection();
        const string sql = """
            SELECT * FROM users 
            WHERE Id = @Id
        """;
        var userInfra = await connection.QuerySingleOrDefaultAsync<UserInfra>(sql, new { id });

        return userInfra?.ToDomain();
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}