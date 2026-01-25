using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

RGWebAdapter rg = new RGWebAdapter();
DBManager db = new DBManager();

app.MapGet("/", () => "Ask random to generate random value");
app.MapGet("/random", [Authorize] () => rg.GetValue());
app.MapGet("/random_params", (int low, int up) => rg.GetValue(low, up));

app.MapPost("/random_params_form", ([FromForm] int low, [FromForm] int up) => rg.GetValue(low, up)).DisableAntiforgery();
app.MapPost("/random_params_header", ([FromHeader] int low, [FromHeader] int up) => rg.GetValue(low, up)).DisableAntiforgery();
app.MapPost("/random_params_json", ([FromBody] Borders borders) => rg.GetValue(borders.low, borders.up)).DisableAntiforgery();

app.MapPost("/picture", (HttpRequest request) =>
{
    try
    {
        var memoryStream = new MemoryStream();
        request.Body.CopyToAsync(memoryStream).Wait();
        memoryStream.Seek(0, SeekOrigin.Begin);
        Image image = Image.Load<Rgba32>(memoryStream);

        return Results.Ok("Received image " + image.Width + "x" + image.Height);
    }
    catch (Exception exp)
    {
        return Results.BadRequest("Wrong image: " + exp.Message);
    }
});

app.MapGet("/current_user", [Authorize] (HttpContext context) =>
{
    if (context.User.Identity == null)
        return Results.BadRequest("Not found username");
    return Results.Ok(context.User.Identity.Name);
});

app.MapPost("/login", async (string login, string password, HttpContext context) =>
{
    if (!db.CheckUser(login, password))
        return Results.Unauthorized();

    var claims = new List<Claim> {new Claim(ClaimTypes.Name, login)};
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookie");
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

    return Results.Ok();
});

app.MapPost("/signup", (string login, string password) => {
    if (db.AddUser(login, password))
        return Results.Ok("User " + login + " registered successfull");
    else
        return Results.Problem("failed to register user " + login);
});

const string DB_PATH = "/home/student/database/users-db";
if (!db.ConnectToDB(DB_PATH))
{
    Console.WriteLine("Failed to connect to db " + DB_PATH);
    Console.WriteLine("Shutdown");
    return;
}
app.Run();
db.Disconnect();

struct Borders
{
    public int low {get; set;}
    public int up {get; set;}
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
    private RandomGenerator rg = new RandomGenerator();

    public IResult GetValue()
    {
        return Results.Ok(new RGResult(rg.GetValue()));
    }

    public IResult GetValue(int lb, int ub)
    {
        if (lb >= ub)
            return Results.Conflict("Can't generate value with such borders");

        return Results.Ok(new RGResult(rg.GetValue(lb, ub)));
    }

    public IResult UpdateBorder(int lb, int ub)
    {
        if (rg.UpdateBorder(lb, ub))
            return Results.Ok("Borders are update sucessfully");

        return Results.Conflict("Can't update borders with such values");
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