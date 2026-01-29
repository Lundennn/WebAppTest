using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebAppTest
{
    /// <summary>
    /// Веб адаптер -- Реализует все функции сервера -- Ядро
    /// </summary>
    public class WebAdapter
    {
        /// <summary>
        /// Доступ к БД
        /// </summary>
        private DBManager db;
        /// <summary>
        /// Сеализация объекта
        /// </summary>
        /// <param name="DB_PATH">Путь до БД</param>
        public WebAdapter(string DB_PATH)
        {
            db = new DBManager(DB_PATH);
            if (!db.ConnectionBD)
            {
                Console.WriteLine("Ошибка соединения с БД " + DB_PATH);
                Console.WriteLine("Выключение");
            }
        }
        ~WebAdapter() => db.Disconnect();
        public void DisconnectBD() => db.Disconnect();
        //public bool ConnectionBD => db.ConnectionBD;

        public async Task<IResult> Login(string username, string pwd, HttpContext context)
        {
            if (!db.CheckUser(username, pwd))
                return Results.Unauthorized();
            var claims = new List<Claim> { new(ClaimTypes.Name, username) };
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookie");
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Results.Ok();
        }
        public IResult Signup(string login, string password)
        {
            if (db.AddUser(login, password))
                return Results.Ok($"Пользователь \"{login}\" успешно зарегистрирован");
            else
                return Results.Problem("Ошибка регистрации пользователя " + login);
        }
        public IResult Current_user(HttpContext context)
        {
            if (context.User.Identity == null)
                return Results.BadRequest("Имя не найдено");
            return Results.Ok(context.User.Identity.Name);
        }

        public IResult ArrayGeneration(string? login, int low, int up, int count) //Генерация массива 
        {
            if (string.IsNullOrEmpty(login)) return Results.Conflict();
            Random rdn = new Random();
            int[] res = new int[count];

            for (int i = 0; i < count; i++)
                res[i] = rdn.Next(low, up);

            db.AddArr(login, res);

            return Results.Ok(res);
        }

        public IResult ArrayWrite(string? login, string? start_array) //Ручное заполнение массива
        {
            if (string.IsNullOrEmpty(login)) return Results.Conflict();
            string[]? string_numbers = start_array?.Split(' ');

            if (string_numbers == null) return Results.Conflict("error format");
            var res = new int[string_numbers.Length];

            for (int i = 0; i < string_numbers.Length; i++)
            {
                if (int.TryParse(string_numbers[i], out int num))
                    res[i] = num;
                else
                {
                    Console.WriteLine($"Ошибка: '{string_numbers[i]}' не является числом. Будет заменено на 0.");
                    res[i] = 0;
                }
            }
            Console.Write($"Res:|");
            Console.WriteLine(string.Join(", ", res) + "|");

            db.AddArr(login, res);

            return Results.Ok(res);
        }
    }
}
