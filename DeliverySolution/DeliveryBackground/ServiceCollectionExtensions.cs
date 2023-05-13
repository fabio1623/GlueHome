using DeliveryBackground.Configurations;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Initializers;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.AutoMapperProfiles;
using DeliveryInfrastructure.Initializers;
using DeliveryInfrastructure.Services;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace DeliveryBackground;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .RegisterConfiguration<IElasticSearchConfiguration, ElasticSearchConfiguration>(configuration)
            .RegisterConfiguration<IRabbitMqConfiguration, RabbitMqConfiguration>(configuration)
            .RegisterConfiguration<IMongoDbConfiguration, MongoDbConfiguration>(configuration);
    }
    
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(InfrastructureModelProfiles));
    }

    public static IServiceCollection AddRabbitMqConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<IConnectionFactory>(_ => new ConnectionFactory
            {
                HostName = configuration["RabbitMQConfiguration:HostName"],
                UserName = configuration["RabbitMQConfiguration:UserName"],
                Password = configuration["RabbitMQConfiguration:Password"]
            })
            .AddSingleton<IRabbitMqService,RabbitMqService>();
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDeliveryService, DeliveryService>()
            .AddSingleton<IRabbitMqService, RabbitMqService>()
            .AddSingleton<IDeliveriesInitializer, DeliveriesInitializer>();
    }
    
    private static IServiceCollection RegisterConfiguration<TConfigurationInterface, TConfigurationClass>(
        this IServiceCollection services,
        IConfiguration configuration)
        where TConfigurationInterface : class
        where TConfigurationClass : class, TConfigurationInterface
    {
        return services
            .Configure<TConfigurationClass>(
                configuration.GetSection(typeof(TConfigurationClass).Name)
            )
            .AddSingleton<TConfigurationInterface>(x =>
                x.GetRequiredService<IOptions<TConfigurationClass>>().Value
            );
    }
}