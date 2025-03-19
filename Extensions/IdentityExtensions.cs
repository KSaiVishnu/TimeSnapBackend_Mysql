using TimeSnapBackend_MySql.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace TimeSnapBackend_MySql.Extensions
{
    public class CustomEmailValidator<TUser> : IUserValidator<TUser> where TUser : IdentityUser
    {
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            if (!user.Email!.EndsWith("@gmail.com") && !user.Email.EndsWith("@framsikt.no"))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidEmailDomain",
                    Description = "Only email addresses ending with @gmail.com or @framsikt.no are allowed."
                }));
            }


            return Task.FromResult(IdentityResult.Success);
        }
    }

    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<AppUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();

            return services;
        }

        public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            });

            services.AddScoped<IUserValidator<AppUser>, CustomEmailValidator<AppUser>>();

            return services;
        }

        public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
        {
            //var jwtSecret = config["AppSettings:JWTSecret"];
            var jwtSecret = Environment.GetEnvironmentVariable("JWTSecret")
    ?? config["AppSettings:JWTSecret"];


            if (string.IsNullOrEmpty(jwtSecret))
            {
                throw new Exception("JWT Secret is missing! Set 'JWTSecret' in environment variables.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }

        public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
