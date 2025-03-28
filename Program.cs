using TimeSnapBackend_MySql.Controllers;
using TimeSnapBackend_MySql.Extensions;
using TimeSnapBackend_MySql.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace TimeSnapBackend_MySql
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(5); // OTP expires in 5 mins
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            builder.Services.AddControllers();

            // Inject MySQL DB Context
            builder.Services.InjectDbContext(builder.Configuration);

            builder.Services.AddSwaggerExplorer()
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

            var app = builder.Build();

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
