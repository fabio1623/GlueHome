using DeliveryApi;

await WebApplication
    .CreateBuilder(args)
    .ConfigureHostBuilder()
    .ConfigureServiceCollection()
    .Build()
    .ConfigureWebApplication()
    .ConfigureMySqlDatabase()
    .Result
    .RunAsync();
