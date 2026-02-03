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
            const string DB_PATH = "H:/!Вяч/WEB Development/database/users-db"; //Путь к базе данных
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
            app.MapGet("/", () => "Comb Sorting Client-Server Application");
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
            

            //Запрос на получение истории запросов
            app.MapGet("/get_history", [Authorize] (HttpContext context)
            => rg.Get_History(context.User.Identity?.Name));
            //Запрос на отчистку истории запросов
            app.MapDelete("/del_history", [Authorize] (HttpContext context)
            => rg.Del_History(context.User.Identity?.Name));

            //Сгенерировать случайный массив
            app.MapPost("/random_generate_array", [Authorize] ([FromBody] Random_Generate_Array value, HttpContext context)
            => rg.ArrayGeneration(context.User.Identity?.Name, value.low, value.up, value.count)); 
            //Передать массив на сервер
            app.MapPost("/write_generate_array", [Authorize] ([FromBody] Write_Generate_Array value, HttpContext context)
            => rg.ArrayWrite(context.User.Identity?.Name, value.start_array));

            //Просмотр массива
            app.MapGet("/give_array", [Authorize] (HttpContext context)
            => rg.ArrayGive(context.User.Identity?.Name));

            //Получить отсортированный массив
            app.MapGet("/give_combsort", [Authorize] (HttpContext context)
            => rg.ArrayCombSort(context.User.Identity?.Name));
            //Получить отсортированную часть массива от индекса до индекса
            app.MapGet("/give_combsort_index", [Authorize] (int start_index, int finish_index, HttpContext context)
            => rg.ArrayCombSortIndex(context.User.Identity?.Name, start_index, finish_index));
            //Отсортировать массив
            app.MapPost("/write_combsort", [Authorize] (HttpContext context)
            => rg.ArrayCombSortWrite(context.User.Identity?.Name));

            //Удалить массив
            app.MapDelete("/del_array", [Authorize] (HttpContext context)
            => rg.ArrayDelete(context.User.Identity?.Name)); 
            
            //Добавить элемент в начало массива
            app.MapPatch("/add_value_start", [Authorize] ([FromBody] Array_Add_Value value, HttpContext context)
            => rg.AddValueStart(context.User.Identity?.Name, value.value));
            //Добавить элемент в конец массива
            app.MapPatch("/add_value_finish", [Authorize] ([FromBody] Array_Add_Value value, HttpContext context)
            => rg.AddValueFinish(context.User.Identity?.Name, value.value));
            //Добавить элемент после указанного индекса
            app.MapPatch("/add_value_index", [Authorize] ([FromBody] Array_Add_Index value, HttpContext context)
            => rg.AddValueIndex(context.User.Identity?.Name, value.value, value.index));
            #endregion REQUEST


            app.Run();
            rg.DisconnectBD();
            return 0;
        }
    }
}