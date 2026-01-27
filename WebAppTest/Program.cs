using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

partial class Program
{
    static public int Main(string[] args)
    {
        #region CONFIG
        const string DB_PATH = "H:/!Вяч/WEB Development/database/users-db";
        #endregion CONFIG


        var rg = new RGWebAdapter(DB_PATH);
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
        builder.Services.AddAuthorization();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        #region  REQUEST
        //Запросы
        app.MapGet("/", () => "Ask random to generate random value");
        app.MapGet("/random", [Authorize] () => rg.GetValue());

        app.MapPost("/random_params_json", [Authorize] ([FromBody] Borders borders)
        => rg.GetValue(borders.low, borders.up)).DisableAntiforgery();

        // app.MapPost("/test", [Authorize] ([FromBody] TestContent_JSON t) => rg.Test(t)).DisableAntiforgery();
        app.MapPost("/random_generate_array", [Authorize] ([FromBody] Array_Json value)
        => rg.ArrayGeneration(value.low, value.up, value.count)).DisableAntiforgery();
        app.MapPost("/write_generate_array", [Authorize] ([FromBody] Write_Array_Json value, HttpContext context)
        => rg.ArrayWrite(value.start_array, context.User.Identity.Name)).DisableAntiforgery(); //Write Now

        app.MapGet("/current_user", [Authorize] (HttpContext context)
        => rg.Current_user(context));

        //Запрос для входа в систему
        app.MapPost("/login", async (string login, string password, HttpContext context)
        => rg.Login(login, password, context));

        //Запрос регистрации
        app.MapPost("/signup", (string login, string password) => rg.Signup(login, password));
        #endregion REQUEST
        
        
        app.Run();
        rg.DisconnectBD();
        return 0;
    }
}


//Основной код
struct Borders
{
    public int low {get; set;}
    public int up {get; set;}
}

// public struct TestContent_JSON
// {
//     public int low {get; set;}
//     public  int up {get; set;}
//     public int count {get; set;}
// }

public struct Array_Json
{
    public int low {get; set;}
    public int up {get; set;}
    public int count {get; set;}
}

public struct Write_Array_Json
{
    public string start_array {get; set;}
}

public struct RGResult
{
    public RGResult(int rv)
    {
        random_value = rv;
    }
    public int random_value { get; set; }
}

public class RGWebAdapter
{
    private DBManager db;
    private RandomGenerator rg = new RandomGenerator();
    public RGWebAdapter(string DB_PATH)
    {
        db = new DBManager(DB_PATH);
        if (!db.ConnectionBD)
        {
            Console.WriteLine("Ошибка соединения с БД " + DB_PATH);
            Console.WriteLine("Выключение");
        }
    }
    public void DisconnectBD() => db.Disconnect();
    //public bool ConnectionBD => db.ConnectionBD;

    public async Task<IResult> Login(string login, string password, HttpContext context)
    {
        if (!db.CheckUser(login, password))
            return Results.Unauthorized();
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, login)};
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookie");
        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        return Results.Ok();
    }
    public IResult Signup(string login, string password)
    {
        if (db.AddUser(login, password))
            return Results.Ok("Пользователь " + login + " успешно зарегистрирован");
        else
            return Results.Problem("Ошибка регистрации пользователя " + login);
    }
    public IResult Current_user(HttpContext context)
    {
        if (context.User.Identity == null)
            return Results.BadRequest("Имя не найдено");
        return Results.Ok(context.User.Identity.Name);
    }
    public IResult GetValue()
    {
        return Results.Ok(new RGResult(rg.GetValue()));
    }

    public IResult GetValue(int lb, int ub)
    {
        if (lb >= ub)
            return Results.Conflict("Не могу сгенерировать значение с помощью указанных рамок");

        return Results.Ok(new RGResult(rg.GetValue(lb, ub)));
    }

    public IResult UpdateBorder(int lb, int ub)
    {
        if (rg.UpdateBorder(lb, ub))
            return Results.Ok("Рамки обновлены успешно!");

        return Results.Conflict("Не могу обновить рамки с помощью выбранных значений");
    }

    // public IResult Test(TestContent_JSON test)
    // {
    //     var res = new Dictionary<string, object>();
    //     res.Add("date time", DateTime.Now);
    //     res.Add("data", test);
    //     return Results.Ok(res);
    // }

    public IResult ArrayGeneration(int low, int up, int count) //Генерация массива 
    {
        Random rdn = new Random();
        int[] res = new int[count];

        for (int i = 0; i < count; i++)
        {
            res[i] = rdn.Next(low, up);
        }

        return Results.Ok(res);
    }

    public IResult ArrayWrite(string? start_array, string login) //Ручное заполнение массива
    {
        string[]? string_numbers = start_array?.Split(' ');

        if(string_numbers == null) return Results.Conflict("error format");
        var res = new int[string_numbers.Length];

        for (int i = 0; i < string_numbers.Length; i++)
        {
            if (int.TryParse(string_numbers[i], out int num))
            {
                res[i] = num;
            }
            else
            {
                Console.WriteLine($"Ошибка: '{string_numbers[i]}' не является числом. Будет заменено на 0.");
                res[i] = 0;
            }
        }
        return Results.Ok(res);
    }
}

public class RandomGenerator
{
    private int low_border;
    private int up_border;
    private Random random;

    public RandomGenerator(int lb = 0, int ub = 10)
    {
        low_border = lb;
        up_border = ub;
        random = new Random();
    }

    public int GetValue()
    {
        return random.Next(low_border, up_border);
    }

    public int GetValue(int lb, int ub)
    {
        return random.Next(lb, ub);
    }

    public bool UpdateBorder(int lb, int ub)
    {
        if (ub <= lb)
            return false;
        
        low_border = lb;
        up_border = ub;

        return true;
    }
}

