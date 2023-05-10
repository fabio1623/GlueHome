using System.Data;
using Dapper;
using DeliveryBackground.Configurations;
using DeliveryBackground.InfrastructureModels;
using DeliveryBackground.RabbitMqMessages;
using MySql.Data.MySqlClient;

namespace DeliveryBackground.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IMySqlConfiguration _mySqlConfiguration;
    private readonly IRabbitMqService _rabbitMqService;

    public DeliveryService(IMySqlConfiguration mySqlConfiguration, IRabbitMqService rabbitMqService)
    {
        _mySqlConfiguration = mySqlConfiguration;
        _rabbitMqService = rabbitMqService;
    }

    public async Task<IEnumerable<string>> GetExpiredDeliveries()
    {
        using var connection = CreateConnection();
        const string sql = """
            SELECT orderNumber FROM deliveries 
            WHERE EndTime < NOW() and State != @State
        """;
        
        return await connection.QueryAsync<string>(sql, new { State = StateInfra.Expired.ToString() });
    }

    public async Task ExpireDeliveries(IEnumerable<string> orderNumbers)
    {
        using var connection = CreateConnection();
        const string sql = """
            UPDATE deliveries 
            SET State = @State
            WHERE OrderNumber IN @orderNumbers
        """;
        
        var parameters = new
        {
            State = StateInfra.Expired.ToString(), 
            OrderNumbers = orderNumbers
        };
        
        await connection.ExecuteAsync(sql, parameters);

        var deliveriesExpiredMessage = new DeliveriesExpiredMessage
        {
            OrderNumbers = orderNumbers
        };

        await _rabbitMqService.ProduceMessage(deliveriesExpiredMessage);
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}