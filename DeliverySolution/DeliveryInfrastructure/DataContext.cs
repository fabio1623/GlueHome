using System.Data;
using Dapper;
using DeliveryDomain.Interfaces.Configurations;
using MySql.Data.MySqlClient;

namespace DeliveryInfrastructure;

public class DataContext : IDataContext
{
    private readonly IMySqlConfiguration _mySqlConfiguration;

    public DataContext(IMySqlConfiguration mySqlConfiguration)
    {
        _mySqlConfiguration = mySqlConfiguration;
    }

    public async Task InitialiseMySql()
    {
        await InitialiseDatabase();
        await InitialiseTables();
    }
    
    private async Task InitialiseDatabase()
    {
        // create database if it doesn't exist
        var connectionString = $"Server={_mySqlConfiguration.Server}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        await using var connection = new MySqlConnection(connectionString);
        var sql = $"CREATE DATABASE IF NOT EXISTS `{_mySqlConfiguration.DbName}`;";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitialiseTables()
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
            """;
        
        await connection.ExecuteAsync(sql);
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}