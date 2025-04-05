using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TimeSnapBackend_MySql.Middlewares;

namespace TimeSnapBackend_MySql.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }

        public static IApplicationBuilder UseNotFoundHandler(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        StatusCode = 404,
                        Message = "The requested resource was not found."
                    }));
                }
            });
        }
    }
}
