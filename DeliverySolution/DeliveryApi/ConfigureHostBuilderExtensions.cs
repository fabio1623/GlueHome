using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace DeliveryApi;

public static class ConfigureHostBuilderExtensions
{
    public static WebApplicationBuilder ConfigureHostBuilder(this WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder
            .Host
            .AddSerilog();

        return webApplicationBuilder;
    }

    private static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder
            .UseSerilog((context, services, configuration) =>
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithEnvironmentUserName()
                    .WriteTo.Console()
                    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticSearchConfiguration:ConnectionString"] ?? string.Empty))
                    {
                        IndexFormat = $"{context.Configuration["Serilog:Properties:Application"]}-logs-{context.HostingEnvironment.EnvironmentName.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
                    })
            );
    }
}