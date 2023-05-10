using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace DeliveryBackground;

public static class ConfigureHostBuilderExtensions
{
    public static IHostBuilder ConfigureHostBuilder(this IHostBuilder hostBuilder)
    {
        hostBuilder
            .AddSerilog();

        return hostBuilder;
    }

    private static void AddSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder
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