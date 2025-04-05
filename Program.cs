using TimeSnapBackend_MySql.Controllers;
using TimeSnapBackend_MySql.Extensions;
using TimeSnapBackend_MySql.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using TimeSnapBackend_MySql.Middlewares;

namespace TimeSnapBackend_MySql
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;

            services.AddDistributedMemoryCache();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5); // OTP expires in 5 mins
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            services.AddControllers();

            // Inject MySQL DB Context
            services.InjectDbContext(builder.Configuration);

            services.AddSwaggerExplorer()
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

            var app = builder.Build();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSession();
            app.UseRouting();

            app.ConfigureSwaggerExplorer()
                .ConfigureCORS(builder.Configuration)
                .AddIdentityAuthMiddlewares();

            app.UseHttpsRedirection();

            app.MapControllers();
            app.MapGroup("/api")
               .MapIdentityApi<AppUser>();
            app.MapGroup("/api")
               .MapIdentityUserEndpoints()
               .MapAccountEndpoints()
               .MapAuthorizationDemoEndpoints();

            app.Run();
        }
    }
}
