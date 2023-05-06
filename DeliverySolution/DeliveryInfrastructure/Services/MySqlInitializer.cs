using System.Data;
using Dapper;
using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.InfrastructureModels.Users;
using MySql.Data.MySqlClient;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DeliveryInfrastructure.Services;

public class MySqlInitializer : IMySqlInitializer
{
    private readonly IMySqlConfiguration _mySqlConfiguration;

    public MySqlInitializer(IMySqlConfiguration mySqlConfiguration)
    {
        _mySqlConfiguration = mySqlConfiguration;
    }

    public async Task InitializeMySql()
    {
        await InitializeDatabase();
        await InitializeTables();
        await SeedUsers();
        // await AddTrigger();
    }
    
    private async Task InitializeDatabase()
    {
        // create database if it doesn't exist
        var connectionString = $"Server={_mySqlConfiguration.Server}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        await using var connection = new MySqlConnection(connectionString);
        var sql = $"CREATE DATABASE IF NOT EXISTS `{_mySqlConfiguration.DbName}`;";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitializeTables()
    {
        // create tables if they don't exist
        using var connection = CreateConnection();
        
        const string sql = """
                CREATE TABLE IF NOT EXISTS deliveries (
                    OrderNumber VARCHAR(255) NOT NULL,
                    State ENUM('Created', 'Approved', 'Completed', 'Cancelled', 'Expired') NOT NULL,
                    Sender VARCHAR(255) NOT NULL,
                    RecipientName VARCHAR(255) NOT NULL,
                    RecipientAddress VARCHAR(255) NOT NULL,
                    RecipientEmail VARCHAR(255) NOT NULL,
                    RecipientPhoneNumber VARCHAR(20) NOT NULL,
                    StartTime DATETIME NOT NULL,
                    EndTime DATETIME NOT NULL,
                    PRIMARY KEY (`OrderNumber`)
                );

                CREATE TABLE IF NOT EXISTS users (
                    Id INT NOT NULL AUTO_INCREMENT,
                    FirstName VARCHAR(255) NOT NULL,
                    LastName VARCHAR(255) NOT NULL,
                    Username VARCHAR(255) NOT NULL UNIQUE,
                    Role ENUM('Admin', 'Partner', 'User') NOT NULL,
                    PasswordHash VARCHAR(255) NOT NULL,
                    PRIMARY KEY (`Id`)
                );
            """;
        
        await connection.ExecuteAsync(sql);
    }

    private async Task SeedUsers()
    {
        using var connection = CreateConnection();
        const string sql = @"
            INSERT IGNORE INTO users (FirstName, LastName, Username, Role, PasswordHash)
            VALUES (@FirstName, @LastName, @Username, @Role, @PasswordHash);
        ";
        
        var users = new List<UserInfra>
        {
            new() { Id = 1, FirstName = "Admin", LastName = "User", Username = "admin", PasswordHash = BCryptNet.HashPassword("admin"), Role = RoleDomain.Admin.ToString() },
            new() { Id = 2, FirstName = "Partner", LastName = "User", Username = "partner", PasswordHash = BCryptNet.HashPassword("partner"), Role = RoleDomain.Partner.ToString() },
            new() { Id = 3, FirstName = "User", LastName = "User", Username = "user", PasswordHash = BCryptNet.HashPassword("user"), Role = RoleDomain.User.ToString() }
        };

        await connection.ExecuteAsync(sql, users);
    }

    private async Task AddTrigger()
    {
        using var connection = CreateConnection();
        const string sql = @"
            CREATE TRIGGER delivery_expire_trigger AFTER INSERT ON deliveries
            FOR EACH ROW
            BEGIN
                UPDATE deliveries
                SET state = 'Expired'
                WHERE state IN ('Created', 'Approved')
                    AND EndTime < UTC_TIMESTAMP()
                    AND OrderNumber = NEW.OrderNumber;
            END;
        ";
        
        await connection.ExecuteAsync(sql);
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}