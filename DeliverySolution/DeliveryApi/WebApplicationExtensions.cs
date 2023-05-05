using DeliveryApi.Middlewares;

namespace DeliveryApi
{
    public static class WebApplicationExtensions
    {
        public static WebApplication ConfigureWebApplication(this WebApplication webApplication)
        {
            webApplication
                .SetCorsConfigurations()
                .SetMiddlewares()
                .UseExceptionHandler("/errors")
                .UseSwagger()
                .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{nameof(DeliveryApi)}-v1"))
                .UseResponseCompression()
                .UseHttpsRedirection()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization();

            webApplication.MapControllers();

            return webApplication;
        }

        private static IApplicationBuilder SetCorsConfigurations(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseCors(x => x
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin());
        }
        
        private static IApplicationBuilder SetMiddlewares(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder
                .UseMiddleware<JwtMiddleware>();
        }
    }
}
