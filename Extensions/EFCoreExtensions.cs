using TimeSnapBackend_MySql.Models;
using Microsoft.EntityFrameworkCore;

namespace TimeSnapBackend_MySql.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection InjectDbContext(
            this IServiceCollection services,
            IConfiguration config)
        {
            //var connectionString = config.GetConnectionString("DefaultConnection");
            //var connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection") 
            //                        ?? config.GetConnectionString("DefaultConnection");

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                        ?? config.GetConnectionString("DefaultConnection");


            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }

            //Console.WriteLine("Using MySQL Connection: " + connectionString);
            // Configure MySQL instead of SQL Server
            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))) // Adjust MySQL version if needed
            //);

            services.AddDbContext<AppDbContext>(options =>
                                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


            return services;
        }
    }
}
