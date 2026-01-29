using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics.Eventing.Reader;
using SixLabors.ImageSharp.Formats.Bmp;

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
            var record = new DBManager.Record("POST", true, "Login user");
            db.UpDateHistory(username, record);
            return Results.Ok();
        }
        public IResult NewPassword(string? login, string new_password)
        {
            if(string.IsNullOrEmpty(login)) return Results.Conflict("Login invalid");
            if (!db.CheckUser(login))
                return Results.Conflict();
            if(db.NewPassword(login, new_password))
            {
                var record = new DBManager.Record("POST", true, "New pwd");
                db.UpDateHistory(login, record);
                return Results.Ok();
            }
            else
            {
                var record = new DBManager.Record("POST", false, "Fail update pwd");
                db.UpDateHistory(login, record);
                return Results.Conflict();
            }
        }
        public IResult Signup(string login, string password)
        {
            if (db.AddUser(login, password))
            {
                var record = new DBManager.Record("POST", true, "New user");
                db.UpDateHistory(login, record);
                return Results.Ok($"Пользователь \"{login}\" успешно зарегистрирован");
            }
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
            // Console.Write($"Res:|");
            // Console.WriteLine(string.Join(", ", res) + "|");

            db.AddArr(login, res);

            return Results.Ok(res);
        }

        public IResult ArrayCombSort(string? login) //Получить отсортированный массив
        {
            if (string.IsNullOrEmpty(login))
                return Results.Conflict("Ошибка: Логина");
            var array = db.GiveArray(login);

            if (array == null)
                return Results.Conflict("Ошибка: массив пуст");

            
            double factor = 1.2473309;
            int step = array.Length - 1;

            while (step >= 1)
            {
                for (int i = 0; i + step < array.Length; i++)
                {
                    if (array[i] > array[i + step])
                    {
                        int temp = array[i];
                        array[i] = array[i + step];
                        array[i + step] = temp;
                    }
                }
                step = (int)(step / factor);
            }
            
            return Results.Ok(array);
        }

        public IResult ArrayCombSortIndex(string? login, int start_index, int finish_index) //Получить отсортированный массив
        {
            if (string.IsNullOrEmpty(login))
                return Results.Conflict("Ошибка: Логина");
            var array = db.GiveArray(login);

            if (array == null)
                return Results.Conflict("Ошибка: массив пуст");
            
            double factor = 1.2473309;
            int step = array.Length - 1;

            while (step >= 1)
            {
                for (int i = 0; i + step < array.Length; i++)
                {
                    if (array[i] > array[i + step])
                    {
                        int temp = array[i];
                        array[i] = array[i + step];
                        array[i + step] = temp;
                    }
                }
                step = (int)(step / factor);
            }

            int length = finish_index - start_index + 1;
            int[] newArray = new int[length];

            for (int i = 0; i < length; i++)
            {
                newArray[i] = array[start_index + i];
            }

            return Results.Ok(newArray);
        }

        public IResult ArrayCombSortWrite(string? login) //Отсортировать массив в БД
        {
            if (string.IsNullOrEmpty(login))
                return Results.Conflict("Ошибка: Логина");
            var array = db.GiveArray(login);

            if (array == null)
                return Results.Conflict("Ошибка: массив пуст");
            
            double factor = 1.2473309;
            int step = array.Length - 1;

            while (step >= 1)
            {
                for (int i = 0; i + step < array.Length; i++)
                {
                    if (array[i] > array[i + step])
                    {
                        int temp = array[i];
                        array[i] = array[i + step];
                        array[i + step] = temp;
                    }
                }
                step = (int)(step / factor);
            }

            db.AddArr(login, array);
            return Results.Ok();
        }

        public IResult ArrayDelete(string? login) //Удалить массив
        {
            if (string.IsNullOrEmpty(login))
                return Results.Conflict("Ошибка: Логина");
            db.DelArr(login);
            
            return Results.Ok();
        }

        public IResult AddValueStart(string? login, int value) //Добавление элемента в начало массива
        {
            if (string.IsNullOrEmpty(login))
                return Results.Conflict("Ошибка: Логина");
            var res = db.GiveArray(login);

            if (res == null) 
            {
                db.AddArr(login, [value]);
                return Results.Ok();
            }

            int[] array = new int[res.Length + 1];
            array[0] = value;

            for (int i = 1; i < array.Length; i++)
                array[i] = res[i-1];

            db.AddArr(login, array);
            return Results.Ok();
        }
    }
}
