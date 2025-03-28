using TimeSnapBackend_MySql.Models;
using Microsoft.EntityFrameworkCore;

namespace TimeSnapBackend_MySql.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(
            this WebApplication app,
            IConfiguration config)
        {
            app.UseCors("AllowAll"); // Apply CORS globally

            // Set COOP & COEP headers for Google Login & postMessage support
            app.Use(async (context, next) =>
            {
                context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin-allow-popups"; // More secure
                context.Response.Headers["Cross-Origin-Resource-Policy"] = "cross-origin";
                await next();
            });

            return app;
        }

        public static IServiceCollection AddAppConfig(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            // Define named CORS policy with allowed origins
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                    builder.WithOrigins(
                        "http://localhost:4200", // Add frontend URL
                        "https://time-snap-frontend-emfd6q0od-sai-vishnus-projects-536bb392.vercel.app",
                        "https://time-snap-frontend-git-main-sai-vishnus-projects-536bb392.vercel.app",
                        "https://time-snap-frontend.vercel.app",
                        "https://time-snap-frontend.vercel.app/",
                        "https://timesnaper.azurewebsites.net/",
                        "https://timesnaper.azurewebsites.net" // url should not contains '/' at last.
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); // Ensure credentials can be shared
            });

            return services;
        }
    }


}
