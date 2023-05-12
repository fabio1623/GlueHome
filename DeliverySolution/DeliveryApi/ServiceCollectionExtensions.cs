using System.Text.Json;
using System.Text.Json.Serialization;
using DeliveryApi.AutoMapperProfiles;
using DeliveryApi.Configurations;
using DeliveryDomain.Businesses;
using DeliveryDomain.Interfaces.Businesses;
using DeliveryDomain.Interfaces.Configurations;
using DeliveryDomain.Interfaces.Initializers;
using DeliveryDomain.Interfaces.Services;
using DeliveryInfrastructure.AutoMapperProfiles;
using DeliveryInfrastructure.Initializers;
using DeliveryInfrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace DeliveryApi;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder ConfigureServiceCollection(this WebApplicationBuilder webApplicationBuilder)
    {
        var configurationManager = webApplicationBuilder.Configuration;

        webApplicationBuilder
            .Services
            .AddCors()
            .AddResponseCompression()
            .AddConfigurations(configurationManager)
            .AddAutoMapperProfiles()
            .AddFluentValidationConfigurations()
            .AddRabbitMqConfigurations(configurationManager)
            .AddBusinesses()
            .AddServices()
            .AddEndpointsApiExplorer()
            .AddSwagger()
            .AddControllers()
            .AddJsonConfigurations();

        return webApplicationBuilder;
    }
    
    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .RegisterConfiguration<IElasticSearchConfiguration, ElasticSearchConfiguration>(configuration)
            .RegisterConfiguration<IMySqlConfiguration, MySqlConfiguration>(configuration)
            .RegisterConfiguration<IRabbitMqConfiguration, RabbitMqConfiguration>(configuration)
            .RegisterConfiguration<IAppSettings, AppSettings>(configuration)
            .RegisterConfiguration<IMongoDbConfiguration, MongoDbConfiguration>(configuration);
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
    
    private static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        return services
            .AddAutoMapper(typeof(ApiModelProfiles))
            .AddAutoMapper(typeof(InfrastructureModelProfiles));
    }
    
    private static IServiceCollection AddFluentValidationConfigurations(this IServiceCollection services)
    {
        return services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<Program>();
    }
    
    private static IServiceCollection AddRabbitMqConfigurations(this IServiceCollection services, IConfiguration configuration)
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
    
    private static IServiceCollection AddBusinesses(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDeliveryBusiness, DeliveryBusiness>()
            .AddScoped<IUserBusiness, UserBusiness>();
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IDeliveryService, DeliveryService>()
            .AddSingleton<IRabbitMqService, RabbitMqService>()
            .AddSingleton<IJwtUtils, JwtUtils>()
            .AddScoped<IUserService, UserService>()
            .AddSingleton<IDeliveriesInitializer, DeliveriesInitializer>()
            .AddSingleton<IUsersInitializer, UsersInitializer>();
    }
    
    private static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        return services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = $"{nameof(DeliveryApi)}",
                    Description = $"Micro-service exposing {nameof(DeliveryApi)} endpoints.",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Fabio SARMENTO PEDRO",
                        Email = "sarmentopedrofabio@gmail.com",
                        Url = new Uri("https://www.linkedin.com/in/fabio-sarmento-pedro-3310a766/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://example.com/license")
                    }
                }
            );

            var securitySchema = new OpenApiSecurityScheme
            {
                Description = "Using the Authorization header with the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            x.AddSecurityDefinition("Bearer", securitySchema);

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securitySchema, new[] { "Bearer" } }
            });
        });
    }
    
    private static void AddJsonConfigurations(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                x.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
                x.JsonSerializerOptions.WriteIndented = true;
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }
}