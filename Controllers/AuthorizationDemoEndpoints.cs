using Microsoft.AspNetCore.Authorization;

namespace TimeSnapBackend_MySql.Controllers
{
    public static class AuthorizationDemoEndpoints
    {

        public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/AdminOnly", AdminOnly);

            app.MapGet("/AdminOrManager", [Authorize(Roles = "Admin,Manager")] () =>
            { return "Admin Or Manager"; });

            return app;
        }

        [Authorize(Roles = "Admin")]
        private static string AdminOnly()
        { return "Admin Only"; }
    }

}
