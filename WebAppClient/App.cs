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
            Console.WriteLine($"Введите URL сервера ({DEFAULT_SERVER_URL} - по умолчанию)");

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

            var r = Api.SingUpOnServer(username!, password!);

            if (!r.Successful)
            {
                Console.WriteLine($"Ошибка: {r.ErrorData!.Message}");
                MenuStart();
                return;
            }
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
            Console.WriteLine("   12. Удалить историю запросов       ");
            Console.WriteLine("   13. Смена пароля                   ");
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
                try 
                {
                    Console.Write("   Введите нижнюю границу выбора чисел: ");
                    int low = int.Parse(Console.ReadLine()!);
                    Console.Write("   Введите верхнюю границу выбора чисел: ");
                    int up = int.Parse(Console.ReadLine()!);
                    Console.Write("   Введите количество чисел: ");
                    int count = int.Parse(Console.ReadLine()!);

                    var res = Api.RandomGenerateArray(low, up, count);

                    Console.WriteLine("Генерация массива выполнена: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "3") //Ручной ввод массива
            {
                try 
                {
                    Console.WriteLine("   Введите массив чисел отделяя их пробелом");
                    Console.WriteLine("   Пример: 6 12 24 35 77");
                    Console.WriteLine("   Неверный символ автоматически заменяется на 0");
                    Console.Write("   Введите массив чисел: ");
                    string start_array = Console.ReadLine()!;

                    var res = Api.WriteGenerateArray(start_array);

                    Console.WriteLine("Запись массива выполнена: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "4") //Добавить элемент в начало
            {
                try 
                {
                    Console.Write("   Введите число: ");
                    int value = int.Parse(Console.ReadLine()!);

                    var res = Api.AddValueStart(value);

                    Console.WriteLine("Добавление числа выполнено: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "5") //Добавить элемент в конец
            {
                try 
                {
                    Console.Write("   Введите число: ");
                    int value = int.Parse(Console.ReadLine()!);

                    var res = Api.AddValueFinish(value);

                    Console.WriteLine("Добавление числа выполнено: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "6") //Добавить элемент после индекса
            {
                try 
                {
                    Console.Write("   Введите число: ");
                    int value = int.Parse(Console.ReadLine()!);
                    Console.Write("   Введите индекс: ");
                    int index = int.Parse(Console.ReadLine()!);

                    var res = Api.AddValueIndex(value, index);

                    Console.WriteLine("Добавление числа выполнено: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "7") //Сортировать массив локально
            {
                try 
                {
                    var res = Api.GiveCombSort();

                    Console.WriteLine("Локальная сортировка массива выполнена: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "8") //Сортировать часть массива локально
            {
                try 
                {
                    Console.Write("   Введите первый индекс: ");
                    int start_index = int.Parse(Console.ReadLine()!);
                    Console.Write("   Введите второй индекс: ");
                    int finish_index = int.Parse(Console.ReadLine()!);
                    var res = Api.GiveCombSortIndex(start_index, finish_index);

                    Console.WriteLine("Локальная сортировка массива выполнена: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "9") //Сортировать массив в базе данных
            {
                try 
                {
                    var res = Api.WriteCombSort();

                    Console.WriteLine("Сортировка массива в базе данных выполнена: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "10") //Удалить массив
            {
                try 
                {
                    var res = Api.DelArray();

                    Console.WriteLine("Удаление массива выполнено: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "11") //Просмотр истории запросов
            {
                try 
                {
                    var res = Api.GetHistory();
                    
                    Console.WriteLine("Просмотр истории выполнен: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "12") //Удалить историю запросов
            {
                try 
                {
                    var res = Api.DelHistory();

                    Console.WriteLine("Удаление истории выполнено: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "13") //Смена пароля
            {
                try 
                {
                    Console.Write("   Введите новый пароль: ");
                    string? new_password = Console.ReadLine();

                    var res = Api.NewPassword(new_password!);

                    Console.WriteLine("Смена пароля выполнена: " + CheckErr(res));
                    Console.WriteLine(res.Answer);
                    Menu();
                    return;
                }
                catch (Exception exp)
                {
                    Console.WriteLine($"Ошибка: {exp}");
                    Menu();
                    return;
                }
            }
            else if (menuGlobal_value == "0") //Вернуться
            {
                MenuStart();
                return;
            }
            else
            {
                Console.WriteLine("Ошибка: Данного пункта меню не существует. \nСделайте выбор снова");
                Menu();
                return;
            }
            string CheckErr<T>(AnswerServer<T> res)
                =>res.Successful? "Успешно" : $"C ошибкой \"{res.ErrorData!.Message}\"";
        }
    }
}