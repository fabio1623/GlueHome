using DeliveryApi;

await WebApplication
    .CreateBuilder(args)
    .ConfigureHostBuilder()
    .ConfigureServiceCollection()
    .Build()
    .ConfigureWebApplication()
    .ConfigureDatabase()
    .Result
    .RunAsync();
