using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAppTest.RequestStructures;

namespace WebAppTest
{
    partial class Program
    {
        static public int Main(string[] args)
        {
            #region CONFIG
            const string DB_PATH = "H:/!Вяч/WEB Development/database/users-db";
            #endregion CONFIG


            var rg = new WebAdapter(DB_PATH);
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            #region  REQUEST
            app.MapGet("/", () => "Ask random to generate random value");
            //Запрос для входа в систему
            app.MapPost("/login", ([FromBody] DataLogin data_login, HttpContext context)
            => rg.Login(data_login.username, data_login.password, context));
            //Запрос для регистрации в системе
            app.MapPost("/signup", ([FromBody] DataLogin data_login)
            => rg.Signup(data_login.username, data_login.password));

            app.MapPost("/random_generate_array", [Authorize] ([FromBody] Random_Generate_Array value, HttpContext context)
            => rg.ArrayGeneration(context.User.Identity?.Name, value.low, value.up, value.count)).DisableAntiforgery();
            app.MapPost("/write_generate_array", [Authorize] ([FromBody] Write_Generate_Array value, HttpContext context)
            => rg.ArrayWrite(context.User.Identity?.Name, value.start_array)).DisableAntiforgery(); //Write Now

            app.MapGet("/current_user", [Authorize] (HttpContext context)
            => rg.Current_user(context));
            #endregion REQUEST


            app.Run();
            rg.DisconnectBD();
            return 0;
        }
    }
}