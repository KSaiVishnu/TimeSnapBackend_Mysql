using TimeSnapBackend_MySql.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Caching.Memory;
using Google.Apis.Auth;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;

namespace TimeSnapBackend_MySql.Controllers
{
    public class UserRegistrationModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Otp { get; set; }
    }

    public class LoginModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class OtpVerificationDto
    {
        public string? Email { get; set; }
        public string? Otp { get; set; }
    }

    public class OtpRequestModel
    {
        public string? Email { get; set; }
    }

    public class Messager
    {
        public string Message { get; set; }
        public Messager(string message)
        {
            Message = message;            
        }
    }



    // Helper Models
    public class GoogleLoginModel
    {
        public string? Token { get; set; }
    }

    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
    }

    public class GoogleUserInfo
    {
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class EmailDto
    {
        public string? Email { get; set; }
    }


    public class PasswordResetDto
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }



    public static class IdentityUserEndpoints
    {
        //public static string? ApiKey { get; private set; }

        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            var httpContextAccessor = app.ServiceProvider.GetRequiredService<IHttpContextAccessor>();

            app.MapPost("/signup", CreateUser);
            app.MapPost("/signin", SignIn);
            //app.MapPost("/google-signin", GoogleSignIn);
            //app.MapPost("/remove-user", RemoveUser);
            app.MapPost("/find-user", FindUser);
            app.MapPost("/send-otp", SendOtp);
            app.MapPost("/verify-otp", VerifyOtp);

            app.MapPost("/google-login", GoogleLogin);

            app.MapPost("/pre-register", PreRegister);


            app.MapPost("/verify-and-create", VerifyAndCreateUser);

            app.MapPost("/send-reset-otp", SendResetOtp);
           

            app.MapPost("/verify-reset-otp", VerifyResetOtp);

           

            app.MapPost("/set-new-password", SetNewPassword);



            return app;
        }



        [AllowAnonymous]
        private static async Task<IResult> GoogleLogin(
    HttpContext httpContext,
    UserManager<AppUser> userManager,
    IOptions<AppSettings> appSettings,
    IConfiguration config,
    [FromBody] GoogleLoginModel googleLoginModel)
        {
            var context = httpContext.RequestServices.GetRequiredService<AppDbContext>();

            // Verify Google Token Properly
            //var settings = new GoogleJsonWebSignature.ValidationSettings
            //{
            //    Audience = new List<string> { "Your-Google-API-Key.apps.googleusercontent.com" } // Replace with your Google Client ID
            //};

            // Verify Google Token Properly
            //var clientId = Environment.GetEnvironmentVariable("GoogleClientId")
            //   ?? config["Google:ClientId"];

            var clientId = Environment.GetEnvironmentVariable("Google__ClientId")
               ?? config["Google:ClientId"];


            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { clientId }
            };


            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(googleLoginModel.Token, settings);
            }
            catch (Exception ex)
            {
                return Results.BadRequest($"Invalid Google token: {ex.Message}");
            }

            string email = payload.Email;
            string fullName = payload.Name;

            // Check if email exists in UserEmployee table
            var employee = context.UserEmployees.FirstOrDefault(e => e.Email == email);
            if (employee == null)
            {
                return Results.BadRequest("Employee not found in UserEmployee table.");
            }

            // Check if user already exists in AppUser table (AspNetUsers)
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // If user does not exist, create a new one
                user = new AppUser()
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmpId = employee.EmployeeId // Assign EmpId from UserEmployee
                };

                var createResult = await userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    return Results.BadRequest($"User registration failed: {errors}");
                }

                // Assign default role
                await userManager.AddToRoleAsync(user, "Employee");
            }

            // Generate JWT Token
            var roles = await userManager.GetRolesAsync(user);
            //var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret));

            //var jwtSecret = Environment.GetEnvironmentVariable("JWTSecret")
            //    ?? appSettings.Value.JWTSecret;


            var jwtSecret = Environment.GetEnvironmentVariable("AppSettings__JWTSecret")
                 ?? appSettings.Value.JWTSecret;


            var signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));


            var claimsIdentity = new ClaimsIdentity(new[]
            {
        new Claim("UserID", user.Id.ToString()),
        new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "Employee"),
    });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            return Results.Ok(new { token = tokenString });
        }


        [AllowAnonymous]
        public static Messager VerifyOtp([FromBody] OtpVerificationDto model, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext; // Get HttpContext

            var storedOtp = httpContext!.Session.GetString("Otp");
            var storedEmail = httpContext.Session.GetString("Email");

            if (storedOtp != null && storedEmail != null && storedOtp == model.Otp && storedEmail == model.Email)
            {
                //return "User verified successfully.";
                return new Messager("User verified successfully.");
            }

            //return "Invalid OTP or email.";
            return new Messager("Invalid OTP or email.");
        }


        [AllowAnonymous]
        private static async Task SendOtpEmail(string email, string otp, IConfiguration config)
        {
            // Retrieve SendGrid API Key from environment variables
            //var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
            //    ?? config["SendGrid:ApiKey"];

            var apiKey = Environment.GetEnvironmentVariable("SendGrid__ApiKey")
    ?? config["SendGrid:ApiKey"];


            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("SendGrid API Key is missing! Set 'SENDGRID_API_KEY' in environment variables.");
            }

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("saivishnukamisetty@gmail.com", "Time Snap");
            var subject = "Your OTP Code";
            var to = new EmailAddress(email);
            var plainTextContent = $"Your OTP code is: {otp}. Please enter this code to complete your registration.";
            var htmlContent = $"<strong>Your OTP code is: {otp}</strong>. Please enter this code to complete your registration.";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            try
            {
                // Send the email
                var response = await client.SendEmailAsync(msg);
                Console.WriteLine($"SendGrid Response: {response.StatusCode}");

                var responseBody = await response.Body.ReadAsStringAsync();
                Console.WriteLine($"SendGrid Response Body: {responseBody}");

                // Check if email was successfully sent
                if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                {
                    throw new Exception($"Failed to send OTP email. SendGrid Response: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending OTP email: {ex.Message}");
                throw; // Rethrow for higher-level handling
            }
        }





        [AllowAnonymous]
        public static async Task<IResult> SendOtp(UserManager<AppUser> userManager, [FromBody] OtpRequestModel body, IHttpContextAccessor httpContextAccessor)
        {
            //var user = await userManager.FindByEmailAsync(email);
            var httpContext = httpContextAccessor.HttpContext; // Get HttpContext
            if (httpContext == null)
            {
                return Results.BadRequest("HttpContext is null.");
            }

            var otp = new Random().Next(100000, 999999).ToString();

            httpContext.Session.SetString("Email", body.Email!);
            httpContext.Session.SetString("Otp", otp);

            Console.WriteLine("Session Otp Set: " + httpContext.Session.GetString("Otp"));
            Console.WriteLine("Session Email Set: " + httpContext.Session.GetString("Email"));



            //await SendOtpEmail(body.Email!, otp);
            await SendOtpEmail(body.Email!, otp, httpContext.RequestServices.GetRequiredService<IConfiguration>());



            return Results.Ok(body.Email);
        }





        [AllowAnonymous]
        private static async Task<IResult> CreateUser(
            HttpContext httpContext,
            UserManager<AppUser> userManager,
            [FromBody] UserRegistrationModel userRegistrationModel)
        {
            var context = httpContext.RequestServices.GetRequiredService<AppDbContext>();
            var employee = context.UserEmployees
                                    .FirstOrDefault(e => e.Email == userRegistrationModel.Email);
            if (employee == null)
            {
                return Results.BadRequest("Employee not found in UserEmployee table.");
            }

            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                EmpId = employee.EmployeeId
            };



            var result = await userManager.CreateAsync(user, userRegistrationModel.Password!);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"User creation failed: {errors}");
            }

            await userManager.AddToRoleAsync(user, userRegistrationModel.Role!);
            return Results.Ok(result);

        }


        [AllowAnonymous]
        private static async Task<IResult> RemoveUser(UserManager<AppUser> userManager,
                ClaimsPrincipal user,
                [FromBody] dynamic body)
        {
            string userID = user.Claims.First(x => x.Type == "UserID").Value;
            //Console.WriteLine(userID);
            var userDetails = await userManager.FindByIdAsync(userID);
            if (userDetails == null)
                return Results.NotFound("User not found");
            await userManager.DeleteAsync(userDetails);
            return Results.Ok($"Deleted");
        }

        [AllowAnonymous]
        private static async Task<IResult> FindUser(UserManager<AppUser> userManager,
        [FromBody] OtpRequestModel body)
        {
            var user = await userManager.FindByEmailAsync(body.Email!);
            if (user != null)
            {
                return Results.Ok($"User Found");
            }
            return Results.NotFound("User not found");
        }


        [AllowAnonymous]
        private static async Task<IResult> SignIn(
            UserManager<AppUser> userManager,
                [FromBody] LoginModel loginModel,
                IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email!);
            //userManager.DeleteAsync(user);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password!))
            {
                var roles = await userManager.GetRolesAsync(user);
                var signInKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret)
                                );
                ClaimsIdentity claims = new(
                    [
                        new("UserID",user.Id.ToString()),
                        new(ClaimTypes.Role,roles.First()),
                    ]);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddDays(10),
                    SigningCredentials = new SigningCredentials(
                        signInKey,
                        SecurityAlgorithms.HmacSha256Signature
                        )
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Results.Ok(new { token });
            }
            else
                return Results.BadRequest(new { message = "Username or password is incorrect." });
        }












            [AllowAnonymous]
            public static async Task<IResult> PreRegister(
                HttpContext http,
                AppDbContext db,
                UserManager<AppUser> userManager,
                IHttpContextAccessor httpContextAccessor,
                [FromBody] EmailDto dto)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(dto.Email))
                        return Results.BadRequest("Email is required.");

                    var email = dto.Email.Trim().ToLower();

                    var emp = await db.UserEmployees.FirstOrDefaultAsync(e => e.Email.ToLower() == email);
                    if (emp == null)
                        return Results.BadRequest("You are not authorized to register.");

                    var existingUser = await db.AppUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
                    if (existingUser != null)
                        return Results.BadRequest("User already registered.");

                    // Reuse your existing SendOtp method
                    var otpRequest = new OtpRequestModel { Email = dto.Email };
                    return await SendOtp(userManager, otpRequest, httpContextAccessor);
                }
                catch (Exception)
                {
                    return Results.Problem("Something went wrong. Please try again later.");
                }
            }











        //    [AllowAnonymous]
        //    public static async Task<IResult> VerifyAndCreateUser(

        //IHttpContextAccessor httpContextAccessor,
        //UserManager<AppUser> userManager,
        //[FromBody] UserRegistrationModel model)
        //    {
        //        var httpContext = httpContextAccessor.HttpContext; // Get HttpContext

        //        Console.WriteLine("Session Keys:");
        //        foreach (var key in httpContext.Session.Keys)
        //        {
        //            Console.WriteLine($"Key: {key}, Value: {httpContext.Session.GetString(key)}");
        //        }


        //        var storedOtp = httpContext!.Session.GetString("Otp");
        //        var storedEmail = httpContext.Session.GetString("Email");

        //        var receivedOtp = model.Otp;

        //        Console.WriteLine("Stored OTP: " + storedOtp);
        //        Console.WriteLine("Received OTP: " + receivedOtp);

        //        if (storedOtp != receivedOtp)
        //        {
        //            return Results.BadRequest("Incorrect OTP");
        //        }



        //        if (storedOtp == null || storedEmail == null || model.Email != storedEmail || model.Otp != storedOtp)
        //        {
        //            return Results.BadRequest(new { message = "Invalid OTP or email." });
        //        }

        //        var context = httpContext.RequestServices.GetRequiredService<AppDbContext>();
        //        var employee = context.UserEmployees.FirstOrDefault(e => e.Email == model.Email);

        //        if (employee == null)
        //        {
        //            return Results.BadRequest(new { message = "Employee not found." });
        //        }

        //        AppUser user = new AppUser
        //        {
        //            UserName = model.Email,
        //            Email = model.Email,
        //            FullName = model.FullName,
        //            EmpId = employee.EmployeeId
        //        };

        //        var result = await userManager.CreateAsync(user, model.Password!);

        //        if (!result.Succeeded)
        //        {
        //            return Results.BadRequest(new { errors = result.Errors });
        //        }

        //        await userManager.AddToRoleAsync(user, model.Role!);
        //        return Results.Ok(new { succeeded = true });
        //    }





        [AllowAnonymous]
        public static async Task<IResult> VerifyAndCreateUser(
    IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager,
    IOptions<AppSettings> appSettings,
    [FromBody] UserRegistrationModel model)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null) return Results.BadRequest("HttpContext is null.");

            var storedOtp = httpContext.Session.GetString("Otp");
            var storedEmail = httpContext.Session.GetString("Email");

            if (storedOtp == null || storedEmail == null || model.Email != storedEmail || model.Otp != storedOtp)
                return Results.BadRequest(new { message = "Invalid OTP or email." });

            var context = httpContext.RequestServices.GetRequiredService<AppDbContext>();
            var employee = context.UserEmployees.FirstOrDefault(e => e.Email == model.Email);

            if (employee == null)
                return Results.BadRequest(new { message = "Employee not found." });

            AppUser user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                EmpId = employee.EmployeeId
            };

            var result = await userManager.CreateAsync(user, model.Password!);
            if (!result.Succeeded)
                return Results.BadRequest(new { errors = result.Errors });

            await userManager.AddToRoleAsync(user, model.Role!);

            // Generate JWT token on successful registration
            var roles = await userManager.GetRolesAsync(user);
            var signInKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret)
            );

            ClaimsIdentity claims = new(
            [
                new("UserID", user.Id.ToString()),
        new(ClaimTypes.Role, roles.FirstOrDefault() ?? "Employee"),
    ]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return Results.Ok(new { token });
        }






        [AllowAnonymous]
        public static async Task<IResult> SendResetOtp(
    [FromBody] EmailDto request,
   IHttpContextAccessor httpContextAccessor,
    AppDbContext db)
        {
            var httpContext = httpContextAccessor.HttpContext; // Get HttpContext
            if (httpContext == null)
            {
                return Results.BadRequest("HttpContext is null.");
            }

            // Check if user exists
            var user = await db.AppUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (user == null)
                return Results.BadRequest(new { message = "Email not registered." });

            var otp = new Random().Next(100000, 999999).ToString();
            httpContext.Session.SetString("ResetEmail", request.Email);
            httpContext.Session.SetString("ResetOtp", otp);

            await SendOtpEmail(request.Email, otp, httpContext.RequestServices.GetRequiredService<IConfiguration>()); // Use your email service
            return Results.Ok();
        }





        [AllowAnonymous]
        public static async Task<IResult> VerifyResetOtp(
    [FromBody] OtpVerificationDto model,
    IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext == null)
                return Results.BadRequest(new { message = "HttpContext is null." });

            var storedEmail = httpContext.Session.GetString("ResetEmail");
            var storedOtp = httpContext.Session.GetString("ResetOtp");

            if (storedEmail == null || storedOtp == null)
                return Results.BadRequest(new { message = "OTP expired or not requested." });

            Console.WriteLine("req email"+model.Email);
            Console.WriteLine("req otp"+model.Otp);

            Console.WriteLine("stored mail"+storedEmail);
            Console.WriteLine("stored otp"+storedOtp);

            //if (model.Email != storedEmail || model.Otp != storedOtp)
            //    return Results.BadRequest(new { message = "Invalid OTP or email." });

            if (model.Email.Trim().ToLower() != storedEmail.Trim().ToLower() || model.Otp.Trim() != storedOtp.Trim())
            {
                return Results.BadRequest(new { message = "Invalid OTP or email." });
            }


            // Optionally clear OTP from session after successful verification
            httpContext.Session.Remove("ResetOtp");

            return Results.Ok(new { message = "OTP verified successfully." });
        }








        [AllowAnonymous]
        public static async Task<IResult> SetNewPassword(
            [FromBody] PasswordResetDto model,
            IHttpContextAccessor http,
            UserManager<AppUser> userManager)
        {
            var session = http.HttpContext!.Session;
            var email = session.GetString("ResetEmail");

            if (email == null || model.Email != email)
                return Results.BadRequest(new { message = "OTP not verified or session expired." });

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Results.BadRequest(new { message = "User not found." });

            var hasPassword = await userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await userManager.ResetPasswordAsync(user, token, model.NewPassword!);

                if (!resetResult.Succeeded)
                    return Results.BadRequest(new { message = "Failed to reset password.", errors = resetResult.Errors });
            }
            else
            {
                var addResult = await userManager.AddPasswordAsync(user, model.NewPassword!);

                if (!addResult.Succeeded)
                    return Results.BadRequest(new { message = "Failed to set password.", errors = addResult.Errors });
            }

            // Clean session after password is set
            session.Remove("ResetEmail");
            session.Remove("ResetOtp");

            return Results.Ok(new { message = "Password set successfully." });
        }





    }
}

