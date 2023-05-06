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
            INSERT IGNORE INTO users (Username, Role, PasswordHash)
            VALUES (@Username, @Role, @PasswordHash);
        ";
        
        var users = new List<UserInfra>
        {
            new() { Id = 1, Username = "admin", PasswordHash = BCryptNet.HashPassword("admin"), Role = RoleDomain.Admin.ToString() },
            new() { Id = 2, Username = "partner", PasswordHash = BCryptNet.HashPassword("partner"), Role = RoleDomain.Partner.ToString() },
            new() { Id = 3, Username = "user", PasswordHash = BCryptNet.HashPassword("user"), Role = RoleDomain.User.ToString() }
        };

        await connection.ExecuteAsync(sql, users);
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}