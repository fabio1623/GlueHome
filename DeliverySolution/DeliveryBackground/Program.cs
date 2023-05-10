using DeliveryBackground;

await Host.CreateDefaultBuilder(args)
    .ConfigureHostBuilder()
    .ConfigureServices((hostBuilderContext, serviceCollection) =>
    {
        var configuration = hostBuilderContext.Configuration;
        serviceCollection
            .AddConfigurations(configuration)
            .AddRabbitMqConfigurations(configuration)
            .AddServices()
            .AddHostedService<Worker>();
    })
    .Build()
    .RunAsync();