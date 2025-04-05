using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TimeSnapBackend_MySql.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                StatusCode = 401,
                                Message = "Unauthorized: Invalid or missing token."
                            }));
                        },
                        OnForbidden = async context =>
                        {
                            context.Response.StatusCode = 403;
                            context.Response.ContentType = "application/json";
                            await context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                StatusCode = 403,
                                Message = "Access denied. You do not have permission."
                            }));
                        }
                    };
                });
            return services;
        }

        public static IServiceCollection ConfigureValidationErrors(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                        .ToList();

                    return new BadRequestObjectResult(new
                    {
                        StatusCode = 400,
                        Message = "Validation failed.",
                        Errors = errors
                    });
                };
            });
            return services;
        }
    }
}
