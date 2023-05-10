using System.Data;
using Dapper;
using DeliveryDomain.DomainModels.Deliveries;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.InfrastructureModels.Deliveries;
using MySql.Data.MySqlClient;

namespace DeliveryInfrastructure.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IMySqlConfiguration _mySqlConfiguration;

    public DeliveryService(IMySqlConfiguration mySqlConfiguration)
    {
        _mySqlConfiguration = mySqlConfiguration;
    }
    
    public async Task Create(CreateDeliveryDomain? deliveryDomain, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        const string sql = """
            INSERT INTO deliveries
            VALUES
            (@OrderNumber, @State, @Sender, @RecipientName, @RecipientAddress, @RecipientEmail, @RecipientPhoneNumber, @StartTime, @EndTime);
        """;

        var deliveryInfra = new DeliveryInfra(deliveryDomain);

        await connection.ExecuteAsync(sql, deliveryInfra);
    }

    public async Task<DeliveryDomain.DomainModels.Deliveries.DeliveryDomain?> Get(string? orderNumber, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        const string sql = """
            SELECT * FROM deliveries 
            WHERE OrderNumber = @orderNumber
        """;
        var deliveryInfra = await connection.QuerySingleOrDefaultAsync<DeliveryInfra>(sql, new { orderNumber });

        return deliveryInfra?.ToDomain();
    }

    public async Task Update(string? orderNumber, DeliveryUpdateDomain? deliveryUpdateDomain, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        const string sql = """
            UPDATE deliveries 
            SET State = @State
            WHERE OrderNumber = @orderNumber
        """;
        var parameters = new
        {
            State = deliveryUpdateDomain?.State?.ToString(), 
            OrderNumber = orderNumber
        };
        await connection.ExecuteAsync(sql, parameters);
    }

    public async Task Delete(string? orderNumber, CancellationToken cancellationToken)
    {
        using var connection = CreateConnection();
        const string sql = """
            DELETE FROM deliveries 
            WHERE OrderNumber = @orderNumber
        """;
        await connection.ExecuteAsync(sql, new { orderNumber });
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
    }
    
    private IDbConnection CreateConnection()
    {
        var connectionString = $"Server={_mySqlConfiguration.Server}; Database={_mySqlConfiguration.DbName}; Uid={_mySqlConfiguration.UserName}; Pwd={_mySqlConfiguration.Password};";
        return new MySqlConnection(connectionString);
    }
}