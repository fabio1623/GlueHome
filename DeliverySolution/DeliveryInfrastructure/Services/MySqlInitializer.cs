using System.Data;
using Dapper;
using DeliveryDomain.DomainModels.Users;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.InfrastructureModels.Deliveries;
using DeliveryInfrastructure.InfrastructureModels.Users;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using BCryptNet = BCrypt.Net.BCrypt;

namespace DeliveryInfrastructure.Services;

public class MySqlInitializer : IMySqlInitializer
{
    private readonly IMySqlConfiguration _mySqlConfiguration;
    private readonly ILogger<MySqlInitializer> _logger;

    public MySqlInitializer(IMySqlConfiguration mySqlConfiguration, ILogger<MySqlInitializer> logger)
    {
        _mySqlConfiguration = mySqlConfiguration;
        _logger = logger;
    }

    public async Task InitializeMySql()
    {
        await InitializeDatabase();
        _logger.LogInformation("Database initialization done.");
        
        await InitializeTables();
        _logger.LogInformation("Tables initialization done.");
        
        await SeedUsers();
        _logger.LogInformation("Users seed done.");
        
        await SeedDeliveries();
        _logger.LogInformation("Deliveries seed done.");
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

    private async Task SeedDeliveries()
    {
        using var connection = CreateConnection();
        const string sql = @"
            INSERT IGNORE INTO deliveries
            VALUES  (@OrderNumber, @State, @Sender, @RecipientName, @RecipientAddress, @RecipientEmail, @RecipientPhoneNumber, @StartTime, @EndTime);
        ";

        var deliveries = new List<DeliveryInfra>();
        var random = new Random();
        var states = Enum.GetValues(typeof(StateInfra));

        for (var i = 1; i <= 1000; i++)
        {
            var delivery = new DeliveryInfra
            {
                OrderNumber = "Order" + i,
                State = states.GetValue(random.Next(states.Length))?.ToString(),
                Sender = "Sender" + i,
                RecipientName = "RecipientName" + i,
                RecipientAddress = "RecipientAddress" + i,
                RecipientEmail = "RecipientEmail" + i + "@example.com",
                RecipientPhoneNumber = "RecipientPhoneNumber" + i,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(i % 5 + 1)
            };

            if (delivery.State == "Expired" && DateTime.UtcNow < delivery.EndTime)
            {
                delivery.EndTime = DateTime.UtcNow.AddDays(-1);
            }

            deliveries.Add(delivery);
        }
        
        await connection.ExecuteAsync(sql, deliveries);
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}