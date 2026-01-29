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
            //Запрос для смены пароля
            app.MapPost("/new_pwd", [Authorize]([FromBody] New_Pwd dataNewPWD, HttpContext context)
            => rg.NewPassword(context.User.Identity?.Name, dataNewPWD.new_password));
            //Запрос для регистрации в системе
            app.MapPost("/signup", ([FromBody] DataLogin data_login)
            => rg.Signup(data_login.username, data_login.password));
            //Запрос проверки пользователя
            app.MapGet("/current_user", [Authorize] (HttpContext context)
            => rg.Current_user(context));
            
            //Сгенерировать случайный массив
            app.MapPost("/random_generate_array", [Authorize] ([FromBody] Random_Generate_Array value, HttpContext context)
            => rg.ArrayGeneration(context.User.Identity?.Name, value.low, value.up, value.count)); 
            //Передать массив на сервер
            app.MapPost("/write_generate_array", [Authorize] ([FromBody] Write_Generate_Array value, HttpContext context)
            => rg.ArrayWrite(context.User.Identity?.Name, value.start_array));

            //Получить отсортированный массив
            app.MapGet("/give_combsort", [Authorize] (HttpContext context)
            => rg.ArrayCombSort(context.User.Identity?.Name));
            //Получить отсортированную часть массива от индекса до индекса
            app.MapGet("/give_combsort_index", [Authorize] ([FromBody] Array_Index value, HttpContext context)
            => rg.ArrayCombSortIndex(context.User.Identity?.Name, value.start_index, value.finish_index));
            //Отсортировать массив
            app.MapPost("/write_combsort", [Authorize] (HttpContext context)
            => rg.ArrayCombSortWrite(context.User.Identity?.Name));
            //Удалить массив
            app.MapDelete("/del_array", [Authorize] (HttpContext context)
            => rg.ArrayDelete(context.User.Identity?.Name)); 
            app.MapPatch("/add_value_start", [Authorize] ([FromBody] int value, HttpContext context)
            => rg.AddValueStart(context.User.Identity?.Name, value));
            
            #endregion REQUEST


            app.Run();
            rg.DisconnectBD();
            return 0;
        }
    }
}