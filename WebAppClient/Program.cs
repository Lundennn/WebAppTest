using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using WebAppClient;


partial class Program
{
    static public void Main(string[] args)
    {
        Console.WriteLine("Ok!");
        StartTest();




        const string DEFAULT_SERVER_URL = "http://localhost:5000";
        Console.WriteLine("Введите URL сервера (http://localhost:5000 - по умолчанию)");

        string? server_url = Console.ReadLine();

        if (server_url == null || server_url.Length == 0)
        {
            server_url = DEFAULT_SERVER_URL;

        }

        try
        {
            Console.WriteLine("======================================");
            Console.WriteLine("              МЕНЮ ВХОДА              ");
            Console.WriteLine("======================================");
            Console.WriteLine("   1. Авторизоваться                  ");
            Console.WriteLine("   2. Зарегистрироваться              ");
            Console.WriteLine("   0. Выход                           ");
            Console.WriteLine("======================================");

            string? menu_login_value = Console.ReadLine();
            Console.Clear();
            if (menu_login_value == "1")
            {
                Console.WriteLine("======================================");
                Console.WriteLine("            Авторизация...            ");
                Console.WriteLine("======================================");
                Console.Write("   Введите логин: ");
                string? username = Console.ReadLine();
                Console.Write("   Введите пароль: ");
                string? password = Console.ReadLine();
                Console.WriteLine("======================================");

                /*
                if (!LoginOnServer(null!, username, password))
                {
                    Console.WriteLine("Не верный логин/пароль, попробуйте снова...");
                    return;
                }
                */

                //Console.WriteLine(GetRandomWoParams());

                string? low = Console.ReadLine();
                string? up = Console.ReadLine();
                //Console.WriteLine(GetRandomWoJson(low, up));
            }
            if (menu_login_value == "2") { }
            if (menu_login_value == "0")
            {
                return;
            }
        }
        catch (Exception exp)
        {
            Console.WriteLine("Ошибка: " + exp);
        }

    }







    static void StartTest()
    {
        const string def_server = "http://localhost:5000";
        var api = new API_server(new Uri(def_server));


        const string username = "user222";
        const string password = "password222";

        var r = api.LoginOnServer(username, password);
        if (!r.Successful)
        {
            Console.WriteLine($"Error: {r.ErrorData!.Message}");
            return;
        }
        Console.WriteLine("Мы зачикинились)\n");

        var res = api.RandomGenerateArray(88, 99, 5);
        Console.WriteLine("Random generate arr operation is " + (res.Successful? "successful" : "error"));
        Console.WriteLine(res.Answer);
        Console.WriteLine("Мы зачикинились)\n");

        string text = "88 99 5 33 44";
        res = api.WriteGenerateArray(text);
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
