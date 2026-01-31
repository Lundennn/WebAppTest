namespace WebAppClient
{
    partial class App
    {
        /// <summary>
        /// Хост для подключения к серверу
        /// </summary>
        const string DEFAULT_SERVER_URL = "http://localhost:5000";
        /// <summary>
        /// Апи с доступным функционалом (Ядро)
        /// </summary>
        private API_server Api;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        public App()
        {
            Console.WriteLine("Ok!");
            Console.WriteLine("Введите URL сервера (http://localhost:5000 - по умолчанию)");

            string? server_url = Console.ReadLine();
            
            if (server_url == null || server_url.Length == 0)
            {
                server_url = DEFAULT_SERVER_URL;
            }
            Api = new API_server(new Uri(server_url)); //New

        }
        public void Start()
        {
            try
            {
                MenuStart();
            }
            catch (Exception exp)
            {
                Console.WriteLine("Ошибка: " + exp);
            }
        }
        /// <summary>
        /// Стартовое меню клиент-серверного приложения
        /// </summary>
        public void MenuStart()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("              МЕНЮ ВХОДА              ");
            Console.WriteLine("======================================");
            Console.WriteLine("   1. Авторизоваться                  ");
            Console.WriteLine("   2. Зарегистрироваться              ");
            Console.WriteLine("   0. Выход                           ");
            Console.WriteLine("======================================");

            string? menuStart_value = Console.ReadLine();

            if (menuStart_value == "1")
            {
                MenuAuth();
            }
            else if (menuStart_value == "2")
            {
                MenuSignUp();
            }
            else if (menuStart_value == "0")
            {
                return;
            }
            else
            {
                Console.WriteLine("Ошибка: Данного пункта меню не существует. \nСделайте выбор снова");
                MenuStart();
            }
        }

        /// <summary>
        /// Меню авторизации
        /// </summary>
        public void MenuAuth()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("            Авторизация...            ");
            Console.WriteLine("======================================");
            Console.Write("   Введите логин: ");
            string? username = Console.ReadLine();
            Console.Write("   Введите пароль: ");
            string? password = Console.ReadLine();
            Console.WriteLine("======================================");

            var r = Api.LoginOnServer(username!, password!);
            if (!r.Successful)
            {
                Console.WriteLine($"Ошибка: {r.ErrorData!.Message}");
                MenuStart();
                return;
            }

            Menu(); //Переход из стартового меню в меню массивов
        }

        /// <summary>
        /// Меню регистрации
        /// </summary>
        public void MenuSignUp()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("            Регистрация...            ");
            Console.WriteLine("======================================");
            Console.Write("   Введите логин: ");
            string? username = Console.ReadLine();
            Console.Write("   Введите пароль: ");
            string? password = Console.ReadLine();
            Console.WriteLine("======================================");

            var res = Api.SingUpOnServer(username!, password!);

            Console.WriteLine("Генерация массива выполнена: " + (res.Successful? "Успешно" : "Неверно"));
            Console.WriteLine(res.Answer);
            MenuStart();
        }

        /// <summary>
        /// Глобальное меню
        /// </summary>
        public void Menu()
        {
            Console.WriteLine("======================================");
            Console.WriteLine("            Меню выбора...            ");
            Console.WriteLine("======================================");
            Console.WriteLine("   1. Просмотр массива                ");
            Console.WriteLine("   2. Генерация массива               ");
            Console.WriteLine("   3. Ручной ввод массива             ");
            Console.WriteLine("   4. Добавить элемент в начало       ");
            Console.WriteLine("   5. Добавить элемент в конец        ");
            Console.WriteLine("   6. Добавить элемент после индекса  ");
            Console.WriteLine("   7. Сорт. массив локально           ");
            Console.WriteLine("   8. Сорт. часть массива локально    ");
            Console.WriteLine("   9. Сорт. массив в базе данных      ");
            Console.WriteLine("   10. Удалить массив                 ");
            Console.WriteLine("   11. Просмотр истории запросов      ");
            Console.WriteLine("   12. Смена пароля                   ");
            Console.WriteLine("   0. Вернуться                       ");
            Console.WriteLine("======================================");

            string? menuGlobal_value = Console.ReadLine();

            if (menuGlobal_value == "1") //Просмотр массива
            {
                var res = Api.GiveArray();

                Console.WriteLine("Просмотр массива выполнен: " + (res.Successful? "Успешно" : $"C ошибкой \"{res.ErrorData!.Message}\""));
                Console.WriteLine(res.Answer);
                Menu();
                return;
            }
            else if (menuGlobal_value == "2") //Генерация массива
            {
                Console.Write("   Введите нижнюю границу выбора чисел: ");
                int low = int.Parse(Console.ReadLine()!);
                Console.Write("   Введите верхнюю границу выбора чисел: ");
                int up = int.Parse(Console.ReadLine()!);
                Console.Write("   Введите количество чисел: ");
                int count = int.Parse(Console.ReadLine()!);

                var res = Api.RandomGenerateArray(low, up, count);

                Console.WriteLine("Генерация массива выполнена: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "3") //Ручной ввод массива
            {
                Console.WriteLine("   Введите массив чисел отделяя их пробелом");
                Console.WriteLine("   Пример: 6 12 24 35 77");
                Console.WriteLine("   Неверный символ автоматически заменяется на 0");
                Console.Write("   Введите массив чисел: ");
                string start_array = Console.ReadLine()!;

                var res = Api.WriteGenerateArray(start_array);

                Console.WriteLine("Запись массива выполнена: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "4") //Добавить элемент в начало
            {
                Console.Write("   Введите число: ");
                int value = int.Parse(Console.ReadLine()!);

                var res = Api.AddValueStart(value);

                Console.WriteLine("Добавление числа выполнено: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "5") //Добавить элемент в конец
            {
                Console.Write("   Введите число: ");
                int value = int.Parse(Console.ReadLine()!);

                var res = Api.AddValueFinish(value);

                Console.WriteLine("Добавление числа выполнено: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "6") //Добавить элемент после индекса
            {
                Console.Write("   Введите число: ");
                int value = int.Parse(Console.ReadLine()!);

                var res = Api.AddValueFinish(value);

                Console.WriteLine("Добавление числа выполнено: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "7") //Сортировать массив локально
            {
                var res = Api.GiveCombSort();

                Console.WriteLine("Локальная сортировка массива выполнена: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "8") //Сортировать часть массива локально
            {
                Console.Write("   Введите первый индекс: ");
                int start_index = int.Parse(Console.ReadLine()!);
                Console.Write("   Введите второй индекс: ");
                int finish_index = int.Parse(Console.ReadLine()!);
                var res = Api.GiveCombSortIndex(start_index, finish_index);

                Console.WriteLine("Локальная сортировка массива выполнена: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "9") //Сортировать массив в базе данных
            {
                var res = Api.WriteCombSort();

                Console.WriteLine("Сортировка массива в базе данных выполнена: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "10") //Удалить массив
            {
                var res = Api.DelArray();

                Console.WriteLine("Удаление массива выполнено: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
                Menu();
            }
            else if (menuGlobal_value == "11") //Просмотр истории запросов
            {
                ; //Now
            }
            else if (menuGlobal_value == "12") //Смена пароля
            {
                Console.Write("   Введите новый пароль: ");
                string? new_password = Console.ReadLine();

                var res = Api.NewPassword(new_password!);

                Console.WriteLine("Смена пароля выполнена: " + (res.Successful? "Успешно" : "Неверно"));
                Console.WriteLine(res.Answer);
            }
            else if (menuGlobal_value == "0") //Вернуться
            {
                MenuStart();
            }
            else
            {
                Console.WriteLine("Ошибка: Данного пункта меню не существует. \nСделайте выбор снова");
                Menu();
            }
        }







        public void StartTest()
        {
            const string def_server = "http://localhost:5000";
            Api = new API_server(new Uri(def_server));


            const string username = "user222";
            const string password = "password222";

            var r = Api.LoginOnServer(username, password);
            if (!r.Successful)
            {
                Console.WriteLine($"Error: {r.ErrorData!.Message}");
                return;
            }
            Console.WriteLine("Мы зачикинились)\n");

            var res = Api.RandomGenerateArray(88, 99, 5);
            Console.WriteLine("Random generate arr operation is " + (res.Successful? "successful" : "error"));
            Console.WriteLine(res.Answer);
            Console.WriteLine("Мы зачикинились)\n");

            string text = "88 99 5 33 44";
            res = Api.WriteGenerateArray(text);
            Console.WriteLine("Write arr operation is " + (res.Successful ? "successful" : "error"));
            Console.WriteLine(res.Answer);
            Console.WriteLine("Мы зачикинились)\n");

            //const int len = 5;
            //var rand = $"{new Random().Next(0, 10^len)}".PadRight(len, '0');
            //string username_rand = "user_" + rand;
            //string password_rand = "pwd_" + rand;

            //r = api.SingUpOnServer(username, password);
            //if (!r.Successful)
            //{
            //    Console.WriteLine($"Error: {r.ErrorData!.Message}");
            //    return;
            //}
            //Console.WriteLine("Мы зачикинили нового юзера)");

        }



        /*
        static string GetRandomWoParams()
        {
            string request = "/random";
            var response = client.GetAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return "Данные не доступны";
            }
        }

        static string GetRandomWoJson(string _low, string _up)
        {
            string request = "/random_params_json";

            var json_data = new
            {
                low = int.Parse(_low),
                up = int.Parse(_up)
            };
            string jsonBody = JsonSerializer.Serialize(json_data);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = client.PostAsync(request, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return "Данные не доступны";
            }
        }
        

        */
    }
}